using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_BarrilSembradio : MonoBehaviour
{
    [SerializeField] int ID;
    [SerializeField] private Sprite icono;
    [SerializeField] private string tecla;
    [SerializeField] private Sprite teclaIcono;
    [SerializeField] private float distancia;
    [SerializeField] string Habilidad;
    [SerializeField] private float velocidadGiro;
    [SerializeField] public Scr_CreadorObjetos TipoFruta;
    [SerializeField] public int Cantidad;
    [SerializeField] public int CantidadMaxima;
    [SerializeField] Sprite[] IconosPanel;
    [SerializeField] Scr_CreadorObjetos[] FrutasQueRecolecta;
    [SerializeField] public bool UltimoDiaPlanta;

    private bool recolectando;
    private bool estaLejos;

    private Transform gata;

    void Start()
    {
        gata = GameObject.Find("Gata").GetComponent<Transform>();

        foreach(Scr_CreadorObjetos Fruta in FrutasQueRecolecta)
        {
            if(Fruta.Nombre==PlayerPrefs.GetString("BarrilSembradio Futa:" + ID, "No"))
            {
                TipoFruta = Fruta;
            }
        }

        Cantidad = PlayerPrefs.GetInt("BarrilSembradio Cantidad:" + ID, 0);
    }

    void Update()
    {
        if (TipoFruta != null)
        {
            if(PlayerPrefs.GetString("BarrilSembradio Futa:" + ID, "No") != TipoFruta.Nombre)
            {
                PlayerPrefs.SetString("BarrilSembradio Futa:" + ID, TipoFruta.Nombre);
                PlayerPrefs.SetInt("BarrilSembradio Cantidad:" + ID, Cantidad);
            }
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = TipoFruta.Icono;
            transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = Cantidad.ToString();

            if (Cantidad >= CantidadMaxima || UltimoDiaPlanta)
            {
                ColocarIconoPanel(true);
            }
            else
            {
                ColocarIconoPanel(false);
            }

            if (!recolectando)
            {
                // Si se acerca, se encienden los iconos
                if (Vector3.Distance(gata.position, transform.position) < distancia)
                {
                    estaLejos = false;
                    ActivarUI();
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
        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" && !string.IsNullOrEmpty(Habilidad))
        {
            animSpeed = 2f; // Doble de velocidad si la habilidad está activa
        }
        gata.GetComponent<Animator>().speed = animSpeed;

        yield return new WaitForSeconds(5.22f / animSpeed);
        gata.GetComponent<Animator>().speed = 1;

        recolectando = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = false;
        if (TipoFruta != null)
        {
            DarObjeto();
            PlayerPrefs.DeleteKey("BarrilSembradio Futa:" + ID);
            PlayerPrefs.DeleteKey("BarrilSembradio Cantidad:" + ID);
            transform.GetChild(0).gameObject.SetActive(false);
            TipoFruta = null;
        }
    }

    void DarObjeto()
    {
        ActualizarInventario(Cantidad, TipoFruta);
    }

    void ActualizarInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        Scr_Inventario inventario = gata.GetChild(7).GetComponent<Scr_Inventario>();
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
        gata.GetChild(3).gameObject.SetActive(true);
        gata.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = tecla;
        gata.GetChild(3).GetChild(0).GetComponent<Image>().sprite = teclaIcono;
        gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = icono;
    }

    void DesactivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
        gata.GetChild(3).gameObject.SetActive(false);
    }

    public void ColocarIconoPanel(bool EnAlerta)
    {
        if(EnAlerta)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = IconosPanel[1];
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = IconosPanel[0];
        }
    }
}
