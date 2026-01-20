using System.Collections;
using System.Runtime.InteropServices;

//using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Fruta_drop;

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
    [SerializeField] GameObject[] Objeto_Izquierdo; //prueba para activar la arma menos la de puños
    [SerializeField] GameObject[] balaPrefab; //bala que dispara
    [SerializeField] public GameObject[] GranadaPrefab; //Granada que dispara
    [SerializeField] AudioSource source;
    [SerializeField] GameObject[] FrutaMano;
    [SerializeField] GameObject[] FrutaDrop;
    public GameObject BalaADisparar; //que va a disparar
    public Transform puntoDisparo; //lugar donde sale la bala
    public Camera camara;
    public float fuerzaDisparo = 70f;
    public int ArmaActual = 0;
    public GameObject[] contador;//es para mostar las balas o si es infinito
    private TextMeshProUGUI contadorbalas;


    public Transform PuntodeArma;//pundo para daño melee
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

    public Animator AnimArma;

    public bool havecombo = false;
    private float tiempoDesdeUltimoGolpe = 0f;
    private int numGolpe = 1;

    public bool empuje = false;
    public bool sangria = false;
    public bool sangriaEspera = false;

    public int Maspenetracion = 0;

    public float daño = 0;

    public float MasDistancia = 0f;

    public float MenosCadencia = 1;

    public bool minLimit = false;
    
    public string Tipo = "";
    public float alcance;
    public float area;
    public float TamaProyectil=1;

    public bool ModoAutomático = false;
    GameObject Gata;
    private bool Manteniendo;
    private bool isGrappling = false;
    private Vector3 grapplePoint;
    private float currentDistance;
    public LineRenderer lineRenderer;

    GameObject Fruta;
    string FrutaNombre;
    bool TieneFruta;
    public Transform puntoFruta; //lugar donde sale la bala
    [Header("Efectos")]
    public string EfectoTemp = "";
    public string EfectoHab = "";

    [Header("Fruta")]
    public float vidaTotal = 10f;
    public float tiempoPudrirse = 6f;
    public float tiempoPodrido = 9f;

    public Color colorNormal = Color.white;
    public Color colorPudriendose = Color.yellow;
    public Color colorPodrido = Color.green;

    private float tiempoActualFruta = 0f;
    private Fruta_drop.EstadoFruta estadoActual;
    private Renderer rendererFruta;

    PlayerInput playerInput;
    InputIconProvider IconProvider;
    private InputAction Dispara;
    private InputAction Recargar;
    private InputAction Recolectar;
    void Start()
    {
        //aplica el volumen 
        int volumen_general = PlayerPrefs.GetInt("Volumen", 50);
        int volumen_ambiental = PlayerPrefs.GetInt("Volumen_Combate", 20);
        float volumen = (volumen_general * volumen_ambiental) / 100;
        camara = Camera.main;

        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Dispara = playerInput.actions["Disparar"];
        Recargar = playerInput.actions["Recargar"];
        Recolectar = playerInput.actions["Recolectar"];


        source.volume = volumen;
        if (ObjetoArmas == null) return;
        //tenia el armaActual pero por ahora es 0
        Transform ArmaAct = ObjetoArmas.transform.GetChild(0);
        GameObject Hijo = ObjetoArmas_reales[ArmaActual];
        if (ArmaAct != null)
        {
            Anim = ArmaAct.GetComponent<Animator>();
        }
        if (Anim == null) return;
        if (Hijo.transform.GetChild(0) != null)
        {
            PuntodeArma = Hijo.transform.GetChild(0);
        }
        ChecarTemporal();
        if (EfectoTemp == "" && TodasLasArmas[ArmaActual].Nombre == "Chilenon")
        {
            EfectoHab = "Quemar";
        }
        Tipo = TodasLasArmas[ArmaActual].Tipo;
        Gata = GameObject.Find("Personaje");
        CantBalasActual = TodasLasArmas[ArmaActual].Capacidad;
        balascargador = TodasLasArmas[ArmaActual].CapacidadTotal;
        alcance = TodasLasArmas[ArmaActual].Alcance;
        Physics.IgnoreLayerCollision(7, 8);
        checarIdle();
        //"Rango" + Nombre del arma
        //PlayerPrefs.GetInt("Rango " + Datosarmas.TodasLasArmas[objShow].Nombre, 1)-1
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
            ObjetoArmas_reales[ArmaActual].SetActive(true);
            puntoDisparo = ObjetoArmas_reales[ArmaActual - 1].GetComponentInChildren<Transform>();
            AnimArma = ObjetoArmas_reales[ArmaActual].GetComponent<Animator>();
        }
        if (TodasLasArmas[ArmaActual].Tipo != "Cuerpo a Cuerpo")
        {
            Mira.SetActive(true);
            Debug.Log("aaa");
            contador[0].SetActive(false);
            contador[1].SetActive(true);
            contadorbalas = contador[1].GetComponent<TextMeshProUGUI>();
            contadorbalas.text = CantBalasActual + "/" + balascargador;

            BalaADisparar = balaPrefab[ArmaActual];
        }
        else
        {
            contador[0].SetActive(true);
            contador[1].SetActive(false);
        }
        daño = TodasLasArmas[ArmaActual].Daño;
        Tipo = TodasLasArmas[ArmaActual].Tipo;

        //checarIdle();
        ChecarTemporal();
        GetComponent<Scr_ControladorBatalla>().ChecarRango(TodasLasArmas[ArmaActual].Nombre);
    }

    void Update()
    {
        if (isGrappling)
        {
            GanchoMover();
        }
        if (Tipo != "Cuerpo a Cuerpo")
        {
            contadorbalas.text = CantBalasActual + ""; //+ "/" + balascargador;
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


        RecolectarFruta();
        checar_estadoFruta();
        // Disparo basado en el modo seleccionado (automático o semiautomático)
        if (Tipo != "Automatica")
        {
            // Modo automático: Dispara continuamente mientras se mantiene presionado el botón del ratón
            if (Dispara.IsPressed() && !Atacando && PuedeDisparar() && temporizadorDisparo >= (cadencia * MenosCadencia))
            {
                Disparar();
                temporizadorDisparo = 0f; // Reinicia el temporizador después de disparar
            }
        }
        else
        {
            // Modo semiautomático: Dispara solo cuando se presiona el botón
            if (Dispara.IsPressed())
            {
                Manteniendo = true;
            }
            if (!Dispara.IsPressed())
            {
                Manteniendo = false;
            }
            if (Manteniendo)
            {
                rotarUva();
                if (!Atacando && PuedeDisparar() && temporizadorDisparo >= (cadencia * MenosCadencia))
                {
                    Disparar();
                    temporizadorDisparo = 0f; // Reinicia el temporizador después de disparar

                }
            }
        }
        //prueba de recarga
        if (Recargar.IsPressed())
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
        //Debug.LogWarning(TodasLasArmas[ArmaActual].Tipo);
        if (Tipo == "Cuerpo a Cuerpo")
        {
            EjecutarAtaque(Anim);
        }
        else if (Tipo == "Escopeta")
        {
            StartCoroutine(GolpePaticula(puntoDisparo));
            source.PlayOneShot(TodasLasArmas[ArmaActual].Sonidos[0]);
            DispararEscopeta();
        }
        else if (Tipo == "Lanzallamas")
        {
            source.PlayOneShot(TodasLasArmas[ArmaActual].Sonidos[0]);
            Lazer();
        }
        else
        {
            StartCoroutine(GolpePaticula(puntoDisparo));
            source.PlayOneShot(TodasLasArmas[ArmaActual].Sonidos[0]);
            DisparaBala();
        }
    }
    IEnumerator GolpePaticula(Transform trans)
    {
        // Muestra el efecto
        GameObject explosion = Instantiate(TodasLasArmas[ArmaActual].particula, trans.position, trans.rotation);
        explosion.transform.SetParent(transform);
        Debug.Log("Descarga eléctrica!");
        yield return new WaitForSeconds(1f);
        Destroy(explosion);
    }
    //cuando sea cuerpo a cuerto
    void EjecutarAtaque(Animator Anim)
    {
        // Bloquea el ataque hasta que termine la animación
        Atacando = true;

        // Selecciona la animación según el golpe actual
        string animacion = checaranimacionGolpe();
        //Debug.Log(animacion);
        if (animacion != "")
        {
            Anim.Play(animacion);
        }
        // Obtiene la duración del golpe y espera antes de permitir otro ataque
        float duracion = GetAnimationClipDuration(Anim, animacion);
        StartCoroutine(EsperarAtaque(duracion));
        StartCoroutine(EsperarHit(duracion * .48f));
        // Avanza en la secuencia de golpes
        numGolpe = (numGolpe % 3) + 1;

        // Reinicia el contador de tiempo
        tiempoDesdeUltimoGolpe = 5f;
    }

    public void Golpea()
    {
        // rango es currentWeapon.range, attackOrigin es el punto del jugador (ej. frente)
        Vector3 center = PuntodeArma != null ? PuntodeArma.position : transform.position;
        float radius = alcance + MasDistancia;

        Collider[] colliders = Physics.OverlapSphere(center, radius);

        foreach (Collider col in colliders)
        {
            Scr_Enemigo ene = col.GetComponent<Scr_Enemigo>();
            if (ene != null)
            {
                ene.RecibirDaño(daño, Color.red);
                ene.realizardaño(daño, EfectoTemp,EfectoHab);
            }
        }
        // Debug
        //Debug.DrawRay(center, transform.forward * radius, Color.red, 0.5f);
    }
    public void GolpeAdelante()
    {
        // rango es currentWeapon.range, attackOrigin es el punto del jugador (ej. frente)
        Vector3 center = PuntodeArma != null ? PuntodeArma.position : transform.position;
        float radius = alcance * 10;
        Collider[] colliders = Physics.OverlapSphere(center, radius);

        foreach (Collider col in colliders)
        {
            Scr_Enemigo ene = col.GetComponent<Scr_Enemigo>();
            if (ene != null)
            {
                ene.RecibirDaño(daño, Color.red);
                ene.realizardaño(daño, EfectoTemp, EfectoHab);
            }
        }
        // Debug
        //Debug.DrawRay(center, transform.forward * radius, Color.red, 0.5f);
    }
    public void DisparaBala()
    {
        if (TieneFruta) return;
        if (TodasLasArmas[ArmaActual].Nombre == "Planta")
        {
            AnimArma.Play("Morder_Planta");
        }
        else if (TodasLasArmas[ArmaActual].Nombre != "Papa")
        {
            Anim.Play(checaranimacionDisparo());
        }
        temporizadorDisparo = 0;
        Atacando = true;
        GameObject bala = Instantiate(BalaADisparar, puntoDisparo.position, puntoDisparo.rotation);
        bala.transform.localScale = bala.transform.localScale * TamaProyectil;
        float masdaño = 0;
        if (minLimit && CantBalasActual < 2)
        {
            masdaño = daño + 2;
        }
        if (EfectoTemp == "Rebotar")
        {
            bala.GetComponent<Balas>().Rebota = true;
        }
        bala.GetComponent<Balas>().daño = daño + masdaño;
        bala.GetComponent<Balas>().penetracion += Maspenetracion;
        bala.GetComponent<Balas>().MasArea += MasDistancia;
        // Calcular dirección del disparo
        Vector3 direccionDisparo;

        // Lanzamos un raycast desde el centro de la cámara hacia adelante
        Ray ray = camara.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
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
        if (EfectoTemp == "Fantasma")
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
        //Anim.SetBool("EstaDisparando", false);
        StartCoroutine(EsperarAtaque(cadencia));
    }

    public void DispararEscopeta()
    {
        if (TodasLasArmas[ArmaActual].Nombre != "Papa")
        {
            Anim.Play(checaranimacionDisparo());
        }
        temporizadorDisparo = 0;
        Atacando = true;
        float dañopoerdigon = daño / cantidadPerdigones;
        for (int i = 0; i < cantidadPerdigones; i++)
        {
            GameObject bala = Instantiate(BalaADisparar, puntoDisparo.position, puntoDisparo.rotation);
            bala.transform.localScale = bala.transform.localScale * TamaProyectil;
            float masdaño = 0;
            if (minLimit && CantBalasActual < 2)
            {
                masdaño = dañopoerdigon + 2;
            }
            if (EfectoTemp == "Rebotar")
            {
                bala.GetComponent<Balas>().Rebota = true;
            }
            bala.GetComponent<Balas>().daño = dañopoerdigon + masdaño;
            bala.GetComponent<Balas>().penetracion = 2 + Maspenetracion;
            bala.GetComponent<Balas>().MasArea += MasDistancia;
            // Direccion base
            Vector3 direccionBase = camara.transform.forward;

            // Añadir dispersión aleatoria
            Vector3 direccionConDispersion = DireccionConDispersion(direccionBase, Random.Range(0, dispersion));

            // Aplicar fuerza
            Rigidbody rb = bala.GetComponent<Rigidbody>();
            rb.AddForce(direccionConDispersion * fuerzaDisparo, ForceMode.Impulse);
        }
        if (EfectoTemp == "Fantasma")
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
        StartCoroutine(EsperarAtaque(cadencia));
    }
    public void Lazer()
    {
        Anim.Play(checaranimacionDisparo());
        // Definir el centro del láser (punto del arma o la posición del jugador)
        Vector3 center = PuntodeArma != null ? PuntodeArma.position : transform.position;

        // Calcular el radio del láser (rango del arma escalado)
        float radius = alcance * 10;

        // Definir la dirección del láser (hacia donde apunta el arma)
        Vector3 direction = camara.transform.forward;

        // Calcular el punto final de la cápsula (centro + rango en la dirección del frente)
        Vector3 capsuleEnd = center + direction * radius;

        // Usar Physics.OverlapCapsule para detectar colisiones dentro del área del láser
        Collider[] colliders = Physics.OverlapCapsule(center, capsuleEnd, 0.5f); // 0.5f es el radio de la cápsula

        foreach (Collider col in colliders)
        {
            Scr_Enemigo ene = col.GetComponent<Scr_Enemigo>();
            if (ene != null)
            {
                // Aplicar daño con efecto visual (color rojo)
                ene.RecibirDaño(daño, Color.red);

                // Aplicar efecto secundario (daño escalado)
                ene.realizardaño(daño, EfectoTemp, EfectoHab);
            }
        }
        if (EfectoTemp == "Fantasma")
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
        // Depuración: visualizar el rango del láser
        //Debug.DrawLine(center, capsuleEnd, Color.red, 0.5f);
    }
    public void EfectoHabilidad(GameObject EfectoHabilidad, float espera)
    {
        // rango es currentWeapon.range, attackOrigin es el punto del jugador (ej. frente)
        Vector3 center = PuntodeArma != null ? PuntodeArma.position : transform.position;
        float radius = alcance * 10;
        GameObject part = Instantiate(EfectoHabilidad, PuntodeArma.transform.position, EfectoHabilidad.transform.rotation);
        Destroy(part, espera);
    }

    public void DisparaGancho()
    {
        RaycastHit hit;
        if (Physics.Raycast(PuntodeArma.transform.position, PuntodeArma.transform.forward, out hit, 40))
        {
            if (hit.collider != null)
            {
                isGrappling = true;
                grapplePoint = hit.point; // Punto donde el gancho se engancha
                currentDistance = Vector3.Distance(PuntodeArma.position, grapplePoint);
                // Activar el LineRenderer para mostrar la cuerda
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, PuntodeArma.position);
                lineRenderer.SetPosition(1, grapplePoint);
                temporizadorDisparo = 0;
                AnimArma.SetBool("Mantiene", true);
            }
        }
    }
    void GanchoMover()
    {
        float step = 10f * Time.deltaTime;
        temporizadorDisparo += Time.deltaTime;
        Gata.transform.position = Vector3.MoveTowards(Gata.transform.position, grapplePoint, step);
        lineRenderer.SetPosition(0, PuntodeArma.position);
        lineRenderer.SetPosition(1, grapplePoint);
        if (Vector3.Distance(Gata.transform.position, grapplePoint) < 0.5f || temporizadorDisparo > 7f) // Cuando el jugador llega al punto de anclaje
        {
            AnimArma.SetBool("Mantiene", false);
            lineRenderer.enabled = false; // Desactivar la cuerda
            isGrappling = false;
        }
    }
    void RecargarBala()
    {
        if (Tipo == "Cuerpo a Cuerpo") return;
        if (CantBalasActual < balascargador)
        {
            Anim.Play(checaranimacionRecarga());
            //balascargador = balascargador - cantidadarestar;
            CantBalasActual = TodasLasArmas[ArmaActual].Capacidad;
            source.PlayOneShot(TodasLasArmas[ArmaActual].Recarga);
            //Anim.SetBool("EstaRecargando", false);
        }
        /*if (CantBalasActual != TodasLasArmas[ArmaActual].Capacidad && balascargador > 0)
        {
            int cantidadarestar = TodasLasArmas[ArmaActual].Capacidad - CantBalasActual;
           
        }*/
    }
    public void Lanzar()
    {
        BalaADisparar = GranadaPrefab[0];
        ObjetoArmas_reales[ArmaActual].SetActive(false);
        DisparaBala();
    }
    public void Recoger()
    {
        ObjetoArmas_reales[ArmaActual].SetActive(true);
    }


    //Rota la Uva
    public void rotarUva()
    {
        if (TodasLasArmas[ArmaActual].Nombre == "Uvalon")
        {
            GameObject rot = ObjetoArmas_reales[ArmaActual].transform.GetChild(1).gameObject;
            rot.transform.Rotate(0, 0, Time.deltaTime * 70);
            float rotz = rot.transform.rotation.z + Time.deltaTime;
            ///rot.transform.rotation = Quaternion.Euler(rot.transform.rotation.x, rot.transform.rotation.y, rotz);
        }
    }

    //
    // Checa que arma tiene y la agrega su estado de animacion
    //
    private void checarIdle()
    {
        Debug.Log(TodasLasArmas[ArmaActual].Nombre);
        switch (TodasLasArmas[ArmaActual].Nombre)
        {
            case "Platano":
                Anim.SetBool("EsPistola", true);
                break;
            case "Tomate":
                Anim.SetBool("EsTomate", true);
                break;
            case "Sandia":
                Anim.SetBool("EsSandia", true);
                break;
            case "Planta":
                Anim.SetBool("EsPlanta", true);
                break;
            case "Mango":
                Anim.SetBool("EsMango", true);
                Objeto_Izquierdo[0].SetActive(true);
                break;
            case "Papa":
                Anim.SetBool("EsPapa", true);
                break;
            case "Coco":
                Anim.SetBool("EsCoco", true);
                break;
            case "Chilenon":
                Anim.SetBool("EsChile", true);
                Objeto_Izquierdo[1].SetActive(true);
                break;
            case "Uvalon":
                Anim.SetBool("EsUva", true);
                break;
        }
    }

    //
    //Checa la animacion de golpe de las armas cuerpo a cuerpo
    //
    private string checaranimacionGolpe()
    {
        switch (TodasLasArmas[ArmaActual].Nombre)
        {
            case "Brazos":
                return "Golpe " + numGolpe;

            case "Sandia":
                return "SandiaGolpe";

            case "Planta":
                AnimArma.Play("Morder_Planta");
                return "";

            case "Mango":
                return "MangoGolpe";

            case "Papa":
                return "PapaLanzar";

            case "Coco":
                return "CocoGolpe";

            default:
                return "Golpe " + numGolpe;
        }
    }

    //
    // Checa la animacion de golpe de las armas a distancia
    //
    private string checaranimacionDisparo()
    {
        switch (TodasLasArmas[ArmaActual].Nombre)
        {
            case "Platano":
                return "Disparo_pistola";

            case "Tomate":
                return "Disparo_Tomate";

            case "Chilenon":
                return "ChileDisparo";

            case "Uvalon":
                return "UvaDisparo";
            default:
                return "Disparo_pistola";
        }
    }
    //
    // Checa la animacion de recarga de las armas
    //
    private string checaranimacionRecarga()
    {
        switch (TodasLasArmas[ArmaActual].Nombre)
        {
            case "Platano":
                return "PistolaRecarga";

            case "Tomate":
                return "TomateRecarga";

            case "Chilenon":
                return "ChileRecarga";

            case "Uvalon":
                return "UvaRecarga";
            default:
                return "PistolaRecarga";
        }
    }

    //Espera el ataque
    IEnumerator EsperarAtaque(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        Atacando = false;  // Ahora puede volver a atacar
    }
    //Espera el ataque
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

    float GetAnimationClipDuration(Animator animator, string clipName)
    {
        if (animator == null) return 0f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) return clip.length;
        }
        return 0f;
    }
    void RecolectarFruta()
    {
        if(Fruta!=null && Recolectar.IsPressed() && !TieneFruta)
        {
            TieneFruta = true;
            for (int i = 0; i < ObjetoArmas_reales.Length; i++)
            {
                ObjetoArmas_reales[i].SetActive(false);
            }
            Fruta_drop drop = Fruta.GetComponent<Fruta_drop>();
            estadoActual = drop.GetEstado();
            tiempoActualFruta = drop.tiempoActual;
            tiempoPodrido = drop.tiempoPodrido;
            tiempoPudrirse = drop.tiempoPudrirse;
            vidaTotal = drop.vidaTotal;
            FrutaNombre = drop.Nombre;
            colorNormal= drop.colorNormal;
            colorPodrido = drop.colorPodrido;
            colorPudriendose = drop.colorPudriendose;


            //Debug.Log(FrutaNombre);
            switch (FrutaNombre)
            {
                case "Baya Azul":
                    FrutaMano[0].SetActive(true);
                    rendererFruta= FrutaMano[0].GetComponent<Renderer>();
                    puntoFruta = FrutaMano[0].transform;
                    break;
                case "Baya Amarillo":
                    FrutaMano[1].SetActive(true);
                    rendererFruta = FrutaMano[1].GetComponent<Renderer>();
                    puntoFruta = FrutaMano[1].transform;
                    break;
                case "Baya Roja":
                    FrutaMano[2].SetActive(true);
                    rendererFruta = FrutaMano[2].GetComponent<Renderer>();
                    puntoFruta = FrutaMano[2].transform;
                    break;
                case "Platano":
                    FrutaMano[3].SetActive(true);
                    rendererFruta = FrutaMano[3].GetComponent<Renderer>();
                    puntoFruta = FrutaMano[3].transform;
                    break;
                default:
                    FrutaMano[0].SetActive(true);
                    rendererFruta = FrutaMano[0].GetComponent<Renderer>();
                    puntoFruta = FrutaMano[0].transform;
                    break;
            }
            //rendererFruta = Fruta.GetComponent<Renderer>();
            Anim.Play("FrutaAgarrada");
            Destroy(Fruta);
        }
        else if (/*Recolectar.IsPressed() ||*/ Dispara.IsPressed() && TieneFruta)
        {
            DestruirFruta();
        }
    }

    private void checar_estadoFruta()
    {
        if (!TieneFruta) return;
        // El tiempo SIEMPRE avanza
        tiempoActualFruta += Time.deltaTime;

        // Cambios de estado
        if (tiempoActualFruta >= tiempoPodrido && estadoActual != EstadoFruta.Podrido)
            CambiarEstadoFruta(EstadoFruta.Podrido);
        else if (tiempoActualFruta >= tiempoPudrirse && estadoActual != EstadoFruta.Pudriendose)
            CambiarEstadoFruta(EstadoFruta.Pudriendose);

        // Muerte
        if (tiempoActualFruta >= vidaTotal)
            DestruirFruta();
    }
    void CambiarEstadoFruta(EstadoFruta nuevoEstado)
    {
        estadoActual = nuevoEstado;

        switch (estadoActual)
        {
            case EstadoFruta.Normal:
                CambiarColor(colorNormal);
                break;

            case EstadoFruta.Pudriendose:
                CambiarColor(colorPudriendose);
                break;

            case EstadoFruta.Podrido:
                CambiarColor(colorPodrido);
                break;
        }
    }

    void CambiarColor(Color color)
    {
        if (rendererFruta == null) return;

        foreach (Material mat in rendererFruta.materials)
        {
            Material[] materiales = rendererFruta.materials;
            // Guardar una copia de los materiales originales
            Material[] materialesOriginales = new Material[materiales.Length];
            for (int i = 0; i < materiales.Length; i++)
            {
                materialesOriginales[i] = new Material(materiales[i]);
            }

            // Crear copias modificadas de los materiales y cambiar el _BaseColor
            Material[] materialesModificados = new Material[materiales.Length];
            for (int i = 0; i < materiales.Length; i++)
            {
                materialesModificados[i] = new Material(materiales[i]);
                materialesModificados[i].SetColor("_Base_Color", color); // Cambiar el color
            }

            // Aplicar materiales modificados
            rendererFruta.materials = materialesModificados;
        }
    }

    void DestruirFruta()
    {
        DispararFruta();
        FrutaNombre = "";
        TieneFruta = false;
        esconderFruta();
    }
    void DispararFruta()
    {
        GameObject Frut= null;
        Anim.Play("Brazos_ NormalIddle");
        switch (FrutaNombre)
        {
            case "Baya Azul":
                Frut = Instantiate(FrutaDrop[0], puntoFruta.position, puntoFruta.rotation);
                break;
            case "Baya Amarillo":
                Frut = Instantiate(FrutaDrop[1], puntoFruta.position, puntoFruta.rotation);
                break;
            case "Baya Roja":
                Frut = Instantiate(FrutaDrop[2], puntoFruta.position, puntoFruta.rotation);
                break;
            case "Platano":
                Frut = Instantiate(FrutaDrop[3], puntoFruta.position, puntoFruta.rotation);
                break;
            default:
                Debug.Log("No hay fruta");
                break;
        }
        if (Frut != null)
        {
            Fruta_drop drop = Frut.GetComponent<Fruta_drop>();
            drop.estadoActual = estadoActual;
            drop.tiempoActual = tiempoActualFruta;
            drop.tiempoPodrido=tiempoPodrido;
            drop.tiempoPudrirse=tiempoPudrirse;
            drop.vidaTotal= vidaTotal;
            Vector3 direccionBase = camara.transform.forward;
            Vector3 direccionConDispersion = DireccionConDispersion(direccionBase, Random.Range(0, dispersion));
            Frut.GetComponent<Rigidbody>().AddForce((direccionConDispersion * fuerzaDisparo) / 3, ForceMode.Impulse);
        }
    }
    void esconderFruta()
    {
        for (int i = 0; i < FrutaMano.Length; i++)
        {
            FrutaMano [i].SetActive(false);
        }
        if (ArmaActual != 0)
        {
            ObjetoArmas_reales[ArmaActual].SetActive(true);
            AnimArma = ObjetoArmas_reales[ArmaActual].GetComponent<Animator>();
        }
        checarIdle();
    }
    void DetectarEnemigoConRaycast()
    {
        if (Gata == null) return;

        // Posición del raycast ajustando altura
        Vector3 origen = Gata.transform.position + new Vector3(0, origenRaycast.y, 0);
        Vector3 direccion = camara.transform.forward;

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
            else if (hit.collider.CompareTag("Fruta"))
            {
                Mira.GetComponent<Image>().color = ColoresMirillas[1];
                Mira.GetComponent<Image>().sprite = Mirillas[1];
                Fruta=hit.collider.gameObject;
                FrutaNombre= Fruta.GetComponent<Fruta_drop>().Nombre;
                return;
            }
        }
        // Si no golpeó enemigo o no golpeó nada
        Fruta=null;
        if (!TieneFruta)
        {
            FrutaNombre = "";
        }
        Mira.GetComponent<Image>().color = ColoresMirillas[0];
        Mira.GetComponent<Image>().sprite = Mirillas[0];
    }
    
    private void ChecarTemporal()
    {
        switch (GetComponent<Scr_ControladorBatalla>().HabilidadT)
        {
            case "Nada":
                EfectoTemp = "";
                break;

            case "Aturdimiento":
                EfectoTemp = "Stunear";
                break;

            case "Bala Incendiaria":
                EfectoTemp = "Quemar";
                break;

            case "Boca Venenosa":
                EfectoTemp = "Veneno";
                break;

            case "Cañon Congelante":
                EfectoTemp = "Congelar";
                break;

            case "Disparo rebote":
                EfectoTemp = "Rebotar";
                break;

            case "Empuje":
                EfectoTemp = "Empujar";
                break;

            case "Golpe con mas area":
                MasDistancia = 15;
                break;

            case "Golpe de fuego":
                EfectoTemp = "Quemar";
                break;

            case "Puño Relampago":
                EfectoTemp = "Electrificar";
                break;

            case "Puño Veneno":
                EfectoTemp = "Veneno";
                break;

            case "Raiz Atadora":
                EfectoTemp = "Stunear";
                break;

            case "Rebote":
                EfectoTemp = "Rebotar";
                break;

            case "Tiro Explosivo":
                EfectoTemp = "Explotar";
                break;

            case "Tiro fantasma":
                EfectoTemp = "Fantasma";
                break;

            case "Tiro Paralizante":
                EfectoTemp = "Electrificar";
                break;

            case "Velocidad mordida":
                MenosCadencia = .6f;
                break;
        }
    }
    public void AumentarDaño(float plus)
    {
        daño = daño + (daño * plus);
    }
    public void AumentarAlcance(float plus)
    {
        alcance = alcance + (alcance * plus);
    }
    public void AumentarCadencia(float plus)
    {
        cadencia = cadencia - (cadencia * plus);
    }
    public void AumentarArea(float plus)
    {
        MasDistancia = MasDistancia + plus;
    }
    public void AumentarCargador(int plus)
    {
        balascargador = balascargador + plus;
    }

    public void AumentarVelProyectil(float plus)
    {
        fuerzaDisparo = fuerzaDisparo + (fuerzaDisparo * plus);
    }
    public void AumentarProyectil(float plus)
    {
        TamaProyectil = TamaProyectil + (TamaProyectil * plus);
    }
    public void DisminuirDispersion(float plus)
    {
        dispersion = dispersion - (dispersion * plus);
    }
}
