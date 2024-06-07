using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Scr_Enemigo : MonoBehaviour
{
    public float Vida;
    public float Velocidad;
    public float Rango;
    public float CoolDown;
    public float DañoMelee;
    public float DañoDistancia;
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
        // Movimiento básico hacia el jugador
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
            // Implementar lógica de ataque cuerpo a cuerpo
            Debug.Log("Melee attack performed");
            // Ejemplo: aplicar daño al jugador si está en rango
        }
        else
        {
            TiempoCoolDown += Time.deltaTime;
        }


    }

    public virtual void AtaqueDistancia()
    {
        // Implementar lógica de ataque a distancia
        Debug.Log("Ranged attack performed");
        if (PrefabBala != null && SpawnBala != null)
        {
            Instantiate(PrefabBala, SpawnBala.position, SpawnBala.rotation);
        }
    }

    public void RecibirDaño(float DañoRecibido)
    {
        Vida -= DañoRecibido;
        if (Vida <= 0)
        {
            Morir();
        }
    }

    public virtual void Morir()
    {
        // Implementar la lógica de muerte del enemigo
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
            RecibirDaño(1);
        }
    }
}