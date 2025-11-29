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

    // Estados de las capas
    private bool lowActive = false;
    private bool midActive = false;

    void Start()
    {
        // Reproduce todas para mantener sincronía
        Base.loop = true;
        Bajo.loop = true;
        Efectos.loop = true;

        Base.Play();
        Bajo.Play();
        Efectos.Play();

        // Solo la base audible al inicio
        Bajo.volume = 0f;
        Efectos.volume = 0f;
        Base.volume = 1f;
    }

    void Update()
    {
        // Volumen dinámico por capas
        Bajo.volume = Mathf.MoveTowards(Bajo.volume, lowActive ? 1f : 0f, fadeSpeed * Time.deltaTime);
        Efectos.volume = Mathf.MoveTowards(Efectos.volume, midActive ? 1f : 0f, fadeSpeed * Time.deltaTime);
    }

    // Activar o desactivar capas
    public void activarBajos(bool Activo)
    {
        lowActive = Activo;
    }

    public void activarEfectos(bool Activo)
    {
        midActive = Activo;
    }
}
