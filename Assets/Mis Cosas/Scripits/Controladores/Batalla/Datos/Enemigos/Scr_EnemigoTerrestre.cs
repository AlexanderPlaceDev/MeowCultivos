using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoTerrestre : Scr_Enemigo
{
    private void Start()
    {
        if (GetComponent<NavMeshAgent>())
        {
            GetComponent<NavMeshAgent>().speed = Velocidad;
        }
    }
    public override void Mover()
    {
        // Movimiento terrestre
        base.Mover();
    }

    public override void AtaqueMelee()
    {
        base.AtaqueMelee();
    }

    public override void AtaqueDistancia()
    {
        // Implementar lógica de ataque a distancia
        Debug.Log("Ground enemy ranged attack");
        base.AtaqueDistancia();
    }
}
