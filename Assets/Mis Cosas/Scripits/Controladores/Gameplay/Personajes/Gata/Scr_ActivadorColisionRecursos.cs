using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ActivadorColisionRecursos : MonoBehaviour
{
    [SerializeField] SphereCollider Collider;

    public void Activar()
    {
        Collider.enabled=true;
    }

    public void Apagar()
    {
        Collider.enabled=false;
    }

    public void ActivarMovimiento()
    {
        transform.parent.GetComponent<Scr_Movimiento>().enabled=true;
        transform.parent.GetComponent<Scr_GiroGata>().enabled=true;
    }

}
