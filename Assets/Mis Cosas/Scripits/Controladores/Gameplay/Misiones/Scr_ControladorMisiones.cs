using PrimeTween;
using System.Collections.Generic;
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
    public bool MisionCompleta;           // ¿La misión actual está completa?
    public bool MisionPCompleta;          // ¿La misión principal está completa?
    public List<bool> MisionesScompletas; // Estado de todas las misiones secundarias
    public MisionesData MisionData;       // Datos serializados para guardar/cargar progreso

    // ================================
    // === CONTROL DE OBJETOS ===
    // ================================
    private Scr_Inventario Inventario;
    private Transform Gata;
    [SerializeField] private GameObject BotonesUI;
    [SerializeField] private GameObject PanelMisiones;
    [SerializeField] private TextMeshProUGUI TextoDescripcion;
    [SerializeField] private TextMeshProUGUI TextoPagina;
    [SerializeField] private KeyCode TeclaAbrirPanel = KeyCode.M;
    [SerializeField] private GameObject ObjetosRecoleccion;

    // ================================
    // === CONTROL DE PÁGINAS Y TECLAS ===
    // ================================
    private int PaginaActual = 1;
    private bool[] TeclasPresionadas;
    private float[] TiempoTeclas;
    private bool PanelOculto = true;

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
    void Start()
    {
        // Buscar referencias importantes
        Gata = GameObject.Find("Gata").transform;
        Inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
        TodasLasConstrucciones = Buscartag.BuscarObjetosConTagInclusoInactivos("Construcciones");

        // Cargar datos guardados y actualizar información de la UI
        CargarMisiones();
        ActualizarUI();
    }

    private string ultimaDescripcion = "";

    void Update()
    {
        // Actualizar misión de tipo "Teclas"
        if (MisionActual != null && MisionActual.Tipo == Tipos.Teclas)
        {
            ProcesarMisionTeclas();
            BotonesUI.SetActive(!MisionCompleta);
        }

        // Solo refrescar UI si la descripción cambió
        if (MisionActual != null)
        {
            string nuevaDescripcion = MisionCompleta ? MisionActual.DescripcionFinal : MisionActual.Descripcion;
            if (nuevaDescripcion != ultimaDescripcion)
            {
                ActualizarUI();
                ultimaDescripcion = nuevaDescripcion;
            }
        }

        if (Input.GetKeyDown(TeclaAbrirPanel))
        {
            AlternarPanelMisiones();
        }
    }


    // ================================
    // === MÉTODOS DE LA UI ===
    // ================================
    private void ActualizarUI()
    {
        if (MisionActual != null)
        {
            //Actualiza en caso de tener mision
            TextoDescripcion.text = MisionCompleta ? MisionActual.DescripcionFinal : MisionActual.Descripcion;
            TextoPagina.text = $"{PaginaActual}/{(MisionesSecundarias.Count > 0 ? MisionesSecundarias.Count : 1)}";
            if (MisionActual.Tipo == Tipos.Caza || MisionActual.Tipo == Tipos.Recoleccion)
            {

                //Nos Aseguramos de desactivar los objetos que no se vayan a usar
                ObjetosRecoleccion.transform.GetChild(0).gameObject.SetActive(false);
                ObjetosRecoleccion.transform.GetChild(1).gameObject.SetActive(false);
                ObjetosRecoleccion.transform.GetChild(2).gameObject.SetActive(false);
                ObjetosRecoleccion.transform.GetChild(3).gameObject.SetActive(false);
                ObjetosRecoleccion.SetActive(true);

                int N = 0;
                if (MisionActual.ObjetosNecesarios != null)
                {
                    foreach (Scr_CreadorObjetos Item in MisionActual.ObjetosNecesarios)
                    {
                        ObjetosRecoleccion.transform.GetChild(N).gameObject.SetActive(true);
                        ObjetosRecoleccion.transform.GetChild(N).GetComponent<Image>().sprite = MisionActual.ObjetosNecesarios[N].Icono;
                        ObjetosRecoleccion.transform.GetChild(N).GetChild(0).GetComponent<TextMeshProUGUI>().text = MisionActual.ObjetosNecesarios[N].Nombre;
                        //Buscar objetos en el inventario
                        int I = 0;
                        foreach (Scr_CreadorObjetos Objeto in Inventario.Objetos)
                        {
                            if (Objeto == MisionActual.ObjetosNecesarios[N])
                            {
                                ObjetosRecoleccion.transform.GetChild(N).GetChild(1).GetComponent<TextMeshProUGUI>().text = Inventario.Cantidades[I].ToString();

                            }
                            I++;
                        }
                        ObjetosRecoleccion.transform.GetChild(N).GetChild(3).GetComponent<TextMeshProUGUI>().text = MisionActual.CantidadesQuita[N].ToString();
                        N++;
                    }
                }
                if (MisionActual.ObjetivosACazar.Length > 0)
                {
                    N = 0;
                    foreach (string Enemigo in MisionActual.ObjetivosACazar)
                    {
                        ObjetosRecoleccion.transform.GetChild(N).gameObject.SetActive(true);
                        ObjetosRecoleccion.transform.GetChild(N).GetComponent<Image>().sprite = MisionActual.IconosACazar[N];
                        ObjetosRecoleccion.transform.GetChild(N).GetChild(0).GetComponent<TextMeshProUGUI>().text = MisionActual.ObjetivosACazar[N];
                        ObjetosRecoleccion.transform.GetChild(N).GetChild(1).GetComponent<TextMeshProUGUI>().text = CantidadCazados.ToArray()[N].ToString();
                        ObjetosRecoleccion.transform.GetChild(N).GetChild(3).GetComponent<TextMeshProUGUI>().text = MisionActual.CantidadACazar[N].ToString();
                        N++;
                    }
                }



            }

        }
        else
        {
            //Muestra por defecto
            TextoDescripcion.text = "Sin objetivo...";
            TextoPagina.text = "...";
            if (ObjetosRecoleccion != null)
            {
                ObjetosRecoleccion.SetActive(false);
            }
        }

        GuardarMisiones();
    }

    private void AlternarPanelMisiones()
    {
        if (PanelOculto)
        {
            PanelOculto = false;
            Tween.PositionX(PanelMisiones.transform, 0, 1);
        }
        else
        {
            PanelOculto = true;
            Tween.PositionX(PanelMisiones.transform, -610, 1);
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
        MisionPCompleta = System.Array.TrueForAll(TeclasPresionadas, t => t);
        if (MisionPrincipal == MisionActual)
        {
            MisionCompleta = MisionPCompleta;
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
                    MisionCompleta = true;

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
                    //completada = RevisarMisionCaza(mision);
                }
                else if (mision.Tipo == Tipos.Recoleccion)
                {
                    //completada = RevisarMisionRecoleccion(mision);
                }

                if (completada)
                {
                    Debug.Log($"✅ Misión secundaria '{mision.name}' completada.");
                    MisionesScompletas[i] = true;
                }
            }
        }
    }

    // ================================
    // === GUARDAR Y CARGAR DATOS ===
    // ================================
    private void GuardarMisiones()
    {
        if (MisionPrincipal != null)
        {
            PlayerPrefs.SetString("MisionPrincipal", MisionPrincipal.name);
        }

        for (int i = 0; i < MisionesSecundarias.Count; i++)
        {
            string nombreMisionExtra = MisionesSecundarias[i].name;
            PlayerPrefs.SetString("MisionExtra_" + i, nombreMisionExtra);
        }

        if (MisionActual != null)
        {
            PlayerPrefs.SetString("MisionActual", MisionActual.MisionName);

        }
    }

    private void CargarMisiones()
    {
        string nombreMisionPrincipal = PlayerPrefs.GetString("MisionPrincipal", "");
        if (!string.IsNullOrEmpty(nombreMisionPrincipal))
        {
            MisionPrincipal = BuscarMisionPorNombre(nombreMisionPrincipal);
        }

        for (int i = 0; i < 10; i++) // Cargar hasta 10 misiones secundarias
        {
            string nombreMisionExtra = PlayerPrefs.GetString("MisionExtra_" + i, "");
            if (!string.IsNullOrEmpty(nombreMisionExtra))
            {
                Scr_CreadorMisiones misionExtra = BuscarMisionPorNombre(nombreMisionExtra);
                if (misionExtra != null && !MisionesSecundarias.Contains(misionExtra))
                {
                    MisionesSecundarias.Add(misionExtra);
                }
            }
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
