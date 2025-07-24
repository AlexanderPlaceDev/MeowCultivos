using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_Enemigo_OjoVolador : Scr_Enemigo
{
    [SerializeField] Animator Anim;
    public GameObject gata;
    public float speed = 10f;
    public float rotationSpeed = 5f;
    public float attackDistance = 100;
    public float retreatHeight = 15f;
    public Vector3 patrolCenter;
    public float patrolRadius = 10f;
    private bool yaAtaco = false;
    private enum State { Patrol, Attack, Retreat, Cooldown }
    private State currentState = State.Patrol;

    private Vector3 PosiciondelObjetivo;

    protected override void Start()
    {

        base.Start(); // en caso de que Scr_Enemigo tenga lógica en Start
        Vector3 pos = transform.position;
        pos.y = retreatHeight;
        transform.position = pos;
        gata = GameObject.Find("Personaje");
        SetNewPatrolPoint();

        Anim.Play(NombreAnimacionAparecer);
        float duracion = Anim.GetCurrentAnimatorStateInfo(0).length;    
        StartCoroutine(EsperarAparicion(4));
    }
    IEnumerator EsperarAparicion(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        Aparecio = true;
    }
    void Update()
    {
        if (Aparecio) 
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
    }
    void Patrol()
    {
        MoveTowards(PosiciondelObjetivo);

        if (Vector3.Distance(transform.position, PosiciondelObjetivo) < 1f)
        {
            SetNewPatrolPoint();
        }

        if (Vector3.Distance(transform.position, gata.transform.position) < attackDistance)
        {
            StartCoroutine(Cooldown_atack());
        }
    }

    void Attack()
    {
        MoveTowards(PosiciondelObjetivo);

        if (!yaAtaco && Vector3.Distance(transform.position, gata.transform.position) < .5f)
        {
            yaAtaco = true;

            Debug.Log("Ataque realizado!");

            Scr_ControladorBatalla batalla = Controlador.GetComponent<Scr_ControladorBatalla>();
            if (batalla.VidaActual >= DañoMelee)
            {
                batalla.VidaActual -= DañoMelee;
            }
            else
            {
                batalla.VidaActual = 0;
            }

            currentState = State.Retreat;
            PosiciondelObjetivo = patrolCenter + new Vector3(
                Random.Range(-patrolRadius, patrolRadius),
                retreatHeight,
                Random.Range(-patrolRadius, patrolRadius)
            );
        }
    }

    void Retreat()
    {
        MoveTowards(PosiciondelObjetivo);

        if (Vector3.Distance(transform.position, PosiciondelObjetivo) < 1f)
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
    IEnumerator Cooldown_atack()
    {
        float waitTime = Random.Range(4f, 10f);
        yield return new WaitForSeconds(waitTime);
        currentState = State.Attack; 
        Vector3 targetPos = gata.transform.position;
        targetPos.y += 1f; // baja para atacar, pero no toca el suelo
        PosiciondelObjetivo = targetPos;
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
        PosiciondelObjetivo = patrolCenter + new Vector3(x, retreatHeight, z); // Usa siempre altura fija
    }
}
