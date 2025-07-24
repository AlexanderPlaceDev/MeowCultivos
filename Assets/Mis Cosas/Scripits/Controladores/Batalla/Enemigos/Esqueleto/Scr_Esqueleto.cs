using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_Esqueleto : Scr_Enemigo
{
    [SerializeField] Animator Anim;

    private GameObject Gata;
    private NavMeshAgent agente;
    private float temporizadorEspera;
    private bool esperando = false;
    private bool Atacando = false;
    private float ContAtaque = 0;
    private bool retrocede = false;
    private float Contretrocede = 0;
    public float duracionRetroceder = 1;

    // 🔹 Nueva lógica para esperar después de retroceder
    public float tiempoEsperaPostRetroceso = 1.5f;
    private bool esperandoPostRetroceso = false;
    private float contEsperaPostRetroceso = 0;

    protected override void Start()
    {
        base.Start();

        Gata = GameObject.Find("Personaje");
        agente = GetComponent<NavMeshAgent>();
        agente.speed = Velocidad;

        if (agente.isOnNavMesh)
        {
            agente.isStopped = true; // 🔹 Detener movimiento inicial
        }

        // Reproducir animación de aparición y esperar a que termine
        Anim.Play(NombreAnimacionAparecer);
        float duracion = Anim.GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(EsperarAparicion(duracion));
    }

    IEnumerator EsperarAparicion(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        Aparecio = true;
        if (agente.isOnNavMesh)
        {
            agente.isStopped = false;
        }
    }

    void Update()
    {
        if (!Aparecio) return;
        if (EstaMuerto) return;

        if (Objetivo != null)
        {
            float distancia = Vector3.Distance(transform.position, Objetivo.position);

            if (esperando)
            {
                Vector3 direccion = Gata.transform.position - transform.position;
                direccion.y = 0;
                if (direccion != Vector3.zero)
                {
                    Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime * 5f);
                }
                temporizadorEspera -= Time.deltaTime;
            }

            if (temporizadorEspera <= 0)
            {
                esperando = false;
            }

            // 🔹 Ataque
            if (distancia <= agente.stoppingDistance + 1f)
            {
                if (!esperando && !Atacando && !retrocede && !esperandoPostRetroceso)
                {
                    agente.isStopped = true;
                    esperando = true;
                    Atacar();
                }
            }
            else
            {
                // 🔹 Movimiento o retroceso
                if (!esperando && !retrocede && !esperandoPostRetroceso)
                {
                    Mover();
                }
                else if (retrocede && Contretrocede < duracionRetroceder)
                {
                    Contretrocede += Time.deltaTime;
                    PaAtras();
                }
                else if (retrocede && !esperandoPostRetroceso)
                {
                    // 🔹 Fin de retroceso → empieza espera
                    Contretrocede = 0;
                    retrocede = false;
                    esperandoPostRetroceso = true;
                    contEsperaPostRetroceso = 0;
                    agente.isStopped = true;
                }
                else if (esperandoPostRetroceso)
                {
                    contEsperaPostRetroceso += Time.deltaTime;
                    if (contEsperaPostRetroceso >= tiempoEsperaPostRetroceso)
                    {
                        esperandoPostRetroceso = false;
                        contEsperaPostRetroceso = 0;
                        agente.isStopped = false;
                    }
                }
            }

            // 🔹 Ciclo de ataque
            if (Atacando && ContAtaque < DuracionDeAtaque)
            {
                ContAtaque += Time.deltaTime;
            }
            else
            {
                ContAtaque = 0;
                Atacando = false;
            }
        }
        else
        {
            if (agente.isOnNavMesh)
            {
                Objetivo = Gata.transform;
                agente.SetDestination(Objetivo.position);
            }
        }
    }

    void Mover()
    {
        if (agente != null && agente.isActiveAndEnabled && agente.isOnNavMesh)
        {
            if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("Mover"))
            {
                Anim.Play("Mover");
            }
            agente.isStopped = false;
            Objetivo = Gata.transform;
            agente.destination = Objetivo.position;
        }
    }

    void PaAtras()
    {
        Vector3 direccion = Gata.transform.position - transform.position;
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime * 5f);
        }

        if (agente != null && agente.isActiveAndEnabled && agente.isOnNavMesh)
        {
            if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("Mover"))
            {
                Anim.Play("Mover");
            }
            agente.isStopped = false;
            agente.destination = transform.position - (transform.forward * 20);
        }
    }

    void Atacar()
    {
        Debug.Log("Comenzo Atacar");
        Atacando = true;
        agente.isStopped = true;

        if (Random.Range(0, 2) == 1)
        {
            Anim.Play("Ataque1");
            DuracionDeAtaque = 2.917f;
        }
        else
        {
            Anim.Play("Ataque2");
            DuracionDeAtaque = 1.042f;
        }

        Tween.ShakeCamera(Camera.main, 3);

        Scr_ControladorBatalla batalla = Controlador.GetComponent<Scr_ControladorBatalla>();
        batalla.VidaActual = Mathf.Max(0, batalla.VidaActual - DañoMelee);

        // 🔹 Comienza retroceso después del ataque
        retrocede = true;
    }
}
