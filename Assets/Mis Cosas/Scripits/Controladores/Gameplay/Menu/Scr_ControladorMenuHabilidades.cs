using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using Unity.VisualScripting;

public class Scr_ControladorMenuHabilidades : MonoBehaviour
{
    [SerializeField] GameObject Arbol;
    [SerializeField] GameObject[] Ramas;
    [SerializeField] string RamaActual;
    [SerializeField] GameObject[] Barras;
    [SerializeField] float DuracionTransicion;
    [SerializeField] Ease TipoTransicionPaneles;
    [SerializeField] Ease TipoTransicionBotonesIn;
    [SerializeField] Ease TipoTransicionBotonesOut;
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject ObjetoHabilidadSeleccionada;
    [SerializeField] GameObject Medallas;
    [SerializeField] private Sprite[] IconosNivelArma;

    public GameObject BotonActual;
    public string HabilidadActual;

    GameObject BotonSeleccionado;
    [SerializeField] Scr_CreadorHabilidades[] Habilidades;
    [SerializeField] TextMeshProUGUI Puntos;
    [SerializeField] GameObject BotonAceptar;

    bool YaSelecciono = false;
    private RectTransform arbolRectTransform;

    private void Awake()
    {
        moveSpeed = moveSpeed * 1000;
        arbolRectTransform = Arbol.GetComponent<RectTransform>();
        ActualizarBarrasPorRango();
    }

    void Start() { }

    void Update()
    {
        Puntos.text = PlayerPrefs.GetInt("PuntosDeHabilidad", 0).ToString();
        InputPruebas();
    }

