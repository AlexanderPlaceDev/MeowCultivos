using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Scr_ControladorCajaVenta : MonoBehaviour
{
    // =========================
    // REFERENCIAS
    // =========================

    Scr_Inventario Inventario;

    [SerializeField] GameObject Items;
    [SerializeField] GameObject PrefabItem;

    [SerializeField] TextMeshProUGUI CantidadCajas;
    [SerializeField] TextMeshProUGUI TextoCajasVendidas;

    [SerializeField] GameObject MarcoVenta;

    [SerializeField] Scrollbar Scroll;

    [SerializeField] GameObject ObjetoFlechaIzquierda;
    [SerializeField] GameObject ObjetoFlechaDerecha;

    [SerializeField] Image Icono;
    [SerializeField] Sprite IconoCaja;
    [SerializeField] Sprite IconoRegalo;

    // =========================
    // ESTADO
    // =========================

    Scr_CreadorObjetos ObjetoSeleccionado;
    int NumeroObjetoSeleccionado = -1;

    int CajasReservadas = 0;

    // =========================
    // UNITY
    // =========================

    void Awake()
    {
        Inventario = GameObject.Find("Gata")
            .transform.GetChild(7)
            .GetComponent<Scr_Inventario>();
    }

    void OnEnable()
    {
        RefrescarItems();
    }

    void OnDisable()
    {
        LimpiarItems();
    }

    // =========================
    // SELECCIÓN DE ITEM
    // =========================

    public void SeleccionarItem(Sprite spriteSeleccionado)
    {
        ResetearMarco();

        for (int i = 0; i < Inventario.Objetos.Length; i++)
        {
            if (Inventario.Objetos[i].IconoInventario == spriteSeleccionado)
            {
                ObjetoSeleccionado = Inventario.Objetos[i];
                NumeroObjetoSeleccionado = i;

                MarcoVenta.transform.GetChild(0).gameObject.SetActive(true);
                MarcoVenta.transform.GetChild(0).GetComponent<Image>().sprite =
                    ObjetoSeleccionado.IconoInventario;

                CajasReservadas = 1;
                CantidadCajas.text = "1";

                ActualizarTextoCantidad();
                ActualizarFlechas();
                return;
            }
        }
    }

    // =========================
    // FLECHAS
    // =========================

    public void FlechaIzquierda()
    {
        if (CajasReservadas <= 1)
            return;

        CajasReservadas--;
        CantidadCajas.text = CajasReservadas.ToString();

        ActualizarTextoCantidad();
        ActualizarFlechas();
    }

    public void FlechaDerecha()
    {
        int maxCajas = Inventario.Cantidades[NumeroObjetoSeleccionado]
            / ObjetoSeleccionado.CantidadVentaMinima;

        if (CajasReservadas >= maxCajas)
            return;

        CajasReservadas++;
        CantidadCajas.text = CajasReservadas.ToString();

        ActualizarTextoCantidad();
        ActualizarFlechas();
    }

    void ActualizarFlechas()
    {
        if (ObjetoSeleccionado == null)
        {
            ObjetoFlechaIzquierda.SetActive(false);
            ObjetoFlechaDerecha.SetActive(false);
            return;
        }

        int maxCajas = Inventario.Cantidades[NumeroObjetoSeleccionado]
            / ObjetoSeleccionado.CantidadVentaMinima;

        ObjetoFlechaIzquierda.SetActive(CajasReservadas > 1);
        ObjetoFlechaDerecha.SetActive(CajasReservadas < maxCajas);
    }

    // =========================
    // BOTONES DIRECTOS
    // =========================

    public void BotonMitad()
    {
        if (ObjetoSeleccionado == null)
            return;

        int maxCajas = Inventario.Cantidades[NumeroObjetoSeleccionado]
            / ObjetoSeleccionado.CantidadVentaMinima;

        CajasReservadas = Mathf.Max(1, maxCajas / 2);

        CantidadCajas.text = CajasReservadas.ToString();
        ActualizarTextoCantidad();
        ActualizarFlechas();
    }

    public void BotonMaximo()
    {
        if (ObjetoSeleccionado == null)
            return;

        CajasReservadas = Inventario.Cantidades[NumeroObjetoSeleccionado]
            / ObjetoSeleccionado.CantidadVentaMinima;

        CantidadCajas.text = CajasReservadas.ToString();
        ActualizarTextoCantidad();
        ActualizarFlechas();
    }

    // =========================
    // EMPACAR / VENDER
    // =========================

    public void BotonEmpacar()
    {
        if (ObjetoSeleccionado == null || CajasReservadas <= 0)
            return;

        int totalUnidades =
            CajasReservadas * ObjetoSeleccionado.CantidadVentaMinima;

        // Quitar del inventario REAL
        Inventario.Cantidades[NumeroObjetoSeleccionado] -= totalUnidades;

        // Dinero
        GameObject.Find("Controlador Tiempo")
            .GetComponent<Scr_ControladorTiempo>()
            .DineroRecompensa +=
            totalUnidades * ObjetoSeleccionado.ValorVentaIndividual;

        ResetearMarco();
        RefrescarItems();
    }

    // =========================
    // UI AUXILIAR
    // =========================

    void ActualizarTextoCantidad()
    {
        MarcoVenta.transform.GetChild(2)
            .GetComponent<TextMeshProUGUI>()
            .text = (CajasReservadas *
            ObjetoSeleccionado.CantidadVentaMinima).ToString();
    }

    void ResetearMarco()
    {
        ObjetoSeleccionado = null;
        NumeroObjetoSeleccionado = -1;
        CajasReservadas = 0;

        CantidadCajas.text = "0";
        MarcoVenta.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "0";

        MarcoVenta.transform.GetChild(0).gameObject.SetActive(false);
        MarcoVenta.transform.GetChild(0).GetComponent<Image>().sprite = null;

        ObjetoFlechaIzquierda.SetActive(false);
        ObjetoFlechaDerecha.SetActive(false);
    }

    // =========================
    // ITEMS
    // =========================

    void RefrescarItems()
    {
        LimpiarItems();

        for (int i = 0; i < Inventario.Objetos.Length; i++)
        {
            if (Inventario.Cantidades[i] >=
                Inventario.Objetos[i].CantidadVentaMinima)
            {
                GameObject hijo =
                    Instantiate(PrefabItem, Items.transform);

                hijo.GetComponent<Image>().sprite =
                    Inventario.Objetos[i].IconoInventario;

                hijo.transform.GetChild(0)
                    .GetComponent<TextMeshProUGUI>()
                    .text = Inventario.Cantidades[i].ToString();

                hijo.transform.GetChild(1)
                    .GetComponent<TextMeshProUGUI>()
                    .text = Inventario.Objetos[i].Nombre;

                int index = i;

                EventTrigger trigger = hijo.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerDown
                };

                entry.callback.AddListener(_ =>
                {
                    SeleccionarItem(
                        Inventario.Objetos[index].IconoInventario);
                });

                trigger.triggers.Add(entry);
            }
        }
    }

    void LimpiarItems()
    {
        foreach (Transform hijo in Items.transform)
            Destroy(hijo.gameObject);
    }
}
