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
    // Guarda si actualmente es día o noche
    private bool esDiaActual;

    void Start()
    {
        source = GetComponent<AudioSource>();
        Tiempo = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();

        if (source == null)
        {
            Debug.LogError("No se encontró un AudioSource.");
            return;
        }

        // Detectar si inicia de día o noche
        esDiaActual = EsDia();

        // Iniciar playlist correcta
        IniciarMusicaSegunTiempo();
    }

    void Update()
    {
        // Detectar cambio de día ↔ noche
        bool ahoraEsDia = EsDia();

        if (ahoraEsDia != esDiaActual)
        {
            esDiaActual = ahoraEsDia;
            IniciarMusicaSegunTiempo();
        }
    }

    // Detener loop aleatorio
    public void DetenerLoopAleatorio() 
    { 
        if (soundLoopCoroutine != null) 
        { 
            StopCoroutine(soundLoopCoroutine); 
            soundLoopCoroutine = null; 
        } 
        if (source.isPlaying) source.Stop(); 
    }

    bool EsDia()
    {
        return Tiempo.HoraActual >= HoraInicioDia && Tiempo.HoraActual < HoraInicioNoche;
    }

    void IniciarMusicaSegunTiempo()
    {
        if (esDiaActual)
            IniciaLoopAtealorio(Musica_Dia);
        else
            IniciaLoopAtealorio(Musica_Noche);
    }

    public void IniciaLoopAtealorio(AudioClip[] sonidos)
    {
        if (sonidos == null || sonidos.Length == 0) return;

        // Detener lo anterior
        if (soundLoopCoroutine != null)
            StopCoroutine(soundLoopCoroutine);

        // Iniciar nueva playlist
        soundLoopCoroutine = StartCoroutine(LoopAleatorioSonido(sonidos));
    }

    IEnumerator LoopAleatorioSonido(AudioClip[] clips)
    {
        // Primera canción inmediata
        AudioClip primerClip = clips[Random.Range(0, clips.Length)];
        if (primerClip != null)
        {
            source.clip = primerClip;
            source.Play();
            ultimoClip = primerClip;

            yield return new WaitForSeconds(primerClip.length);
        }

        // LOOP infinito con pausa aleatoria
        while (true)
        {
            float delay = Random.Range(MintiempoEspera, MaxtiempoEspera);
            yield return new WaitForSeconds(delay);

            AudioClip clip;
            do
            {
                clip = clips[Random.Range(0, clips.Length)];
            }
            while (clip == ultimoClip && clips.Length > 1);

            if (clip != null)
            {
                source.clip = clip;
                source.Play();
                ultimoClip = clip;

                yield return new WaitForSeconds(clip.length);
            }
        }
    }
}
