using System.Collections;
using UnityEngine;

public class Scr_GeneradorDeSonidos : MonoBehaviour
{
    [Header("Lista de sonidos")]
    public AudioClip[] Sonidos;

    [Header("Configuración de reproducción")]
    public bool EsRandom = true;            // Si es true, elige sonidos aleatorios
    public bool Espera = true;              // Si es true, espera antes de reproducir el siguiente
    public bool UsaHoras = false;           // Si es true, solo reproduce en el rango horario
    public bool UsaProbabilidad = false;    // Si es true, usa la probabilidad de reproducción

    [Range(0, 23)]
    public int HoraMinima;
    [Range(0, 23)]
    public int HoraMaxima;

    [Range(0, 100)]
    public int Probabilidad = 100;          // Probabilidad (0 a 100) de que suene un clip

    [Header("Tiempo de espera (segundos)")]
    public float MinTiempoEspera = 1f;
    public float MaxTiempoEspera = 3f;

    private AudioSource source;
    private Coroutine sonidoLoopCoroutine;
    private int indiceActual = 0;
    private AudioClip ultimoClip;
    private Scr_ControladorTiempo Tiempo;

    void Start()
    {
        source = GetComponent<AudioSource>();
        Tiempo = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();

        if (source == null)
        {
            Debug.LogError("Scr_GeneradorDeSonidos necesita un AudioSource en el mismo GameObject.");
            return;
        }

        if (Sonidos != null && Sonidos.Length > 0)
        {
            IniciaGenerador();
        }
    }

    // Inicia la reproducción controlada
    public void IniciaGenerador()
    {
        if (sonidoLoopCoroutine != null)
            StopCoroutine(sonidoLoopCoroutine);

        sonidoLoopCoroutine = StartCoroutine(LoopSonidos());
    }

    // Detiene la reproducción
    public void DetenerGenerador()
    {
        if (sonidoLoopCoroutine != null)
        {
            StopCoroutine(sonidoLoopCoroutine);
            sonidoLoopCoroutine = null;
        }

        if (source.isPlaying)
            source.Stop();
    }

    // Corrutina principal
    IEnumerator LoopSonidos()
    {
        while (true)
        {
            // Si hay que esperar, lo hace
            if (Espera)
            {
                float delay = Random.Range(MinTiempoEspera, MaxTiempoEspera);
                yield return new WaitForSeconds(delay);
            }

            // ⏰ Restricción por hora
            if (UsaHoras && !EstaDentroDelRango(Tiempo.HoraActual))
            {
                continue; // vuelve al siguiente ciclo
            }

            // 🎲 Probabilidad de reproducción
            if (UsaProbabilidad)
            {
                int dado = Random.Range(0, 100);
                if (dado >= Probabilidad)
                {
                    continue; // vuelve al siguiente ciclo
                }
            }

            // 🎵 Obtener clip
            AudioClip clip = ObtenerClip();
            if (clip != null)
            {
                source.clip = clip;
                source.Play();

                // Espera a que termine el clip
                yield return new WaitForSeconds(clip.length);
            }
            else
            {
                yield return null;
            }
        }
    }

    // ✅ Comprueba si la hora actual está dentro del rango (soporta cruce de medianoche)
    private bool EstaDentroDelRango(int hora)
    {
        if (HoraMinima <= HoraMaxima)
        {
            // Rango normal (ej: 8 a 20)
            return hora >= HoraMinima && hora <= HoraMaxima;
        }
        else
        {
            // Rango cruzado (ej: 22 a 5)
            return hora >= HoraMinima || hora <= HoraMaxima;
        }
    }

    // Devuelve el siguiente clip dependiendo de la configuración
    private AudioClip ObtenerClip()
    {
        if (Sonidos == null || Sonidos.Length == 0) return null;

        if (EsRandom)
        {
            AudioClip clip;
            do
            {
                clip = Sonidos[Random.Range(0, Sonidos.Length)];
            } while (clip == ultimoClip && Sonidos.Length > 1);

            ultimoClip = clip;
            return clip;
        }
        else
        {
            // Orden secuencial
            AudioClip clip = Sonidos[indiceActual];
            indiceActual = (indiceActual + 1) % Sonidos.Length;
            return clip;
        }
    }
}
