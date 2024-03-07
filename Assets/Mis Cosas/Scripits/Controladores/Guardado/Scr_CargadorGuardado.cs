using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class Scr_CargadorGuardado : MonoBehaviour
{
    [Header("Objetos con datos a guardar")]
    [SerializeField] GameObject CinematicaInicial;
    [SerializeField] GameObject Gata;
    [SerializeField] GameObject Camara360;
    [SerializeField] GameObject GusanoTutotial;
    [SerializeField] GameObject Radio;
    [SerializeField] GameObject Tablero;

    [SerializeField] bool Moviendo;

    GameObject camara;

    private void Awake()
    {
        //Animacion autobus
        camara = Camera.main.gameObject;
        if (PlayerPrefs.GetString("CinematicaInicial", "No") == "Si")
        {
            CinematicaInicial.GetComponent<PlayableDirector>().enabled = false;
            camara.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        }
        //Posicion y Rotacion
        if (PlayerPrefs.GetInt("DialogoGusano", 0) > 0)
        {
            Gata.transform.position = new Vector3(PlayerPrefs.GetFloat("GataPosX", 62), PlayerPrefs.GetFloat("GataPosY", 7), PlayerPrefs.GetFloat("GataPosZ", 103.5f));
            Gata.transform.rotation = Quaternion.Euler(PlayerPrefs.GetFloat("GataRotX", 0), PlayerPrefs.GetFloat("GataRotY", -67), PlayerPrefs.GetFloat("GataRotZ", 0));
            Camara360.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x = 0;
        }
        //Movimiento
        if (PlayerPrefs.GetString("Movimiento", "No") == "Si")
        {
            Gata.GetComponent<Scr_Movimiento>().enabled = true;
            Gata.GetComponent<Scr_GiroGata>().enabled = true;
        }
        //Inventario
        Image[] Casillas = Gata.transform.GetChild(3).GetComponent<Scr_ControladorInventario>().Casillas;
        for (int i = 0; i < Gata.transform.GetChild(3).GetComponent<Scr_ControladorInventario>().CasillasContenido.Length; i++)
        {
            Gata.transform.GetChild(3).GetComponent<Scr_ControladorInventario>().CasillasContenido[i] = PlayerPrefs.GetString("Casilla" + i, "");
            Gata.transform.GetChild(3).GetComponent<Scr_ControladorInventario>().Cantidades[i] = PlayerPrefs.GetInt("CasillaCantidad" + i, 0);
            if (Gata.transform.GetChild(3).GetComponent<Scr_ControladorInventario>().CasillasContenido[i] != "")
            {
                if (Casillas[i].GetComponent<Scr_CasillaInventario>().FormaConHermanas.Length == 0)
                {
                    Casillas[i].GetComponent<Scr_CasillaInventario>().FormaConHermanas = new bool[18];
                }

                Casillas[i].GetComponent<Scr_CasillaInventario>().PuedeAgarrar = true;
                for (int j = 0; j < PlayerPrefs.GetInt("CasillasHermanasCantidad" + i, 0); j++)
                {
                    for (int l = 0; l < Casillas.Length; l++)
                    {
                        if (Casillas[l].gameObject.name == PlayerPrefs.GetString("CasillasHermanas" + i + "Hemana" + j, ""))
                        {
                            Casillas[i].GetComponent<Scr_CasillaInventario>().CasillasHermanas.Add(Casillas[l]);

                        }

                    }
                }
                for (int j = 0; j < 18; j++)
                {
                    if (PlayerPrefs.GetString("CasillasFormaHermanas" + i + j, "No") == "Si")
                    {
                        Casillas[i].GetComponent<Scr_CasillaInventario>().FormaConHermanas[j] = true;

                    }
                }
            }



        }
        //Radio
        if (PlayerPrefs.GetString("Radio", "No") == "Si")
        {
            Radio.GetComponent<Image>().color = Color.white;
            GameObject.Find("Radio").transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        Radio.GetComponent<Scr_Radio>().Lineas = new string[PlayerPrefs.GetInt("CantLineasRadio", 0)];
        for (int i = 0; i < PlayerPrefs.GetInt("CantLineasRadio", 0); i++)
        {
            Radio.GetComponent<Scr_Radio>().Lineas[i] = PlayerPrefs.GetString("Linea" + i + "Radio", "");
        }
        //Dialogos
        GusanoTutotial.GetComponent<Scr_ControladorDialogos>().DialogoActual = PlayerPrefs.GetInt("DialogoGusano", 0);
        //Tablero
        Tablero.GetComponent<Scr_MenuTablero>().TipoActual = PlayerPrefs.GetInt("TipoTablero", 1);
        Tablero.GetComponent<Scr_MenuTablero>().EstructuraActual = PlayerPrefs.GetInt("EstructuraTablero", 0);
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                for (int i2 = 0; i2 < Tablero.GetComponent<Scr_MenuTablero>().EstructurasIndustrialesGuardadas.Length; i2++)
                {
                    if (PlayerPrefs.GetString(i.ToString() + i2.ToString(), "No") == "Si")
                    {
                        Tablero.GetComponent<Scr_MenuTablero>().EstructurasIndustrialesGuardadas[i2] = true;
                    }
                }
            }
            if (i == 1)
            {
                for (int i2 = 0; i2 < Tablero.GetComponent<Scr_MenuTablero>().EstructurasGranjaGuardadas.Length; i2++)
                {
                    if (PlayerPrefs.GetString(i.ToString() + i2.ToString(), "No") == "Si")
                    {
                        Tablero.GetComponent<Scr_MenuTablero>().EstructurasGranjaGuardadas[i2] = true;
                    }
                }
            }
        }

    }

    private void Update()
    {
        if (Moviendo)
        {
            Gata.transform.position = new Vector3(65, 7, 122);
        }
    }
}
