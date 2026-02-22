using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tienda_3D : MonoBehaviour
{
    public float descuento = 20;
    public GameObject FoodTruck;

    [Header("Variables Generales"), Space, Space]
    public Scr_Inventario Inventario;
    [SerializeField] GameObject Campana;
    private Material[] materialesCampana;
    private bool mouseSobreCampana = false;
    [SerializeField] GameObject Marco;
    private Material[] materialesMarco;
    [SerializeField] GameObject CamaraTienda;
    [SerializeField] GameObject CamaraVenta;
    private List<int> indicesLista = new List<int>();
    private Scr_ControladorTiempo Tiempo;
    private string DiaDeHoy;
    private string ultimoDiaProcesado = "";


    [Header("Variables Venta"), Space, Space]
    [SerializeField] Scr_CreadorObjetos[] ObjetosQueVende;
    [SerializeField] GameObject[] ObjetosAvender;
    [SerializeField] TextMeshProUGUI Dinero;
    private bool bloquearClicksObjetos = false;
    private float tiempoBloqueo = 0.2f; // en segundos
    private float temporizadorBloqueo = 0f;

    private GameObject spriteActualHover = null;

    [Header("Variables Compra"), Space, Space]
    [SerializeField] Scr_CreadorObjetos[] ObjetosCompra;
    [SerializeField] Scrollbar ScrollCompra;
    [SerializeField] GameObject AreaObjetosCompra;
    [SerializeField] GameObject PrefabItemCompra;
    [SerializeField] TextMeshProUGUI DineroAPagar;
    // Guarda las cantidades seleccionadas, por índice del inventario
    private Dictionary<int, int> cantidadesSeleccionadas = new Dictionary<int, int>();

    ChecarInput Checar_input;
    PlayerInput playerInput;
    private InputAction Click;
    void Start()
    {
        Tiempo = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
        DiaDeHoy = Tiempo.DiaActual;
        ultimoDiaProcesado = PlayerPrefs.GetString("UltimoDiaGenerado", "NINGUNO");

        // 🔹 Al iniciar, si es un nuevo día o nunca se generó, generar objetos
        if (ultimoDiaProcesado != DiaDeHoy)
        {
            Debug.Log($"Nuevo día detectado ({DiaDeHoy}) al iniciar, generando objetos...");
            GenerarObjetos();
            PlayerPrefs.SetString("UltimoDiaGenerado", DiaDeHoy);
            PlayerPrefs.Save();
            ultimoDiaProcesado = DiaDeHoy;
        }
        else
        {
            Debug.Log($"Día actual sin cambios ({DiaDeHoy}), cargando objetos guardados...");
            CargarObjetosGuardados();
        }

        // 🔹 El resto de tu Start() igual
        if (Campana != null && Campana.GetComponent<Collider>() == null)
            Campana.AddComponent<BoxCollider>();

        var renderer = Campana != null ? Campana.GetComponent<Renderer>() : null;
        if (renderer != null)
            materialesCampana = renderer.materials;

        if (Marco != null)
            materialesMarco = Marco.GetComponent<Renderer>().materials;

        var gata = GameObject.Find("Gata");
        if (gata != null)
        {
            var child = gata.transform.GetChild(7);
            if (child != null)
                Inventario = child.GetComponent<Scr_Inventario>();
        }

        foreach (var obj in ObjetosAvender)
        {
            if (obj == null) continue;
            if (obj.transform.childCount == 0) continue;
            Transform hijo0 = obj.transform.GetChild(0);
            if (hijo0.GetComponent<Collider>() == null && hijo0.GetComponent<Collider2D>() == null)
                Debug.LogWarning($"Hijo 0 de {obj.name} no tiene collider.");
        }


        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        Checar_input = GameObject.Find("Singleton").GetComponent<ChecarInput>();

        Click = playerInput.actions["Click"];
        GenerarListaCompraDesdeInventario();
        ActualizarTextoDinero();
        ActualizarTextoDineroAPagar();
    }


    void Update()
    {
        // 🔹 Detectar si cambió el día mientras el juego está activo
        if (Tiempo != null && Tiempo.DiaActual != ultimoDiaProcesado)
        {
            Debug.Log($"Cambio de día detectado en tiempo real ({ultimoDiaProcesado} → {Tiempo.DiaActual})");
            ultimoDiaProcesado = Tiempo.DiaActual;

            // Solo regenerar si es un día permitido (opcional)
            GenerarObjetos();
            PlayerPrefs.SetString("UltimoDiaGenerado", ultimoDiaProcesado);
            PlayerPrefs.Save();
        }

        // 🔹 El resto de tu Update() sigue igual:
        if (bloquearClicksObjetos)
        {
            temporizadorBloqueo -= Time.deltaTime;
            if (temporizadorBloqueo <= 0f)
                bloquearClicksObjetos = false;
        }

        if (!bloquearClicksObjetos)
        {
            DetectarCampanaHoverYClick();
            DetectarObjetosAvenderHoverYClick();
        }

        ActualizarScrollerCompra();
    }

    ////////////////////////////////////
    ///     Funciones de venta       ///
    ////////////////////////////////////

    public void GenerarObjetos()
    {
        int i = 0;
        List<int> indicesValidos = new List<int>();

        // 🧹 Limpia descuentos viejos antes de generar
        for (int j = 0; j < ObjetosAvender.Length; j++)
        {
            PlayerPrefs.SetFloat("DescuentoSirilo" + j, 0f);
        }

        // 🧱 Generar objetos nuevos
        foreach (GameObject Objeto in ObjetosAvender)
        {
            if (Objeto == null)
            {
                i++;
                continue;
            }

            int r = Random.Range(0, ObjetosQueVende.Length);
            int rcant = Random.Range(1, 11);

            PlayerPrefs.SetString("ObjetosSirilo" + i, ObjetosQueVende[r].Nombre);
            PlayerPrefs.SetInt("CantidadesSirilo" + i, rcant);
            PlayerPrefs.SetInt("VendidoSirilo" + i, 0);
            // Reiniciamos descuento explícitamente
            PlayerPrefs.SetFloat("DescuentoSirilo" + i, 0f);

            var spriteR = Objeto.GetComponent<SpriteRenderer>();
            if (spriteR != null)
                spriteR.sprite = ObjetosQueVende[r].Icono;

            Transform UI = Objeto.transform.GetChild(0).GetChild(0);

            UI.GetChild(0).GetComponent<TextMeshProUGUI>().text = ObjetosQueVende[r].Nombre;
            UI.GetChild(1).GetComponent<TextMeshProUGUI>().text = rcant.ToString();

            float precioTotal = ObjetosQueVende[r].ValorVentaIndividual * rcant;
            UI.GetChild(3).GetComponent<TextMeshProUGUI>().text = "$" + precioTotal.ToString();

            // Si el precio es razonable, es candidato a descuento
            if (precioTotal > 1f)
                indicesValidos.Add(i);

            // 🔒 Ocultar cartel de oferta por defecto
            Transform oferta = UI.Find("Oferta");
            if (oferta != null)
                oferta.gameObject.SetActive(false);

            i++;
        }

        // 🎯 Elegir exactamente 2 descuentos distintos
        if (indicesValidos.Count >= 2)
        {
            int idx1 = indicesValidos[Random.Range(0, indicesValidos.Count)];
            int idx2;
            do { idx2 = indicesValidos[Random.Range(0, indicesValidos.Count)]; } while (idx2 == idx1);

            AplicarDescuentoAObjeto(idx1);
            AplicarDescuentoAObjeto(idx2);
        }
        else if (indicesValidos.Count == 1)
        {
            AplicarDescuentoAObjeto(indicesValidos[0]);
        }

        PlayerPrefs.Save();
    }


    private void AplicarDescuentoAObjeto(int index)
    {
        if (index < 0 || index >= ObjetosAvender.Length) return;

        GameObject objeto = ObjetosAvender[index];
        if (objeto == null) return;

        // UI root: Item -> MarcoVendedor -> UI Cmprar Item1
        if (objeto.transform.childCount < 1) return;
        Transform marco = objeto.transform.GetChild(0);
        if (marco.childCount < 1) return;
        Transform ui = marco.GetChild(0);

        // leer nombre y cantidad desde la UI
        if (ui.childCount < 4) return;
        string nombre = ui.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        int cantidad = 1;
        int.TryParse(ui.GetChild(1).GetComponent<TextMeshProUGUI>().text, out cantidad);

        // buscar el Scriptable correspondiente
        Scr_CreadorObjetos data = null;
        foreach (var o in ObjetosQueVende)
        {
            if (o != null && o.Nombre == nombre)
            {
                data = o;
                break;
            }
        }
        if (data == null) return;

        float precioTotal = data.ValorVentaIndividual * cantidad;
        if (precioTotal <= 1f) return; // no aplicamos descuentos si no vale la pena

        float descuento = Random.Range(10f, 50f); // porcentaje
        float factor = 1f - (descuento / 100f);
        float nuevoPrecio = Mathf.Round(data.ValorVentaIndividual * cantidad * factor);

        // Actualizar texto del precio (UI GetChild(3))
        ui.GetChild(3).GetComponent<TextMeshProUGUI>().text = "$" + nuevoPrecio.ToString();

        // Guardar descuento en PlayerPrefs (por índice)
        PlayerPrefs.SetFloat("DescuentoSirilo" + index, descuento);

        // Activar cartel Oferta dentro de la UI: buscar por índice (6) o por nombre "Oferta"
        Transform ofertaTransform = null;
        if (ui.childCount > 6)
            ofertaTransform = ui.GetChild(6);
        else
        {
            // fallback: buscar hijo llamado "Oferta"
            var t = ui.Find("Oferta");
            if (t != null) ofertaTransform = t;
        }

        if (ofertaTransform != null)
        {
            ofertaTransform.gameObject.SetActive(true);
            if (ofertaTransform.childCount > 0)
            {
                var tm = ofertaTransform.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (tm != null) tm.text = "-" + Mathf.RoundToInt(descuento) + "%";
            }
        }

        PlayerPrefs.Save();
    }




    // 🔹 NUEVA FUNCIÓN: carga los objetos guardados al iniciar
    private void CargarObjetosGuardados()
    {
        for (int i = 0; i < ObjetosAvender.Length; i++)
        {
            string keyNombre = "ObjetosSirilo" + i;
            string keyCant = "CantidadesSirilo" + i;
            string keyVendido = "VendidoSirilo" + i;
            string keyDescuento = "DescuentoSirilo" + i;

            if (PlayerPrefs.HasKey(keyNombre))
            {
                string nombre = PlayerPrefs.GetString(keyNombre);
                int cantidad = PlayerPrefs.GetInt(keyCant);
                int vendido = PlayerPrefs.GetInt(keyVendido, 0);
                float descuento = PlayerPrefs.GetFloat(keyDescuento, 0f);

                Scr_CreadorObjetos data = null;
                foreach (var obj in ObjetosQueVende)
                {
                    if (obj != null && obj.Nombre == nombre)
                    {
                        data = obj;
                        break;
                    }
                }

                if (data != null)
                {
                    var objeto = ObjetosAvender[i];
                    var spriteR = objeto.GetComponent<SpriteRenderer>();
                    if (spriteR != null)
                        spriteR.sprite = data.Icono;

                    Transform ui = objeto.transform.GetChild(0).GetChild(0);
                    ui.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.Nombre;
                    ui.GetChild(1).GetComponent<TextMeshProUGUI>().text = cantidad.ToString();

                    float precio = data.ValorVentaIndividual * cantidad;
                    if (descuento > 0f)
                        precio = Mathf.Round(precio * (1f - descuento / 100f));
                    ui.GetChild(3).GetComponent<TextMeshProUGUI>().text = "$" + precio.ToString();

                    Transform ofertaTransform = ui.Find("Oferta");
                    if (ofertaTransform != null)
                    {
                        if (descuento > 0f)
                        {
                            ofertaTransform.gameObject.SetActive(true);
                            ofertaTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                                "-" + Mathf.RoundToInt(descuento) + "%";
                        }
                        else
                        {
                            ofertaTransform.gameObject.SetActive(false);
                        }
                    }

                    if (vendido == 1)
                    {
                        spriteR.enabled = false;
                        foreach (var c in objeto.GetComponentsInChildren<Collider>(true))
                            c.enabled = false;
                        objeto.name += "_VENDIDO";
                    }
                }
            }
        }
    }



    private void ActualizarTextoDinero()
    {
        if (Dinero != null)
            Dinero.text = "$" + PlayerPrefs.GetInt("Dinero", 0);
    }

    public void ComprarObjeto(int IDObjeto)
    {
        if (ObjetosAvender == null || IDObjeto < 0 || IDObjeto >= ObjetosAvender.Length)
            return;

        if (ObjetosAvender[IDObjeto].GetComponent<SpriteRenderer>().enabled == false)
            return;

        GameObject objetoVisual = ObjetosAvender[IDObjeto];
        Transform uiRoot = objetoVisual.transform.GetChild(0).GetChild(0);

        string nombre = uiRoot.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        int cantidad = int.Parse(uiRoot.GetChild(1).GetComponent<TextMeshProUGUI>().text);

        Scr_CreadorObjetos objetoScriptable = null;
        foreach (var s in ObjetosQueVende)
        {
            if (s != null && s.Nombre == nombre)
            {
                objetoScriptable = s;
                break;
            }
        }
        if (objetoScriptable == null) return;

        float descuentoGuardado = PlayerPrefs.GetFloat("DescuentoSirilo" + IDObjeto, 0f);
        float costoTotalFloat = objetoScriptable.ValorVentaIndividual * cantidad;
        if (descuentoGuardado > 0f)
            costoTotalFloat = Mathf.Round(costoTotalFloat * (1f - descuentoGuardado / 100f));

        int costoTotal = Mathf.CeilToInt(costoTotalFloat);
        int dineroActual = PlayerPrefs.GetInt("Dinero", 0);

        if (dineroActual < costoTotal)
        {
            Debug.Log("No tienes suficiente dinero para comprar este objeto.");
            return;
        }

        dineroActual -= costoTotal;
        PlayerPrefs.SetInt("Dinero", dineroActual);
        ActualizarTextoDinero();

        PlayerPrefs.SetInt("VendidoSirilo" + IDObjeto, 1);
        PlayerPrefs.Save();

        var spr = objetoVisual.GetComponent<SpriteRenderer>();
        if (spr != null)
            spr.enabled = false;
        foreach (var c in objetoVisual.GetComponentsInChildren<Collider>(true))
            c.enabled = false;

        objetoVisual.name += "_VENDIDO";
        Debug.Log($"Compraste {cantidad} x {nombre} por ${costoTotal}.");
    }

    ////////////////////////////////////
    ///     Funciones generales      ///
    ////////////////////////////////////

    public void VolverACamaraTienda()
    {
        if (CamaraTienda != null) CamaraTienda.SetActive(true);
        if (CamaraVenta != null) CamaraVenta.SetActive(false);
        ApagarTodasLasCamarasDeVendedores();
        bloquearClicksObjetos = true;
        temporizadorBloqueo = tiempoBloqueo;
    }

    private void ActualizarScrollerCompra()
    {
        if (ScrollCompra == null || AreaObjetosCompra == null) return;
        int totalObjetos = indicesLista.Count;

        if (totalObjetos <= 8)
        {
            ScrollCompra.size = 1f;
            ScrollCompra.gameObject.SetActive(false);

            for (int i = 0; i < AreaObjetosCompra.transform.childCount; i++)
                AreaObjetosCompra.transform.GetChild(i).gameObject.SetActive(true);
            return;
        }

        ScrollCompra.gameObject.SetActive(true);
        float visibleRatio = Mathf.Clamp01((float)8 / totalObjetos);
        ScrollCompra.size = visibleRatio;

        int ocultar = Mathf.RoundToInt(ScrollCompra.value * (totalObjetos - 8));
        for (int i = 0; i < AreaObjetosCompra.transform.childCount; i++)
        {
            bool visible = (i >= ocultar && i < ocultar + 8);
            AreaObjetosCompra.transform.GetChild(i).gameObject.SetActive(visible);
        }
    }

    public void GenerarListaCompraDesdeInventario()
    {
        if (Inventario == null)
        {
            Debug.LogWarning("No se encontró el inventario de la Gata.");
            return;
        }

        for (int i = AreaObjetosCompra.transform.childCount - 1; i >= 0; i--)
            Destroy(AreaObjetosCompra.transform.GetChild(i).gameObject);

        indicesLista.Clear();

        for (int i = 0; i < Inventario.Objetos.Length; i++)
        {
            if (Inventario.Cantidades[i] > 0)
            {
                indicesLista.Add(i);
                GameObject nuevoItem = Instantiate(PrefabItemCompra, AreaObjetosCompra.transform);
                nuevoItem.transform.GetChild(0).GetComponent<Image>().sprite = Inventario.Objetos[i].Icono;
                nuevoItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Inventario.Objetos[i].Nombre;
                nuevoItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Inventario.Objetos[i].ValorVentaIndividual.ToString();
                nuevoItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "0";

                var botones = nuevoItem.GetComponentsInChildren<BotonItemTienda>();
                foreach (var boton in botones)
                {
                    boton.AsignarCantidadMaxima(Inventario.Cantidades[i]);
                    boton.AsignarIndiceInventario(i); // 🔹 Nuevo
                }

                nuevoItem.SetActive(true);
            }
        }

        ActualizarScrollerCompra();
    }

    ////////////////////////////////////
    ///   Campana Hover y Click      ///
    ////////////////////////////////////

    private void DetectarCampanaHoverYClick()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool sobreCampana = false;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && Campana != null && (hit.collider.gameObject == Campana || hit.collider.transform.IsChildOf(Campana.transform)))
            {
                sobreCampana = true;
                if (Input.GetMouseButtonDown(0))
                {
                    if (CamaraTienda != null) CamaraTienda.SetActive(false);
                    if (CamaraVenta != null) CamaraVenta.SetActive(true);
                }
            }
        }

        if (sobreCampana && !mouseSobreCampana)
        {
            mouseSobreCampana = true;
            CambiarIntensidadMateriales(5f);
        }
        else if (!sobreCampana && mouseSobreCampana)
        {
            mouseSobreCampana = false;
            CambiarIntensidadMateriales(1f);
        }
    }

    private void CambiarIntensidadMateriales(float valor)
    {
        if (materialesCampana == null || materialesMarco == null) return;
        foreach (var mat in materialesCampana)
            if (mat.HasProperty("_Intensidad")) mat.SetFloat("_Intensidad", valor);
        foreach (var mat in materialesMarco)
            if (mat.HasProperty("_Intensidad")) mat.SetFloat("_Intensidad", valor);
    }

    ////////////////////////////////////
    ///  Hover & Click en objetos    ///
    ////////////////////////////////////

    void DetectarObjetosAvenderHoverYClick()
    {
        if (ObjetosAvender == null || ObjetosAvender.Length == 0) return;
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Transform hitTransform3D = null;
        if (Physics.Raycast(ray, out RaycastHit hit3d))
            hitTransform3D = hit3d.collider != null ? hit3d.collider.transform : null;

        Vector3 mouseWorld3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseWorld2 = new Vector2(mouseWorld3.x, mouseWorld3.y);
        Collider2D hit2d = Physics2D.OverlapPoint(mouseWorld2);

        GameObject nuevoHover = null;

        foreach (GameObject obj in ObjetosAvender)
        {
            if (obj == null) continue;
            if (obj.transform.childCount == 0) continue;

            Transform hijo0 = obj.transform.GetChild(0);

            bool esteEsHover = false;
            if (hitTransform3D != null && (hitTransform3D == hijo0 || hitTransform3D.IsChildOf(hijo0)))
                esteEsHover = true;
            if (!esteEsHover && hit2d != null && (hit2d.transform == hijo0 || hit2d.transform.IsChildOf(hijo0)))
                esteEsHover = true;

            bool camActiva = obj.transform.childCount > 1 && obj.transform.GetChild(1).gameObject.activeSelf;

            // Si la cámara está activa, mantener blanco y saltar hover
            if (camActiva)
            {
                CambiarColorSprite(obj, Color.white);
                continue;
            }

            if (esteEsHover)
            {
                nuevoHover = obj;
                break;
            }
        }

        // Cambio de hover
        if (nuevoHover != spriteActualHover)
        {
            if (spriteActualHover != null)
            {
                bool camActiva = spriteActualHover.transform.childCount > 1 && spriteActualHover.transform.GetChild(1).gameObject.activeSelf;
                CambiarIntensidadYColor(spriteActualHover, 1f, camActiva ? Color.white : Color.grey);
            }

            if (nuevoHover != null)
                CambiarIntensidadYColor(nuevoHover, 5f, Color.white);

            spriteActualHover = nuevoHover;
        }

        // Click izquierdo
        if (Click.WasPressedThisFrame() && spriteActualHover != null && CamaraTienda.gameObject.activeSelf)
        {
            Debug.Log("entra2");
            //ApagarTodasLasCamarasDeVendedores();

            if (spriteActualHover.transform.childCount > 1)
            {
                GameObject camObj = spriteActualHover.transform.GetChild(1).gameObject;
                if (camObj != null) camObj.SetActive(true);
            }

            CambiarColorSprite(spriteActualHover, Color.white);
            if (CamaraTienda != null)
                CamaraTienda.SetActive(false);
        }
    }

    void CambiarIntensidadYColor(GameObject objeto, float intensidad, Color colorSprite)
    {
        if (objeto == null || objeto.transform.childCount == 0) return;
        Transform hijo0 = objeto.transform.GetChild(0);
        Renderer rend = hijo0.GetComponent<Renderer>();
        if (rend != null)
        {
            Material[] mats = rend.materials;
            foreach (Material mat in mats)
            {
                if (mat == null) continue;
                if (mat.HasProperty("_Intensidad"))
                    mat.SetFloat("_Intensidad", intensidad);
                else if (mat.HasProperty("_Color"))
                {
                    float t = Mathf.Clamp01((intensidad - 1f) / 4f);
                    Color baseColor = mat.GetColor("_Color");
                    Color target = Color.Lerp(baseColor * 0.6f, baseColor, t);
                    mat.SetColor("_Color", target);
                }
            }
            rend.materials = mats;
        }

        CambiarColorSprite(objeto, colorSprite);
    }

    void CambiarColorSprite(GameObject objeto, Color color)
    {
        SpriteRenderer spr = objeto.GetComponent<SpriteRenderer>();
        if (spr != null)
            spr.color = color;
    }

    public void ApagarTodasLasCamarasDeVendedores()
    {
        if (ObjetosAvender == null) return;
        foreach (GameObject obj in ObjetosAvender)
        {
            if (obj == null) continue;
            if (obj.transform.childCount > 1)
            {
                GameObject camObj = obj.transform.GetChild(1).gameObject;
                if (camObj != null)
                    camObj.SetActive(false);
            }
            CambiarColorSprite(obj, Color.grey);
        }

        if (CamaraTienda != null)
            CamaraTienda.SetActive(true);
    }

    ////////////////////////////////////
    ///     Funciones de compra      ///
    ////////////////////////////////////

    public void LimpiarCompra()
    {
        if (DineroAPagar.text != "$0")
        {
            DineroAPagar.text = "$0";
            foreach (Transform Hijo in AreaObjetosCompra.GetComponentInChildren<Transform>())
                Hijo.GetChild(3).GetComponent<TextMeshProUGUI>().text = "0";
        }
    }

    public void RecalcularDineroAPagar()
    {
        float total = 0f;
        foreach (var kvp in cantidadesSeleccionadas)
        {
            var objeto = Inventario.Objetos[kvp.Key];
            total += objeto.ValorVentaIndividual * kvp.Value;
        }
        DineroAPagar.text = "$" + total.ToString("F0");
    }


    public void ComprarObjetosSeleccionados()
    {
        if (Inventario == null) return;
        float TotalCompra = 0f;

        foreach (var kvp in cantidadesSeleccionadas)
        {
            int indice = kvp.Key;
            int cantidadAComprar = kvp.Value;
            if (cantidadAComprar <= 0) continue;

            var objeto = Inventario.Objetos[indice];
            float precioUnitario = objeto.ValorVentaIndividual;

            Inventario.Cantidades[indice] -= cantidadAComprar;
            if (Inventario.Cantidades[indice] < 0)
                Inventario.Cantidades[indice] = 0;

            TotalCompra += precioUnitario * cantidadAComprar;
        }

        // Aplicar dinero
        PlayerPrefs.SetInt("Dinero", PlayerPrefs.GetInt("Dinero", 0) + Mathf.FloorToInt(TotalCompra));
        PlayerPrefs.Save();
        ActualizarTextoDinero();

        // Limpiar registro de cantidades seleccionadas
        cantidadesSeleccionadas.Clear();

        // Refrescar UI
        GenerarListaCompraDesdeInventario();
        ActualizarTextoDineroAPagar();
    }


    private void ActualizarTextoDineroAPagar()
    {
        if (DineroAPagar != null)
            DineroAPagar.text = "$0";
    }

    // 🔹 Registra o actualiza la cantidad seleccionada para un ítem específico del inventario
    public void ActualizarCantidadSeleccionada(int indiceInventario, int nuevaCantidad)
    {
        if (cantidadesSeleccionadas.ContainsKey(indiceInventario))
            cantidadesSeleccionadas[indiceInventario] = nuevaCantidad;
        else
            cantidadesSeleccionadas.Add(indiceInventario, nuevaCantidad);
    }
}
