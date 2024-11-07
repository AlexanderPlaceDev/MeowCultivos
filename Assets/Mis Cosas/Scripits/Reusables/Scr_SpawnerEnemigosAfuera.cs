using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_SpawnerEnemigosAfuera : MonoBehaviour
{
    [SerializeField] string IDSpawner;
    [SerializeField] private GameObject Enemigo;
    [SerializeField] private int CantidadDeEnemigos;
    [SerializeField] private float TiempoSpawn;
    public List<GameObject> Enemigos;

    void Start()
    {
        Enemigos = new List<GameObject>();
        RestaurarPosiciones();
        StartCoroutine(SpawnEnemies());
    }

    void LateUpdate()
    {
        GuardarPosiciones();
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            for (int i = 0; i < CantidadDeEnemigos; i++)
            {
                if (Enemigos.Count <= i || Enemigos[i] == null)
                {
                    GameObject nuevoEnemigo = Instantiate(Enemigo, transform.position, Quaternion.identity, transform.parent.parent);
                    if (Enemigos.Count <= i)
                    {
                        Enemigos.Add(nuevoEnemigo);
                    }
                    else
                    {
                        Enemigos[i] = nuevoEnemigo;
                    }
                    yield return new WaitForSeconds(TiempoSpawn);
                }
            }
            yield return null;
        }
    }

    void GuardarPosiciones()
    {
        for (int i = Enemigos.Count - 1; i >= 0; i--)
        {
            if (Enemigos[i] != null && !Enemigos[i].GetComponent<Scr_CambiadorBatalla>().Cambiando)
            {
                Vector3 pos = Enemigos[i].transform.position;
                PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoX_{i}", pos.x);
                PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoY_{i}", pos.y);
                PlayerPrefs.SetFloat($"{IDSpawner}_EnemigoZ_{i}", pos.z);
            }
            else if (Enemigos[i] != null && Enemigos[i].GetComponent<Scr_CambiadorBatalla>().Cambiando)
            {
                PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoX_{i}");
                PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoY_{i}");
                PlayerPrefs.DeleteKey($"{IDSpawner}_EnemigoZ_{i}");

                Enemigos.RemoveAt(i);  // Eliminar enemigo de la lista
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
                GameObject enemigoRestaurado = Instantiate(Enemigo, pos, Quaternion.identity, transform.parent.parent);
                Enemigos.Add(enemigoRestaurado);
            }
        }
    }
}
