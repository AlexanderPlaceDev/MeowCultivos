using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_ActivadorDialogos : MonoBehaviour
{
    private bool estaAdentro = false;

    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private GameObject[] iconos;
    [SerializeField] private GameObject camara;
    [SerializeField] private GameObject camaraGata;
    [SerializeField] private GameObject CanvasNPC;
    [SerializeField] private bool Principal;
    [SerializeField] private Scr_CreadorMisiones[] MisionesSecundarisDar;
    public Scr_CreadorMisiones Misionesqueespera;
    public int misionespera;
    private bool CanvasActivo = false;
    private Scr_SistemaDialogos sistemaDialogos;
    private Scr_ControladorMisiones ControladorMisiones;
    private Transform Gata;

    private CinemachineBrain brain;
    private float TransicionDuracion = 1f;
    private void Start()
    {
        Gata = GameObject.Find("Gata").transform;
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

    public void BotonHablar()
    {
        CanvasActivo = false;
        CanvasNPC.SetActive(false);
        ComprobarMision();
        ActivarDialogo();
    }

    public void BotonMisionesSecundarias()
    {
        CanvasActivo = false;
        CanvasNPC.SetActive(false);
        ComprobarMision();
        ActivarDialogo();
    }
    public void Salir()
    {
        CanvasActivo = false;
        CanvasNPC.SetActive(false);
        camara.SetActive(true);
        camaraGata.SetActive(true);
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
            if (ControladorMisiones.MisionPrincipal == Misionesqueespera && ControladorMisiones.MisionPCompleta)
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
                ControladorMisiones.MisionPrincipal = null;
                sistemaDialogos.LineaActual = 0;
                sistemaDialogos.Leido = false;
                sistemaDialogos.DialogoActual++;
                misionespera++;
            }
        }
        else
        {
            for (int i = 0; i < ControladorMisiones.MisionesExtra.Count; i++)
            {
                if (ControladorMisiones.MisionesScompletas[i]== Misionesqueespera && ControladorMisiones.MisionesScompletas[i])
                {

                }
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
