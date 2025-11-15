using System;
using UnityEngine;

public class Scr_Inventario : MonoBehaviour
{
    [SerializeField] public Scr_CreadorObjetos[] Objetos;
    [SerializeField] public int[] Cantidades;

    public Scr_ControladorMisiones ControladorMisiones;
    public event Action OnInventarioActualizado;
    private Scr_ObjetosAgregados objetosAgregados;

    // Control de guardado/eficiencia
    private bool inventarioModificado = false;
    private int[] ultimaCopiaCantidades;

    private void Awake()
    {
        // Asegurarnos que los arrays existan y tengan el mismo tamaño
        if (Objetos == null) Objetos = new Scr_CreadorObjetos[0];
        if (Cantidades == null || Cantidades.Length != Objetos.Length)
        {
            Cantidades = new int[Objetos.Length];
        }

        // Inicializar copia para detectar cambios si fuera necesario
        ultimaCopiaCantidades = new int[Cantidades.Length];
    }

    private void Start()
    {
        // Cargar las cantidades desde PlayerPrefs al iniciar (sólo si existen claves)
        for (int i = 0; i < Objetos.Length; i++)
        {
            if (Objetos[i] == null) continue;
            string key = Objetos[i].Nombre + "_Cantidad";
            if (PlayerPrefs.HasKey(key))
            {
                Cantidades[i] = PlayerPrefs.GetInt(key);
            }
            else
            {
                PlayerPrefs.SetInt(key, Cantidades[i]); // Guardar inicial si no existe
            }

            ultimaCopiaCantidades[i] = Cantidades[i];
        }

        // Buscar referencias
        var gata = GameObject.Find("Gata");
        if (gata)
        {
            // aquí asumo que el controlador misiones está en child index 4 como antes
            var posible = gata.transform.GetChild(4).GetComponent<Scr_ControladorMisiones>();
            if (posible != null) ControladorMisiones = posible;
        }

        objetosAgregados = FindObjectOfType<Scr_ObjetosAgregados>();
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

    public void AgregarObjeto(int cantidad, string nombre)
    {
        if (Objetos == null) return;

        // ✅ Obtener límite actualizado según mochilas
        int limiteActual = ObtenerLimiteActual();
        Debug.Log("Limite:" + limiteActual);

        for (int i = 0; i < Objetos.Length; i++)
        {
            var Objeto = Objetos[i];
            if (Objeto == null) continue;

            if (Objeto.Nombre == nombre)
            {
                // Aplicar límite dinámico
                if (Cantidades[i] + cantidad > limiteActual)
                {
                    Cantidades[i] = limiteActual;
                }
                else
                {
                    Cantidades[i] += cantidad;
                }

                // Agregar XP si aplica
                if (Objeto.XPRecolecta > 0 && objetosAgregados != null)
                {
                    objetosAgregados.AgregarExperiencia(Objeto.XPRecolecta);
                }

                inventarioModificado = true;
                OnInventarioActualizado?.Invoke();
                return;
            }
        }

        Debug.LogWarning($"AgregarObjeto: no se encontró '{nombre}' en Inventario.");
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


    public void QuitarObjeto(int cantidad, string nombre)
    {
        if (Objetos == null) return;

        for (int i = 0; i < Objetos.Length; i++)
        {
            var Objeto = Objetos[i];
            if (Objeto == null) continue;

            if (Objeto.Nombre == nombre)
            {
                if (Cantidades[i] > cantidad)
                {
                    Cantidades[i] -= cantidad;
                }
                else
                {
                    Cantidades[i] = 0;
                }

                inventarioModificado = true;
                OnInventarioActualizado?.Invoke();
                return;
            }
        }

        Debug.LogWarning($"QuitarObjeto: no se encontró '{nombre}' en Inventario.");
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
