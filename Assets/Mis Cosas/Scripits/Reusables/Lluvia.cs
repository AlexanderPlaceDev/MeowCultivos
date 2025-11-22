using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lluvia : MonoBehaviour
{
    public float Friccion = .3f;
    public float Fuerza = 100f;

    public void Intensidad(float fuerza,float ficcion)
    {
        Fuerza = fuerza;
        Friccion = ficcion;
        ParticleSystem part = GetComponent<ParticleSystem>();
        part.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = part.main;

        // Normalizamos la fuerza entre 100 y 300 → produce un valor 0–1
        float t = Mathf.InverseLerp(100, 300, Fuerza);

        // t=0 → 3000 partículas
        // t=1 → 200 partículas
        float maxParticulas = Mathf.Lerp(3000f, 200f, t);

        main.maxParticles = Mathf.RoundToInt(maxParticulas);

        // Reiniciar el sistema para aplicar cambios
        part.Play();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            Scr_Movimiento rb = other.GetComponent<Scr_Movimiento>();
            if (rb != null)
            {
                rb.EstaLloviendo = true;
                rb.MultiplicadorResbalado = Friccion;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            Scr_Movimiento rb = other.GetComponent<Scr_Movimiento>();
            if (rb != null)
            {
                rb.EstaLloviendo = false;
            }
        }
    }
}
