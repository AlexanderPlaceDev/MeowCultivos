using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Scr_MovimientoEnemigosFuera : MonoBehaviour
{
    public GameObject Jugador;  // El jugador que el enemigo podría perseguir
    public float radioDeambulacion = 10f;
    public float tiempoEspera = 5f;  // Tiempo entre deambulaciones
    public float velocidad = 3.5f;
    public bool Huye = false;
    public float distanciaHuir = 15f;  // Distancia a la que deja de huir y vuelve a patrullar

    private float TiempoHuida = 3;
    private NavMeshAgent agente;
    private float temporizador;
    public bool SeEstaMoviendo = false;
    Animator Animador;
    public bool estaEnPausa = false;  // Nueva variable para controlar si está en pausa

    public enum EstadoEnemigo
    {
        Patrullando,
        Persiguiendo,
        Ocultandose,
        Huyendo,
        Disparando
    }

    // Definir el estado actual del enemigo
    public EstadoEnemigo estadoActual;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidad;
        Animador = GetComponent<Animator>();
        temporizador = tiempoEspera;  // Inicializamos el temporizador para el deambular
        estadoActual = EstadoEnemigo.Patrullando;  // Iniciar en el estado de patrullando
    }

    void Update()
    {
        switch (estadoActual)
        {
            case EstadoEnemigo.Patrullando:
                Patrullar();
                break;
            case EstadoEnemigo.Persiguiendo:
                if (Jugador != null)
                {
                    PerseguirJugador();
                }
                break;
            case EstadoEnemigo.Ocultandose:
                Ocultarse();
                break;
            case EstadoEnemigo.Huyendo:
                if (Jugador != null)
                {
                    Huir();
                }
                else
                {
                    TiempoHuida = 3;
                    Patrullar();
                }
                break;
            case EstadoEnemigo.Disparando:
                Disparar();
                break;
        }

        // Si ve al jugador, cambia a perseguir o huir según la situación
        if (Jugador != null)
        {
            if (Huye)
            {
                if (estadoActual != EstadoEnemigo.Huyendo)
                {
                    CambiarEstado(EstadoEnemigo.Huyendo);
                }
            }
            else
            {
                if (estadoActual != EstadoEnemigo.Persiguiendo)
                {
                    CambiarEstado(EstadoEnemigo.Persiguiendo);
                }
            }
        }
    }

    // Función para cambiar de estado
    void CambiarEstado(EstadoEnemigo nuevoEstado)
    {
        estadoActual = nuevoEstado;
        SeEstaMoviendo = false; // Reiniciar el estado de movimiento al cambiar de estado
    }

    // Estado de patrullar
    void Patrullar()
    {
        temporizador += Time.deltaTime;

        if (temporizador >= tiempoEspera && !estaEnPausa)
        {
            MoverAUnaNuevaPosicion();
            temporizador = 0;
            SeEstaMoviendo = true;
            Animador.SetBool("Caminando", true);
        }

        if (agente.remainingDistance <= agente.stoppingDistance && !agente.pathPending)
        {
            StartCoroutine(PausaAntesDeMover());
        }
    }

    // Estado de perseguir al jugador
    void PerseguirJugador()
    {
        SeEstaMoviendo = true;
        agente.isStopped = false;
        agente.SetDestination(Jugador.transform.position);
        Animador.SetBool("Caminando", true);

        // Cambiar a disparar si está suficientemente cerca
        if (Vector3.Distance(transform.position, Jugador.transform.position) <= 5f)
        {
            CambiarEstado(EstadoEnemigo.Disparando);
        }
    }

    // Estado de ocultarse
    void Ocultarse()
    {
        agente.isStopped = false;
        Vector3 posicionCobertura = EncontrarCobertura();
        agente.SetDestination(posicionCobertura);
        Animador.SetBool("Caminando", true);
    }

    // Estado de huir
    void Huir()
    {

        TiempoHuida += Time.deltaTime;

        if (TiempoHuida >= 3)
        {
            SeEstaMoviendo = true;
            agente.isStopped = false;
            MoverAUnaNuevaPosicion();
            temporizador = 0;
            Animador.SetBool("Caminando", true);

            // Cambiar a disparar si está suficientemente cerca
            if (Vector3.Distance(transform.position, Jugador.transform.position) <= 5f)
            {
                CambiarEstado(EstadoEnemigo.Disparando);
            }
            TiempoHuida = 0;
        }

    }

    // Estado de disparar al jugador
    void Disparar()
    {
        agente.isStopped = true;
        Animador.SetBool("Caminando", false);
        Debug.Log("Disparando al jugador!");
    }

    // Moverse a una posición aleatoria dentro del radio especificado
    void MoverAUnaNuevaPosicion()
    {
        Vector3 nuevaPosicion = RandomNavSphere(transform.position, radioDeambulacion);
        agente.SetDestination(nuevaPosicion);
    }

    public static Vector3 RandomNavSphere(Vector3 origen, float distancia)
    {
        Vector3 direccionAleatoria = Random.insideUnitSphere * distancia;
        direccionAleatoria += origen;

        NavMeshHit hit;
        NavMesh.SamplePosition(direccionAleatoria, out hit, distancia, NavMesh.AllAreas);

        return hit.position;
    }

    IEnumerator PausaAntesDeMover()
    {
        SeEstaMoviendo = false;
        agente.isStopped = true;
        Animador.SetBool("Caminando", false);
        estaEnPausa = true;

        float tiempoPausa = Random.Range(2f, 5f);
        float tiempoPausado = 0f;

        while (tiempoPausado < tiempoPausa && Jugador == null)
        {
            tiempoPausado += Time.deltaTime;
            yield return null;
        }

        estaEnPausa = false;
        agente.isStopped = false;
    }

    // Encuentra una posición de cobertura para el enemigo
    Vector3 EncontrarCobertura()
    {
        return RandomNavSphere(transform.position, radioDeambulacion);
    }
}
