using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using UnityEngine.UI;
using Cinemachine;

public class Scr_ControladorUI : MonoBehaviour
{
    [Header("Inventario")]
    [SerializeField] GameObject Mochila;
    [SerializeField] GameObject UIMochila;
    [SerializeField] float DuracionMochila;
    [SerializeField] Material MaterialMochila;
    public bool MochilaActiva = false;
    public bool PuedeAbrirMochila=true;

    [Header("Info")]
    [SerializeField] GameObject Radio;

    [Header("Dialogos")]
    [SerializeField] GameObject PanelDialogos;

    [Header("Menu Pausa")]
    [SerializeField] GameObject MenuPausa;
    [SerializeField] GameObject Camara;
    Transform PosCamara;
    bool Pausado=false;

    private void Update()
    {
        if (!Pausado && PuedeAbrirMochila)
        {
            OcultarIconosDialogo();
            AbrirMochila();
        }
        Pausar();
        
    }

    void OcultarIconosDialogo()
    {
        if (PanelDialogos.activeSelf)
        {
            UIMochila.SetActive(false);
            Radio.SetActive(false);
        }
        else
        {
            UIMochila.SetActive(true);
            Radio.SetActive(true);
        }

    }
    void AbrirMochila()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (MochilaActiva)
            {
                Tween.PositionY(Mochila.transform, -1070, DuracionMochila / 2);
                Tween.UIAnchoredPosition3DY(UIMochila.GetComponent<RectTransform>(), 20, DuracionMochila);
                Tween.UIAnchoredPosition3DY(Radio.GetComponent<RectTransform>(), -20, DuracionMochila);
                Tween.MaterialColor(MaterialMochila, new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 0), DuracionMochila);
                MochilaActiva = false;
            }
            else
            {
                Tween.PositionY(Mochila.transform, 0, DuracionMochila);
                Tween.UIAnchoredPosition3DY(UIMochila.GetComponent<RectTransform>(), -120, DuracionMochila / 2);
                Tween.UIAnchoredPosition3DY(Radio.GetComponent<RectTransform>(), 137, DuracionMochila / 2);
                Tween.MaterialColor(MaterialMochila, new Color32(255, 255, 255, 0), new Color32(255, 255, 255, 255), DuracionMochila);
                MochilaActiva = true;
            }
        }

    }

    void Pausar()
    {

        if (Time.timeScale == 1)
        {
            MenuPausa.SetActive(false);
            Pausado = false;
        }


        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (MenuPausa.activeSelf)
            {
                MenuPausa.SetActive(false);
                Pausado = false;
                Time.timeScale = 1;
            }
            else
            {
                MenuPausa.SetActive(true);
                Pausado=true;
                Time.timeScale = 0;
            }
        }
    }

}



