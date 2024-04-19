using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_AjustarCinematica : MonoBehaviour
{
    [SerializeField] GameObject Gata;
    [SerializeField] GameObject[] Objetos;
    [SerializeField] Vector3[] Posiciones;
    [SerializeField] Quaternion[] Rotaciones;
    

    public void Setear(int Escena)
    {
        for (int i = 0; i < Objetos.Length; ++i)
        {
            Objetos[i].transform.position = Posiciones[i*Escena];
            Objetos[i].transform.rotation = Rotaciones[i*Escena];
        }
    }
}
