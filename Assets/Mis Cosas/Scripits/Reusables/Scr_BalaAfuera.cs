using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_BalaAfuera : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gata")
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
