using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoLobo : Scr_Enemigo
{
    [SerializeField] float RadioDeambulacion;
    [SerializeField] float TiempoDeEsperaMin;
    [SerializeField] float TiempoDeEsperaMax;
    [SerializeField] Animator Anim;

    private GameObject Gata;
    private NavMeshAgent agente;
    private float temporizadorEspera;
    private bool enDeambulacion = false;
    private bool Atacando = false;
    private float ContAtaque = 0;
    private Vector3 ultimaPosicion;
    private float temporizadorRecalculacion = 0f;
    public float tiempoRecalculacion = 0.5f;

    protected override void Start()
    {
        base.Start();
        Gata = GameObject.Find("Personaje");
        agente = GetComponent<NavMeshAgent>();
        agente.speed = Velocidad;

        if (agente.isOnNavMesh)
        {
            Objetivo = Gata.transform;
            agente.SetDestination(Objetivo.position);
        }

        // Animación de aparición
        Anim.Play(NombreAnimacionAparecer);
        float duracion = Anim.GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(EsperarAparicion(duracion));
    }

    IEnumerator EsperarAparicion(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        Aparecio = true;
        if (agente.isOnNavMesh)
            agente.isStopped = false;
    }

    void Update()
    {
        if (!Aparecio || EstaMuerto) return;
        if (agente == null) return;

        // Si está stuneado, congelado o empujado: NO MOVERSE
        if (estaStuneado || estaCongelado || estaEmpujado)
        {
            if (agente.isOnNavMesh)
                agente.isStopped = true;

            Anim.Play("Idle");
            return;
        }

        if (Objetivo == null)
        {
            if (agente.isOnNavMesh)
                Objetivo = Gata.transform;
            return;
        }

        float distancia = Vector3.Distance(transform.position, Objetivo.position);

        // 🔹 Si está atacando, maneja el ciclo
        if (Atacando)
        {
            if (ContAtaque < DuracionDeAtaque)
            {
                ContAtaque += Time.deltaTime;
            }
            else
            {
                ContAtaque = 0;
                Atacando = false;
                // 🔹 Después de atacar entra en deambulación
                IniciarDeambulacion();
            }
            return;
        }

        // 🔹 Si está en modo deambulación
        if (enDeambulacion)
        {
            temporizadorEspera -= Time.deltaTime;

            // Si ya llegó al destino y aún queda tiempo de espera → buscar nuevo punto
            if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
            {
                if (temporizadorEspera > 0)
                {
                    MoverANuevaPosicion();
                }
            }

            if (temporizadorEspera <= 0)
            {
                enDeambulacion = false; // termina deambulación → volverá a perseguir
            }
            return;
        }


        // 🔹 Persecución normal
        if (distancia <= agente.stoppingDistance + 1f)
        {
            Atacar();
        }
        else
        {
            Mover();
        }
    }

    void Mover()
    {
        if (agente != null && agente.isActiveAndEnabled && agente.isOnNavMesh)
        {
            if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("Mover"))
                Anim.Play("Mover");
            agente.isStopped = false;
            Objetivo = Gata.transform;

            temporizadorRecalculacion += Time.deltaTime;
            if (temporizadorRecalculacion >= tiempoRecalculacion)
            {
                agente.SetDestination(Objetivo.position);
                temporizadorRecalculacion = 0f;
            }
        }
    }

    void IniciarDeambulacion()
    {
        enDeambulacion = true;
        temporizadorEspera = Random.Range(TiempoDeEsperaMin, TiempoDeEsperaMax);
        ultimaPosicion = transform.position;
        MoverANuevaPosicion();
    }


    void MoverANuevaPosicion()
    {
        int intentos = 0;
        while (intentos < 10)
        {
            Vector3 randomDirection = Random.insideUnitSphere * RadioDeambulacion;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, RadioDeambulacion, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (agente.CalculatePath(hit.position, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        agente.isStopped = false;
                        agente.SetPath(path);
                        if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("Mover"))
                            Anim.Play("Mover");
                        return;
                    }
                }
            }
            intentos++;
        }
        Debug.LogWarning("⚠ No se encontró un punto válido en el NavMesh después de varios intentos.");
    }

    void Atacar()
    {
        Debug.Log("▶ Comenzó ataque");
        Atacando = true;
        agente.isStopped = true; // se detiene durante el ataque
        Anim.Play("Mordida");
        Tween.ShakeCamera(Camera.main, 3);

        Scr_ControladorBatalla batalla = Controlador.GetComponent<Scr_ControladorBatalla>();
        base.source.PlayOneShot(base.Golpe);
        batalla.RecibirDaño(DañoMelee);
        batalla.RecibirEfecto(base.Efecto.ToString());
    }
}
