using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sembradios : MonoBehaviour
{
    public GameObject[] SembradiosObj;

    public int SembradiosCount=0; //cuantos sembradios hay activos
    // Start is called before the first frame update
    void Start()
    {
        checarSembradioos();
    }


    public void checarSembradioos()
    {
        int sem = 0;
        for (int i = 0; i < SembradiosObj.Length; i++) 
        {
            if (PlayerPrefs.GetString(SembradiosObj[i].name + i, "NO") == "SI")
            {
                SembradiosObj[i].SetActive(true);
                SembradiosCount++;
                sem++;
            }
        }
        if(sem== 0)
        {
            sembradioInicial();
        }
    }

    public void sembradioInicial()
    {
        SembradiosObj[Random.Range(0, SembradiosObj.Length - 1)].SetActive(true);
        guardarSembradioos();
    }

    public void sembradioNuevo()
    {
        for (int i = 0; i < SembradiosObj.Length; i++)
        {
            int nuevo = Random.Range(0, SembradiosObj.Length - 1);
            if (SembradiosObj[nuevo].activeSelf)
            {
                SembradiosObj[nuevo].SetActive(true);
                guardarSembradioos();
                break;
            }
        }
    }

    public void guardarSembradioos()
    {
        SembradiosCount=0;
        for (int i = 0; i < SembradiosObj.Length; i++)
        {
            if (SembradiosObj[i].activeSelf)
            {
                PlayerPrefs.SetString(SembradiosObj[i].name + i, "SI");
                SembradiosCount++;
            }
            else
            {
                PlayerPrefs.SetString(SembradiosObj[i].name + i, "NO");
            }
        }
    }
}
