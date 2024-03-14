using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorMapas : MonoBehaviour
{
    [SerializeField] bool Primero;
    [SerializeField] GameObject[] Mapas;
    [SerializeField] float Velocidad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gato Mesh")
        {
            if (!Primero)
            {
                if (Mapas[0] != null)
                {
                    Mapas[0].SetActive(false);
                }
                if (Mapas[1] != null)
                {
                    Mapas[1].SetActive(true);
                }
            }
            else
            {
                if (Mapas[0] != null)
                {
                    Mapas[0].SetActive(true);
                }
                if (Mapas[1] != null)
                {
                    Mapas[1].SetActive(false);
                }
            }

            //Tween.PositionY(Mapas[0].transform, -100, Velocidad);
            //Tween.PositionY(Mapas[1].transform, 0, Velocidad);
        }
    }
}
