using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ActivadorDialogos : MonoBehaviour
{
    //===========================
    //=== Variables de Estado ===
    //===========================
    private bool estaAdentro = false; // ¿Está la gata dentro del área de activación?
    private bool canvasActivo = false; // ¿Está el canvas de diálogo activo?
    private bool completoSecundaria = false; // ¿Alguna misión secundaria está completa?
    private bool Hablando = false; // ¿Está hablando actualmente?
    public bool ViendoMisiones = false; // ¿Está viendo la UI de misiones?

    //========================================
    //=== Configuración del Comportamiento ===
    //========================================
    [SerializeField] private bool autoIniciarDialogo = false; // ¿El diálogo se inicia automáticamente?
    [SerializeField] private bool MuestraMisiones; // ¿Mostrar misiones secundarias?

    //==========================================
    //=== Referencias a Objetos de la Escena ===
    //==========================================
    [SerializeField] private GameObject MisionesSecundariasUI;
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private GameObject[] iconos;
    [SerializeField] private GameObject camaraDialogo;
    [SerializeField] private GameObject camaraGata;

    //===================================
    //=== Referencias a Otros Scripts ===
    //===================================
    public MisionesSecundrias_UI ControladorMisionesSecundariasUI;
    public List<Scr_CreadorMisiones> MisionesSecundarias;
    private Scr_SistemaDialogos sistemaDialogos;
    private Scr_ControladorMisiones controladorMisiones;
    private Transform Gata;

    //===================
    //=== Cinemachine ===
    //===================
    private CinemachineBrain brain;
    private float transicionDuracion = 1f;

    private void Start()
    {
        Gata = GameObject.Find("Gata").transform;
        brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain != null)
        {
            brain.m_DefaultBlend.m_Time = transicionDuracion; // Ajusta duración de transición de cámara
        }

        sistemaDialogos = GetComponent<Scr_SistemaDialogos>();
        controladorMisiones = Gata.GetChild(4).GetComponent<Scr_ControladorMisiones>();
    }

    //========================================================================
    //=== MÉTODO Update: Controla la entrada de teclas y lógica de diálogo ===
    //========================================================================
    private void Update()
    {
        if (!estaAdentro) return; // Salir si la gata no está dentro

        if (sistemaDialogos != null && !panelDialogo.activeSelf)
        {
            sistemaDialogos.EnPausa = true; // Pausa sistema de diálogos si el panel está inactivo
        }

        if (!autoIniciarDialogo)
        {
            //==========================================
            //=== Tecla E: Iniciar diálogo principal ===
            //==========================================
            if (Input.GetKeyDown(KeyCode.E) && !ViendoMisiones)
            {
                Hablando = true;
                Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
                ManejarDialogoPrincipal();
            }

            //=========================================
            //=== Tecla F: Ver misiones secundarias ===
            //=========================================
            if (Input.GetKeyDown(KeyCode.F) && !Hablando)
            {
                Debug.Log("Presiona F");
                ViendoMisiones = true;
                Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
                ManejarMisionesSecundarias();
            }
        }

        //==================================================
        //=== Si ya se leyó el diálogo, desactivar panel ===
        //==================================================
        if (sistemaDialogos != null && sistemaDialogos.Leido && !panelDialogo.activeSelf && !canvasActivo && !ViendoMisiones)
        {
            DesactivarDialogo();
        }
    }

    //=================================
    //=== Manejar diálogo principal ===
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
            StartCoroutine(EsperarYCambiarCamaraPrincipal());
        }
    }

    //====================================
    //=== Manejar misiones secundarias ===
    //====================================
    private void ManejarMisionesSecundarias()
    {
        completoSecundaria = false;

        foreach (var mision in MisionesSecundarias)
        {
            int index = controladorMisiones.MisionesSecundarias.IndexOf(mision);
            if (index >= 0 && controladorMisiones.MisionesScompletas[index])
            {
                completoSecundaria = true;
                break;
            }
        }

        if (completoSecundaria)
        {
            sistemaDialogos.IniciarDialogo(false); // Inicia diálogo de recompensa secundaria
            completoSecundaria = false;
        }
        else
        {
            Debug.Log("Cambiando Camara Secundaria");
            StartCoroutine(EsperarYCambiarCamaraSecundaria());
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
        }
    }

    //=================================================================
    //=== Corutina: Cambiar cámara y esperar para diálogo principal ===
    //=================================================================
    private IEnumerator EsperarYCambiarCamaraPrincipal()
    {
        CambiarACamaraDialogo();
        yield return new WaitForSeconds(transicionDuracion);
        VerificarMisionPrincipal();
        ActivarDialogo(true);
    }

    //==================================================================
    //=== Corutina: Cambiar cámara y esperar para diálogo secundario ===
    //==================================================================
    private IEnumerator EsperarYCambiarCamaraSecundaria()
    {
        CambiarACamaraDialogo();
        yield return new WaitForSeconds(transicionDuracion);
        ActivarDialogo(false);
    }

    //===========
    //=== Verificar estado de la misión principal ===
    //===========
    private void VerificarMisionPrincipal()
    {
        Debug.Log("Verifica la mision");
        if (controladorMisiones.MisionPrincipal == null) return;
        controladorMisiones.RevisarMisionPrincipal();

        if (controladorMisiones.MisionPCompleta)
        {
            controladorMisiones.MisionPCompleta = false;
            controladorMisiones.MisionPrincipal = null;
            sistemaDialogos.LineaActual = 0;
            sistemaDialogos.Leido = false;
            sistemaDialogos.DialogoActual++;
        }
    }

    private void ActivarDialogo(bool Principal)
    {
        if (camaraDialogo != null) camaraDialogo.SetActive(true);
        if (camaraGata != null) camaraGata.SetActive(false);
        OcultarIconos();
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;

        if (!panelDialogo.activeSelf)
        {
            panelDialogo.SetActive(true);
            sistemaDialogos.EnPausa = false;

            if (sistemaDialogos.Leyendo)
            {
                sistemaDialogos.SaltarDialogo();
            }
            else
            {
                Debug.Log("Inicia Dialogo + Es principal:" + Principal);
                sistemaDialogos.IniciarDialogo(Principal);
            }
        }
    }

    public void DesactivarDialogo()
    {
        GuardarNPC();
        Hablando = false;

        if (!ViendoMisiones)
        {
            if (camaraDialogo != null) camaraDialogo.SetActive(false);
            if (camaraGata != null) camaraGata.SetActive(true);
            if (!autoIniciarDialogo)
                MostrarIconos();
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        }
        else
        {
            ControladorMisionesSecundariasUI.activadorActual = this;
            MisionesSecundariasUI.SetActive(true);
        }
    }

    private void CambiarACamaraDialogo()
    {
        if (camaraDialogo != null) camaraDialogo.SetActive(true);
        if (camaraGata != null) camaraGata.SetActive(false);
        OcultarIconos();
    }

    private void OcultarIconos()
    {
        foreach (var icono in iconos)
        {
            if (icono != null) icono.SetActive(false);
        }
    }

    public void MostrarIconos()
    {
        if (MisionesSecundarias.Count > 0 && MuestraMisiones)
        {
            iconos[0].SetActive(true);
            iconos[1].SetActive(true);
            iconos[2].SetActive(true);
            iconos[3].SetActive(true);
        }
        else
        {
            iconos[0].SetActive(true);
            iconos[1].SetActive(true);
        }
    }

    
    public void GuardarNPC()
    {
        var eventosGuardado = GetComponent<Scr_EventosGuardado>();
        if (eventosGuardado != null)
        {
            eventosGuardado.EventoDialogo(sistemaDialogos.DialogoActual, sistemaDialogos.NombreNPC);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Gata")) return;

        estaAdentro = true;

        if (!autoIniciarDialogo)
            MostrarIconos();

        if (autoIniciarDialogo && !Hablando)
        {
            Hablando = true;
            ManejarDialogoPrincipal();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Gata")) return;

        estaAdentro = false;
        OcultarIconos();
    }
}
