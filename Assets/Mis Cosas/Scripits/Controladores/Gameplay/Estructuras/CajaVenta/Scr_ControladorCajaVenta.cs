using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Scr_ControladorCajaVenta : MonoBehaviour
{
    Scr_Inventario Inventario;
    [SerializeField] GameObject Items;          // Contenedor con GridLayoutGroup
    [SerializeField] GameObject PrefabItem;     // Prefab de ítem
    [SerializeField] TextMeshProUGUI CantidadCajas;
    [SerializeField] TextMeshProUGUI TextoCajasVendidas;
    [SerializeField] GameObject MarcoVenta;
    [SerializeField] Scrollbar Scroll;
    [SerializeField] GameObject ObjetoFlechaIzquierda;
    [SerializeField] GameObject ObjetoFlechaDerecha;
    [SerializeField] Image Icono;
    [SerializeField] Sprite IconoCaja;
    [SerializeField] Sprite IconoRegalo;
    Scr_CreadorObjetos ObjetoSeleccionado;
    int NumeroObjetoSeleccionado;
    float ValorSlider;
    int CajasReservadas = 0;  // cajas reservadas del item seleccionado
    [SerializeField] float VelocidadSlider = 10f; // Velocidad de interpolación del scroll

    void Awake()
    {
        Inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();
    }

    private void OnEnable()
    {
        int NumeroObjeto = 0;
        foreach (int CantidadObjeto in Inventario.Cantidades)
        {
            if (CantidadObjeto >= Inventario.Objetos[NumeroObjeto].CantidadVentaMinima)
            {
                GameObject Hijo = Instantiate(PrefabItem, Vector3.zero, quaternion.identity, Items.transform);
                Hijo.GetComponent<Image>().sprite = Inventario.Objetos[NumeroObjeto].IconoInventario;
                Hijo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Inventario.Objetos[NumeroObjeto].Nombre;
                Hijo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Inventario.Cantidades[NumeroObjeto].ToString();

                EventTrigger trigger = Hijo.AddComponent<EventTrigger>();

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((data) =>
                {
                    SeleccionarItem(Hijo.GetComponent<Image>().sprite);
                });

                trigger.triggers.Add(entry);
            }
            NumeroObjeto++;
        }

        // 🔹 Cargar PlayerPrefs y actualizar texto
        int cajasVendidas = PlayerPrefs.GetInt("Cajas Vendidas", 0);
        TextoCajasVendidas.text = $"{cajasVendidas} / 100";

        if (PlayerPrefs.GetString("CajaVentaRegalo", "No") == "Si")
        {
            Icono.sprite = IconoRegalo;

        }
        else
        {
            Icono.sprite = IconoCaja;
        }
    }



    private void OnDisable()
    {
        foreach (Transform Hijo in Items.transform)
        {
            Destroy(Hijo.gameObject);
        }

        // 🔹 Resetear solo el marco de venta
        MarcoVenta.transform.GetChild(0).gameObject.SetActive(false); // ocultar icono
        MarcoVenta.transform.GetChild(0).GetComponent<Image>().sprite = null;
        MarcoVenta.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "0";
        CantidadCajas.text = "0";

        // 🔹 Resetear selección
        ObjetoSeleccionado = null;
        NumeroObjetoSeleccionado = -1;
        CajasReservadas = 0;

        // 🔹 Desactivar flechas
        ObjetoFlechaIzquierda.SetActive(false);
        ObjetoFlechaDerecha.SetActive(false);
    }

    void Update()
    {
        ScrollBar();
    }

    void ScrollBar()
    {
        int casillasActivas = 0;
        foreach (Transform casilla in Items.transform)
        {
            if (casilla.gameObject.activeSelf)
                casillasActivas++;
        }

        GridLayoutGroup grid = Items.GetComponent<GridLayoutGroup>();

        // 3 columnas fijas
        int columnas = 3;
        int filas = Mathf.CeilToInt((float)casillasActivas / columnas) - 2;

        // Altura total del contenido
        float contenidoAlto = (filas * grid.cellSize.y) + ((filas - 1) * grid.spacing.y);

        // Altura visible (viewport)
        float viewportAlto = ((RectTransform)grid.transform).rect.height;

        // Rango máximo de scroll
        float scrollMax = Mathf.Max(0, contenidoAlto - viewportAlto);

        // Ajustar tamaño de la scrollbar
        Scroll.size = contenidoAlto <= viewportAlto ? 1 : viewportAlto / contenidoAlto;

        // Input con la rueda del mouse
        float scrollDelta = -Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            ValorSlider = Mathf.Clamp01(ValorSlider + scrollDelta);
            Scroll.value = ValorSlider;
        }
        else
        {
            ValorSlider = Scroll.value;
        }

        // Ajustar padding.top
        float topActual = grid.padding.top;
        float nuevoTop = Mathf.Lerp(topActual, -ValorSlider * scrollMax, Time.deltaTime * VelocidadSlider);

        grid.padding = new RectOffset(grid.padding.left, grid.padding.right, Mathf.RoundToInt(nuevoTop), grid.padding.bottom);
    }

    public void SeleccionarItem(Sprite spriteSeleccionado)
    {
        // --- Restaurar reservas del ítem anterior ---
        if (ObjetoSeleccionado != null && CajasReservadas > 0)
        {
            foreach (Transform Item in Items.GetComponentInChildren<Transform>())
            {
                if (Item.GetComponent<Image>().sprite == ObjetoSeleccionado.IconoInventario)
                {
                    Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                        (int.Parse(Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text) + (CajasReservadas * ObjetoSeleccionado.CantidadVentaMinima)).ToString();
                    break;
                }
            }
        }

        // Reset reserva
        CajasReservadas = 0;

        // --- Selección normal de ítem ---
        int i = 0;
        foreach (Scr_CreadorObjetos objeto in Inventario.Objetos)
        {
            if (objeto.IconoInventario == spriteSeleccionado)
            {
                MarcoVenta.transform.GetChild(0).GetComponent<Image>().sprite = objeto.IconoInventario;
                MarcoVenta.transform.GetChild(0).gameObject.SetActive(true);

                CantidadCajas.text = "1";
                NumeroObjetoSeleccionado = i;
                ObjetoSeleccionado = objeto;

                MarcoVenta.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = objeto.CantidadVentaMinima.ToString();

                // Reservamos la 1 caja inicial
                foreach (Transform Item in Items.GetComponentInChildren<Transform>())
                {
                    if (Item.GetComponent<Image>().sprite == objeto.IconoInventario)
                    {
                        Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                            (int.Parse(Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text) - objeto.CantidadVentaMinima).ToString();
                        break;
                    }
                }

                CajasReservadas = 1; // ya reservamos 1

                ActualizarFlechas();
                break;
            }
            i++;
        }
    }


    public void FlechaIzquierda()
    {
        int cantidadActual = int.Parse(CantidadCajas.text);
        if (cantidadActual > 1)
        {
            CantidadCajas.text = (cantidadActual - 1).ToString();
            foreach (Transform Item in Items.GetComponentInChildren<Transform>())
            {
                if (Item.GetComponent<Image>().sprite == MarcoVenta.transform.GetChild(0).GetComponent<Image>().sprite)
                {
                    Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                        (int.Parse(Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text) + ObjetoSeleccionado.CantidadVentaMinima).ToString();
                }
            }

            CajasReservadas--; // 👈 faltaba reducir la reserva
        }
        ActualizarFlechas();
    }


    public void FlechaDerecha()
    {
        int cantidadActual = int.Parse(CantidadCajas.text);
        int maxCajas = Inventario.Cantidades[NumeroObjetoSeleccionado] / ObjetoSeleccionado.CantidadVentaMinima;

        if (cantidadActual < maxCajas)
        {
            CantidadCajas.text = (cantidadActual + 1).ToString();
            MarcoVenta.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (ObjetoSeleccionado.CantidadVentaMinima * int.Parse(CantidadCajas.text)).ToString();

            foreach (Transform Item in Items.GetComponentInChildren<Transform>())
            {
                if (Item.GetComponent<Image>().sprite == MarcoVenta.transform.GetChild(0).GetComponent<Image>().sprite)
                {
                    Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                        (int.Parse(Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text) - ObjetoSeleccionado.CantidadVentaMinima).ToString();
                    break;
                }
            }

            CajasReservadas++; // ✅ aumentar reserva
        }

        ActualizarFlechas();
    }

    private void ActualizarFlechas()
    {
        if (CantidadCajas.text != "0")
        {
            int cantidadActual = int.Parse(CantidadCajas.text);
            int maxCajas = Inventario.Cantidades[NumeroObjetoSeleccionado] / ObjetoSeleccionado.CantidadVentaMinima;
            MarcoVenta.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (ObjetoSeleccionado.CantidadVentaMinima * int.Parse(CantidadCajas.text)).ToString();


            // Si solo puede vender 1 → apagar ambas
            if (maxCajas <= 1)
            {
                ObjetoFlechaIzquierda.SetActive(false);
                ObjetoFlechaDerecha.SetActive(false);
                return;
            }

            // Flecha izquierda → solo activa si cantidad > 1
            ObjetoFlechaIzquierda.SetActive(cantidadActual > 1);

            // Flecha derecha → solo activa si cantidad < max
            ObjetoFlechaDerecha.SetActive(cantidadActual < maxCajas);
        }

    }

    public void BotonMitad()
    {
        if (ObjetoSeleccionado == null) return;

        int maxCajas = Inventario.Cantidades[NumeroObjetoSeleccionado] / ObjetoSeleccionado.CantidadVentaMinima;
        int mitad = Mathf.Max(1, maxCajas / 2); // al menos 1 caja

        int cantidadActual = int.Parse(CantidadCajas.text);
        int diferencia = mitad - cantidadActual;

        // Ajustar inventario en Items
        foreach (Transform Item in Items.GetComponentInChildren<Transform>())
        {
            if (Item.GetComponent<Image>().sprite == MarcoVenta.transform.GetChild(0).GetComponent<Image>().sprite)
            {
                Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    (int.Parse(Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text) - (diferencia * ObjetoSeleccionado.CantidadVentaMinima)).ToString();
            }
        }

        // Actualizar marco
        CantidadCajas.text = mitad.ToString();
        MarcoVenta.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (ObjetoSeleccionado.CantidadVentaMinima * mitad).ToString();

        ActualizarFlechas();
    }

    public void BotonMaximo()
    {
        if (ObjetoSeleccionado == null) return;

        int maxCajas = Inventario.Cantidades[NumeroObjetoSeleccionado] / ObjetoSeleccionado.CantidadVentaMinima;
        int cantidadActual = int.Parse(CantidadCajas.text);
        int diferencia = maxCajas - cantidadActual;

        // Ajustar inventario en Items
        foreach (Transform Item in Items.GetComponentInChildren<Transform>())
        {
            if (Item.GetComponent<Image>().sprite == MarcoVenta.transform.GetChild(0).GetComponent<Image>().sprite)
            {
                Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    (int.Parse(Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text) - (diferencia * ObjetoSeleccionado.CantidadVentaMinima)).ToString();
            }
        }

        // Actualizar marco
        CantidadCajas.text = maxCajas.ToString();
        MarcoVenta.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (ObjetoSeleccionado.CantidadVentaMinima * maxCajas).ToString();

        ActualizarFlechas();
    }

    public void BotonEmpacar()
    {
        if (ObjetoSeleccionado == null) return;

        int cantidadCajas = int.Parse(CantidadCajas.text);
        if (cantidadCajas <= 0) return;

        // 🔹 Sumar a PlayerPrefs
        int cajasVendidas = PlayerPrefs.GetInt("Cajas Vendidas", 0);
        int regalosEntregados = PlayerPrefs.GetInt("Regalos Entregados", 0);

        cajasVendidas += cantidadCajas;

        // Si ya hay un regalo pendiente y se pasa de 100 → congelar en 100
        if (PlayerPrefs.GetString("CajaVentaRegalo", "No") == "Si" && cajasVendidas > 100)
        {
            cajasVendidas = 100;
        }
        else if (cajasVendidas >= 100 && regalosEntregados < 2)
        {
            // Solo dar regalo si NO hay uno pendiente
            if (PlayerPrefs.GetString("CajaVentaRegalo", "No") == "No")
            {
                cajasVendidas -= 100;
                PlayerPrefs.SetString("CajaVentaRegalo", "Si");
                transform.parent.GetComponent<Scr_ControladorCajaVenta1>().EntregaRegalo = true;

                regalosEntregados++;
                PlayerPrefs.SetInt("Regalos Entregados", regalosEntregados);
            }
        }

        PlayerPrefs.SetInt("Cajas Vendidas", cajasVendidas);
        PlayerPrefs.Save();

        // 🔹 Actualizar texto
        TextoCajasVendidas.text = $"{cajasVendidas} / 100";

        // 🔹 Quitar del inventario
        Inventario.Cantidades[NumeroObjetoSeleccionado] -= (cantidadCajas * ObjetoSeleccionado.CantidadVentaMinima);
        GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().DineroRecompensa
            += cantidadCajas * ObjetoSeleccionado.ValorVentaIndividual * ObjetoSeleccionado.CantidadVentaMinima;

        foreach (Transform Item in Items.GetComponentInChildren<Transform>())
        {
            if (Item.GetComponent<Image>().sprite == ObjetoSeleccionado.IconoInventario)
            {
                Item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    Inventario.Cantidades[NumeroObjetoSeleccionado].ToString();
                break;
            }
        }

        // 🔹 Resetear marco
        MarcoVenta.transform.GetChild(0).gameObject.SetActive(false);
        MarcoVenta.transform.GetChild(0).GetComponent<Image>().sprite = null;
        MarcoVenta.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "0";
        CantidadCajas.text = "0";

        ObjetoSeleccionado = null;
        NumeroObjetoSeleccionado = -1;
        CajasReservadas = 0;
        ObjetoFlechaIzquierda.SetActive(false);
        ObjetoFlechaDerecha.SetActive(false);

        // 🔹 Cambiar icono según progreso
        if (PlayerPrefs.GetString("CajaVentaRegalo", "No") == "Si")
        {
            Icono.sprite = IconoRegalo;

        }
        else
        {
            Icono.sprite = IconoCaja;
        }
    }



}
