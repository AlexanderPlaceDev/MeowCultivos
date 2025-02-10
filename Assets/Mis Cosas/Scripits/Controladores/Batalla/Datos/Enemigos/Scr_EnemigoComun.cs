using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoComun : Scr_Enemigo
{
    public float CoolDownAtaque;

    [SerializeField] Scr_VisionEnemigosPelea Vision;
    [SerializeField] float RadioDeambulacion;
    [SerializeField] float TiempoDeEsperaMin;
    [SerializeField] float TiempoDeEsperaMax;
    [SerializeField] float TiempoDeEsperaEntreAtaque;
    [SerializeField] Animator Anim;

    [Header("Configuración de Raycast")]
    [SerializeField] float OffsetOrigen = 1f; // Altura del enemigo
    [SerializeField] float OffsetDestino = 1f; // Altura de la gata

    private NavMeshAgent agente;
    private float temporizadorEspera;
    private bool esperando = false; // Indica si el enemigo está esperando
    private bool Atacando = false; // Indica si el enemigo está Atacando

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        // Verifica si el agente está sobre el NavMesh antes de asignarle un destino
        temporizadorEspera = Random.Range(TiempoDeEsperaMin, TiempoDeEsperaMax);
        if (agente.isOnNavMesh)
        {
            MoverANuevaPosicion();
        }
    }

    void Update()
    {
        if (Vision.Gata != null)
        {
            if (PuedeVerGata())
            {
                Objetivo = Vision.Gata.transform;
                Debug.Log("Gata detectada, asignando objetivo: " + Objetivo.name);

                float distancia = Vector3.Distance(transform.position, Objetivo.position);
                Debug.Log("Distancia a la gata: " + distancia);

                if (distancia <= agente.stoppingDistance)
                {
                    Debug.Log("Listo para atacar");
                    agente.isStopped = true;

                    if (!Atacando)
                    {
                        StartCoroutine(Atacar());
                    }
                }
                else
                {
                    Debug.Log("Entra en persecución");
                    agente.isStopped = false;
                    Mover();
                }
            }
            else
            {
                Debug.Log("No puede ver a la gata, patrullando...");
                Objetivo = null;
                Deambular();
            }
        }
        else
        {
            Debug.Log("No hay gata en la escena, patrullando...");
            Objetivo = null;
            Deambular();

        }
    }


    void Mover()
    {
        if (agente != null && agente.isActiveAndEnabled && agente.isOnNavMesh)
        {
            if (Objetivo != null)
            {
                if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("Mover"))
                {
                    Anim.Play("Mover");
                }
                agente.isStopped = false;
                agente.destination = Objetivo.position;
            }
            else
            {
                agente.isStopped = true;
            }
        }
    }

    void Deambular()
    {
        // Si el enemigo está esperando, cuenta el tiempo
        if (esperando)
        {
            temporizadorEspera -= Time.deltaTime;
            if (temporizadorEspera <= 0)
            {
                esperando = false; // Deja de esperar y busca un nuevo destino
                MoverANuevaPosicion();
            }
            return; // No hace nada hasta que termine la espera
        }

        // Verifica que el agente está en el NavMesh antes de llamar a remainingDistance
        if (agente.isOnNavMesh && !agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
        {
            Anim.SetBool("Caminando", false);
            esperando = true;
            temporizadorEspera = Random.Range(TiempoDeEsperaMin, TiempoDeEsperaMax);
        }
    }

    void MoverANuevaPosicion()
    {
        int intentos = 0;
        while (intentos < 10)
        {
            Vector3 randomDirection = Random.insideUnitSphere * RadioDeambulacion;
            randomDirection += transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, RadioDeambulacion, NavMesh.AllAreas))
            {
                if (agente.isOnNavMesh)
                {
                    agente.isStopped = false;  // 🔹 ¡IMPORTANTE! Asegurar que se mueva
                    agente.SetDestination(hit.position);
                    Anim.SetBool("Caminando", true);
                    Debug.Log("Moviéndose a nueva posición: " + hit.position);
                    return;
                }
            }
            intentos++;
        }
        Debug.LogWarning("No se encontró un punto válido en el NavMesh después de varios intentos.");
    }

    bool PuedeVerGata()
    {
        if (Vision.Gata == null) return false;

        Vector3 origenRaycast = transform.position + Vector3.up * OffsetOrigen;
        Vector3 destinoRaycast = Vision.Gata.transform.position + Vector3.up * OffsetDestino;
        Vector3 direccion = destinoRaycast - origenRaycast;

        RaycastHit hit;
        if (Physics.Raycast(origenRaycast, direccion, out hit, direccion.magnitude))
        {
            if (hit.collider.CompareTag("Gata"))
            {
                Debug.DrawLine(origenRaycast, destinoRaycast, Color.green); // Debug en la escena
                return true; // No hay obstáculos
            }
            else
            {
                Debug.DrawLine(origenRaycast, hit.point, Color.red); // Debug en la escena
                return false; // Algo está bloqueando la vista
            }
        }
        return false;
    }

    IEnumerator Atacar()
    {
        Tween.ShakeCamera(Camera.main, 3);
        Atacando = true;
        Anim.Play("Ataque1");
        if (Controlador.GetComponent<Scr_ControladorBatalla>().VidaActual >= DañoMelee)
        {
            Controlador.GetComponent<Scr_ControladorBatalla>().VidaActual -= DañoMelee;
        }
        else
        {
            Controlador.GetComponent<Scr_ControladorBatalla>().VidaActual -= Controlador.GetComponent<Scr_ControladorBatalla>().VidaActual;
        }
        yield return new WaitForSeconds(TiempoDeEsperaEntreAtaque);
        Atacando = false;
    }
}
