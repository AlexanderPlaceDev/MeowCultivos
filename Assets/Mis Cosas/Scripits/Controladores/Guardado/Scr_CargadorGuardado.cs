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
        else
        {
            if(CinematicaInicial.GetComponent<PlayableDirector>().enabled == false)
            {
                camara.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            }
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
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        }
        //Inventario
        
        //Dialogos
        //GusanoTutotial.GetComponent<Scr_ControladorDialogos>().DialogoActual = PlayerPrefs.GetInt("DialogoGusano", 0);
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
