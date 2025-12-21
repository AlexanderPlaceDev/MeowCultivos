using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoJaba : Scr_Enemigo
{
    [SerializeField] Animator Anim;

    private GameObject Gata;          // objetivo del enemigo
    private NavMeshAgent agente;      // componente de navegación

    private float temporizadorEspera; // cooldown entre ataques
    private bool esperando = false;   // si espera entre ataques
    private bool Atacando = false;    // si está en animación de ataque
    private float ContAtaque = 0;     // timer de ataque actual

    private bool AtacandoFruta;
    protected override void Start()
    {
        base.Start();

        Gata = GameObject.Find("Personaje");
        agente = GetComponent<NavMeshAgent>();
        agente.speed = Velocidad;

        // detener movimiento mientras aparece
        if (agente.isOnNavMesh)
            agente.isStopped = true;

        // animación de aparición
        Anim.Play(NombreAnimacionAparecer);
        float duracion = Anim.GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(EsperarAparicion(duracion));
    }

    // espera hasta terminar animación de spawn
    IEnumerator EsperarAparicion(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        Aparecio = true;
        if (agente.isOnNavMesh)
            agente.isStopped = false;
    }

    void Update()
    {
        if (!Aparecio || EstaMuerto) return;
        if (agente == null) return;

        // Si está stuneado, congelado o empujado: NO MOVERSE
        if (estaStuneado || estaCongelado || estaEmpujado)
        {
            if (agente.isOnNavMesh)
                agente.isStopped = true;

            Anim.Play("Idle");
            return;
        }

        // Movimiento normal si no ataca o espera
        if (!Atacando && !esperando)
            Mover();

        // Si tiene objetivo
        if (Objetivo != null)
        {
            float distancia = Vector3.Distance(transform.position, Objetivo.position);

            // Si está en cooldown entre ataques
            if (esperando)
                temporizadorEspera -= Time.deltaTime;

            if (temporizadorEspera <= 0)
                esperando = false;

            if (distancia <= agente.stoppingDistance + 1f)
            {
                if (!esperando && !Atacando)
                {
                    agente.isStopped = true;
                    esperando = true;
                    Atacar();
                }
            }
            else
            {
                if (!esperando)
                    Mover();
            }

            // Ciclo de ataque
            if (Atacando && ContAtaque < DuracionDeAtaque)
                ContAtaque += Time.deltaTime;
            else
            {
                ContAtaque = 0;
                Atacando = false;
            }
        }
        else
        {
            // si pierde objetivo, reasignarlo
            if (agente.isOnNavMesh)
            {
                if (Fruta)
                {
                    Objetivo = BuscarPlanta("Planta").transform;
                    AtacandoFruta = true;
                }
                else if (Vida <= (Vida * .3))
                {
                    Objetivo = Gata.transform;
                    AtacandoFruta = false;
                }
                else
                {
                    Objetivo = Gata.transform;
                    AtacandoFruta = false;
                }
                agente.SetDestination(Objetivo.position);
            }
        }
    }

    void Mover()
    {
        if (!agente.isActiveAndEnabled || !agente.isOnNavMesh) return;
        if (estaEmpujado) return; // evita que el empujón se anule

        if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("Mover"))
            Anim.Play("Mover");

        agente.isStopped = false;
        if (Fruta)
        {
            Objetivo = BuscarPlanta("Planta").transform;
            AtacandoFruta = true;
        }
        else if (Vida <= (Vida * .3))
        {
            Objetivo = Gata.transform;
            AtacandoFruta = false;
        }
        else
        {
            Objetivo = Gata.transform;
            AtacandoFruta = false;
        }

        // rotación suave hacia el jugador
        Vector3 dir = (Objetivo.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }

        // seguir al jugador
        agente.SetDestination(Objetivo.position);
    }

    public GameObject BuscarPlanta(string tag)
    {
        Debug.Log("eee");
        GameObject[] objetos = GameObject.FindGameObjectsWithTag(tag);

        if (objetos.Length == 0)
            return null;

        GameObject masCercano = null;
        float distanciaMinima = Mathf.Infinity;
        Vector3 posicionActual = transform.position;

        foreach (GameObject obj in objetos)
        {
            float distancia = Vector3.Distance(posicionActual, obj.transform.position);

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                masCercano = obj;
            }
        }
        Debug.Log(masCercano.name);
        return masCercano;
    }
    void Atacar()
    {
        Debug.Log("Comenzo Atacar");

        Atacando = true;

        // detener movimiento durante ataque
        if (agente.isOnNavMesh)
            agente.isStopped = true;

        // 👉 Girar instantáneo hacia el jugador ANTES de atacar
        Vector3 dir = (Objetivo.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        // animación aleatoria
        if (Random.Range(0, 2) == 1)
        {
            Anim.Play("Ataque1");
            DuracionDeAtaque = 2.917f;
        }
        else
        {
            Anim.Play("Ataque2");
            DuracionDeAtaque = 1.042f;
        }
        if (AtacandoFruta)
        {
            GameObject plant = BuscarPlanta("Planta");
            plant.GetComponent<Aparecer_Fruta>().RecibirDaño(DañoMelee);
        }
        else
        {

            // efectos del ataque
            Tween.ShakeCamera(Camera.main, 3);

            Scr_ControladorBatalla batalla = Controlador.GetComponent<Scr_ControladorBatalla>();

            base.source.PlayOneShot(base.Golpe);
            batalla.RecibirDaño(DañoMelee);
            batalla.RecibirEfecto(base.Efecto.ToString());
        }
    }
}
