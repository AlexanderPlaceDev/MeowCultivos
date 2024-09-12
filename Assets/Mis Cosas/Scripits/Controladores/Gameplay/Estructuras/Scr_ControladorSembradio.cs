using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorSembradio : MonoBehaviour
{
    [SerializeField] int ID;
    [SerializeField] Image[] Iconos;
    [SerializeField] Sprite[] Sprites;
    [SerializeField] GameObject[] Botones;
    [SerializeField] Color ColorBoton;
    [SerializeField] bool Regado;
    [SerializeField] bool Abonado;
    [SerializeField] Scr_SpawnerRecolectable Barril;
    Scr_CreadorObjetos Fruta;

    void Start()
    {

        if (PlayerPrefs.GetString("SembradioRegado:" + ID, "No") == "Si")
        {
            Regado = true;
        }


        if (Regado)
        {
            Iconos[0].sprite = Sprites[1];
            Botones[0].SetActive(false);
        }
        if (Abonado)
        {
            Iconos[1].sprite = Sprites[1];
            Botones[1].SetActive(false);
            Botones[2].SetActive(false);
        }
        if (!GetComponent<Scr_ActivadorMenuEstructuraFijo>().EstaDentro)
        {
            Botones[0].SetActive(false);
            Botones[1].SetActive(false);
            Botones[2].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Scr_ActivadorMenuEstructuraFijo>().EstaDentro)
        {
            if (!Regado && !Botones[0].activeSelf)
            {
                Botones[0].SetActive(true);
            }
            if (!Abonado && !Botones[1].activeSelf)
            {
                Botones[1].SetActive(true);
                Botones[2].SetActive(true);
            }

        }
        else
        {
            if (Botones[0].activeSelf || Botones[1].activeSelf || Botones[2].activeSelf)
            {
                Botones[0].SetActive(false);
                Botones[1].SetActive(false);
                Botones[2].SetActive(false);
            }

        }

    }

    public void BotonRegar()
    {
        if (PlayerPrefs.GetInt("CantidadAgua", 0) > 2)
        {
            Regado = true;
            Botones[0].SetActive(false);
            Iconos[0].sprite = Sprites[1];
            PlayerPrefs.SetInt("CantidadAgua", PlayerPrefs.GetInt("CantidadAgua", 0) - 2);
            PlayerPrefs.SetString("SembradioRegado:" + ID, "Si");
        }
    }

    public void EntraBoton(string ID)
    {
        // Convertir el segundo carácter a un número entero
        int index = (int)char.GetNumericValue(ID[1]);

        if (ID[0] == '1')
        {
            Botones[index].GetComponent<Image>().color = Color.white;
            Botones[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            Botones[index].GetComponent<Image>().color = ColorBoton;
            Botones[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = ColorBoton;
        }
    }
}
