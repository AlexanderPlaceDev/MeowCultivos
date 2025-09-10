using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ActivadorElementos : MonoBehaviour
{
    [SerializeField] private bool EsEvento;
    [SerializeField] private string NombreEvento;
    [SerializeField] private string HabilidadNecesaria;
    [SerializeField] private int HoraMinima;
    [SerializeField] private int MinutosMinimos;
    [SerializeField] private int HoraMaxima;
    [SerializeField] private int MinutosMaximos;

    [Header("Condiciones extra")]
    [SerializeField] private string[] DiasValidos;        // Ej: LUN, MAR, MIE
    [Range(0f, 1f)]
    [SerializeField] private float ProbabilidadDia = 1f;  // 1 = 100%, 0.5 = 50%, etc.

    [SerializeField] private GameObject[] ElementosAActivar;

    private Scr_ControladorTiempo Tiempo;

    // Control interno
    private string diaUltimaRevision = "";
    private bool diaPermitido = true;
    private bool rangoActivoContinuado = false; // para permitir seguir tras medianoche

    void Start()
    {
        Tiempo = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
    }

    void Update()
    {
        // Habilidad
        string keyHabilidad = "Habilidad:" + HabilidadNecesaria;
        bool tieneHabilidad = PlayerPrefs.GetString(keyHabilidad, "No") == "Si";

        // Hora actual en minutos totales (00:00 = 0, 23:59 = 1439)
        float horaActual = Tiempo.HoraActual;
        float minutoActual = Tiempo.MinutoActual;
        float actualEnMinutos = horaActual * 60f + minutoActual;

        // Rango configurado convertido a minutos totales
        float rangoMin = HoraMinima * 60f + MinutosMinimos;
        float rangoMax = HoraMaxima * 60f + MinutosMaximos;

        bool dentroDeRango;
        if (rangoMin <= rangoMax)
        {
            // Caso normal: dentro del mismo día
            dentroDeRango = actualEnMinutos >= rangoMin && actualEnMinutos <= rangoMax;
        }
        else
        {
            // Caso que cruza medianoche: ej. 22:00 a 02:00
            dentroDeRango = (actualEnMinutos >= rangoMin && actualEnMinutos <= 1439f) ||
                            (actualEnMinutos >= 0f && actualEnMinutos <= rangoMax);
        }

        // Evento
        bool eventoValido = true;
        if (EsEvento)
        {
            string key = "Cinematica " + NombreEvento;
            eventoValido = PlayerPrefs.GetString(key, "No") != "Si";
        }

        // Día actual
        string diaActual = Tiempo.DiaActual;

        // Si no estamos en rango, reseteamos el "modo continuado"
        if (!dentroDeRango)
        {
            rangoActivoContinuado = false;
        }

        // ✅ Verificar días válidos con probabilidad
        bool diaCorrecto = true;
        if (DiasValidos != null && DiasValidos.Length > 0)
        {
            // Si el rango apenas empieza, hacemos el sorteo
            if (!rangoActivoContinuado && dentroDeRango)
            {
                // Nuevo rango = nuevo chequeo
                if (diaUltimaRevision != diaActual)
                {
                    diaUltimaRevision = diaActual;
                    diaPermitido = Random.value <= ProbabilidadDia;
                }

                // Validamos día + probabilidad
                diaCorrecto = System.Array.Exists(DiasValidos, d => d == diaActual) && diaPermitido;

                // Si entra, mantenemos el "modo continuado"
                if (diaCorrecto) rangoActivoContinuado = true;
            }
            else if (rangoActivoContinuado)
            {
                // Si ya estaba activo antes de la medianoche, seguimos hasta que termine rango
                diaCorrecto = true;
            }
            else
            {
                diaCorrecto = false;
            }
        }

        // ✅ Condiciones finales
        bool condicionesCumplidas = tieneHabilidad && dentroDeRango && eventoValido && diaCorrecto;

        foreach (var elemento in ElementosAActivar)
        {
            if (elemento != null)
            {
                elemento.SetActive(condicionesCumplidas);
            }
        }

    }
}
