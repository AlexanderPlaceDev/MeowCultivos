using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoEsqueleto : Scr_Enemigo
{
    [Header("Referencias")]
    [SerializeField] Animator anim;
    private GameObject jugador;
    private NavMeshAgent agente;

    [Header("Ajustes de comportamiento")]
    public float distanciaAtaque = 2f;
    public float duracionRetroceso = 2f;   // tiempo que camina hacia atrás
    public float duracionIdle = 2f;        // tiempo en idle antes de volver a perseguir
    public float tiempoRecalculo = 0.5f;   // cada cuánto recalcula el path

    private Transform objetivo;
    private float temporizadorEstado;
    private float temporizadorRecalculo;


    private enum Estado { Aparicion, Persecucion, Ataque, Retroceso, Idle }
    private Estado estadoActual;

    protected override void Start()
    {
        base.Start();
        jugador = GameObject.Find("Personaje");
        agente = GetComponent<NavMeshAgent>();
        agente.speed = Velocidad;

        if (jugador != null)
            objetivo = jugador.transform;

        // Animación de aparición
        estadoActual = Estado.Aparicion;
        anim.Play(NombreAnimacionAparecer);

        float duracion = anim.GetCurrentAnimatorStateInfo(0).length;
        Invoke(nameof(FinAparicion), duracion);
    }

    void FinAparicion()
    {
        Aparecio = true;
        CambiarEstado(Estado.Persecucion);
    }

    void Update()
    {
        if (!Aparecio || EstaMuerto || objetivo == null) return;

        switch (estadoActual)
        {
            case Estado.Persecucion:
                Perseguir();
                break;

            case Estado.Ataque:
                // se maneja en la corrutina del ataque
                break;

            case Estado.Retroceso:
                Retroceder();
                break;

            case Estado.Idle:
                EsperarIdle();
                break;
        }
    }

    void Perseguir()
    {
        float distancia = Vector3.Distance(transform.position, objetivo.position);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Caminar"))
            anim.Play("Caminar");

        temporizadorRecalculo += Time.deltaTime;
        if (temporizadorRecalculo >= tiempoRecalculo)
        {
            if (agente != null && agente.isOnNavMesh)
                agente.SetDestination(objetivo.position);
            temporizadorRecalculo = 0f;
        }

        if (distancia <= distanciaAtaque)
        {
            CambiarEstado(Estado.Ataque);
        }
    }

    void EjecutarAtaque()
    {
        anim.Play("Atacar");
        agente.isStopped = true;

        // aplicar daño
        Scr_ControladorBatalla batalla = Controlador.GetComponent<Scr_ControladorBatalla>();
        batalla.VidaActual = Mathf.Max(0, batalla.VidaActual - DañoMelee);

        // después del ataque → retroceso
        Invoke(nameof(IniciarRetroceso), anim.GetCurrentAnimatorStateInfo(0).length);
    }

    void IniciarRetroceso()
    {
        CambiarEstado(Estado.Retroceso);
    }

    void Retroceder()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Retroceder"))
            anim.Play("Retroceder");

        temporizadorEstado -= Time.deltaTime;
        if (temporizadorEstado <= 0)
        {
            CambiarEstado(Estado.Idle);
            return;
        }

        if (agente != null && agente.isOnNavMesh)
        {
            Vector3 direccionOpuesta = (transform.position - objetivo.position).normalized;
            Vector3 destino = transform.position + direccionOpuesta * 2f;
            agente.SetDestination(destino);
        }
    }


    void EsperarIdle()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            anim.Play("Idle");

        temporizadorEstado -= Time.deltaTime;
        if (temporizadorEstado <= 0)
        {
            CambiarEstado(Estado.Persecucion);
        }
    }

    void CambiarEstado(Estado nuevoEstado)
    {
        estadoActual = nuevoEstado;

        switch (estadoActual)
        {
            case Estado.Persecucion:
                if (agente != null && agente.isOnNavMesh)
                    agente.isStopped = false;
                break;

            case Estado.Ataque:
                EjecutarAtaque();
                break;

            case Estado.Retroceso:
                temporizadorEstado = duracionRetroceso;
                if (agente != null && agente.isOnNavMesh)
                    agente.isStopped = false; // puede seguir moviéndose hacia atrás
                break;

            case Estado.Idle:
                temporizadorEstado = duracionIdle;
                if (agente != null && agente.isOnNavMesh)
                    agente.isStopped = true;
                break;
        }
    }

}
