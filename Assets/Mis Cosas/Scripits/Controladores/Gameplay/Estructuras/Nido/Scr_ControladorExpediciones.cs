using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorExpediciones : MonoBehaviour
{
    // =========================================================
    // VARIABLES PERSISTENTES
    // =========================================================

    public bool TieneHuevo;
    public int EtapaHuevo;

    public bool Eclosiono;
    public int EtapaGrifo;



    // =========================================================
    // COMIDAS PARA EVOLUCIONAR
    // =========================================================

    public List<int> CantidadComidasRequeridas;
    public List<Scr_CreadorObjetos> ComidasRequeridasParaEvo;

    // Cantidad de comida que ya está dentro del comedero
    public int ComidaDepositada;


    // =========================================================
    // BARRA DE PROGRESO
    // =========================================================

    [Header("Canvas Progreso")]

    [SerializeField] GameObject CanvasProgreso;
    [SerializeField] TextMeshProUGUI TextoTiempoFaltante;
    [SerializeField] TextMeshProUGUI TextoPorcentajeFaltante;
    [SerializeField] Image Barra;


    // =========================================================
    // MODELOS DEL GRIFO
    // =========================================================

    [Header("Modelos Grifo")]

    [SerializeField] GameObject GrifoChico;
    [SerializeField] GameObject Grifovolador;
    [SerializeField] GameObject GrifoMediano;
    [SerializeField] GameObject GrifoGrande;


    // =========================================================
    // EVOLUCIÓN / COMEDERO
    // =========================================================

    [Header("Comedero")]

    [SerializeField] GameObject Comedero;
    [SerializeField] Image BarraComedero;
    [SerializeField] GameObject Botonx10;
    [SerializeField] Image IconoComidaRequeridaParaEvo;
    private GameObject CanvasComedero;
    private TextMeshProUGUI TextoCantidadFrutaComedero;


    // =========================================================
    // COLLIDER / ACTIVADOR
    // =========================================================

    [Header("Activador de comida")]

    [SerializeField] BoxCollider BoxColliderComedero;

    [SerializeField] Scr_ActivadorMenuEstructuraFijo ActivadorMenuEstructuraFijo;

    // =========================================================
    // EXPEDICIONES
    // =========================================================

    [Header("Expediciones")]

    [SerializeField] GameObject ObjetoExpedicion;
    [SerializeField] Scr_CreadorObjetos ComidaExpedicionBatalla;
    [SerializeField] int CantidadRequeridaBatalla;
    [SerializeField] int DuracionExpedicionBatalla;
    [SerializeField] Scr_CreadorObjetos ComidaExpedicionMineral;
    [SerializeField] int CantidadRequeridaMineral;
    [SerializeField] int DuracionExpedicionMineral;
    [SerializeField] Scr_CreadorObjetos ComidaExpedicionNatural;
    [SerializeField] int CantidadRequeridaNatural;
    [SerializeField] int DuracionExpedicionNatural;

    [SerializeField] Image ImagenComida;
    [SerializeField] TextMeshProUGUI TextoCantidadComida;
    [SerializeField] TextMeshProUGUI TextoCapacidad;


    [SerializeField] Scr_CreadorObjetos[] RecompensasBatalla;
    [SerializeField] int[] ProbabilidadesBatalla;
    [SerializeField] Scr_CreadorObjetos[] RecompensasMineral;
    [SerializeField] int[] ProbabilidadesMineral;
    [SerializeField] Scr_CreadorObjetos[] RecompensasNatural;
    [SerializeField] int[] ProbabilidadesNatural;

    [SerializeField] Transform PadreRecompensas;
    [SerializeField] GameObject PrefabRecompensa;

    [SerializeField] TextMeshProUGUI TextoTiempoExpedicion;

    [SerializeField] GameObject ObjetoRecompensa;
    private GameObject CanvasExpedicion;
    private bool BanderaCanvasExpeciones = false;


    // =========================================================
    // CONFIGURACIÓN DE UI EXPEDICIONES
    // =========================================================

    [Header("Botones Expediciones")]

    [SerializeField] Button BotonExpedicionBatalla;
    [SerializeField] Button BotonExpedicionMineral;
    [SerializeField] Button BotonExpedicionNatural;
    [SerializeField] Button BotonEnviar;


    // =========================================================
    // GRIFO DURANTE EXPEDICIÓN
    // =========================================================

    [Header("Animación Grifo Expedición")]

    [SerializeField] Animator AnimatorGrifoGrande;

    // =========================================================
    // ESTADO DE EXPEDICIÓN
    // =========================================================

    // 0 = batalla
    // 1 = mineral
    // 2 = natural
    private int TipoExpedicionActual;

    private bool RecompensasPendientes;

    private int TipoExpedicionPendiente;

    private int[] CantidadesRecompensasPendientes;

    private int CapacidadGrifoExpedicion = 20;

    // Tipos de expedición
    private const int EXPEDICION_BATALLA = 0;
    private const int EXPEDICION_MINERAL = 1;
    private const int EXPEDICION_NATURAL = 2;


    // Cantidad máxima de objetos que puede traer
    private const int MAX_OBJETOS_EXPEDICION = 20;


    // Cantidad máxima de recompensas diferentes que soportaremos
    // para guardar en PlayerPrefs.
    private const int MAX_RECOMPENSAS_GUARDADAS = 50;


    // =========================================================
    // DATOS EXTRA
    // =========================================================

    Scr_Inventario Inventario;
    Scr_ControladorTiempo Tiempo;

    private GameObject Nido;
    private GameObject Huevo1;
    private GameObject Huevo2;
    private GameObject Huevo3;

    // =========================================================
    // ESTADO ACTUAL DE LA CUENTA
    // =========================================================

    private bool CuentaActiva;

    // Primera aparición del huevo
    private bool EsperandoPrimerHuevo;
    private bool SonidoPrimerHuevoReproducido;
    private AudioSource AudioPrimerHuevo;

    [Header("Audio Grifo")]
    [SerializeField] private AudioSource AudioGrifo; // arrastra un AudioSource del grifo
    [SerializeField] AudioClip[] Sonidos;
    private Coroutine RutinaSonidoGrifo;

    // 0 = huevo
    // 1 = evolución del grifo
    private int TipoCuenta;

    private int DuracionCuentaMinutos;

    private int SemanaFinal;
    private int DiaFinal;
    private int HoraFinal;
    private int MinutoFinal;


    // =========================================================
    // CONFIGURACIÓN DE TIEMPOS
    // =========================================================

    [Header("Tiempo de eclosión")]

    [SerializeField] int DiasEclosion = 1;
    [SerializeField] int HorasEclosion = 0;
    [SerializeField] int MinutosEclosion = 0;

    [Header("Tiempo entre evoluciones")]

    [SerializeField] int DiasEvolucion = 0;
    [SerializeField] int HorasEvolucion = 12;
    [SerializeField] int MinutosEvolucion = 0;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        CanvasExpedicion =
            ObjetoExpedicion.transform.GetChild(0).gameObject;

        CanvasComedero =
            Comedero.transform.GetChild(0).gameObject;

        TextoCantidadFrutaComedero =
            CanvasComedero.transform
                .GetChild(4)
                .GetChild(0)
                .GetComponent<TextMeshProUGUI>();

        Inventario =
            GameObject.Find("Gata")
                .transform
                .GetChild(7)
                .GetComponent<Scr_Inventario>();

        AudioPrimerHuevo = GetComponent<AudioSource>();

        Nido =
            transform.GetChild(2).gameObject;

        Huevo1 = Nido.transform.GetChild(0).gameObject;
        Huevo2 = Nido.transform.GetChild(1).gameObject;
        Huevo3 = Nido.transform.GetChild(2).gameObject;

        Tiempo =
            GameObject.Find("Controlador Tiempo")
                .GetComponent<Scr_ControladorTiempo>();


        CargarVariables();
        ActualizarBarraComedero();
        InicializarEstado();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        // Primera aparición del huevo
        if (EsperandoPrimerHuevo)
        {
            ComprobarPrimerHuevo();
        }

        // Cuenta regresiva
        if (CuentaActiva)
        {
            ActualizarCuenta();
        }

        // Actualizar botón x10
        if (Eclosiono && !CuentaActiva && EtapaGrifo < 3)
        {
            ActualizarBotonX10();
        }



        if (CanvasExpedicion != null && CanvasExpedicion.gameObject.activeSelf)
        {
            if (!BanderaCanvasExpeciones)
            {
                BanderaCanvasExpeciones = true;
                TerminandoExpedicion();

            }
        }
        else
        {
            BanderaCanvasExpeciones = false;
        }
    }

    private void CrearPrimerHuevo()
    {
        if (!EsperandoPrimerHuevo)
            return;

        EsperandoPrimerHuevo = false;

        TieneHuevo = true;
        Eclosiono = false;
        EtapaHuevo = 1;

        // Guardamos inmediatamente que el huevo ya apareció.
        PlayerPrefs.SetInt("GrifoEsperandoPrimerHuevo", 0);

        GuardarVariables();

        // Activar el primer modelo del huevo.
        AplicarEtapaHuevo(1, true);

        // Reproducir sonido solamente la primera vez.
        if (!SonidoPrimerHuevoReproducido)
        {
            if (AudioPrimerHuevo != null)
            {
                AudioPrimerHuevo.Play();
            }

            SonidoPrimerHuevoReproducido = true;

            PlayerPrefs.SetInt("GrifoSonidoPrimerHuevo", 1);
            PlayerPrefs.Save();
        }

        // Comenzar cuenta regresiva.
        IniciarCuentaHuevo();
    }

    private void ComprobarPrimerHuevo()
    {
        if (Tiempo == null)
            return;

        // El huevo aparece al llegar a las 20:00.
        if (Tiempo.HoraActual < 20)
            return;

        CrearPrimerHuevo();
    }


    // =========================================================
    // INICIALIZAR
    // =========================================================

    private void InicializarEstado()
    {
        bool primeraPartida = !PlayerPrefs.HasKey("GrifoTieneHuevo");

        if (primeraPartida)
        {
            // El juego comienza sin huevo.
            TieneHuevo = false;
            Eclosiono = false;
            EtapaHuevo = 0;
            EtapaGrifo = 0;
            ComidaDepositada = 0;

            EsperandoPrimerHuevo = true;

            GuardarVariables();

            // El nido debe comenzar vacío.
            AplicarEtapaHuevo(0, false);

            if (Nido != null)
                Nido.SetActive(true);

            // Aseguramos que no haya cuenta regresiva todavía.
            CuentaActiva = false;
            CanvasProgreso.SetActive(false);

            DesactivarSistemaComida();

            return;
        }

        // -----------------------------
        // PARTIDA YA INICIADA
        // -----------------------------

        if (!TieneHuevo && !Eclosiono)
        {
            EsperandoPrimerHuevo = true;

            AplicarEtapaHuevo(0, false);

            CanvasProgreso.SetActive(false);

            return;
        }

        if (TieneHuevo && !Eclosiono)
        {
            AplicarEtapaHuevo(EtapaHuevo, true);

            if (CuentaActiva)
            {
                CanvasProgreso.SetActive(true);
                DesactivarSistemaComida();
            }
            else
            {
                IniciarCuentaHuevo();
            }

            return;
        }

        // -----------------------------
        // GRIFO YA ECLOSIONADO
        // -----------------------------

        if (Eclosiono)
        {
            // Cargar información de la expedición guardada
            CargarDatosExpedicion();

            AplicarEtapaGrifo();

            if (EtapaGrifo >= 3)
            {
                // -----------------------------------------------------
                // EL GRIFO ESTÁ EN UNA EXPEDICIÓN
                // -----------------------------------------------------

                if (CuentaActiva && TipoCuenta == 2)
                {
                    RestaurarExpedicionEnCurso();
                }
                else
                {
                    PrepararExpediciones();
                }
            }
            else if (CuentaActiva)
            {
                CanvasProgreso.SetActive(true);
                DesactivarSistemaComida();
            }
            else
            {
                PrepararComedero();
            }
        }
    }


    // =========================================================
    // INICIAR CUENTA DE HUEVO
    // =========================================================

    public void IniciarCuentaHuevo()
    {
        int minutos = ConvertirAMinutos(
            DiasEclosion,
            HorasEclosion,
            MinutosEclosion
        );

        IniciarCuenta(minutos, 0);
    }


    // =========================================================
    // INICIAR CUENTA DE EVOLUCIÓN
    // =========================================================

    public void IniciarCuentaEvolucion()
    {

        int minutos = ConvertirAMinutos(
            DiasEvolucion,
            HorasEvolucion,
            MinutosEvolucion
        );

        IniciarCuenta(minutos, 1);
    }


    // =========================================================
    // FUNCIÓN GENERAL DE CUENTA ATRÁS
    // =========================================================

    private void IniciarCuenta(int minutos, int tipo)
    {
        if (minutos <= 0)
        {
            TerminarCuenta();

            return;
        }

        CuentaActiva = true;

        TipoCuenta = tipo;

        DuracionCuentaMinutos = minutos;


        // Obtener tiempo actual

        int tiempoActual =
            ObtenerMinutosTotales();


        // Calcular tiempo final

        int tiempoFinal =
            tiempoActual + minutos;


        // Convertir a semana/día/hora/minuto

        SemanaFinal =
            tiempoFinal / (7 * 24 * 60);

        int resto =
            tiempoFinal % (7 * 24 * 60);

        DiaFinal =
            resto / (24 * 60);

        resto %= (24 * 60);

        HoraFinal =
            resto / 60;

        MinutoFinal =
            resto % 60;


        GuardarCuenta();


        // Mostrar barra

        CanvasProgreso.SetActive(true);

        CanvasComedero.SetActive(false);


        // Actualizar inmediatamente

        ActualizarCuenta();
    }


    // =========================================================
    // ACTUALIZAR CUENTA
    // =========================================================

    private void ActualizarCuenta()
    {
        int actual =
            ObtenerMinutosTotales();

        int final =
            ObtenerMinutosFinales();

        int restantes =
            final - actual;


        if (restantes < 0)
        {
            restantes = 0;
        }


        // -----------------------------------------------------
        // TEXTO
        // -----------------------------------------------------

        ActualizarTextoTiempo(restantes);


        // -----------------------------------------------------
        // PORCENTAJE
        // -----------------------------------------------------

        float porcentaje = 0f;

        if (DuracionCuentaMinutos > 0)
        {
            porcentaje =
                1f -
                ((float)restantes /
                 DuracionCuentaMinutos);
        }

        porcentaje =
            Mathf.Clamp01(porcentaje);


        Barra.fillAmount =
            porcentaje;

        TextoPorcentajeFaltante.text =
            Mathf.RoundToInt(
                porcentaje * 100f
            ) + "%";


        // -----------------------------------------------------
        // SI TERMINÓ
        // -----------------------------------------------------

        if (restantes <= 0)
        {
            TerminarCuenta();
        }
        else
        {
            if (TipoCuenta == 0)
            {
                ActualizarEtapaHuevoPorTiempo(restantes);
            }
        }
    }


    // =========================================================
    // TERMINAR CUENTA
    // =========================================================

    private void TerminarCuenta()
    {
        CuentaActiva = false;

        GuardarCuenta();


        // -----------------------------------------------------
        // CUENTA DEL HUEVO
        // -----------------------------------------------------

        if (TipoCuenta == 0)
        {
            EclosionarHuevo();

            return;
        }


        // -----------------------------------------------------
        // CUENTA DE EVOLUCIÓN
        // -----------------------------------------------------

        if (TipoCuenta == 1)
        {
            CompletarEvolucion();

            return;
        }


        // -----------------------------------------------------
        // CUENTA DE EXPEDICIÓN
        // -----------------------------------------------------

        if (TipoCuenta == 2)
        {
            CompletarExpedicion();

            return;
        }
    }


    // =========================================================
    // ETAPAS DEL HUEVO
    // =========================================================

    private void ActualizarEtapaHuevoPorTiempo(int minutosRestantes)
    {
        if (DuracionCuentaMinutos <= 0)
            return;

        float porcentajeRestante =
            (float)minutosRestantes / DuracionCuentaMinutos;

        int nuevaEtapa;

        // Más de 2/3 del tiempo restante = huevo 1
        if (porcentajeRestante > (2f / 3f))
        {
            nuevaEtapa = 1;
        }
        // Más de 1/3 y hasta 2/3 = huevo 2
        else if (porcentajeRestante > (1f / 3f))
        {
            nuevaEtapa = 2;
        }
        // 1/3 o menos = huevo 3
        else
        {
            nuevaEtapa = 3;
        }

        if (EtapaHuevo != nuevaEtapa)
        {
            EtapaHuevo = nuevaEtapa;

            AplicarEtapaHuevo(EtapaHuevo, true);

            PlayerPrefs.SetInt("GrifoEtapaHuevo", EtapaHuevo);
            PlayerPrefs.Save();
        }
    }


    // =========================================================
    // APLICAR MODELO DEL HUEVO
    // =========================================================

    private void AplicarEtapaHuevo(int etapa, bool activar)
    {
        if (Huevo1 != null)
            Huevo1.SetActive(false);

        if (Huevo2 != null)
            Huevo2.SetActive(false);

        if (Huevo3 != null)
            Huevo3.SetActive(false);

        if (!activar)
            return;

        bool aparecioAhora = false;
        GameObject huevoActivado = null;

        switch (etapa)
        {
            case 1:
                if (Huevo1 != null)
                {
                    if (!Huevo1.activeSelf) aparecioAhora = true;
                    huevoActivado = Huevo1;
                    Huevo1.SetActive(true);
                }
                break;

            case 2:
                if (Huevo2 != null)
                {
                    if (!Huevo2.activeSelf) aparecioAhora = true;
                    huevoActivado = Huevo2;
                    Huevo2.SetActive(true);
                }
                break;

            case 3:
                if (Huevo3 != null)
                {
                    if (!Huevo3.activeSelf) aparecioAhora = true;
                    huevoActivado = Huevo3;
                    Huevo3.SetActive(true);
                }
                break;
        }

        // Primer sonido del huevo en 2D aunque tu AudioSource esté en 3D (Spatial Blend = 1)
        if (aparecioAhora && Sonidos != null && Sonidos.Length > 0 && Sonidos[0] != null)
        {
            if (AudioPrimerHuevo != null)
            {
                // Guarda el valor original para regresarlo después
                float blendOriginal = AudioPrimerHuevo.spatialBlend;

                // Lo ponemos en 2D solo para este grito
                AudioPrimerHuevo.spatialBlend = 0f;

                AudioPrimerHuevo.PlayOneShot(Sonidos[0]);

                // Lo regresamos a 3D cuando termine el clip
                StartCoroutine(RegresarA3D(AudioPrimerHuevo, blendOriginal, Sonidos[0].length));
            }
        }
    }

    private IEnumerator RegresarA3D(AudioSource source, float valorOriginal, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (source != null)
        {
            source.spatialBlend = valorOriginal; // vuelve a 1
        }
    }


    // =========================================================
    // ECLOSIONAR
    // =========================================================

    private void EclosionarHuevo()
    {
        Eclosiono = true;

        EtapaGrifo = 0;

        CuentaActiva = false;

        // Primera etapa comienza con 0 comida.
        ComidaDepositada = 0;


        GuardarVariables();


        // Activar grifo chico

        AplicarEtapaGrifo();


        // Cambiar interfaces

        CanvasProgreso.SetActive(false);

        CanvasComedero.SetActive(true);


        // Activar sistema de alimentación

        ActivarSistemaComida();


        // Mostrar 0 / cantidad requerida

        ActualizarTextoCantidadComida();

        ActualizarIconoComida();

        ActualizarBotonX10();
    }


    // =========================================================
    // APLICAR MODELO DEL GRIFO
    // =========================================================

    private void AplicarEtapaGrifo()
    {
        if (GrifoChico != null) GrifoChico.SetActive(false);
        if (Grifovolador != null) Grifovolador.SetActive(false);
        if (GrifoMediano != null) GrifoMediano.SetActive(false);
        if (GrifoGrande != null) GrifoGrande.SetActive(false);

        DetenerSonidoGrifo();

        switch (EtapaGrifo)
        {
            case 0:
                if (GrifoChico != null) GrifoChico.SetActive(true);
                if (Huevo1 != null) Huevo1.SetActive(false);
                if (Huevo2 != null) Huevo2.SetActive(false);
                if (Huevo3 != null) Huevo3.SetActive(false);
                if (Nido != null) Nido.SetActive(true);
                IniciarSonidoGrifo(1); // Sonidos[1] chico
                break;

            case 1: // volador - SIN SONIDO
                if (Grifovolador != null) Grifovolador.SetActive(true);
                if (Nido != null) Nido.SetActive(true);
                // no sonido
                break;

            case 2:
                if (GrifoMediano != null) GrifoMediano.SetActive(true);
                if (Nido != null) Nido.SetActive(false);
                IniciarSonidoGrifo(2); // Sonidos[2] mediano
                break;

            case 3:
                if (GrifoGrande != null) GrifoGrande.SetActive(true);
                if (CanvasProgreso != null) CanvasProgreso.SetActive(false);
                if (Nido != null) Nido.SetActive(false);
                IniciarSonidoGrifo(3); // Sonidos[3] grande
                break;
        }
    }

    private void IniciarSonidoGrifo(int indiceSonido)
    {
        if (Sonidos == null || Sonidos.Length <= indiceSonido || Sonidos[indiceSonido] == null) return;
        if (AudioGrifo == null) return;

        DetenerSonidoGrifo();
        RutinaSonidoGrifo = StartCoroutine(LoopSonidoGrifo(Sonidos[indiceSonido]));
    }

    private void DetenerSonidoGrifo()
    {
        if (RutinaSonidoGrifo != null)
        {
            StopCoroutine(RutinaSonidoGrifo);
            RutinaSonidoGrifo = null;
        }
        if (AudioGrifo != null && AudioGrifo.isPlaying)
            AudioGrifo.Stop();
    }

    private IEnumerator LoopSonidoGrifo(AudioClip clip)
    {
        if (clip == null) yield break;

        while (true)
        {
            // 1. Reproducir el sonido
            if (AudioGrifo != null)
            {
                AudioGrifo.PlayOneShot(clip);
            }

            // 2. Esperar a que TERMINE el clip
            yield return new WaitForSeconds(clip.length);

            // 3. Ahora sí esperar tiempo aleatorio de 1 a 5 segundos
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }


    // =========================================================
    // PREPARAR COMEDERO
    // =========================================================

    private void PrepararComedero()
    {
        CanvasProgreso.SetActive(false);

        CanvasComedero.SetActive(true);

        ActivarSistemaComida();

        ActualizarIconoComida();

        ActualizarTextoCantidadComida();

        ActualizarBotonX10();
    }


    // =========================================================
    // ACTIVAR SISTEMA DE COMIDA
    // =========================================================

    private void ActivarSistemaComida()
    {
        if (EtapaGrifo >= 3)
        {
            DesactivarSistemaComida();

            return;
        }


        Comedero.SetActive(true);


        if (BoxColliderComedero != null)
        {
            BoxColliderComedero.enabled = true;
        }


        if (ActivadorMenuEstructuraFijo != null)
        {
            ActivadorMenuEstructuraFijo.enabled = true;
        }
    }


    // =========================================================
    // DESACTIVAR SISTEMA DE COMIDA
    // =========================================================

    private void DesactivarSistemaComida()
    {
        CanvasComedero.SetActive(false);

        Comedero.SetActive(false);


        if (BoxColliderComedero != null)
        {
            BoxColliderComedero.enabled = false;
        }


        if (ActivadorMenuEstructuraFijo != null)
        {
            ActivadorMenuEstructuraFijo.enabled = false;
        }
    }


    // =========================================================
    // ACTUALIZAR ICONO DE COMIDA
    // =========================================================

    private void ActualizarIconoComida()
    {
        if (EtapaGrifo >=
            ComidasRequeridasParaEvo.Count)
        {
            return;
        }


        if (ComidasRequeridasParaEvo[EtapaGrifo] == null)
        {
            return;
        }


        IconoComidaRequeridaParaEvo.sprite =
            ComidasRequeridasParaEvo[
                EtapaGrifo
            ].Icono;
    }


    // =========================================================
    // ACTUALIZAR TEXTO DE CANTIDAD
    // =========================================================

    private void ActualizarTextoCantidadComida()
    {
        if (TextoCantidadFrutaComedero == null)
        {
            return;
        }


        if (EtapaGrifo >=
            CantidadComidasRequeridas.Count)
        {
            TextoCantidadFrutaComedero.text = "0 / 0";

            return;
        }


        int cantidadNecesaria =
            CantidadComidasRequeridas[
                EtapaGrifo
            ];


        // Nunca mostrar una cantidad superior
        // a la requerida.

        int cantidadMostrar =
            Mathf.Clamp(
                ComidaDepositada,
                0,
                cantidadNecesaria
            );


        TextoCantidadFrutaComedero.text =
            cantidadMostrar +
            " / " +
            cantidadNecesaria;
    }

    private void ActualizarBarraComedero()
    {
        if (BarraComedero == null) return;

        int requerida = 0;
        if (EtapaGrifo < 3)
        {
            requerida = CantidadComidasRequeridas[EtapaGrifo]; // o la variable que uses para el huevo
        }

        if (requerida <= 0)
        {
            BarraComedero.fillAmount = 0f;
            return;
        }

        float progreso = (float)ComidaDepositada / requerida;
        BarraComedero.fillAmount = Mathf.Clamp01(progreso);
    }


    // =========================================================
    // DEPOSITAR FRUTA
    // =========================================================


    public void DepositarFruta()
    {
        // Comprobar que tenemos una fruta requerida para la etapa actual
        if (EtapaGrifo < 0 || EtapaGrifo >= ComidasRequeridasParaEvo.Count)
            return;

        string nombreObjeto = ComidasRequeridasParaEvo[EtapaGrifo].Nombre;

        if (string.IsNullOrEmpty(nombreObjeto))
            return;

        // Buscar la fruta dentro del inventario
        bool tieneFruta = false;

        if (Inventario != null &&
            Inventario.Objetos != null &&
            Inventario.Cantidades != null)
        {
            int cantidadElementos = Mathf.Min(
                Inventario.Objetos.Length,
                Inventario.Cantidades.Length
            );

            for (int i = 0; i < cantidadElementos; i++)
            {
                if (Inventario.Objetos[i] == null)
                    continue;

                if (Inventario.Objetos[i].Nombre != nombreObjeto)
                    continue;

                // Tenemos al menos 1 fruta disponible
                if (Inventario.Cantidades[i] >= 1)
                {
                    tieneFruta = true;
                }

                break;
            }
        }

        // Si no tenemos la fruta, no hacemos absolutamente nada
        if (!tieneFruta)
            return;

        // Ahora sí podemos quitar 1 fruta del inventario
        Inventario.QuitarObjeto(nombreObjeto, 1, true);

        BarraComedero.fillAmount = ComidaDepositada / CantidadComidasRequeridas[EtapaGrifo];
        // Aumentar la cantidad depositada
        ComidaDepositada++;

        // Guardar el progreso
        PlayerPrefs.SetInt("GrifoComidaDepositada", ComidaDepositada);
        PlayerPrefs.Save();

        // Actualizar el texto de la UI
        ActualizarTextoCantidadComida();

        ActualizarBarraComedero();

        // Comprobar si ya se completó la cantidad necesaria
        if (EtapaGrifo < CantidadComidasRequeridas.Count)
        {
            int cantidadNecesaria = CantidadComidasRequeridas[EtapaGrifo];

            if (ComidaDepositada >= cantidadNecesaria)
            {
                ComidaCompleta();
            }
        }
    }

    // =========================================================
    // DEPOSITAR 10 FRUTAS
    // =========================================================

    public void DepositarFrutaX10()
    {
        // Comprobar que existe una comida para la etapa actual
        if (EtapaGrifo < 0 || EtapaGrifo >= ComidasRequeridasParaEvo.Count)
            return;

        string nombreObjeto = ComidasRequeridasParaEvo[EtapaGrifo].Nombre;

        if (string.IsNullOrEmpty(nombreObjeto))
            return;

        // Comprobar que tenemos al menos 10 frutas
        bool tieneDiezFrutas = false;

        if (Inventario != null &&
            Inventario.Objetos != null &&
            Inventario.Cantidades != null)
        {
            int cantidadElementos = Mathf.Min(
                Inventario.Objetos.Length,
                Inventario.Cantidades.Length
            );

            for (int i = 0; i < cantidadElementos; i++)
            {
                if (Inventario.Objetos[i] == null)
                    continue;

                if (Inventario.Objetos[i].Nombre != nombreObjeto)
                    continue;

                if (Inventario.Cantidades[i] >= 10)
                {
                    tieneDiezFrutas = true;
                }

                break;
            }
        }

        // Si no tenemos 10, no hacemos nada
        if (!tieneDiezFrutas)
            return;

        // Comprobar cuánto falta para completar el comedero
        if (EtapaGrifo >= CantidadComidasRequeridas.Count)
            return;

        int cantidadNecesaria =
            CantidadComidasRequeridas[EtapaGrifo];

        int cantidadFaltante =
            cantidadNecesaria - ComidaDepositada;

        // Si faltan menos de 10, no depositamos 10
        if (cantidadFaltante < 10)
            return;

        // Quitar 10 frutas del inventario
        Inventario.QuitarObjeto(
            nombreObjeto,
            10,
            true
        );

        // Agregar 10 al comedero
        ComidaDepositada += 10;

        // Guardar progreso
        PlayerPrefs.SetInt(
            "GrifoComidaDepositada",
            ComidaDepositada
        );

        PlayerPrefs.Save();

        ActualizarBarraComedero();

        // Actualizar UI
        ActualizarTextoCantidadComida();

        // Actualizar estado del botón x10
        ActualizarBotonX10();

        // Comprobar si se completó la comida
        if (ComidaDepositada >= cantidadNecesaria)
        {
            ComidaCompleta();
        }
    }

    // =========================================================
    // ACTUALIZAR BOTÓN X10
    // =========================================================

    private void ActualizarBotonX10()
    {
        if (Botonx10 == null)
            return;

        // Si no existe una comida para esta etapa,
        // desactivar botón.
        if (EtapaGrifo < 0 ||
            EtapaGrifo >= ComidasRequeridasParaEvo.Count)
        {
            Botonx10.SetActive(false);
            return;
        }

        string nombreObjeto =
            ComidasRequeridasParaEvo[EtapaGrifo].Nombre;

        if (string.IsNullOrEmpty(nombreObjeto))
        {
            Botonx10.SetActive(false);
            return;
        }

        bool tieneDiez = false;

        if (Inventario != null &&
            Inventario.Objetos != null &&
            Inventario.Cantidades != null)
        {
            int cantidadElementos = Mathf.Min(
                Inventario.Objetos.Length,
                Inventario.Cantidades.Length
            );

            for (int i = 0; i < cantidadElementos; i++)
            {
                if (Inventario.Objetos[i] == null)
                    continue;

                if (Inventario.Objetos[i].Nombre != nombreObjeto)
                    continue;

                if (Inventario.Cantidades[i] >= 10)
                {
                    tieneDiez = true;
                }

                break;
            }
        }

        // También comprobar que todavía falten
        // al menos 10 para completar.
        bool PuedeDepositarDiez = false;

        if (EtapaGrifo < CantidadComidasRequeridas.Count)
        {
            int cantidadNecesaria =
                CantidadComidasRequeridas[EtapaGrifo];

            int cantidadFaltante =
                cantidadNecesaria - ComidaDepositada;

            PuedeDepositarDiez =
                cantidadFaltante >= 10;
        }

        Botonx10.SetActive(
            tieneDiez && PuedeDepositarDiez
        );
    }


    // =========================================================
    // RETIRAR FRUTA
    // =========================================================


    public void RetirarFruta()
    {
        // No hay comida almacenada para retirar
        if (ComidaDepositada <= 0)
            return;

        if (EtapaGrifo < 0 || EtapaGrifo >= ComidasRequeridasParaEvo.Count)
            return;

        string nombreObjeto = ComidasRequeridasParaEvo[EtapaGrifo].Nombre;

        if (string.IsNullOrEmpty(nombreObjeto))
            return;

        // Devolver 1 fruta al inventario
        int cantidadAgregada = Inventario.AgregarObjeto(
            nombreObjeto,
            1,
            true,
            false
        );

        // AgregarObjeto devuelve la cantidad que realmente pudo agregar.
        // Si devuelve 0, el inventario está lleno y no retiramos la fruta.
        if (cantidadAgregada <= 0)
            return;

        // Ahora sí retiramos 1 fruta del comedero
        ComidaDepositada--;

        // Guardar el progreso
        PlayerPrefs.SetInt("GrifoComidaDepositada", ComidaDepositada);
        PlayerPrefs.Save();

        ActualizarBarraComedero();

        // Actualizar el texto
        ActualizarTextoCantidadComida();

        ActualizarBotonX10();

        // Si estaba completa y retiramos comida,
        // debemos volver a mostrar el comedero.
        if (EtapaGrifo < CantidadComidasRequeridas.Count)
        {
            int cantidadNecesaria = CantidadComidasRequeridas[EtapaGrifo];

            if (ComidaDepositada < cantidadNecesaria)
            {
                CanvasComedero.SetActive(true);
                BoxColliderComedero.enabled = true;
                ActivadorMenuEstructuraFijo.enabled = true;

                CanvasProgreso.SetActive(false);
            }
        }
    }



    // =========================================================
    // TODA LA COMIDA FUE ENTREGADA
    // =========================================================

    private void ComidaCompleta()
    {
        StartCoroutine(ComidaCompletaCoroutine());
    }

    private IEnumerator ComidaCompletaCoroutine()
    {
        // Actualizar una última vez
        ActualizarTextoCantidadComida();

        // Cerrar tablero y comenzar restauración de transparencia
        if (ActivadorMenuEstructuraFijo != null)
        {
            ActivadorMenuEstructuraFijo.EstaDentro = false;
            ActivadorMenuEstructuraFijo.EstaEnRango = false;
            ActivadorMenuEstructuraFijo.CerrarTablero();
        }

        // Esperar a que termine la restauración visual
        yield return new WaitForSeconds(0.21f);

        // Mostrar progreso
        CanvasProgreso.SetActive(true);

        // Ahora sí desactivar el sistema de comida
        if (BoxColliderComedero != null)
        {
            BoxColliderComedero.enabled = false;
        }

        if (ActivadorMenuEstructuraFijo != null)
        {
            ActivadorMenuEstructuraFijo.enabled = false;
        }

        // Comenzar tiempo de evolución
        IniciarCuentaEvolucion();
    }


    // =========================================================
    // COMPLETAR EVOLUCIÓN
    // =========================================================

    private void CompletarEvolucion()
    {
        CuentaActiva = false;


        // Subir etapa

        EtapaGrifo++;


        if (EtapaGrifo > 3)
        {
            EtapaGrifo = 3;
        }


        // La nueva etapa comienza con 0 comida.

        ComidaDepositada = 0;
        ActualizarBarraComedero();

        GuardarVariables();


        // -----------------------------------------------------
        // ¿YA ES ADULTO?
        // -----------------------------------------------------

        if (EtapaGrifo >= 3)
        {
            AplicarEtapaGrifo();

            PrepararExpediciones();

            return;
        }


        // -----------------------------------------------------
        // TODAVÍA PUEDE EVOLUCIONAR
        // -----------------------------------------------------

        AplicarEtapaGrifo();

        PrepararComedero();
    }


    // =========================================================
    // PREPARAR EXPEDICIONES
    // =========================================================

    private void PrepararExpediciones()
    {
        // Desactivar alimentación
        DesactivarSistemaComida();

        // Activar objeto de expediciones
        ObjetoExpedicion.SetActive(true);

        // Canvas de expediciones oculto
        CanvasExpedicion.SetActive(false);

        // Mostrar indicador de recompensa si ya terminó
        if (ObjetoRecompensa != null)
        {
            ObjetoRecompensa.SetActive(RecompensasPendientes);
        }
    }


    // =========================================================
    // CONVERTIR DÍAS/HORAS/MINUTOS
    // =========================================================

    private int ConvertirAMinutos(
        int dias,
        int horas,
        int minutos)
    {
        return
            dias * 24 * 60 +
            horas * 60 +
            minutos;
    }


    // =========================================================
    // OBTENER TIEMPO ACTUAL
    // =========================================================

    private int ObtenerMinutosTotales()
    {
        int diaNumero =
            ObtenerNumeroDia(
                Tiempo.DiaActual
            );


        return
            Tiempo.SemanaActual *
                7 * 24 * 60 +

            diaNumero *
                24 * 60 +

            Tiempo.HoraActual *
                60 +

            (int)Tiempo.MinutoActual;
    }


    // =========================================================
    // OBTENER TIEMPO FINAL
    // =========================================================

    private int ObtenerMinutosFinales()
    {
        return
            SemanaFinal *
                7 * 24 * 60 +

            DiaFinal *
                24 * 60 +

            HoraFinal *
                60 +

            MinutoFinal;
    }


    // =========================================================
    // CONVERTIR DÍA DE LA SEMANA
    // =========================================================

    private int ObtenerNumeroDia(string dia)
    {
        switch (dia)
        {
            case "LUN":
                return 0;

            case "MAR":
                return 1;

            case "MIE":
                return 2;

            case "JUE":
                return 3;

            case "VIE":
                return 4;

            case "SAB":
                return 5;

            case "DOM":
                return 6;
        }


        return 0;
    }


    // =========================================================
    // TEXTO DE TIEMPO
    // =========================================================

    private void ActualizarTextoTiempo(int minutos)
    {
        int dias =
            minutos / (24 * 60);

        int resto =
            minutos % (24 * 60);

        int horas =
            resto / 60;

        int minutosRestantes =
            resto % 60;


        if (dias > 0)
        {
            TextoTiempoFaltante.text =
                dias + " d " +
                horas + " h " +
                minutosRestantes + " m";
        }
        else
        {
            TextoTiempoFaltante.text =
                horas + " h " +
                minutosRestantes + " m";
        }
    }


    // =========================================================
    // GUARDAR VARIABLES
    // =========================================================

    private void GuardarVariables()
    {
        PlayerPrefs.SetInt(
            "GrifoEsperandoPrimerHuevo",
            EsperandoPrimerHuevo ? 1 : 0
        );

        PlayerPrefs.SetInt(
            "GrifoSonidoPrimerHuevo",
            SonidoPrimerHuevoReproducido ? 1 : 0
        );

        PlayerPrefs.SetInt(
            "GrifoTieneHuevo",
            TieneHuevo ? 1 : 0
        );


        PlayerPrefs.SetInt(
            "GrifoEtapaHuevo",
            EtapaHuevo
        );


        PlayerPrefs.SetInt(
            "GrifoEclosiono",
            Eclosiono ? 1 : 0
        );


        PlayerPrefs.SetInt(
            "GrifoEtapa",
            EtapaGrifo
        );


        // =====================================================
        // GUARDAR COMIDA DEL COMEDERO
        // =====================================================

        PlayerPrefs.SetInt(
            "GrifoComidaDepositada",
            ComidaDepositada
        );


        PlayerPrefs.Save();
    }


    // =========================================================
    // GUARDAR CUENTA
    // =========================================================

    private void GuardarCuenta()
    {
        PlayerPrefs.SetInt(
            "GrifoCuentaActiva",
            CuentaActiva ? 1 : 0
        );


        PlayerPrefs.SetInt(
            "GrifoTipoCuenta",
            TipoCuenta
        );


        PlayerPrefs.SetInt(
            "GrifoDuracionCuenta",
            DuracionCuentaMinutos
        );


        PlayerPrefs.SetInt(
            "GrifoSemanaFinal",
            SemanaFinal
        );


        PlayerPrefs.SetInt(
            "GrifoDiaFinal",
            DiaFinal
        );


        PlayerPrefs.SetInt(
            "GrifoHoraFinal",
            HoraFinal
        );


        PlayerPrefs.SetInt(
            "GrifoMinutoFinal",
            MinutoFinal
        );


        PlayerPrefs.Save();
    }


    // =========================================================
    // CARGAR VARIABLES
    // =========================================================

    private void CargarVariables()
    {
        EsperandoPrimerHuevo =
            PlayerPrefs.GetInt("GrifoEsperandoPrimerHuevo", 0) == 1;

        SonidoPrimerHuevoReproducido =
            PlayerPrefs.GetInt("GrifoSonidoPrimerHuevo", 0) == 1;

        TieneHuevo =
            PlayerPrefs.GetInt(
                "GrifoTieneHuevo",
                0
            ) == 1;


        EtapaHuevo =
            PlayerPrefs.GetInt(
                "GrifoEtapaHuevo",
                1
            );


        Eclosiono =
            PlayerPrefs.GetInt(
                "GrifoEclosiono",
                0
            ) == 1;


        EtapaGrifo =
            PlayerPrefs.GetInt(
                "GrifoEtapa",
                0
            );


        // =====================================================
        // CARGAR COMIDA DEL COMEDERO
        // =====================================================

        ComidaDepositada =
            PlayerPrefs.GetInt(
                "GrifoComidaDepositada",
                0
            );


        // -----------------------------------------------------
        // CARGAR CUENTA
        // -----------------------------------------------------

        CuentaActiva =
            PlayerPrefs.GetInt(
                "GrifoCuentaActiva",
                0
            ) == 1;


        TipoCuenta =
            PlayerPrefs.GetInt(
                "GrifoTipoCuenta",
                0
            );


        DuracionCuentaMinutos =
            PlayerPrefs.GetInt(
                "GrifoDuracionCuenta",
                0
            );


        SemanaFinal =
            PlayerPrefs.GetInt(
                "GrifoSemanaFinal",
                0
            );


        DiaFinal =
            PlayerPrefs.GetInt(
                "GrifoDiaFinal",
                0
            );


        HoraFinal =
            PlayerPrefs.GetInt(
                "GrifoHoraFinal",
                0
            );


        MinutoFinal =
            PlayerPrefs.GetInt(
                "GrifoMinutoFinal",
                0
            );
    }

    // =========================================================
    // LÓGICA DE LA UI DE EXPEDICIONES
    // =========================================================


    // =========================================================
    // ABRIR UI DE EXPEDICIONES
    // =========================================================

    public void TerminandoExpedicion()
    {
        // =====================================================
        // INTENTAR ENTREGAR RECOMPENSAS
        // =====================================================

        EntregarRecompensasPendientes();

        // =====================================================
        // RESTO DE LA APERTURA NORMAL
        // =====================================================

        if (CuentaActiva && TipoCuenta == 2)
        {
            CanvasExpedicion.SetActive(false);
            CanvasProgreso.SetActive(true);

            return;
        }


        if (!Eclosiono || EtapaGrifo < 3)
            return;


        ObjetoExpedicion.SetActive(true);

        CanvasExpedicion.SetActive(true);

        CanvasProgreso.SetActive(false);

        ActualizarUIExpedicion();

        ActualizarBotonEnviar();
    }


    // =========================================================
    // CERRAR UI
    // =========================================================

    public void CerrarUIExpedicion()
    {
        if (CanvasExpedicion != null)
        {
            CanvasExpedicion.SetActive(false);
        }
    }


    // =========================================================
    // SELECCIONAR EXPEDICIÓN
    // =========================================================

    public void SeleccionarExpedicion(int tipo)
    {
        if (CuentaActiva && TipoCuenta == 2)
            return;

        // ESTA LÍNEA era la que bloqueaba que cambiaran las recompensas
        // Si quieres que SÍ deje cambiar aunque haya pendientes, bórrala o coméntala
        // if (RecompensasPendientes) return;

        if (tipo < EXPEDICION_BATALLA || tipo > EXPEDICION_NATURAL)
            return;

        TipoExpedicionActual = tipo;

        PlayerPrefs.SetInt("GrifoExpedicionSeleccionada", TipoExpedicionActual);
        PlayerPrefs.Save();

        // Forzar refresco completo
        ActualizarUIExpedicion();
        ActualizarBotonEnviar();

        Debug.Log("Expedición seleccionada: " + TipoExpedicionActual);
    }


    // =========================================================
    // BOTONES INDIVIDUALES
    // =========================================================

    public void SeleccionarExpedicionBatalla()
    {
        SeleccionarExpedicion(EXPEDICION_BATALLA);
    }


    public void SeleccionarExpedicionMineral()
    {
        SeleccionarExpedicion(EXPEDICION_MINERAL);
    }


    public void SeleccionarExpedicionNatural()
    {
        SeleccionarExpedicion(EXPEDICION_NATURAL);
    }


    // =========================================================
    // OBTENER COMIDA DE LA EXPEDICIÓN
    // =========================================================

    private Scr_CreadorObjetos ObtenerComidaExpedicion()
    {
        switch (TipoExpedicionActual)
        {
            case EXPEDICION_BATALLA:
                return ComidaExpedicionBatalla;

            case EXPEDICION_MINERAL:
                return ComidaExpedicionMineral;

            case EXPEDICION_NATURAL:
                return ComidaExpedicionNatural;
        }


        return null;
    }


    // =========================================================
    // OBTENER CANTIDAD REQUERIDA
    // =========================================================

    private int ObtenerCantidadComidaExpedicion()
    {
        switch (TipoExpedicionActual)
        {
            case EXPEDICION_BATALLA:
                return CantidadRequeridaBatalla;

            case EXPEDICION_MINERAL:
                return CantidadRequeridaMineral;

            case EXPEDICION_NATURAL:
                return CantidadRequeridaNatural;
        }


        return 0;
    }


    // =========================================================
    // OBTENER DURACIÓN
    // =========================================================

    private int ObtenerDuracionExpedicion()
    {
        switch (TipoExpedicionActual)
        {
            case EXPEDICION_BATALLA:
                return DuracionExpedicionBatalla;

            case EXPEDICION_MINERAL:
                return DuracionExpedicionMineral;

            case EXPEDICION_NATURAL:
                return DuracionExpedicionNatural;
        }


        return 0;
    }


    // =========================================================
    // OBTENER RECOMPENSAS
    // =========================================================

    private Scr_CreadorObjetos[] ObtenerRecompensasExpedicion()
    {
        switch (TipoExpedicionActual)
        {
            case EXPEDICION_BATALLA:
                return RecompensasBatalla;

            case EXPEDICION_MINERAL:
                return RecompensasMineral;

            case EXPEDICION_NATURAL:
                return RecompensasNatural;
        }


        return null;
    }


    // =========================================================
    // OBTENER PROBABILIDADES
    // =========================================================

    private int[] ObtenerProbabilidadesExpedicion()
    {
        switch (TipoExpedicionActual)
        {
            case EXPEDICION_BATALLA:
                return ProbabilidadesBatalla;

            case EXPEDICION_MINERAL:
                return ProbabilidadesMineral;

            case EXPEDICION_NATURAL:
                return ProbabilidadesNatural;
        }


        return null;
    }


    // =========================================================
    // ACTUALIZAR TODA LA UI
    // =========================================================

    private void ActualizarUIExpedicion()
    {
        Scr_CreadorObjetos comida =
            ObtenerComidaExpedicion();

        int cantidad =
            ObtenerCantidadComidaExpedicion();

        int duracion =
            ObtenerDuracionExpedicion();


        // -----------------------------------------------------
        // ICONO DE COMIDA
        // -----------------------------------------------------

        if (ImagenComida != null)
        {
            if (comida != null)
            {
                ImagenComida.sprite = comida.Icono;
                ImagenComida.enabled = true;
            }
            else
            {
                ImagenComida.enabled = false;
            }
        }


        // -----------------------------------------------------
        // CANTIDAD DE COMIDA
        // -----------------------------------------------------

        if (TextoCantidadComida != null)
        {
            TextoCantidadComida.text =
                cantidad.ToString();
        }


        // -----------------------------------------------------
        // DURACIÓN
        // -----------------------------------------------------

        if (TextoTiempoExpedicion != null)
        {
            TextoTiempoExpedicion.text =
                FormatearDuracionExpedicion(duracion);
        }


        // -----------------------------------------------------
        // RECOMPENSAS
        // -----------------------------------------------------

        ActualizarListaRecompensas();


        // -----------------------------------------------------
        // BOTÓN ENVIAR
        // -----------------------------------------------------

        ActualizarBotonEnviar();
    }


    // =========================================================
    // FORMATEAR DURACIÓN
    // =========================================================

    private string FormatearDuracionExpedicion(int horas)
    {
        int dias = horas / 24;
        int horasRestantes = horas % 24;

        if (dias > 0)
        {
            if (horasRestantes > 0)
            {
                return dias + " d " +
                       horasRestantes + " h";
            }

            return dias + " d";
        }

        return horas + " h";
    }


    // =========================================================
    // LIMPIAR RECOMPENSAS
    // =========================================================

    private void LimpiarRecompensasUI()
    {
        if (PadreRecompensas == null)
            return;


        for (int i = PadreRecompensas.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(
                PadreRecompensas.GetChild(i).gameObject
            );
        }
    }


    // =========================================================
    // ACTUALIZAR LISTA DE RECOMPENSAS
    // =========================================================

    private void ActualizarListaRecompensas()
    {
        LimpiarRecompensasUI();

        // IMPORTANTE: leer directamente con el tipo actual, sin trucos
        Scr_CreadorObjetos[] recompensas = null;
        int[] probabilidades = null;

        switch (TipoExpedicionActual)
        {
            case EXPEDICION_BATALLA:
                recompensas = RecompensasBatalla;
                probabilidades = ProbabilidadesBatalla;
                break;
            case EXPEDICION_MINERAL:
                recompensas = RecompensasMineral;
                probabilidades = ProbabilidadesMineral;
                break;
            case EXPEDICION_NATURAL:
                recompensas = RecompensasNatural;
                probabilidades = ProbabilidadesNatural;
                break;
        }

        if (recompensas == null || probabilidades == null) return;

        int cantidad = Mathf.Min(recompensas.Length, probabilidades.Length);

        for (int i = 0; i < cantidad; i++)
        {
            if (recompensas[i] == null) continue;

            GameObject nuevo = Instantiate(PrefabRecompensa, PadreRecompensas);
            Transform t = nuevo.transform;

            if (t.childCount > 0 && t.GetChild(0).childCount > 0)
                t.GetChild(0).GetChild(0).GetComponent<Image>().sprite = recompensas[i].Icono;

            if (t.childCount > 1 && t.GetChild(1).childCount > 0)
                t.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = recompensas[i].Nombre;

            if (t.childCount > 2 && t.GetChild(2).childCount > 0)
                t.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = probabilidades[i] + "%";
        }
    }


    // =========================================================
    // VALIDAR PROBABILIDADES
    // =========================================================

    private bool ValidarProbabilidadesExpedicion()
    {
        Scr_CreadorObjetos[] recompensas =
            ObtenerRecompensasExpedicion();

        int[] probabilidades =
            ObtenerProbabilidadesExpedicion();


        if (recompensas == null ||
            probabilidades == null)
        {
            return false;
        }


        if (recompensas.Length == 0)
        {
            return false;
        }


        if (recompensas.Length != probabilidades.Length)
        {
            Debug.LogError(
                "Las recompensas y probabilidades de la expedición " +
                "no tienen la misma cantidad de elementos."
            );

            return false;
        }


        int suma = 0;


        for (int i = 0;
             i < probabilidades.Length;
             i++)
        {
            if (recompensas[i] == null)
            {
                Debug.LogError(
                    "Existe una recompensa nula en la expedición."
                );

                return false;
            }


            if (probabilidades[i] <= 0)
            {
                Debug.LogError(
                    "Una probabilidad de expedición es menor o igual a 0."
                );

                return false;
            }


            suma += probabilidades[i];
        }


        if (suma != 100)
        {
            Debug.LogError(
                "Las probabilidades de la expedición deben sumar 100. " +
                "Actualmente suman: " + suma
            );

            return false;
        }


        return true;
    }


    // =========================================================
    // OBTENER CANTIDAD DE UN OBJETO DEL INVENTARIO
    // =========================================================

    private int ObtenerCantidadInventario(string nombreObjeto)
    {
        if (Inventario == null ||
            Inventario.Objetos == null ||
            Inventario.Cantidades == null)
        {
            return 0;
        }


        int cantidadElementos =
            Mathf.Min(
                Inventario.Objetos.Length,
                Inventario.Cantidades.Length
            );


        for (int i = 0;
             i < cantidadElementos;
             i++)
        {
            if (Inventario.Objetos[i] == null)
                continue;


            if (Inventario.Objetos[i].Nombre != nombreObjeto)
                continue;


            return Inventario.Cantidades[i];
        }


        return 0;
    }

    // =========================================================
    // ACTUALIZAR ESTADO DEL BOTÓN ENVIAR
    // =========================================================

    private void ActualizarBotonEnviar()
    {
        if (BotonEnviar == null) return;

        bool puedeEnviar = true;

        if (!Eclosiono || EtapaGrifo < 3) puedeEnviar = false;
        if (CuentaActiva) puedeEnviar = false;
        if (!ValidarProbabilidadesExpedicion()) puedeEnviar = false;

        // NUEVO: Solo bloquea cuando está en 0 (lleno total)
        // Con 1, 8, 15, 20 sí deja
        if (CapacidadGrifoExpedicion <= 0)
        {
            puedeEnviar = false;
        }

        Scr_CreadorObjetos comida = ObtenerComidaExpedicion();
        int cantidadNecesaria = ObtenerCantidadComidaExpedicion();

        if (comida == null) puedeEnviar = false;
        else
        {
            int cantidadJugador = ObtenerCantidadInventario(comida.Nombre);
            if (cantidadJugador < cantidadNecesaria) puedeEnviar = false;
        }

        BotonEnviar.interactable = puedeEnviar;
    }


    // =========================================================
    // ENVIAR EXPEDICIÓN
    // =========================================================

    public void EnviarExpedicion()
    {
        if (!Eclosiono || EtapaGrifo < 3) return;
        if (CuentaActiva) return;
        if (!ValidarProbabilidadesExpedicion()) return;

        // NUEVO: Solo bloquea el 0
        if (CapacidadGrifoExpedicion <= 0) return;

        Scr_CreadorObjetos comida = ObtenerComidaExpedicion();
        int cantidadNecesaria = ObtenerCantidadComidaExpedicion();
        if (comida == null) return;

        int cantidadJugador = ObtenerCantidadInventario(comida.Nombre);
        if (cantidadJugador < cantidadNecesaria) return;

        // Si mandas con capacidad 15, los 5 que quedaban se pierden / se sobreescriben
        // Si no quieres que se pierdan, comenta estas 3 líneas y deja que se acumulen
        // pero se van a juntar con los 20 nuevos
        if (RecompensasPendientes && CapacidadGrifoExpedicion > 0)
        {
            Debug.Log("Enviando con capacidad " + CapacidadGrifoExpedicion + ", se limpiarán " + (MAX_OBJETOS_EXPEDICION - CapacidadGrifoExpedicion) + " objetos pendientes restantes");
        }

        Inventario.QuitarObjeto(comida.Nombre, cantidadNecesaria, true);
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("GrifoExpedicionSeleccionada", TipoExpedicionActual);
        PlayerPrefs.Save();

        StartCoroutine(IniciarExpedicionCoroutine());
    }


    // =========================================================
    // ANIMACIÓN ANTES DE IRSE
    // =========================================================

    private IEnumerator IniciarExpedicionCoroutine()
    {
        if (ObjetoRecompensa != null) ObjetoRecompensa.SetActive(false);

        // DETENER loop del grande y reproducir GRITO
        DetenerSonidoGrifo();
        if (Sonidos != null && Sonidos.Length > 0 && Sonidos[0] != null)
        {
            if (AudioPrimerHuevo != null)
                AudioPrimerHuevo.PlayOneShot(Sonidos[0]); // grito al enviar
        }

        // pequeña pausa para que se escuche el grito antes del despegue
        yield return new WaitForSeconds(0.3f);

        Animator animador = AnimatorGrifoGrande;
        if (animador == null && GrifoGrande != null) animador = GrifoGrande.GetComponent<Animator>();
        if (animador == null && GrifoGrande != null) animador = GrifoGrande.GetComponentInChildren<Animator>();

        if (animador != null) animador.SetBool("Despego", true);
        yield return new WaitForSeconds(1.35f);

        ObjetoExpedicion.GetComponent<Scr_ActivadorMenuEstructuraFijo>().CerrarTablero();

        if (animador != null) animador.SetBool("Despego", false);

        if (GrifoGrande != null) GrifoGrande.SetActive(false);
        if (Grifovolador != null) Grifovolador.SetActive(true);

        int duracionHoras = ObtenerDuracionExpedicion();
        int duracionMinutos = duracionHoras * 60;
        IniciarCuenta(duracionMinutos, 2);
    }


    // =========================================================
    // RESTAURAR EXPEDICIÓN EN CURSO
    // =========================================================

    private void RestaurarExpedicionEnCurso()
    {
        DesactivarSistemaComida();
        ObjetoExpedicion.SetActive(true);
        CanvasExpedicion.SetActive(false);
        CanvasProgreso.SetActive(true);
        DetenerSonidoGrifo(); // volador sin sonido

        if (GrifoGrande != null) GrifoGrande.SetActive(false);
        if (Grifovolador != null) Grifovolador.SetActive(true);
    }


    // =========================================================
    // COMPLETAR EXPEDICIÓN
    // =========================================================

    private void CompletarExpedicion()
    {
        CuentaActiva = false;
        GenerarRecompensasExpedicion();

        if (GrifoGrande != null) GrifoGrande.SetActive(true);
        if (Grifovolador != null) Grifovolador.SetActive(false);
        if (GrifoChico != null) GrifoChico.SetActive(false);
        if (GrifoMediano != null) GrifoMediano.SetActive(false);

        Animator animador = AnimatorGrifoGrande;
        if (animador == null && GrifoGrande != null) animador = GrifoGrande.GetComponentInChildren<Animator>();
        if (animador != null) animador.SetBool("Despego", false);

        CanvasProgreso.SetActive(false);
        if (ObjetoRecompensa != null) ObjetoRecompensa.SetActive(true);

        // Volver a iniciar loop del grande al regresar
        IniciarSonidoGrifo(3);

        GuardarDatosExpedicion();
        GuardarCuenta();
    }


    // =========================================================
    // GENERAR RECOMPENSAS
    // =========================================================

    private void GenerarRecompensasExpedicion()
    {
        Scr_CreadorObjetos[] recompensas = ObtenerRecompensasExpedicion();
        int[] probabilidades = ObtenerProbabilidadesExpedicion();

        if (!ValidarProbabilidadesExpedicion()) return;

        CantidadesRecompensasPendientes = new int[recompensas.Length];

        for (int tirada = 0; tirada < MAX_OBJETOS_EXPEDICION; tirada++)
        {
            int numero = Random.Range(1, 101);
            int acumulado = 0;
            for (int i = 0; i < probabilidades.Length; i++)
            {
                acumulado += probabilidades[i];
                if (numero <= acumulado)
                {
                    CantidadesRecompensasPendientes[i]++;
                    break;
                }
            }
        }

        TipoExpedicionPendiente = TipoExpedicionActual;
        RecompensasPendientes = true;

        // Al generar, capacidad pasa a 0 automaticamente
        ActualizarTextoCapacidad();
        GuardarDatosExpedicion();
    }


    // =========================================================
    // ENTREGAR RECOMPENSAS
    // =========================================================

    private void EntregarRecompensasPendientes()
    {
        if (!RecompensasPendientes) return;
        if (Inventario == null) return;
        if (CantidadesRecompensasPendientes == null) return;

        int tipoActual = TipoExpedicionActual;
        TipoExpedicionActual = TipoExpedicionPendiente;
        Scr_CreadorObjetos[] recompensas = ObtenerRecompensasExpedicion();
        TipoExpedicionActual = tipoActual;

        if (recompensas == null) return;

        int cantidadElementos = Mathf.Min(recompensas.Length, CantidadesRecompensasPendientes.Length);
        bool quedanRecompensas = false;

        for (int i = 0; i < cantidadElementos; i++)
        {
            int cantidadPendiente = CantidadesRecompensasPendientes[i];
            if (cantidadPendiente <= 0) continue;
            if (recompensas[i] == null) continue;

            string nombreObjeto = recompensas[i].Nombre;

            // INTENTA AGREGAR TODO LO QUE TIENE EL GRIFO DE ESTE ITEM
            // AgregarObjeto devuelve CUANTO SI PUDO METER (ej: tenias 15/20 y el grifo trae 10, devuelve 5)
            int cantidadAgregada = Inventario.AgregarObjeto(
                nombreObjeto,
                cantidadPendiente,
                true,
                false
            );

            // SOLO restamos lo que SI entró. El resto se queda en el grifo, no se destruye.
            if (cantidadAgregada > 0)
            {
                CantidadesRecompensasPendientes[i] -= cantidadAgregada;
            }

            if (CantidadesRecompensasPendientes[i] > 0)
            {
                quedanRecompensas = true;
            }
        }

        // Verificar si aún queda algo de cualquier tipo
        for (int i = 0; i < CantidadesRecompensasPendientes.Length; i++)
        {
            if (CantidadesRecompensasPendientes[i] > 0)
            {
                quedanRecompensas = true;
                break;
            }
        }

        RecompensasPendientes = quedanRecompensas;

        // Esto ya recalcula capacidad a 8 si recogiste 8, a 12 si recogiste 12, etc.
        ActualizarTextoCapacidad();

        if (ObjetoRecompensa != null)
        {
            ObjetoRecompensa.SetActive(RecompensasPendientes);
        }

        GuardarDatosExpedicion();
    }

    private void ActualizarTextoCapacidad()
    {
        if (TextoCapacidad == null)
            return;

        int totalPendiente = 0;

        if (CantidadesRecompensasPendientes != null)
        {
            for (int i = 0; i < CantidadesRecompensasPendientes.Length; i++)
            {
                totalPendiente += Mathf.Max(0, CantidadesRecompensasPendientes[i]);
            }
        }

        // Si no hay recompensas pendientes, totalPendiente será 0
        // Si aún quedan 3, totalPendiente=3 -> capacidad=17
        // Si ya recogiste todo, totalPendiente=0 -> capacidad=20
        if (!RecompensasPendientes)
        {
            totalPendiente = 0;
        }

        CapacidadGrifoExpedicion = MAX_OBJETOS_EXPEDICION - totalPendiente;
        CapacidadGrifoExpedicion = Mathf.Clamp(CapacidadGrifoExpedicion, 0, MAX_OBJETOS_EXPEDICION);

        TextoCapacidad.text = CapacidadGrifoExpedicion.ToString();

        PlayerPrefs.SetInt("GrifoExpedicionCapacidad", CapacidadGrifoExpedicion);
        PlayerPrefs.Save();
    }


    // =========================================================
    // GUARDAR DATOS DE EXPEDICIÓN
    // =========================================================

    private void GuardarDatosExpedicion()
    {
        PlayerPrefs.SetInt(
            "GrifoExpedicionSeleccionada",
            TipoExpedicionActual
        );


        PlayerPrefs.SetInt(
            "GrifoExpedicionRecompensasPendientes",
            RecompensasPendientes ? 1 : 0
        );


        PlayerPrefs.SetInt(
            "GrifoExpedicionTipoPendiente",
            TipoExpedicionPendiente
        );

        PlayerPrefs.SetInt("GrifoExpedicionCapacidad", CapacidadGrifoExpedicion);


        // Guardar cantidades.
        if (CantidadesRecompensasPendientes != null)
        {
            int cantidad =
                Mathf.Min(
                    CantidadesRecompensasPendientes.Length,
                    MAX_RECOMPENSAS_GUARDADAS
                );


            for (int i = 0;
                 i < cantidad;
                 i++)
            {
                PlayerPrefs.SetInt(
                    "GrifoExpedicionRecompensa_" + i,
                    CantidadesRecompensasPendientes[i]
                );
            }
        }


        PlayerPrefs.Save();
    }


    // =========================================================
    // CARGAR DATOS DE EXPEDICIÓN
    // =========================================================

    private void CargarDatosExpedicion()
    {
        TipoExpedicionActual =
            PlayerPrefs.GetInt(
                "GrifoExpedicionSeleccionada",
                EXPEDICION_BATALLA
            );


        RecompensasPendientes =
            PlayerPrefs.GetInt(
                "GrifoExpedicionRecompensasPendientes",
                0
            ) == 1;


        TipoExpedicionPendiente =
            PlayerPrefs.GetInt(
                "GrifoExpedicionTipoPendiente",
                EXPEDICION_BATALLA
            );

        CapacidadGrifoExpedicion =
            PlayerPrefs.GetInt(
                "GrifoExpedicionCapacidad",
            20
        );


        // -----------------------------------------------------
        // CARGAR CANTIDADES PENDIENTES
        // -----------------------------------------------------

        Scr_CreadorObjetos[] recompensas;


        int tipoAnterior =
            TipoExpedicionActual;


        TipoExpedicionActual =
            TipoExpedicionPendiente;


        recompensas =
            ObtenerRecompensasExpedicion();


        TipoExpedicionActual =
            tipoAnterior;


        if (recompensas == null)
        {
            CantidadesRecompensasPendientes =
                new int[0];

            return;
        }


        CantidadesRecompensasPendientes =
            new int[recompensas.Length];


        for (int i = 0;
             i < CantidadesRecompensasPendientes.Length;
             i++)
        {
            CantidadesRecompensasPendientes[i] =
                PlayerPrefs.GetInt(
                    "GrifoExpedicionRecompensa_" + i,
                    0
                );
        }


        // -----------------------------------------------------
        // SI HAY RECOMPENSAS, MOSTRAR INDICADOR
        // -----------------------------------------------------

        if (ObjetoRecompensa != null)
        {
            ObjetoRecompensa.SetActive(
                false
            );
        }



        ActualizarTextoCapacidad();
    }



}
