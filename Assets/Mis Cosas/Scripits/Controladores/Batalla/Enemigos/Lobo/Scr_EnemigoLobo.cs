﻿using PrimeTween;
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
    private bool esperando = false;
    private bool Atacando = false;
    private float ContAtaque = 0;
    private bool YaAtaco = false;

    protected override void Start()
    {
     
        base.Start();
        Gata = GameObject.Find("Personaje");
        agente = GetComponent<NavMeshAgent>();
        agente.speed = Velocidad;
        temporizadorEspera = Random.Range(TiempoDeEsperaMin, TiempoDeEsperaMax);

        if (agente.isOnNavMesh)
        {
            Objetivo = Gata.transform;
            agente.SetDestination(Objetivo.position);
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
                        temporizadorEspera = Random.Range(TiempoDeEsperaMin, TiempoDeEsperaMax);
                    }
                }
                else
                {
                    if (!Atacando)
                    {
                        Debug.Log("Deambulando");
                        agente.isStopped = false;
                        Deambular();
                    }

                }
            }
            else
            {
                if (esperando)
                {
                    if (!Atacando)
                    {
                        Deambular();
                    }
                }
                else
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
                YaAtaco = false;
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

    void Deambular()
    {
        if (agente.isOnNavMesh && !agente.pathPending && agente.remainingDistance <= agente.stoppingDistance + 1f)
        {
            MoverANuevaPosicion();
        }
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
                if (agente.isOnNavMesh)
                {
                    agente.isStopped = false;
                    agente.SetDestination(hit.position);
                    return;
                }
            }
            intentos++;
        }
        Debug.LogWarning("No se encontró un punto válido en el NavMesh después de varios intentos.");
    }

    void Atacar()
    {
        Debug.Log("Comenzo Atacar");
        Atacando = true;
        agente.isStopped = true; // 🔹 Evita que se mueva mientras ataca
        Anim.Play("Mordida");
        Tween.ShakeCamera(Camera.main, 3);
        Scr_ControladorBatalla batalla = Controlador.GetComponent<Scr_ControladorBatalla>();

        if (batalla.VidaActual >= DañoMelee)
        {
            batalla.VidaActual -= DañoMelee;
        }
        else
        {
            batalla.VidaActual = 0; // 🔹 Evita valores negativos
        }
    }
}