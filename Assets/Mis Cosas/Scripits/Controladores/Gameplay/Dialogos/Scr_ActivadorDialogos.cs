using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controla los diálogos, misiones y tiendas con los NPCs.
/// Gestiona transiciones de cámaras y activación de interfaces.
/// </summary>
public class Scr_ActivadorDialogos : MonoBehaviour
{
    //=========================
    //=== VARIABLES DE ESTADO ===
    //=========================
    public bool estaAdentro = false;      // Si la gata está dentro del trigger del NPC
    private bool canvasActivo = false;     // Si el canvas de diálogo está activo
    private bool Hablando = false;         // Si el jugador está en un diálogo
    private bool Comprando = false;        // Si selecciono comprar
    public bool ViendoMisiones = false;    // Si está viendo las misiones secundarias
    private bool ViendoTienda = false;     // Si ya está dentro de la tienda

    //===============================
    //=== CONFIGURACIÓN DEL NPC ===
    //===============================
    [SerializeField] private bool autoIniciarDialogo = false;  // Si el diálogo se inicia automáticamente al entrar
    [SerializeField] private bool EsTienda;                    // Si este NPC es una tienda
    [SerializeField] private bool UsaMisionesSecundarias = true;

    //=================================
    //=== REFERENCIAS DE LA ESCENA ===
    //=================================
    [SerializeField] public GameObject MisionesSecundariasUI;
    [SerializeField] public GameObject panelDialogo;
    [SerializeField] private GameObject[] iconos;
    [SerializeField] private GameObject camaraTienda;
    [SerializeField] private GameObject camaraDialogo;
    [SerializeField] private GameObject camaraGata;

    //=============================
    //=== REFERENCIAS A SCRIPTS ===
    //=============================
    public MisionesSecundrias_UI ControladorMisionesSecundariasUI;
    public List<Scr_CreadorMisiones> MisionesSecundarias;
    private Scr_SistemaDialogos sistemaDialogos;
    private Scr_ControladorMisiones controladorMisiones;
    private Transform Gata;

    //=====================
    //=== CINEMACHINE ===
    //=====================
    private CinemachineBrain brain;
    private float transicionDuracion = 1f;

    //====================
    //=== INICIALIZACIÓN ===
    //====================
    private void Start()
    {
        Gata = GameObject.Find("Gata").transform;
        brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain != null)
            brain.m_DefaultBlend.m_Time = transicionDuracion;

