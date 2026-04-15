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
    // ================================
    // REFERENCIAS
    // ================================
    GameObject Gata;

    [HideInInspector]
    public Scr_ActivadorDialogos activadorActual;

    private Scr_ControladorMisiones ControladorMisiones;
    private Scr_CreadorMisiones MisionActual;
    private ChecarInput checarInput;

    // ================================
    // UI PRINCIPAL
    // ================================
    [Header("UI Misión")]
    [SerializeField] TextMeshProUGUI TituloMision;
    [SerializeField] Image LogoMision;
    [SerializeField] TextMeshProUGUI DescripcionMision;

    [Header("Items necesarios")]
    [SerializeField] GameObject[] ItemsNecesarios;

    [Header("Recompensas")]
    [SerializeField] TextMeshProUGUI TextoRecompensaDinero;
    [SerializeField] TextMeshProUGUI TextoRecompensaXP;
    [SerializeField] GameObject[] ItemsRecompensa;

    // ================================
    // LISTA DE MISIONES
    // ================================
    [Header("Botones de misiones")]
    [SerializeField] GameObject BotonesMisiones;
    [SerializeField] GameObject PrefabMision;
    [SerializeField] Sprite[] PanelesNombreMisiones;

    // ================================
    // INICIALIZACIÓN
    // ================================
    void Start()
    {
        Gata = GameObject.Find("Gata").gameObject;

        ControladorMisiones = Gata.transform
            .GetChild(4)
            .GetComponent<Scr_ControladorMisiones>();

        checarInput = GameObject.Find("Singleton").GetComponent<ChecarInput>();

        // Crear botones dinámicamente
        if (activadorActual != null)
        {
            for (int i = 0; i < activadorActual.MisionesSecundarias.Count; i++)
            {
                Scr_CreadorMisiones mision = activadorActual.MisionesSecundarias[i];

                GameObject Hijo = Instantiate(
                    PrefabMision,
                    Vector3.zero,
                    Quaternion.identity,
                    BotonesMisiones.transform
                );

                // Cambiar sprite según tipo
                if (mision.Tipo == Scr_CreadorMisiones.Tipos.Caza)
                    Hijo.GetComponent<Image>().sprite = PanelesNombreMisiones[1];

                // Texto
                Hijo.transform.GetChild(0)
                    .GetComponent<TextMeshProUGUI>().text = mision.TituloMision;

                // Evento botón
                Button boton = Hijo.GetComponent<Button>();
                boton.onClick.AddListener(() => SeleccionarMision(mision));
            }
        }
    }

    // ================================
    // CERRAR UI
    // ================================
    public void cerrar()
    {
        if (!gameObject.activeSelf) return;

        if (activadorActual != null)
        {
            activadorActual.ViendoMisiones = false;
            activadorActual.MostrarIconos();
            activadorActual.DesactivarDialogo();
            activadorActual = null;
        }

        EventSystem.current.SetSelectedGameObject(null);
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;

        checarInput.CammbiarAction_Player();
        gameObject.SetActive(false);
    }

    // ================================
    // SELECCIONAR MISIÓN
    // ================================
    public void SeleccionarMision(Scr_CreadorMisiones Mision)
    {
        // Limpiar UI previa
        foreach (GameObject item in ItemsNecesarios) item.SetActive(false);
        foreach (GameObject item in ItemsRecompensa) item.SetActive(false);

        // Asignar misión actual
        MisionActual = Mision;

        EventSystem.current.SetSelectedGameObject(null);

        // Datos básicos
        TituloMision.text = Mision.TituloMision;
        LogoMision.sprite = Mision.LogoMision;
        DescripcionMision.text = Mision.DescripcionEnMision;

        int c = 0;

        // ============================
        // OBJETIVOS
        // ============================
        switch (Mision.Tipo)
        {
            case Scr_CreadorMisiones.Tipos.Recoleccion:
                foreach (var Objeto in Mision.ObjetosNecesarios)
                {
                    ItemsNecesarios[c].SetActive(true);
                    ItemsNecesarios[c].transform.GetChild(0).GetComponent<Image>().sprite = Objeto.Icono;
                    ItemsNecesarios[c].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Objeto.Nombre;
                    ItemsNecesarios[c].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mision.CantidadesQuita[c].ToString();
                    c++;
                }
                break;

            case Scr_CreadorMisiones.Tipos.Caza:
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

        // ============================
        // RECOMPENSAS
        // ============================
        TextoRecompensaDinero.text = "$" + Mision.RecompensaDinero.ToString("N0");
        TextoRecompensaXP.text = Mision.RecompensaXP + "XP";

        for (int i = 0; i < Mision.ObjetosQueDa.Length && i < ItemsRecompensa.Length; i++)
        {
            var objeto = Mision.ObjetosQueDa[i];
            GameObject itemUI = ItemsRecompensa[i];

            itemUI.SetActive(true);
            itemUI.transform.GetChild(0).GetComponent<Image>().sprite = objeto.Icono;
            itemUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = objeto.Nombre;
            itemUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mision.CantidadesQueDa[i].ToString();
        }
    }

    // ===============
    // ACEPTAR MISIÓN
    // ===============
    public void AceptarMision()
    {
        bool Encontro = false;

        // Verificar duplicados
        foreach (var MisionSecundaria in ControladorMisiones.Misiones)
        {
            if (MisionActual != null &&
                MisionActual.TituloMision == MisionSecundaria.TituloMision)
            {
                Encontro = true;
                break;
            }
        }

        if (!Encontro)
        {
            // Agregar misión
            ControladorMisiones.Misiones.Add(MisionActual);
            ControladorMisiones.MisionesCompletas.Add(false);
            ControladorMisiones.MisionesVistas.Add(false);

            // 🔥 FIX CLAVE: actualizar página correctamente
            ControladorMisiones.PaginaActual =
                ControladorMisiones.Misiones.Count - 1;

            // Asignar misión actual
            ControladorMisiones.MisionActual = MisionActual;

            // Inicializar datos de caza si aplica
            if (MisionActual.Tipo == Scr_CreadorMisiones.Tipos.Caza)
                ControladorMisiones.CantidadCazados.Add(0);
        }

        ControladorMisiones.GuardarMisiones();
        ControladorMisiones.ActualizarUI();
    }
}
