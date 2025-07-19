using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo_volador : MonoBehaviour
{
    public Transform player;
    public GameObject gata;
    public float speed = 10f;
    public float rotationSpeed = 5f;
    public float attackDistance = 100f;
    public float retreatHeight = 15f;
    public Vector3 patrolCenter;
    public float patrolRadius = 10f;

    private enum State { Patrol, Attack, Retreat, Cooldown }
    private State currentState = State.Patrol;

    private Vector3 targetPosition;

    void Start()
    {

        gata = GameObject.Find("Personaje");
        SetNewPatrolPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Attack:
                Attack();
                break;

            case State.Retreat:
                Retreat();
                break;

            case State.Cooldown:
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

        if (Vector3.Distance(transform.position, gata.transform.position) < attackDistance)
        {
            currentState = State.Attack;
            targetPosition = gata.transform.position; // Picado al jugador
        }
    }

    void Attack()
    {
        MoveTowards(targetPosition);

        if (Vector3.Distance(transform.position, gata.transform.position) < 2f)
        {
            Debug.Log("Ataque realizado!");

            // Aquí puedes hacer daño al jugador

            currentState = State.Retreat;
            targetPosition = patrolCenter + new Vector3(Random.Range(-patrolRadius, patrolRadius), retreatHeight, Random.Range(-patrolRadius, patrolRadius));
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
        currentState = State.Cooldown;
        float waitTime = Random.Range(2f, 5f);
        yield return new WaitForSeconds(waitTime);

        currentState = State.Patrol;
        SetNewPatrolPoint();
    }

    void MoveTowards(Vector3 point)
    {
        Vector3 direction = (point - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void SetNewPatrolPoint()
    {
        float x = Random.Range(-patrolRadius, patrolRadius);
        float z = Random.Range(-patrolRadius, patrolRadius);
        targetPosition = patrolCenter + new Vector3(x, retreatHeight, z);
    }
}
