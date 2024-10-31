using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_SpawnerEnemigosAfuera : MonoBehaviour
{
    [SerializeField] string IDSpawner; // Asegúrate de que sea único para cada Spawner
    [SerializeField] private GameObject Enemigo;
    [SerializeField] private int CantidadDeEnemigos;
    [SerializeField] private float TiempoSpawn;
    private GameObject[] Enemigos;

    void Start()
    {
        Enemigos = new GameObject[CantidadDeEnemigos];
        RestaurarPosiciones(); // Restaurar posiciones al iniciar el juego
        StartCoroutine(SpawnEnemies());
    }

    void LateUpdate()
    {
        GuardarPosiciones(); // Guardar las posiciones continuamente en cada frame
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            for (int i = 0; i < CantidadDeEnemigos; i++)
            {
                if (Enemigos[i] == null)
                {
                    Enemigos[i] = Instantiate(Enemigo, transform.position, Quaternion.identity, transform.parent.parent);
                    yield return new WaitForSeconds(TiempoSpawn);
                }
            }
            yield return null;
        }
    }

    void GuardarPosiciones()
    {
        for (int i = 0; i < CantidadDeEnemigos; i++)
        {
            if (Enemigos[i] != null)
            {
                Vector3 pos = Enemigos[i].transform.position;
                PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoX_{i}", pos.x);
                PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoY_{i}", pos.y);
                PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoZ_{i}", pos.z);
            }
            else
            {
                PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoX_{i}");
                PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoY_{i}");
                PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoZ_{i}");
            }
        }
        PlayerPrefs.Save();
    }

    void RestaurarPosiciones()
    {
        for (int i = 0; i < CantidadDeEnemigos; i++)
        {
            string keyX = $"{IDSpawner}_EnemigoX_{i}";
            string keyY = $"{IDSpawner}_EnemigoY_{i}";
            string keyZ = $"{IDSpawner}_EnemigoZ_{i}";

            if (PlayerPrefs.HasKey(keyX) && PlayerPrefs.HasKey(keyY) && PlayerPrefs.HasKey(keyZ))
            {
                Vector3 pos = new Vector3(
                    PlayerPrefs.GetFloat(keyX),
                    PlayerPrefs.GetFloat(keyY),
                    PlayerPrefs.GetFloat(keyZ)
                );
                Enemigos[i] = Instantiate(Enemigo, pos, Quaternion.identity, transform.parent.parent);
            }
        }
    }
}
