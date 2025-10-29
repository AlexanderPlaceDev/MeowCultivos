using System.Collections;
using UnityEngine;

public class Musica_fondo : MonoBehaviour
{
    [Header("Clips de música de dia")]
    public AudioClip[] Musica_Dia;
    public int HoraInicioDia;
    [Header("Clips de música de noche")]
    public AudioClip[] Musica_Noche;
    public int HoraInicioNoche;

    [Header("Tiempo de espera aleatorio (segundos)")]
    public float MintiempoEspera = 2f;
    public float MaxtiempoEspera = 5f;

    private AudioSource source;
    private Coroutine soundLoopCoroutine;
    private AudioClip ultimoClip;

    private Scr_ControladorTiempo Tiempo;

    void Start()
    {
        source = GetComponent<AudioSource>();
        Tiempo = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
        if (source == null)
        {
            Debug.LogError("No se encontró un AudioSource en este objeto.");
            return;
        }


        if (Tiempo.HoraActual >= HoraInicioDia && Tiempo.HoraActual < HoraInicioNoche)
        {
            IniciaLoopAtealorio(Musica_Dia);

        }
        else
        {
            IniciaLoopAtealorio(Musica_Noche);
        }
    }

    // Iniciar loop aleatorio
    public void IniciaLoopAtealorio(AudioClip[] sonidos)
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

        soundLoopCoroutine = StartCoroutine(LoopAleatorioSonido(sonidos));
    }

    // Detener loop aleatorio
    public void DetenerLoopAleatorio()
    {
        if (soundLoopCoroutine != null)
        {
            StopCoroutine(soundLoopCoroutine);
            soundLoopCoroutine = null;
        }

        if (source.isPlaying)
            source.Stop();
    }

    // Corutina que reproduce sonidos en loop
    IEnumerator LoopAleatorioSonido(AudioClip[] clips)
    {
        // 🔹 Primera canción inmediatamente al iniciar
        AudioClip primerClip = clips[Random.Range(0, clips.Length)];
        if (primerClip != null)
        {
            source.clip = primerClip;
            source.Play();
            ultimoClip = primerClip;
            yield return new WaitForSeconds(primerClip.length);
        }

        // 🔹 Ahora sí empieza el bucle con esperas aleatorias
        while (true)
        {
            float delay = Random.Range(MintiempoEspera, MaxtiempoEspera);
            yield return new WaitForSeconds(delay);

            AudioClip clip;
            do
            {
                clip = clips[Random.Range(0, clips.Length)];
            } while (clip == ultimoClip && clips.Length > 1);

            if (clip != null && !source.isPlaying)
            {
                source.clip = clip;
                source.Play();
                Debug.Log("Reproduciendo: " + clip.name);

                ultimoClip = clip;
                yield return new WaitForSeconds(clip.length);
            }
        }
    }
}
