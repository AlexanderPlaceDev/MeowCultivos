using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scr_Dialogos : MonoBehaviour
{

    [SerializeField, TextArea(4, 6)] public string[] Lineas;
    [SerializeField] GameObject PanelDialogo;
    [SerializeField] public TextMeshProUGUI Texto;
    [SerializeField] Scr_ControladorDialogos ControladorDialogos;
    [SerializeField] float VelocidadAlHablar;
    [SerializeField] GameObject CamaraDialogo;
    [SerializeField] GameObject CamaraGata;
    [SerializeField] float VelocidadGiro;

    public bool EstaEnRango;
    public bool Comenzo;
    public bool YaLeido;
    int LineaActual;

    GameObject Gata;
    GameObject ControladorUI;

    void Start()
    {
        Gata = GameObject.Find("Gata");
        try
        {
            ControladorUI = Gata.transform.GetChild(2).gameObject;
        }
        catch { }
        ControladorDialogos = GetComponent<Scr_ControladorDialogos>();
    }

    void Update()
    {
        if (ControladorUI != null)
        {
            if (EstaEnRango && Input.GetKeyDown(KeyCode.E) && Time.timeScale == 1)
            {
                if (!Comenzo)
                {
                    IniciarDialogo();
                }
                else
                {
                    if (Texto.text == Lineas[LineaActual])
                    {
                        LineaSiguiente();
                    }
                    else
                    {
                        StopAllCoroutines();
                        Texto.text = Lineas[LineaActual];
                    }
                }
            }
        }
        else
        {
            if (EstaEnRango && Input.GetKeyDown(KeyCode.E) && Time.timeScale == 1)
            {
                if (!Comenzo)
                {
                    IniciarDialogo();
                }
                else
                {
                    if (Texto.text == Lineas[LineaActual])
                    {
                        LineaSiguiente();
                    }
                    else
                    {
                        StopAllCoroutines();
                        Texto.text = Lineas[LineaActual];
                    }
                }
            }
        }


        if(Comenzo && CamaraDialogo !=null)
        {
            Quaternion Objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, Gata.transform.position.y, transform.position.z) - Gata.transform.position);
            Gata.transform.rotation = Quaternion.RotateTowards(Gata.transform.rotation, Objetivo, VelocidadGiro * Time.deltaTime);
        }

    }

    public void IniciarDialogo()
    {
        Comenzo = true;
        PanelDialogo.SetActive(true);
        if (CamaraDialogo != null)
        {
            CamaraGata.SetActive(false);
            CamaraDialogo.SetActive(true);
        }
        try
        {
            Gata.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Gata.GetComponent<Scr_Movimiento>().enabled = false;
            Gata.GetComponent<Scr_GiroGata>().enabled = false;
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        catch
        {

        }

        LineaActual = 0;
        StartCoroutine(MostrarLinea());

    }

    public void IniciarDialogoCinematica()
    {
        EstaEnRango = true;
        Comenzo = true;
        PanelDialogo.SetActive(true);
        if (CamaraDialogo != null)
        {
            CamaraGata.SetActive(false);
            CamaraDialogo.SetActive(true);
        }
        try
        {
            Gata.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Gata.GetComponent<Scr_Movimiento>().enabled = false;
            Gata.GetComponent<Scr_GiroGata>().enabled = false;
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        catch
        {

        }

        LineaActual = 0;
        StartCoroutine(MostrarLinea());

    }

    public IEnumerator MostrarLinea()
    {
        Texto.text = "";
        foreach (char ch in Lineas[LineaActual])
        {
            Texto.text += ch;
            yield return new WaitForSeconds(VelocidadAlHablar);
        }
    }

    public void LineaSiguiente()
    {
        LineaActual++;
        if (LineaActual < Lineas.Length)
        {
            StartCoroutine(MostrarLinea());
        }
        else
        {
            if (CamaraDialogo != null)
            {
                CamaraGata.SetActive(true);
                CamaraDialogo.SetActive(false);
            }
            Comenzo = false;
            YaLeido = true;
            PanelDialogo.SetActive(false);
            EstaEnRango = false;
            try
            {
                Debug.Log("Entra");
                Gata.GetComponent<Scr_Movimiento>().enabled = true;
                Gata.GetComponent<Scr_GiroGata>().enabled = true;
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(false);
            }
            catch
            {

            }

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = true;
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = false;
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
    }

}
