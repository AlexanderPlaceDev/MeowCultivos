using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_ControladorRevistero : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextoTuDinero;
    [SerializeField] TextMeshProUGUI TextoInserta;
    [SerializeField] TextMeshProUGUI TextoDinero;
    [SerializeField] TextMeshProUGUI TextoDescripcion;
    [SerializeField] GameObject UI;
    [SerializeField] private int[] PrecioDeRangos;
    [SerializeField] GameObject BotonAceptar;

    void Start()
    {
        ActualizarDineroYPrecio();
    }

    void Update()
    {
        if (UI.gameObject.activeSelf)
        {

            if (PlayerPrefs.GetInt("Rango Barra Tecnica5", 0) < 4)
            {
                ActualizarDineroYPrecio();
            }
            else
            {
                TextoDinero.gameObject.SetActive(false);
                TextoInserta.gameObject.SetActive(false);
                TextoDescripcion.text = "Ya no quedan libros utiles";
                BotonAceptar.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                PlayerPrefs.SetInt("Dinero", PlayerPrefs.GetInt("Dinero", 0) + 1000);
            }
        }

    }

    public void CerrarUI()
    {
        UI.SetActive(false);
        GetComponent<Scr_ActivadorMenuEstructuraFijo>().CerrarTablero();
    }

    public void Comprar()
    {
        CerrarUI();
        PlayerPrefs.SetInt("Dinero", PlayerPrefs.GetInt("Dinero", 0) - PrecioDeRangos[PlayerPrefs.GetInt("Rango Barra Tecnica5", 0)]);
        PlayerPrefs.SetInt("Rango Barra Tecnica5", PlayerPrefs.GetInt("Rango Barra Tecnica5", 0) + 1);
        GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_ControladorMenuHabilidades>().ActualizarBarrasPorRango();
    }

    void ActualizarDineroYPrecio()
    {
        TextoTuDinero.text = "$" + PlayerPrefs.GetInt("Dinero", 0);

        if (PlayerPrefs.GetInt("Dinero", 0) >= PrecioDeRangos[PlayerPrefs.GetInt("Rango Barra Tecnica5", 0)]-1)
        {
            BotonAceptar.SetActive(true);
        }
        else
        {
            BotonAceptar.SetActive(false);
        }
        switch (PlayerPrefs.GetInt("Rango Barra Tecnica5", 0))
        {
            case 0:
                {
                    TextoDinero.text = "$" + PrecioDeRangos[0];
                    break;
                }

            case 1:
                {
                    TextoDinero.text = "$" + PrecioDeRangos[1];
                    break;
                }

            case 2:
                {
                    TextoDinero.text = "$" + PrecioDeRangos[2];
                    break;
                }

            case 3:
                {
                    TextoDinero.text = "$" + PrecioDeRangos[3];
                    break;
                }
        }
    }
}