        sistemaDialogos = GetComponent<Scr_SistemaDialogos>();
        controladorMisiones = Gata.GetChild(4).GetComponent<Scr_ControladorMisiones>();
    }

    //===================
    //=== UPDATE LOOP ===
    //===================
    private void Update()
    {
        if (!estaAdentro) return;

        // Si el panel está cerrado, pausamos el sistema de diálogo
        if (sistemaDialogos != null && !panelDialogo.activeSelf)
            sistemaDialogos.EnPausa = true;
        if (panelDialogo.activeSelf)
        {
            Girar();
        }
        if (!autoIniciarDialogo)
        {
            // Iniciar diálogo (E)
            if (Input.GetKeyDown(KeyCode.E) && !ViendoMisiones && !Comprando)
            {
                if (EsTienda)
                {
                    Comprando = true;
                    ManejarUIVenta();
                }
                else
                {
                    Hablando = true;
                    Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
                    ManejarDialogoPrincipal();
                }
            }

            // Ver misiones (F)
            if (UsaMisionesSecundarias && Input.GetKeyDown(KeyCode.F) && !Hablando && !Comprando)
            {
                Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
                ControladorMisionesSecundariasUI.activadorActual = this;
                ViendoMisiones = true;
                ManejarMisionesSecundarias();
            }
        }

        // Si el diálogo terminó y no hay tienda activa → salir del modo diálogo
        if (sistemaDialogos != null && sistemaDialogos.Leido &&
            !panelDialogo.activeSelf && !canvasActivo && !ViendoMisiones && !EsTienda)
        {
            DesactivarDialogo();
        }
    }

    //=================================
    //=== GESTIÓN DE CÁMARAS ===
    //=================================
    private void CambiarACamaraDialogo()
    {
        camaraDialogo?.SetActive(true);
        camaraGata?.SetActive(false);
        OcultarIconos();
    }

    private void CambiarACamaraTienda()
    {
        camaraTienda?.SetActive(true);
        camaraDialogo?.SetActive(false);
        camaraGata?.SetActive(false);
        OcultarIconos();
    }

    /// <summary>
    /// ✅ Nueva función: Regresa al estado base del jugador (cámara de la gata, puede moverse, muestra iconos).
    /// </summary>
    public void RegresarACamaraBase()
    {
        Debug.Log("pico");
        // Apagar cámaras de diálogo y tienda
        camaraDialogo?.SetActive(false);
        if (camaraTienda != null)
        {
            camaraTienda?.SetActive(false);
        }


        // Volver a la cámara principal del jugador
        camaraGata?.SetActive(true);

        // Reset de estados
        ViendoTienda = false;
        ViendoMisiones = false;
        Hablando = false;
        Comprando = false;
        canvasActivo = false;
        controladorMisiones.EstaEnDialogo = false;

        // Permitir movimiento
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;

        // Mostrar iconos
        MostrarIconos();

        Debug.Log("🎬 Estado base restaurado: cámara de la gata activa.");
    }
    //=================================
    //=== CONTROL DE DIÁLOGOS ===
    //=================================
    private void ManejarDialogoPrincipal()
    {
        if (panelDialogo.activeSelf)
        {
            VerificarMisionPrincipal();
            ActivarDialogo(true);
        }
        else
        {
            canvasActivo = true;
            controladorMisiones.EstaEnDialogo = true;
            StartCoroutine(EsperarYCambiarCamaraPrincipal());
        }
    }

    private IEnumerator EsperarYCambiarCamaraPrincipal()
    {
        CambiarACamaraDialogo();
        yield return new WaitForSeconds(transicionDuracion);
        VerificarMisionPrincipal();
        ActivarDialogo(true);
        if(camaraTienda != null)
        {
            PlayerPrefs.SetString("DialogoSirilo", "Si");
        }
    }

    private void ActivarDialogo(bool Principal)
    {
        camaraDialogo?.SetActive(true);
        camaraGata?.SetActive(false);
        OcultarIconos();
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;

        if (!panelDialogo.activeSelf)
        {
            panelDialogo.SetActive(true);
            sistemaDialogos.EnPausa = false;

            if (sistemaDialogos.Leyendo)
                sistemaDialogos.SaltarDialogo();
            else
                sistemaDialogos.IniciarDialogo(Principal);
        }
    }

    public void DesactivarDialogo()
    {
        GuardarNPC();
        Hablando = false;

        if (ViendoMisiones)
        {
            MisionesSecundariasUI.GetComponent<MisionesSecundrias_UI>().SeleccionarMision(MisionesSecundarias[0]);
            MisionesSecundariasUI.SetActive(true);
        }
        else if (ViendoTienda)
        {
            CambiarACamaraTienda();
        }
        else
        {
            RegresarACamaraBase(); // Usamos la nueva función
        }
    }

    //=================================
    //=== CONTROL DE TIENDA ===
    //=================================
    private void ManejarUIVenta()
    {
        ViendoTienda = true;
        Debug.Log("🛍️ Iniciando interacción con tienda...");

        if (panelDialogo.activeSelf)
        {
            ActivarDialogo(true);
            StartCoroutine(EsperarYCambiarCamaraTienda());
        }
        else
        {
            canvasActivo = true;

            // Solo asigna un diálogo aleatorio si NO es la primera vez
            if (PlayerPrefs.GetString("DialogoSirilo", "No") == "Si")
            {
                sistemaDialogos.DialogoActual = Random.Range(1, sistemaDialogos.Dialogos.Length);
                Debug.Log("🗨️ Sirilo ya conocido → diálogo aleatorio");
            }
            else
            {
                sistemaDialogos.DialogoActual = 0;
                Debug.Log("👋 Primer encuentro → diálogo 0 (no se cambia)");
            }

            StartCoroutine(EsperarYCambiarCamaraPrincipal());
        }
    }

    private IEnumerator EsperarYCambiarCamaraTienda()
    {
        CambiarACamaraTienda();
        yield return new WaitForSeconds(transicionDuracion);
        camaraTienda?.SetActive(true);
        Debug.Log("🔓 Cámara y UI de tienda activadas");
    }

    //=================================
    //=== CONTROL DE MISIONES ===
    //=================================
    private void ManejarMisionesSecundarias()
    {
        List<int> misionesAEliminar = new List<int>();

        var objetosAgregados = GameObject.Find("ObjetosAgregados").GetComponent<Scr_ObjetosAgregados>();
        var inventario = Gata.transform.GetChild(7).GetComponent<Scr_Inventario>();

        int DineroAcumulado = 0;
        int XPAcumulado = 0;

        // Revisión y recompensas
        for (int i = 0; i < controladorMisiones.MisionesSecundarias.Count; i++)
        {
            var mision = controladorMisiones.MisionesSecundarias[i];
            bool esMisionDelActivador = MisionesSecundarias.Any(m => m.TituloMision == mision.TituloMision);
            if (!esMisionDelActivador || !controladorMisiones.MisionesScompletas[i]) continue;

            misionesAEliminar.Add(i);
            DineroAcumulado += mision.RecompensaDinero;
            XPAcumulado += mision.RecompensaXP;

            // Recompensas de objetos
            for (int j = 0; j < mision.ObjetosQueDa.Length; j++)
            {
                var objeto = mision.ObjetosQueDa[j];
                int cantidad = mision.CantidadesDa[j];

                for (int k = 0; k < inventario.Objetos.Length; k++)
                {
                    if (inventario.Objetos[k] == objeto)
                    {
                        inventario.Cantidades[k] += cantidad;
                        objetosAgregados.Lista.Add(objeto);
                        objetosAgregados.Cantidades.Add(cantidad);
                        break;
                    }
                }
            }

            // Quitar objetos requeridos
            for (int j = 0; j < mision.ObjetosNecesarios.Length; j++)
                inventario.QuitarObjeto(mision.CantidadesQuita[j], mision.ObjetosNecesarios[j].Nombre);
        }

        // Recompensas totales
        if (DineroAcumulado > 0) objetosAgregados.AgregarDinero(DineroAcumulado);
        if (XPAcumulado > 0) objetosAgregados.AgregarExperiencia(XPAcumulado);

        StartCoroutine(EsperarYCambiarCamaraSecundaria(misionesAEliminar));
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
    }

    private IEnumerator EsperarYCambiarCamaraSecundaria(List<int> misionesAEliminar)
    {
        CambiarACamaraDialogo();
        yield return new WaitForSeconds(transicionDuracion);
        ActivarDialogo(false);
        yield return new WaitUntil(() => !panelDialogo.activeSelf);

        // Eliminar misiones completadas
        for (int i = misionesAEliminar.Count - 1; i >= 0; i--)
        {
            int idx = misionesAEliminar[i];
            var m = controladorMisiones.MisionesSecundarias[idx];

            if (m.ObjetivosACazar != null)
            {
                for (int j = 0; j < m.ObjetivosACazar.Length; j++)
                {
                    string clave = $"{m.name}_CantidadCazados_{j}";
                    if (PlayerPrefs.HasKey(clave)) PlayerPrefs.DeleteKey(clave);
                }
            }

            controladorMisiones.MisionesSecundarias.RemoveAt(idx);
            controladorMisiones.MisionesScompletas.RemoveAt(idx);
        }

        PlayerPrefs.Save();
        ActualizarMisionActual();
    }
    private void Girar()
    {
        Quaternion objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, Gata.position.y, transform.position.z) - Gata.position);
        Gata.rotation = Quaternion.RotateTowards(Gata.rotation, objetivo, 200 * Time.deltaTime);
    }
    private void VerificarMisionPrincipal()
    {
        if (controladorMisiones.MisionPrincipal == null) return;
        controladorMisiones.RevisarMisionPrincipal();

        if (controladorMisiones.MisionPCompleta)
        {
            controladorMisiones.MisionPCompleta = false;
            controladorMisiones.MisionPrincipal = null;
            controladorMisiones.MisionActualCompleta = false;
            ActualizarMisionActual();
            sistemaDialogos.LineaActual = 0;
            sistemaDialogos.Leido = false;
            sistemaDialogos.DialogoActual++;
        }
    }

    private void ActualizarMisionActual()
    {
        if (controladorMisiones.MisionPrincipal != null)
            controladorMisiones.MisionActual = controladorMisiones.MisionPrincipal;
        else if (controladorMisiones.MisionesSecundarias.Count > 0)
            controladorMisiones.MisionActual = controladorMisiones.MisionesSecundarias[0];
        else
            controladorMisiones.MisionActual = null;

        controladorMisiones.ActualizarUI();
    }

    //=================================
    //=== UTILIDADES Y EVENTOS ===
    //=================================
    public void OcultarIconos()
    {
        foreach (var icono in iconos)
            if (icono != null) icono.SetActive(false);
    }

    public void MostrarIconos()
    {
        if (iconos == null || iconos.Length == 0) return;

        // ✅ Si el NPC usa misiones secundarias, muestra todos los iconos
        if (UsaMisionesSecundarias)
        {
            foreach (var icono in iconos)
                if (icono != null) icono.SetActive(true);
        }
        else
        {
            // ✅ Si NO usa misiones, solo muestra los dos primeros (diálogo)
            for (int i = 0; i < iconos.Length; i++)
            {
                if (iconos[i] == null) continue;

                if (i < 2)  // solo los primeros dos
                    iconos[i].SetActive(true);
                else
                    iconos[i].SetActive(false);
            }
        }
    }


    public void GuardarNPC()
    {
        var eventosGuardado = GetComponent<Scr_EventosGuardado>();
        if (eventosGuardado != null)
            eventosGuardado.EventoDialogo(sistemaDialogos.DialogoActual, sistemaDialogos.NombreNPC);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Gata")) return;
        estaAdentro = true;

        if (!autoIniciarDialogo)
        {
            // Mostrar iconos (MostrarIconos() manejará si muestra todos o solo los primeros 2).
            MostrarIconos();
        }
        else if (!Hablando)
        {
            Hablando = true;
            ManejarDialogoPrincipal();
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Gata")) return;
        Debug.Log("Sale");
        estaAdentro = false;
        OcultarIconos();
    }


}
