using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scr_SpawnerEnemigosAfuera : MonoBehaviour
{
    [Header("Configuración del Spawner")]
    [SerializeField] private string IDSpawner;
    [SerializeField] private GameObject Enemigo;
    [SerializeField] private int CantidadDeEnemigos = 1;
    [SerializeField] private int Enemigos_normales = 1;
    [SerializeField] private float TiempoSpawn = 5f;
    [SerializeField] private bool UsaTiempo = false;

    [Header("Configuración de tiempo")]
    [SerializeField] private int HoraInicio;
    [SerializeField] private int HoraFin;
    [SerializeField] private float TiempoRestanteSpawn;

    [Header("Activación del Spawner")]
    [Tooltip("Si está activado, ignora PlayerPrefs y considera el spawner siempre activo.")]
    [SerializeField] private bool NoUsaHaveActive = false;
    [SerializeField] public int haveAcivate = 0;

    private List<GameObject> Enemigos = new List<GameObject>();
    public Scr_ControladorTiempo ControlT;
    void Start()
    {
        ControlT = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();

        // Recuperar el tiempo de respawn guardado
        TiempoRestanteSpawn = PlayerPrefs.GetFloat($"{IDSpawner}_TiempoRestanteSpawn", TiempoSpawn);
        RestaurarEnemigos(); // Cargar enemigos en escena
        StartCoroutine(SpawnEnemies()); // Iniciar la rutina
    }

    void FixedUpdate()
    {
        GuardarEstado();
        checartiempoDeNOSpawn();
        ActivarLunaRoja();
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Si NoUsaHaveActive está activo, tratamos haveAcivate como 1 siempre
            int estado = NoUsaHaveActive ? 1 : haveAcivate;

            if (estado == 1 && checartiempoDeSpawn())
            {
                if (Enemigos.Count < CantidadDeEnemigos)
                {
                    while (TiempoRestanteSpawn > 0f)
                    {
                        TiempoRestanteSpawn -= Time.deltaTime;
                        PlayerPrefs.SetFloat($"{IDSpawner}_TiempoRestanteSpawn", TiempoRestanteSpawn);
                        yield return null;
                    }

                    // Spawnear enemigo
                    GameObject nuevoEnemigo = Instantiate(Enemigo, transform.position, Quaternion.identity, transform.parent.parent);
                    Enemigos.Add(nuevoEnemigo);

                    // Reiniciar tiempo
                    TiempoRestanteSpawn = TiempoSpawn;
                    PlayerPrefs.SetFloat($"{IDSpawner}_TiempoRestanteSpawn", TiempoRestanteSpawn);
                    PlayerPrefs.Save();
                }
                else
                {
                    foreach (var enem in Enemigos)
                    {
                        Destroy(enem);
                    }
                    Enemigos.Clear();
                }
            }
            yield return null;
        }
    }
    public bool ChecarLunaRoja()
    {
       
        if (ControlT.ClimaSemanal.Count>0 && ControlT.ClimaSemanal[ControlT.ConseguirDia()].ToString() == "LunaRoja" && ControlT.EstaActivoClima)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ActivarLunaRoja()
    {
        if(ChecarLunaRoja())
        {
            CantidadDeEnemigos = Enemigos_normales * 2;
        }
        else
        {
            CantidadDeEnemigos = Enemigos_normales;
        }
    }
    public bool checartiempoDeSpawn()
    {
        if (!UsaTiempo) return true;

        if (HoraInicio < HoraFin)
        {
            return (ControlT.HoraActual >= HoraInicio) && (ControlT.HoraActual < HoraFin);
        }
        else
        {
            return (ControlT.HoraActual >= HoraInicio) || (ControlT.HoraActual < HoraFin);
        }
    }
    
    public void checartiempoDeNOSpawn()
    {
        if (!UsaTiempo) return;
        if (Enemigos.Count <= 0) return;

        bool dentroHorario;

        if (HoraInicio < HoraFin)
        {
            dentroHorario = ControlT.HoraActual >= HoraInicio && ControlT.HoraActual < HoraFin;
        }
        else
        {
            dentroHorario = ControlT.HoraActual >= HoraInicio || ControlT.HoraActual < HoraFin;
        }

        if (!dentroHorario)
        {
            foreach (var enem in Enemigos)
            {
                Destroy(enem);
            }
            Enemigos.Clear();
        }
    }

    public void GuardarEstado()
    {
        int estado = NoUsaHaveActive ? 1 : haveAcivate;

        if (estado == 1 && checartiempoDeSpawn())
        {
            int i = 0;
            foreach (var enem in Enemigos)
            {
                Vector3 pos = Enemigos[i].transform.position;
                if (enem.GetComponent<Scr_CambiadorBatalla>())
                {
                    if (enem.GetComponent<Scr_CambiadorBatalla>().Cambiando)
                    {
                        Debug.Log("Remueve");
                        Enemigos.RemoveAt(i);
                        PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoX_{i}");
                        PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoY_{i}");
                        PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoZ_{i}");
                        break;
                    }
                }

                i++;
            }

            PlayerPrefs.SetInt($"{IDSpawner}_CantidadEnemigosVivos", Enemigos.Count);

            for (i = 0; i < Enemigos.Count; i++)
            {
                if (Enemigos[i] != null)
                {
                    Vector3 pos = Enemigos[i].transform.position;
                    PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoX_{i}", pos.x);
                    PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoY_{i}", pos.y);
                    PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoZ_{i}", pos.z);
                }
            }
        }

        // Guardar estado real
        PlayerPrefs.SetInt($"{IDSpawner}_Active", estado);
        PlayerPrefs.Save();
    }

    void RestaurarEnemigos()
    {
        haveAcivate = PlayerPrefs.GetInt($"{IDSpawner}_Active");

        // Forzar activación si NoUsaHaveActive está en true
        if (NoUsaHaveActive)
            haveAcivate = 1;

        if (haveAcivate == 1)
        {
            int enemigosVivos = PlayerPrefs.GetInt($"{IDSpawner}_CantidadEnemigosVivos", CantidadDeEnemigos);

            for (int i = 0; i < enemigosVivos; i++)
            {
                string keyX = $"{IDSpawner}_EnemigoX_{i}";
                string keyY = $"{IDSpawner}_EnemigoY_{i}";
                string keyZ = $"{IDSpawner}_EnemigoZ_{i}";

                Vector3 pos = transform.position;
                if (PlayerPrefs.HasKey(keyX) && PlayerPrefs.HasKey(keyY) && PlayerPrefs.HasKey(keyZ))
                {
                    pos = new Vector3(
                        PlayerPrefs.GetFloat(keyX),
                        PlayerPrefs.GetFloat(keyY),
                        PlayerPrefs.GetFloat(keyZ)
                    );
                }

                GameObject enemigoRestaurado = Instantiate(Enemigo, pos, Quaternion.identity, transform.parent.parent);
                if (!enemigoRestaurado.GetComponent<NavMeshAgent>().isOnNavMesh)
                {
                    Debug.Log(enemigoRestaurado.name);
                }
                Enemigos.Add(enemigoRestaurado);
            }
        }
    }
}
