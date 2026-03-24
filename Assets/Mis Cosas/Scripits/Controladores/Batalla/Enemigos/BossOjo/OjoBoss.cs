using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static Scr_CreadorMisiones;

public class OjoBoss : Scr_Enemigo
{
    [SerializeField] Animator Anim;
    public GameObject barraVida;
    public TextMeshProUGUI vidaText;

    public float menosVida=0;

    private GameObject Gata;          // objetivo del enemigo
    [SerializeField] private NavMeshAgent agente;      // componente de navegación
    [SerializeField] GameObject[] Subtitos;
    [SerializeField] Transform[] SpawnSub;
    private float temporizadorEspera; // cooldown entre ataques
    private bool esperando = false;   // si espera entre ataques
    private bool Atacando = false;    // si está en animación de ataque
    private float ContAtaque = 0;     // timer de ataque actual
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
        Anim.SetBool("Dormido", true);
    }

    public void crizalidamenos(float more)
    {
        menosVida = menosVida + more;
    }
    public void Despertar()
    {
        Anim.SetBool("Dormido", false);
        StartCoroutine(EsperarAparicion(1.3f));
    }
    // espera hasta terminar animación de spawn
    IEnumerator EsperarAparicion(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        Aparecio = true;
        agente.enabled = true;
        if (agente.isOnNavMesh)
            agente.isStopped = false;

        barraVida.SetActive(true);

        Vida= Vida*(1-menosVida);
    }

    public void ActivarSubditos()
    {
        Instantiate(Subtitos[Random.Range(0, Subtitos.Length)], SpawnSub[0].position, Quaternion.identity);
        Instantiate(Subtitos[Random.Range(0, Subtitos.Length)], SpawnSub[1].position, Quaternion.identity);
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

        if (agente.isOnNavMesh)
        {
            Anim.SetBool("Moviendo", !agente.isStopped);
        }
        // Si tiene objetivo
        if (Objetivo != null)
        {
            float distancia = Vector3.Distance(transform.position, Objetivo.position);

            // Si está en cooldown entre ataques
            if (esperando)
                temporizadorEspera -= Time.deltaTime;

            if (temporizadorEspera <= 0)
                esperando = false;
            if (distancia <= agente.stoppingDistance + 2.8f)
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
                Objetivo = Gata.transform;
                agente.SetDestination(Objetivo.position);
            }
        }

        if (barraVida.activeSelf)
        {
            Slider bvida= barraVida.transform.GetChild(0).GetComponent<Slider>();
            float vid = Vida / VidaMaxima;
            bvida.value = Mathf.Clamp(vid,0,1);
            vidaText.text = Vida + "/" + VidaMaxima;
        }
    }

    void Mover()
    {
        if (!agente.isActiveAndEnabled || !agente.isOnNavMesh) return;
        if (estaEmpujado) return; // evita que el empujón se anule

        

        agente.isStopped = false;

        Objetivo = Gata.transform;
        /*
        // rotación suave hacia el jugador
        Vector3 dir = (Objetivo.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
        */
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
            Anim.Play("ataque1");
            DuracionDeAtaque = Anim.GetCurrentAnimatorClipInfo(0).Length;
        }
        else
        {
            Anim.Play("ataque2");
            DuracionDeAtaque = Anim.GetCurrentAnimatorClipInfo(0).Length;
        }

    }


    public void Ataque(Transform PuntodeArma, string efecfto)
    {
        // rango es currentWeapon.range, attackOrigin es el punto del jugador (ej. frente)
        Vector3 center = PuntodeArma != null ? PuntodeArma.position : transform.position;
        float radius = DistanciaDeAtaque;

        Collider[] colliders = Physics.OverlapSphere(center, radius);

        foreach (Collider col in colliders)
        {

            if (col.gameObject.CompareTag("Gata"))
            {
                Scr_ControladorBatalla batalla = Controlador.GetComponent<Scr_ControladorBatalla>();
                batalla.RecibirDaño(DañoMelee);
                batalla.RecibirEfecto(efecfto.ToString());
                // efectos del ataque
                Tween.ShakeCamera(Camera.main, 3);
            }
        }
    }
}
