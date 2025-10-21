using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_AreaDeAtaqueEnemiga : MonoBehaviour
{
    public bool EstaDentro;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Gata")
        {
            EstaDentro = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Gata")
        {
            EstaDentro = false;
        }
    }
}
