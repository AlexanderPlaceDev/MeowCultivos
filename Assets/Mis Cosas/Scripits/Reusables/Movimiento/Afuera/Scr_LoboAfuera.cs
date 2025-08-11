using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_LoboAfuera : Scr_EnemigoFuera
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
        if (Vision != null && Vision.Gata != null && PuedeVerGata())
        {
            StopAllCoroutines();
            GetComponent<NavMeshAgent>().enabled = true;
            Anim.Play("Mover");
            Esperando = false;

            Destino = Vision.Gata.transform.position;
            Mover();
        }
        else
        {
            if (!Esperando)
            {
                if (Vector3.Distance(transform.position, Destino) <= agente.stoppingDistance || Destino == Vector3.zero)
                {
                    switch (Random.Range(0, 6))
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            MoverANuevaPosicion();
                            break;
                        case 4:
                            Anim.Play("Iddle");
                            Esperando = true;
                            StartCoroutine(Esperar(28.1f));
                            break;
                        case 5:
                            Anim.Play("Iddle 2");
                            Esperando = true;
                            StartCoroutine(Esperar(7f));
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
        }
    }

    bool PuedeVerGata()
    {
        Vector3 origen = transform.position + new Vector3(0, offsetZInicio, 0);
        Vector3 destino = Vision.Gata.transform.position + new Vector3(0, offsetZFinal, 0);
        RaycastHit hit;

        Debug.DrawLine(origen, destino, Color.yellow);

        if (Physics.Raycast(origen, (destino - origen).normalized, out hit, Vector3.Distance(origen, destino), ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider != null)  // Verifica si el Raycast golpeó algo
            {
                Debug.Log("Hit con: " + hit.collider.gameObject.name);

                if (hit.collider.CompareTag("Gata"))
                {
                    Debug.Log("La mira");
                    
                    return true;
                }
            }
        }

        return false;
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

        if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("Mover"))
        {
            Anim.Play("Mover", 0, 0f);
        }

        agente.isStopped = false;
        agente.SetDestination(Destino);
    }
}
