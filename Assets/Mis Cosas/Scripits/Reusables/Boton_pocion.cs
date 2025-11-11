using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boton_pocion : MonoBehaviour
{
    Scr_ControladorBatalla ControladorBatalla;
    Scr_ControladorUIBatalla ControladorUIBatalla;
    public string NombrePocion;
    public int No;
    public Color[] ColorSelec;
    // Start is called before the first frame update
    void Start()
    {
        ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        ControladorUIBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorUIBatalla>();
        transform.GetChild(0).gameObject.SetActive(false);
        Boton_Exit();
    }

    public void PocionSeleccionada()
    {
        ControladorBatalla.Pocion = NombrePocion;
        ControladorUIBatalla.NoPocion = No;
        ControladorUIBatalla.PocionSelec = true;
        ControladorUIBatalla.ChecarPocionActual();
    }
    public void Boton_On()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        if (!ControladorUIBatalla.PocionSelec || ControladorUIBatalla.NoPocion == No)
        {
            transform.GetChild(0).gameObject.GetComponent<Image>().color = ColorSelec[0];
        }
        else
        {
            transform.GetChild(0).gameObject.GetComponent<Image>().color = ColorSelec[1];
        }
        ControladorUIBatalla.MostrarPocion(No);
    }
    public void Boton_Exit()
    {
        if (!ControladorUIBatalla.PocionSelec)
        {
            transform.GetChild(0).gameObject.GetComponent<Image>().color = ColorSelec[0];
            transform.GetChild(0).gameObject.SetActive(false);
        }
        if (ControladorUIBatalla.NoPocion == No)
        {
            transform.GetChild(0).gameObject.GetComponent<Image>().color = ColorSelec[0];
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.GetComponent<Image>().color = ColorSelec[1];
            transform.GetChild(0).gameObject.SetActive(false);
        }
        ControladorUIBatalla.EsconderDescipcion();
    }
}
