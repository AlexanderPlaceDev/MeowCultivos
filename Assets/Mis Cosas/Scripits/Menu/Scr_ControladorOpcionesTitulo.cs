using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorOpcionesTitulo : MonoBehaviour
{

    [SerializeField] GameObject[] Iconos;
    int IconoSeleccionado;

    void Start()
    {
        
    }

    void Update()
    {
        switch (IconoSeleccionado)
        {
            case 0:
                {
                    foreach(GameObject Icono in Iconos)
                    {
                        Icono.transform.GetChild(0).gameObject.SetActive(false);
                        Icono.GetComponent<Image>().color = Color.black;
                    }
                    break;
                }
            case 1:
                {
                    Iconos[0].GetComponent<Image>().color = Color.red;
                    Iconos[0].transform.GetChild(0).gameObject.SetActive(true);
                    break;
                }
            case 2:
                {
                    Iconos[1].GetComponent<Image>().color = Color.red;
                    Iconos[1].transform.GetChild(0).gameObject.SetActive(true);
                    break;
                }
            case 3:
                {
                    Iconos[2].GetComponent<Image>().color = Color.red;
                    Iconos[2].transform.GetChild(0).gameObject.SetActive(true);
                    break;
                }
            case 4:
                {
                    Iconos[3].GetComponent<Image>().color = Color.red;
                    Iconos[3].transform.GetChild(0).gameObject.SetActive(true);
                    break;
                }
        }
        
    }

    public void SeleccionarIcono(int NumeroIcono)
    {
        IconoSeleccionado = NumeroIcono;
    }

    public void DeseleccionarIcono()
    {
        IconoSeleccionado = 0;
    }

}
