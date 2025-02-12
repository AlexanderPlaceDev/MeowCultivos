using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoGallina : Scr_Enemigo
{

    [SerializeField] float RadioDeambulacion;

    [Header("Configuración de Raycast")]

    private NavMeshAgent agente;
    private float temporizadorEspera;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        GetComponent<Animator>().SetBool("Caminando", true);

        if (agente.isOnNavMesh)
        {
            MoverANuevaPosicion();
        }
    }

    void Update()
    {
        if (Objetivo == null)
        {
            Deambular(); // Solo cambiar de posición cuando sea necesario
        }
    }

    void Deambular()
    {
        // Verifica que el agente esté en el NavMesh y que haya llegado a su destino antes de moverse
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
            Vector3 randomDirection = Random.onUnitSphere * RadioDeambulacion;
            randomDirection.y = 0; // Mantener el movimiento en el plano horizontal
            randomDirection += transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, RadioDeambulacion, NavMesh.AllAreas))
            {
                if (agente.isOnNavMesh && Vector3.Distance(transform.position, hit.position) > agente.stoppingDistance * 2)
                {
                    agente.isStopped = false;
                    agente.SetDestination(hit.position);
                    Debug.Log("Moviéndose a nueva posición: " + hit.position);
                    return;
                }
            }
            intentos++;
        }
        Debug.LogWarning("No se encontró un punto válido en el NavMesh después de varios intentos.");
    }



}
