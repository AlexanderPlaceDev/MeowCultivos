using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorUIBatalla : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] Color[] ColoresBotones;
    [SerializeField] Image[] BotonesMenus;
    [SerializeField] GameObject PanelMenus;
    [Header("Alerta")]
    [SerializeField] GameObject PanelAlerta;
    bool AlertaAdentro = false;
    [Header("Generales")]
    public int BotonActual = -1;

    void Start()
    {
    }

    void Update()
    {
        ActualizarBotonesMenus();
        ActualizarAlerta();
    }

    public void EntraBotonMenu(int Boton)
    {
        BotonActual = Boton;


    }

    public void SaleBotonMenu()
    {
        BotonActual = -1;
    }

    void ActualizarBotonesMenus()
    {
        if (BotonActual != -1)
        {
            PanelMenus.SetActive(true);
            BotonesMenus[BotonActual].color = ColoresBotones[1];

            switch (BotonActual)
            {
                case 0:
                    {
                        PanelMenus.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Habilidades";
                        break;
                    }

                case 1:
                    {
                        PanelMenus.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Skins";
                        break;
                    }

                case 2:
                    {
                        PanelMenus.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Controles";
                        break;
                    }
            }

            for (int i = 0; i < BotonesMenus.Length; i++)
            {
                if (i != BotonActual)
                {
                    BotonesMenus[i].color = ColoresBotones[0];
                }
            }
        }
        else
        {
            PanelMenus.SetActive(false);
            BotonesMenus[0].color = ColoresBotones[0];
            BotonesMenus[1].color = ColoresBotones[0];
            BotonesMenus[2].color = ColoresBotones[0];
        }
    }

    public void EntraAlerta()
    {
        AlertaAdentro = true;
    }

    public void SaleAlerta()
    {
        AlertaAdentro = false;
    }

    void ActualizarAlerta()
    {
        if (AlertaAdentro)
        {
            PanelAlerta.SetActive(true);
        }
        else
        {
            PanelAlerta.SetActive(false);
        }
    }

}
