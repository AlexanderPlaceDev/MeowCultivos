using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Scr_EventosGuardado : MonoBehaviour
{
    GameObject Gata;

    private void Start()
    {
        Gata = GameObject.Find("Gata");
    }

    //Cinematicas
    public void GuardarCinematicaInicial()
    {
        PlayerPrefs.SetString("CinematicaInicial", "Si");
        Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
    }

    //Dialogos
    public void EventoDialogo(int UltimoDialogo, string Personaje)
    {
        switch (Personaje)
        {
            case "Gusano":
                {
                    if(UltimoDialogo == 0)
                    {
                        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
                    }
                    if (UltimoDialogo == 2)
                    {
                        Debug.Log("Se guardo el movimiento");
                        PlayerPrefs.SetString("Movimiento", "Si");
                        GameObject.Find("Canvas").transform.GetChild(2).gameObject.SetActive(true);
                        PlayerPrefs.SetString("Reloj", "Si");
                    }
                    PlayerPrefs.SetInt("DialogoGusano", UltimoDialogo);
                    break;
                }
        }
    }

    public void EventoLineasRadio(string[] Lineas)
    {
        int i = 0;
        foreach (string line in Lineas)
        {
            PlayerPrefs.SetInt("CantLineasRadio", Lineas.Length);
            PlayerPrefs.SetString("Linea" + i + "Radio", Lineas[i]);
            i++;
        }
    }

    //Datos Generales
    public void GuardarPosicion(Transform Trans)
    {
        PlayerPrefs.SetFloat("GataPosX", Trans.position.x);
        PlayerPrefs.SetFloat("GataPosY", Trans.position.y);
        PlayerPrefs.SetFloat("GataPosZ", Trans.position.z);

        PlayerPrefs.SetFloat("GataRotX", Trans.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("GataRotY", Trans.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("GataRotZ", Trans.rotation.eulerAngles.z);
    }

    //Estructuras
    public void GuardarTablero(int EstructuraActual)
    {
        PlayerPrefs.SetInt("EstructuraTablero", EstructuraActual);
    }

    public void GuardarEstructurasTablero(int EstructuraActual)
    {
        PlayerPrefs.SetInt("Estructura"+EstructuraActual, 1);

    }
}
