using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_CargaTeclas : MonoBehaviour
{
    public Image imagen;
    public float valor; // Este valor debe estar entre 0 y 1
    public float velocidadTransicion = 1.0f; // Velocidad de transici�n

    private Color colorInicio = Color.red;
    private Color colorMedio = Color.yellow;
    private Color colorFinal = Color.green;

    void Update()
    {
        // Aseg�rate de que el valor est� dentro del rango v�lido (0 a 1)
        valor = imagen.fillAmount;

        // Interpola entre los colores bas�ndose en el valor
        Color colorActual;
        if (valor < 0.5f)
        {
            // Interpola desde el colorInicio hasta el colorMedio
            colorActual = Color.Lerp(colorInicio, colorMedio, valor * 2f);
        }
        else
        {
            // Interpola desde el colorMedio hasta el colorFinal
            colorActual = Color.Lerp(colorMedio, colorFinal, (valor - 0.5f) * 2f);
        }

        // Asigna el color resultante a la imagen
        imagen.color = colorActual;
    }
}