using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Scr_SpawnerEnemigosAfuera : MonoBehaviour
{
    [SerializeField] private string IDSpawner;
    [SerializeField] private GameObject Enemigo;
    [SerializeField] private int CantidadDeEnemigos;
    [SerializeField] private float TiempoSpawn;

    private List<GameObject> Enemigos = new List<GameObject>();
    private float TiempoRestanteSpawn;

    void Start()
    {
        // Recuperar el tiempo de respawn guardado
        TiempoRestanteSpawn = PlayerPrefs.GetFloat($"{IDSpawner}_TiempoRestanteSpawn", TiempoSpawn);

        RestaurarEnemigos(); // Cargar enemigos en escena

        // Iniciar la rutina de respawn si faltan enemigos
        StartCoroutine(SpawnEnemies());
    }

    void FixedUpdate()
    {
        GuardarEstado();
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (Enemigos.Count < CantidadDeEnemigos) // Si faltan enemigos
            {
                Debug.Log($"Faltan {CantidadDeEnemigos - Enemigos.Count} enemigos, esperando {TiempoRestanteSpawn} segundos.");
                yield return new WaitForSeconds(TiempoRestanteSpawn);

                // Spawnear un enemigo
                GameObject nuevoEnemigo = Instantiate(Enemigo, transform.position, Quaternion.identity, transform.parent.parent);
                Enemigos.Add(nuevoEnemigo);

                // Reiniciar el tiempo de espera solo si sigue habiendo espacio
                if (Enemigos.Count < CantidadDeEnemigos)
                {
                    TiempoRestanteSpawn = TiempoSpawn;
                    PlayerPrefs.SetFloat($"{IDSpawner}_TiempoRestanteSpawn", TiempoRestanteSpawn);
                }
            }
            yield return null;
        }
    }

    void GuardarEstado()
    {
        int i = 0;
        foreach (var enem in Enemigos)
        {
            Vector3 pos = Enemigos[i].transform.position;
            if (enem.GetComponent<Scr_CambiadorBatalla>().Cambiando)
            {
                Debug.Log("Remueve");
                Enemigos.RemoveAt(i);
                PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoX_{i}");
                PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoY_{i}");
                PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoZ_{i}");
                break;
            }
            i++;
        }
        // Guardar la cantidad de enemigos vivos
        PlayerPrefs.SetInt($"{IDSpawner}_CantidadEnemigosVivos", Enemigos.Count);

        // Guardar posiciones de enemigos vivos
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

        PlayerPrefs.Save();
    }

    void RestaurarEnemigos()
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
            Enemigos.Add(enemigoRestaurado);
        }
    }
}
