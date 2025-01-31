using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_VisionEnemigosPelea : MonoBehaviour
{
    public GameObject Gata;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Gata"))
        {
            Gata = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gata"))
        {
            Gata = null;
        }
    }
}
