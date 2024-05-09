using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ActivadorCinematica : MonoBehaviour
{
    [SerializeField] GameObject Elementos;
    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.transform.parent.tag != "Gata" && PlayerPrefs.GetString("Cinematica "+ gameObject.transform.parent.name,"No")== "No")
        {
            Elementos.SetActive(true);
            PlayerPrefs.SetString("Cinematica " + gameObject.transform.parent.name, "Si");
        }
    }
}
