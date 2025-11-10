using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boton_pocion : MonoBehaviour
{
    Scr_ControladorBatalla ControladorBatalla;
    Scr_ControladorUIBatalla ControladorUIBatalla;
    public string NombrePocion;
    public int No;
    // Start is called before the first frame update
    void Start()
    {
        ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        ControladorUIBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorUIBatalla>();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void PocionSeleccionada()
    {
        ControladorBatalla.Pocion = NombrePocion;
        ControladorUIBatalla.PocionSelec = true;
    }
    public void Boton_On()
    {
        if (!ControladorUIBatalla.PocionSelec)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        ControladorUIBatalla.MostrarPocion(No);

    }
    public void Boton_Exit()
    {
        if (!ControladorUIBatalla.PocionSelec)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        ControladorUIBatalla.EsconderDescipcion();
    }
}
