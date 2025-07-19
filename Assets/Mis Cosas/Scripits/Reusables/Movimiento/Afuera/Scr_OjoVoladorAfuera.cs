using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using PrimeTween;
public class Scr_OjoVoladorAfuera : Scr_EnemigoFuera
{
    //public Transform player;
    public GameObject gata;
    public float VelocidadD = 10f; //velocidad de movimiento
    public float VelocidadDeRotacion = 5f; //velocidad en la que rota
    public float DistanciadeAtaque = 50f; //distancia que puede ir a buscarte
    public float Altura = 15f;
    public Vector3 CentrodePatrol;
    public float RadiodePatrol = 5f;

    private enum State { Patrullando, Atacando, Rebuscar, Esperando }
    private State currentState = State.Patrullando;

    private Vector3 targetPosition;

    void Start()
    {

        gata = GameObject.Find("Gata");
        SetNewPatrolPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrullando:
                Patrol();
                break;

            case State.Atacando:
                Attack();
                break;

            case State.Rebuscar:
                Retreat();
                break;

            case State.Esperando:
                // Solo espera en Cooldown
                break;
        }
    }

    void Patrol()
    {
        MoveTowards(targetPosition);

        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            SetNewPatrolPoint();
        }

        if (Vector3.Distance(transform.position, gata.transform.position) < DistanciadeAtaque)
        {
            currentState = State.Atacando;
            targetPosition = gata.transform.position; // Picado al jugador
        }
    }

    void Attack()
    {
        MoveTowards(targetPosition);

        if (Vector3.Distance(transform.position, targetPosition) < 2f)
        {
            Debug.LogWarning("Ataque realizado!");
            currentState = State.Rebuscar;
            targetPosition = CentrodePatrol + new Vector3(Random.Range(-RadiodePatrol, RadiodePatrol), transform.position.y + Altura, Random.Range(-RadiodePatrol, RadiodePatrol));
            
        }
    }

    void Retreat()
    {
        MoveTowards(targetPosition);

        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        currentState = State.Esperando;
        float waitTime = Random.Range(2f, 5f);
        yield return new WaitForSeconds(waitTime);

        currentState = State.Patrullando;
        SetNewPatrolPoint();
    }

    void MoveTowards(Vector3 point)
    {
        Vector3 direction = (point - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, VelocidadDeRotacion * Time.deltaTime);
        transform.position += transform.forward * VelocidadD * Time.deltaTime;
    }

    void SetNewPatrolPoint()
    {
        float x = Random.Range(-RadiodePatrol, RadiodePatrol);
        float z = Random.Range(-RadiodePatrol, RadiodePatrol);
        targetPosition = CentrodePatrol + new Vector3(x, transform.position.y + Altura, z);
    }
}
