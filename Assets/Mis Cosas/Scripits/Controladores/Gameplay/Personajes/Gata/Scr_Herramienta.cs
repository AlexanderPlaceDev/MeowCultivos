using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Herramienta : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        try
        {
            if(other.GetComponent<SpawnerActivo>().UsaHacha)
            {
                other.GetComponent<SpawnerActivo>().Vida--;
            }
            if (other.GetComponent<SpawnerActivo>().UsaPico)
            {
                other.GetComponent<SpawnerActivo>().Vida--;
            }
        }
        catch{}

    }
}