    public void SeleccionarRama(string NombreRama)
    {
        RamaActual = NombreRama;

        switch (RamaActual)
        {
            case "Naturaleza":
                {
                    Tween.LocalScaleY(Barras[0].transform.GetChild(0), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalScale(Ramas[0].transform.GetChild(2), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalScale(Ramas[0].transform.GetChild(3), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPositionY(Ramas[0].transform.GetChild(3), -75, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalScale(Ramas[0].transform.GetChild(0), new Vector3(4, 4, 1), DuracionTransicion, TipoTransicionPaneles, default);

                    Tween.LocalScale(Arbol.transform, new Vector3(1.5f, 1.5f, 1.5f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, new Vector3(-1105, -110, 0), DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }

            case "Industrial":
                {
                    Tween.LocalScaleY(Barras[2].transform.GetChild(0), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalScale(Ramas[2].transform.GetChild(1), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalScale(Ramas[2].transform.GetChild(2), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPositionY(Ramas[2].transform.GetChild(2), -75, DuracionTransicion, TipoTransicionPaneles, default);

                    Tween.LocalScale(Arbol.transform, new Vector3(1.5f, 1.5f, 1.5f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, new Vector3(1105, -110, 0), DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }

            case "Tecnica":
                {
                    Tween.LocalScaleY(Barras[1].transform.GetChild(0), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalScale(Ramas[1].transform.GetChild(1), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalScale(Ramas[1].transform.GetChild(2), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPositionY(Ramas[1].transform.GetChild(2), -440, DuracionTransicion, TipoTransicionPaneles, default);

                    Tween.LocalScale(Arbol.transform, new Vector3(1.5f, 1.5f, 1.5f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, new Vector3(-850, 890, 0), DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }

            case "Arsenal":
                {
                    Tween.LocalScaleY(Barras[3].transform.GetChild(0), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalScale(Ramas[3].transform.GetChild(1), 1, DuracionTransicion, TipoTransicionPaneles, default);

                    Tween.LocalScale(Arbol.transform, new Vector3(1.5f, 1.5f, 1.5f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, new Vector3(850, 780, 0), DuracionTransicion, TipoTransicionPaneles, default);

                    ActualizarIconoRango();

                    break;
                }
        }
    }

    public void CerrarRama(string NombreRama)
    {
        switch (RamaActual)
        {
            case "Naturaleza":
                {
                    Tween.LocalScaleY(Barras[0].transform.GetChild(0), 0, DuracionTransicion, TipoTransicionBotonesIn, default);
                    Tween.LocalScale(Ramas[0].transform.GetChild(2), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    Tween.LocalScale(Ramas[0].transform.GetChild(3), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    Tween.LocalPositionY(Ramas[0].transform.GetChild(3), 180, DuracionTransicion, TipoTransicionBotonesOut, default);
                    Tween.LocalScale(Ramas[0].transform.GetChild(0), new Vector3(0, 0, 1), DuracionTransicion, TipoTransicionBotonesIn, default);

                    Tween.LocalScale(Arbol.transform, new Vector3(1, 1, 1), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, Vector3.zero, DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }

            case "Industrial":
                {
                    Tween.LocalScaleY(Barras[2].transform.GetChild(0), 0, DuracionTransicion, TipoTransicionBotonesIn, default);
                    Tween.LocalScale(Ramas[2].transform.GetChild(1), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    Tween.LocalScale(Ramas[2].transform.GetChild(2), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    Tween.LocalPositionY(Ramas[2].transform.GetChild(2), 180, DuracionTransicion, TipoTransicionBotonesOut, default);

                    Tween.LocalScale(Arbol.transform, new Vector3(1, 1, 1), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, Vector3.zero, DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }

            case "Tecnica":
                {
                    Tween.LocalScaleY(Barras[1].transform.GetChild(0), 0, DuracionTransicion, TipoTransicionBotonesIn, default);
                    Tween.LocalScale(Ramas[1].transform.GetChild(1), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    Tween.LocalScale(Ramas[1].transform.GetChild(2), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    Tween.LocalPositionY(Ramas[1].transform.GetChild(2), -180, DuracionTransicion, TipoTransicionBotonesOut, default);

                    Tween.LocalScale(Arbol.transform, new Vector3(1, 1, 1), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, Vector3.zero, DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }

            case "Arsenal":
                {
                    Tween.LocalScaleY(Barras[3].transform.GetChild(0), 0, DuracionTransicion, TipoTransicionBotonesIn, default);
                    Tween.LocalScale(Ramas[3].transform.GetChild(1), 0, DuracionTransicion, TipoTransicionBotonesOut, default);

                    Tween.LocalScale(Arbol.transform, new Vector3(1, 1, 1), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, Vector3.zero, DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }
        }

        RamaActual = "";
    }

    void ActualizarIconoRango()
    {
        for (int i = 0; i < 10; i++)
        {
            switch (PlayerPrefs.GetInt("Rango " + Barras[3].transform.GetChild(0).GetChild(i).name))
            {
                case 1:
                    {
                        Barras[3].transform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(true);
                        Barras[3].transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = IconosNivelArma[0];
                        break;
                    }
                case 2:
                    {
                        Barras[3].transform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(true);
                        Barras[3].transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = IconosNivelArma[1];
                        break;
                    }
                case 3:
                    {
                        Barras[3].transform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(true);
                        Barras[3].transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = IconosNivelArma[2];
                        break;
                    }
                case 4:
                    {
                        Barras[3].transform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(true);
                        Barras[3].transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = IconosNivelArma[3];
                        break;
                    }
                default:
                    {
                        Debug.Log("No se encontro el rango o no coincide el nombre");
                        break;
                    }
            }
        }
    }

    public void SeleccionarHabilidad()
    {
        if (YaSelecciono)
        {
            Debug.Log("[SH] YaSelecciono es true => ignorando nueva selección.");
            return;
        }

        if (string.IsNullOrEmpty(HabilidadActual))
        {
            Debug.LogWarning("[SH] No hay HabilidadActual (string vacío).");
            return;
        }

        Scr_CreadorHabilidades habilidadEncontrada = null;

        foreach (Scr_CreadorHabilidades hab in Habilidades)
        {
            if (hab != null && hab.NombreBoton == HabilidadActual)
            {
                habilidadEncontrada = hab;
                break;
            }
        }

        Image imagenBoton = BotonActual.GetComponent<Image>();

        if (PlayerPrefs.GetString("Habilidad:" + BotonActual.name, "No") == "No" && !habilidadEncontrada.RequiereMedallas)
        {
            if (imagenBoton != null)
            {
                Color color = imagenBoton.color;
                color.a = 100f / 255f;
                imagenBoton.color = color;
            }
        }

        if (habilidadEncontrada == null)
        {
            Debug.LogError($"[SH] No se encontró la habilidad '{HabilidadActual}' en el array Habilidades.");
            return;
        }

        if (ObjetoHabilidadSeleccionada == null)
        {
            Debug.LogError("[SH] ObjetoHabilidadSeleccionada es NULL. Asigna en inspector.");
            return;
        }

        if (BotonSeleccionado == null && BotonActual != null)
        {
            BotonSeleccionado = BotonActual;
        }

        Transform panel = ObjetoHabilidadSeleccionada.transform;

        try
        {
            if (panel.childCount > 0 && panel.GetChild(1).childCount > 0)
                panel.GetChild(1).GetChild(0).GetComponent<Image>().sprite = habilidadEncontrada.Icono;
            else
                Debug.LogWarning("[SH] Falta icono en child(0).");

            if (panel.childCount > 2)
                panel.GetChild(2).GetComponent<TextMeshProUGUI>().text = habilidadEncontrada.Nombre;

            if (panel.childCount > 3)
                panel.GetChild(3).GetComponent<TextMeshProUGUI>().text = habilidadEncontrada.Descripcion;

            if (panel.childCount > 4)
                panel.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Costo: " + habilidadEncontrada.Costo;
        }
        catch (Exception e)
        {
            Debug.LogError("[SH] Error al rellenar UI: " + e);
        }

        Transform panelItems = null;
        if (panel.childCount > 5)
        {
            panelItems = panel.GetChild(5);
        }

        // --------------------------------------------------------------------
        // -------------------- REQUIERE MEDALLAS -----------------------------
        // --------------------------------------------------------------------

        if (habilidadEncontrada.RequiereMedallas)
        {
            Medallas.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text =
                habilidadEncontrada.DescripcionesMejorasPermanentes[0];

            Medallas.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text =
                habilidadEncontrada.DescripcionesMejorasPermanentes[1];

            Medallas.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text =
                habilidadEncontrada.DescripcionesMejorasPermanentes[2];

            Medallas.transform.GetChild(3).GetChild(2).GetComponent<TextMeshProUGUI>().text =
                habilidadEncontrada.DescripcionesMejorasPermanentes[3];

            Medallas.SetActive(true);

            if (panelItems == null)
            {
                Debug.LogError("[SH] RequiereMedallas=true pero no existe panelItems.");
            }
            else
            {
                int rango = PlayerPrefs.GetInt("Rango " + habilidadEncontrada.NombreBoton, 0);

                Transform panelMedallas = Medallas.transform;

                for (int i = 0; i < panelMedallas.childCount; i++)
                {
                    Transform panelRango = panelMedallas.GetChild(i);

                    Transform iconoMedalla = panelRango.Find("Medalla");
                    Transform iconoCandado = panelRango.Find("Candado");

                    if (iconoMedalla == null || iconoCandado == null)
                    {
                        Debug.LogWarning("[SH] Panel de medallas sin 'Medalla' o 'Candado'.");
                        continue;
                    }

                    if (i < rango)
                    {
                        iconoMedalla.gameObject.SetActive(true);
                        iconoCandado.gameObject.SetActive(false);
                    }
                    else
                    {
                        iconoMedalla.gameObject.SetActive(false);
                        iconoCandado.gameObject.SetActive(true);
                    }
                }

                Debug.Log($"[SH] Rango actual = {rango}");

                if (rango >= 5)
                {
                    panelItems.gameObject.SetActive(false);
                }
                else
                {
                    panelItems.gameObject.SetActive(true);

                    int slotsUI = panelItems.childCount;

                    for (int i = 0; i < slotsUI; i++)
                    {
                        Transform slot = panelItems.GetChild(i);

                        slot.gameObject.SetActive(i == rango);

                        if (i == rango)
                        {
                            if (habilidadEncontrada.ItemsRequeridos != null &&
                                habilidadEncontrada.CantidadesRequeridas != null &&
                                rango < habilidadEncontrada.ItemsRequeridos.Length &&
                                rango < habilidadEncontrada.CantidadesRequeridas.Length)
                            {
                                if (slot.GetComponent<Image>() != null &&
                                    habilidadEncontrada.ItemsRequeridos[rango] != null)
                                {
                                    slot.GetComponent<Image>().sprite =
                                        habilidadEncontrada.ItemsRequeridos[rango].IconoInventario;
                                }

                                if (slot.childCount > 0 &&
                                    slot.GetChild(0).GetComponent<TextMeshProUGUI>() != null)
                                {
                                    slot.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                                        habilidadEncontrada.CantidadesRequeridas[rango].ToString();
                                }
                            }
                            else
                            {
                                Debug.LogError("[SH] Arrays inválidos para este rango.");
                            }
                        }
                    }
                }
            }
        }

        // --------------------------------------------------------------------
        // -------------------- REQUIERE ITEMS NORMALES -----------------------
        // --------------------------------------------------------------------
        else if (habilidadEncontrada.RequiereItems)
        {
            Medallas.SetActive(false);

            if (panelItems == null)
            {
                Debug.LogError("[SH] RequiereItems=true pero no existe panelItems.");
            }
            else
            {
                panelItems.gameObject.SetActive(true);

                int slots = panelItems.childCount;
                int itemsLen = habilidadEncontrada.ItemsRequeridos?.Length ?? 0;
                int cantLen = habilidadEncontrada.CantidadesRequeridas?.Length ?? 0;

                for (int i = 0; i < slots; i++)
                {
                    Transform slot = panelItems.GetChild(i);

                    if (i < itemsLen && i < cantLen)
                    {
                        slot.gameObject.SetActive(true);

                        if (slot.GetComponent<Image>() != null &&
                            habilidadEncontrada.ItemsRequeridos[i] != null)
                        {
                            slot.GetComponent<Image>().sprite =
                                habilidadEncontrada.ItemsRequeridos[i].IconoInventario;
                        }

                        if (slot.childCount > 0 && slot.GetChild(0).GetComponent<TextMeshProUGUI>() != null)
                        {
                            slot.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                                habilidadEncontrada.CantidadesRequeridas[i].ToString();
                        }
                    }
                    else
                    {
                        slot.gameObject.SetActive(false);
                    }
                }
            }
        }

        // --------------------------------------------------------------------
        // -------------------- NO REQUIERE NADA ------------------------------
        // --------------------------------------------------------------------
        else
        {
            Medallas.SetActive(false);
            if (panelItems != null)
                panelItems.gameObject.SetActive(false);
        }

        // --------------------------------------------------------------------
        // -------------------- ACTIVAR BOTÓN ACEPTAR --------------------------
        // --------------------------------------------------------------------

        bool yaComprada = PlayerPrefs.GetString("Habilidad:" + HabilidadActual, "No") == "Si";
        int puntos = PlayerPrefs.GetInt("PuntosDeHabilidad", 0);
        bool permiteComprar = true;

        if (yaComprada)
        {
            permiteComprar = false;
        }
        else if (puntos < habilidadEncontrada.Costo)
        {
            permiteComprar = false;
        }
        else
        {
            if (habilidadEncontrada.RequiereMedallas)
            {
                Scr_Inventario inv = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();
                int rango = PlayerPrefs.GetInt("Rango " + habilidadEncontrada.NombreBoton, 0);

                if (rango >= 5)
                {
                    permiteComprar = false;
                }
                else
                {
                    string medallaReq = habilidadEncontrada.ItemsRequeridos[rango].Nombre;
                    int cantReq = habilidadEncontrada.CantidadesRequeridas[rango];

                    bool tiene = false;

                    for (int i = 0; i < inv.Objetos.Length; i++)
                    {
                        if (inv.Objetos[i].Nombre == medallaReq && inv.Cantidades[i] >= cantReq)
                        {
                            tiene = true;
                            break;
                        }
                    }

                    permiteComprar = tiene;
                }
            }
            else if (habilidadEncontrada.RequiereItems)
            {
                Scr_Inventario inv = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();

                bool tieneTodo = true;

                if (habilidadEncontrada.ItemsRequeridos != null)
                {
                    for (int n = 0; n < habilidadEncontrada.ItemsRequeridos.Length; n++)
                    {
                        Scr_CreadorObjetos req = habilidadEncontrada.ItemsRequeridos[n];
                        int cantReq = habilidadEncontrada.CantidadesRequeridas[n];

                        bool encontrado = false;

                        for (int j = 0; j < inv.Objetos.Length; j++)
                        {
                            if (inv.Objetos[j] == req && inv.Cantidades[j] >= cantReq)
                            {
                                encontrado = true;
                                break;
                            }
                        }

                        if (!encontrado)
                        {
                            tieneTodo = false;
                            break;
                        }
                    }
                }
                else
                {
                    tieneTodo = false;
                }

                permiteComprar = tieneTodo;
            }
        }

        BotonAceptar.SetActive(permiteComprar);
        ObjetoHabilidadSeleccionada.SetActive(true);
        YaSelecciono = true;
    }

    public void ComprarHabilidad()
    {
        Debug.Log($"[DEBUG] Intentando comprar: {HabilidadActual}");

        if (string.IsNullOrEmpty(HabilidadActual))
        {
            Debug.LogWarning("[DEBUG] No hay habilidad seleccionada para comprar.");
            return;
        }

        foreach (Scr_CreadorHabilidades habilidad in Habilidades)
        {
            if (habilidad != null && habilidad.NombreBoton == HabilidadActual)
            {
                int puntosActuales = PlayerPrefs.GetInt("PuntosDeHabilidad", 0);

                if (puntosActuales >= habilidad.Costo &&
                    PlayerPrefs.GetString("Habilidad:" + HabilidadActual, "No") == "No")
                {
                    if (habilidad.RequiereItems)
                    {
                        int Obj = 0;
                        foreach (Scr_CreadorObjetos Objeto in habilidad.ItemsRequeridos)
                        {
                            GameObject.Find("Gata")
                                .transform.GetChild(7)
                                .GetComponent<Scr_Inventario>()
                                .QuitarObjeto(habilidad.CantidadesRequeridas[Obj], Objeto.Nombre);

                            Obj++;
                        }
                    }

                    if (!habilidad.RequiereMedallas)
                    {
                        PlayerPrefs.SetString("Habilidad:" + HabilidadActual, "Si");
                    }

                    if (habilidad.RequiereMedallas)
                    {
                        int rango = PlayerPrefs.GetInt("Rango " + habilidad.NombreBoton, 0);

                        if (rango < 5)
                        {

                            GameObject.Find("Gata")
                                .transform.GetChild(7)
                                .GetComponent<Scr_Inventario>()
                                .QuitarObjeto(1, habilidad.ItemsRequeridos[rango].Nombre);

                            rango++;
                            PlayerPrefs.SetInt("Rango " + habilidad.NombreBoton, rango);
                            ActualizarIconoRango();
                        }
                    }

                    puntosActuales -= habilidad.Costo;
                    PlayerPrefs.SetInt("PuntosDeHabilidad", puntosActuales);

                    PlayerPrefs.Save();
                    ActualizarBarrasPorRango();

                    YaSelecciono = false;
                    ObjetoHabilidadSeleccionada.SetActive(false);
                    BotonAceptar.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("[DEBUG] No tienes puntos o ya fue comprada.");
                }

                return;
            }
        }

        YaSelecciono = false;
        BotonActual = null;
        HabilidadActual = "";
        ObjetoHabilidadSeleccionada.SetActive(false);
        BotonAceptar.SetActive(false);
    }

    public void RechazarHabilidad()
    {
        YaSelecciono = false;
        BotonActual = null;
        HabilidadActual = "";
        ObjetoHabilidadSeleccionada.SetActive(false);
    }

    public void EntraHabilidad(GameObject boton)
    {
        if (YaSelecciono) return;
        if (boton.GetComponent<Image>().color == Color.black) return;

        BotonActual = boton;
        HabilidadActual = boton.name;

        Scr_CreadorHabilidades habilidad = null;

        foreach (var h in Habilidades)
        {
            if (h != null && h.NombreBoton == HabilidadActual)
            {
                habilidad = h;
                break;
            }
        }

        Image imagenBoton = boton.GetComponent<Image>();
        if (imagenBoton == null) return;

        if (habilidad != null && habilidad.RequiereMedallas)
        {
            Color col = imagenBoton.color;
            col.a = 1f;
            imagenBoton.color = col;
            return;
        }

        Color color = imagenBoton.color;
        color.a = 1f;
        imagenBoton.color = color;
    }

    public void SaleHabilidad()
    {
        if (YaSelecciono) return;

        if (BotonActual != null)
        {
            Scr_CreadorHabilidades habilidad = null;

            foreach (var h in Habilidades)
            {
                if (h != null && h.NombreBoton == BotonActual.name)
                {
                    habilidad = h;
                    break;
                }
            }

            Image imagenBoton = BotonActual.GetComponent<Image>();

            if (habilidad != null && habilidad.RequiereMedallas)
            {
                if (imagenBoton != null)
                {
                    Color col = imagenBoton.color;
                    col.a = 1f;
                    imagenBoton.color = col;
                }

                HabilidadActual = "";
                BotonActual = null;
                return;
            }

            if (PlayerPrefs.GetString("Habilidad:" + BotonActual.name, "No") == "No")
            {
                if (imagenBoton != null)
                {
                    Color color = imagenBoton.color;
                    color.a = 100f / 255f;
                    imagenBoton.color = color;
                }
            }

            HabilidadActual = "";
            BotonActual = null;
        }
    }

    public void EntraRango(int Numero)
    {
        if (Medallas.transform.GetChild(Numero).GetComponent<RectTransform>().anchoredPosition.x == -165)
        {
            Tween.UIAnchoredPositionX(
                Medallas.transform.GetChild(Numero).GetComponent<RectTransform>(),
                0,
                0.1f,
                Ease.Default,
                default
            );
        }
    }

    public void SaleRango(int Numero)
    {
        if (Medallas.transform.GetChild(Numero).GetComponent<RectTransform>().anchoredPosition.x == 0)
        {
            Tween.UIAnchoredPositionX(
                Medallas.transform.GetChild(Numero).GetComponent<RectTransform>(),
                -165,
                0.11f,
                Ease.Default,
                default
            );
        }
    }

    public void ActualizarBarrasPorRango()
    {
        foreach (GameObject Barra in Barras)
        {
            int rango = PlayerPrefs.GetInt("Rango " + Barra.name, 0);
            int hijosTotales = Barra.transform.GetChild(0).childCount;
            int totalBotones = 0;

            if (Barra.name == "Barra Arsenal3")
            {
                totalBotones = 1 + (rango * 3);
            }
            else
            {
                int CantBotones = int.Parse(Barra.name[Barra.name.Length - 1].ToString());
                totalBotones = CantBotones * rango;
            }

            for (int i = 0; i < hijosTotales; i++)
            {
                Image boton = Barra.transform.GetChild(0).GetChild(i).GetComponent<Image>();

                if (i < totalBotones)
                {
                    string nombreHabilidad = boton.name;
                    bool estaComprada = PlayerPrefs.GetString("Habilidad:" + nombreHabilidad, "No") == "Si";

                    if (estaComprada)
                        boton.color = Color.white;
                    else
                    {
                        Scr_CreadorHabilidades habTemp = null;

                        foreach (var h in Habilidades)
                        {
                            if (h != null && h.NombreBoton == nombreHabilidad)
                            {
                                habTemp = h;
                                break;
                            }
                        }

                        if (habTemp != null && habTemp.RequiereMedallas)
                        {
                            boton.color = Color.white;
                        }
                        else
                        {
                            Color c = Color.white;
                            c.a = 100f / 255f;
                            boton.color = c;
                        }
                    }
                }
                else
                {
                    boton.color = Color.black;
                }
            }
        }
    }

    public void InputPruebas()
    {
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            foreach (GameObject barra in Barras)
            {
                string key = "Rango " + barra.name;
                PlayerPrefs.DeleteKey(key);
            }

            ActualizarBarrasPorRango();
        }

        for (int i = 0; i < Barras.Length; i++)
        {
            KeyCode keyPadNumber = KeyCode.Keypad0 + (i + 1);

            if (Input.GetKey(keyPadNumber))
            {
                string key = "Rango " + Barras[i].name;
                int rangoActual = PlayerPrefs.GetInt(key, 0);

                if (Input.GetKeyDown(KeyCode.KeypadPlus))
                {
                    rangoActual++;
                    PlayerPrefs.SetInt(key, rangoActual);
                    ActualizarBarrasPorRango();
                }

                if (Input.GetKeyDown(KeyCode.KeypadMinus))
                {
                    rangoActual = Mathf.Max(0, rangoActual - 1);
                    PlayerPrefs.SetInt(key, rangoActual);
                    ActualizarBarrasPorRango();
                }
            }
        }
    }
}
