using PrimeTween;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Scr_CreadorMisiones;

public class Scr_ControladorMisiones : MonoBehaviour
{
    // ================================
    // === REFERENCIAS PRINCIPALES ===
    // ================================
    public Scr_CreadorMisiones MisionActual;          // Misión que se está mostrando actualmente en el panel
    public Scr_CreadorMisiones MisionPrincipal;       // Misión principal del jugador
    public List<Scr_CreadorMisiones> MisionesSecundarias;   // Lista de misiones secundarias activas
    public Scr_CreadorMisiones[] TodasLasMisiones;    // Todas las misiones posibles en el juego

    // ================================
    // === ESTADO DE LAS MISIONES ===
    // ================================
    public bool MisionActualCompleta;           // ¿La misión actual está completa?
    public bool MisionPCompleta;                // ¿La misión principal está completa?
    public List<bool> MisionesScompletas;       // Estado de todas las misiones secundarias

    // ================================
    // === CONTROL DE OBJETOS ===
    // ================================
    private Scr_Inventario Inventario;
    private Transform Gata;
    private bool EstadoPanelMisiones;
    [SerializeField] private GameObject BotonesUI;
    [SerializeField] private GameObject PanelMisiones;
    [SerializeField] private TextMeshProUGUI TextoDescripcion;
    [SerializeField] private TextMeshProUGUI TextoPagina;
    [SerializeField] private GameObject ObjetosRecoleccion;

    // ================================
    // === CONTROL DE PÁGINAS Y TECLAS ===
    // ================================
    private int PaginaActual = 1;
    private bool[] TeclasPresionadas;
    private float[] TiempoTeclas;

    // ================================
    // === TRACKING DE PROGRESO ===
    // ================================
    public List<string> EnemigosCazados;      // Nombres de los enemigos eliminados
    public List<int> CantidadCazados;         // Cantidad cazada de cada enemigo
    public List<string> LugaresExplorados;    // Lista de lugares explorados

    // ================================
    // === OBJETOS DE ESCENA ===
    // ================================
    public GameObject[] TodasLasConstrucciones;

    // ================================
    // === MÉTODOS UNITY ===
    // ================================
    void Awake()
    {
        // Buscar referencias importantes
        Gata = GameObject.Find("Gata").transform;
        Inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
        TodasLasConstrucciones = Buscartag.BuscarObjetosConTagInclusoInactivos("Construcciones");

        // Cargar datos guardados
        CargarMisiones();
        ActualizarUI();

    }

    private string ultimaDescripcion = "";

    void Update()
    {
        if (MisionActual != null && MisionActual.Tipo == Tipos.Teclas)
        {
            ProcesarMisionTeclas();
            BotonesUI.SetActive(!MisionActualCompleta);
        }

        if (MisionActual != null)
        {
            RevisarMisionesSecundarias(); // <-- MOVER AQUÍ PRIMERO

            string nuevaDescripcion = MisionActualCompleta ? MisionActual.DescripcionCompleta : MisionActual.Descripcion;
            if (nuevaDescripcion != ultimaDescripcion)
            {
                ActualizarUI();
                ultimaDescripcion = nuevaDescripcion;
            }

            InputPanelMisiones();
        }

    }

