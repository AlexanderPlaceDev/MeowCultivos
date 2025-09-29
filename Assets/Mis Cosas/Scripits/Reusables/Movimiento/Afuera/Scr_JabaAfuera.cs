using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scr_JabaAfuera : Scr_EnemigoFuera
{
    [SerializeField] Scr_VisionEnemigosFuera Vision;
    [SerializeField] private float offsetZInicio = 0.5f;
    [SerializeField] private float offsetZFinal = 0.5f;
    [SerializeField] float TiempoLimitePersiguiendo = 0;
    float TiempoPersiguiendo = 0;
    private Vector3 Destino;
    private NavMeshAgent agente;
    private Animator Anim;
    private bool Esperando = false;

    // ---------------- NUEVO ----------------
    [Header("Sonidos")]
    [SerializeField] private List<AudioClip> SonidosPersecucion = new List<AudioClip>();
    [SerializeField] private List<AudioClip> SonidosIddle = new List<AudioClip>();
    [SerializeField] private List<AudioClip> SonidosEspecificos = new List<AudioClip>();

    [Header("Tiempos de sonidos Iddle")]
    [SerializeField] private float TiempoSonidoMinIddle = 4f;
    [SerializeField] private float TiempoSonidoMaxIddle = 7f;

    [Header("Tiempos de sonidos Persecución")]
    [SerializeField] private float TiempoSonidoMinPersecucion = 1f;
    [SerializeField] private float TiempoSonidoMaxPersecucion = 3f;

    private bool persiguiendo = false;
    private Coroutine sonidoCoroutine;
    private Coroutine esperarCoroutine;
    private AudioSource audioSource;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = Velocidad;
        Anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Iniciar la corutina de sonidos una sola vez
        sonidoCoroutine = StartCoroutine(ControlarSonidos());
    }

    void Update()
    {
        if (Vision != null && Vision.Gata != null)
        {
            // Está persiguiendo
            GetComponent<NavMeshAgent>().enabled = true;
            Anim.SetBool("Caminando", true);
            Esperando = false;

            Destino = Vision.Gata.transform.position;
            Mover();

            // Cambió de estado a persecución
            if (!persiguiendo)
            {
                persiguiendo = true;
            }
        }
        else
        {
            // No está persiguiendo
            if (!Esperando)
            {
                Anim.SetBool("Caminando", true);
                if (Vector3.Distance(transform.position, Destino) <= agente.stoppingDistance || Destino == Vector3.zero)
                {
                    switch (Random.Range(0, 2))
                    {
                        case 0:
                            MoverANuevaPosicion();
                            break;
                        case 1:
                            Esperando = true;
                            if (esperarCoroutine != null) StopCoroutine(esperarCoroutine);
                            esperarCoroutine = StartCoroutine(Esperar(7.91f));
                            break;
                    }
                }
                else
                {
                    TiempoPersiguiendo += Time.deltaTime;
                    if (TiempoPersiguiendo >= TiempoLimitePersiguiendo)
                    {
                        TiempoPersiguiendo = 0;
                        MoverANuevaPosicion();
                    }
                }
            }
            else
            {
                Anim.SetBool("Caminando", false);
            }

            // Cambió de estado a idle
            if (persiguiendo)
            {
                persiguiendo = false;
            }
        }
    }

    IEnumerator Esperar(float Tiempo)
    {
        GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSeconds(Tiempo);
        Esperando = false;
        GetComponent<NavMeshAgent>().enabled = true;
        MoverANuevaPosicion();
    }

    void MoverANuevaPosicion()
    {
        if (!agente.isOnNavMesh) return;

        int intentos = 0;
        Vector3 nuevaPosicion = transform.position;

        while (intentos < 10)
        {
            Vector3 randomDirection = Random.insideUnitSphere * RadioDeDeambulacion;
            randomDirection.y = 0;
            randomDirection += transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, RadioDeDeambulacion, NavMesh.AllAreas))
            {
                if (Vector3.Distance(transform.position, hit.position) > agente.stoppingDistance * 2)
                {
                    nuevaPosicion = hit.position;
                    break;
                }
            }
            intentos++;
        }

        Destino = nuevaPosicion;
        agente.isStopped = false;
        agente.SetDestination(Destino);
    }

    void Mover()
    {
        if (agente == null || !agente.isActiveAndEnabled || !agente.isOnNavMesh || Destino == Vector3.zero) return;
        agente.isStopped = false;
        agente.SetDestination(Destino);
    }

    // ---------------- NUEVO ----------------
    IEnumerator ControlarSonidos()
    {
        bool ultimoEstado = persiguiendo; // para detectar cambios de estado

        while (true)
        {
            // Detectar si el estado cambió
            if (ultimoEstado != persiguiendo)
            {
                // Cortar sonido actual al cambiar de estado
                if (audioSource.isPlaying)
                    audioSource.Stop();

                ultimoEstado = persiguiendo;

                // Reproducir inmediatamente un clip del nuevo estado
                List<AudioClip> listaCambio = persiguiendo ? SonidosPersecucion : SonidosIddle;
                if (listaCambio != null && listaCambio.Count > 0)
                {
                    AudioClip clipCambio = listaCambio[Random.Range(0, listaCambio.Count)];
                    audioSource.clip = clipCambio;
                    audioSource.Play();
                }
            }
            else
            {
                // Seleccionar lista y tiempos según estado actual
                List<AudioClip> listaActual = persiguiendo ? SonidosPersecucion : SonidosIddle;
                float tiempoMin = persiguiendo ? TiempoSonidoMinPersecucion : TiempoSonidoMinIddle;
                float tiempoMax = persiguiendo ? TiempoSonidoMaxPersecucion : TiempoSonidoMaxIddle;

                // Reproducir clip si hay
                if (listaActual != null && listaActual.Count > 0)
                {
                    AudioClip clip = listaActual[Random.Range(0, listaActual.Count)];
                    audioSource.clip = clip;
                    audioSource.Play();
                }

                // Espera aleatoria, pero interrumpible si cambia de estado
                float tiempoEspera = Random.Range(tiempoMin, tiempoMax);
                float t = 0f;
                while (t < tiempoEspera)
                {
                    if (ultimoEstado != persiguiendo) // cambio de estado → romper espera
                        break;

                    t += Time.deltaTime;
                    yield return null;
                }
            }

            yield return null;
        }
    }

    public void ReproducirSonidoEspecifico()
    {
        if (SonidosEspecificos != null && SonidosEspecificos.Count > 0)
        {
            // Elegir un clip aleatorio de la lista
            AudioClip clip = SonidosEspecificos[Random.Range(0, SonidosEspecificos.Count)];

            // Usar el AudioSource principal (o el del hijo si quieres mantenerlo)
            AudioSource source = transform.GetChild(0).GetComponent<AudioSource>();

            if (!source.isPlaying)
            {
                source.clip = clip;
                source.Play();
            }
        }
    }

}
