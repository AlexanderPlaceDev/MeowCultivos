using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scr_Arbusto : MonoBehaviour
{
    [Header("Configuración del Arbusto")]
    [SerializeField] private Sprite icono;
    [SerializeField] private string tecla;
    [SerializeField] private Sprite teclaIcono;
    [SerializeField] private Material[] tipos;
    [SerializeField] private float distancia;
    [SerializeField] private float velocidadGiro;
    [SerializeField] private Scr_CreadorObjetos[] objetosQueDa;
    [SerializeField] private int[] minimoMaximo;
    [SerializeField] string HabilidadGuante;
    [SerializeField] string HabilidadRamas;
    [SerializeField] Scr_CreadorObjetos Rama;

    [Header("Estado del Arbusto")]
    private Scr_CambiadorBatalla batalla;
    private int tipoActual = 0;
    private bool recolectando;
    private bool tieneMoras;
    private bool estaLejos;

    private Transform gata;

    void Start()
    {
        gata = GameObject.Find("Gata").GetComponent<Transform>();
        tipoActual = Random.Range(0, 4);
        GetComponent<MeshRenderer>().material = tipos[tipoActual];
        tieneMoras = (tipoActual > 0);
        batalla=GetComponent<Scr_CambiadorBatalla>();
        batalla.Fruta=objetosQueDa[tipoActual].Nombre;
        batalla.Item = objetosQueDa[tipoActual].Nombre;
    }

    void Update()
    {
        // Verificar si tiene moras y no está recolectando
        if (tieneMoras && !recolectando)
        {
            // Si se acerca, se encienden los iconos
            if (Vector3.Distance(gata.position, transform.position) < distancia)
            {
                estaLejos = false;
                ActivarUI();
                if (Input.GetKeyDown(KeyCode.E) && !batalla.escenaCargada)
                {
                    batalla.Iniciar();
                }
                if (gata.GetComponent<Animator>().GetBool("Recolectando"))
                {
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = true;
                    recolectando = true;
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
                    StartCoroutine(Esperar());
                }
            }
            else
            {
                if (!estaLejos)
                {
                    DesactivarUI();
                    estaLejos = true;
                }
            }
        }
        if (recolectando)
        {
            DesactivarUI();
            Quaternion objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, gata.position.y, transform.position.z) - gata.position);
            gata.rotation = Quaternion.RotateTowards(gata.rotation, objetivo, velocidadGiro * Time.deltaTime);
        }
    }

    IEnumerator Esperar()
    {
        float animSpeed = 1f; // Valor por defecto

        // Verificar si la habilidad está activa o no
        if (PlayerPrefs.GetString("Habilidad:" + HabilidadGuante, "No") == "Si" || string.IsNullOrEmpty(HabilidadGuante))
        {
            animSpeed = 2f; // Doble de velocidad si la habilidad está activa
        }

        gata.GetComponent<Animator>().speed = animSpeed;

        // Ajusta el tiempo de espera según la velocidad de la animación
        yield return new WaitForSeconds(5.22f / animSpeed);
        gata.GetComponent<Animator>().speed = 1f; // Restablece la velocidad de la animación a 1x
        recolectando = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = false;

        if (tieneMoras)
        {
            if (PlayerPrefs.GetString("Habilidad:" + HabilidadRamas, "No") == "Si" || string.IsNullOrEmpty(HabilidadRamas))
            {
                int cantidad = Random.Range(minimoMaximo[0], minimoMaximo[1]);
                ActualizarInventario(cantidad, Rama);
            }
            DarMoras();
            DarFibra();
            tieneMoras = false;
        }
        tipoActual = 0;
        GetComponent<MeshRenderer>().material = tipos[tipoActual];
    }

    void DarMoras()
    {
        int cantidad = Random.Range(minimoMaximo[0], minimoMaximo[1]);
        ActualizarInventario(cantidad, objetosQueDa[tipoActual]);
    }

    void DarFibra()
    {
        int cantidad = Random.Range(minimoMaximo[2], minimoMaximo[3]);
        ActualizarInventario(cantidad, objetosQueDa[0]);
    }

    void ActualizarInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        Scr_ObjetosAgregados controlador = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<Scr_ObjetosAgregados>();

        if (controlador.Lista.Contains(objeto))
        {
            int indice = controlador.Lista.IndexOf(objeto);
            controlador.Cantidades[indice] += cantidad;
            if (indice <= 3)
                controlador.Tiempo[indice] = 2;
        }
        else
        {
            controlador.Lista.Add(objeto);
            controlador.Cantidades.Add(cantidad);
        }
    }


    void ActivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = true;
        gata.GetChild(3).gameObject.SetActive(true);
        gata.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = tecla;
        gata.GetChild(3).GetChild(0).GetComponent<Image>().sprite = teclaIcono;
        gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = icono;
        GameObject ui = gata.GetChild(3).GetChild(2).gameObject;
        GameObject ui2 = gata.GetChild(3).GetChild(3).gameObject;

        if (!ui.activeSelf)
        {
            ui.SetActive(true);
        }
        if (!ui2.activeSelf)
        {
            ui2.SetActive(true);
        }

        gata.GetChild(3).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "E";
        gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(1,0,0);
        gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(3, 0, 0);
    }
    void DesactivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
        gata.GetChild(3).gameObject.SetActive(false);
        gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
        gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);
        gata.GetChild(3).GetChild(2).gameObject.SetActive(false);
        gata.GetChild(3).GetChild(3).gameObject.SetActive(false);
    }
}
