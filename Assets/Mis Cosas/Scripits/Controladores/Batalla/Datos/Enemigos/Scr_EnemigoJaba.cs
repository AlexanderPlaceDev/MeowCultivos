using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoJaba : Scr_Enemigo
{
    public float CoolDownAtaque;
    private float TiempoCoolDownAtaque;

    [SerializeField] Scr_VisionEnemigosPelea Vision;
    [SerializeField] float RadioDeambulacion;
    [SerializeField] float TiempoDeEsperaMin;
    [SerializeField] float TiempoDeEsperaMax;
    [SerializeField] Animator Anim;

    [Header("Configuración de Raycast")]
    [SerializeField] float OffsetOrigen = 1f; // Altura del enemigo
    [SerializeField] float OffsetDestino = 1f; // Altura de la gata

    private Transform Objetivo;
    private NavMeshAgent agente;
    private float temporizadorEspera;
    private bool esperando = false; // Indica si el enemigo está esperando

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        // Verifica si el agente está sobre el NavMesh antes de asignarle un destino
        if (agente.isOnNavMesh)
        {
            MoverANuevaPosicion();
        }
        temporizadorEspera = 0;
    }

    void Update()
    {
        if (Vision.Gata != null)
        {
            if (PuedeVerGata())
            {
                Objetivo = Vision.Gata.transform;

                // Verifica que el agente está en el NavMesh antes de llamar a remainingDistance
                if (agente.isOnNavMesh && !agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
                {
                    //atacar
                }
                else
                {
                    Mover(); // Persigue al jugador

                }
            }
            else
            {
                Objetivo = null;
                Deambular(); // Modo de patrullaje aleatorio
            }
        }
        else
        {
            Objetivo = null;
            Deambular(); // Modo de patrullaje aleatorio
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

    void Deambular()
    {
        // Si el enemigo está esperando, cuenta el tiempo
        if (esperando)
        {
            temporizadorEspera -= Time.deltaTime;
            if (temporizadorEspera <= 0)
            {
                esperando = false; // Deja de esperar y busca un nuevo destino
                MoverANuevaPosicion();
            }
            return; // No hace nada hasta que termine la espera
        }

        // Verifica que el agente está en el NavMesh antes de llamar a remainingDistance
        if (agente.isOnNavMesh && !agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
        {
            Anim.SetBool("Caminando", false);
            esperando = true;
            temporizadorEspera = Random.Range(TiempoDeEsperaMin, TiempoDeEsperaMax);
        }
    }

    void MoverANuevaPosicion()
    {
        Vector3 randomDirection = Random.insideUnitSphere * RadioDeambulacion;
        randomDirection += transform.position;
        NavMeshHit hit;

        // Busca un punto en el NavMesh cercano a la posición aleatoria
        if (NavMesh.SamplePosition(randomDirection, out hit, RadioDeambulacion, NavMesh.AllAreas))
        {
            // Si el punto está en el NavMesh, se mueve ahí
            if (agente.isOnNavMesh)
            {
                agente.SetDestination(hit.position);
                Anim.SetBool("Caminando", true);
            }
        }
        else
        {
            // Si no encontró un punto válido, intenta otra vez
            MoverANuevaPosicion();
        }
    }

    bool PuedeVerGata()
    {
        if (Vision.Gata == null) return false;

        Vector3 origenRaycast = transform.position + Vector3.up * OffsetOrigen;
        Vector3 destinoRaycast = Vision.Gata.transform.position + Vector3.up * OffsetDestino;
        Vector3 direccion = destinoRaycast - origenRaycast;

        RaycastHit hit;
        if (Physics.Raycast(origenRaycast, direccion, out hit, direccion.magnitude))
        {
            if (hit.collider.CompareTag("Gata"))
            {
                Debug.DrawLine(origenRaycast, destinoRaycast, Color.green); // Debug en la escena
                return true; // No hay obstáculos
            }
            else
            {
                Debug.DrawLine(origenRaycast, hit.point, Color.red); // Debug en la escena
                return false; // Algo está bloqueando la vista
            }
        }
        return false;
    }
}
