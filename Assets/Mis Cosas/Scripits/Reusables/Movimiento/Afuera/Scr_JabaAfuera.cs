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

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = Velocidad;
        Anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Vision != null && Vision.Gata != null)
        {
            StopAllCoroutines();
            GetComponent<NavMeshAgent>().enabled = true;
            Anim.SetBool("Caminando", true);
            Esperando = false;

            Destino = Vision.Gata.transform.position;
            Mover();
        }
        else
        {
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
                            StartCoroutine(Esperar(7.91f));
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
}
