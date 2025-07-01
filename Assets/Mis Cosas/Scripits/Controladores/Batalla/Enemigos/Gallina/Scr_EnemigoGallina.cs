using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Scr_EnemigoGallina : Scr_Enemigo
{
    [SerializeField] float RadioDeambulacion;
    [SerializeField] LayerMask capasPermitidas; // <<< NUEVO

    private NavMeshAgent agente;

    protected override void Start()
    {
        Animator Anim = GetComponent<Animator>();

        base.Start(); // en caso de que Scr_Enemigo tenga lógica en Start
        agente = GetComponent<NavMeshAgent>();
        if (!Aparecio)
        {
            float duracion = Anim.GetCurrentAnimatorStateInfo(0).length;
            StartCoroutine(EsperarAparicion(duracion));
        }

        if (agente.isOnNavMesh)
        {
            MoverANuevaPosicion();
        }
    }

    IEnumerator EsperarAparicion(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        Aparecio = true;
        GetComponent<Animator>().SetBool("Caminando", true);
        if (agente.isOnNavMesh)
        {
            agente.isStopped = false;
        }
    }

    void Update()
    {
        if (Objetivo == null && Aparecio && !EstaMuerto)
        {
            Deambular();
        }
    }

    void Deambular()
    {
        if (agente.isOnNavMesh && !agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
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
            randomDirection.y = 0; // Movimiento horizontal
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, RadioDeambulacion, NavMesh.AllAreas))
            {
                Vector3 puntoDestino = hit.position;

                // Validar que el punto esté sobre un layer permitido
                Ray ray = new Ray(puntoDestino + Vector3.up * 2f, Vector3.down);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, 5f, capasPermitidas)) // <<< NUEVA VERIFICACIÓN
                {
                    if (agente.isOnNavMesh && Vector3.Distance(transform.position, puntoDestino) > agente.stoppingDistance * 2)
                    {
                        agente.isStopped = false;
                        agente.SetDestination(puntoDestino);
                        Debug.Log("Moviéndose a nueva posición: " + puntoDestino);
                        return;
                    }
                }
            }
            intentos++;
        }

        Debug.LogWarning("No se encontró un punto válido en capas permitidas después de varios intentos.");
    }
}
