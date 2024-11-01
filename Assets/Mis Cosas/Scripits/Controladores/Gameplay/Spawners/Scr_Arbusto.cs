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
    [SerializeField] string Habilidad;
    [SerializeField] string Habilidad2;

    [Header("Estado del Arbusto")]
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
                if (gata.GetChild(0).GetComponent<Animator>().GetBool("Recolectar"))
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
        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" || string.IsNullOrEmpty(Habilidad))
        {
            animSpeed = 2f; // Doble de velocidad si la habilidad está activa
        }

        gata.GetChild(0).GetComponent<Animator>().speed = animSpeed;

        // Ajusta el tiempo de espera según la velocidad de la animación
        yield return new WaitForSeconds(5.22f / animSpeed);
        gata.GetChild(0).GetComponent<Animator>().speed = 1;
        gata.GetChild(0).GetComponent<Animator>().speed = 1f; // Restablece la velocidad de la animación a 1x
        recolectando = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = false;

        if (tieneMoras)
        {
            if (PlayerPrefs.GetString("Habilidad:" + Habilidad2, "No") == "Si" || string.IsNullOrEmpty(Habilidad2))
            {
                DarMoras(true);
                DarFibra(true);

            }
            else
            {
                DarMoras(false);
                DarFibra(false);
            }
            tieneMoras = false;
        }
        tipoActual = 0;
        GetComponent<MeshRenderer>().material = tipos[tipoActual];
    }

    void DarMoras(bool DaDoble)
    {
        int cantidad = Random.Range(minimoMaximo[0], minimoMaximo[1]);
        if (DaDoble)
        {
            cantidad = cantidad * 2;
        }
        ActualizarInventario(cantidad, objetosQueDa[tipoActual]);
    }

    void DarFibra(bool DaDoble)
    {
        int cantidad = Random.Range(minimoMaximo[2], minimoMaximo[3]);
        if (DaDoble)
        {
            cantidad = cantidad * 2;
        }
        ActualizarInventario(cantidad, objetosQueDa[0]);
    }

    void ActualizarInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        Scr_Inventario inventario = gata.GetChild(6).GetComponent<Scr_Inventario>();
        inventario.AgregarObjeto(cantidad, objeto.Nombre);
        Scr_ObjetosAgregados controlador = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<Scr_ObjetosAgregados>();
        if (controlador.Lista.Contains(objeto))
        {
            int indice = controlador.Lista.IndexOf(objeto);
            controlador.Cantidades[indice] += cantidad;
            if (indice <= 3)
            {
                controlador.Tiempo[indice] = 2;
            }
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
        gata.GetChild(2).gameObject.SetActive(true);
        gata.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = tecla;
        gata.GetChild(2).GetChild(0).GetComponent<Image>().sprite = teclaIcono;
        gata.GetChild(2).GetChild(1).GetComponent<Image>().sprite = icono;
    }

    void DesactivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
        gata.GetChild(2).gameObject.SetActive(false);
    }
}
