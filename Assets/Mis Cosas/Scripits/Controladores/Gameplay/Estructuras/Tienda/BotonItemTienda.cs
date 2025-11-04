using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotonItemTienda : MonoBehaviour
{
    [Tooltip("Tipo de acción del botón: +, -, Mitad o Maximo")]
    public TipoBoton tipo;

    private TextMeshProUGUI textoCantidad;
    private int cantidadMaxima = 0;
    private int indiceInventario = -1;  // 🔹 Nuevo
    private Tienda_3D Tienda;

    private void Start()
    {
        // Buscar la tienda en los padres del prefab
        Tienda = GetComponentInParent<Tienda_3D>();

        // Buscar el texto de cantidad
        Transform parentItem = transform.parent;
        if (parentItem != null)
        {
            var t = parentItem.Find("Cantidad");
            if (t != null) textoCantidad = t.GetComponent<TextMeshProUGUI>();
            else
            {
                var candidate = parentItem.GetChild(3);
                if (candidate != null)
                    textoCantidad = candidate.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    public void EjecutarAccion()
    {
        if (textoCantidad == null) return;

        int cantidadActual = 0;
        int.TryParse(textoCantidad.text, out cantidadActual);

        switch (tipo)
        {
            case TipoBoton.Mas:
                cantidadActual++;
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
                    cantidadActual = Mathf.RoundToInt(cantidadActual / 2f);
                break;

            case TipoBoton.Maximo:
                if (cantidadMaxima > 0)
                    cantidadActual = cantidadMaxima;
                break;
        }

        textoCantidad.text = cantidadActual.ToString();

        // 🔹 Actualizar el registro en la Tienda
        if (Tienda != null && indiceInventario >= 0)
        {
            Tienda.ActualizarCantidadSeleccionada(indiceInventario, cantidadActual);
            Tienda.RecalcularDineroAPagar();
        }
    }

    // 🔹 Método llamado desde Tienda_3D
    public void AsignarCantidadMaxima(int max)
    {
        cantidadMaxima = max;
    }

    // 🔹 Nuevo método para identificar qué ítem representa este botón
    public void AsignarIndiceInventario(int indice)
    {
        indiceInventario = indice;
    }
}

public enum TipoBoton
{
    Mas,
    Menos,
    Mitad,
    Maximo
}
