using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotonItemTienda : MonoBehaviour
{
    [Tooltip("Tipo de acción del botón: +, -, Mitad o Maximo")]
    public TipoBoton tipo;

    private TextMeshProUGUI textoCantidad;
    private int cantidadMaxima = 0;
    private Tienda_3D Tienda;

    private void Start()
    {
        // Buscar la tienda en los padres del prefab (más seguro que parent.parent.parent...)
        Tienda = GetComponentInParent<Tienda_3D>();

        // Buscar el texto de cantidad en el mismo prefab (suponiendo que está en el padre directo)
        Transform parentItem = transform.parent;
        if (parentItem != null)
        {
            // Intentamos por nombre "Cantidad" y si no existe, probamos GetChild(3) por compatibilidad
            var t = parentItem.Find("Cantidad");
            if (t != null) textoCantidad = t.GetComponent<TextMeshProUGUI>();
            else
            {
                // fallback — según tu estructura inicial, child index 3 era la cantidad
                var candidate = parentItem.GetChild(3);
                if (candidate != null)
                    textoCantidad = candidate.GetComponent<TextMeshProUGUI>();
            }
        }

        // Nota: cantidadMaxima será asignada desde Tienda_3D al instanciar los prefabs.
    }

    public void EjecutarAccion()
    {
        if (textoCantidad == null) return;
        Debug.Log("Entra");

        int cantidadActual = 0;
        int.TryParse(textoCantidad.text, out cantidadActual);

        switch (tipo)
        {
            case TipoBoton.Mas:
                cantidadActual++;
                // No superar el máximo disponible
                if (cantidadMaxima > 0)
                    cantidadActual = Mathf.Min(cantidadActual, cantidadMaxima);
                break;

            case TipoBoton.Menos:
                cantidadActual = Mathf.Max(0, cantidadActual - 1);
                break;

            case TipoBoton.Mitad:
                if (cantidadMaxima > 0)
                    cantidadActual = Mathf.RoundToInt(cantidadMaxima / 2f);
                else
                    cantidadActual = Mathf.RoundToInt(cantidadActual / 2f); // fallback si no hay máximo
                break;

            case TipoBoton.Maximo:
                if (cantidadMaxima > 0)
                    cantidadActual = cantidadMaxima;
                break;
        }

        textoCantidad.text = cantidadActual.ToString();

        // 🔹 Recalcular el DineroAPagar de la Tienda
        if (Tienda != null)
        {
            Tienda.RecalcularDineroAPagar();
        }
    }

    // Método público opcional para definir el máximo desde otro script
    public void AsignarCantidadMaxima(int max)
    {
        cantidadMaxima = max;
    }
}

public enum TipoBoton
{
    Mas,
    Menos,
    Mitad,
    Maximo
}
