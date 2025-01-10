using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ActivadorCinematica : MonoBehaviour
{
    [SerializeField] GameObject Elementos;
    [SerializeField] string Cinematica;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Gata" && PlayerPrefs.GetString("Cinematica "+ Cinematica,"No")== "No")
        {
            Elementos.SetActive(true);
        }
    }
}
