using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorMenuHabilidades : MonoBehaviour
{

    [SerializeField] GameObject[] Botones;
    GameObject BotonActual;

    string HabilidadActual;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void EntraHabilidad(string Habilidad)
    {
        HabilidadActual = Habilidad;

        foreach(GameObject Boton in Botones)
        {
            if(Boton.gameObject.name== HabilidadActual)
            {
                Boton.transform.GetChild(2).gameObject.SetActive(true);
                BotonActual = Boton;
                break;
            }
        }
    }

    public void SaleHabilidad()
    {
        HabilidadActual = "";
        BotonActual.transform.GetChild(2).gameObject.SetActive(false);
        BotonActual = null;
    }
}
