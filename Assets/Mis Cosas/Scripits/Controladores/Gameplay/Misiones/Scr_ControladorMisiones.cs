using PrimeTween;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Scr_CreadorMisiones;

public class Scr_ControladorMisiones : MonoBehaviour
{
    [Header("UI Misiones")]
    [SerializeField] private GameObject UIBase;
    [SerializeField] private Sprite[] IconosUIBase;
    private GameObject UIForma2;
    private GameObject UIForma3;
    private int FormaActual = 1;
    private int FormaAnterior = 2;
    private bool CambiandoDeForma = false;
    [SerializeField] private Image ImagenIconoMision;
    [SerializeField] private TextMeshProUGUI DescripcionForma2;
    [SerializeField] private Color[] ColoresDrescripcionForma2;
    [SerializeField] private TextMeshProUGUI DescripcionForma3;
    [SerializeField] private TextMeshProUGUI DescripcionForma3Falsa;
    [SerializeField] private TextMeshProUGUI TextoPagina;
    [SerializeField] private GameObject Basura;
    [SerializeField] private GameObject Estrella;
    [SerializeField] private float velocidadParpadeo = 2f;
    [SerializeField] private GameObject Cambio;
    [SerializeField] private GameObject IconoProgreso;
    [SerializeField] private TextMeshProUGUI TextoPersonaje;
    [SerializeField] private TextMeshProUGUI TextoLugar;
    [SerializeField] private TextMeshProUGUI TextoNombreMision;
    [SerializeField] private GameObject TextoNoAplica;

    [Header("Estados de las misiones")]
    public Scr_CreadorMisiones MisionActual;          // Misión que se está mostrando actualmente en el panel
    public List<Scr_CreadorMisiones> Misiones;   // Lista de misiones activas
    public List<bool> MisionesCompletas;       // Estado de todas las misiones
    public List<bool> MisionesVistas; // false = nueva, true = ya vista
    public Scr_CreadorMisiones[] TodasLasMisiones;    // Todas las misiones posibles en el juego



    public bool EstaEnDialogo = false;

    [Space, Header("Control de objetos")]
    [SerializeField] private GameObject BotonesUI;
    [SerializeField] private GameObject ObjetosRecoleccion;
    private Scr_Inventario Inventario;
    private Transform Gata;
    public int PaginaActual = 0;

    [Space, Header("Teclas")]
    [SerializeField] GameObject TeclaMisiones;
    [SerializeField] private Sprite IconoTeclaMisiones;
    [SerializeField] private Sprite[] PalancasIcono;

    private bool[] TeclasPresionadas;
    private float[] TiempoTeclas;

    private bool ultimogamepad = false;
    PlayerInput playerInput;
    InputIconProvider IconProvider;
    private InputAction InputTeclaMisiones;
    private InputAction InputTeclaBorrarMision;
    private InputAction InputTeclaCambiarMision;
    private string TextoTeclaMisiones = "";


    [Space, Header("Progresos")]
    public List<string> EnemigosCazados;      // Nombres de los enemigos eliminados
    public List<int> CantidadCazados;         // Cantidad cazada de cada enemigo
    public List<string> LugaresExplorados;    // Lista de lugares explorados

    [Space, Header("Objetos de misiones para construccion")]
    public GameObject[] TodasLasConstrucciones;

    // ================================
    // === MÉTODOS UNITY ===
    // ================================
    private InputAction MoverHorizontal;
    private InputAction MoverVertical;
    private bool[] DireccionesCompletadas;

    //Rutinas
    private Coroutine rutinaTextoNuevo;
    private Coroutine rutinaIconoBase;
    private Coroutine rutinaParpadeo;

    void Awake()
    {
        // Buscar referencias importantes
        Gata = GameObject.Find("Gata").transform;
        Inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
        Inventario.OnInventarioActualizado += OnInventarioCambiado;
        TodasLasConstrucciones = Buscartag.BuscarObjetosConTagInclusoInactivos("Construcciones");

        //Inputs (Teclado y control)
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        MoverHorizontal = playerInput.actions["MoverHorizontal"];
        MoverVertical = playerInput.actions["MoverVertical"];
        InputTeclaMisiones = playerInput.actions["Mapa"];
        InputTeclaCambiarMision = playerInput.actions["CambiarMision"];
        InputTeclaBorrarMision = playerInput.actions["BorrarMision"];

        //asignamos las formas
        UIForma2 = UIBase.transform.GetChild(0).gameObject;
        UIForma3 = UIBase.transform.GetChild(1).gameObject;

        CargarMisiones();
        ProcesarEnemigosDelSingleton();


    }

    private void Start()
    {
        RevisarProgresoMisiones();
        ActualizarUI();
    }

    private void OnInventarioCambiado()
    {
        // Solo revisar si hay misiones de recolección en curso
        if (MisionActual != null && MisionActual.Tipo == Tipos.Recoleccion)
        {
            RevisarProgresoMisiones(); // Esto también actualiza la UI
        }
    }

