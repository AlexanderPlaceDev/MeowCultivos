using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_VisionEnemigosFuera : MonoBehaviour
{
    public GameObject Gata;
    public bool seguir;
    //chea si esta en dialogo o cinematica para que no pueda entrar en combate
    private void Update()
    {
        if(Gata != null)
        {
            if (Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar)
            {
                seguir = true;
            }
            else
            {
                seguir = false;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Gata")
        {
            Gata = other.gameObject;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gata")
        {
            Gata = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Gata")
        {
            Gata = null;
        }
    }
}
