using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MisionesSecundrias_UI : MonoBehaviour
{
    GameObject Gata;
    [HideInInspector]
    public Scr_ActivadorDialogos activadorActual;

    [Header("Variables de la UI")]
    [SerializeField] TextMeshProUGUI TituloMision;
    [SerializeField] Image LogoMision;
    [SerializeField] TextMeshProUGUI DescripcionMision;
    [SerializeField] GameObject[] ItemsNecesarios;
    [SerializeField] TextMeshProUGUI TextoRecompensa;

    private Scr_ControladorMisiones ControladorMisiones;
    private Scr_CreadorMisiones MisionActual;
    void Start()
    {
        Gata = GameObject.Find("Gata").gameObject;
        ControladorMisiones = Gata.transform.GetChild(4).GetComponent<Scr_ControladorMisiones>();
    }

    public void cerrar()
    {
        if (!gameObject.activeSelf) return; // 🔥 Evita doble cierre

        if (activadorActual != null)
        {
            activadorActual.ViendoMisiones = false;
            activadorActual.MostrarIconos();
            activadorActual.DesactivarDialogo();
            activadorActual = null;
        }

        EventSystem.current.SetSelectedGameObject(null);
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        gameObject.SetActive(false);
    }


    public void SeleccionarMision(Scr_CreadorMisiones Mision)
    {
        MisionActual = Mision;
        EventSystem.current.SetSelectedGameObject(null);
        TituloMision.text = Mision.MisionName;
        LogoMision.sprite = Mision.LogoMision;
        DescripcionMision.text = Mision.Descripcion;
        int c = 0;

        switch (Mision.Tipo)
        {
            case Scr_CreadorMisiones.Tipos.Recoleccion:
                {
                    foreach (Scr_CreadorObjetos Objeto in Mision.ObjetosNecesarios)
                    {
                        ItemsNecesarios[c].SetActive(true);
                        ItemsNecesarios[c].transform.GetChild(0).GetComponent<Image>().sprite = Objeto.Icono;
                        ItemsNecesarios[c].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Objeto.Nombre;
                        ItemsNecesarios[c].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mision.CantidadesQuita[c].ToString();
                        c++;
                    }
                    break;
                }

            case Scr_CreadorMisiones.Tipos.Caza:
                {
                    foreach (string Enemigo in Mision.ObjetivosACazar)
                    {
                        ItemsNecesarios[c].SetActive(true);
                        ItemsNecesarios[c].transform.GetChild(0).GetComponent<Image>().sprite = Mision.IconosACazar[c];
                        ItemsNecesarios[c].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Enemigo;
                        ItemsNecesarios[c].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mision.CantidadACazar[c].ToString();
                        c++;
                    }
                    break;
                }
        }

        if (Mision.RecompensaDinero > 0)
        {
            TextoRecompensa.text = "$" + Mision.RecompensaDinero;

        }
        else
        {
            TextoRecompensa.text = Mision.RecompensaXP + " XP";
        }
    }

    public void AceptarMision()
    {
        bool Encontro = false;
        foreach (Scr_CreadorMisiones MisionSecundaria in ControladorMisiones.MisionesSecundarias)
        {
            if (MisionActual != null && MisionActual.MisionName == MisionSecundaria.MisionName)
            {
                Encontro = true;
                break;
            }
        }
        if (!Encontro)
        {
            ControladorMisiones.MisionesSecundarias.Add(MisionActual);
            ControladorMisiones.MisionesScompletas.Add(false);
            ControladorMisiones.MisionActual = MisionActual;
            if (MisionActual.Tipo == Scr_CreadorMisiones.Tipos.Caza) { ControladorMisiones.CantidadCazados.Add(0); }
        }

    }
}
