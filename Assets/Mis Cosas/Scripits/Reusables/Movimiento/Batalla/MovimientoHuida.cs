using UnityEngine;
using UnityEngine.AI;

public class MovimientoHuida : IMovimiento
{
    private float distanciaHuir;
    private float TiempoAEsperar;
    private float Contador;
    Vector3 destinoHuida;
    public MovimientoHuida(float distancia, float TiempoEspera)
    {
        distanciaHuir = distancia;
        TiempoAEsperar = TiempoEspera;
    }

    public void Mover(NavMeshAgent agente, GameObject jugador)
    {
        if (jugador != null)
        {

            if (destinoHuida == null || destinoHuida == Vector3.zero)
            {
                if (Vector3.Distance(agente.transform.position, destinoHuida) <= agente.stoppingDistance || destinoHuida == Vector3.zero)
                {
                    destinoHuida = Scr_MovimientoEnemigosFuera.RandomNavSphere(agente.transform.position, 10f * distanciaHuir);
                    agente.SetDestination(destinoHuida);
                }
            }
            else
            {
                Contador += Time.deltaTime;
                if (Contador >= TiempoAEsperar)
                {
                    Contador = 0;
                    destinoHuida = Scr_MovimientoEnemigosFuera.RandomNavSphere(agente.transform.position, 10f * distanciaHuir);
                    agente.SetDestination(destinoHuida);
                }
            }


        }
    }
}
