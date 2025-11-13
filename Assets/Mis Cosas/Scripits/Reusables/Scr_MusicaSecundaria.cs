using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Scr_MusicaSecundaria : MonoBehaviour
{
    [Header("Referencias de audio")]
    [Tooltip("Audio principal (normalmente en la cámara)")]
    [SerializeField] private AudioSource musicaPrincipal;

    private AudioSource audioSecundario;
    private Transform gata;

    [Header("Rango de influencia")]
    [Tooltip("Distancia a la que empieza la transición de mezcla (la música principal comienza a bajar)")]
    [SerializeField] private float distanciaInicioTransicion = 40f;

    [Tooltip("Distancia máxima a la que la música secundaria se escucha con fuerza máxima")]
    [SerializeField] private float distanciaMaximaSecundaria = 20f;

    [Header("Volúmenes relativos")]
    [Range(0f, 1f)][SerializeField] private float volumenRelativoSecundario = 1f;
    [Range(0f, 1f)][SerializeField] private float volumenMinimoPrincipal = 0.3f;

    [Header("Suavizado")]
    [Tooltip("Qué tan rápido se ajustan los volúmenes (transición más o menos fluida)")]
    [SerializeField] private float suavizado = 3f;

    // Volumen máximo permitido por las preferencias del jugador
    private float volumenMusicaBase;

    void Start()
    {
        audioSecundario = GetComponent<AudioSource>();
        if (!audioSecundario.isPlaying)
            audioSecundario.Play();

        // Buscar a la Gata
        GameObject objGata = GameObject.FindGameObjectWithTag("Gata");
        if (objGata != null)
            gata = objGata.transform;
        else
            Debug.LogWarning("⚠️ No se encontró ningún objeto con el tag 'Gata'.");

        // Buscar música principal si no se asignó manualmente
        if (musicaPrincipal == null && Camera.main != null)
            musicaPrincipal = Camera.main.GetComponent<AudioSource>();

        // Leer volumen del jugador (solo el de música)
        volumenMusicaBase = PlayerPrefs.GetInt("Volumen_Musica", 30) / 100f;
    }

    void Update()
    {
        if (gata == null || musicaPrincipal == null) return;

        // Calcular distancia y factor de mezcla
        float distancia = Vector3.Distance(gata.position, transform.position);
        float factor = CalcularFactorDeProximidad(distancia);

        // 🔊 Usar solo el volumen de música como límite máximo
        float volumenSecundarioObjetivo = volumenMusicaBase * volumenRelativoSecundario * factor;
        float volumenPrincipalObjetivo = volumenMusicaBase * Mathf.Lerp(1f, volumenMinimoPrincipal, factor);

        // Transiciones suaves
        audioSecundario.volume = Mathf.Lerp(audioSecundario.volume, volumenSecundarioObjetivo, Time.deltaTime * suavizado);
        musicaPrincipal.volume = Mathf.Lerp(musicaPrincipal.volume, volumenPrincipalObjetivo, Time.deltaTime * suavizado);
    }

    /// <summary>
    /// Calcula un factor 0–1 dependiendo de la distancia.
    /// </summary>
    private float CalcularFactorDeProximidad(float distancia)
    {
        if (distancia > distanciaInicioTransicion)
            return 0f;
        if (distancia <= distanciaMaximaSecundaria)
            return 1f;

        return 1f - ((distancia - distanciaMaximaSecundaria) / (distanciaInicioTransicion - distanciaMaximaSecundaria));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.7f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, distanciaInicioTransicion);

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, distanciaMaximaSecundaria);
    }
}
