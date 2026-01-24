using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_Enemigo_OjoVolador : Scr_Enemigo
{
    [SerializeField] Animator Anim;

    public GameObject gata;

    public float VelocidadRotacion = 4f;
    public float AlturaPatrulla = 15f;
    public float AlturaAtaque = 2f;

    public Vector3 CentroPatrol;
    public float RadioPatrol = 10f;

    public float amplitudVuelo = 0.4f;
    public float frecuenciaVuelo = 2f;

    private float offsetVuelo;
    private bool yaAtaco = false;

    private Vector3 PosiciondelObjetivo;
    private Vector3 velocidadActual;

    private enum State
    {
        Patrol,
        PrepareAttack,   // se coloca arriba del jugador
        DiveAttack,      // picado
        Retreat,
        Cooldown
    }

    private State currentState = State.Patrol;

    protected override void Start()
    {
        base.Start();

        gata = GameObject.Find("Personaje");
        offsetVuelo = Random.Range(0f, 10f);

        Vector3 pos = transform.position;
        pos.y = AlturaPatrulla;
        transform.position = pos;

        SetNewPatrolPoint();

        Anim.Play(NombreAnimacionAparecer);
        StartCoroutine(EsperarAparicion(4f));
    }

    IEnumerator EsperarAparicion(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        Aparecio = true;
    }

    void Update()
    {
        if (!Aparecio || EstaMuerto) return;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.PrepareAttack:
                PrepareAttack();
                break;

            case State.DiveAttack:
                DiveAttack();
                break;

            case State.Retreat:
                Retreat();
                break;

            case State.Cooldown:
                break;
        }
    }

    // ================= ESTADOS =================

    void Patrol()
    {
        MoveTowards(PosiciondelObjetivo, false);

        if (Vector3.Distance(transform.position, PosiciondelObjetivo) < 1f)
            SetNewPatrolPoint();

        if (Vector3.Distance(transform.position, gata.transform.position) < DistanciaDeAtaque)
        {
            currentState = State.PrepareAttack;
        }
    }

    void PrepareAttack()
    {
        // Se coloca encima del jugador
        Vector3 arribaJugador = gata.transform.position;
        arribaJugador.y += AlturaPatrulla;
        PosiciondelObjetivo = arribaJugador;

        MoveTowards(PosiciondelObjetivo, false);

        if (Mathf.Abs(transform.position.y - PosiciondelObjetivo.y) < 0.5f)
        {
            currentState = State.DiveAttack;
            yaAtaco = false;
        }
    }

    void DiveAttack()
    {
        // Picado directo
        Vector3 picado = gata.transform.position;
        picado.y += AlturaAtaque;
        PosiciondelObjetivo = picado;

        MoveTowards(PosiciondelObjetivo, true);

        if (!yaAtaco &&
            Vector3.Distance(transform.position, gata.transform.position) < 2.5f)
        {
            yaAtaco = true;

            Scr_ControladorBatalla batalla =
                Controlador.GetComponent<Scr_ControladorBatalla>();

            batalla.VidaActual =
                Mathf.Max(0, batalla.VidaActual - DañoMelee);

            currentState = State.Retreat;

            Vector3 escape = transform.position;
            escape.y = AlturaPatrulla;
            PosiciondelObjetivo = escape;
        }
    }

    void Retreat()
    {
        MoveTowards(PosiciondelObjetivo, false);

        if (transform.position.y >= AlturaPatrulla - 0.5f)
        {
            StartCoroutine(Cooldown());
        }
    }

    // ================= CORRUTINAS =================

    IEnumerator Cooldown()
    {
        currentState = State.Cooldown;

        yield return new WaitForSeconds(Random.Range(2f, 4f));

        currentState = State.Patrol;
        SetNewPatrolPoint();
    }

    // ================= MOVIMIENTO =================

    void MoveTowards(Vector3 point, bool ataque)
    {
        Vector3 direccion = point - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            VelocidadRotacion * Time.deltaTime
        );

        float smoothTime = ataque ? 0.12f : 0.45f;
        float velocidadMax = ataque ? Velocidad * 1.8f : Velocidad;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            point,
            ref velocidadActual,
            smoothTime,
            velocidadMax
        );

        // Oscilación SOLO fuera del ataque
        if (!ataque)
        {
            Vector3 pos = transform.position;
            pos.y += Mathf.Sin(Time.time * frecuenciaVuelo + offsetVuelo)
                     * amplitudVuelo * Time.deltaTime;
            transform.position = pos;
        }
    }

    void SetNewPatrolPoint()
    {
        float x = Random.Range(-RadioPatrol, RadioPatrol);
        float z = Random.Range(-RadioPatrol, RadioPatrol);

        PosiciondelObjetivo = CentroPatrol + new Vector3(
            x,
            AlturaPatrulla,
            z
        );
    }
}
