using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_EnemigoFuera : MonoBehaviour
{

    [SerializeField] public int VidaActual;
    [SerializeField] public int VidaMaxima;
    [SerializeField] public int Velocidad;
    [SerializeField] public float RadioDeDeambulacion;
    
    void Morir()
    {
        Destroy(gameObject);
    }

}
