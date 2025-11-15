using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scr_ControladorBatalla : MonoBehaviour
{
    [Header("Panel Final")]
    [SerializeField] Color[] ColoresBoton;
    [SerializeField] TextMeshProUGUI TextoNivel;
    [SerializeField] TextMeshProUGUI TextoSiguienteNivel;
    [SerializeField] Image Barra;

    public GameObject ArmaActual;

    [Header("Habilidades")]

    public int usosHabilidad;
    public string HabilidadT;
    public string Habilidad1;
    public string Habilidad2;
    public string HabilidadEspecial;
    public float PuntosActualesHabilidad = 0;

    [Header("Pociones")]
    public string Pocion;
    public bool PocionPermanente;
    public float PocionPuntos;
    public float PocionDuracion;
    public float PocionUsos;
    public string Resistencia;
    [Header("Cuenta")]

    [SerializeField] public TextMeshProUGUI NumeroCuenta;

    private float Cuenta = 4;
    private bool ComenzarCuenta = false;
    public bool ComenzoBatalla = false;

    [SerializeField] private GameObject PanelFinal;
    [SerializeField] Animator[] BarrasNegras;
    [SerializeField] GameObject CirculoCarga;

    [Header("Vida")]
    [SerializeField] GameObject Vidas;
    [SerializeField] GameObject BarraVidaImage;
    [SerializeField] TextMeshProUGUI TextoVidas;
    [SerializeField] public float VidaMaxima;
    public float PorcentajeQuitar = 1;
    public float VidaAnterior = 3;
    public float VidaActual = 3;
    public bool Stuneado = false;
    public bool Congelado = false;
    public float acumularCura = 0;
    public float menosDaño = 0;
    [SerializeField] Slider BarraVida;

    [Header("Objetivo")]
    [SerializeField] TextMeshProUGUI Mision;
    [SerializeField] TextMeshProUGUI Complemento;
    [SerializeField] TextMeshProUGUI Item;

    [Header("Barra Oleadas")]
    [SerializeField] Transform BarraSlider;
    [SerializeField] float TiempoEntreOleadas;
    float ContTiempoEntreOleadas;
    private bool PrimerSpawn = false;


    [Header("Otros")]
    [SerializeField] Light OrigenLuz;
    private Scr_DatosSingletonBatalla Singleton;
    Scr_DatosArmas Datosarmas;
    private GameObject Personaje;
    private Scr_GirarCamaraBatalla CamaraBatalla;
    private Scr_ControladorOleadas controladorOleadas;
    private bool DioRecompensa = false;

    [SerializeField] public GameObject particulaElectrica;
    [SerializeField] public GameObject particulaQuemado;
    [SerializeField] public GameObject particulaCongelado;
    [SerializeField] public GameObject particulaEnvenado;

    [Header("Efectos")]
    private Color ColorPrincipal = new Color(0, 0, 0);

    private Color dañado = new Color(1f, 0f, 0f);      // Rojo
    private Color quemado = new Color(1f, 0.365f, 0.133f);  // Naranja
    private Color congelado = new Color(0.059f, 0.816f, 1f);  // Azul claro
    private Color electrificado = new Color(1f, 0.922f, 0.118f); // Amarillo
    private Color envenenado = new Color(0.835f, 0.286f, 1f);  // Morado

    public float resistenciaStunear = 0.1f;  // 10% de probabilidad de resistir el Stun
    public float resistenciaQuemar = 0.2f;   // 20% de probabilidad de resistir el Quemar
    public float resistenciaVeneno = 0.3f;   // 30% de probabilidad de resistir el Veneno
    public float resistenciaCongelar = 0.15f; // 15% de probabilidad de resistir el Congelar
    public float resistenciaEmpujar = 0.05f; // 5% de probabilidad de resistir el Empujar
    public float resistenciaElectrificar = 0.25f; // 25% de probabilidad de resistir el Electrificar
    public float resistenciaExplotar = 0.4f; // 40% de probabilidad de resistir la Explotación

    [Header("Cuenta Atras")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip SonidoReloj;
    [SerializeField] private AudioClip SonidoPelea; // el sonido que suena cuando dice "Pelea"
    private string ultimoTextoMostrado = "";


    void Start()
    {
        Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
        Datosarmas = Singleton.GetComponent<Scr_DatosArmas>();
        controladorOleadas = GetComponent<Scr_ControladorOleadas>();
        Personaje = GameObject.Find("Personaje");
        CamaraBatalla = Personaje.transform.GetChild(0).gameObject.GetComponent<Scr_GirarCamaraBatalla>();
        Mision.text = Singleton.Mision;
        Mision.color = Singleton.ColorMision;
        Complemento.text = Singleton.Complemento;
        Item.text = Singleton.Item;
        Item.color = Singleton.ColorItem;
        /*
        Habilidad1 = PlayerPrefs.GetString("Habilidad1", "Ojo");
        Habilidad2 = PlayerPrefs.GetString("Habilidad2", "Rugido");
        HabilidadEspecial = PlayerPrefs.GetString("HabilidadEspecial", "Garras");*/

        ColorPrincipal = BarraVidaImage.GetComponent<Image>().color;

        TextoVidas.text = VidaMaxima + "/" + VidaMaxima;
        if (Singleton.HoraActual > 7 && Singleton.HoraActual < 19)
        {
            RenderSettings.skybox = Singleton.SkyBoxDia;
        }
        else
        {
            RenderSettings.skybox = Singleton.SkyBoxNoche;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12)) PlayerPrefs.DeleteAll();

        Comienzo();
        ActualizarVida();
        Terminar();
    }

    //obtiene las habilidades que tiene la arma
    public void ConseguirHabilidadesArma(string arma)
    {
        usosHabilidad = PlayerPrefs.GetInt(arma + "Usos", 0);
        HabilidadT = PlayerPrefs.GetString(arma + "HT", "Nada");
        Habilidad1 = PlayerPrefs.GetString(arma + "H1", "Ojo");
        Habilidad2 = PlayerPrefs.GetString(arma + "H2", "Rugido");
        HabilidadEspecial = PlayerPrefs.GetString(arma + "HE", "Garras");
    }
    public void GuardarHabilidadesArma(string arma)
    {
        PlayerPrefs.SetString(arma + "HT", HabilidadT);
        PlayerPrefs.SetString(arma + "H1", Habilidad1);
        PlayerPrefs.SetString(arma + "H2", Habilidad2);
        PlayerPrefs.SetString(arma + "HE", HabilidadEspecial);
        PlayerPrefs.SetInt(arma + "Usos", usosHabilidad);
        Datosarmas.guardarHabilidades();
    }

    public void Guardar_Pocion()
    {
        Datosarmas.QuitarCanidadPociones(Pocion);
    }
    public void HabilidadEsPasiva()
    {

    }
    private void ActualizarVida()
    {
        if (VidaActual != VidaAnterior)
        {
            VidaAnterior = VidaActual;
            TextoVidas.text = VidaActual + "/" + VidaMaxima;
            BarraVida.value = VidaActual / VidaMaxima;
        }
    }

    public void IniciarCuentaRegresiva()
    {
        NumeroCuenta.gameObject.SetActive(true);
        ComenzarCuenta = true;
    }

    private void Comienzo()
    {
        if (ComenzarCuenta)
        {
            if (Cuenta > 0)
            {
                // --- Preparación al inicio ---
                if (Cuenta == 4)
                {
                    if (!PrimerSpawn)
                    {
                        OrigenLuz.color = Singleton.Luz;
                        PrepararBatalla();
                        controladorOleadas.IniciarPrimeraOleada();
                        PrimerSpawn = true;
                    }
                }

                // --- Actualizar número visual ---
                int numeroActual = Mathf.FloorToInt(Cuenta); // floor evita el salto del 1
                if (numeroActual > 3) numeroActual = 3;      // nunca mostrar 4

                string textoMostrado = numeroActual > 0 ? numeroActual.ToString() : "Pelea";
                NumeroCuenta.text = textoMostrado;

                // --- Cambiar sonido solo cuando cambia el número o pasa a "Pelea" ---
                if (textoMostrado != ultimoTextoMostrado)
                {
                    if (textoMostrado == "Pelea")
                    {
                        audioSource.clip = SonidoPelea; // sonido especial
                    }
                    else
                    {
                        audioSource.clip = SonidoReloj; // sonido del reloj (3,2,1)
                    }
                    audioSource.Play();

                    ultimoTextoMostrado = textoMostrado;
                }

                Cuenta -= Time.deltaTime;
            }
            else
            {
                // --- Fin de la cuenta atrás ---
                if (controladorOleadas.enemigosOleada.Count > 0 && ComenzarCuenta)
                {
                    foreach (GameObject Enemigo in controladorOleadas.enemigosOleada)
                    {
                        Enemigo.GetComponent<NavMeshAgent>().enabled = true;
                    }
                }

                NumeroCuenta.gameObject.SetActive(false);
                ComenzarCuenta = false;
                Cuenta = 4;
                ActivarControles(true);
                ComenzoBatalla = true;
            }
        }
    }




    private void Terminar()
    {
        if (VidaActual <= 0)
        {
            VidaActual = Mathf.Max(VidaActual, 0);
            ComenzoBatalla = false;
            FinalizarBatalla(false);
        }
        else if (ComenzoBatalla)
        {
            controladorOleadas.ComprobarOleada();
        }


    }

    public void FinalizarBatalla(bool Gano)
    {
        ActivarControles(false);
        if (Gano && !DioRecompensa)
        {
            DioRecompensa = true;
            DarRecompensa();
        }

        if (PlayerPrefs.GetInt("XPActual", 0) >= PlayerPrefs.GetInt("XPSiguiente", 10))
        {
            PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual") - PlayerPrefs.GetInt("XPSiguiente", 10));
            PlayerPrefs.SetInt("Nivel", PlayerPrefs.GetInt("Nivel", 0) + 1);
            PlayerPrefs.SetInt("XPSiguiente", PlayerPrefs.GetInt("XPSiguiente", 10) * 2);
            PlayerPrefs.SetInt("PuntosDeHabilidad", PlayerPrefs.GetInt("PuntosDeHabilidad", 0) + 3);
        }

        TextoNivel.text = PlayerPrefs.GetInt("Nivel", 0).ToString();
        TextoSiguienteNivel.text = "Siguiente Nivel: " + PlayerPrefs.GetInt("XPActual", 0) + "/" + PlayerPrefs.GetInt("XPSiguiente", 10);
        Barra.fillAmount = (float)PlayerPrefs.GetInt("XPActual", 0) / PlayerPrefs.GetInt("XPSiguiente", 10);

        PanelFinal.SetActive(true);
    }

    private void DarRecompensa()
    {
        var enemigo = Singleton.Enemigo.GetComponent<Scr_Enemigo>();
        var recompensasDict = new Dictionary<Scr_CreadorObjetos, int>();
        
        int totalEnemigos = enemigo.CantidadEnemigosPorOleada * controladorOleadas.OleadaActual;
        Debug.Log(totalEnemigos + "**********");
        for (int k = 0; k < totalEnemigos; k++)
        {
            // Tiradas por enemigo individual
            for (int i = 0; i < enemigo.Drops.Length; i++)
            {
                if (Random.Range(0, 100) <= enemigo.Probabilidades[i])
                {
                    if (recompensasDict.ContainsKey(enemigo.Drops[i]))
                        recompensasDict[enemigo.Drops[i]] += 1;
                    else
                        recompensasDict[enemigo.Drops[i]] = 1;
                }
            }

            // XP individual por enemigo
            PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual") + Random.Range(enemigo.XPMinima, enemigo.XPMaxima));
        }
        
        int cazado = PlayerPrefs.GetInt("Cazado_Cantidad", 0);
        bool havecaza = false;
        if (cazado > 0)
        {
            for (int i = 0; i < cazado; i++)
            {
                string nombrecazado = PlayerPrefs.GetString("Cazado_" + i, "");
                int cantCazados = PlayerPrefs.GetInt("cazado_cant" + i, 0);
                if (!string.IsNullOrEmpty(nombrecazado) && cantCazados > 0)
                {
                    if (nombrecazado == enemigo.name)
                    {
                        Debug.Log("ya exite");
                        PlayerPrefs.SetString("Cazado_" + i, nombrecazado);
                        PlayerPrefs.SetInt("cazado_cant" + i, cantCazados + totalEnemigos);
                        havecaza = true;
                        break;
                    }
                }
            }
        }


        if (!havecaza)
        {

            Debug.Log("NO exite" + cazado);
            //Debug.Log("se caso" + enemigo.name + " num" + cazado);
            PlayerPrefs.SetString("Cazado_" + cazado, enemigo.name);
            PlayerPrefs.SetInt("cazado_cant" + cazado, totalEnemigos);
            PlayerPrefs.SetInt("Cazado_Cantidad", cazado+1);
        }
        PlayerPrefs.Save();

        // Mostrar recompensas
        var datos = Singleton;
        datos.ObjetosRecompensa.Clear();
        datos.CantidadesRecompensa.Clear();
        //datos.enemicaza = enemigo.name;
        //datos.cantcaza = totalEnemigos;
        int index = 0;
        foreach (var kvp in recompensasDict)
        {
            datos.ObjetosRecompensa.Add(kvp.Key);
            datos.CantidadesRecompensa.Add(kvp.Value);

            Transform rewardUI = PanelFinal.transform.GetChild(0).GetChild(6).GetChild(index);
            rewardUI.GetComponent<Image>().sprite = kvp.Key.IconoInventario;
            rewardUI.gameObject.SetActive(true);
            rewardUI.GetChild(0).gameObject.SetActive(true);
            rewardUI.GetChild(0).GetComponent<TextMeshProUGUI>().text = kvp.Value.ToString();
            index++;
        }
    }


    private void ActivarControles(bool activar)
    {
        Cursor.visible = !activar;
        Cursor.lockState = activar ? CursorLockMode.Locked : CursorLockMode.None;

        var camara = Camera.main;
        GetComponent<Scr_ControladorArmas>().enabled = activar;
        camara.GetComponent<Scr_GirarCamaraBatalla>().enabled = activar;
        camara.transform.parent.GetComponent<Rigidbody>().useGravity = activar;
        camara.transform.parent.GetComponent<Scr_Movimiento>().enabled = activar;
    }

    public void BotonAceptar()
    {
        foreach (Animator anim in BarrasNegras)
            anim.Play("Cerrar");

        StartCoroutine(EsperarCierre());
        PanelFinal.GetComponent<Animator>().Play("Cerrar");
    }

    public void CambiarColorAceptar(bool Entra)
    {
        PanelFinal.transform.GetChild(0).GetChild(5).GetComponent<Image>().color = Entra ? ColoresBoton[0] : ColoresBoton[1];
    }

    IEnumerator EsperarCierre()
    {
        yield return new WaitForSeconds(1);
        CirculoCarga.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(2);
    }

    public void PrepararBatalla()
    {
        // Activar mapa y posicionar jugador
        var mapa = GameObject.Find("Mapa").transform;

        foreach (Transform child in mapa)
        {
            if (child.name == Singleton.NombreMapa)
            {
                child.gameObject.SetActive(true);
                foreach (Transform objeto in child)
                {

                    if (objeto.name.Contains("Posicion Inicial") && controladorOleadas.OleadaActual == 1)
                    {
                        GameObject.Find("Personaje").transform.position = objeto.transform.position;
                        GameObject.Find("Personaje").transform.rotation = objeto.transform.rotation;
                    }
                }
            }
        }

        // Reconstruir NavMesh
        GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>().BuildNavMesh();
    }


    //////////////////////////////////////////////////Estado de efecto para el jugador
    public void RecibirDaño(float DañoRecibido)
    {
        // Reducir la vida del enemigo
        if (menosDaño > 0)
        {
            DañoRecibido = DañoRecibido * menosDaño;
        }
        if (VidaActual >= DañoRecibido)
        {
            VidaActual -= DañoRecibido;
        }
        else
        {
            VidaActual = 0; // 🔹 Evita valores negativos
        }
        CamaraBatalla.ResetSuavizado();
    }
    public void Curar(float CuraRecibida)
    {
        VidaActual = VidaActual + CuraRecibida;
        if (VidaActual > VidaMaxima)
        {
            VidaActual = VidaMaxima;
        }
    }
    public void RecibirEfecto(string efecto)
    {
        checarEfecto(efecto);
    }

    public void checarEfecto(string efecto)
    {
        // Variable que determina la probabilidad de que el efecto sea resistido
        float probabilidadResistencia = Random.Range(0f, 1f);

        // Evaluar la resistencia según el tipo de efecto
        switch (efecto)
        {
            case "Stunear":
                if (probabilidadResistencia <= resistenciaStunear)
                {
                    Debug.Log("Efecto Stun resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoStuneado(1f));
                break;

            case "Quemar":
                if (probabilidadResistencia <= resistenciaQuemar)
                {
                    Debug.Log("Efecto Quemar resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoQuemando(1.6f, 2f)); // duración 5s, 2 de daño por segundo
                break;

            case "Veneno":
                if (probabilidadResistencia <= resistenciaVeneno)
                {
                    Debug.Log("Efecto Veneno resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoVeneno(4f, 1f)); // duración 5s, 1 de daño por segundo
                break;

            case "Congelar":
                if (probabilidadResistencia <= resistenciaCongelar)
                {
                    Debug.Log("Efecto Congelar resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoCongelado(2f)); // duración 4s
                break;

            case "Empujar":
                if (probabilidadResistencia <= resistenciaEmpujar)
                {
                    Debug.Log("Efecto Empujar resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoEmpujado(270f)); // dirección y fuerza
                break;

            case "Electrificar":
                if (probabilidadResistencia <= resistenciaElectrificar)
                {
                    Debug.Log("Efecto Electrificar resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoElectrificado(3f, 3f)); // duración 3s, daño 3 por segundo
                efecto = "Rebotar"; // cambio de lógica como mencionaste
                break;

            case "Explotar":
                if (probabilidadResistencia <= resistenciaExplotar)
                {
                    Debug.Log("Efecto Explotar resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoExplotado(20f, 170f, transform.position - Vector3.forward)); // daño, fuerza, origen
                break;

            default:
                break;
        }

        CamaraBatalla.ResetSuavizado();
    }

    private IEnumerator ChangeMaterial(Color mat, float time)
    {
        Color ColorBarra = BarraVida.GetComponent<Image>().color;
        if (ColorBarra != null)
        {
            Color ColorOriginal = ColorBarra;
            ColorBarra = mat;

            // Esperar el tiempo deseado
            float tiempo = 0;
            while (tiempo < time)
            {
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Restaurar materiales originales
            ColorBarra = ColorOriginal;
        }
    }
    IEnumerator EstadoStuneado(float duracion)
    {
        Stuneado = true;
        BarraVidaImage.GetComponent<Image>().color = dañado;
        yield return new WaitForSeconds(duracion);
        BarraVidaImage.GetComponent<Image>().color = ColorPrincipal;
        Stuneado = false;
    }

    IEnumerator EstadoQuemando(float duracion, float dañoPorSegundo)
    {
        // Muestra el efecto
        //GameObject explosion = Instantiate(particulaQuemado, transform.position, transform.rotation);

        float tiempoPasado = 0f;
        while (tiempoPasado < duracion)
        {
            RecibirDaño(dañoPorSegundo);
            BarraVidaImage.GetComponent<Image>().color = quemado;
            yield return new WaitForSeconds(1f);
            BarraVidaImage.GetComponent<Image>().color = ColorPrincipal;
            tiempoPasado += 1f;
        }
        //Destroy(explosion);
    }

    IEnumerator EstadoVeneno(float duracion, float dañoPorSegundo)
    {
        // Muestra el efecto
        //GameObject explosion = Instantiate(particulaEnvenado, transform.position, transform.rotation);
        float tiempoPasado = 0f;
        while (tiempoPasado < duracion)
        {
            RecibirDaño(dañoPorSegundo);
            BarraVidaImage.GetComponent<Image>().color = envenenado;
            yield return new WaitForSeconds(1f);
            BarraVidaImage.GetComponent<Image>().color = ColorPrincipal;
            tiempoPasado += 1f;
        }
        //Destroy(explosion);
    }

    IEnumerator EstadoCongelado(float duracion)
    {
        // Muestra el efecto
        //GameObject explosion = Instantiate(particulaCongelado, transform.position, transform.rotation);
        Congelado = true;
        Debug.Log("Enemigo congelado");
        BarraVidaImage.GetComponent<Image>().color = congelado;
        yield return new WaitForSeconds(duracion);
        BarraVidaImage.GetComponent<Image>().color = ColorPrincipal;
        Congelado = true;
        //Destroy(explosion);
    }

    IEnumerator EstadoEmpujado(float fuerza)
    {
        Rigidbody rb = Personaje.GetComponent<Rigidbody>();
        Vector3 direccion = -Personaje.transform.forward; // dirección hacia atrás del personaje
        rb.AddForce(direccion * fuerza, ForceMode.Force);
        Vector3 arriba = Personaje.transform.up; // dirección hacia atrás del personaje
        rb.AddForce(arriba * (fuerza*.7f), ForceMode.Force);
        yield return null;
    }

    IEnumerator EstadoExplotado(float daño, float fuerzaEmpuje, Vector3 origenExplosion)
    {
        RecibirDaño(daño);
        //Vector3 direccion = transform.position - origenExplosion;
        Vector3 direccion = -Personaje.transform.forward; // dirección hacia atrás del personaje
        Personaje.GetComponent<Rigidbody>().AddForce(direccion.normalized * fuerzaEmpuje, ForceMode.Impulse);
        yield return null;
    }

    IEnumerator EstadoElectrificado(float duracion, float dañoPorSegundo)
    {
        // Muestra el efecto
        //GameObject explosion = Instantiate(particulaElectrica, transform.position, transform.rotation);
        float tiempoPasado = 0f;
        while (tiempoPasado < duracion)
        {
            RecibirDaño(dañoPorSegundo);
            BarraVidaImage.GetComponent<Image>().color = congelado;
            yield return new WaitForSeconds(1f);
            BarraVidaImage.GetComponent<Image>().color = ColorPrincipal;
            tiempoPasado += 1f;
        }
        //Destroy(explosion);
    }
}
