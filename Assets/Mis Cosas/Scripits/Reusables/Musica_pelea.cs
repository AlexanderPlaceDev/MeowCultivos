using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musica_pelea : MonoBehaviour
{
    [Header("Pistas")]
    public AudioSource Base;   // Siempre suena
    public AudioSource Bajo;    // Capa baja
    public AudioSource Efectos;    // Capa media

    [Header("Fade de capas")]
    public float fadeSpeed = 2f;

    [Header("Porcentajes de activación")]
    [Range(0, 100)] public int CuandoDetenerBajo = 50;
    [Range(0, 100)] public int CuandoEmpezarEfectos = 50;

    private float PorcentajeVida;

    void Start()
    {
        Base.loop = true;
        Bajo.loop = true;
        Efectos.loop = true;

        Base.Play();
        Bajo.Play();
        Efectos.Play();

        Base.volume = 1f;
        Bajo.volume = 0f;
        Efectos.volume = 0f;
    }

    void Update()
    {
        if(!GameObject.Find("Personaje").GetComponent<Scr_Movimiento>().enabled)
        {
            return;
        }

        // ----------------------------
        //    VOLUMEN DEL BAJO
        // ----------------------------
        // Si la vida está por ENCIMA del límite → volumen 0 (apagado)
        // Si está por DEBAJO → sube proporcionalmente hasta volumen = 1
        float targetBajoVolume = 0f;

        if (PorcentajeVida < CuandoDetenerBajo)
        {
            targetBajoVolume = 1f - (PorcentajeVida / CuandoDetenerBajo);
            //  Vida = 50% (igual a limite) → 1 - (50/50)=0
            //  Vida = 0%  → 1 - 0 = 1
        }

        Bajo.volume = Mathf.MoveTowards(Bajo.volume, targetBajoVolume, fadeSpeed * Time.deltaTime);



        // ----------------------------
        //   VOLUMEN DE EFECTOS
        // ----------------------------
        // Si la vida está por DEBAJO del inicio → volumen 0 (apagado)
        // Si está por ENCIMA → sube hasta volumen = 1
        float targetEfectosVolume = 0f;

        if (PorcentajeVida > CuandoEmpezarEfectos)
        {
            float rango = 100f - CuandoEmpezarEfectos;     // ejemplo: 100 - 50 = 50
            float diferencia = PorcentajeVida - CuandoEmpezarEfectos;
            targetEfectosVolume = Mathf.Clamp01(diferencia / rango);

            // Vida = 50% → volumen 0
            // Vida = 100% → volumen 1
        }

        Efectos.volume = Mathf.MoveTowards(Efectos.volume, targetEfectosVolume, fadeSpeed * Time.deltaTime);
    }

    // Se llama desde afuera
    public void ConseguirPorcentajeVida(float Porcentaje)
    {
        PorcentajeVida = Mathf.Clamp(Porcentaje, 0f, 100f);
    }
}
