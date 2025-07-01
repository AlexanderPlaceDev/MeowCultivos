using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorMapas : MonoBehaviour
{
    [SerializeField] public bool EsMapa;
    [SerializeField] GameObject[] MapasQueActiva;
    [SerializeField] GameObject[] MapasQueDesactiva;
    [SerializeField] string NombreMapaBatalla;
    [SerializeField] Material SkyBoxDia;
    [SerializeField] Material SkyBoxNoche;

    public void Start()
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
                    child.gameObject.SetActive(false);
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
        if (other.name == "Gata")
        {
            Scr_DatosSingletonBatalla Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
            Singleton.NombreMapa = NombreMapaBatalla;
            Singleton.SkyBoxDia = SkyBoxDia;
            Singleton.SkyBoxNoche = SkyBoxNoche;
            PlayerPrefs.SetString("Mapa Actual", NombreMapaBatalla);
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

    public void ActualizarMapas()
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
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}
