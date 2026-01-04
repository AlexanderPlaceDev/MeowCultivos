using System;
using UnityEngine;

public class Scr_Inventario : MonoBehaviour
{
    [SerializeField] public Scr_CreadorObjetos[] Objetos;
    [SerializeField] public int[] Cantidades;

    public Scr_ControladorMisiones ControladorMisiones;
    public event Action OnInventarioActualizado;
    private Scr_ObjetosAgregados objetosAgregados;
    private Scr_DatosSingletonBatalla DatosBatalla;

    // Control de guardado/eficiencia
    private bool inventarioModificado = false;
    private int[] ultimaCopiaCantidades;

    private void Awake()
    {
        if (Objetos == null) Objetos = new Scr_CreadorObjetos[0];

        if (Cantidades == null || Cantidades.Length != Objetos.Length)
            Cantidades = new int[Objetos.Length];

        ultimaCopiaCantidades = new int[Cantidades.Length];

        // 🔹 CARGA REAL AQUÍ
        for (int i = 0; i < Objetos.Length; i++)
        {
            if (Objetos[i] == null) continue;

            string key = Objetos[i].Nombre + "_Cantidad";

            Cantidades[i] = PlayerPrefs.GetInt(key, Cantidades[i]);
            ultimaCopiaCantidades[i] = Cantidades[i];
        }
    }


    private void Start()
    {
        // Controlador misiones
        var gata = GameObject.Find("Gata");
        if (gata != null)
        {
            ControladorMisiones = gata.transform
                .GetChild(4)
                .GetComponent<Scr_ControladorMisiones>();
        }

        // 🔹 UI PRIMERO
        objetosAgregados = FindObjectOfType<Scr_ObjetosAgregados>();

        // 🔹 Singleton después
        var singletonGO = GameObject.Find("Singleton");
        if (singletonGO != null)
        {
            DatosBatalla = singletonGO.GetComponent<Scr_DatosSingletonBatalla>();
            if (DatosBatalla != null)
                ProcesarRecompensasPendientes();
        }

        // 🔹 Forzar actualización visual
        OnInventarioActualizado?.Invoke();
    }


    private void ProcesarRecompensasPendientes()
    {
        if (DatosBatalla.ObjetosRecompensa == null ||
            DatosBatalla.CantidadesRecompensa == null)
            return;

        int count = Mathf.Min(
            DatosBatalla.ObjetosRecompensa.Count,
            DatosBatalla.CantidadesRecompensa.Count
        );

        for (int i = 0; i < count; i++)
        {
            var obj = DatosBatalla.ObjetosRecompensa[i];
            int cant = DatosBatalla.CantidadesRecompensa[i];

            if (obj == null || cant <= 0)
                continue;

            // 🔹 INVENTARIO ES LA FUENTE DE VERDAD
            AgregarObjeto(obj.Nombre, cant, mostrarUI: true, darXP: true);
        }

        DatosBatalla.ObjetosRecompensa.Clear();
        DatosBatalla.CantidadesRecompensa.Clear();
    }



    private void Update()
    {
        // Si hubo modificaciones, guardamos una sola vez (evita PlayerPrefs cada frame)
        if (inventarioModificado)
        {
            GuardarInventario();
            inventarioModificado = false;
        }


        bool cambio = false;
        if (Cantidades != null && ultimaCopiaCantidades != null && Cantidades.Length == ultimaCopiaCantidades.Length)
        {
            for (int i = 0; i < Cantidades.Length; i++)
            {
                if (Cantidades[i] != ultimaCopiaCantidades[i])
                {
                    ultimaCopiaCantidades[i] = Cantidades[i];
                    cambio = true;
                }
            }
        }

        if (cambio)
        {
            inventarioModificado = true;
            OnInventarioActualizado?.Invoke();
        }

    }