    private void OnDestroy()
    {
        if (Inventario != null)
            Inventario.OnInventarioActualizado -= OnInventarioCambiado;
    }

    void Update()
    {
        //Cambiar forma UI
        if (InputTeclaMisiones.WasPressedThisFrame() && CambiandoDeForma == false)
        {

            StartCoroutine(CambiarForma());
        }

        //Control Parpadeo
        ControlParpadeoEstrella();

        //Control Cambio
        if (FormaActual == 3 && Misiones.Count > 1)
        {
            Cambio.SetActive(true);
        }
        else
        {
            Cambio.SetActive(false);
        }

        //Aqui va el codigo para borrar la mision secundaria
        if (FormaActual == 3 && InputTeclaBorrarMision.WasPerformedThisFrame() && !CambiandoDeForma)
        {
            if (MisionActual != null && MisionActual.prioridad == prioridadM.Secundaria)
            {
                int index = Misiones.IndexOf(MisionActual);

                if (index >= 0)
                {
                    // 🔥 BORRAR PROGRESO DE CAZA
                    if (MisionActual.ObjetivosACazar != null)
                    {
                        for (int i = 0; i < MisionActual.ObjetivosACazar.Length; i++)
                        {
                            string clave = $"{MisionActual.name}_CantidadCazados_{i}";
                            if (PlayerPrefs.HasKey(clave))
                            {
                                PlayerPrefs.DeleteKey(clave);
                            }
                        }
                    }

                    Misiones.RemoveAt(index);
                    MisionesCompletas.RemoveAt(index);
                    MisionesVistas.RemoveAt(index);

                    if (Misiones.Count > 0)
                    {
                        PaginaActual = Mathf.Clamp(index, 0, Misiones.Count - 1);
                        MisionActual = Misiones[PaginaActual];
                    }
                    else
                    {
                        MisionActual = null;
                        PaginaActual = 0;
                    }

                    ActualizarUI();
                }
            }
        }
        //Aqui va el codigo para cambiar de mision
        if (FormaActual == 3 && InputTeclaCambiarMision.WasPerformedThisFrame() && !CambiandoDeForma)
        {
            if (Misiones.Count > 1)
            {
                PaginaActual++;

                if (PaginaActual >= Misiones.Count)
                    PaginaActual = 0;

                MisionActual = Misiones[PaginaActual];

                ActualizarUI();
            }
        }

        //[AQUI SE DEBE COMBINAR LAS MISIONES DE TECLAS Y MOVIMIENTO EN UNA SOLA]
        if (MisionActual != null && MisionActual.Tipo == Tipos.Movimiento)
        {
            if (EstaEnDialogo)
            {
                BotonesUI.SetActive(false);
            }
            else
            {
                ChecarImagenMovimiento();
                ProcesarMisionMovimiento();
                BotonesUI.SetActive(!MisionesCompletas[PaginaActual]);
            }
        }
        else if (MisionActual != null && MisionActual.Tipo == Tipos.Teclas)
        {
            if (EstaEnDialogo)
            {
                BotonesUI.SetActive(false);
            }
            else
            {
                ProcesarMisionTeclas();
                BotonesUI.SetActive(!MisionesCompletas[PaginaActual]);
            }
        }

        IconProvider.ActualizarIconoUI(InputTeclaMisiones, TeclaMisiones.transform, ref IconoTeclaMisiones, ref TextoTeclaMisiones, false);
    }



