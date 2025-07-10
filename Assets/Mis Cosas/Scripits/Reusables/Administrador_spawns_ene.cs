using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Administrador_spawns_ene : MonoBehaviour
{
    [SerializeField] private Scr_SpawnerEnemigosAfuera[] spawmsEnemies;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnall()
    {
        for (int i = 0; i < spawmsEnemies.Length; i++)
        {
            spawmsEnemies[i].haveAcivate = 1;
        }
    }

    public void spawnselec(int i)
    {
        spawmsEnemies[i].haveAcivate = 1;
    }
}
