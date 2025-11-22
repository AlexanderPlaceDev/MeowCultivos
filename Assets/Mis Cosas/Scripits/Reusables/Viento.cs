using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Viento : MonoBehaviour
{
    public float MinFuerza = 100f;
    public float MaxFuerza = 300f;
    public float FuerzaViento = 100f; 

    public float FuerzaNueva()
    {
        return Random.Range(MinFuerza, MaxFuerza);
    }
    public void Intensidad(float fuerza)
    {
        FuerzaViento = fuerza;
        ParticleSystem part = GetComponent<ParticleSystem>();
        part.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = part.main;

        // Normalizamos la fuerza entre 0 y 1
        float t = Mathf.InverseLerp(MinFuerza, MaxFuerza, FuerzaViento);

        // Cuando t es 0 → duración = 5
        // Cuando t es 1 → duración = 0.01
        float duracion = Mathf.Lerp(3f, 0.05f, t);

        main.duration = duracion;

        // Reiniciar el sistema para aplicar cambios
        part.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 windDir = transform.forward; // viento hacia atrás
                rb.AddForce(windDir * FuerzaViento, ForceMode.Acceleration);
            }
        }
        
    }
}