    IEnumerator CambiarForma()
    {
        DetenerEfectosUI();
        CambiandoDeForma = true;

        switch (FormaActual)
        {
            case 1:
                FormaActual = 2;
                FormaAnterior = 1;
                ActualizarUI();
                Tween.LocalScale(UIBase.transform, Vector3.zero, 0.2f);
                yield return new WaitForSeconds(0.2f);
                UIBase.GetComponent<Image>().enabled = false;
                Tween.LocalScale(UIBase.transform, Vector3.one * 5, 0.01f);
                Tween.LocalScale(UIForma2.transform, Vector3.one, 0.2f);
                yield return new WaitForSeconds(0.2f);
                break;

            case 2:


                if (FormaAnterior == 1)
                {
                    FormaActual = 3;
                    FormaAnterior = 2;
                    ActualizarUI();
                    UIForma2.SetActive(false);
                    UIForma3.SetActive(true);
                    UIForma3.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = UIForma2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    Tween.LocalScaleY(UIForma3.transform.GetChild(2), 0, 0.3f);
                    yield return new WaitForSeconds(0.3f);
                    Tween.LocalScaleY(UIForma3.transform.GetChild(3), 1, 0.4f);
                    Tween.UIAnchoredPositionY(UIForma3.transform.GetChild(1).GetComponent<RectTransform>(), -157, 0.4f);
                    yield return new WaitForSeconds(0.4f);
                    break;
                }
                else
                {
                    FormaActual = 1;
                    FormaAnterior = 2;
                    ActualizarUI();
                    Tween.LocalScale(UIForma2.transform, Vector3.zero, 0.2f);
                    Tween.LocalScale(UIBase.transform, Vector3.zero, 0.2f);
                    yield return new WaitForSeconds(0.2f);
                    UIBase.GetComponent<Image>().enabled = true;
                    Tween.LocalScale(UIBase.transform, Vector3.one * 5, 0.2f);
                    yield return new WaitForSeconds(0.2f);
                    break;
                }



            case 3:
                FormaActual = 2;
                FormaAnterior = 3;

                // 🔥 FORZAR TEXTO ANTES DE ANIMAR
                if (MisionActual != null)
                {
                    string texto = MisionesCompletas[PaginaActual]
                        ? MisionActual.DescripcionMisionCompleta
                        : MisionActual.DescripcionEnMision;
                    Debug.Log("Cambia 1");
                    DescripcionForma2.text = texto;
                    DescripcionForma3Falsa.text = texto;
                }

                // 🔥 OPCIONAL: detener cualquier coroutine que esté tocando texto
                DetenerEfectosUI();

                Tween.LocalScaleY(UIForma3.transform.GetChild(3), 0, 0.4f);
                Tween.UIAnchoredPositionY(UIForma3.transform.GetChild(1).GetComponent<RectTransform>(), -6, 0.4f);
                yield return new WaitForSeconds(0.4f);

                Tween.LocalScaleY(UIForma3.transform.GetChild(2), 1, 0.3f);
                yield return new WaitForSeconds(0.3f);

                UIForma2.SetActive(true);
                UIForma3.SetActive(false);

                ActualizarUI();
                break;
        }

        CambiandoDeForma = false;
    }


