
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_Totem : MonoBehaviour
{
    [SerializeField] private float DistanciaRetroceso = 5f;
    [SerializeField] private float TiempoMaximoRetroceso = 8f;

    private void OnTriggerEnter(Collider other)
    {
        // Buscar el controlador en el objeto detectado o sus padres.
        Scr_JabaAfuera jaba = other.GetComponentInParent<Scr_JabaAfuera>();

        // Ignorar objetos que no sean Jabas.
        if (jaba == null)
            return;

        // Evitar activar dos veces la maniobra.
        if (!jaba.enabled)
            return;

        // Excluir gallinas.
        if (other.GetComponentInParent<Scr_GallinaAfuera>() != null || other.GetComponent<Scr_VisionEnemigosFuera>())
            return;

        NavMeshAgent agente = jaba.GetComponent<NavMeshAgent>();

        if (agente == null || !agente.isActiveAndEnabled ||
            !agente.isOnNavMesh)
        {
            Debug.LogWarning("El Jaba no tiene un NavMeshAgent activo en el NavMesh.");
            return;
        }

        StartCoroutine(HacerRetroceder(jaba, agente));
    }

    private IEnumerator HacerRetroceder(
        Scr_JabaAfuera jaba,
        NavMeshAgent agente)
    {
        // Guardar la dirección de movimiento antes de detener la persecución.
        Vector3 direccion = agente.velocity;

        if (direccion.sqrMagnitude < 0.01f)
            direccion = jaba.transform.forward;

        direccion.y = 0f;
        direccion.Normalize();

        // Detener temporalmente la lógica que persigue al jugador.
        jaba.enabled = false;

        agente.isStopped = false;

        Vector3 destinoDeseado =
            jaba.transform.position - direccion * DistanciaRetroceso;

        // Buscar un punto válido en el NavMesh.
        NavMeshHit hit;

        if (!NavMesh.SamplePosition(
            destinoDeseado,
            out hit,
            DistanciaRetroceso,
            NavMesh.AllAreas))
        {
            Debug.LogWarning("No se encontró un punto válido para el retroceso.");

            jaba.enabled = true;
            yield break;
        }

        agente.SetDestination(hit.position);

        float tiempo = 0f;

        // Esperar hasta llegar o alcanzar el límite de tiempo.
        while (tiempo < TiempoMaximoRetroceso)
        {
            if (jaba == null || agente == null ||
                !agente.isActiveAndEnabled || !agente.isOnNavMesh)
                yield break;

            tiempo += Time.deltaTime;

            if (!agente.pathPending &&
                agente.hasPath &&
                agente.remainingDistance <= agente.stoppingDistance + 0.1f)
            {
                break;
            }

            yield return null;
        }

        if (jaba == null || agente == null)
            yield break;

        // Detener el agente antes de reanudar la persecución.
        agente.ResetPath();

        // Reactivar la lógica original del Jaba.
        jaba.enabled = true;
    }
}