    public int AgregarObjeto(
    string nombre,
    int cantidad,
    bool mostrarUI = true,
    bool darXP = true
)
    {
        if (cantidad <= 0 || Objetos == null)
            return 0;

        int limiteActual = ObtenerLimiteActual();
        int cantidadAgregada = 0;

        for (int i = 0; i < Objetos.Length; i++)
        {
            var obj = Objetos[i];
            if (obj == null) continue;

            if (obj.Nombre != nombre)
                continue;

            int espacioDisponible = limiteActual - Cantidades[i];
            if (espacioDisponible <= 0)
                break;

            cantidadAgregada = Mathf.Min(cantidad, espacioDisponible);
            Cantidades[i] += cantidadAgregada;

            inventarioModificado = true;
            break;
        }

        int excedente = cantidad - cantidadAgregada;

        // 🔹 UI OBJETOS AGREGADOS
        if (mostrarUI && objetosAgregados != null)
        {
            Scr_CreadorObjetos obj =
                System.Array.Find(Objetos, o => o != null && o.Nombre == nombre);

            if (obj != null)
            {
                objetosAgregados.Lista.Add(obj);

                if (cantidadAgregada > 0)
                {
                    objetosAgregados.Cantidades.Add(cantidadAgregada);
                    objetosAgregados.FueExcedente.Add(false);
                }
                else
                {
                    objetosAgregados.Cantidades.Add(cantidad);
                    objetosAgregados.FueExcedente.Add(true);
                }

                if (objetosAgregados.Tiempo != null &&
                    objetosAgregados.Lista.Count - 1 < objetosAgregados.Tiempo.Length)
                {
                    objetosAgregados.Tiempo[objetosAgregados.Lista.Count - 1] = 2f;
                }
            }
        }

        // 🔹 XP
        if (darXP && cantidadAgregada > 0)
        {
            Scr_CreadorObjetos obj =
                System.Array.Find(Objetos, o => o != null && o.Nombre == nombre);

            if (obj != null && obj.XPRecolecta > 0 && objetosAgregados != null)
            {
                objetosAgregados.AgregarExperiencia(
                    obj.XPRecolecta * cantidadAgregada
                );
            }
        }

        GuardarInventario();
        OnInventarioActualizado?.Invoke();

        return cantidadAgregada;
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


    public void QuitarObjeto(
    string nombre,
    int cantidad,
    bool mostrarUI = false
)
    {
        if (cantidad <= 0 || Objetos == null)
            return;

        for (int i = 0; i < Objetos.Length; i++)
        {
            var obj = Objetos[i];
            if (obj == null) continue;

            if (obj.Nombre != nombre)
                continue;

            int cantidadQuitada = Mathf.Min(cantidad, Cantidades[i]);
            Cantidades[i] -= cantidadQuitada;

            inventarioModificado = true;

            // 🔹 UI opcional (por ejemplo cofres que quitan)
            if (mostrarUI && objetosAgregados != null && cantidadQuitada > 0)
            {
                objetosAgregados.Lista.Add(obj);
                objetosAgregados.Cantidades.Add(cantidadQuitada);
                objetosAgregados.FueExcedente.Add(true);

                if (objetosAgregados.Tiempo != null &&
                    objetosAgregados.Lista.Count - 1 < objetosAgregados.Tiempo.Length)
                {
                    objetosAgregados.Tiempo[objetosAgregados.Lista.Count - 1] = 2f;
                }
            }

            break;
        }

        GuardarInventario();
        OnInventarioActualizado?.Invoke();
    }


    private void GuardarInventario()
    {
        for (int i = 0; i < Objetos.Length; i++)
        {
            if (Objetos[i] == null) continue;
            string key = Objetos[i].Nombre + "_Cantidad";
            // Guardar sólo si cambio
            if (PlayerPrefs.GetInt(key, -9999) != Cantidades[i])
            {
                PlayerPrefs.SetInt(key, Cantidades[i]);
            }
        }

        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        GuardarInventario();
    }

}