    // ================================
    // === MÉTODOS DE LA UI ===
    // ================================
    public void ActualizarUI()
    {

        if (MisionActual != null)
        {
            DetenerEfectosUI();

            //Forzar sincronizacion
            int indexReal = Misiones.IndexOf(MisionActual);
            if (indexReal != -1)
            {
                PaginaActual = indexReal;
            }


            switch (FormaActual)
            {
                case 1:
                    {
                        if (HayMisionesNoVistas())
                        {
                            if (rutinaIconoBase == null)
                                rutinaIconoBase = StartCoroutine(IntercalarIconoBase());
                        }
                        else
                        {
                            if (rutinaIconoBase != null)
                            {
                                StopCoroutine(rutinaIconoBase);
                                rutinaIconoBase = null;
                            }

                            UIBase.GetComponent<Image>().sprite = IconosUIBase[0];
                        }
                        break;
                    }
                case 2:
                    {
                        bool hayNueva = MisionesVistas.Contains(false);

                        // 🔥 limpiar siempre
                        if (rutinaTextoNuevo != null)
                        {
                            StopCoroutine(rutinaTextoNuevo);
                            rutinaTextoNuevo = null;
                        }

                        if (hayNueva)
                        {
                            rutinaTextoNuevo = StartCoroutine(IntercalarTextoNueva());
                        }
                        else
                        {
                            DescripcionForma2.color = ColoresDrescripcionForma2[0];

                            Debug.Log("Cambia 2");
                            DescripcionForma2.text = MisionesCompletas[PaginaActual]
                                ? MisionActual.DescripcionMisionCompleta
                                : MisionActual.DescripcionEnMision;

                            DescripcionForma3Falsa.text = MisionesCompletas[PaginaActual]
                                ? MisionActual.DescripcionMisionCompleta
                                : MisionActual.DescripcionEnMision;
                        }
                        break;
                    }
                case 3:
                    {

                        // 🔥 ICONO PROGRESO (Forma 3)

                        bool MostrarIcono = false;

                        Debug.Log("cant misiones:" + Misiones.Count);
                        if (Misiones.Count > 1)
                        {
                            for (int i = 0; i < MisionesVistas.Count; i++)
                            {
                                // Si hay una misión NO vista y NO es la actual
                                if (!MisionesVistas[i] && i != PaginaActual)
                                {
                                    MostrarIcono = true;
                                    break;
                                }
                            }
                        }
                        IconoProgreso.SetActive(MostrarIcono);

                        // Marcar como vista
                        if (!MisionesVistas[PaginaActual])
                        {
                            MisionesVistas[PaginaActual] = true;
                            GuardarMisiones();

                            // 🔥 DETENER EFECTOS DE "NUEVA"
                            if (rutinaTextoNuevo != null)
                            {
                                StopCoroutine(rutinaTextoNuevo);
                                rutinaTextoNuevo = null;
                            }

                            if (rutinaIconoBase != null)
                            {
                                StopCoroutine(rutinaIconoBase);
                                rutinaIconoBase = null;
                            }

                            // Reset visual inmediato
                            UIBase.GetComponent<Image>().sprite = IconosUIBase[0];
                            Debug.Log("Cambia 3");
                            DescripcionForma2.color = ColoresDrescripcionForma2[0];
                            DescripcionForma2.text = MisionesCompletas[PaginaActual]
                                ? MisionActual.DescripcionMisionCompleta
                                : MisionActual.DescripcionEnMision;

                            DescripcionForma3Falsa.text = MisionesCompletas[PaginaActual]
                                ? MisionActual.DescripcionMisionCompleta
                                : MisionActual.DescripcionEnMision;
                        }

                        ImagenIconoMision.sprite = MisionActual.LogoMision;
                        if (MisionActual.prioridad == prioridadM.Principal)
                        {
                            Estrella.SetActive(true);
                            Basura.SetActive(false);
                        }
                        else
                        {
                            Estrella.SetActive(false);
                            Basura.SetActive(true);
                        }
                        TextoPersonaje.text = MisionActual.Personaje;
                        TextoLugar.text = MisionActual.LugarMision;
                        TextoPagina.text = (PaginaActual + 1) + "/" + Misiones.Count;
                        TextoNombreMision.text = MisionActual.TituloMision;

                        if (MisionesCompletas[PaginaActual])
                        {
                            DescripcionForma3.text = MisionActual.DescripcionMisionCompleta;
                        }
                        else
                        {
                            DescripcionForma3.text = MisionActual.DescripcionEnMision;
                        }
                        break;
                    }

            }



            // Oculta el panel si no es caza ni recolección
            if (MisionActual.Tipo != Tipos.Caza && MisionActual.Tipo != Tipos.Recoleccion)
            {
                ObjetosRecoleccion.SetActive(false);
                TextoNoAplica.SetActive(true);
                return; // Ya que no hay más que mostrar
            }

            // Ocultar todos los hijos
            for (int i = 0; i < ObjetosRecoleccion.transform.childCount; i++)
                ObjetosRecoleccion.transform.GetChild(i).gameObject.SetActive(false);

            ObjetosRecoleccion.SetActive(true);
            TextoNoAplica.SetActive(false);

            int N = 0;

            // RECOLECCIÓN
            if (MisionActual.ObjetosNecesarios != null)
            {
                foreach (Scr_CreadorObjetos Item in MisionActual.ObjetosNecesarios)
                {
                    if (N >= ObjetosRecoleccion.transform.childCount) break;

                    Transform hijo = ObjetosRecoleccion.transform.GetChild(N);
                    hijo.gameObject.SetActive(true);
                    hijo.GetComponent<Image>().sprite = Item.Icono;
                    hijo.GetChild(0).GetComponent<TextMeshProUGUI>().text = Item.Nombre;

                    int cantidadEnInventario = 0;
                    int index = Array.IndexOf(Inventario.Objetos, Item);
                    if (index >= 0) cantidadEnInventario = Inventario.Cantidades[index];

                    hijo.GetChild(1).GetComponent<TextMeshProUGUI>().text = cantidadEnInventario.ToString();
                    hijo.GetChild(3).GetComponent<TextMeshProUGUI>().text = MisionActual.CantidadesQuita[N].ToString();
                    N++;
                }
            }

            // CAZA
            if (MisionActual.ObjetivosACazar.Length > 0)
            {
                N = 0;
                foreach (string Enemigo in MisionActual.ObjetivosACazar)
                {
                    if (N >= ObjetosRecoleccion.transform.childCount) break;

                    Transform hijo = ObjetosRecoleccion.transform.GetChild(N);
                    hijo.gameObject.SetActive(true);
                    hijo.GetComponent<Image>().sprite = MisionActual.IconosACazar[N];
                    hijo.GetChild(0).GetComponent<TextMeshProUGUI>().text = Enemigo;

                    string clave = $"{MisionActual.name}_CantidadCazados_{N}";
                    int cantidadCazada = PlayerPrefs.GetInt(clave, 0);

                    hijo.GetChild(1).GetComponent<TextMeshProUGUI>().text = cantidadCazada.ToString();
                    hijo.GetChild(3).GetComponent<TextMeshProUGUI>().text = MisionActual.CantidadACazar[N].ToString();
                    N++;
                }
            }
        }

    }

    void ControlParpadeoEstrella()
    {
        bool debeParpadear = FormaActual == 3
            && MisionActual != null
            && MisionActual.prioridad == prioridadM.Principal;

        if (debeParpadear && rutinaParpadeo == null)
        {
            rutinaParpadeo = StartCoroutine(ParpadeoEstrella());
        }
        else if (!debeParpadear && rutinaParpadeo != null)
        {
            StopCoroutine(rutinaParpadeo);
            rutinaParpadeo = null;

            ResetAlphaEstrella();
        }
    }

    IEnumerator ParpadeoEstrella()
    {
        Image img = Estrella.GetComponent<Image>();
        TextMeshProUGUI tmp = Estrella.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * velocidadParpadeo;

            float alpha = Mathf.PingPong(t, 1f); // 0 → 1 → 0 → 1...

            if (img != null)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }

