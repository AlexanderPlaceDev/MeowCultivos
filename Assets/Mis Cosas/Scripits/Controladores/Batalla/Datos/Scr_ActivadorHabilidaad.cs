using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ActivadorHabilidad : MonoBehaviour
{
    [SerializeField] float TiempoActual = 0;
    [SerializeField] float TiempoMaximo = 5;
    [SerializeField] float PuntosActuales;
    [SerializeField] GameObject TextoTiempo;
    [SerializeField] GameObject Bloqueo;
    [SerializeField] bool EsFinal;
    [SerializeField] GameObject Porcentaje;
    [SerializeField] Sprite[] IconosBloqueo;
    [SerializeField] Color[] Colores;

    void Start()
    {

    }

    void Update()
    {
        if (EsFinal)
        {
            if (PuntosActuales >= 100)
            {
                Bloqueo.SetActive(false);
                TextoTiempo.SetActive(false);

            }
            else
            {
                Bloqueo.SetActive(true);
                TextoTiempo.SetActive(true);
                TextoTiempo.GetComponent<TextMeshProUGUI>().text = ((int)PuntosActuales).ToString();
                if (PuntosActuales > 66)
                {
                    Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[2];
                    TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[2];
                    Porcentaje.GetComponent<TextMeshProUGUI>().color = Colores[2];

                }
                else
                {
                    if (PuntosActuales > 33)
                    {
                        Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[1];
                        Porcentaje.GetComponent<TextMeshProUGUI>().color = Colores[1];
                        TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[1];

                    }
                    else
                    {

                        Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[0];
                        TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[0];
                        Porcentaje.GetComponent<TextMeshProUGUI>().color = Colores[0];
                    }
                }
            }

        }
        else
        {
            if (TiempoActual > 0)
            {
                // Reducir el tiempo actual
                TiempoActual -= Time.deltaTime;

                // Mostrar el texto de tiempo restante
                Bloqueo.SetActive(true);
                TextoTiempo.SetActive(true);

                TextoTiempo.GetComponent<TextMeshProUGUI>().text = ((int)TiempoActual + 1).ToString();

                // Actualizar el sprite del bloqueo según el porcentaje de tiempo
                float porcentajeTiempo = TiempoActual / TiempoMaximo;

                if (porcentajeTiempo > 0.66f)
                {
                    Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[0];
                    TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[0];
                }
                else if (porcentajeTiempo > 0.33f)
                {
                    Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[1];
                    TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[1];
                }
                else
                {
                    Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[2];
                    TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[2];
                }
            }
            else
            {
                // Asegurarse de que el tiempo no sea negativo
                TiempoActual = 0;

                // Ocultar el bloqueo y el texto de tiempo
                Bloqueo.SetActive(false);
                TextoTiempo.SetActive(false);
            }
        }




    }
}