using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_ActivadorDialogos : MonoBehaviour
{
    private bool estaAdentro = false;
    [SerializeField] private GameObject MisionesSecundariasUI;
    [SerializeField] private GameObject MisionesExtrUI;
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private GameObject[] iconos;
    [SerializeField] private GameObject camara;
    [SerializeField] private GameObject camaraGata;
    [SerializeField] private GameObject CanvasNPC;
    [SerializeField] public bool Principal;
    [SerializeField] private List<Scr_CreadorDialogos> MisionesSecundarisDar;
    [SerializeField] private List<Scr_CreadorDialogos> Misiones_deDialogoExtra;
    public MisionesSecundrias_UI misionSEc;
    public Misiones_Extra misionExtr;
    public Scr_CreadorMisiones Misionequeespera;
    public List<Scr_CreadorMisiones> Misionesqueespera;
    public int misionespera;
    private bool CanvasActivo = false;
    public bool vaCambio = false;
    private Scr_SistemaDialogos sistemaDialogos;
    private Scr_ControladorMisiones ControladorMisiones;
    private Transform Gata;

    private CinemachineBrain brain;
    private float TransicionDuracion = 1f;
    public bool completoSecundaria=false;
    private void Start()
    {
        Gata = GameObject.Find("Gata").transform;
        misionSEc= MisionesSecundariasUI.GetComponent<MisionesSecundrias_UI>();

        misionExtr = MisionesExtrUI.GetComponent<Misiones_Extra>();
        camaraGata = GameObject.Find("Camara 360")?.gameObject; // Asegurar que sea un GameObject válido
        brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            // Cambiar duración del blend por defecto
            brain.m_DefaultBlend.m_Time = TransicionDuracion;
        }
        sistemaDialogos = GetComponent<Scr_SistemaDialogos>();
        ControladorMisiones = Gata.GetChild(4).GetComponent<Scr_ControladorMisiones>();
    }

    private void Update()
    {
        if (!estaAdentro)
        {
            sistemaDialogos.EnPausa = true;
            return;
        }

        if (panelDialogo.activeSelf)
        {
            ControlarGata();
        }
        else if (sistemaDialogos.Leido)
        {
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (panelDialogo.activeSelf)
            {
                ComprobarMision();
                ActivarDialogo();
            }
            else
            {
                CanvasActivo = true;
                StartCoroutine(EsperarCamara());
                CambiarACamaraDialogo();
            }
        }

        if (sistemaDialogos.Leido && !sistemaDialogos.Leyendo && !panelDialogo.activeSelf && !CanvasActivo)
        {
            CanvasActivo = false;
            DesactivarDialogo();
        }
    }

    private void ControlarGata()
    {
        Girar();
        var movimiento = Gata.GetComponent<Scr_Movimiento>();
        movimiento.Estado = Scr_Movimiento.Estados.Quieto;
        movimiento.InputVer = 0;
        movimiento.InputHor = 0;

        var animaciones = Gata.GetComponent<Scr_ControladorAnimacionesGata>();
        animaciones.PuedeCaminar = false;

        Gata.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void CambiarACamaraDialogo()
    {
        Debug.Log("Cambiando a camara de dialogo");
        camara.SetActive(true);
        camaraGata.SetActive(false);
        iconos[0].SetActive(false);
        iconos[1].SetActive(false);
    }

    private IEnumerator EsperarCamara()
    {
        yield return new WaitForSeconds(TransicionDuracion);
        if (!Principal)
        {
            CanvasNPC.SetActive(true);
        }
        else
        {
            BotonHablar();
        }
    }
    public void Boton_hablarUI()
    {
        Debug.Log("que");
        CanvasActivo = false;
        CanvasNPC.SetActive(false);
        MisionesExtrUI.SetActive(true);
        misionExtr.conseguirNPC(gameObject);
        misionExtr.coseguir_misiones(Misiones_deDialogoExtra);
    }
    public void opcionesUI()
    {
        CanvasActivo = false;
        CanvasNPC.SetActive(false);
        MisionesSecundariasUI.SetActive(false);
    }

    
    public void opcionesUI2()
    {
        CanvasActivo = false;
        CanvasNPC.SetActive(false);
        MisionesExtrUI.SetActive(false);
    }
    public void cerrarHablar_extra()
    {
        CanvasNPC.SetActive(true);
        CanvasActivo = true;
        MisionesExtrUI.SetActive(false);
    }
    public void cerrarMisionesSecundaris()
    {
        CanvasNPC.SetActive(true);
        CanvasActivo = true;
        MisionesSecundariasUI.SetActive(false);
    }
    public void BotonHablar()
    {
        CanvasActivo = false;
        CanvasNPC.SetActive(false);
        ComprobarMision();
        ActivarDialogo();
        
    }

    public void BotonMisionesSecundarias()
    {
        CanvasNPC.SetActive(false);
        if (Misionesqueespera != null)
        {
            //Debug.Log("ae");
            for (int t = 0; t < Misionesqueespera.Count; t++)
            {
                for (int i = 0; i < ControladorMisiones.MisionesExtra.Count; i++)
                {
                    if (ControladorMisiones.MisionesExtra[i] == Misionesqueespera[t] && ControladorMisiones.MisionesScompletas[i])
                    {
                        ControladorMisiones.TerminarMisionSexundaria(Misionesqueespera[t]);
                        completoSecundaria = true;
                        Debug.Log("aeeee" + completoSecundaria);
                    }
                }
            }

        }

        sistemaDialogos.recompensarSecundarias = completoSecundaria;
        if (completoSecundaria)
        {
            sistemaDialogos.IniciarDialogo();
            opcionesUI();
            completoSecundaria = false;
        }
        else
        {
            MisionesSecundariasUI.SetActive(true);
            misionSEc.conseguirNPC(gameObject);
            misionSEc.coseguir_misiones(MisionesSecundarisDar);
        }

    }
    public void Salir()
    {
        CanvasActivo = false;
        CanvasNPC.SetActive(false);
        camara.SetActive(true);
        camaraGata.SetActive(true);
    }
    public void quitarMisionSecundaria(Scr_CreadorDialogos mision)
    {
        for (int i = 0; i < MisionesSecundarisDar.Count; i++) 
        {
            Debug.LogError(MisionesSecundarisDar[i] == mision);
            if (MisionesSecundarisDar[i]== mision)
            {
                MisionesSecundarisDar.RemoveAt(i);
                Debug.Log("quito Mision");
            }
        }
    }
    private void ActivarDialogo()
    {
        Debug.Log("Activando dialogo y desactivando camaraGata");
        camaraGata.SetActive(false); // Asegurar que se apaga al iniciar diálogo
        camara.SetActive(true);

        if (!panelDialogo.activeSelf)
        {
            panelDialogo.SetActive(true);
            sistemaDialogos.EnPausa = false;
        }

        if (!sistemaDialogos.EnPausa)
        {
            if (sistemaDialogos.Leyendo)
            {
                sistemaDialogos.SaltarDialogo();
            }
            else
            {
                string textoActual = panelDialogo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                if (sistemaDialogos.Dialogos[sistemaDialogos.DialogoActual].Lineas[sistemaDialogos.LineaActual] == textoActual)
                {
                    sistemaDialogos.SiguienteLinea();
                }
                else
                {
                    sistemaDialogos.IniciarDialogo();

                }
            }
        }
    }

    private void DesactivarDialogo()
    {
        camara.SetActive(false);
        camaraGata.SetActive(true);
        iconos[0].SetActive(true);
        iconos[1].SetActive(true);
    }

    private void Girar()
    {
        Quaternion objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, Gata.position.y, transform.position.z) - Gata.position);
        Gata.rotation = Quaternion.RotateTowards(Gata.rotation, objetivo, 200 * Time.deltaTime);
    }

    private void ComprobarMision()
    {
        if (ControladorMisiones.MisionPrincipal == null) return;
        if (ControladorMisiones.MisionActual == null) return;

        if (Principal)
        {
            if (ControladorMisiones.MisionPrincipal == Misionequeespera && ControladorMisiones.MisionPCompleta)
            {
                if (GetComponent<Scr_EventosGuardado>() != null)
                {
                    Debug.Log("Guardando progreso de diálogo");
                    GetComponent<Scr_EventosGuardado>().EventoDialogo(sistemaDialogos.DialogoActual, "Gusano");
                }
                ControladorMisiones.MisionPCompleta = false;
                ControladorMisiones.MisionCompleta = false;
                if (ControladorMisiones.MisionActual == ControladorMisiones.MisionPrincipal)
                {
                    ControladorMisiones.MisionActual= null;
                }
                if (vaCambio && Principal)
                {
                    Principal = false;
                }
                else if(vaCambio && !Principal)
                {
                    Principal = true;
                }

                ControladorMisiones.MisionPrincipal = null;
                sistemaDialogos.LineaActual = 0;
                sistemaDialogos.Leido = false;
                sistemaDialogos.DialogoActual++;
                misionespera++;
            }
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Gata")) return;

        iconos[0].SetActive(true);
        iconos[1].SetActive(true);
        estaAdentro = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Gata")) return;

        iconos[0].SetActive(false);
        iconos[1].SetActive(false);
        estaAdentro = false;
    }
}
