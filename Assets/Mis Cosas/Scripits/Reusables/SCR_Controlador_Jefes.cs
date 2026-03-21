using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class SCR_Controlador_Jefes : MonoBehaviour
{
    [Header("Hud")]
    [SerializeField] float TiempoEntreOleadas;
    [SerializeField] TextMeshProUGUI TextoCantidadFrutas;
    [SerializeField] GameObject BarraJefe;
    [SerializeField] TextMeshProUGUI Objetivo;

    [Header("Cuenta Regresiva")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip SonidoPelea;

    [Header("Enemigos Base")]
    [SerializeField] GameObject[] Enemigos;
    [SerializeField] int[] CantidadEnemigos;

    [Header("Spawn")]
    private Transform Spawn;
    [SerializeField] float TiempoRevision = 1.5f;
    private Transform[] puntosSpawn;
    public List<GameObject> enemigosOleada = new List<GameObject>();

    [Header("Control Distancia Enemigos")] // NUEVO
    [SerializeField] float DistanciaMaximaJugador = 60f; // NUEVO: distancia máxima antes de teletransportar

    [Header("Recompensas")]
    [SerializeField] public Scr_CreadorObjetos[] Recompensa;

    private Scr_DatosSingletonBatalla singleton;
    public int CantidadPlantas = 0;

    private Scr_ControladorBatalla ControladorBatalla;
    private Transform jugador;

    private Coroutine detector_Enemigos;
    void Start()
    {
        ControladorBatalla = GetComponent<Scr_ControladorBatalla>();
        singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
    }

    // ------------------------------------------------
    // BUSCAR PUNTOS DE SPAWN
    // ------------------------------------------------
    public void obtenerSpawns()
    {
        Transform mapa = GameObject.Find("Mapa").transform;

        foreach (Transform zona in mapa)
        {
            if (zona.name == singleton.NombreMapa)
            {
                Transform planta = zona.Find("Spawners");

                if (planta != null)
                {
                    Spawn = planta;
                }
                else
                {
                    Debug.Log("No hay spawners");
                }
            }
        }

        puntosSpawn = new Transform[Spawn.childCount];

        for (int i = 0; i < Spawn.childCount; i++)
        {
            puntosSpawn[i] = Spawn.GetChild(i);
        }
    }

    // ------------------------------------------------
    // ACTIVAR MOVIMIENTO DE LOS ENEMIGOS
    // ------------------------------------------------
    public void IniciarAtaque()
    {
        foreach (GameObject enemigoGO in enemigosOleada)
        {
            if (enemigoGO != null)
            {
                NavMeshAgent agent = enemigoGO.GetComponent<NavMeshAgent>();

                if (agent != null)
                    agent.enabled = true;
            }
        }
    }

    // ------------------------------------------------
    // INICIAR SISTEMA
    // ------------------------------------------------
    public void IniciarExploracion()
    {
        obtenerSpawns();

        GameObject objJugador = GameObject.FindGameObjectWithTag("Gata");

        if (objJugador != null)
            jugador = objJugador.transform;

        detector_Enemigos= StartCoroutine(ControlarEnemigos());
    }

    // ------------------------------------------------
    // CONTROLADOR DE SPAWN
    // ------------------------------------------------
    IEnumerator ControlarEnemigos()
    {
        while (true)
        {
            LimpiarLista();

            RevisarDistanciaEnemigos(); // NUEVO: evitar enemigos demasiado lejos

            int tipo = ObtenerTipoFaltante();

            if (tipo != -1)
            {
                Vector3 posicionSpawn = ObtenerPosicionSpawn();

                GameObject enemigo = Instantiate(Enemigos[tipo], posicionSpawn, Quaternion.identity);

                enemigosOleada.Add(enemigo);

                NavMeshAgent agent = enemigo.GetComponent<NavMeshAgent>();

                if (agent != null && agent.isOnNavMesh)
                {
                    agent.enabled = false;
                }
                else
                {
                    Debug.LogWarning("El enemigo no está en el NavMesh.");
                }
            }

            yield return new WaitForSeconds(TiempoRevision);
        }
    }

    // ------------------------------------------------
    // TELETRANSPORTAR ENEMIGOS LEJANOS
    // ------------------------------------------------
    void RevisarDistanciaEnemigos() // NUEVO
    {
        if (jugador == null) return;

        foreach (GameObject enemigo in enemigosOleada)
        {
            if (enemigo == null) continue;

            float distancia = Vector3.Distance(jugador.position, enemigo.transform.position);

            if (distancia > DistanciaMaximaJugador)
            {
                NavMeshAgent agent = enemigo.GetComponent<NavMeshAgent>();

                if (agent != null && agent.isOnNavMesh)
                {
                    Vector3 nuevaPos = ObtenerPosicionSpawn();

                    agent.Warp(nuevaPos); // Forma segura de mover agentes NavMesh
                }
            }
        }
    }

    // ------------------------------------------------
    // ELEGIR TIPO DE ENEMIGO QUE FALTA
    // ------------------------------------------------
    int ObtenerTipoFaltante()
    {
        List<int> disponibles = new List<int>();

        for (int i = 0; i < Enemigos.Length; i++)
        {
            int faltan = CantidadEnemigos[i] - ContarEnemigos(Enemigos[i]);

            if (faltan > 0)
            {
                disponibles.Add(i);
            }
        }

        if (disponibles.Count == 0)
            return -1;

        return disponibles[Random.Range(0, disponibles.Count)];
    }

    // ------------------------------------------------
    // CONTAR ENEMIGOS ACTIVOS
    // ------------------------------------------------
    private int ContarEnemigos(GameObject ene)
    {
        int cont = 0;

        for (int i = 0; i < enemigosOleada.Count; i++)
        {
            if (enemigosOleada[i] != null && enemigosOleada[i].CompareTag(ene.tag))
            {
                cont++;
            }
        }

        return cont;
    }

    // ------------------------------------------------
    // LIMPIAR LISTA
    // ------------------------------------------------
    void LimpiarLista()
    {
        for (int i = enemigosOleada.Count - 1; i >= 0; i--)
        {
            if (enemigosOleada[i] == null)
                enemigosOleada.RemoveAt(i);
        }
    }

    // ------------------------------------------------
    // OBTENER POSICION DE SPAWN
    // ------------------------------------------------
    Vector3 ObtenerPosicionSpawn()
    {
        if (puntosSpawn.Length == 0 || jugador == null)
            return Vector3.zero;

        List<Transform> spawnsValidos = new List<Transform>();

        foreach (Transform spawn in puntosSpawn)
        {
            float distancia = Vector3.Distance(jugador.position, spawn.position);

            if (distancia > 15f && distancia < 40f)
            {
                spawnsValidos.Add(spawn);
            }
        }

        if (spawnsValidos.Count > 0)
        {
            int indice = Random.Range(0, spawnsValidos.Count);
            return spawnsValidos[indice].position;
        }

        int random = Random.Range(0, puntosSpawn.Length);
        return puntosSpawn[random].position;
    }

    // ------------------------------------------------
    // DESTRUIR ENEMIGOS
    // ------------------------------------------------
    public void DestruirTodosLosEnemigos()
    {
        for (int i = 0; i < Enemigos.Length; i++)
        {
            GameObject[] enemigos = GameObject.FindGameObjectsWithTag(Enemigos[i].tag);

            foreach (GameObject e in enemigos)
            {
                Destroy(e);
            }
        }
        StopCoroutine(detector_Enemigos);

    }


    public void DespertoJefe()
    {
        StartCoroutine(IniciarBarra());
    }
    IEnumerator IniciarBarra()
    {
        yield return new WaitForSeconds(3f);
        BarraJefe.SetActive(true);
    }
}
