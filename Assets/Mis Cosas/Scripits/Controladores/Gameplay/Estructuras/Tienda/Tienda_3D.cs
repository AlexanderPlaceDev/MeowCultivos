using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    private float DineroCompraActual = 0f;

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
            PlayerPrefs.SetInt("VendidoSirilo" + i, 0); // 🔹 nuevo campo para estado de venta

            var spriteR = Objeto.GetComponent<SpriteRenderer>();
            if (spriteR != null && ObjetosQueVende.Length > 0)
                spriteR.sprite = ObjetosQueVende[r].Icono;

            if (Objeto.transform.GetChild(0).GetChild(0).childCount > 0)
            {
                Transform UI = Objeto.transform.GetChild(0).GetChild(0);
                if (UI.childCount > 0)
                {
                    var t = UI.GetChild(0);
                    if (t != null && t.GetComponent<TextMeshProUGUI>() != null)
                        t.GetComponent<TextMeshProUGUI>().text = ObjetosQueVende[r].Nombre;
                }

                if (UI.childCount > 1)
                {
                    var t1 = UI.GetChild(1);
                    var text1 = t1.GetComponent<TextMeshProUGUI>();
                    if (text1 != null) text1.text = rcant.ToString();
                }

                if (UI.childCount > 3)
                {
                    var t3 = UI.GetChild(3);
                    var text3 = t3.GetComponent<TextMeshProUGUI>();
                    if (text3 != null)
                        text3.text = "$" + (ObjetosQueVende[r].ValorVentaIndividual * rcant).ToString();
                }
            }
            i++;
        }

        PlayerPrefs.Save(); // 🔹 guardar todo
    }

    // 🔹 NUEVA FUNCIÓN: carga los objetos guardados al iniciar
    private void CargarObjetosGuardados()
    {
        for (int i = 0; i < ObjetosAvender.Length; i++)
        {
            string keyNombre = "ObjetosSirilo" + i;
            string keyCant = "CantidadesSirilo" + i;
            string keyVendido = "VendidoSirilo" + i;

            if (PlayerPrefs.HasKey(keyNombre))
            {
                string nombre = PlayerPrefs.GetString(keyNombre);
                int cantidad = PlayerPrefs.GetInt(keyCant);
                int vendido = PlayerPrefs.GetInt(keyVendido, 0);

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

                    // Actualizar textos UI
                    Transform ui = objeto.transform.GetChild(0).GetChild(0);
                    ui.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.Nombre;
                    ui.GetChild(1).GetComponent<TextMeshProUGUI>().text = cantidad.ToString();
                    ui.GetChild(3).GetComponent<TextMeshProUGUI>().text = "$" + (data.ValorVentaIndividual * cantidad);

                    // Si ya estaba vendido, desactivar visualmente
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
        {
            Debug.LogWarning("IDObjeto fuera de rango o ObjetosAvender no inicializado.");
            return;
        }

        if (ObjetosAvender[IDObjeto].GetComponent<SpriteRenderer>().enabled == false)
        {
            return;
        }

        GameObject objetoVisual = ObjetosAvender[IDObjeto];
        if (objetoVisual == null)
        {
            Debug.LogWarning("Objeto visual nulo en ObjetosAvender.");
            return;
        }

        // 🔹 Extraer datos del objeto (como ya tenías)
        Transform uiRoot = null;
        if (objetoVisual.transform.childCount > 0)
        {
            uiRoot = objetoVisual.transform.GetChild(0);
            if (uiRoot.childCount > 0)
                uiRoot = uiRoot.GetChild(0);
        }

        string nombre = null;
        int cantidad = 1;
        float precioUnitario = -1f;

        if (uiRoot != null)
        {
            if (uiRoot.childCount > 0)
            {
                var t = uiRoot.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (t != null) nombre = t.text;
            }

            if (uiRoot.childCount > 1)
            {
                var tq = uiRoot.GetChild(1).GetComponent<TextMeshProUGUI>();
                if (tq != null) int.TryParse(tq.text, out cantidad);
            }
        }

        Scr_CreadorObjetos objetoScriptable = null;
        foreach (var s in ObjetosQueVende)
        {
            if (s != null && s.Nombre == nombre)
            {
                objetoScriptable = s;
                break;
            }
        }

        if (objetoScriptable == null)
        {
            Debug.LogWarning("No se encontró el objeto en la lista de venta.");
            return;
        }

        float costoTotalFloat = objetoScriptable.ValorVentaIndividual * cantidad;
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

        // Agregar al inventario o lista
        var goOA = GameObject.Find("ObjetosAgregados");
        if (goOA != null)
        {
            Scr_ObjetosAgregados objetosAgregados = goOA.GetComponent<Scr_ObjetosAgregados>();
            if (objetosAgregados != null)
            {
                objetosAgregados.Lista.Add(objetoScriptable);
                objetosAgregados.Cantidades.Add(cantidad);
            }
        }

        // 🔹 Guardar estado vendido
        PlayerPrefs.SetInt("VendidoSirilo" + IDObjeto, 1);
        PlayerPrefs.Save();

        // 🔹 Desactivar visualmente el objeto
        var spr = objetoVisual.GetComponent<SpriteRenderer>();
        if (spr != null)
            spr.enabled = false;
        foreach (var c in objetoVisual.GetComponentsInChildren<Collider>(true))
            c.enabled = false;

        objetoVisual.name += "_VENDIDO";
        Debug.Log($"Compraste {cantidad} x {nombre} por ${costoTotal}.");

        VolverACamaraTienda();
        GenerarListaCompraDesdeInventario();
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
                    boton.AsignarCantidadMaxima(Inventario.Cantidades[i]);

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
        if (Input.GetMouseButtonDown(0) && spriteActualHover != null && CamaraTienda.gameObject.activeSelf)
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
        if (DineroAPagar == null) return;
        float total = 0f;
        for (int i = 0; i < AreaObjetosCompra.transform.childCount; i++)
        {
            Transform item = AreaObjetosCompra.transform.GetChild(i);
            if (!item.gameObject.activeSelf) continue;
            float valorUnitario = 0f;
            float.TryParse(item.GetChild(2).GetComponent<TextMeshProUGUI>().text, out valorUnitario);
            int cantidadSeleccionada = 0;
            int.TryParse(item.GetChild(3).GetComponent<TextMeshProUGUI>().text, out cantidadSeleccionada);
            total += valorUnitario * cantidadSeleccionada;
        }
        DineroAPagar.text = "$" + total.ToString();
    }

    public void ComprarObjetosSeleccionados()
    {
        if (Inventario == null || DineroAPagar == null) return;
        float TotalCompra = 0f;

        for (int i = 0; i < AreaObjetosCompra.transform.childCount; i++)
        {
            Transform item = AreaObjetosCompra.transform.GetChild(i);
            if (!item.gameObject.activeSelf) continue;

            string nombre = item.GetChild(1).GetComponent<TextMeshProUGUI>().text;
            float precioUnitario = 0f;
            float.TryParse(item.GetChild(2).GetComponent<TextMeshProUGUI>().text, out precioUnitario);
            int cantidadAComprar = 0;
            int.TryParse(item.GetChild(3).GetComponent<TextMeshProUGUI>().text, out cantidadAComprar);

            if (cantidadAComprar <= 0) continue;

            for (int j = 0; j < Inventario.Objetos.Length; j++)
            {
                if (Inventario.Objetos[j].Nombre == nombre)
                {
                    Inventario.Cantidades[j] -= cantidadAComprar;
                    if (Inventario.Cantidades[j] < 0)
                        Inventario.Cantidades[j] = 0;
                    TotalCompra += precioUnitario * cantidadAComprar;
                    break;
                }
            }

            item.GetChild(3).GetComponent<TextMeshProUGUI>().text = "0";
        }

        DineroCompraActual += TotalCompra;
        PlayerPrefs.SetInt("Dinero", PlayerPrefs.GetInt("Dinero", 0) + (int)DineroCompraActual);
        ActualizarTextoDinero();
        ActualizarTextoDineroAPagar();
        GenerarListaCompraDesdeInventario();
    }

    private void ActualizarTextoDineroAPagar()
    {
        if (DineroAPagar != null)
            DineroAPagar.text = "$0";
    }
}
