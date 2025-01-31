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
    }
}
