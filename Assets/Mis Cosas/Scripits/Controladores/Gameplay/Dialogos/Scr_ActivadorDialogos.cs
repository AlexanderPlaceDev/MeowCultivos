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
    private bool Hablando = false; // ¿Está hablando actualmente?
    private bool Comprando = false; // ¿Está hablando actualmente?
    public bool ViendoMisiones = false; // ¿Está viendo la UI de misiones?
    private bool ViendoTienda = false;

    //========================================
    //=== Configuración del Comportamiento ===
    //========================================
    [SerializeField] private bool autoIniciarDialogo = false; // ¿El diálogo se inicia automáticamente?
    [SerializeField] private bool MuestraMisiones; // ¿Mostrar misiones secundarias?

    //==========================================
    //=== Referencias a Objetos de la Escena ===
    //==========================================
    [SerializeField] private GameObject MisionesSecundariasUI;
    [SerializeField] private GameObject TiendaUI;
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private GameObject[] iconos;
    [SerializeField] private GameObject camaraTienda;
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
            if (Input.GetKeyDown(KeyCode.E) && !ViendoMisiones & !Comprando)
            {
                Hablando = true;
                Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
                ManejarDialogoPrincipal();
            }

            //=========================================
            //=== Tecla F: Ver misiones secundarias ===
            //=========================================
            if (Input.GetKeyDown(KeyCode.F) && !Hablando && !Comprando)
            {
                Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;

                if (MuestraMisiones)
                {
                    ControladorMisionesSecundariasUI.activadorActual = this;
                    ViendoMisiones = true;
                    ManejarMisionesSecundarias();
                }
                else
                {
                    Comprando = true;
                    ManejarUIVenta();
                }
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

        //Primero Verifica si completo alguna mision
        List<int> MisionesAEliminar = new List<int>();
        int index = 0;

        foreach (var mision in controladorMisiones.MisionesSecundarias)
        {
            Debug.Log("Accede a " + index);

            //Aqui comprueba
            if (controladorMisiones.MisionesScompletas[index])
            {
                MisionesAEliminar.Add(index); // 📌 Marcar para eliminar después

                //🎁 Recompensas
                if (mision.RecompensaDinero > 0)
                    GameObject.Find("ObjetosAgregados").GetComponent<Scr_ObjetosAgregados>().AgregarDinero(mision.RecompensaDinero);

                if (mision.RecompensaXP > 0)
                    GameObject.Find("ObjetosAgregados").GetComponent<Scr_ObjetosAgregados>().AgregarExperiencia(mision.RecompensaXP);

                if (mision.ObjetosQueDa.Length > 0)
                {
                    Scr_Inventario inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();
                    int j = 0;
                    foreach (Scr_CreadorObjetos item in mision.ObjetosQueDa)
                    {
                        int i = 0;
                        foreach (Scr_CreadorObjetos obj in inventario.Objetos)
                        {
                            if (obj == item)
                            {
                                inventario.Cantidades[i] += mision.CantidadesDa[j];
                                GameObject.Find("ObjetosAgregados").GetComponent<Scr_ObjetosAgregados>().Lista.Add(item);
                                GameObject.Find("ObjetosAgregados").GetComponent<Scr_ObjetosAgregados>().Cantidades.Add(mision.CantidadesDa[j]);
                            }
                            i++;
                        }
                        j++;
                    }
                }

                //📦 Quitar objetos necesarios
                if (mision.ObjetosNecesarios.Length > 0)
                {
                    Scr_Inventario inventario = Gata.transform.GetChild(7).GetComponent<Scr_Inventario>();
                    int i = 0;
                    foreach (Scr_CreadorObjetos item in mision.ObjetosNecesarios)
                    {
                        inventario.QuitarObjeto(mision.CantidadesQuita[i], item.Nombre);
                        i++;
                    }
                }
            }
            index++;
        }

        Debug.Log("Cambiando Camara Secundaria");
        StartCoroutine(EsperarYCambiarCamaraSecundaria(MisionesAEliminar));
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
    }

    private void ManejarUIVenta()
    {
        ViendoTienda = true; // 📌 Activar bandera
        Debug.Log("Cambiando a cámara de tienda");
        StartCoroutine(EsperarYCambiarCamaraTienda());
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
    private IEnumerator EsperarYCambiarCamaraSecundaria(List<int> misionesAEliminar)
    {
        CambiarACamaraDialogo();
        yield return new WaitForSeconds(transicionDuracion);

        ActivarDialogo(false); // 🎤 Mostrar diálogo de misión completada

        yield return new WaitUntil(() => !panelDialogo.activeSelf); // ⏳ Espera a que termine el diálogo

        // 📌 Eliminar misiones completadas
        for (int i = misionesAEliminar.Count - 1; i >= 0; i--)
        {
            int idx = misionesAEliminar[i];
            controladorMisiones.MisionesSecundarias.RemoveAt(idx);
            controladorMisiones.MisionesScompletas.RemoveAt(idx);
        }

        ActualizarMisionActual();
    }

    private void ActualizarMisionActual()
    {
        if (controladorMisiones.MisionPrincipal != null)
        {
            controladorMisiones.MisionActual = controladorMisiones.MisionPrincipal;
        }
        else if (controladorMisiones.MisionesSecundarias.Count > 0)
        {
            controladorMisiones.MisionActual = controladorMisiones.MisionesSecundarias[0];
        }
        else
        {
            controladorMisiones.MisionActual = null;
            Debug.Log("ℹ️ No hay más misiones activas.");
        }
        controladorMisiones.ActualizarUI();
    }

    private IEnumerator EsperarYCambiarCamaraTienda()
    {
        CambiarACamaraDialogo();
        yield return new WaitForSeconds(transicionDuracion);

        ActivarDialogo(false); // 🎤 Mostrar diálogo secundario

        yield return new WaitUntil(() => !panelDialogo.activeSelf); // Espera a que termine el diálogo

        CambiarACamaraTienda();

        // 👇 Aquí activas la UI de la tienda
        if (camaraTienda != null) camaraTienda.SetActive(true);

        Debug.Log("🔓 UI de tienda activada");
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
            controladorMisiones.MisionActualCompleta = false;
            ActualizarMisionActual();
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

        if (ViendoMisiones)
        {
            MisionesSecundariasUI.GetComponent<MisionesSecundrias_UI>().SeleccionarMision(MisionesSecundarias[0]);
            MisionesSecundariasUI.SetActive(true);
        }
        else if (ViendoTienda)
        {
            TiendaUI.SetActive(true); // ✅ Activar UI de tienda
                                      // La cámara ya está en la tienda, no cambies nada
        }
        else
        {
            if (camaraDialogo != null) camaraDialogo.SetActive(false);
            if (camaraGata != null) camaraGata.SetActive(true);
            if (!autoIniciarDialogo)
                MostrarIconos();
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        }
    }

    private void CambiarACamaraDialogo()
    {
        if (camaraDialogo != null) camaraDialogo.SetActive(true);
        if (camaraGata != null) camaraGata.SetActive(false);
        OcultarIconos();
    }

    private void CambiarACamaraTienda()
    {
        if (camaraTienda != null) camaraTienda.SetActive(true);
        if (camaraDialogo != null) camaraDialogo.SetActive(false);
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
            if (camaraTienda != null)
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
    }

    public void CerrarTienda()
    {
        TiendaUI.SetActive(false);
        if (camaraTienda != null) camaraTienda.SetActive(false);
        if (camaraGata != null) camaraGata.SetActive(true);
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        ViendoTienda = false;
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
