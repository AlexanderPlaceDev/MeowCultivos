using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scr_GallinaAfuera : Scr_EnemigoFuera
{
    [SerializeField] Scr_VisionEnemigosFuera Vision;
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
            Debug.Log("Entra4");

            if (Vector3.Distance(transform.position, Destino) <= agente.stoppingDistance || Destino == Vector3.zero)
            {
                Debug.Log("Entra2");
                MoverANuevaPosicion();
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
            Debug.Log("Entra1");
            if (!Esperando)
            {
                Anim.SetBool("Caminando", true);
                if (Vector3.Distance(transform.position, Destino) <= agente.stoppingDistance || Destino == Vector3.zero)
                {
                    Debug.Log("Entra2");
                    switch (Random.Range(0, 5))
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            Esperando = true;
                            StartCoroutine(Esperar(20.83f));
                            break;
                        case 4:
                            MoverANuevaPosicion();
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
}
