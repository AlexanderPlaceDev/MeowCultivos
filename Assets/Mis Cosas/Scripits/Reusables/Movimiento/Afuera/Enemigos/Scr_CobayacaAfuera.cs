using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_CobayacaAfuera : Scr_EnemigoFuera
{
    [Header("Parámetros de comportamiento")]
    [SerializeField] private float velocidadCorrer = 6f;
    [SerializeField] private float duracionIdle1 = 13.083f;
    [SerializeField] private float duracionIdle2 = 6.583f;

    [Header("Animación")]
    [SerializeField] private Animator anim;

    private NavMeshAgent agente;
    private GameObject jaba;

    private bool corriendo = false;
    private bool ejecutando = false;
    private bool iniciado = false;

    private Vector3 destino;

    // ===== SONIDOS =====
    private Scr_Sonidos sonidos;
    private AudioSource audioSource;
    private Coroutine rutinaSonidos;

    private float pitchOriginal;
    private float velocidadOriginal;

    private float tiempoSinMover = 0f;
    private const float tiempoMaxSinMover = 1.5f;

    void Start()
    {
        sonidos = GetComponent<Scr_Sonidos>();
        audioSource = GetComponent<AudioSource>();

        if (!anim) anim = GetComponent<Animator>();

        pitchOriginal = audioSource.pitch;
        velocidadOriginal = anim.speed;

        if (TryGetComponent(out NavMeshAgent nav) && nav.isOnNavMesh)
        {
            agente = nav;
            agente.speed = Velocidad;
            agente.isStopped = true;
            agente.autoBraking = true;
        }

        rutinaSonidos = StartCoroutine(RutinaSonidosCaminar());

        CambiarAnimacion("Iddle1");
        LanzarNuevoEstado();
        iniciado = true;
    }

    void Update()
    {
        if (corriendo && jaba == null)
        {
            SalirDeHuida();
            return;
        }

        if (agente != null && agente.hasPath && !agente.isStopped)
        {
            if (agente.velocity.magnitude < 0.05f)
                tiempoSinMover += Time.deltaTime;
            else
                tiempoSinMover = 0f;

            // 🔴 EVITA QUEDARSE CAMINANDO EN EL BORDE
            if (tiempoSinMover >= tiempoMaxSinMover)
            {
                agente.ResetPath();
                agente.isStopped = true;

                CambiarAnimacion(Random.value < 0.5f ? "Iddle1" : "Iddle2");

                ejecutando = false;
                tiempoSinMover = 0f;
                LanzarNuevoEstado();
            }
        }
    }

    // ================================
    // COMPORTAMIENTO BASE
    // ================================
    void LanzarNuevoEstado()
    {
        if (corriendo || ejecutando) return;

        ejecutando = true;

        int rnd = Random.Range(0, 100);
        if (rnd < 40)
            StartCoroutine(EjecutarIdle("Iddle1", duracionIdle1));
        else if (rnd < 60)
            StartCoroutine(EjecutarIdle("Iddle2", duracionIdle2));
        else
            StartCoroutine(EjecutarCaminar());
    }

    IEnumerator EjecutarIdle(string animacion, float duracion)
    {
        if (agente != null)
        {
            agente.isStopped = true;
            agente.ResetPath();
        }

        CambiarAnimacion(animacion);
        yield return new WaitForSeconds(duracion);

        ejecutando = false;
        LanzarNuevoEstado();
    }

    IEnumerator EjecutarCaminar()
    {
        CambiarAnimacion("Caminar");

        if (agente != null)
        {
            Vector3 punto = ObtenerDestinoSeguro();

            agente.isStopped = false;
            agente.speed = Velocidad;
            agente.SetDestination(punto);

            while (agente.pathPending)
                yield return null;

            while (!corriendo && agente.hasPath && agente.remainingDistance > agente.stoppingDistance + 0.1f)
                yield return null;

            agente.ResetPath();
            agente.isStopped = true;
        }

        // 🔴 FORZAR IDLE AL TERMINAR
        CambiarAnimacion(Random.value < 0.5f ? "Iddle1" : "Iddle2");

        ejecutando = false;
        LanzarNuevoEstado();
    }

    // ================================
    // HUÍDA
    // ================================
    private void OnTriggerStay(Collider other)
    {
        if (!iniciado || corriendo) return;
        if (!other.gameObject.name.Contains("Jaba")) return;

        jaba = other.gameObject;
        corriendo = true;

        if (rutinaSonidos != null)
            StopCoroutine(rutinaSonidos);

        anim.speed = velocidadOriginal * 1.3f;
        audioSource.pitch = pitchOriginal + 0.1f;
        rutinaSonidos = StartCoroutine(RutinaSonidosCorrer());

        CambiarAnimacion("Corriendo");

        if (agente != null)
        {
            agente.isStopped = false;
            agente.speed = velocidadCorrer;
            agente.SetDestination(ObtenerDestinoSeguro());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == jaba)
            SalirDeHuida();
    }

    private void SalirDeHuida()
    {
        corriendo = false;
        jaba = null;

        if (rutinaSonidos != null)
            StopCoroutine(rutinaSonidos);

        anim.speed = velocidadOriginal;
        audioSource.pitch = pitchOriginal;
        rutinaSonidos = StartCoroutine(RutinaSonidosCaminar());

        if (agente != null)
        {
            agente.ResetPath();
            agente.isStopped = true;
        }

        CambiarAnimacion("Iddle1");
        ejecutando = false;
        LanzarNuevoEstado();
    }

    // ================================
    // DESTINO SEGURO (CLAVE)
    // ================================
    Vector3 ObtenerDestinoSeguro()
    {
        if (agente == null)
            return transform.position;

        int areaMask = agente.areaMask;

        for (int i = 0; i < 40; i++)
        {
            Vector3 random = transform.position +
                             Random.insideUnitSphere * RadioDeDeambulacion;
            random.y = transform.position.y;

            // 1️⃣ Proyectar al NavMesh DEL AGENTE
            if (!NavMesh.SamplePosition(random, out NavMeshHit hit, 3f, areaMask))
                continue;

            // 2️⃣ Rechazar bordes del NavMesh
            if (NavMesh.FindClosestEdge(hit.position, out NavMeshHit edge, areaMask))
            {
                if (edge.distance < agente.radius * 1.5f)
                    continue;
            }

            // 3️⃣ Validar path COMPLETO desde la posición actual
            NavMeshPath path = new NavMeshPath();
            if (!agente.CalculatePath(hit.position, path))
                continue;

            if (path.status != NavMeshPathStatus.PathComplete)
                continue;

            // 4️⃣ Punto final suficientemente lejos
            if (Vector3.Distance(transform.position, hit.position) < 1f)
                continue;

            return hit.position;
        }

        // 🔁 Fallback seguro
        agente.ResetPath();
        return transform.position;
    }


    // ================================
    // SONIDOS
    // ================================
    IEnumerator RutinaSonidosCaminar()
    {
        while (!corriendo)
        {
            if (sonidos.caminar_sonido.Length > 0)
                audioSource.PlayOneShot(
                    sonidos.caminar_sonido[Random.Range(0, sonidos.caminar_sonido.Length)]
                );

            yield return new WaitForSeconds(Random.Range(5f, 20f));
        }
    }

    IEnumerator RutinaSonidosCorrer()
    {
        while (corriendo)
        {
            if (sonidos.correr_sonido.Length > 0)
                audioSource.PlayOneShot(
                    sonidos.correr_sonido[Random.Range(0, sonidos.correr_sonido.Length)]
                );

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    void CambiarAnimacion(string nueva)
    {
        anim.SetBool("Iddle1", false);
        anim.SetBool("Iddle2", false);
        anim.SetBool("Caminar", false);
        anim.SetBool("Corriendo", false);
        anim.SetBool(nueva, true);
    }
}
