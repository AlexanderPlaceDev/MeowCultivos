using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_SpawnerRecolectable : MonoBehaviour
{
    [Header("Configuraci�n del spawner")]
    [SerializeField] private Sprite icono;
    [SerializeField] private string tecla;
    [SerializeField] private Sprite teclaIcono;
    [SerializeField] private float distancia;
    [SerializeField] private float velocidadGiro;
    [SerializeField] private Scr_CreadorObjetos objetoQueDa;
    [SerializeField] private int[] minimoMaximo;
    [SerializeField] string Habilidad;

    [Header("Estado del spawner")]
    public bool TieneObjeto = true;
    private bool recolectando;
    private bool estaLejos;

    private Transform gata;

    void Start()
    {
        gata = GameObject.Find("Gata").GetComponent<Transform>();
    }

    void Update()
    {
        if (TieneObjeto)
        {
            if (!recolectando)
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

        }
        else
        {
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
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

        // Verificar si la habilidad est� activa o no
        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" && !string.IsNullOrEmpty(Habilidad))
        {
            animSpeed = 2f; // Doble de velocidad si la habilidad est� activa
        }
        gata.GetChild(0).GetComponent<Animator>().speed = animSpeed;

        yield return new WaitForSeconds(5.22f/animSpeed);
        gata.GetChild(0).GetComponent<Animator>().speed = 1;

        recolectando = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = false;
        if (TieneObjeto)
        {
            DarObjeto();
            GetComponent<Collider>().enabled = false;
            TieneObjeto = false;
        }
    }

    void DarObjeto()
    {
        int cantidad = Random.Range(minimoMaximo[0], minimoMaximo[1]);
        ActualizarInventario(cantidad, objetoQueDa);
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
