using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class Scr_EventosGuardado : MonoBehaviour
{
    public void GuardarCinematicaInicial()
    {
        PlayerPrefs.SetString("CinematicaInicial", "Si");
        Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
    }

    public void EventoDialogo(int Dialogo, string Personaje)
    {
        switch (Personaje)
        {
            case "Gusano":
                {
                    if (Dialogo == 1)
                    {
                        PlayerPrefs.SetString("Movimiento", "Si");
                    }
                    if (Dialogo == 2)
                    {
                        PlayerPrefs.SetString("Radio", "Si");
                        GameObject.Find("Radio").GetComponent<Image>().color = Color.white;
                        GameObject.Find("Radio").transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    }
                    PlayerPrefs.SetInt("DialogoGusano", Dialogo);
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

    public void GuardarPosicion(Transform Trans)
    {
        PlayerPrefs.SetFloat("GataPosX", Trans.position.x);
        PlayerPrefs.SetFloat("GataPosY", Trans.position.y);
        PlayerPrefs.SetFloat("GataPosZ", Trans.position.z);

        PlayerPrefs.SetFloat("GataRotX", Trans.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("GataRotY", Trans.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("GataRotZ", Trans.rotation.eulerAngles.z);
    }

    public void GuardarTablero(int Tipo, int EstructuraActual)
    {
        PlayerPrefs.SetInt("TipoTablero", Tipo);
        PlayerPrefs.SetInt("EstructuraTablero", EstructuraActual);
    }

    public void GuardarEstructurasTablero(int Tipo, int EstructuraActual)
    {
        PlayerPrefs.SetString(Tipo.ToString() + EstructuraActual.ToString(), "Si");
        
    }
}
