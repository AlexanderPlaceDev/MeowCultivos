using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Scr_ControladorCajaVenta : MonoBehaviour
{
    // =========================
    // PLAYER PREFS
    // =========================
    const string PREF_CAJAS = "CajasVendidas";
    const string PREF_REGALO = "CajaVentaRegalo";

    // =========================
    // CONFIGURACIÓN
    // =========================
    [Header("Configuración de Ventas")]
    [SerializeField] int VentasParaRegalo = 100;   // ← CAMBIA AQUÍ
    [SerializeField] int TotalCajas3D = 12;
    [SerializeField] private Scr_ObjetosAgregados ObjetosAgregados;


    // =========================
    // REFERENCIAS
    // =========================
    Scr_Inventario Inventario;

    [Header("Items")]
    [SerializeField] GameObject Items;
    [SerializeField] GameObject PrefabItem;

    [Header("Venta UI")]
    [SerializeField] TextMeshProUGUI CantidadCajas;
    [SerializeField] TextMeshProUGUI TextoCajasVendidas;
    [SerializeField] GameObject MarcoVenta;

    [Header("Flechas")]
    [SerializeField] GameObject FlechaIzquierda;
    [SerializeField] GameObject FlechaDerecha;

    [Header("Progreso")]
    [SerializeField] Image Icono;
    [SerializeField] Sprite IconoCaja;
    [SerializeField] Sprite IconoRegalo;
    [SerializeField] GameObject[] Cajas3D;

    // =========================
    // ESTADO
    // =========================
    Scr_CreadorObjetos ObjetoSeleccionado;
    int IndexSeleccionado = -1;

    int CajasReservadas = 0;
    int UnidadesReservadas = 0;

    TextMeshProUGUI TextoCantidadPrefabSeleccionado;

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
        CargarProgreso();
        RefrescarItems();
    }

    // =========================
    // SELECCIÓN ITEM
    // =========================
    public void SeleccionarItem(GameObject prefabSeleccionado)
    {
        ResetearMarco();

        Image img = prefabSeleccionado.GetComponent<Image>();
        Sprite icono = img.sprite;

        TextoCantidadPrefabSeleccionado =
            prefabSeleccionado.transform.GetChild(0)
            .GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < Inventario.Objetos.Length; i++)
        {
            if (Inventario.Objetos[i].IconoInventario == icono)
            {
                ObjetoSeleccionado = Inventario.Objetos[i];
                IndexSeleccionado = i;
                break;
            }
        }

        if (ObjetoSeleccionado == null) return;

        MarcoVenta.transform.GetChild(0).gameObject.SetActive(true);
        MarcoVenta.transform.GetChild(0)
            .GetComponent<Image>().sprite = icono;

        CajasReservadas = 1;
        ActualizarReservaVisual();
    }

    // =========================
    // FLECHAS
    // =========================
    public void FlechaMas()
    {
        if (CajasReservadas >= ObtenerMaxCajas()) return;
        CajasReservadas++;
        ActualizarReservaVisual();
    }

    public void FlechaMenos()
    {
        if (CajasReservadas <= 1) return;
        CajasReservadas--;
        ActualizarReservaVisual();
    }

    int ObtenerMaxCajas()
    {
        return Inventario.Cantidades[IndexSeleccionado] /
               ObjetoSeleccionado.CantidadVentaMinima;
    }

    // =========================
    // RESERVA VISUAL
    // =========================
    void ActualizarReservaVisual()
    {
        UnidadesReservadas =
            CajasReservadas * ObjetoSeleccionado.CantidadVentaMinima;

        CantidadCajas.text = CajasReservadas.ToString();

        MarcoVenta.transform.GetChild(2)
            .GetComponent<TextMeshProUGUI>()
            .text = UnidadesReservadas.ToString();

        int disponiblesVisual =
            Inventario.Cantidades[IndexSeleccionado] - UnidadesReservadas;

        TextoCantidadPrefabSeleccionado.text =
            disponiblesVisual.ToString();

        FlechaIzquierda.SetActive(CajasReservadas > 1);
        FlechaDerecha.SetActive(CajasReservadas < ObtenerMaxCajas());
    }

    // =========================
    // EMPACAR
    // =========================
    public void BotonEmpacar()
    {
        if (ObjetoSeleccionado == null || CajasReservadas <= 0)
            return;

        if (BloqueadoPorRegalo())
            return;

        // 1️ Restar del inventario
        Inventario.QuitarObjeto(Inventario.Objetos[IndexSeleccionado].Nombre, UnidadesReservadas, false);

        // 2️ Calcular dinero pendiente generado
        int dineroGenerado =
            ObjetoSeleccionado.CantidadVentaMinima *
            ObjetoSeleccionado.ValorVentaIndividual *
            CajasReservadas;

        // 3️⃣ Agregar dinero pendiente (NO inmediato)
        if (ObjetosAgregados != null && dineroGenerado > 0)
        {
            ObjetosAgregados.AgregarDineroPendiente(dineroGenerado);
        }
        else
        {
            Debug.LogWarning("ObjetosAgregados no está asignado o dinero generado es 0");
        }

        // 4️⃣ Progreso de cajas vendidas
        int cajas = PlayerPrefs.GetInt(PREF_CAJAS, 0);
        cajas += CajasReservadas;
        cajas = Mathf.Min(cajas, VentasParaRegalo);

        PlayerPrefs.SetInt(PREF_CAJAS, cajas);
        PlayerPrefs.Save();

        ActualizarUIProgreso(cajas);

        ResetearMarco();
        RefrescarItems();
    }


    bool BloqueadoPorRegalo()
    {
        return PlayerPrefs.GetInt(PREF_CAJAS, 0) >= VentasParaRegalo &&
               PlayerPrefs.GetString(PREF_REGALO, "No") == "No";
    }

    // =========================
    // PROGRESO
    // =========================
    void CargarProgreso()
    {
        int cajas = PlayerPrefs.GetInt(PREF_CAJAS, 0);
        ActualizarUIProgreso(cajas);
    }

    void ActualizarUIProgreso(int cajas)
    {
        TextoCajasVendidas.text = $"{cajas} / {VentasParaRegalo}";

        Icono.sprite = cajas >= VentasParaRegalo
            ? IconoRegalo
            : IconoCaja;

        // ✅ MARCAR REGALO COMO DISPONIBLE AL LLEGAR AL OBJETIVO
        if (cajas >= VentasParaRegalo &&
            PlayerPrefs.GetString(PREF_REGALO, "No") == "No")
        {
            PlayerPrefs.SetString(PREF_REGALO, "Si");
            PlayerPrefs.Save();
        }

        ActivarCajas3D(cajas);
    }


    void ActivarCajas3D(int cajas)
    {
        float progreso = (float)cajas / VentasParaRegalo;
        int activas = Mathf.FloorToInt(progreso * TotalCajas3D);

        for (int i = 0; i < Cajas3D.Length; i++)
            Cajas3D[i].SetActive(i < activas);
    }

    // =========================
    // RESET
    // =========================
    void ResetearMarco()
    {
        ObjetoSeleccionado = null;
        IndexSeleccionado = -1;
        CajasReservadas = 0;
        UnidadesReservadas = 0;
        TextoCantidadPrefabSeleccionado = null;

        CantidadCajas.text = "0";
        MarcoVenta.transform.GetChild(2)
            .GetComponent<TextMeshProUGUI>().text = "0";

        MarcoVenta.transform.GetChild(0).gameObject.SetActive(false);

        FlechaIzquierda.SetActive(false);
        FlechaDerecha.SetActive(false);
    }

    // =========================
    // ITEMS
    // =========================
    void RefrescarItems()
    {
        foreach (Transform t in Items.transform)
            Destroy(t.gameObject);

        for (int i = 0; i < Inventario.Objetos.Length; i++)
        {
            if (Inventario.Cantidades[i] <
                Inventario.Objetos[i].CantidadVentaMinima)
                continue;

            GameObject obj =
                Instantiate(PrefabItem, Items.transform);

            obj.GetComponent<Image>().sprite =
                Inventario.Objetos[i].IconoInventario;

            obj.transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>()
                .text = Inventario.Cantidades[i].ToString();

            obj.transform.GetChild(1)
                .GetComponent<TextMeshProUGUI>()
                .text = Inventario.Objetos[i].Nombre;

            EventTrigger trigger = obj.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };

            entry.callback.AddListener(_ =>
                SeleccionarItem(obj));

            trigger.triggers.Add(entry);
        }
    }
}
