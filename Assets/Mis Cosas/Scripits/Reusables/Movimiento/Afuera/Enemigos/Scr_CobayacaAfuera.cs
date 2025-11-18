using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_CobayacaAfuera : Scr_EnemigoFuera
{
    [Header("Parámetros de comportamiento")]
    [SerializeField] private float velocidadCorrer = 6f;
    [SerializeField] private float duracionIdle1 = 13.083f;
    [SerializeField] private float duracionIdle2 = 6.583f;

    [Header("Animación")]
    [SerializeField] private Animator anim;

    private NavMeshAgent agente;
    private GameObject jaba;
    private bool corriendo = false;
    private bool ejecutando = false; // para evitar solapamientos de corutinas
    private Vector3 destino;

    void Start()
    {
        if (GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            agente = GetComponent<NavMeshAgent>();
            agente.speed = Velocidad;
            agente.isStopped = true;
        }
        if (anim == null) anim = GetComponent<Animator>();
        LanzarNuevoEstado();
    }

    void Update()
    {
        // =========================
        // Estado de huida
        // =========================
        if (corriendo)
        {
            if (Vector3.Distance(transform.position, destino) < agente.stoppingDistance)
            {
                // Genera nuevo punto de huida cada poco tiempo
                destino = ObtenerDestinoAleatorio();
                agente.SetDestination(destino);
            }
        }
    }

    // ================================
    // RUTINA PRINCIPAL DE COMPORTAMIENTO
    // ================================
    void LanzarNuevoEstado()
    {
        if (corriendo || ejecutando) return;
        ejecutando = true;

        int rnd = Random.Range(0, 100);
        if (rnd < 60)
            StartCoroutine(EjecutarIdle("Iddle1", duracionIdle1));
        else if (rnd < 80)
            StartCoroutine(EjecutarIdle("Iddle2", duracionIdle2));
        else
            StartCoroutine(EjecutarCaminar());
    }

    IEnumerator EjecutarIdle(string animacion, float duracion)
    {
        if (GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            agente.isStopped = true;
            agente.ResetPath();
            agente.velocity = Vector3.zero;
        }

        CambiarAnimacion(animacion);
        yield return new WaitForSeconds(duracion);

        ejecutando = false;
        LanzarNuevoEstado();
    }

    IEnumerator EjecutarCaminar()
    {
        CambiarAnimacion("Caminar");

        if (GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            agente.isStopped = false;
            agente.speed = Velocidad;
            destino = ObtenerDestinoAleatorio();
            agente.SetDestination(destino);
            
            float tiempo = 0f;

        // Espera a que el agente tenga un camino válido antes de verificar distancia
        while (agente.pathPending)
            yield return null;

        while (!corriendo && tiempo < 7f)
        {
            // Espera a que realmente se mueva antes de cortar
            if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
                break;

            tiempo += Time.deltaTime;
            yield return null;
        }

        agente.isStopped = true;
        agente.ResetPath();

        ejecutando = false;
        LanzarNuevoEstado();
        }

        
    }

    // ================================
    // HUÍDA
    // ================================
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Contains("Jaba") && !corriendo)
        {
            Debug.Log("Mantiene");
            jaba = other.gameObject;
            corriendo = true;
            StopAllCoroutines();
            ejecutando = false;
            CambiarAnimacion("Corriendo");

            agente.isStopped = false;
            agente.speed = velocidadCorrer;
            destino = ObtenerDestinoAleatorio();
            agente.SetDestination(destino);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == jaba)
        {
            jaba = null;
            corriendo = false;
            agente.isStopped = true;
            agente.ResetPath();
            agente.velocity = Vector3.zero;

            CambiarAnimacion("Iddle1");
            ejecutando = false;
            LanzarNuevoEstado();
        }
    }

    // ================================
    // UTILIDADES
    // ================================
    Vector3 ObtenerDestinoAleatorio()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * RadioDeDeambulacion;
            randomDirection.y = 0;
            randomDirection += transform.position;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, RadioDeDeambulacion, NavMesh.AllAreas))
                if (Vector3.Distance(transform.position, hit.position) > agente.stoppingDistance * 2)
                    return hit.position;
        }
        return transform.position;
    }

    void CambiarAnimacion(string nueva)
    {
        anim.SetBool("Iddle1", false);
        anim.SetBool("Iddle2", false);
        anim.SetBool("Caminar", false);
        anim.SetBool("Corriendo", false);
        anim.SetBool(nueva, true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = corriendo ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, RadioDeDeambulacion);
    }
}
