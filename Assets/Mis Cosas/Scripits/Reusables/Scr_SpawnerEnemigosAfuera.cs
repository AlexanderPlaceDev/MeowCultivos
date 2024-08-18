using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_SpawnerEnemigosAfuera : MonoBehaviour
{
    [SerializeField] private GameObject Enemigo;
    [SerializeField] private int CantidadDeEnemigos;
    [SerializeField] private float TiempoSpawn; // Nueva variable para el tiempo de espera

    private GameObject[] Enemigos;

    void Start()
    {
        Enemigos = new GameObject[CantidadDeEnemigos];
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true) // Bucle infinito para que siempre esté revisando
        {
            for (int i = 0; i < CantidadDeEnemigos; i++)
            {
                if (Enemigos[i] == null)
                {
                    Enemigos[i] = Instantiate(Enemigo, transform.position, Quaternion.identity, transform.parent.parent);
                    yield return new WaitForSeconds(TiempoSpawn); // Espera el tiempo especificado antes de spawnear el siguiente enemigo
                }
            }

            // Espera un momento antes de revisar nuevamente para evitar un bucle de comprobación constante sin pausa
            yield return null; // Esto hace que el bucle se ejecute en cada frame, pero sin bloquear el juego
        }
    }
}
