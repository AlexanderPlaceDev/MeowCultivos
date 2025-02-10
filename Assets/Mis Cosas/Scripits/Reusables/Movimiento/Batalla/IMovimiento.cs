using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IMovimiento
{
    void Mover(NavMeshAgent agente, GameObject jugador);
}
