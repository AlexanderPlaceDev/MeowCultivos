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
    [SerializeField] GameObject Botonx10;
    private GameObject CanvasComedero;
    private TextMeshProUGUI TextoCantidadFrutaComedero;

    [SerializeField] Image IconoComidaRequeridaParaEvo;


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

    [SerializeField]
    float TiempoDespegue =
        1.35f;

    [SerializeField]
    string AnimacionGrifoExpedicion =
        "Huesos Grifo_Iddle Grifo Grande";


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

        switch (etapa)
        {
            case 1:
                if (Huevo1 != null)
                    Huevo1.SetActive(true);
                break;

            case 2:
                if (Huevo2 != null)
                    Huevo2.SetActive(true);
                break;

            case 3:
                if (Huevo3 != null)
                    Huevo3.SetActive(true);
                break;
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
        GrifoChico.SetActive(false);
        Grifovolador.SetActive(false);
        GrifoMediano.SetActive(false);
        GrifoGrande.SetActive(false);


        switch (EtapaGrifo)
        {
            case 0:

                GrifoChico.SetActive(true);
                Huevo1.SetActive(false);
                Huevo2.SetActive(false);
                Huevo3.SetActive(false);
                Nido.SetActive(true);

                break;


            case 1:

                Grifovolador.SetActive(true);

                Nido.SetActive(true);

                break;


            case 2:

                GrifoMediano.SetActive(true);

                Nido.SetActive(false);

                break;


            case 3:

                GrifoGrande.SetActive(true);
                CanvasProgreso.SetActive(false);
                Nido.SetActive(false);

                break;
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

        // Aumentar la cantidad depositada
        ComidaDepositada++;

        // Guardar el progreso
        PlayerPrefs.SetInt("GrifoComidaDepositada", ComidaDepositada);
        PlayerPrefs.Save();

        // Actualizar el texto de la UI
        ActualizarTextoCantidadComida();

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
        // SI TODAVÍA QUEDAN RECOMPENSAS
        // NO PERMITIR NUEVA EXPEDICIÓN
        // =====================================================

        if (RecompensasPendientes)
        {
            CanvasExpedicion.SetActive(true);

            ActualizarUIExpedicion();

            return;
        }


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
        // No permitir cambiar de expedición mientras
        // hay una expedición funcionando.
        if (CuentaActiva && TipoCuenta == 2)
            return;

        // No permitir seleccionar otra si hay recompensas
        // pendientes de recoger.
        if (RecompensasPendientes)
            return;


        if (tipo < EXPEDICION_BATALLA ||
            tipo > EXPEDICION_NATURAL)
        {
            return;
        }


        TipoExpedicionActual = tipo;


        PlayerPrefs.SetInt(
            "GrifoExpedicionSeleccionada",
            TipoExpedicionActual
        );

        PlayerPrefs.Save();


        ActualizarUIExpedicion();
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


        Scr_CreadorObjetos[] recompensas =
            ObtenerRecompensasExpedicion();

        int[] probabilidades =
            ObtenerProbabilidadesExpedicion();


        if (recompensas == null ||
            probabilidades == null)
        {
            return;
        }


        int cantidad =
            Mathf.Min(
                recompensas.Length,
                probabilidades.Length
            );


        for (int i = 0; i < cantidad; i++)
        {
            if (recompensas[i] == null)
                continue;


            GameObject nuevo =
                Instantiate(
                    PrefabRecompensa,
                    PadreRecompensas
                );


            Transform transformNuevo =
                nuevo.transform;


            // -------------------------------------------------
            // ICONO
            // -------------------------------------------------

            if (transformNuevo.childCount > 0)
            {
                Transform contenedorIcono =
                    transformNuevo.GetChild(0);


                if (contenedorIcono.childCount > 0)
                {
                    Image icono =
                        contenedorIcono
                            .GetChild(0)
                            .GetComponent<Image>();


                    if (icono != null)
                    {
                        icono.sprite =
                            recompensas[i].Icono;
                    }
                }
            }


            // -------------------------------------------------
            // NOMBRE DEL ITEM
            // -------------------------------------------------

            if (transformNuevo.childCount > 1)
            {
                Transform contenedorNombre =
                    transformNuevo.GetChild(1);

                if (contenedorNombre.childCount > 0)
                {
                    TextMeshProUGUI textoNombre =
                        contenedorNombre
                            .GetChild(0)
                            .GetComponent<TextMeshProUGUI>();

                    if (textoNombre != null)
                    {
                        textoNombre.text =
                            recompensas[i].Nombre;
                    }
                }
            }


            // -------------------------------------------------
            // PROBABILIDAD
            // -------------------------------------------------

            if (transformNuevo.childCount > 2)
            {
                Transform contenedorProbabilidad =
                    transformNuevo.GetChild(2);

                if (contenedorProbabilidad.childCount > 0)
                {
                    TextMeshProUGUI textoProbabilidad =
                        contenedorProbabilidad
                            .GetChild(0)
                            .GetComponent<TextMeshProUGUI>();

                    if (textoProbabilidad != null)
                    {
                        textoProbabilidad.text =
                            probabilidades[i] + "%";
                    }
                }
            }
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
        if (BotonEnviar == null)
            return;


        bool puedeEnviar = true;


        // -----------------------------------------------------
        // GRIFO ADULTO
        // -----------------------------------------------------

        if (!Eclosiono || EtapaGrifo < 3)
        {
            puedeEnviar = false;
        }


        // -----------------------------------------------------
        // NO ESTÁ EN OTRA EXPEDICIÓN
        // -----------------------------------------------------

        if (CuentaActiva)
        {
            puedeEnviar = false;
        }


        // -----------------------------------------------------
        // NO HAY RECOMPENSAS PENDIENTES
        // -----------------------------------------------------

        if (RecompensasPendientes)
        {
            puedeEnviar = false;
        }


        // -----------------------------------------------------
        // PROBABILIDADES
        // -----------------------------------------------------

        if (!ValidarProbabilidadesExpedicion())
        {
            puedeEnviar = false;
        }


        // -----------------------------------------------------
        // COMIDA
        // -----------------------------------------------------

        Scr_CreadorObjetos comida =
            ObtenerComidaExpedicion();


        int cantidadNecesaria =
            ObtenerCantidadComidaExpedicion();


        if (comida == null)
        {
            puedeEnviar = false;
        }
        else
        {
            int cantidadJugador =
                ObtenerCantidadInventario(comida.Nombre);


            if (cantidadJugador < cantidadNecesaria)
            {
                puedeEnviar = false;
            }
        }


        BotonEnviar.interactable =
            puedeEnviar;
    }


    // =========================================================
    // ENVIAR EXPEDICIÓN
    // =========================================================

    public void EnviarExpedicion()
    {
        // -----------------------------------------------------
        // VALIDACIONES
        // -----------------------------------------------------

        if (!Eclosiono ||
            EtapaGrifo < 3)
        {
            return;
        }


        if (CuentaActiva)
        {
            return;
        }


        if (RecompensasPendientes)
        {
            return;
        }


        if (!ValidarProbabilidadesExpedicion())
        {
            return;
        }


        Scr_CreadorObjetos comida =
            ObtenerComidaExpedicion();


        int cantidadNecesaria =
            ObtenerCantidadComidaExpedicion();


        if (comida == null)
        {
            return;
        }


        // -----------------------------------------------------
        // COMPROBAR COMIDA
        // -----------------------------------------------------

        int cantidadJugador =
            ObtenerCantidadInventario(comida.Nombre);


        if (cantidadJugador < cantidadNecesaria)
        {
            return;
        }


        // -----------------------------------------------------
        // QUITAR COMIDA
        // -----------------------------------------------------

        Inventario.QuitarObjeto(
            comida.Nombre,
            cantidadNecesaria,
            true
        );


        // Guardar inmediatamente.
        PlayerPrefs.Save();


        // -----------------------------------------------------
        // GUARDAR TIPO DE EXPEDICIÓN
        // -----------------------------------------------------

        PlayerPrefs.SetInt(
            "GrifoExpedicionSeleccionada",
            TipoExpedicionActual
        );


        PlayerPrefs.Save();





        // -----------------------------------------------------
        // ANIMACIÓN DEL GRIFO
        // -----------------------------------------------------

        StartCoroutine(
            IniciarExpedicionCoroutine()
        );
    }


    // =========================================================
    // ANIMACIÓN ANTES DE IRSE
    // =========================================================

    private IEnumerator IniciarExpedicionCoroutine()
    {
        Animator animador = AnimatorGrifoGrande;

        if (animador == null && GrifoGrande != null)
        {
            animador = GrifoGrande.GetComponent<Animator>();
        }

        if (animador == null && GrifoGrande != null)
        {
            animador = GrifoGrande.GetComponentInChildren<Animator>();
        }


        // =====================================================
        // INICIAR DESPEGUE
        // =====================================================

        if (animador != null)
        {
            animador.SetBool("Despego", true);
        }


        // =====================================================
        // ESPERAR A QUE TERMINE LA ANIMACIÓN
        // =====================================================

        yield return new WaitForSeconds(1.35f);

        // -----------------------------------------------------
        // CERRAR UI
        // -----------------------------------------------------

        ObjetoExpedicion.GetComponent<Scr_ActivadorMenuEstructuraFijo>().CerrarTablero();

        // =====================================================
        // REGRESAR A IDLE
        // =====================================================

        if (animador != null)
        {
            animador.SetBool("Despego", false);
        }


        // =====================================================
        // AHORA SÍ OCULTAR EL GRIFO
        // =====================================================

        if (GrifoGrande != null)
        {
            GrifoGrande.SetActive(false);
        }


        // =====================================================
        // INICIAR CUENTA DE EXPEDICIÓN
        // =====================================================

        int duracionHoras =
            ObtenerDuracionExpedicion();

        int duracionMinutos =
            duracionHoras * 60;

        IniciarCuenta(
            duracionMinutos,
            2
        );
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


        // El grifo permanece fuera mientras está explorando.
        if (GrifoGrande != null)
        {
            GrifoGrande.SetActive(false);
        }
    }


    // =========================================================
    // COMPLETAR EXPEDICIÓN
    // =========================================================

    private void CompletarExpedicion()
    {
        CuentaActiva = false;


        // -----------------------------------------------------
        // GENERAR LOS 20 OBJETOS
        // -----------------------------------------------------

        GenerarRecompensasExpedicion();


        // -----------------------------------------------------
        // MOSTRAR GRIFO NUEVAMENTE
        // -----------------------------------------------------

        if (GrifoGrande != null)
        {
            GrifoGrande.SetActive(true);
        }

        Animator animador = AnimatorGrifoGrande;

        if (animador == null && GrifoGrande != null)
        {
            animador = GrifoGrande.GetComponent<Animator>();
        }

        if (animador == null && GrifoGrande != null)
        {
            animador = GrifoGrande.GetComponentInChildren<Animator>();
        }

        if (animador != null)
        {
            animador.SetBool("Despego", false);
        }


        // -----------------------------------------------------
        // OCULTAR PROGRESO
        // -----------------------------------------------------

        CanvasProgreso.SetActive(false);


        // -----------------------------------------------------
        // ACTIVAR INDICADOR DE RECOMPENSA
        // -----------------------------------------------------

        if (ObjetoRecompensa != null)
        {
            ObjetoRecompensa.SetActive(true);
        }


        ObjetoExpedicion.SetActive(true);

        CanvasExpedicion.SetActive(false);


        GuardarDatosExpedicion();

        GuardarCuenta();
    }


    // =========================================================
    // GENERAR RECOMPENSAS
    // =========================================================

    private void GenerarRecompensasExpedicion()
    {
        Scr_CreadorObjetos[] recompensas =
            ObtenerRecompensasExpedicion();


        int[] probabilidades =
            ObtenerProbabilidadesExpedicion();


        if (!ValidarProbabilidadesExpedicion())
        {
            return;
        }


        // Crear arreglo donde se almacenará
        // la cantidad obtenida de cada recompensa.
        CantidadesRecompensasPendientes =
            new int[recompensas.Length];


        // -----------------------------------------------------
        // HACER 20 TIRADAS
        // -----------------------------------------------------

        for (int tirada = 0;
             tirada < MAX_OBJETOS_EXPEDICION;
             tirada++)
        {
            int numero =
                Random.Range(1, 101);


            int acumulado = 0;


            for (int i = 0;
                 i < probabilidades.Length;
                 i++)
            {
                acumulado +=
                    probabilidades[i];


                if (numero <= acumulado)
                {
                    CantidadesRecompensasPendientes[i]++;

                    break;
                }
            }
        }


        // -----------------------------------------------------
        // GUARDAR
        // -----------------------------------------------------

        TipoExpedicionPendiente =
            TipoExpedicionActual;


        RecompensasPendientes =
            true;


        GuardarDatosExpedicion();
    }


    // =========================================================
    // ENTREGAR RECOMPENSAS
    // =========================================================

    private void EntregarRecompensasPendientes()
    {
        if (!RecompensasPendientes)
            return;

        if (Inventario == null)
            return;

        if (CantidadesRecompensasPendientes == null)
            return;


        // =====================================================
        // GUARDAR TIPO ACTUAL
        // =====================================================

        int tipoActual =
            TipoExpedicionActual;


        // =====================================================
        // UTILIZAR EL TIPO QUE GENERÓ LAS RECOMPENSAS
        // =====================================================

        TipoExpedicionActual =
            TipoExpedicionPendiente;


        Scr_CreadorObjetos[] recompensas =
            ObtenerRecompensasExpedicion();


        // Restaurar la selección actual del jugador
        TipoExpedicionActual =
            tipoActual;


        if (recompensas == null)
            return;


        int cantidadElementos =
            Mathf.Min(
                recompensas.Length,
                CantidadesRecompensasPendientes.Length
            );


        bool quedanRecompensas = false;


        // =====================================================
        // PROCESAR CADA RECOMPENSA
        // =====================================================

        for (int i = 0;
             i < cantidadElementos;
             i++)
        {
            int cantidadPendiente =
                CantidadesRecompensasPendientes[i];


            if (cantidadPendiente <= 0)
                continue;


            if (recompensas[i] == null)
                continue;


            string nombreObjeto =
                recompensas[i].Nombre;


            // =================================================
            // INTENTAR AGREGAR AL INVENTARIO
            // =================================================

            int cantidadAgregada =
                Inventario.AgregarObjeto(
                    nombreObjeto,
                    cantidadPendiente,
                    true,
                    false
                );


            // =================================================
            // ¿CUÁNTO PUDO ENTRAR?
            // =================================================

            if (cantidadAgregada > 0)
            {
                // Evitamos que por seguridad se descuente
                // más de lo que estaba pendiente.

                cantidadAgregada =
                    Mathf.Min(
                        cantidadAgregada,
                        cantidadPendiente
                    );


                // =================================================
                // RETIRAR INMEDIATAMENTE LO QUE ENTRÓ
                // =================================================

                Inventario.QuitarObjeto(
                    nombreObjeto,
                    cantidadAgregada,
                    false
                );


                // =================================================
                // QUITARLO DE LAS RECOMPENSAS PENDIENTES
                // =================================================

                CantidadesRecompensasPendientes[i] -=
                    cantidadAgregada;
            }


            // =================================================
            // ¿TODAVÍA QUEDA ESTE OBJETO?
            // =================================================

            if (CantidadesRecompensasPendientes[i] > 0)
            {
                quedanRecompensas = true;
            }
        }


        // =====================================================
        // ACTUALIZAR ESTADO GENERAL
        // =====================================================

        RecompensasPendientes =
            quedanRecompensas;


        // =====================================================
        // INDICADOR DE RECOMPENSAS
        // =====================================================

        if (ObjetoRecompensa != null)
        {
            ObjetoRecompensa.SetActive(
                RecompensasPendientes
            );
        }


        // =====================================================
        // GUARDAR
        // =====================================================

        GuardarDatosExpedicion();
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
                RecompensasPendientes
            );
        }
    }



}
