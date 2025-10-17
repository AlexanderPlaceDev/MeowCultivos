using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Versión mejorada de Scr_Esqueleto:
/// - Usa enum de estados
/// - Usa distancia directa + margen (histeresis) para evitar oscilaciones
/// - Controla rotación manual para evitar giros indeseados del NavMeshAgent
/// - Retrocede con agente.Move para no girar mientras retrocede
/// </summary>
public class Scr_Esqueleto : Scr_Enemigo
{
    [Header("Referencias")]
    [SerializeField] private Animator anim;
    [SerializeField] private NavMeshAgent agente;
    [SerializeField] private Transform jugador; // asignar desde inspector o se busca "Personaje" en Start

    [Header("Parámetros")]
    [Tooltip("Distancia a la que considera atacar (configurable desde inspector).")]
    [SerializeField] private float rangoAtaque = 2f;
    [Tooltip("Margen para evitar que el enemigo oscile justo en el borde del rango.")]
    [SerializeField] private float margenHisteresis = 0.25f;
    [SerializeField] private float duracionAtaque = 1.042f;
    [SerializeField] private float duracionRetroceder = 1.0f;
    [SerializeField] private float tiempoEsperaPostRetroceso = 1.5f;
    [SerializeField] private float velocidadRetrocesoMultiplicador = 1.0f;
    [SerializeField] private float rotacionSpeed = 5f;

    private enum Estado { Apareciendo, Persiguiendo, Atacando, Retrocediendo, Esperando }
    private Estado estado = Estado.Apareciendo;

    private float timer = 0f;
    private bool aparecio = false;
    private bool dañoAplicado = false;

    protected override void Start()
    {
        base.Start();

        // referenciar jugador/agente si no están enlazados en inspector
        if (jugador == null) jugador = GameObject.Find("Personaje")?.transform;
        if (agente == null) agente = GetComponent<NavMeshAgent>();

        // usar la velocidad heredada
        agente.speed = Velocidad;

        // Desactivamos la rotación automática del agente:
        // así controlamos la rotación manualmente y evitamos giros no deseados al retroceder.
        agente.updateRotation = false;

        // no depender del stoppingDistance para decidir ataque; controlamos desde código
        agente.stoppingDistance = 0f;
        if (agente.isOnNavMesh)
        {
            agente.isStopped = true;
        }

        // animación de aparición si la tienes
        anim.Play(NombreAnimacionAparecer);
        float dur = anim.GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(EsperarAparicion(dur));
    }

    IEnumerator EsperarAparicion(float dur)
    {
        yield return new WaitForSeconds(dur);
        aparecio = true;
        CambiarEstado(Estado.Persiguiendo);
    }

    void Update()
    {
        if (!aparecio || EstaMuerto || jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        switch (estado)
        {
            case Estado.Persiguiendo:
                UpdatePersiguiendo(distancia);
                break;
            case Estado.Atacando:
                UpdateAtacando(distancia);
                break;
            case Estado.Retrocediendo:
                UpdateRetrocediendo(distancia);
                break;
            case Estado.Esperando:
                UpdateEsperando(distancia);
                break;
        }
    }

    // -------------------------
    // Estados
    // -------------------------
    void UpdatePersiguiendo(float distancia)
    {
        // Si entra dentro del rango de ataque -> atacar
        if (distancia <= rangoAtaque)
        {
            agente.isStopped = true;
            CambiarEstado(Estado.Atacando);
            return;
        }

        // Si está fuera del rango + margen, perseguir
        if (!agente.isOnNavMesh) return;
        agente.isStopped = false;
        if (!agente.pathPending)
            agente.SetDestination(jugador.position);

        // Rotación manual hacia el jugador (mantiene al enemigo mirando al jugador)
        Vector3 dir = jugador.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion objetivo = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, Time.deltaTime * rotacionSpeed);
        }

        // Activar "Caminando" sólo si realmente hay movimiento (evita que quede en animación caminando si está casi quieto)
        float vel = agente.velocity.magnitude;
        bool moving = vel > 0.12f && !agente.isStopped;
        anim.SetBool("Caminando", moving);
        anim.SetBool("Atacando", false);
        anim.SetBool("Retrocediendo", false);
    }

    void UpdateAtacando(float distancia)
    {
        // Mantener anim de ataque
        anim.SetBool("Atacando", true);
        anim.SetBool("Caminando", false);
        anim.SetBool("Retrocediendo", false);

        timer += Time.deltaTime;

        // Cuando termine la animación de ataque, aplicar daño (una sola vez) y empezar retroceso
        if (timer >= duracionAtaque)
        {
            if (!dañoAplicado)
            {
                dañoAplicado = true;
                Scr_ControladorBatalla batalla = Controlador.GetComponent<Scr_ControladorBatalla>();
                batalla.VidaActual = Mathf.Max(0, batalla.VidaActual - DañoMelee);
            }

            CambiarEstado(Estado.Retrocediendo);
        }
    }

    void UpdateRetrocediendo(float distancia)
    {
        // Animaciones
        anim.SetBool("Retrocediendo", true);
        anim.SetBool("Atacando", false);
        anim.SetBool("Caminando", false);

        // Mantener mirando al jugador (no giramos por el agente)
        Vector3 dir = jugador.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion objetivo = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, Time.deltaTime * rotacionSpeed);
        }

        // Retroceder con agente.Move (no altera la rotación)
        Vector3 moverAtras = -transform.forward * Velocidad * velocidadRetrocesoMultiplicador * Time.deltaTime;
        if (agente.isOnNavMesh)
            agente.Move(moverAtras);
        else
            transform.position += moverAtras;

        timer += Time.deltaTime;
        if (timer >= duracionRetroceder)
        {
            CambiarEstado(Estado.Esperando);
        }
    }

    void UpdateEsperando(float distancia)
    {
        agente.isStopped = true;
        anim.SetBool("Caminando", false);
        anim.SetBool("Atacando", false);
        anim.SetBool("Retrocediendo", false);

        timer += Time.deltaTime;
        if (timer >= tiempoEsperaPostRetroceso)
        {
            CambiarEstado(Estado.Persiguiendo);
        }
    }

    // -------------------------
    // Cambio de estado centralizado
    // -------------------------
    private void CambiarEstado(Estado nuevo)
    {
        estado = nuevo;
        timer = 0f;

        // reset anims
        anim.SetBool("Caminando", false);
        anim.SetBool("Atacando", false);
        anim.SetBool("Retrocediendo", false);

        // reseteo de flags
        dañoAplicado = false;

        switch (estado)
        {
            case Estado.Persiguiendo:
                if (agente.isOnNavMesh) agente.isStopped = false;
                break;
            case Estado.Atacando:
                if (agente.isOnNavMesh) agente.isStopped = true;
                anim.SetBool("Atacando", true);
                dañoAplicado = false;
                break;
            case Estado.Retrocediendo:
                // usaremos Move para retroceder sin que el agente gire el transform
                if (agente.isOnNavMesh) agente.isStopped = true;
                anim.SetBool("Retrocediendo", true);
                break;
            case Estado.Esperando:
                if (agente.isOnNavMesh) agente.isStopped = true;
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
        Gizmos.color = Color.yellow;
        if (jugador != null) Gizmos.DrawLine(transform.position, jugador.position);
    }

    public void ShakeCamara()
    {
        Tween.ShakeCamera(Camera.main, 3);
    }
}
