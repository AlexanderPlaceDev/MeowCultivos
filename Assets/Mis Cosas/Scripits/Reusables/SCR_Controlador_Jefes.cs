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
    private Transform Spawn; // Padre que contiene todos los puntos de spawn
    [SerializeField] float TiempoRevision = 2f;
    private Transform[] puntosSpawn;
    public List<GameObject> enemigosOleada = new List<GameObject>();


    [Header("Recompensas")]
    [SerializeField] public Scr_CreadorObjetos[] Recompensa;


    private Scr_DatosSingletonBatalla singleton;
    public int CantidadPlantas = 0;

    private Scr_ControladorBatalla ControladorBatalla;
    private Transform jugador;

    void Start()
    {
        ControladorBatalla = GetComponent<Scr_ControladorBatalla>();
        singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();


    }

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

        // Obtener todos los hijos del objeto Spawn como puntos de aparición
        puntosSpawn = new Transform[Spawn.childCount];

        for (int i = 0; i < Spawn.childCount; i++)
        {
            puntosSpawn[i] = Spawn.GetChild(i);
        }
    }
    public void IniciarExploracion()
    {
        obtenerSpawns();
        // Buscar jugador por tag
        GameObject objJugador = GameObject.FindGameObjectWithTag("Gata");

        if (objJugador != null)
            jugador = objJugador.transform;

        StartCoroutine(ControlarEnemigos());
    }

    IEnumerator ControlarEnemigos()
    {
        while (true)
        {
            Debug.Log(Enemigos.Length);
            for (int i = 0; i < Enemigos.Length; i++)
            {
                int faltan = CantidadEnemigos[i] - ContarEnemegigos(Enemigos[i]);

                if (faltan > 0)
                {
                    for (int j = 0; j < faltan; j++)
                    {
                        Vector3 posicionSpawn = ObtenerPosicionSpawn();
                        // Instanciar enemigo
                        GameObject enemigo = Instantiate(Enemigos[i], posicionSpawn, Quaternion.identity);
                        enemigosOleada.Add(enemigo);
                        // Verificar agente
                        NavMeshAgent agent = enemigo.GetComponent<NavMeshAgent>();
                        if (agent != null && agent.isOnNavMesh)
                        {
                            agent.enabled = false; // se activará después de la cuenta
                        }
                        else
                        {
                            Debug.LogWarning("El enemigo no está en el NavMesh.");
                        }
                    }
                }
            }

            yield return new WaitForSeconds(TiempoRevision);
        }
    }

    private int ContarEnemegigos(GameObject ene)
    {
        int cont = 0;
        for (int i = 0; i < enemigosOleada.Count; i++)
        {
            if (enemigosOleada[i] == ene)
            {
                cont++;
            }
            if (enemigosOleada[i] == null)
            {
                enemigosOleada.Remove(enemigosOleada[i]);
            }
        }
        return cont;
    }
    Vector3 ObtenerPosicionSpawn()
    {
        if (puntosSpawn.Length == 0 || jugador == null)
            return Vector3.zero;

        List<Transform> spawnsValidos = new List<Transform>();

        foreach (Transform spawn in puntosSpawn)
        {
            float distancia = Vector3.Distance(jugador.position, spawn.position);

            // rango natural de combate
            if (distancia > 15f && distancia < 40f)
            {
                spawnsValidos.Add(spawn);
            }
        }

        // si encontramos spawns en el rango
        if (spawnsValidos.Count > 0)
        {
            int indice = Random.Range(0, spawnsValidos.Count);
            return spawnsValidos[indice].position;
        }

        // fallback por si no hay ninguno
        int random = Random.Range(0, puntosSpawn.Length);
        return puntosSpawn[random].position;
    }

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

        StartCoroutine(IniciarBarra());
    }
    IEnumerator IniciarBarra()
    {
        yield return new WaitForSeconds(3f);
        BarraJefe.SetActive(true);
    }
}
