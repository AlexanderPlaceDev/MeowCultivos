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

    public bool EstaEnRango;
    public bool Comenzo;
    public bool YaLeido;
    int LineaActual;

    GameObject Gata;
    GameObject Radio;
    GameObject ControladorUI;

    void Start()
    {
        Gata = GameObject.Find("Gata");
        try
        {
            ControladorUI = Gata.transform.GetChild(2).gameObject;
            Radio = GameObject.Find("Canvas").transform.GetChild(3).gameObject;
        }
        catch { }
        ControladorDialogos = GetComponent<Scr_ControladorDialogos>();
    }

    void Update()
    {
        if (ControladorUI != null)
        {
            if (EstaEnRango && Input.GetKeyDown(KeyCode.E) && Time.timeScale == 1 && !ControladorUI.GetComponent<Scr_ControladorUI>().MochilaActiva)
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


    }

    public void IniciarDialogo()
    {
        Comenzo = true;
        PanelDialogo.SetActive(true);
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
            Comenzo = false;
            YaLeido = true;
            PanelDialogo.SetActive(false);
            EstaEnRango = false;
            try
            {
                Radio.GetComponent<Scr_Radio>().Comenzo = false;
                Radio.transform.GetChild(1).gameObject.SetActive(false);
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