            if (tmp != null)
            {
                Color c = tmp.color;
                c.a = alpha;
                tmp.color = c;
            }

            yield return null;
        }
    }

    void ResetAlphaEstrella()
    {
        Image img = Estrella.GetComponent<Image>();
        TextMeshProUGUI tmp = Estrella.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (img != null)
        {
            Color c = img.color;
            c.a = 1f;
            img.color = c;
        }

        if (tmp != null)
        {
            Color c = tmp.color;
            c.a = 1f;
            tmp.color = c;
        }
    }

    bool HayMisionesNoVistas()
    {
        for (int i = 0; i < MisionesVistas.Count; i++)
        {
            if (!MisionesVistas[i])
                return true;
        }
        return false;
    }

    IEnumerator IntercalarTextoNueva()
    {
        while (true)
        {
            // 🔥 Si ya no hay nuevas → salir
            if (!MisionesVistas.Contains(false))
            {
                rutinaTextoNuevo = null;
                yield break;
            }

            // 🔥 Si ya no estamos en forma 2 → salir
            if (FormaActual != 2)
            {
                rutinaTextoNuevo = null;
                yield break;
            }

            // 🔁 Mostrar "Nueva Misión"
            DescripcionForma2.color = ColoresDrescripcionForma2[1];
            DescripcionForma2.text = "Nueva Mision";
            yield return new WaitForSeconds(1f);

            // 🔁 Mostrar descripción de la MISIÓN ACTUAL (no otra)
            DescripcionForma2.color = ColoresDrescripcionForma2[0];

            Debug.Log("Cambia 4");
            DescripcionForma2.text = MisionesCompletas[PaginaActual]
                ? MisionActual.DescripcionMisionCompleta
                : MisionActual.DescripcionEnMision;

            DescripcionForma3Falsa.text = MisionesCompletas[PaginaActual]
                ? MisionActual.DescripcionMisionCompleta
                : MisionActual.DescripcionEnMision;

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator IntercalarIconoBase()
    {
        Image img = UIBase.GetComponent<Image>();

        while (true)
        {
            img.sprite = IconosUIBase[1]; // nueva
            yield return new WaitForSeconds(0.8f);

            img.sprite = IconosUIBase[0]; // normal
            yield return new WaitForSeconds(0.8f);
        }
    }

    void DetenerEfectosUI()
    {
        if (rutinaTextoNuevo != null)
        {
            StopCoroutine(rutinaTextoNuevo);
            rutinaTextoNuevo = null;
        }

        if (rutinaIconoBase != null)
        {
            StopCoroutine(rutinaIconoBase);
            rutinaIconoBase = null;
        }

        // 🔥 FIX: restaurar texto inmediatamente
        if (MisionActual != null)
        {
            Debug.Log("Cambia 5");
            DescripcionForma2.text = MisionesCompletas[PaginaActual]
                ? MisionActual.DescripcionMisionCompleta
                : MisionActual.DescripcionEnMision;

            DescripcionForma3Falsa.text = MisionesCompletas[PaginaActual]
                ? MisionActual.DescripcionMisionCompleta
                : MisionActual.DescripcionEnMision;

            DescripcionForma2.color = ColoresDrescripcionForma2[0];
        }
    }

    // ================================
    // === MISIONES DE TIPO TECLAS ===
    // ================================
    private void ProcesarMisionTeclas()
    {
        if (TeclasPresionadas == null || TeclasPresionadas.Length != MisionActual.Teclas.Length)
        {
            TeclasPresionadas = new bool[MisionActual.Teclas.Length];
            TiempoTeclas = new float[MisionActual.Teclas.Length];
        }

        for (int i = 0; i < MisionActual.Teclas.Length; i++)
        {
            if (TiempoTeclas[i] >= 1)
            {
                TeclasPresionadas[i] = true;
            }
            else
            {
                TiempoTeclas[i] += Input.GetKey(MisionActual.Teclas[i]) ? Time.deltaTime : -Time.deltaTime;
                TiempoTeclas[i] = Mathf.Clamp(TiempoTeclas[i], 0, 1);
            }

            // Actualizar UI de progreso de teclas
            Image fillImage = BotonesUI.transform.GetChild(i).GetChild(1).GetComponent<Image>();
            fillImage.fillAmount = TiempoTeclas[i];
        }

        // Verificar si todas las teclas fueron presionadas correctamente
        MisionesCompletas[PaginaActual] = System.Array.TrueForAll(TeclasPresionadas, t => t);
    }

    private void ChecarImagenMovimiento()
    {
        if (IconProvider.UsandoGamepad() && !ultimogamepad)
        {
            for (int i = 0; i < 4; i++)
            {
                // Actualizar UI de progreso de teclas
                Image Icono = BotonesUI.transform.GetChild(i).GetComponent<Image>();
                Icono.sprite = PalancasIcono[i];

                TextMeshProUGUI text = BotonesUI.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                text.text = "";
                ultimogamepad = true;
            }
        }
        else if (!IconProvider.UsandoGamepad() && ultimogamepad)
        {
            for (int i = 0; i < 4; i++)
            {
                // Actualizar UI de progreso de teclas
                Image Icono = BotonesUI.transform.GetChild(i).GetComponent<Image>();
                Icono.sprite = IconoTeclaMisiones;

                TextMeshProUGUI text = BotonesUI.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                text.text = MisionActual.Teclas[i].ToString();
            }

            ultimogamepad = false;
        }

    }

    private void ProcesarMisionMovimiento()
    {
        if (DireccionesCompletadas == null ||
            DireccionesCompletadas.Length != MisionActual.DireccionesRequeridas.Length)
        {
            DireccionesCompletadas = new bool[MisionActual.DireccionesRequeridas.Length];
            TiempoTeclas = new float[MisionActual.DireccionesRequeridas.Length];
        }

        float horizontal = MoverHorizontal.ReadValue<float>();
        float vertical = MoverVertical.ReadValue<float>();

        for (int i = 0; i < MisionActual.DireccionesRequeridas.Length; i++)
        {
            bool direccionCorrecta = false;

            switch (MisionActual.DireccionesRequeridas[i])
            {
                case DireccionMovimiento.Arriba:
                    direccionCorrecta = vertical > 0.5f;
                    break;

                case DireccionMovimiento.Abajo:
                    direccionCorrecta = vertical < -0.5f;
                    break;

                case DireccionMovimiento.Derecha:
                    direccionCorrecta = horizontal > 0.5f;
                    break;

                case DireccionMovimiento.Izquierda:
                    direccionCorrecta = horizontal < -0.5f;
                    break;
            }

            if (TiempoTeclas[i] >= 1f)
            {
                DireccionesCompletadas[i] = true;
            }
            else
            {
                TiempoTeclas[i] += direccionCorrecta ? Time.deltaTime : -Time.deltaTime;
                TiempoTeclas[i] = Mathf.Clamp01(TiempoTeclas[i]);
            }

            Image fillImage = BotonesUI.transform
                .GetChild(i)
                .GetChild(1)
                .GetComponent<Image>();

            fillImage.fillAmount = TiempoTeclas[i];
        }
        bool completa = System.Array.TrueForAll(DireccionesCompletadas, d => d);

    }

    // ================================
    // === PROGRESO Y VALIDACIÓN ===
    // ================================
    public void RevisarLugarExplorado(string lugar)
    {

        for (int i = 0; i < Misiones.Count; i++)
        {
            if (Misiones[i].Tipo == Tipos.Exploracion)
            {
                if (Misiones[i].LugarObjetivoExploracion == lugar)
                {
                    MisionesCompletas[i] = true;
                }
            }
        }
    }

    public void RevisarProgresoMisiones()
    {
        RevisarMisiones();
        ActualizarUI();
    }

    private void RevisarMisiones()
    {
        for (int i = 0; i < Misiones.Count; i++)
        {
            Scr_CreadorMisiones mision = Misiones[i];
            if (!MisionesCompletas[i])
            {
                bool completada = false;

                if (mision.Tipo == Tipos.Construccion)
                {
                    completada = ConstruccionesCompletadas(mision);
                }
                else if (mision.Tipo == Tipos.Caza)
                {
                    completada = RevisarMisionCaza(mision);
                }
                else if (mision.Tipo == Tipos.Recoleccion)
                {
                    completada = RevisarMisionRecoleccion(mision);
                }

                if (completada)
                {
                    Debug.Log($"✅ Misión secundaria '{mision.name}' completada.");
                    MisionesCompletas[i] = true;
                }
            }
        }
    }

    private bool RevisarMisionCaza(Scr_CreadorMisiones mision)
    {
        for (int i = 0; i < mision.ObjetivosACazar.Length; i++)
        {
            string clave = $"{mision.name}_CantidadCazados_{i}";
            int cazados = PlayerPrefs.GetInt(clave, 0);

            if (cazados < mision.CantidadACazar[i])
                return false;
        }

        return true;
    }

    private bool RevisarMisionRecoleccion(Scr_CreadorMisiones mision)
    {
        int j = 0;
        foreach (Scr_CreadorObjetos Objeto in mision.ObjetosNecesarios)
        {
            int i = 0;
            foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
            {
                if (Item == Objeto)
                {
                    if (Inventario.Cantidades[i] < mision.CantidadesQuita[j])
                    {
                        return false;
                    }
                }
                i++;
            }
            j++;
        }
        return true;
    }

    // ================================
    // === GUARDAR Y CARGAR DATOS ===
    // ================================

    private void ProcesarEnemigosDelSingleton()
    {
        Scr_DatosSingletonBatalla singleton = GameObject.Find("Singleton")
            .GetComponent<Scr_DatosSingletonBatalla>();

        if (singleton.EnemigosCazados.Count == 0)
            return;

        List<string> enemigosRestantes = new List<string>();
        List<int> cantidadesRestantes = new List<int>();

        for (int j = 0; j < singleton.EnemigosCazados.Count; j++)
        {
            string enemigo = singleton.EnemigosCazados[j];
            int cantidad = singleton.CantidadCazados[j];

            bool usadoEnAlgunaMision = false;

            // 🔍 Buscar en TODAS las misiones activas
            for (int m = 0; m < Misiones.Count; m++)
            {
                var mision = Misiones[m];

                if (mision.Tipo != Tipos.Caza)
                    continue;

                for (int i = 0; i < mision.ObjetivosACazar.Length; i++)
                {
                    if (mision.ObjetivosACazar[i] == enemigo)
                    {
                        string clave = $"{mision.name}_CantidadCazados_{i}";

                        int actual = PlayerPrefs.GetInt(clave, 0);
                        int objetivo = mision.CantidadACazar[i];
                        int faltante = objetivo - actual;

                        if (faltante > 0)
                        {
                            int sumar = Mathf.Min(cantidad, faltante);
                            PlayerPrefs.SetInt(clave, actual + sumar);
                            PlayerPrefs.Save();

                            Debug.Log($"✅ {enemigo} aplicado a misión {mision.name}");
                        }

                        usadoEnAlgunaMision = true;
                    }
                }
            }

            // ❌ Si NO pertenece a ninguna misión → se elimina
            if (!usadoEnAlgunaMision)
            {
                Debug.Log($"🗑️ {enemigo} eliminado del singleton (no pertenece a ninguna misión)");
            }
            else
            {
                // (Opcional) mantener si quieres debug o reprocesar
                // enemigosRestantes.Add(enemigo);
                // cantidadesRestantes.Add(cantidad);
            }
        }

        // 🔥 LIMPIAR TODO (porque ya procesaste correctamente)
        singleton.EnemigosCazados.Clear();
        singleton.CantidadCazados.Clear();
    }

    public void GuardarMisiones()
    {
        for (int i = 0; i < Misiones.Count; i++)
        {
            PlayerPrefs.SetString("Mision" + i, Misiones[i].name);
            PlayerPrefs.SetInt("MisionCompleta" + i, MisionesCompletas[i] ? 1 : 0);
            PlayerPrefs.SetInt("MisionVista" + i, MisionesVistas[i] ? 1 : 0);
        }

        PlayerPrefs.SetInt("CantidadMisiones", Misiones.Count);

        // Guardar misión actual
        if (MisionActual != null)
        {
            PlayerPrefs.SetString("MisionActual", MisionActual.name);
        }

        // Guardar página
        PlayerPrefs.SetInt("PaginaActual", PaginaActual);

        // Guardar progreso de caza
        PlayerPrefs.SetInt("CantidadEnemigosCazados", EnemigosCazados.Count);
        for (int i = 0; i < EnemigosCazados.Count; i++)
        {
            PlayerPrefs.SetString("EnemigoCazado_" + i, EnemigosCazados[i]);
            PlayerPrefs.SetInt("CantidadCazada_" + i, CantidadCazados[i]);
        }

        PlayerPrefs.Save(); // Asegúrate de guardar todos los cambios
    }

    public void CargarMisiones()
    {
        Misiones.Clear();
        MisionesCompletas.Clear();
        MisionesVistas.Clear();

        int cantidad = PlayerPrefs.GetInt("CantidadMisiones", 0);

        for (int i = 0; i < cantidad; i++)
        {
            string nombre = PlayerPrefs.GetString("Mision" + i, "");

            // Buscar en todas las misiones disponibles
            foreach (var mision in TodasLasMisiones)
            {
                if (mision.name == nombre)
                {
                    Misiones.Add(mision);

                    bool completa = PlayerPrefs.GetInt("MisionCompleta" + i, 0) == 1;
                    MisionesCompletas.Add(completa);

                    bool vista = PlayerPrefs.GetInt("MisionVista" + i, 0) == 1;
                    MisionesVistas.Add(vista);
                    break;
                }
            }
        }

        // Cargar página actual
        PaginaActual = PlayerPrefs.GetInt("PaginaActual", 0);

        // Seguridad
        if (PaginaActual >= Misiones.Count)
            PaginaActual = 0;

        // Cargar misión actual
        string nombreActual = PlayerPrefs.GetString("MisionActual", "");

        foreach (var mision in Misiones)
        {
            if (mision.name == nombreActual)
            {
                MisionActual = mision;
                break;
            }
        }

        // Fallback
        if (MisionActual == null && Misiones.Count > 0)
            MisionActual = Misiones[PaginaActual];

        // Cargar progreso de caza (opcional)
        EnemigosCazados.Clear();
        CantidadCazados.Clear();

        int cant = PlayerPrefs.GetInt("CantidadEnemigosCazados", 0);

        for (int i = 0; i < cant; i++)
        {
            EnemigosCazados.Add(PlayerPrefs.GetString("EnemigoCazado_" + i));
            CantidadCazados.Add(PlayerPrefs.GetInt("CantidadCazada_" + i));
        }

        ActualizarUI();
    }

    private bool ConstruccionesCompletadas(Scr_CreadorMisiones mision)
    {
        foreach (string nombre in mision.EstructurasRequeridas)
        {
            foreach (GameObject construccion in TodasLasConstrucciones)
            {
                Debug.Log("La construccion:" + construccion.name + " esta:" + construccion.activeInHierarchy);
                if (construccion.name == nombre && construccion.activeInHierarchy)
                {
                    return true; // 🔥 con una basta
                }
            }
        }

        return false;
    }

    public bool HayMisionRecolectar()
    {
        for (int i = 0; i < Misiones.Count; i++)
        {
            if (Misiones[i].Tipo == Scr_CreadorMisiones.Tipos.Recoleccion)
            {
                return true;
            }
        }

        return false;
    }
    public bool HayMisionDefensa()
    {
        for (int i = 0; i < Misiones.Count; i++)
        {
            if (Misiones[i].Tipo == Scr_CreadorMisiones.Tipos.Defensa)
            {
                return true;
            }
        }

        return false;
    }
    public void AsignarMisionDesdeSignal(Scr_CreadorMisiones nuevaMision)
    {
        if (nuevaMision == null)
        {
            Debug.LogWarning("⚠️ Intentaste asignar una misión nula desde signal.");
            return;
        }

        // Evitar duplicados
        if (Misiones.Contains(nuevaMision))
        {
            Debug.Log($"ℹ️ La misión '{nuevaMision.name}' ya está activa.");
            return;
        }

        // Agregar misión
        Misiones.Add(nuevaMision);
        MisionesCompletas.Add(false);
        MisionesVistas.Add(false);

        if (MisionActual == null)
        {
            PaginaActual = Misiones.Count - 1;
            MisionActual = nuevaMision;
        }


        Debug.Log($"🆕 Misión '{nuevaMision.name}' asignada desde signal.");

        // Actualizar UI y guardar
        ActualizarUI();
        GuardarMisiones();
    }

    public void SustituirMisionDesdeSingleton(Scr_CreadorMisiones nuevaMision)
    {
        if (nuevaMision == null)
        {
            Debug.LogWarning("⚠️ Nueva misión null.");
            return;
        }

        if (nuevaMision.MisionAnterior == null)
        {
            Debug.LogWarning("⚠️ No tiene MisionAnterior.");
            return;
        }

        int index = Misiones.IndexOf(nuevaMision.MisionAnterior);

        if (index == -1)
        {
            Debug.LogWarning($"⚠️ No se encontró '{nuevaMision.MisionAnterior.name}'.");
            return;
        }

        var misionAnterior = Misiones[index];

        // =========================================
        // 🔥 1. BORRAR PROGRESO DE CAZA (OBLIGATORIO)
        // =========================================
        if (misionAnterior.ObjetivosACazar != null)
        {
            for (int i = 0; i < misionAnterior.ObjetivosACazar.Length; i++)
            {
                string clave = $"{misionAnterior.name}_CantidadCazados_{i}";

                if (PlayerPrefs.HasKey(clave))
                {
                    PlayerPrefs.DeleteKey(clave);
                }
            }

            Debug.Log($"🧹 Progreso de caza eliminado de '{misionAnterior.name}'");
        }

        // =========================================
        // 📦 2. QUITAR OBJETOS (SI APLICA)
        // =========================================
        if (nuevaMision.QuitaObjetosDesdeSignal &&
            nuevaMision.Tipo == Tipos.Recoleccion &&
            nuevaMision.ObjetosNecesarios != null)
        {
            for (int i = 0; i < nuevaMision.ObjetosNecesarios.Length; i++)
            {
                var objeto = nuevaMision.ObjetosNecesarios[i];
                int cantidadAQuitar = nuevaMision.CantidadesQuita[i];

                Inventario.QuitarObjeto(
                    objeto.Nombre,
                    cantidadAQuitar,
                    false
                );
            }

            Debug.Log($"📦 Objetos removidos por '{nuevaMision.name}'");
        }

        // =========================================
        // 🔄 3. REEMPLAZAR MISIÓN
        // =========================================
        Misiones[index] = nuevaMision;

        // ⚠️ NO asumimos nada → siempre reinicia estado
        MisionesCompletas[index] = false;

        MisionesVistas[index] = false; //

        // =========================================
        // 🎯 4. ACTUALIZAR MISIÓN ACTUAL
        // =========================================
        if (MisionActual == misionAnterior)
        {
            MisionActual = nuevaMision;
            PaginaActual = index;
        }

        Debug.Log($"🔄 '{misionAnterior.name}' → '{nuevaMision.name}'");

        // =========================================
        // 💾 5. REFRESCAR
        // =========================================
        ActualizarUI();
        GuardarMisiones();
    }

}
