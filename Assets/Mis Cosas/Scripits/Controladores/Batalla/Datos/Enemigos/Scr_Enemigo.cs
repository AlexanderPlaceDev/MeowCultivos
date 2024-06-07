using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Scr_Enemigo : MonoBehaviour
{
    public float Vida;
    public float Velocidad;
    public float Rango;
    public float CoolDown;
    public float Da�oMelee;
    public float Da�oDistancia;
    public GameObject PrefabBala; // Prefab del proyectil para ataques a distancia
    public Transform SpawnBala; // Punto de inicio del proyectil
    public Transform Objetivo;

    private float TiempoCoolDown;

    public enum TipoEnemigo { Terrestre, Volador }
    public TipoEnemigo tipoenemigo;

    public enum TipoComportamiento { Agresivo, Miedoso, Pacifico }
    public TipoComportamiento tipocomportamiento;

    private void Start()
    {
        TiempoCoolDown = CoolDown;
    }
    public virtual void Mover()
    {
        // Movimiento b�sico hacia el jugador
        if (GetComponent<NavMeshAgent>() != null && GetComponent<NavMeshAgent>().isActiveAndEnabled)
        {
            if (Objetivo != null)
            {
                if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Mover"))
                {
                    GetComponent<Animator>().Play("Mover");
                }
                GetComponent<NavMeshAgent>().isStopped = false;
                GetComponent<NavMeshAgent>().destination = Objetivo.position;

            }
            else
            {
                GetComponent<NavMeshAgent>().isStopped = true;
            }
        }
    }

    public virtual void AtaqueMelee()
    {
        if (TiempoCoolDown >= CoolDown)
        {
            TiempoCoolDown = 0;
            GetComponent<Animator>().Play("Ataque1");
            // Implementar l�gica de ataque cuerpo a cuerpo
            Debug.Log("Melee attack performed");
            // Ejemplo: aplicar da�o al jugador si est� en rango
        }
        else
        {
            TiempoCoolDown += Time.deltaTime;
        }


    }

    public virtual void AtaqueDistancia()
    {
        // Implementar l�gica de ataque a distancia
        Debug.Log("Ranged attack performed");
        if (PrefabBala != null && SpawnBala != null)
        {
            Instantiate(PrefabBala, SpawnBala.position, SpawnBala.rotation);
        }
    }

    public void RecibirDa�o(float Da�oRecibido)
    {
        Vida -= Da�oRecibido;
        if (Vida <= 0)
        {
            Morir();
        }
    }

    public virtual void Morir()
    {
        // Implementar la l�gica de muerte del enemigo
        Destroy(gameObject);
    }

    public void EstablecerComportamiento(TipoComportamiento NuevoComportamiento)
    {
        tipocomportamiento = NuevoComportamiento;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bala")
        {

        }

        if (other.gameObject.tag == "Golpe")
        {
            other.gameObject.SetActive(false);
            RecibirDa�o(1);
        }
    }
}