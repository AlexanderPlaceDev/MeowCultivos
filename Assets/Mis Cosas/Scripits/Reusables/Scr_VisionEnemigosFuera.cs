using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_VisionEnemigosFuera : MonoBehaviour
{


    Scr_MovimientoEnemigosFuera Movimiento;

    private void Start()
    {
        Movimiento = transform.parent.GetComponent<Scr_MovimientoEnemigosFuera>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gata")
        {
            Movimiento.Jugador = other.gameObject.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Gata")
        {
            Movimiento.Jugador = null;
        }
    }
}
