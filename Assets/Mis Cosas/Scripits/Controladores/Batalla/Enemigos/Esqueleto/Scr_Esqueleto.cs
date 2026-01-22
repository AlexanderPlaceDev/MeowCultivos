using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Unity.VisualScripting.Member;

public class Scr_Esqueleto : Scr_Enemigo
{
    [Header("Referencias")]
    [SerializeField] private Animator anim;
    [SerializeField] private NavMeshAgent agente;
    [SerializeField] private Transform jugador; // asignar desde inspector o se busca "Personaje" en Start
    [SerializeField] private GameObject areaAtaque; // 👈 Nueva referencia al área de ataque

    [Header("Parámetros")]
    [Tooltip("Distancia a la que considera atacar (configurable desde inspector).")]
    [SerializeField] private float rangoAtaque = 2f;
    [Tooltip("Margen para evitar que el enemigo oscile justo en el borde del rango.")]
    [SerializeField] private float duracionAtaque = 1.042f;
    [SerializeField] private float duracionRetroceder = 1.0f;
    [SerializeField] private float tiempoEsperaPostRetroceso = 1.5f;
    [SerializeField] private float velocidadRetrocesoMultiplicador = 1.0f;
    [SerializeField] private float rotacionSpeed = 5f;

    
    
    private enum Estado { Apareciendo, Persiguiendo, Atacando, Retrocediendo, Esperando }
    private Estado estado = Estado.Apareciendo;

    private float timer = 0f;
    private bool aparecio = false;

    protected override void Start()
    {
        base.Start(); 
        if (jugador == null) jugador = GameObject.Find("Personaje")?.transform;
        if (agente == null) agente = GetComponent<NavMeshAgent>();

        agente.speed = Velocidad;
        agente.updateRotation = false;
        agente.stoppingDistance = 0f;
        if (agente.isOnNavMesh) agente.isStopped = true;

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
        if (agente == null) return;

        // Si está stuneado, congelado o empujado: NO MOVERSE
        if (estaStuneado || estaCongelado || estaEmpujado)
        {
            if (agente.isOnNavMesh)
                agente.isStopped = true;
            return;
        }
        float distancia = Vector3.Distance(transform.position, jugador.position);

        switch (estado)
        {
            case Estado.Persiguiendo: UpdatePersiguiendo(distancia); break;
            case Estado.Atacando: UpdateAtacando(distancia); break;
            case Estado.Retrocediendo: UpdateRetrocediendo(distancia); break;
            case Estado.Esperando: UpdateEsperando(distancia); break;
        }
    }

    // -------------------------
    // Estados
    // -------------------------
    void UpdatePersiguiendo(float distancia)
    {
        if (distancia <= rangoAtaque)
        {
            agente.isStopped = true;
            CambiarEstado(Estado.Atacando);
            return;
        }

        if (!agente.isOnNavMesh) return;
        agente.isStopped = false;
        if (!agente.pathPending)
            agente.SetDestination(jugador.position);

        Vector3 dir = jugador.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion objetivo = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, Time.deltaTime * rotacionSpeed);
        }

        float vel = agente.velocity.magnitude;
        bool moving = vel > 0.12f && !agente.isStopped;
        anim.SetBool("Caminando", moving);
        anim.SetBool("Atacando", false);
        anim.SetBool("Retrocediendo", false);
    }

    void UpdateAtacando(float distancia)
    {
        anim.SetBool("Atacando", true);
        anim.SetBool("Caminando", false);
        anim.SetBool("Retrocediendo", false);

        timer += Time.deltaTime;

        // Solo cambia de estado al finalizar el ataque, sin aplicar daño aquí
        if (timer >= duracionAtaque)
        {
            CambiarEstado(Estado.Retrocediendo);
        }
    }


    void UpdateRetrocediendo(float distancia)
    {
        anim.SetBool("Retrocediendo", true);
        anim.SetBool("Atacando", false);
        anim.SetBool("Caminando", false);

        Vector3 dir = jugador.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion objetivo = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, Time.deltaTime * rotacionSpeed);
        }

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

    private void CambiarEstado(Estado nuevo)
    {
        estado = nuevo;
        timer = 0f;

        anim.SetBool("Caminando", false);
        anim.SetBool("Atacando", false);
        anim.SetBool("Retrocediendo", false);

        switch (estado)
        {
            case Estado.Persiguiendo:
                if (agente.isOnNavMesh) agente.isStopped = false;
                break;
            case Estado.Atacando:
                if (agente.isOnNavMesh) agente.isStopped = true;
                anim.SetBool("Atacando", true);
                break;
            case Estado.Retrocediendo:
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

    public void activar_areaAtaque()
    {
        areaAtaque.SetActive(true);
    }
    public void desaactivar_areaAtaque()
    {
        areaAtaque.SetActive(false);
    }
    public void HacerDaño()
    {
        
        Scr_AreaDeAtaqueEnemiga area = areaAtaque.GetComponent<Scr_AreaDeAtaqueEnemiga>();
        if (EstaMuerto || area == null) return;

        // Solo causa daño si el jugador está dentro del área en el momento exacto
        if (area.EstaDentro)
        {
            base.source.PlayOneShot(base.Golpe);
            Scr_ControladorBatalla batalla = Controlador.GetComponent<Scr_ControladorBatalla>();
            batalla.RecibirDaño(DañoMelee);
            batalla.RecibirEfecto(base.Efecto.ToString());
            //batalla.VidaActual = Mathf.Max(0, batalla.VidaActual - DañoMelee);

            // Si tienes sacudida de cámara o efectos, también puedes ponerlos aquí
            ShakeCamara();
        }
    }

}
