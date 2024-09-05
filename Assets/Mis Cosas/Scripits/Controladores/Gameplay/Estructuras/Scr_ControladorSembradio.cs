using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorSembradio : MonoBehaviour
{
    [SerializeField] Image[] Iconos;
    [SerializeField] Sprite[] Sprites;
    [SerializeField] GameObject[] Botones;
    [SerializeField] bool Regado;
    [SerializeField] bool Abonado;
    [SerializeField] Scr_SpawnerRecolectable Barril;
    Scr_CreadorObjetos Fruta;

    void Start()
    {
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
            if(Botones[0].activeSelf || Botones[1].activeSelf || Botones[2].activeSelf)
            {
                Botones[0].SetActive(false);
                Botones[1].SetActive(false);
                Botones[2].SetActive(false);
            }
                
        }

    }
}
