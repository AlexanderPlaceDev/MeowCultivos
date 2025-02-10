using UnityEngine;
using UnityEngine.AI;

public class Scr_MovimientoEnemigosFuera : MonoBehaviour
{
    public GameObject Jugador;
    public float DistanciaHuida;
    public float TiempoHuida;
    private NavMeshAgent agente;
    private IMovimiento comportamientoMovimiento;

    public enum EstadoEnemigo { Patrullando, Persiguiendo, Huyendo }
    public EstadoEnemigo estadoActual;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        CambiarEstado(estadoActual);
    }

    void Update()
    {
        if (comportamientoMovimiento != null)
        {
            comportamientoMovimiento.Mover(agente, Jugador);
        }
    }

    public void CambiarEstado(EstadoEnemigo nuevoEstado)
    {
        estadoActual = nuevoEstado;
        switch (nuevoEstado)
        {
            case EstadoEnemigo.Patrullando:
                comportamientoMovimiento = new MovimientoPatrulla(DistanciaHuida,TiempoHuida);
                break;
            case EstadoEnemigo.Persiguiendo:
                comportamientoMovimiento = new MovimientoPersecucion(DistanciaHuida,TiempoHuida);
                break;
            case EstadoEnemigo.Huyendo:
                comportamientoMovimiento = new MovimientoHuida(DistanciaHuida,TiempoHuida);
                break;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origen, float distancia)
    {
        Vector3 direccionAleatoria = Random.insideUnitSphere * distancia;
        direccionAleatoria += origen;

        NavMeshHit hit;
        NavMesh.SamplePosition(direccionAleatoria, out hit, distancia, NavMesh.AllAreas);

        return hit.position;
    }
}
