using TMPro;
using UnityEngine;


public class Scr_ActivadorDialogos : MonoBehaviour
{
    bool estaAdentro = false;
    [SerializeField] GameObject panelDialogo;
    [SerializeField] GameObject[] iconos;
    [SerializeField] GameObject camara;
    [SerializeField] GameObject camaraGata;
    private Scr_SistemaDialogos sistemaDialogos;
    private Scr_ControladorMisiones ControladorMisiones;
    private Transform Gata;

    private void Start()
    {
        Gata = GameObject.Find("Gata").transform;
        camaraGata = GameObject.Find("Camara 360");
        sistemaDialogos = GetComponent<Scr_SistemaDialogos>();
        ControladorMisiones = GameObject.Find("Gata").transform.GetChild(3).GetComponent<Scr_ControladorMisiones>();
    }

    void Update()
    {
        if (estaAdentro)
        {
            if (panelDialogo.activeSelf)
            {
                Girar();
                Gata.GetComponent<Scr_Movimiento>().enabled = false;
            }
            else
            {
                Gata.GetComponent<Scr_Movimiento>().enabled = true;

            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ComprobarMision();

                ActivarDialogo();
            }

            if (sistemaDialogos.Leido && !sistemaDialogos.Leyendo && !panelDialogo.activeSelf)
            {
                DesactivarDialogo();
            }
        }
        else
        {
            sistemaDialogos.EnPausa = true;
        }
    }

    void ActivarDialogo()
    {
        iconos[0].SetActive(false);
        iconos[1].SetActive(false);
        camara.SetActive(true);
        camaraGata.SetActive(false);
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
                if (sistemaDialogos.Dialogos[sistemaDialogos.DialogoActual].Lineas[sistemaDialogos.LineaActual] == panelDialogo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
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

    void Girar()
    {
        Quaternion Objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, Gata.position.y, transform.position.z) - Gata.position);
        Gata.rotation = Quaternion.RotateTowards(Gata.rotation, Objetivo, 200 * Time.deltaTime);
    }

    void DesactivarDialogo()
    {
        camara.SetActive(false);
        camaraGata.SetActive(true);
        iconos[0].SetActive(true);
        iconos[1].SetActive(true);
    }

    void ComprobarMision()
    {
        if (ControladorMisiones.MisionActual != null)
        {
            if (ControladorMisiones.MisionCompleta)
            {
                //Guardar Dialogo
                if (GetComponent<Scr_EventosGuardado>() != null)
                {
                    Debug.Log("Entra1");
                    GetComponent<Scr_EventosGuardado>().EventoDialogo(sistemaDialogos.DialogoActual, "Gusano");
                }
                ControladorMisiones.MisionActual = null;
                sistemaDialogos.LineaActual = 0;
                sistemaDialogos.Leido = false;
                sistemaDialogos.DialogoActual++; // Avanzar al siguiente diálogo
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gata"))
        {
            iconos[0].SetActive(true);
            iconos[1].SetActive(true);
            estaAdentro = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gata"))
        {
            iconos[0].SetActive(false);
            iconos[1].SetActive(false);
            estaAdentro = false;
        }
    }
}