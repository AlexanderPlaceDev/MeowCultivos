using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_BalaAfuera : MonoBehaviour
{
    public GameObject Padre;
    bool Choco;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gata")
        {
            Choco = true;
            Destroy(Padre);
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            if (!Choco)
            {
                Destroy(gameObject);
            }
        }
    }
}
