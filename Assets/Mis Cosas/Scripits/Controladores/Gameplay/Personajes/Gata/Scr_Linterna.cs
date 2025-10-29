using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Linterna : MonoBehaviour
{
    [SerializeField] GameObject Gata;                   // Modelo de la gata
    [SerializeField] Scr_ControladorTiempo Tiempo;      // Referencia al controlador del tiempo
    [SerializeField] int HoraEncender = 16;             // Hora para encender (por defecto 16)
    [SerializeField] int HoraApagar = 6;                // Hora para apagar (por defecto 6)
    [SerializeField] string Habilidad;

    private Light pointLight;                           // Luz del mismo objeto
    private Material[] materialesGata;                  // Todos los materiales de la gata
    private bool linternaEncendida;                     // Estado actual

    void Start()
    {

        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si")
        {
            // Buscar la luz del mismo objeto
            pointLight = GetComponent<Light>();

            // Buscar los materiales de la gata
            if (Gata != null)
            {
                Renderer[] renderers = Gata.GetComponentsInChildren<Renderer>();
                List<Material> mats = new List<Material>();

                foreach (Renderer rend in renderers)
                    mats.AddRange(rend.materials);

                materialesGata = mats.ToArray();
            }

            // 🔥 Forzar una comprobación inicial
            ActualizarEstadoSegunHora();
        }


    }

    void LateUpdate()
    {
        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si")
        {
            if (pointLight == null)
            {
                // Buscar la luz del mismo objeto
                pointLight = GetComponent<Light>();

                if (Gata != null)
                {
                    Renderer[] renderers = Gata.GetComponentsInChildren<Renderer>();
                    List<Material> mats = new List<Material>();

                    foreach (Renderer rend in renderers)
                        mats.AddRange(rend.materials);

                    materialesGata = mats.ToArray();
                }
            }
            // Verificar cada frame si la hora cambió y actualizar si es necesario
            ActualizarEstadoSegunHora();
        }
    }

    void ActualizarEstadoSegunHora()
    {
        if (Tiempo == null) return;

        int hora = Tiempo.HoraActual;
        bool debeEstarEncendida;

        // Determinar si debe estar encendida (soporta paso por medianoche)
        if (HoraEncender > HoraApagar)
        {
            // Ejemplo: encender desde 16 hasta 23, y de 0 a antes de 6
            debeEstarEncendida = (hora >= HoraEncender || hora < HoraApagar);
        }
        else
        {
            // Ejemplo: encender desde 6 hasta 20
            debeEstarEncendida = (hora >= HoraEncender && hora < HoraApagar);
        }

        // Si el estado cambió o si estamos al inicio
        if (debeEstarEncendida != linternaEncendida || pointLight.enabled != debeEstarEncendida)
        {
            linternaEncendida = debeEstarEncendida;
            ActualizarEstadoLinterna(linternaEncendida);
        }
    }

    void ActualizarEstadoLinterna(bool encendida)
    {
        // Encender o apagar la luz
        if (pointLight != null)
            pointLight.enabled = encendida;

        // Ajustar materiales
        if (materialesGata != null)
        {
            foreach (Material mat in materialesGata)
            {
                if (mat.HasProperty("_Intensidad"))
                    mat.SetFloat("_Intensidad", encendida ? 5f : 1f);
            }
        }

        Debug.Log($"[Scr_Linterna] {(encendida ? "Encendida" : "Apagada")} a las {Tiempo.HoraActual}h");
    }
}
