using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoJaba : Scr_Enemigo
{
    [SerializeField] Animator Anim;
    private GameObject Gata;
    private NavMeshAgent agente;
    private float temporizadorEspera;
    private bool esperando = false;
    private bool Atacando = false;
    private float ContAtaque = 0;

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
        if (estaCongelado || estaStuneado || !Aparecio || EstaMuerto) return;

        if (Objetivo != null)
        {
            float distancia = Vector3.Distance(transform.position, Objetivo.position);
            //Debug.Log("Distancia a la gata: " + distancia);
            // 🔹 Se reduce el temporizador de espera en todo momento
            if (esperando)
            {
                temporizadorEspera -= Time.deltaTime;
            }
            // 🔹 Si el temporizador ha terminado, persigue a la gata
            if (temporizadorEspera <= 0)
            {
                esperando = false;
            }
            // 🔹 Lógica de ataque
            if (distancia <= agente.stoppingDistance + 1f)
            {
                if (!esperando)
                {
                    if (!Atacando)
                    {
                        agente.isStopped = true;
                        esperando = true;
                        Atacar();
                    }
                }
            }
            else
            {
                if (!esperando)
                {
                    Mover();

                }
            }


            //CicloDeAtaque
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

    void Atacar()
    {
        Debug.Log("Comenzo Atacar");
        Atacando = true;
        agente.isStopped = true; // 🔹 Evita que se mueva mientras ataca
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

        batalla.RecibirDaño(DañoMelee);
        batalla.RecibirEfecto(base.Efecto.ToString());
        /*if (batalla.VidaActual >= DañoMelee)
        {
            batalla.VidaActual -= DañoMelee;
        }
        else
        {
            batalla.VidaActual = 0; // 🔹 Evita valores negativos
        }*/
    }
}
