using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_VisionEnemigosFuera : MonoBehaviour
{
    public GameObject Gata;

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
