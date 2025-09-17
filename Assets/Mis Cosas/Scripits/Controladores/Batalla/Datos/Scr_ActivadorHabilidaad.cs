using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ActivadorHabilidad : MonoBehaviour
{
    [SerializeField] float TiempoActual = 0;
    [SerializeField] float TiempoMaximo = 5;
    [SerializeField] GameObject TextoTiempo;
    [SerializeField] GameObject Bloqueo;
    [SerializeField] bool EsFinal;
    [SerializeField] bool Eshabilidad1;
    [SerializeField] GameObject Porcentaje;
    [SerializeField] Sprite[] IconosBloqueo;
    [SerializeField] Color[] Colores;
    [SerializeField] KeyCode Tecla;

    // Referencia al script Scr_Habilidades
    private Scr_Habilidades habilidades;

    Scr_ControladorBatalla ControladorBatalla;

    void Start()
    {
        ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        // Buscar el script Scr_Habilidades en el mismo objeto o en otro específico
        habilidades = GetComponent<Scr_Habilidades>();
    }

    void Update()
    {
        ActivarHabilidad();
        if (EsFinal)
        {
            if (ControladorBatalla.PuntosActualesHabilidad >= 100)
            {
                Bloqueo.SetActive(false);
                TextoTiempo.SetActive(false);
                Porcentaje.SetActive(false);
            }
            else
            {
                Bloqueo.SetActive(true);
                TextoTiempo.SetActive(true);
                Porcentaje.SetActive(true);
                TextoTiempo.GetComponent<TextMeshProUGUI>().text = ((int)ControladorBatalla.PuntosActualesHabilidad).ToString();
                if (ControladorBatalla.PuntosActualesHabilidad > 66)
                {
                    Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[2];
                    TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[2];
                    Porcentaje.GetComponent<TextMeshProUGUI>().color = Colores[2];

                }
                else
                {
                    if (ControladorBatalla.PuntosActualesHabilidad > 33)
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
                TiempoActual -= Time.deltaTime;

                Bloqueo.SetActive(true);
                TextoTiempo.SetActive(true);

                TextoTiempo.GetComponent<TextMeshProUGUI>().text = ((int)TiempoActual + 1).ToString();

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
                TiempoActual = 0;

                Bloqueo.SetActive(false);
                TextoTiempo.SetActive(false);
            }
        }
    }

    private void ActivarHabilidad()
    {
        if (Input.GetKeyDown(Tecla))
        {
            if (EsFinal)
            {
                if (ControladorBatalla.PuntosActualesHabilidad >= 100)
                {
                    // Reiniciar puntos de habilidad final
                    ControladorBatalla.PuntosActualesHabilidad = 0;

                    // Activar una habilidad final del script Scr_Habilidades
                    if (habilidades != null)
                    {
                        habilidades.Habilidad(ControladorBatalla.HabilidadEspecial); // Cambiar por la habilidad deseada
                    }
                    else
                    {
                        Debug.LogWarning("No se encontró el script Scr_Habilidades");
                    }
                }

            }
            else
            {
                if (habilidades != null)
                {
                    if (TiempoActual == 0)
                    {
                        if (Eshabilidad1)
                        {
                            habilidades.Habilidad(ControladorBatalla.Habilidad1); // Cambiar por la habilidad deseada
                        }
                        else
                        {
                            habilidades.Habilidad(ControladorBatalla.Habilidad2); // Cambiar por la habilidad deseada
                        }
                        TiempoActual = TiempoMaximo;
                    }

                }
                else
                {
                    Debug.LogWarning("No se encontró el script Scr_Habilidades");
                }
            }
        }
    }
}