    private void InputPanelMisiones()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (EstadoPanelMisiones)
            {
                Tween.PositionX(PanelMisiones.GetComponent<RectTransform>(), -610, 0.5f, Ease.Default);
                EstadoPanelMisiones = false;
            }
            else
            {
                Tween.PositionX(PanelMisiones.GetComponent<RectTransform>(), 0, 0.5f, Ease.Default);
                EstadoPanelMisiones = true;
            }
        }
    }

    // ================================
    // === MÉTODOS DE LA UI ===
    // ================================
    public void ActualizarUI()
    {
        if (TextoDescripcion == null || TextoPagina == null) return;

        if (MisionActual != null)
        {
            TextoDescripcion.text = MisionActualCompleta ? MisionActual.DescripcionCompleta : MisionActual.Descripcion;
            int totalPaginas = (MisionPrincipal != null ? 1 : 0) + MisionesSecundarias.Count;
            TextoPagina.text = $"{PaginaActual}/{(totalPaginas > 0 ? totalPaginas : 1)}";

            // Oculta el panel si no es caza ni recolección
            if (MisionActual.Tipo != Tipos.Caza && MisionActual.Tipo != Tipos.Recoleccion)
            {
                ObjetosRecoleccion.SetActive(false);
                return; // Ya que no hay más que mostrar
            }

            // Ocultar todos los hijos
            for (int i = 0; i < ObjetosRecoleccion.transform.childCount; i++)
                ObjetosRecoleccion.transform.GetChild(i).gameObject.SetActive(false);

            ObjetosRecoleccion.SetActive(true);

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
        else
        {
            TextoDescripcion.text = "Sin objetivo...";
            TextoPagina.text = "...";
            if (ObjetosRecoleccion != null)
                ObjetosRecoleccion.SetActive(false);
        }

        GuardarMisiones();
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
        MisionPCompleta = System.Array.TrueForAll(TeclasPresionadas, t => t);
        if (MisionPrincipal == MisionActual)
        {
            MisionActualCompleta = MisionPCompleta;
        }
    }

    // ================================
    // === PROGRESO Y VALIDACIÓN ===
    // ================================
    public void MarcarLugarExplorado(string lugar)
    {
        if (!LugaresExplorados.Contains(lugar))
        {
            LugaresExplorados.Add(lugar);
            Debug.Log($"Se agregó {lugar} a LugaresExplorados");
        }

        RevisarProgresoMisiones();
    }

    public void RevisarProgresoMisiones()
    {
        RevisarMisionPrincipal();
        RevisarMisionesSecundarias();
        ActualizarUI();
    }

    public void RevisarMisionPrincipal()
    {
        if (MisionPrincipal != null && !MisionPCompleta)
        {
            bool completada = false;

            // Revisar según el tipo de misión
            if (MisionPrincipal.Tipo == Tipos.Construccion)
            {
                completada = ConstruccionesCompletadas();
            }
            else if (MisionPrincipal.Tipo == Tipos.Caza)
            {
                //completada = RevisarMisionCaza(MisionPrincipal);
            }
            else if (MisionPrincipal.Tipo == Tipos.Recoleccion)
            {
                //completada = RevisarMisionRecoleccion(MisionPrincipal);
            }

            if (completada)
            {
                Debug.Log($"✅ Misión principal '{MisionPrincipal.name}' completada.");
                MisionPCompleta = true;

                if (MisionPrincipal == MisionActual)
                    MisionActualCompleta = true;

                // Guarda el progreso
                GuardarMisiones();
            }
        }
    }
    private void RevisarMisionesSecundarias()
    {
        for (int i = 0; i < MisionesSecundarias.Count; i++)
        {
            Scr_CreadorMisiones mision = MisionesSecundarias[i];
            if (!MisionesScompletas[i])
            {
                bool completada = false;

                if (mision.Tipo == Tipos.Construccion)
                {
                    completada = ConstruccionesCompletadas();
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
                    MisionesScompletas[i] = true;
                }
            }
        }
    }

    private bool RevisarMisionCaza(Scr_CreadorMisiones mision)
    {
        Scr_DatosSingletonBatalla Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();

        if (Singleton.EnemigosCazados.Count > 0)
        {
            // Recorrer cada enemigo cazado en el Singleton
            for (int j = 0; j < Singleton.EnemigosCazados.Count; j++)
            {
                string enemigoCazado = Singleton.EnemigosCazados[j];
                int cantidadCazada = Singleton.CantidadCazados[j];

                // Revisar si esta misión requiere ese enemigo
                for (int i = 0; i < mision.ObjetivosACazar.Length; i++)
                {
                    if (mision.ObjetivosACazar[i] == enemigoCazado)
                    {
                        // Construir la clave de PlayerPrefs para esta misión y enemigo
                        string clave = $"{mision.name}_CantidadCazados_{i}";

                        // Obtener la cantidad actual guardada y la cantidad que falta
                        int cantidadActual = PlayerPrefs.GetInt(clave, 0);
                        int cantidadObjetivo = mision.CantidadACazar[i];
                        int cantidadFaltante = cantidadObjetivo - cantidadActual;

                        if (cantidadFaltante > 0)
                        {
                            // Sumar solo lo necesario para no pasar la cantidad objetivo
                            int cantidadASumar = Mathf.Min(cantidadCazada, cantidadFaltante);
                            int nuevoTotal = cantidadActual + cantidadASumar;

                            PlayerPrefs.SetInt(clave, nuevoTotal);
                            PlayerPrefs.Save();

                            Debug.Log($"✅ Misión '{mision.name}': Enemigo '{enemigoCazado}' actualizado. Sumado: {cantidadASumar}, Total: {nuevoTotal}/{cantidadObjetivo}");
                        }
                        else
                        {
                            Debug.Log($"🎯 Misión '{mision.name}': Enemigo '{enemigoCazado}' ya completo ({cantidadActual}/{cantidadObjetivo}). No se suma más.");
                        }
                    }
                }
            }
        }

        // Vaciar las listas del Singleton después de procesar
        Singleton.EnemigosCazados.Clear();
        Singleton.CantidadCazados.Clear();

        // Revisar si la misión está completa
        for (int i = 0; i < mision.ObjetivosACazar.Length; i++)
        {
            string clave = $"{mision.name}_CantidadCazados_{i}";
            int cazados = PlayerPrefs.GetInt(clave, 0);
            if (cazados < mision.CantidadACazar[i])
            {
                return false; // Aún no se ha cazado la cantidad necesaria para al menos un enemigo
            }
        }

        return true; // Todos los enemigos cazados en la cantidad necesaria
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
    public void GuardarMisiones()
    {
        Debug.Log("Guarda misiones");
        // Guardar misión principal
        if (MisionPrincipal != null)
        {
            Debug.Log("Guarda mision principal");
            PlayerPrefs.SetString("MisionPrincipal", MisionPrincipal.name);
            PlayerPrefs.SetInt("MisionPrincipalCompleta", MisionPCompleta ? 1 : 0);
        }

        // Guardar misiones secundarias
        for (int i = 0; i < MisionesSecundarias.Count; i++)
        {
            PlayerPrefs.SetString("MisionSecundaria_" + i, MisionesSecundarias[i].name);
            PlayerPrefs.SetInt("MisionSecundariaCompleta_" + i, MisionesScompletas[i] ? 1 : 0);
        }

        PlayerPrefs.SetInt("CantidadMisionesSecundarias", MisionesSecundarias.Count);

        // Guardar misión actual
        if (MisionActual != null)
        {
            PlayerPrefs.SetString("MisionActual", MisionActual.name);
            PlayerPrefs.SetInt("MisionActualCompleta", MisionActualCompleta ? 1 : 0);
        }

        // Guardar progreso de caza
        PlayerPrefs.SetInt("CantidadEnemigosCazados", EnemigosCazados.Count);
        for (int i = 0; i < EnemigosCazados.Count; i++)
        {
            PlayerPrefs.SetString("EnemigoCazado_" + i, EnemigosCazados[i]);
            PlayerPrefs.SetInt("CantidadCazada_" + i, CantidadCazados[i]);
        }

        // Guardar lugares explorados
        PlayerPrefs.SetInt("CantidadLugaresExplorados", LugaresExplorados.Count);
        for (int i = 0; i < LugaresExplorados.Count; i++)
        {
            PlayerPrefs.SetString("LugarExplorado_" + i, LugaresExplorados[i]);
        }

        PlayerPrefs.Save(); // Asegúrate de guardar todos los cambios
    }


    public void CargarMisiones()
    {
        // Cargar misión principal
        string nombreMisionPrincipal = PlayerPrefs.GetString("MisionPrincipal", "");
        if (!string.IsNullOrEmpty(nombreMisionPrincipal))
        {
            Debug.Log("Encontro Mision Principal");
            MisionPrincipal = BuscarMisionPorNombre(nombreMisionPrincipal);
            MisionPCompleta = PlayerPrefs.GetInt("MisionPrincipalCompleta", 0) == 1;
        }

        // Cargar misiones secundarias
        int cantidadSecundarias = PlayerPrefs.GetInt("CantidadMisionesSecundarias", 0);
        MisionesSecundarias.Clear();
        MisionesScompletas.Clear();
        for (int i = 0; i < cantidadSecundarias; i++)
        {
            string nombreMision = PlayerPrefs.GetString("MisionSecundaria_" + i, "");
            if (!string.IsNullOrEmpty(nombreMision))
            {
                Scr_CreadorMisiones mision = BuscarMisionPorNombre(nombreMision);
                if (mision != null)
                {
                    MisionesSecundarias.Add(mision);
                    bool completa = PlayerPrefs.GetInt("MisionSecundariaCompleta_" + i, 0) == 1;
                    MisionesScompletas.Add(completa);
                }
            }
        }

        // Cargar misión actual DESPUÉS de cargar todas las misiones
        string nombreMisionActual = PlayerPrefs.GetString("MisionActual", "");
        if (!string.IsNullOrEmpty(nombreMisionActual))
        {
            MisionActual = BuscarMisionPorNombre(nombreMisionActual);
            MisionActualCompleta = PlayerPrefs.GetInt("MisionActualCompleta", 0) == 1;
        }

        // Cargar lugares explorados
        LugaresExplorados.Clear();
        int cantidadLugares = PlayerPrefs.GetInt("CantidadLugaresExplorados", 0);
        for (int i = 0; i < cantidadLugares; i++)
        {
            LugaresExplorados.Add(PlayerPrefs.GetString("LugarExplorado_" + i, ""));
        }
    }


    private Scr_CreadorMisiones BuscarMisionPorNombre(string nombre)
    {
        foreach (var mision in TodasLasMisiones)
        {
            if (mision.name == nombre)
            {
                return mision;
            }
        }
        return null;
    }

    private bool ConstruccionesCompletadas()
    {
        foreach (GameObject construccion in TodasLasConstrucciones)
        {
            if (!construccion.activeInHierarchy)
                return false; // Falta al menos una construcción
        }
        return true; // Todas las construcciones están listas
    }



}
