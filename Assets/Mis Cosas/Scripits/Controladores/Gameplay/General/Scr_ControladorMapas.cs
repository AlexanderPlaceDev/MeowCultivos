using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorMapas : MonoBehaviour
{
    [SerializeField] bool EsMapa;
    [SerializeField] GameObject[] MapasQueActiva;
    [SerializeField] GameObject[] MapasQueDesactiva;


    private void Start()
    {
        if (EsMapa)
        {
            // Obtenemos la cantidad de hijos directos del objeto
            int childCount = transform.childCount;

            // Recorremos cada hijo directo
            for (int i = 0; i < childCount; i++)
            {
                // Obtenemos el hijo en la posición i
                Transform child = transform.GetChild(i);
                if (PlayerPrefs.GetString("MapaActivo:" + child.name, "No") == "Si")
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    if (PlayerPrefs.GetString("CinematicaInicial", "No") == "Si")
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (EsMapa)
        {
            // Obtenemos la cantidad de hijos directos del objeto
            int childCount = transform.childCount;

            // Recorremos cada hijo directo
            for (int i = 0; i < childCount; i++)
            {
                // Obtenemos el hijo en la posición i
                Transform child = transform.GetChild(i);

                if (child.gameObject.activeSelf)
                {
                    PlayerPrefs.SetString("MapaActivo:" + child.name, "Si");
                }
                else
                {
                    PlayerPrefs.SetString("MapaActivo:" + child.name, "No");
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gato Mesh")
        {

            foreach (GameObject Mapa in MapasQueActiva)
            {
                Mapa.SetActive(true);
            }
            foreach (GameObject Mapa in MapasQueDesactiva)
            {
                Mapa.SetActive(false);
            }

        }
    }
}
