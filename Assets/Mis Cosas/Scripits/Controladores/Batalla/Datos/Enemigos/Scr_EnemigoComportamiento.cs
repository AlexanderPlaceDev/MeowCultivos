using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoComportamiento : MonoBehaviour
{
    private Scr_Enemigo BaseEnemigo;
    public float radioDeambulacion = 10f;
    float TimerMiedoso = 0;
    [SerializeField] float TiempoRecalcularDestino = 3f; // Tiempo antes de recalcular un nuevo destino
    [SerializeField] float distanciaMinimaDestino = 1f; // Distancia mínima para considerar que alcanzó el destino
    Transform Jugador;
    Animator Animador;
    NavMeshAgent agente;

    void Start()
    {
        Animador = GetComponent<Animator>();
        Jugador = Camera.main.transform;
        BaseEnemigo = GetComponent<Scr_Enemigo>();
        agente = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        switch (BaseEnemigo.tipocomportamiento)
        {
            case Scr_Enemigo.TipoComportamiento.Agresivo:
                ComportamientoAgresivo();
                break;
            case Scr_Enemigo.TipoComportamiento.Miedoso:
                ComportamientoMiedoso();
                break;
            case Scr_Enemigo.TipoComportamiento.Pacifico:
                ComportamientoPacifico();
                break;
        }
    }

    void ComportamientoAgresivo()
    {
        BaseEnemigo.Mover();

        if (Vector3.Distance(transform.position, Jugador.position) < BaseEnemigo.Rango)
        {
            BaseEnemigo.Objetivo = null;
            if (BaseEnemigo.tipoenemigo == Scr_Enemigo.TipoEnemigo.Terrestre)
                BaseEnemigo.AtaqueMelee();
            else
                BaseEnemigo.AtaqueDistancia();
        }
        else
        {
            BaseEnemigo.Objetivo = Jugador;
        }
    }

    void ComportamientoMiedoso()
    {
        if (agente != null && agente.isActiveAndEnabled && agente.isOnNavMesh)
        {
            // Si el enemigo está cerca del destino o lleva mucho tiempo sin recalcular, encuentra una nueva posición
            if (agente.remainingDistance <= distanciaMinimaDestino || TimerMiedoso >= TiempoRecalcularDestino)
            {
                TimerMiedoso = 0;
                Vector3 nuevaPosicion = RandomNavSphere(transform.position, radioDeambulacion, 10);
                agente.SetDestination(nuevaPosicion);
            }
            else
            {
                TimerMiedoso += Time.deltaTime;
            }

            Animador.SetBool("Caminando", true);
        }
        else
        {
            Debug.LogWarning("El NavMeshAgent no está activo o no se encuentra en el NavMesh.");
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origen, float distancia, int intentosMaximos)
    {
        NavMeshHit hit;
        for (int i = 0; i < intentosMaximos; i++)
        {
            Vector3 direccionAleatoria = Random.insideUnitSphere * distancia + origen;
            if (NavMesh.SamplePosition(direccionAleatoria, out hit, distancia, NavMesh.AllAreas))
            {
                return hit.position; // Retorna la posición válida
            }
        }
        return origen; // Si no encuentra un punto, regresa la posición actual
    }

    void ComportamientoPacifico()
    {
        transform.Translate(Vector3.forward * BaseEnemigo.Velocidad * Time.deltaTime);
    }
}
