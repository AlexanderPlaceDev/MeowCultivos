using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_SpawnerEnemigosAfueraVariable : MonoBehaviour
{
    [SerializeField] private string IDSpawner;
    [SerializeField] private GameObject Enemigo;
    [SerializeField] private GameObject Objeto;
    [SerializeField] private int CantidadDeEnemigos;
    [SerializeField] private float TiempoSpawn;
    [SerializeField] private float ProbabilidadSpawn;
    [SerializeField] private string CinematicaRequerida;

    private List<GameObject> Entidades = new List<GameObject>();
    private float TiempoRestanteSpawn;

    // NUEVO: controla solo el spawn de enemigos
    private bool PuedeSpawnearEnemigos = true;

    void Start()
    {
        // 🔹 Regla de cinemática
        if (!string.IsNullOrEmpty(CinematicaRequerida))
        {
            string key = "Cinematica " + CinematicaRequerida;
            PuedeSpawnearEnemigos = PlayerPrefs.GetString(key, "No") == "Si";
        }
        else
        {
            // Cinemática vacía → enemigos permitidos
            PuedeSpawnearEnemigos = true;
        }

        TiempoRestanteSpawn = PlayerPrefs.GetFloat($"{IDSpawner}_TiempoRestanteSpawn", TiempoSpawn);
        RestaurarEntidades();
        StartCoroutine(SpawnLoop());
    }

    void FixedUpdate()
    {
        GuardarEstado();
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (Entidades.Count < CantidadDeEnemigos)
            {
                yield return new WaitForSeconds(TiempoRestanteSpawn);

                SpawnearEntidad();

                if (Entidades.Count < CantidadDeEnemigos)
                {
                    TiempoRestanteSpawn = TiempoSpawn;
                    PlayerPrefs.SetFloat($"{IDSpawner}_TiempoRestanteSpawn", TiempoRestanteSpawn);
                }
            }
            yield return null;
        }
    }

    void SpawnearEntidad()
    {
        float randomValue = Random.Range(0f, 100f);
        GameObject nuevaEntidad;

        bool puedeSalirEnemigo = PuedeSpawnearEnemigos && randomValue < ProbabilidadSpawn;

        if (puedeSalirEnemigo)
        {
            nuevaEntidad = Instantiate(Enemigo, transform.position, Quaternion.identity, transform.parent.parent);
        }
        else
        {
            nuevaEntidad = Instantiate(Objeto, transform.position, Quaternion.identity, transform.parent.parent);
        }

        Entidades.Add(nuevaEntidad);

        nuevaEntidad.AddComponent<Scr_DesaparecerListener>().OnDesaparecer = () =>
        {
            Entidades.Remove(nuevaEntidad);

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(EsperarYSpawnear());
            }
        };
    }

    IEnumerator EsperarYSpawnear()
    {
        yield return new WaitForSeconds(TiempoSpawn);
        SpawnearEntidad();
    }

    void GuardarEstado()
    {
        List<int> indicesParaEliminar = new List<int>();

        for (int i = Entidades.Count - 1; i >= 0; i--)
        {
            if (Entidades[i] == null || (Entidades[i].GetComponent<Scr_CambiadorBatalla>()?.Cambiando ?? false))
            {
                indicesParaEliminar.Add(i);
                Entidades.RemoveAt(i);
            }
        }

        foreach (int i in indicesParaEliminar)
        {
            PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoX_{i}");
            PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoY_{i}");
            PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoZ_{i}");
        }

        PlayerPrefs.SetInt($"{IDSpawner}_CantidadEntidadesVivas", Entidades.Count);

        for (int i = 0; i < Entidades.Count; i++)
        {
            if (Entidades[i] != null)
            {
                Vector3 pos = Entidades[i].transform.position;
                PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoX_{i}", pos.x);
                PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoY_{i}", pos.y);
                PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoZ_{i}", pos.z);
            }
        }

        PlayerPrefs.SetFloat($"{IDSpawner}_TiempoRestanteSpawn", TiempoRestanteSpawn);
        PlayerPrefs.Save();
    }

    void RestaurarEntidades()
    {
        int entidadesVivas = PlayerPrefs.GetInt($"{IDSpawner}_CantidadEntidadesVivas", 0);
        TiempoRestanteSpawn = PlayerPrefs.GetFloat($"{IDSpawner}_TiempoRestanteSpawn", TiempoSpawn);

        for (int i = 0; i < entidadesVivas; i++)
        {
            string keyX = $"{IDSpawner}_EnemigoX_{i}";
            string keyY = $"{IDSpawner}_EnemigoY_{i}";
            string keyZ = $"{IDSpawner}_EnemigoZ_{i}";

            if (!PlayerPrefs.HasKey(keyX) || !PlayerPrefs.HasKey(keyY) || !PlayerPrefs.HasKey(keyZ))
            {
                StartCoroutine(EsperarYSpawnear());
                continue;
            }

            Vector3 pos = new Vector3(
                PlayerPrefs.GetFloat(keyX),
                PlayerPrefs.GetFloat(keyY),
                PlayerPrefs.GetFloat(keyZ)
            );

            float randomValue = Random.Range(0f, 100f);
            bool puedeSalirEnemigo = PuedeSpawnearEnemigos && randomValue < ProbabilidadSpawn;

            GameObject entidadRestaurada = puedeSalirEnemigo
                ? Instantiate(Enemigo, pos, Quaternion.identity, transform.parent.parent)
                : Instantiate(Objeto, pos, Quaternion.identity, transform.parent.parent);

            Entidades.Add(entidadRestaurada);
        }
    }
}

// Clase para detectar cuando un objeto o enemigo desaparece
public class Scr_DesaparecerListener : MonoBehaviour
{
    public System.Action OnDesaparecer;

    private void OnDestroy()
    {
        OnDesaparecer?.Invoke();
    }
}
