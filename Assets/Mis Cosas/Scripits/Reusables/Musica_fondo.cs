using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musica_fondo : MonoBehaviour
{
    public AudioClip[] musica_fondo;
    private AudioSource source;
    private Coroutine soundLoopCoroutine;
    public float MintiempoEspera;
    public float MaztiempoEspera;

    void Start()
    {
        source = GetComponent<AudioSource>();
        //aplica el volumen 
        int volumen_general = PlayerPrefs.GetInt("Volumen", 50);
        int volumen_ambiental = PlayerPrefs.GetInt("Volumen_Musica", 20);
        float volumen = (volumen_general * volumen_ambiental) / 100;
        source.volume = volumen;
        IniciaLoopAtealorio(musica_fondo, MintiempoEspera, MaztiempoEspera);
    }

    // Iniciar loop aleatorio
    public void IniciaLoopAtealorio(AudioClip[] sonidos, float minDelay, float maxDelay)
    {
        if (sonidos == null || sonidos.Length == 0) return;

        bool hasValidClip = false;
        foreach (var clip in sonidos)
        {
            if (clip != null)
            {
                hasValidClip = true;
                break;
            }
        }

        if (!hasValidClip) return;

        if (soundLoopCoroutine != null)
            StopCoroutine(soundLoopCoroutine);

        soundLoopCoroutine = StartCoroutine(LoopAleatorioSonido(sonidos, minDelay, maxDelay));
    }

    // Detener loop aleatorio
    public void DetenerLoopAleatorio()
    {
        if (soundLoopCoroutine != null)
        {
            StopCoroutine(soundLoopCoroutine);
            soundLoopCoroutine = null;
        }

        // Opcional: detener el sonido actual
        source.Stop();
    }

    // Corutina que reproduce sonidos en loop con delay aleatorio antes de cada reproducción
    IEnumerator LoopAleatorioSonido(AudioClip[] clips, float minDelay, float maxDelay)
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (clip != null)
            {
                source.clip = clip;
                source.Play();
                yield return new WaitForSeconds(clip.length);
            }
        }
    }
}
