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
    [SerializeField] TextMeshProUGUI TextoRecompensaDinero;
    [SerializeField] TextMeshProUGUI TextoRecompensaXP;
    [SerializeField] GameObject[] ItemsRecompensa;

    [Header("Sistema de Cantidad De Misiones")]
    [SerializeField] GameObject BotonesMisiones;
    [SerializeField] GameObject PrefabMision;
    [SerializeField] Sprite[] PanelesNombreMisiones;

    private Scr_ControladorMisiones ControladorMisiones;
    private Scr_CreadorMisiones MisionActual;
    void Start()
    {
        Gata = GameObject.Find("Gata").gameObject;
        ControladorMisiones = Gata.transform.GetChild(4).GetComponent<Scr_ControladorMisiones>();

        if (activadorActual != null)
        {
            for (int i = 0; i < activadorActual.MisionesSecundarias.Count; i++)
            {
                Scr_CreadorMisiones mision = activadorActual.MisionesSecundarias[i]; // Guardar referencia local
                GameObject Hijo = Instantiate(PrefabMision, Vector3.zero, Quaternion.identity, BotonesMisiones.transform);
                if (mision.Tipo == Scr_CreadorMisiones.Tipos.Caza)
                {
                    Hijo.GetComponent<Image>().sprite = PanelesNombreMisiones[1];
                }

                // Texto del botón
                Hijo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = mision.TituloMision;

                // Asignar función al botón
                Button boton = Hijo.GetComponent<Button>();
                boton.onClick.AddListener(() => SeleccionarMision(mision));
            }
        }
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
        //Limpiar datos anteriores
        foreach (GameObject item in ItemsNecesarios) item.SetActive(false);
        foreach (GameObject item in ItemsRecompensa) item.SetActive(false);

        //Actualizar UI
        MisionActual = Mision;
        EventSystem.current.SetSelectedGameObject(null);
        TituloMision.text = Mision.TituloMision;
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
            TextoRecompensaDinero.text = "$" + Mision.RecompensaDinero.ToString("N0");
        }
        else
        {
            TextoRecompensaDinero.text = "$0";
        }

        if (Mision.RecompensaXP > 0)
        {
            TextoRecompensaXP.text = Mision.RecompensaXP + "XP";

        }
        else
        {
            TextoRecompensaXP.text = "0XP";
        }

        // Mostrar recompensas de objetos
        for (int i = 0; i < Mision.ObjetosQueDa.Length && i < ItemsRecompensa.Length; i++)
        {
            Scr_CreadorObjetos objeto = Mision.ObjetosQueDa[i];
            GameObject itemUI = ItemsRecompensa[i];

            itemUI.SetActive(true);
            itemUI.transform.GetChild(0).GetComponent<Image>().sprite = objeto.Icono;
            itemUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = objeto.Nombre;
            itemUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mision.CantidadesDa[i].ToString();

        }

    }

    public void AceptarMision()
    {
        bool Encontro = false;
        foreach (Scr_CreadorMisiones MisionSecundaria in ControladorMisiones.MisionesSecundarias)
        {
            if (MisionActual != null && MisionActual.TituloMision == MisionSecundaria.TituloMision)
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
