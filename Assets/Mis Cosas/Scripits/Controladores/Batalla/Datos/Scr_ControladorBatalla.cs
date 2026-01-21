using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Scr_DatosSingletonBatalla;

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
    public string HabilidadTDefault;
    public string Habilidad1Default;
    public string Habilidad2Default;
    public string HabilidadEspecialDefault;

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

    public GameObject ContadorFruta;
    public GameObject ContadorEnemigos;
    public int FrutasRecolectadas;
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
    private Scr_ControladorRecolleccion controladorRecoleccion;
    private bool DioRecompensa = false;
    private int experiencia = 0;
    int Bonus=0;
    [SerializeField] public GameObject particulaElectrica;
    [SerializeField] public GameObject particulaQuemado;
    [SerializeField] public GameObject particulaCongelado;
    [SerializeField] public GameObject particulaEnvenado;
    [SerializeField] public GameObject ParticulaGolpe;

    Scr_ControladorArmas armas;
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

    Musica_pelea Musica;
    ChecarInput Checar_input;
    void Start()
    {
        Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
        Datosarmas = Singleton.GetComponent<Scr_DatosArmas>();
        armas= GetComponent<Scr_ControladorArmas>();
        controladorOleadas = GetComponent<Scr_ControladorOleadas>();
        controladorRecoleccion = GetComponent<Scr_ControladorRecolleccion>();
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
        ActivarControladores();
        Musica = GameObject.Find("Musica").GetComponent<Musica_pelea>();

        Checar_input = GameObject.Find("Singleton").GetComponent<ChecarInput>();
        Checar_input.CammbiarAction_UI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12)) PlayerPrefs.DeleteAll();

        Comienzo();
        ActualizarVida();
        Terminar();
        checarmusica();
    }

    public void ActivarControladores()
    {
        switch (Singleton.ModoSeleccionado.ToString())
        {
            case "Defensa":
                controladorOleadas.enabled = true;
                controladorRecoleccion.enabled = true;
                ContadorEnemigos.SetActive(true);
                ContadorFruta.SetActive(true);
                break;
            case "Recoleccion":
                controladorOleadas.enabled = false;
                controladorRecoleccion.enabled = true;
                ContadorEnemigos.SetActive(false);
                ContadorFruta.SetActive(true);
                break;
            case "Pelea":
                controladorOleadas.enabled = true;
                controladorRecoleccion.enabled = false;
                ContadorEnemigos.SetActive(true);
                ContadorFruta.SetActive(false);
                break;
            case "":
                controladorOleadas.enabled = true;
                controladorRecoleccion.enabled = false;
                ContadorEnemigos.SetActive(true);
                ContadorFruta.SetActive(false);
                break;
        }
    }
    public void CehcarHabilidadDefault(string arma)
    {
        switch (arma)
        {
            case "Brazos":
                HabilidadTDefault = "Nada";
                Habilidad1Default = "Ojo";
                Habilidad2Default = "Rugido";
                HabilidadEspecialDefault = "Garras";
                break;
            case "Chilenon":
                HabilidadTDefault = "Nada";
                Habilidad1Default = "Ojo";
                Habilidad2Default = "Rugido";
                HabilidadEspecialDefault = "Garras";
                break;
            case "Coco":
                HabilidadTDefault = "Nada";
                Habilidad1Default = "Ojo";
                Habilidad2Default = "Rugido";
                HabilidadEspecialDefault = "Garras";
                break;
            case "Mango":
                HabilidadTDefault = "Nada";
                Habilidad1Default = "Ojo";
                Habilidad2Default = "Rugido";
                HabilidadEspecialDefault = "Garras";
                break;
            case "Papa":
                HabilidadTDefault = "Nada";
                Habilidad1Default = "Ojo";
                Habilidad2Default = "Rugido";
                HabilidadEspecialDefault = "Garras";
                break;
            case "Planta":
                HabilidadTDefault = "Nada";
                Habilidad1Default = "Crecimiento Rapido";
                Habilidad2Default = "Cuidado Automata";
                HabilidadEspecialDefault = "Cuello extensible";
                break;
            case "Platano":
                HabilidadTDefault = "Nada";
                Habilidad1Default = "Municion Perpetua";
                Habilidad2Default = "Tiro explosivo";
                HabilidadEspecialDefault = "Tiro Esparcido";
                break;
            case "Sandia":
                HabilidadTDefault = "Nada";
                Habilidad1Default = "Fuerza Imparable";
                Habilidad2Default = "Protección del Coloso";
                HabilidadEspecialDefault = "Semilla explosiva";
                break;
            case "Tomate":
                HabilidadTDefault = "Nada";
                Habilidad1Default = "Pulso Sonoro";
                Habilidad2Default = "Disparo explosivo";
                HabilidadEspecialDefault = "Disparo maciso";
                break;
            case "Uvalon":
                HabilidadTDefault = "Nada";
                Habilidad1Default = "Ojo";
                Habilidad2Default = "Rugido";
                HabilidadEspecialDefault = "Garras";
                break;
        }
    }
    //obtiene las habilidades que tiene la arma
    public void ConseguirHabilidadesArma(string arma)
    {
        CehcarHabilidadDefault(arma);
        usosHabilidad = PlayerPrefs.GetInt(arma + "Usos", 0);
        HabilidadT = PlayerPrefs.GetString(arma + "HT", HabilidadTDefault);
        Habilidad1 = PlayerPrefs.GetString(arma + "H1", Habilidad1Default);
        Habilidad2 = PlayerPrefs.GetString(arma + "H2", Habilidad2Default);
        HabilidadEspecial = PlayerPrefs.GetString(arma + "HE", HabilidadEspecialDefault);
    }
    public void GuardarHabilidadesArma(string arma)
    {
        PlayerPrefs.SetString(arma + "H1", Habilidad1);
        PlayerPrefs.SetString(arma + "H2", Habilidad2);
        PlayerPrefs.SetString(arma + "HE", HabilidadEspecial);
        if (usosHabilidad > 0)
        {
            PlayerPrefs.SetString(arma + "HT", HabilidadT);
            PlayerPrefs.SetInt(arma + "Usos", usosHabilidad);
        }
        else
        {
            PlayerPrefs.SetString(arma + "HT", "Nada");
            PlayerPrefs.SetInt(arma + "Usos", 0);
        }
        PlayerPrefs.Save();
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

    public void IniciarCuentaRegresiva(bool Checar)
    {
        if (PlayerPrefs.GetString("TutorialPeleas", "NO") == "SI" || Checar)
        {
            NumeroCuenta.gameObject.SetActive(true);
            ComenzarCuenta = true;
        }
        if (Singleton.ModoSeleccionado == Modo.Defensa)
        {
            ComienzoDefensa();
        }
        else if (Singleton.ModoSeleccionado == Modo.Recoleccion)
        {
            ComienzoRecoleccion();
        }
        else if (Singleton.ModoSeleccionado == Modo.Pelea)
        {
            ComienzoBatalla();
        }

    }
    private void Comienzo()
    {
        if (Singleton.ModoSeleccionado == Modo.Defensa)
        {
            ComienzoDefensa();
        }
        else if (Singleton.ModoSeleccionado == Modo.Recoleccion)
        {
            ComienzoRecoleccion();
        }
        else if (Singleton.ModoSeleccionado == Modo.Pelea)
        {
            ComienzoBatalla();
        }
    }
    private void ComienzoBatalla()
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
                        NavMeshAgent enenav = Enemigo.GetComponent<NavMeshAgent>();
                        if (enenav != null)
                        {
                            NavMeshHit hit;
                            bool enNavMesh = NavMesh.SamplePosition(Enemigo.transform.position, out hit, 1.5f, NavMesh.AllAreas);

                            Debug.Log($"¿Está {Enemigo.name} sobre NavMesh? {enNavMesh}");
                            enenav.enabled = true;
                        }
                        else
                        {
                            Debug.Log("No tiene el NavMeshAgent");
                        }
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
    private void ComienzoRecoleccion()
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
                        controladorRecoleccion.IniciarRecoleccion();
                        PrimerSpawn = true;
                    }
                }

                // --- Actualizar número visual ---
                int numeroActual = Mathf.FloorToInt(Cuenta); // floor evita el salto del 1
                if (numeroActual > 3) numeroActual = 3;      // nunca mostrar 4

                string textoMostrado = numeroActual > 0 ? numeroActual.ToString() : "¡Recolecta!";
                NumeroCuenta.text = textoMostrado;

                // --- Cambiar sonido solo cuando cambia el número o pasa a "Pelea" ---
                if (textoMostrado != ultimoTextoMostrado)
                {
                    if (textoMostrado == "¡Recolecta!")
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
                if (controladorRecoleccion.enemigosOleada.Count > 0 && ComenzarCuenta)
                {
                    foreach (GameObject Enemigo in controladorRecoleccion.enemigosOleada)
                    {
                        NavMeshAgent enenav = Enemigo.GetComponent<NavMeshAgent>();
                        if (enenav != null)
                        {
                            NavMeshHit hit;
                            bool enNavMesh = NavMesh.SamplePosition(Enemigo.transform.position, out hit, 1.5f, NavMesh.AllAreas);

                            Debug.Log($"¿Está {Enemigo.name} sobre NavMesh? {enNavMesh}");
                            enenav.enabled = true;
                        }
                        else
                        {
                            Debug.Log("No tiene el NavMeshAgent");
                        }
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
    private void ComienzoDefensa()
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
                        controladorRecoleccion.IniciarRecoleccion();
                        PrimerSpawn = true;
                    }
                }

                // --- Actualizar número visual ---
                int numeroActual = Mathf.FloorToInt(Cuenta); // floor evita el salto del 1
                if (numeroActual > 3) numeroActual = 3;      // nunca mostrar 4

                string textoMostrado = numeroActual > 0 ? numeroActual.ToString() : "¡Defiende!";
                NumeroCuenta.text = textoMostrado;

                // --- Cambiar sonido solo cuando cambia el número o pasa a "Pelea" ---
                if (textoMostrado != ultimoTextoMostrado)
                {
                    if (textoMostrado == "¡Defiende!")
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
                        NavMeshAgent enenav = Enemigo.GetComponent<NavMeshAgent>();
                        if (enenav != null)
                        {
                            NavMeshHit hit;
                            bool enNavMesh = NavMesh.SamplePosition(Enemigo.transform.position, out hit, 1.5f, NavMesh.AllAreas);

                            Debug.Log($"¿Está {Enemigo.name} sobre NavMesh? {enNavMesh}");
                            enenav.enabled = true;
                        }
                        else
                        {
                            Debug.Log("No tiene el NavMeshAgent");
                        }
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

        PlayerPrefs.SetString("TutorialPeleas", "SI");

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

        Checar_input.CammbiarAction_UI();
    }


    private void DarRecompensa()
    {
        var recompensasDict = new Dictionary<Scr_CreadorObjetos, int>();
        if (Singleton.ModoSeleccionado == Modo.Defensa)
        {
            RecompensaRecolec(recompensasDict);
            RecopensaEnemigo(recompensasDict);
        }
        else if (Singleton.ModoSeleccionado == Modo.Recoleccion)
        {
            RecompensaRecolec(recompensasDict);
        }
        else if (Singleton.ModoSeleccionado == Modo.Pelea)
        {
            RecopensaEnemigo(recompensasDict);
        }

        MostrarRecompensas(recompensasDict);

        PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual") + (experiencia + Bonus));
        PlayerPrefs.Save();
    }
    private void RecompensaRecolec(Dictionary<Scr_CreadorObjetos, int> dict)
    {
        var extra = controladorRecoleccion.FrutaRecom; // ScriptableObject

        if (extra == null) return;

        if (dict.ContainsKey(extra))
            dict[extra]++;
        else
            dict.Add(extra, 1);
        PlayerPrefs.SetInt("Dinero", PlayerPrefs.GetInt("Dinero", 0) + Random.Range(controladorRecoleccion.DineroMin, controladorRecoleccion.DineroMax));
    }
    private void RecopensaEnemigo(Dictionary<Scr_CreadorObjetos, int> dict)
    {
        var enemigo = Singleton.Enemigo.GetComponent<Scr_Enemigo>();
        //var recompensasDict = new Dictionary<Scr_CreadorObjetos, int>();

        int totalEnemigos = enemigo.CantidadEnemigosPorOleada * controladorOleadas.OleadaActual;
        Debug.Log(totalEnemigos + "**********");
        for (int k = 0; k < totalEnemigos; k++)
        {
            // Tiradas por enemigo individual
            for (int i = 0; i < enemigo.Drops.Length; i++)
            {
                if (Random.Range(0, 100) <= enemigo.Probabilidades[i])
                {
                    if (dict.ContainsKey(enemigo.Drops[i]))
                        dict[enemigo.Drops[i]] += 1;
                    else
                        dict[enemigo.Drops[i]] = 1;
                }
            }

            // XP individual por enemigo
            experiencia += Random.Range(enemigo.XPMinima, enemigo.XPMaxima);
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
            PlayerPrefs.SetInt("Cazado_Cantidad", cazado + 1);
        }
        PlayerPrefs.Save();
        /*
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
        */
    }
    private void MostrarRecompensas(Dictionary<Scr_CreadorObjetos, int> dict)
    {
        var datos = Singleton;
        datos.ObjetosRecompensa.Clear();
        datos.CantidadesRecompensa.Clear();

        int index = 0;
        int maxRecompensas = 3;

        foreach (var kvp in dict)
        {
            if (index >= maxRecompensas) break;

            datos.ObjetosRecompensa.Add(kvp.Key);
            datos.CantidadesRecompensa.Add(kvp.Value);

            Transform rewardUI = PanelFinal.transform
                .GetChild(0).GetChild(6).GetChild(index);

            rewardUI.GetComponent<Image>().sprite = kvp.Key.IconoInventario;
            rewardUI.gameObject.SetActive(true);

            rewardUI.GetChild(0).gameObject.SetActive(true);
            rewardUI.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = kvp.Value.ToString();

            index++;
        }
    }
    private void ActivarControles(bool activar)
    {
        Cursor.visible = !activar;
        Cursor.lockState = activar ? CursorLockMode.Locked : CursorLockMode.None;

        var camara = Camera.main;
        camara.GetComponent<Scr_GirarCamaraBatalla>().enabled = activar;
        camara.transform.parent.GetComponent<Rigidbody>().useGravity = activar;
        camara.transform.parent.GetComponent<Scr_Movimiento>().enabled = activar;
        if (GetComponent<Scr_ControladorArmas>() != null)
        {
            GetComponent<Scr_ControladorArmas>().enabled = activar;
        }
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
                if (PlayerPrefs.GetString("ClimaActivadoEsta", "NO") == "SI")
                {
                    Transform Clim = child.transform.Find("Climas");
                    string[] dias = { "LUN", "MAR", "MIE", "JUE", "VIE", "SAB", "DOM" };
                    int diaActualIndex = System.Array.IndexOf(dias, PlayerPrefs.GetString("DiaActual", "LUN"));
                    int diaclim = PlayerPrefs.GetInt("Clima" + diaActualIndex, 0);
                    if (Clim != null && diaclim>0)
                    {
                        //Clim.gameObject.SetActive(true);
                        Clim.GetChild(diaclim).gameObject.SetActive(true);
                    }
                }
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

        Checar_input.CammbiarAction_Player();
        //GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    public void PrepararReoleccion()
    {
        // Activar mapa y posicionar jugador
        var mapa = GameObject.Find("Mapa").transform;
        foreach (Transform child in mapa)
        {
            if (child.name == Singleton.NombreMapa)
            {
                child.gameObject.SetActive(true);
                if (PlayerPrefs.GetString("ClimaActivadoEsta", "NO") == "SI")
                {
                    Transform Clim = child.transform.Find("Climas");
                    string[] dias = { "LUN", "MAR", "MIE", "JUE", "VIE", "SAB", "DOM" };
                    int diaActualIndex = System.Array.IndexOf(dias, PlayerPrefs.GetString("DiaActual", "LUN"));
                    int diaclim = PlayerPrefs.GetInt("Clima" + diaActualIndex, 0);
                    if (Clim != null && diaclim > 0)
                    {
                        //Clim.gameObject.SetActive(true);
                        Clim.GetChild(diaclim).gameObject.SetActive(true);
                    }
                }
                foreach (Transform objeto in child)
                {

                    if (objeto.name.Contains("Posicion Inicial"))
                    {
                        GameObject.Find("Personaje").transform.position = objeto.transform.position;
                        GameObject.Find("Personaje").transform.rotation = objeto.transform.rotation;
                    }
                }
            }
        }

        // Reconstruir NavMesh
        GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>().BuildNavMesh();

        //GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>().BuildNavMesh();
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

    public void checarmusica()
    {
        float cehcarcantidad = (VidaActual * 100) / VidaMaxima;
        Musica.ConseguirPorcentajeVida(cehcarcantidad);
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
                    //Debug.Log("Efecto Empujar resistido.");
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
        Debug.Log("Empujado");
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


    public void ChecarRango(string arma)
    {
        int Rango = PlayerPrefs.GetInt("Rango " + arma, 1) - 1;
        Debug.Log(Rango);
        if (Rango <= 0) return;
        //"Rango" + Nombre del arma
        //PlayerPrefs.GetInt("Rango " + Datosarmas.TodasLasArmas[objShow].Nombre, 1)-1

        switch (arma)
        {
            case "Brazos":
                Checar_Rango_Brazos(Rango);
                break;
            case "Chile":
                Checar_Rango_Chile(Rango);
                break;
            case "Coco":
                Checar_Rango_Coco(Rango);
                break;
            case "Mango":
                Checar_Rango_Mango(Rango);
                break;
            case "Papa":
                Checar_Rango_Papa(Rango);
                break;
            case "Planta":
                Checar_Rango_Planta(Rango);
                break;
            case "Platano":
                Checar_Rango_Platano(Rango);
                break;
            case "Sandia":
                Checar_Rango_Sandia(Rango);
                break;
            case "Tomate":
                Checar_Rango_Tomate(Rango);
                break;
            case "Uva":
                Checar_Rango_Uva(Rango);
                break;
        }
    }

    public void AumentarVelocidad(float plus)
    {
        Scr_Movimiento mov=Personaje.GetComponent<Scr_Movimiento>();
        mov.VelAgachado = mov.VelAgachado + (mov.VelAgachado * plus);
        mov.VelCaminar = mov.VelCaminar + (mov.VelCaminar * plus);
        mov.VelCorrer = mov.VelCorrer + (mov.VelCorrer * plus);
    }
    public void Checar_Rango_Brazos(int Rango)
    {
        switch (Rango)
        {
            case 1:
                armas.AumentarDaño(.2f);
                break;
            case 2:
                armas.AumentarDaño(.2f);
                armas.AumentarCadencia(1);

                break;
            case 3:
                armas.AumentarDaño(.2f);
                armas.AumentarCadencia(1);
                armas.AumentarAlcance(1f);
                break;
            case 4:
                armas.AumentarDaño(.2f);
                armas.AumentarCadencia(1);
                armas.AumentarAlcance(1f);
                break;
        }
    }
    public void Checar_Rango_Chile(int Rango)
    {
        switch (Rango)
        {
            case 1:
                armas.AumentarDaño(1);
                break;
            case 2:
                armas.AumentarDaño(1);
                armas.AumentarAlcance(1f);
                break;
            case 3:
                armas.AumentarDaño(1);
                armas.AumentarAlcance(1f);
                armas.AumentarCargador(50);
                break;
            case 4:
                armas.AumentarDaño(1);
                armas.AumentarAlcance(1f);
                armas.AumentarCargador(50);
                break;
        }
    }
    public void Checar_Rango_Coco(int Rango)
    {
        switch (Rango)
        {
            case 1:
                menosDaño = .5f;
                break;
            case 2:
                menosDaño = .5f;
                AumentarVelocidad(.4f);
                break;
            case 3:
                menosDaño = .5f;
                AumentarVelocidad(.4f);
                Bonus = 10;
                break;
            case 4:
                menosDaño = .5f;
                AumentarVelocidad(.4f);
                Bonus = 10;
                break;
        }
    }
    public void Checar_Rango_Mango(int Rango)
    {
        switch (Rango)
        {
            case 1:
                AumentarVelocidad(.4f);
                break;
            case 2:
                AumentarVelocidad(.4f);
                armas.AumentarDaño(2);
                break;
            case 3:
                AumentarVelocidad(.4f);
                armas.AumentarDaño(2);
                armas.AumentarDaño(4);
                break;
            case 4:
                AumentarVelocidad(.4f);
                armas.AumentarDaño(2);
                armas.AumentarDaño(4);
                break;
        }
    }
    public void Checar_Rango_Papa(int Rango)
    {
        switch (Rango)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }
    public void Checar_Rango_Planta(int Rango)
    {
        switch (Rango)
        {
            case 1:
                armas.AumentarArea(7);
                break;
            case 2:
                armas.AumentarArea(7);
                armas.AumentarAlcance(.8f);
                break;
            case 3:
                armas.AumentarArea(7);
                armas.AumentarAlcance(.8f);
                AumentarVelocidad(.4f);
                break;
            case 4:
                armas.AumentarArea(7);
                armas.AumentarAlcance(.8f);
                AumentarVelocidad(.4f);
                break;
        }
    }
    public void Checar_Rango_Platano(int Rango)
    {
        switch (Rango)
        {
            case 1:
                armas.AumentarDaño(.15f);
                break;
            case 2:
                armas.AumentarDaño(.15f);
                armas.AumentarCargador(5);
                break;
            case 3:
                armas.AumentarDaño(.15f);
                armas.AumentarCargador(5);
                armas.AumentarCadencia(1);
                break;
            case 4:
                armas.AumentarDaño(.15f);
                armas.AumentarCargador(5);
                armas.AumentarCadencia(1);
                break;
        }
    }
    public void Checar_Rango_Sandia(int Rango)
    {
        switch (Rango)
        {
            case 1:
                armas.AumentarDaño(.3f);
                break;
            case 2:
                armas.AumentarDaño(.3f);
                AumentarVelocidad(.3f);
                break;
            case 3:
                armas.AumentarDaño(.3f);
                AumentarVelocidad(.3f);
                armas.AumentarAlcance(.5f);
                break;
            case 4:
                armas.AumentarDaño(.3f);
                AumentarVelocidad(.3f);
                armas.AumentarAlcance(.5f);
                break;
        }
    }
    public void Checar_Rango_Tomate(int Rango)
    {
        switch (Rango)
        {
            case 1:
                armas.AumentarVelProyectil(.5f);
                break;
            case 2:
                armas.AumentarVelProyectil(.5f);
                armas.DisminuirDispersion(.5f);
                break;
            case 3:
                armas.AumentarVelProyectil(.5f);
                armas.DisminuirDispersion(.5f);
                armas.AumentarProyectil(.5f);
                break;
            case 4:
                armas.AumentarVelProyectil(.5f);
                armas.DisminuirDispersion(.5f);
                armas.AumentarProyectil(.5f);
                break;
        }
    }
    public void Checar_Rango_Uva(int Rango)
    {
        switch (Rango)
        {
            case 1:
                armas.AumentarDaño(.15f);
                break;
            case 2:
                armas.AumentarDaño(.15f);
                armas.AumentarProyectil(.5f);
                break;
            case 3:
                armas.AumentarDaño(.15f);
                armas.AumentarProyectil(.5f);
                armas.AumentarCargador(5);
                break;
            case 4:
                armas.AumentarDaño(.15f);
                armas.AumentarProyectil(.5f);
                armas.AumentarCargador(5);
                break;
        }
    }
}
