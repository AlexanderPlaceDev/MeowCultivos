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
    public List<Scr_CreadorDialogos> Misiones;
    GameObject Gata;
    [HideInInspector]
    public Scr_ActivadorDialogos activadorActual;

    [Header("Variables de la UI")]
    [SerializeField] TextMeshProUGUI TituloMision;
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
        // 🔥 Quitar selección activa del EventSystem
        EventSystem.current.SetSelectedGameObject(null);
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        activadorActual.ViendoMisiones = false;
        if (activadorActual != null)
        {
            activadorActual.MostrarIconos(); // Opcional: reactivar iconos del NPC
            activadorActual = null; // Limpiar referencia
        }

        gameObject.SetActive(false);
    }

    public void SeleccionarMision(Scr_CreadorMisiones Mision)
    {
        MisionActual = Mision;
        EventSystem.current.SetSelectedGameObject(null);
        TituloMision.text = Mision.MisionName;
        DescripcionMision.text = Mision.Descripcion;
        int c = 0;
        foreach (Scr_CreadorObjetos Objeto in Mision.ObjetosNecesarios)
        {
            ItemsNecesarios[c].SetActive(true);
            ItemsNecesarios[c].transform.GetChild(0).GetComponent<Image>().sprite = Objeto.Icono;
            ItemsNecesarios[c].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Objeto.Nombre;
            ItemsNecesarios[c].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mision.CantidadesQuita[c].ToString();
            c++;
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
        if(!Encontro)
        {
            ControladorMisiones.MisionesSecundarias.Add(MisionActual);
            ControladorMisiones.MisionActual = MisionActual;
        }

    }
}
