using System.Collections.Generic;
using UnityEngine;

public class Scr_Linterna : MonoBehaviour
{
    [SerializeField] private Scr_ControladorTiempo Tiempo;  // Referencia al controlador del tiempo
    [SerializeField] private GameObject Gata;               // Modelo de la gata (para ajustar materiales)
    [SerializeField] private int HoraEncender = 20;         // Hora para encender (20:00)
    [SerializeField] private int HoraApagar = 8;            // Hora para apagar (08:00)

    private Light luz;                                      // Luz del objeto
    private Material[] materialesGata;                      // Materiales del modelo
    private float temporizador;                             // Control del intervalo
    private bool linternaEncendida;                         // Estado actual

    void Start()
    {
        // Buscar referencias si no están asignadas
        if (Tiempo == null)
            Tiempo = FindObjectOfType<Scr_ControladorTiempo>();

        luz = GetComponent<Light>();

        if (Gata != null)
        {
            // Recolecta todos los materiales de la gata
            var renderers = Gata.GetComponentsInChildren<Renderer>();
            var mats = new List<Material>();
            foreach (var r in renderers)
                mats.AddRange(r.materials);
            materialesGata = mats.ToArray();
        }

        // Sincroniza al inicio
        VerificarHoraYLuz();
    }

    void Update()
    {
        temporizador += Time.deltaTime;

        if (temporizador >= 5f)
        {
            temporizador = 0f;
            VerificarHoraYLuz();
        }
    }

    void VerificarHoraYLuz()
    {
        if (Tiempo == null || PlayerPrefs.GetString("Habilidad:Linterna", "No") == "No") return;

        int hora = Tiempo.HoraActual;
        bool debeEncender = EstaEnRango(hora, HoraEncender, HoraApagar);

        if (debeEncender != linternaEncendida)
        {
            linternaEncendida = debeEncender;
            ActualizarEstado(linternaEncendida);
        }
    }

    bool EstaEnRango(int horaActual, int horaInicio, int horaFin)
    {
        // Si el rango cruza medianoche (ej: 20 → 8)
        if (horaInicio > horaFin)
            return (horaActual >= horaInicio || horaActual < horaFin);
        else
            return (horaActual >= horaInicio && horaActual < horaFin);
    }

    void ActualizarEstado(bool encendida)
    {
        if (luz != null)
            luz.enabled = encendida;

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
