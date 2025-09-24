using System.Collections;
//using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorArmas : MonoBehaviour
{
    [SerializeField] Scr_CreadorArmas[] TodasLasArmas;
    [SerializeField] GameObject ObjetoArmas;
    [SerializeField] GameObject Mira;
    [SerializeField] Sprite[] Mirillas;
    [SerializeField] Color[] ColoresMirillas;
    [SerializeField] float LongitudRaycast;
    [SerializeField] Vector3 origenRaycast;
    [SerializeField] bool mostrarRaycast = false;

    [SerializeField] GameObject[] ObjetoArmas_reales; //prueba para activar la arma menos la de puños
    [SerializeField] GameObject[] balaPrefab; //bala que dispara
    [SerializeField] public GameObject[] GranadaPrefab; //Granada que dispara
    [SerializeField] AudioSource source;
    public GameObject BalaADisparar; //que va a disparar
    public Transform puntoDisparo; //lugar donde sale la bala
    public Camera camera;
    public float fuerzaDisparo = 70f;
    public int ArmaActual = 0;
    public GameObject[] contador;//es para mostar las balas o si es infinito
    private TextMeshProUGUI contadorbalas;

    bool Atacando = false;  // Evita que se interrumpa la animación actual
    public int CantBalasActual = 0;
    public int balascargador = 0;
    public int cantidadPerdigones = 8;//si es ecopeta
    public float dispersion = 25f; // en grados de dispersion
    public float cadencia = 0;
    public float temporizadorDisparo = 0f;
    public bool hizoHit = false; //detecta si golpeo algo
    private bool yasonohit = false;

    public Animator Anim;

    public bool havecombo=false;
    private float tiempoDesdeUltimoGolpe = 0f;
    private int numGolpe = 1;

    public bool empuje=false;
    public bool sangria=false;

    public int Maspenetracion=0;

    public float MasDistancia = 0f;

    public float MenosCadencia = 1;

    public bool minLimit=false;

    public string efecto = "";

    GameObject Gata;
    void Start()
    {
        //aplica el volumen 
        int volumen_general = PlayerPrefs.GetInt("Volumen", 50);
        int volumen_ambiental = PlayerPrefs.GetInt("Volumen_Combate", 20);
        float volumen = (volumen_general * volumen_ambiental) / 100;

        //Debug.LogError(PlayerPrefs.GetInt("Volumen", 50) + "//" + PlayerPrefs.GetInt("Volumen_Combate", 20) );
        //Debug.LogError(volumen + "//"+ volumen_general +"//" + volumen_ambiental);
        source.volume = volumen;
        if (ObjetoArmas == null) return;
        //tenia el armaActual pero por ahora es 0
        Transform ArmaAct = ObjetoArmas.transform.GetChild(0);

        if (ArmaAct == null) return;

        Anim = ArmaAct.GetComponent<Animator>();
        if (Anim == null) return;

        if (TodasLasArmas[ArmaActual].Tipo == "Pistola")
        {
            Anim.SetBool("EsPistola", true);
        }
        else if (TodasLasArmas[ArmaActual].Tipo == "Escopeta")
        {
            Anim.SetBool("EsPistola", true);
        }
        Gata = GameObject.Find("Personaje");
        CantBalasActual = TodasLasArmas[ArmaActual].Capacidad;
        balascargador = TodasLasArmas[ArmaActual].CapacidadTotal;
        Physics.IgnoreLayerCollision(7, 8);


    }

    //activa el arma pero si es cuerpo a cuerpo no la toma en cuenta
    void OnEnable()
    {
        Mira.SetActive(true);
        for (int i = 0; i < ObjetoArmas_reales.Length; i++)
        {
            ObjetoArmas_reales[i].SetActive(false);
        }
        cadencia = TodasLasArmas[ArmaActual].Cadencia;
        temporizadorDisparo = cadencia;
        if (ArmaActual != 0)
        {
            ObjetoArmas_reales[ArmaActual - 1].SetActive(true);
            puntoDisparo = ObjetoArmas_reales[ArmaActual - 1].GetComponentInChildren<Transform>();
        }
        if (TodasLasArmas[ArmaActual].Tipo != "Cuerpo a Cuerpo")
        {
            Mira.SetActive(true);
            Debug.Log("aaa");
            contador[0].SetActive(false);
            contador[1].SetActive(true);
            contadorbalas = contador[1].GetComponent<TextMeshProUGUI>();
            contadorbalas.text = CantBalasActual + "/" + balascargador;
        }
        else
        {
            contador[0].SetActive(true);
            contador[1].SetActive(false);
        }
        //BalaADisparar = balaPrefab[ArmaActual - 1];
        ChecarTemporal();
    }


    void Update()
    {
        if (TodasLasArmas[ArmaActual].Tipo != "Cuerpo a Cuerpo")
        {
            contadorbalas.text = CantBalasActual + "/" + balascargador;
        }
        //Debug.Log(temporizadorDisparo+ "?"+cadencia+ " +++++++++++++++" + (temporizadorDisparo >= cadencia));
        if (cadencia > 0)
        {
            temporizadorDisparo += Time.deltaTime;
        }
        // Contador de tiempo para resetear el combo
        if (tiempoDesdeUltimoGolpe > 0)
        {
            tiempoDesdeUltimoGolpe -= Time.deltaTime;
            if (tiempoDesdeUltimoGolpe <= 0)
            {
                numGolpe = 1; // Si pasan más de 5s sin atacar, vuelve a Golpe 1
            }
        }
        // Solo permite atacar si no está atacando y puede disparar
        if (Input.GetKey(KeyCode.Mouse0) && !Atacando && PuedeDisparar() && temporizadorDisparo >= (cadencia * MenosCadencia))
        {
            Disparar();
        }
        //prueba de recarga
        if (Input.GetKey(KeyCode.R))
        {
            RecargarBala();
        }

    }

    private void LateUpdate()
    {
        DetectarEnemigoConRaycast();
    }

    bool PuedeDisparar()
    {
        return CantBalasActual > 0 || TodasLasArmas[ArmaActual].Tipo == "Cuerpo a Cuerpo";
    }

    void Disparar()
    {

        Debug.LogWarning(TodasLasArmas[ArmaActual].Tipo);
        if (TodasLasArmas[ArmaActual].Tipo == "Cuerpo a Cuerpo")
        {
            //source.PlayOneShot(TodasLasArmas[ArmaActual].Sonidos[0]);
            EjecutarAtaque(Anim);
        }
        else if (TodasLasArmas[ArmaActual].Tipo == "Escopeta")
        {

            source.PlayOneShot(TodasLasArmas[ArmaActual].Sonidos[0]);
            DispararEscopeta();
        }
        else
        {

            source.PlayOneShot(TodasLasArmas[ArmaActual].Sonidos[0]);
            DisparaBala();
        }
    }
    //cuando sea cuerpo a cuerto
    void EjecutarAtaque(Animator Anim)
    {
        // Bloquea el ataque hasta que termine la animación
        Atacando = true;

        // Selecciona la animación según el golpe actual
        string animacion = "Golpe " + numGolpe;
        Anim.Play(animacion);

        // Obtiene la duración del golpe y espera antes de permitir otro ataque
        float duracion = GetAnimationClipDuration(Anim, animacion);
        StartCoroutine(EsperarAtaque(duracion));
        StartCoroutine(EsperarHit(duracion*.48f));
        // Avanza en la secuencia de golpes
        numGolpe = (numGolpe % 3) + 1;

        // Reinicia el contador de tiempo
        tiempoDesdeUltimoGolpe = 5f;
    }

    void DisparaBala()
    {
        Anim.Play("Disparo_pistola");
        temporizadorDisparo = 0;
        Atacando = true;
        GameObject bala = Instantiate(BalaADisparar, puntoDisparo.position, puntoDisparo.rotation);
        float masdaño = 0;
        if (minLimit && CantBalasActual<2)
        {
            masdaño = TodasLasArmas[ArmaActual].Daño + 2;
        }
        if(efecto == "Rebotar")
        {
            bala.GetComponent<Balas>().Rebota = true;
        }
        bala.GetComponent<Balas>().daño = TodasLasArmas[ArmaActual].Daño + masdaño;
        bala.GetComponent<Balas>().penetracion += Maspenetracion;
        // Calcular dirección del disparo
        Vector3 direccionDisparo;

        // Lanzamos un raycast desde el centro de la cámara hacia adelante
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // Si choca algo, disparamos hacia ese punto
            direccionDisparo = (hit.point - puntoDisparo.position).normalized;
        }
        else
        {
            // Si no choca nada, disparamos hacia adelante de la cámara
            direccionDisparo = ray.direction;
        }

        // Aplicamos fuerza a la bala
        Rigidbody rb = bala.GetComponent<Rigidbody>();
        rb.AddForce(direccionDisparo * fuerzaDisparo, ForceMode.Impulse);
        if (efecto == "Fantasma")
        {
            int checar = Random.Range(0,100);
            if (checar < 60)
            {
                CantBalasActual--;
            }
        }
        else
        {
            CantBalasActual--;
        }
        //Anim.SetBool("EstaDisparando", false);
        StartCoroutine(EsperarAtaque(TodasLasArmas[ArmaActual].Cadencia));
    }

    void DispararEscopeta()
    {
        temporizadorDisparo = 0;
        Atacando = true;
        for (int i = 0; i < cantidadPerdigones; i++)
        {
            GameObject bala = Instantiate(BalaADisparar, puntoDisparo.position, puntoDisparo.rotation);
            float masdaño = 0;
            if (minLimit && CantBalasActual < 2)
            {
                masdaño = TodasLasArmas[ArmaActual].Daño + 2;
            }
            if (efecto == "Rebotar")
            {
                bala.GetComponent<Balas>().Rebota = true;
            }
            bala.GetComponent<Balas>().daño = TodasLasArmas[ArmaActual].Daño + masdaño;
            bala.GetComponent<Balas>().penetracion = 2+Maspenetracion;
            // Direccion base
            Vector3 direccionBase = camera.transform.forward;

            // Añadir dispersión aleatoria
            Vector3 direccionConDispersion = DireccionConDispersion(direccionBase, Random.Range(0, dispersion));

            // Aplicar fuerza
            Rigidbody rb = bala.GetComponent<Rigidbody>();
            rb.AddForce(direccionConDispersion * fuerzaDisparo, ForceMode.Impulse);
        }
        if (efecto == "Fantasma")
        {
            int checar = Random.Range(0, 100);
            if (checar < 60)
            {
                CantBalasActual--;
            }
        }
        else
        {
            CantBalasActual--;
        }
        StartCoroutine(EsperarAtaque(TodasLasArmas[ArmaActual].Cadencia));
    }

    IEnumerator EsperarAtaque(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        Atacando = false;  // Ahora puede volver a atacar
    }
    IEnumerator EsperarHit(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        golpe();
        hizoHit = false;
        yasonohit = false;
    }

    public void golpe()
    {
        if (yasonohit) return;
        if (hizoHit)
        {
            //Debug.LogError("Golpe");
            source.PlayOneShot(TodasLasArmas[ArmaActual].Sonidos[0]); // Golpe con impacto
            yasonohit = true;
        }
        else
        {
            //Debug.LogError("NO Golpe");
            source.PlayOneShot(TodasLasArmas[ArmaActual].Sonidos[1]); // Golpe sin impacto
            yasonohit = true;
        }
    }

    Vector3 DireccionConDispersion(Vector3 direccion, float dispersionEnGrados)
    {
        // Convertir grados a radianes
        float maxAngle = dispersionEnGrados / 2f;

        // Generar un pequeño desvío aleatorio
        float angleX = Random.Range(-maxAngle, maxAngle);
        float angleY = Random.Range(-maxAngle, maxAngle);

        // Aplicar rotación a la dirección
        Quaternion rot = Quaternion.Euler(angleX, angleY, 0);
        return rot * direccion;
    }
    void RecargarBala()
    {
        if (TodasLasArmas[ArmaActual].Tipo == "Cuerpo a Cuerpo") return;
        if (CantBalasActual != TodasLasArmas[ArmaActual].Capacidad && balascargador > 0)
        {
            int cantidadarestar = TodasLasArmas[ArmaActual].Capacidad - CantBalasActual;
            if (balascargador >= cantidadarestar)
            {
                Anim.Play("PistolaRecarga");
                balascargador = balascargador - cantidadarestar;
                CantBalasActual = TodasLasArmas[ArmaActual].Capacidad;
                source.PlayOneShot(TodasLasArmas[ArmaActual].Recarga);
                //Anim.SetBool("EstaRecargando", false);
            }
        }
    }
    float GetAnimationClipDuration(Animator animator, string clipName)
    {
        if (animator == null) return 0f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) return clip.length;
        }
        return 0f;
    }

    void DetectarEnemigoConRaycast()
    {
        if (Gata == null) return;

        // Posición del raycast ajustando altura
        Vector3 origen = Gata.transform.position + new Vector3(0, origenRaycast.y, 0);
        Vector3 direccion = camera.transform.forward;

        Ray rayo = new Ray(origen, direccion);
        RaycastHit hit;

        if (mostrarRaycast)
        {
            Debug.DrawRay(origen, direccion * LongitudRaycast, Color.red);
        }

        if (Physics.Raycast(rayo, out hit, LongitudRaycast))
        {
            if (hit.collider.CompareTag("Enemigo"))
            {
                Mira.GetComponent<Image>().color = ColoresMirillas[1];
                Mira.GetComponent<Image>().sprite = Mirillas[1];
                return;
            }
        }

        // Si no golpeó enemigo o no golpeó nada
        Mira.GetComponent<Image>().color = ColoresMirillas[0];
        Mira.GetComponent<Image>().sprite = Mirillas[0];
    }
    
    private void ChecarTemporal()
    {
        switch (GetComponent<Scr_ControladorBatalla>().HabilidadT)
        {
            case "Nada":
                efecto = "";
                break;

            case "Aturdimiento":
                efecto = "Stunear";
                break;

            case "Bala Incendiaria":
                efecto = "Quemar";
                break;

            case "Boca Venenosa":
                efecto = "Veneno";
                break;

            case "Cañon Congelante":
                efecto = "Congelar";
                break;

            case "Disparo rebote":
                efecto = "Rebotar";
                break;

            case "Empuje":
                efecto = "Empujar";
                break;

            case "Golpe con mas area":
                MasDistancia = 15;
                break;

            case "Golpe de fuego":
                efecto = "Quemar";
                break;

            case "Puño Relampago":
                efecto = "Electrificar";
                break;

            case "Puño Veneno":
                efecto = "Veneno";
                break;

            case "Raiz Atadora":
                efecto = "Stunear";
                break;

            case "Rebote":
                efecto = "Rebotar";
                break;

            case "Tiro Explosivo":
                efecto = "Explotar";
                break;

            case "Tiro fantasma":
                efecto = "Fantasma";
                break;

            case "Tiro Paralizante":
                efecto = "Electrificar";
                break;

            case "Velocidad mordida":
                MenosCadencia = .6f;
                break;
        }
    }
}
