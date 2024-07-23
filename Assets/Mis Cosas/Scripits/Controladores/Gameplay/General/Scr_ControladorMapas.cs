using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorMapas : MonoBehaviour
{
    [SerializeField] GameObject[] MapasQueActiva;
    [SerializeField] GameObject[] MapasQueDesactiva;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gato Mesh")
        {

            foreach(GameObject Mapa in MapasQueActiva)
            {
                Mapa.SetActive(true);
            }
            foreach (GameObject Mapa in MapasQueDesactiva)
            {
                Mapa.SetActive(false);
            }

        }
    }
}
