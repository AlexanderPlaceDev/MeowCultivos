using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scr_Cofre : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] string IDCofre;
    [SerializeField] int CantidadAlmacenableCofre;
    [SerializeField] Image FlechaDireccion;
    [SerializeField] Sprite FlechaDer;
    [SerializeField] Sprite FlechaIzq;

    [Header("UI Inventarios")]
    [SerializeField] private GameObject[] ItemsMochila;
    [SerializeField] private GameObject[] ItemsCofre;
    private string FiltroActual = "";

    [Header("UI Control")]
    [SerializeField] Scrollbar Barra;

    [Header("UI Selección")]
    [SerializeField] Image ImagenItemSeleccionado;
    [SerializeField] TextMeshProUGUI TextoCantidad;

    private GameObject Gata;
    private Scr_Inventario Inventario;

    // Mochila
    private List<Scr_CreadorObjetos> ObjetosEnMochila;
    private List<int> CantidadesEnMochila;

    // Cofre
    private int[] CantidadesCofre;
    private List<Scr_CreadorObjetos> ObjetosEnCofre;
    private List<int> CantidadesFiltradasCofre;

    // Paginación
    private int PaginaMochilaActual;
    private int PaginaCofreActual;

    // Selección
    private Scr_CreadorObjetos ItemSeleccionado = null;
    private int IndexItemSeleccionado = -1;
    private bool ItemVieneDeMochila = true;

    private int CantidadAPasar = 0;


    private void OnEnable()
    {
        ActualizarTodo();
    }

    void Start()
    {
        Gata = GameObject.Find("Gata");
        Inventario = Gata.transform.GetChild(7).GetComponent<Scr_Inventario>();

        InicializarListas();
        InicializarCofre();

        Barra.onValueChanged.AddListener(delegate
        {
            ActualizarTodo();
        });

        ActualizarTodo();
    }

    // =========================
    // INICIALIZACIÓN
    // =========================

    private void InicializarListas()
    {
        ObjetosEnMochila = new List<Scr_CreadorObjetos>();
        CantidadesEnMochila = new List<int>();

        ObjetosEnCofre = new List<Scr_CreadorObjetos>();
        CantidadesFiltradasCofre = new List<int>();
    }

    private void InicializarCofre()
    {
        int tamaño = Inventario.Objetos.Length;
        CantidadesCofre = new int[tamaño];

        for (int i = 0; i < tamaño; i++)
        {
            string key = IDCofre + "_item_" + i;
            CantidadesCofre[i] = PlayerPrefs.GetInt(key, 0);
        }
    }

    private void GuardarCofre()
    {
        for (int i = 0; i < CantidadesCofre.Length; i++)
        {
            string key = IDCofre + "_item_" + i;
            PlayerPrefs.SetInt(key, CantidadesCofre[i]);
        }

        PlayerPrefs.Save();
    }

    // =========================
    // ACTUALIZACIÓN GENERAL
    // =========================

    private void ActualizarTodo()
    {
        ActualizarInventarioMochila();
        ActualizarInventarioCofre();
        ActualizarSeleccionUI();
    }

    // =========================
    // MOCHILA
    // =========================

    private void ActualizarInventarioMochila()
    {
        ObjetosEnMochila.Clear();
        CantidadesEnMochila.Clear();

        for (int i = 0; i < Inventario.Objetos.Length; i++)
        {
            if (Inventario.Cantidades[i] > 0)
            {
                var objeto = Inventario.Objetos[i];

                // 🔥 FILTRO
                if (FiltroActual == "" || objeto.TipoMaterial == FiltroActual)
                {
                    ObjetosEnMochila.Add(objeto);
                    CantidadesEnMochila.Add(Inventario.Cantidades[i]);
                }
            }
        }

        PaginaMochilaActual = CalcularPagina(ObjetosEnMochila.Count, ItemsMochila.Length);

        ActualizarUILista(ObjetosEnMochila, CantidadesEnMochila, ItemsMochila, PaginaMochilaActual);
    }

    // =========================
    // COFRE
    // =========================

    private void ActualizarInventarioCofre()
    {
        ObjetosEnCofre.Clear();
        CantidadesFiltradasCofre.Clear();

        for (int i = 0; i < CantidadesCofre.Length; i++)
        {
            if (CantidadesCofre[i] > 0)
            {
                var objeto = Inventario.Objetos[i];

                // 🔥 FILTRO
                if (FiltroActual == "" || objeto.TipoMaterial == FiltroActual)
                {
                    ObjetosEnCofre.Add(objeto);
                    CantidadesFiltradasCofre.Add(CantidadesCofre[i]);
                }
            }
        }

        PaginaCofreActual = CalcularPagina(ObjetosEnCofre.Count, ItemsCofre.Length);

        ActualizarUILista(ObjetosEnCofre, CantidadesFiltradasCofre, ItemsCofre, PaginaCofreActual);
    }

    // =========================
    // PAGINACIÓN
    // =========================

    private int CalcularPagina(int totalItems, int itemsPorPagina)
    {
        int totalPaginas = Mathf.CeilToInt((float)totalItems / itemsPorPagina);

        int pagina = Mathf.FloorToInt(Barra.value * totalPaginas);

        if (pagina >= totalPaginas)
            pagina = totalPaginas - 1;

        if (pagina < 0)
            pagina = 0;

        return pagina;
    }

    // =========================
    // UI
    // =========================

    private void ActualizarUILista(List<Scr_CreadorObjetos> objetos, List<int> cantidades, GameObject[] slots, int paginaActual)
    {
        int itemsPorPagina = slots.Length;
        int indiceInicial = paginaActual * itemsPorPagina;

        for (int i = 0; i < slots.Length; i++)
        {
            int indiceReal = indiceInicial + i;

            if (indiceReal < objetos.Count)
            {
                var objeto = objetos[indiceReal];

                slots[i].SetActive(true);

                slots[i].transform.GetChild(0).GetChild(0)
                    .GetComponent<TextMeshProUGUI>().text = cantidades[indiceReal].ToString();

                slots[i].transform.GetChild(1).GetChild(0)
                    .GetComponent<Image>().sprite = objeto.Icono;

                slots[i].transform.GetChild(1).GetChild(1)
                    .GetComponent<TextMeshProUGUI>().text = objeto.Nombre;
            }
            else
            {
                slots[i].SetActive(false);
            }
        }
    }

    private void ActualizarSeleccionUI()
    {
        if (ItemSeleccionado == null)
        {
            ImagenItemSeleccionado.color = new Color(1, 1, 1, 0);
            TextoCantidad.text = "0";
            TextoCantidad.color = Color.white;

            // 🔥 Desactivar flecha
            FlechaDireccion.gameObject.SetActive(false);
        }
        else
        {
            ImagenItemSeleccionado.color = new Color(1, 1, 1, 1);
            ImagenItemSeleccionado.sprite = ItemSeleccionado.Icono;
            TextoCantidad.text = CantidadAPasar.ToString();

            TextoCantidad.color = CantidadValida() ? Color.white : Color.red;

            // 🔥 Activar flecha
            FlechaDireccion.gameObject.SetActive(true);
        }
    }

    // =========================
    // SELECCIÓN
    // =========================

    public void Filtro(string nombreFiltro)
    {
        if (nombreFiltro == "Limpiar")
        {
            FiltroActual = "";
        }
        else
        {
            FiltroActual = nombreFiltro;
        }

        // 🔥 Reset de selección para evitar inconsistencias
        ItemSeleccionado = null;
        IndexItemSeleccionado = -1;
        CantidadAPasar = 0;

        ActualizarTodo();
    }

    public void SeleccionarItemMochila(int indiceBoton)
    {
        int indiceReal = PaginaMochilaActual * ItemsMochila.Length + indiceBoton;

        if (indiceReal < ObjetosEnMochila.Count)
        {
            ItemSeleccionado = ObjetosEnMochila[indiceReal];

            IndexItemSeleccionado = System.Array.IndexOf(Inventario.Objetos, ItemSeleccionado);
            ItemVieneDeMochila = true;
            FlechaDireccion.sprite = FlechaDer;

            CantidadAPasar = 0;
            ActualizarSeleccionUI();
        }
    }

    public void SeleccionarItemCofre(int indiceBoton)
    {
        int indiceReal = PaginaCofreActual * ItemsCofre.Length + indiceBoton;

        if (indiceReal < ObjetosEnCofre.Count)
        {
            ItemSeleccionado = ObjetosEnCofre[indiceReal];

            IndexItemSeleccionado = System.Array.IndexOf(Inventario.Objetos, ItemSeleccionado);
            ItemVieneDeMochila = false;
            FlechaDireccion.sprite = FlechaIzq;

            CantidadAPasar = 0;
            ActualizarSeleccionUI();
        }
    }

    // =========================
    // FLECHAS
    // =========================

    public void FlechaMas()
    {
        if (ItemSeleccionado == null) return;

        int max = ItemVieneDeMochila
            ? Inventario.Cantidades[IndexItemSeleccionado]
            : CantidadesCofre[IndexItemSeleccionado];

        if (CantidadAPasar < max)
            CantidadAPasar++;

        ActualizarSeleccionUI();
    }

    public void FlechaMenos()
    {
        if (CantidadAPasar > 0)
            CantidadAPasar--;

        ActualizarSeleccionUI();
    }

    public void CambiarDireccion()
    {
        // Invertir dirección
        ItemVieneDeMochila = !ItemVieneDeMochila;

        if (ItemSeleccionado != null)
        {
            int limiteReal = 0;

            if (ItemVieneDeMochila)
            {
                // ===== HACIA COFRE =====
                int cantidadActualCofre = CantidadesCofre[IndexItemSeleccionado];
                int espacioCofre = CantidadAlmacenableCofre - cantidadActualCofre;

                int disponibleMochila = Inventario.Cantidades[IndexItemSeleccionado];

                limiteReal = Mathf.Min(espacioCofre, disponibleMochila);

                FlechaDireccion.sprite = FlechaDer;
            }
            else
            {
                // ===== HACIA MOCHILA =====
                int limiteMochila = ObtenerLimiteActual();
                int cantidadActualMochila = Inventario.Cantidades[IndexItemSeleccionado];

                int espacioMochila = limiteMochila - cantidadActualMochila;

                int disponibleCofre = CantidadesCofre[IndexItemSeleccionado];

                limiteReal = Mathf.Min(espacioMochila, disponibleCofre);

                FlechaDireccion.sprite = FlechaIzq;
            }

            // 🔥 AQUÍ ESTÁ LA CLAVE
            if (limiteReal > 0 && CantidadAPasar > limiteReal)
            {
                CantidadAPasar = limiteReal;
            }
            // ⚠️ si limiteReal es 0 → NO tocar la cantidad
        }

        ActualizarSeleccionUI();
    }

    // =========================
    // TRANSFERENCIA
    // =========================

    private int CalcularMaximoTransferible()
    {
        if (ItemSeleccionado == null) return 0;

        if (ItemVieneDeMochila)
        {
            // ===== HACIA COFRE =====
            int cantidadActualCofre = CantidadesCofre[IndexItemSeleccionado];
            int espacioCofre = CantidadAlmacenableCofre - cantidadActualCofre;

            int disponibleMochila = Inventario.Cantidades[IndexItemSeleccionado];

            return Mathf.Max(0, Mathf.Min(espacioCofre, disponibleMochila));
        }
        else
        {
            // ===== HACIA MOCHILA =====
            int limiteMochila = ObtenerLimiteActual();
            int cantidadActualMochila = Inventario.Cantidades[IndexItemSeleccionado];

            int espacioMochila = limiteMochila - cantidadActualMochila;

            int disponibleCofre = CantidadesCofre[IndexItemSeleccionado];

            return Mathf.Max(0, Mathf.Min(espacioMochila, disponibleCofre));
        }
    }

    public void PasarItem()
    {
        if (ItemSeleccionado == null || CantidadAPasar <= 0) return;

        if (!CantidadValida()) return; // 🔥 VALIDACIÓN REAL

        if (ItemVieneDeMochila)
            PasarAlCofre(IndexItemSeleccionado, CantidadAPasar);
        else
            PasarALaMochila(IndexItemSeleccionado, CantidadAPasar);

        CantidadAPasar = 0;
        ActualizarTodo();
    }

    public void SetCantidadMaxima()
    {
        if (ItemSeleccionado == null) return;

        CantidadAPasar = CalcularMaximoTransferible();
        ActualizarSeleccionUI();
    }

    public void SetCantidadMitad()
    {
        if (ItemSeleccionado == null) return;

        int max = CalcularMaximoTransferible();
        CantidadAPasar = max / 2;

        ActualizarSeleccionUI();
    }

    public void ResetSeleccion()
    {
        ItemSeleccionado = null;
        IndexItemSeleccionado = -1;
        CantidadAPasar = 0;

        ActualizarSeleccionUI();
    }

    private void PasarAlCofre(int index, int cantidad)
    {
        int cantidadActual = CantidadesCofre[index];
        int espacioDisponible = CantidadAlmacenableCofre - cantidadActual;

        if (cantidad > espacioDisponible)
        {
            Debug.Log("Stack lleno en el cofre");
            return;
        }

        if (Inventario.Cantidades[index] >= cantidad)
        {
            Inventario.Cantidades[index] -= cantidad;
            CantidadesCofre[index] += cantidad;

            GuardarCofre();
        }
    }

    private void PasarALaMochila(int index, int cantidad)
    {
        if (CantidadesCofre[index] >= cantidad)
        {
            CantidadesCofre[index] -= cantidad;
            Inventario.Cantidades[index] += cantidad;

            GuardarCofre();
        }
    }

    private bool CantidadValida()
    {
        if (ItemSeleccionado == null) return true;

        if (ItemVieneDeMochila)
        {
            // ===== HACIA COFRE =====
            int cantidadActual = CantidadesCofre[IndexItemSeleccionado];
            int espacioDisponible = CantidadAlmacenableCofre - cantidadActual;

            return CantidadAPasar <= espacioDisponible &&
                   CantidadAPasar <= Inventario.Cantidades[IndexItemSeleccionado];
        }
        else
        {
            // ===== HACIA MOCHILA =====
            int limite = ObtenerLimiteActual();

            // 🔥 SOLO el item seleccionado
            int cantidadActual = Inventario.Cantidades[IndexItemSeleccionado];

            int espacioDisponible = limite - cantidadActual;
            return CantidadAPasar <= espacioDisponible &&
                   CantidadAPasar <= CantidadesCofre[IndexItemSeleccionado];
        }
    }

    private int ObtenerLimiteActual()
    {
        // Leer los PlayerPrefs en orden descendente de prioridad
        string mochila4 = PlayerPrefs.GetString("Habilidad:Mochila IV", "No");
        string mochila3 = PlayerPrefs.GetString("Habilidad:Mochila III", "No");
        string mochila2 = PlayerPrefs.GetString("Habilidad:Mochila II", "No");
        string mochila1 = PlayerPrefs.GetString("Habilidad:Mochila I", "No");

        // Determinar el límite según la mochila activa más alta
        if (mochila4 == "Si") return 100;
        if (mochila3 == "Si") return 80;
        if (mochila2 == "Si") return 60;
        if (mochila1 == "Si") return 40;
        return 20;
    }
}