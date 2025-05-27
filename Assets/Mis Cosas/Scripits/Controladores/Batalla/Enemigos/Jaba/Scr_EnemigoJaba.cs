using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoJaba : Scr_Enemigo
{

    GameObject Gata;
    [SerializeField] float TiempoDeEsperaEntreAtaque;
    [SerializeField] Animator Anim;

    private NavMeshAgent agente;
    private bool Atacando = false; // Indica si el enemigo está Atacando

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        // Verifica si el agente está sobre el NavMesh antes de asignarle un destino
        Gata = GameObject.Find("Personaje");
    }

    void Update()
    {

        Objetivo = Gata.transform;
        Debug.Log("Gata detectada, asignando objetivo: " + Objetivo.name);

        float distancia = Vector3.Distance(transform.position, Objetivo.position);
        Debug.Log("Distancia a la gata: " + distancia);

        if (distancia <= agente.stoppingDistance + 1f)
        {
            Debug.Log("Listo para atacar");
            agente.isStopped = true;

            if (!Atacando)
            {
                StartCoroutine(Atacar());
            }
        }
        else
        {
            Debug.Log("Entra en persecución");
            if (agente.isOnNavMesh)
            {
                agente.isStopped = false;
                Mover();

            }
        }


    }


    void Mover()
    {
        if (agente != null && agente.isActiveAndEnabled && agente.isOnNavMesh)
        {
            if (Objetivo != null)
            {
                if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("Mover"))
                {
                    Anim.Play("Mover");
                }
                agente.isStopped = false;
                agente.destination = Objetivo.position;
            }
            else
            {
                agente.isStopped = true;
            }
        }
    }



    IEnumerator Atacar()
    {
        Tween.ShakeCamera(Camera.main, 3);
        Atacando = true;
        Anim.Play("Ataque1");
        if (Controlador.GetComponent<Scr_ControladorBatalla>().VidaActual >= DañoMelee)
        {
            Controlador.GetComponent<Scr_ControladorBatalla>().VidaActual -= DañoMelee;
        }
        else
        {
            Controlador.GetComponent<Scr_ControladorBatalla>().VidaActual -= Controlador.GetComponent<Scr_ControladorBatalla>().VidaActual;
        }
        yield return new WaitForSeconds(TiempoDeEsperaEntreAtaque);
        Atacando = false;
    }
}
