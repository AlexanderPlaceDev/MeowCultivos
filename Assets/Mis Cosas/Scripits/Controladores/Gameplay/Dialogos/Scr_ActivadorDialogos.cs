using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ActivadorDialogos : MonoBehaviour
{
    private bool estaAdentro = false;
    [SerializeField] private GameObject MisionesSecundariasUI;
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private GameObject[] iconos;
    [SerializeField] private GameObject camaraDialogo;
    [SerializeField] private GameObject camaraGata;

    [SerializeField] private List<Scr_CreadorDialogos> MisionesSecundariasDar;
    public MisionesSecundrias_UI ControladorMisionesSecundariasUI;
    public List<Scr_CreadorMisiones> MisionesSecundarias;

    private bool canvasActivo = false;
    private bool completoSecundaria = false;

    private Scr_SistemaDialogos sistemaDialogos;
    private Scr_ControladorMisiones controladorMisiones;
    private Transform Gata;

    private CinemachineBrain brain;
    private float transicionDuracion = 1f;

    bool Hablando = false;
    bool ViendoMisiones = false;

    [SerializeField] private bool autoIniciarDialogo = false;

    private void Start()
    {
        Gata = GameObject.Find("Gata").transform;
        ControladorMisionesSecundariasUI = MisionesSecundariasUI.GetComponent<MisionesSecundrias_UI>();
        brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain != null)
        {
            brain.m_DefaultBlend.m_Time = transicionDuracion;
        }

        sistemaDialogos = GetComponent<Scr_SistemaDialogos>();
        controladorMisiones = Gata.GetChild(4).GetComponent<Scr_ControladorMisiones>();
    }

    private void Update()
    {
        if (!estaAdentro) return; // Salir si no está dentro del trigger

        if (sistemaDialogos != null && panelDialogo.activeSelf == false)
        {
            sistemaDialogos.EnPausa = true;
        }

        if (!autoIniciarDialogo) // Solo escuchar input si no es autoIniciar
        {
            if (Input.GetKeyDown(KeyCode.E) && !ViendoMisiones)
            {
                Hablando = true;
                ManejarDialogoPrincipal();
            }

            if (Input.GetKeyDown(KeyCode.F) && !Hablando)
            {
                ViendoMisiones = true;
                ManejarMisionesSecundarias();
            }
        }

        //Asegurarse de desactivar los dialogos
        if (sistemaDialogos != null && sistemaDialogos.Leido && !panelDialogo.activeSelf && !canvasActivo)
        {
            DesactivarDialogo();
        }
    }

    private void ManejarDialogoPrincipal()
    {
        if (panelDialogo.activeSelf)
        {
            VerificarMisionPrincipal();
            ActivarDialogo();
        }
        else
        {
            canvasActivo = true;
            StartCoroutine(EsperarYCambiarCamara());
        }
    }
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

        sistemaDialogos.recompensarSecundarias = completoSecundaria;

        if (completoSecundaria)
        {
            sistemaDialogos.IniciarDialogo();
            completoSecundaria = false;
        }
        else
        {
            MisionesSecundariasUI.SetActive(true);
            ControladorMisionesSecundariasUI.conseguirNPC(gameObject);
            ControladorMisionesSecundariasUI.coseguir_misiones(MisionesSecundariasDar);
        }
    }
    private IEnumerator EsperarYCambiarCamara()
    {
        CambiarACamaraDialogo();
        yield return new WaitForSeconds(transicionDuracion);
        VerificarMisionPrincipal();
        ActivarDialogo();
    }

    private void VerificarMisionPrincipal()
    {
        Debug.Log("Verifica la mision");
        if (controladorMisiones.MisionPrincipal == null) return;
        controladorMisiones.RevisarMisionPrincipal();
        if (controladorMisiones.MisionPCompleta)
        {
            controladorMisiones.MisionPCompleta = false;
            controladorMisiones.MisionPrincipal = null;



            //Agregar logica para quitar/dar items/Rango/XP/Dinero




            sistemaDialogos.LineaActual = 0;
            sistemaDialogos.Leido = false;
            sistemaDialogos.DialogoActual++;
        }
    }

    

    private void ActivarDialogo()
    {
        if (camaraDialogo != null) camaraDialogo.SetActive(true);
        if (camaraGata != null) camaraGata.SetActive(false);

        OcultarIconos(); // Siempre ocultar iconos al iniciar diálogo
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
                sistemaDialogos.IniciarDialogo();
            }
        }

        
            
        
    }

    public void DesactivarDialogo()
    {
        GuardarNPC();
        Hablando = false;

        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;

        if (camaraDialogo != null) camaraDialogo.SetActive(false);
        if (camaraGata != null) camaraGata.SetActive(true);

        if (!autoIniciarDialogo) // Solo mostrar iconos si no es autoIniciar
            MostrarIconos();
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

    private void MostrarIconos()
    {
        foreach (var icono in iconos)
        {
            if (icono != null) icono.SetActive(true);
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

    public void cerrarMisionesSecundarias()
    {
        GuardarNPC();
        ViendoMisiones = false;
        MisionesSecundariasUI.SetActive(false);
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
