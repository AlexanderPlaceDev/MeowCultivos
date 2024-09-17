using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorTiempo : MonoBehaviour
{
    public int DuracionDelDiaPorHora; // Duración en segundos de una hora en el juego
    public string DiaActual = "Lunes"; // Día inicial del juego
    public int HoraActual = 6; // Hora inicial, por ejemplo 6:00 AM
    public float MinutoActual = 0; // Minuto actual, avanza en intervalos de 10 minutos
    [SerializeField] Sprite[] IconosClima; // Iconos del clima para representar visualmente
    [SerializeField] Image ImagenClima;
    [SerializeField] TextMeshProUGUI TextoFecha; // Texto que muestra la fecha actual
    [SerializeField] TextMeshProUGUI TextoHora; // Texto que muestra la hora actual

    private float tiempoTranscurrido = 0; // Controla el tiempo que pasa entre frames
    private int[] intervalosMinutos = { 0, 10, 20, 30, 40, 50 }; // Intervalos de 10 minutos

    void Start()
    {
        // Cargar la fecha y hora desde PlayerPrefs si ya existen
        DiaActual = PlayerPrefs.GetString("DiaActual", "Lunes");
        HoraActual = PlayerPrefs.GetInt("HoraActual", 6);
        MinutoActual = PlayerPrefs.GetFloat("MinutoActual", 0);

        ActualizarTextoFecha();
        ActualizarTextoHora();
    }

    void Update()
    {
        // Incrementa el tiempo transcurrido
        tiempoTranscurrido += Time.deltaTime;

        // Si el tiempo transcurrido es mayor a la duración de una "hora" en el juego
        if (tiempoTranscurrido >= DuracionDelDiaPorHora / 6f) // Cada 1/6 de DuracionDelDiaPorHora es equivalente a 10 minutos
        {
            // Reinicia el contador de tiempo
            tiempoTranscurrido = 0;

            // Actualiza los minutos en bloques de 10
            ActualizarMinuto();

            // Actualiza los textos de la fecha y hora
            ActualizarTextoFecha();
            ActualizarTextoHora();

            // Guardar la fecha y hora actualizadas en PlayerPrefs
            GuardarTiempo();
        }
    }

    void ActualizarMinuto()
    {
        // Encuentra el siguiente intervalo de minutos
        int indexMinutos = System.Array.IndexOf(intervalosMinutos, (int)MinutoActual);
        if (indexMinutos < intervalosMinutos.Length - 1)
        {
            // Avanza al siguiente intervalo de 10 minutos
            MinutoActual = intervalosMinutos[indexMinutos + 1];
        }
        else
        {
            // Si ya llegó a 50 minutos, avanza la hora
            MinutoActual = 0;
            ActualizarHora();
        }
    }

    void ActualizarHora()
    {
        // Avanza la hora
        HoraActual++;

        // Si pasa de las 24 horas, reinicia el día
        if (HoraActual >= 24)
        {
            HoraActual = 0;
            ActualizarDia();
        }
    }

    void ActualizarDia()
    {
        // Cambiar de día cuando termina el ciclo de 24 horas
        string[] dias = { "LUN", "MAR", "MIE", "JUE", "VIE", "SAB", "DOM" };
        int diaActualIndex = System.Array.IndexOf(dias, DiaActual);

        DiaActual = dias[(diaActualIndex + 1) % dias.Length]; // Cambia al siguiente día
    }

    void ActualizarTextoFecha()
    {
        // Actualiza el texto de la fecha con el día actual
        TextoFecha.text = DiaActual;
    }

    void ActualizarTextoHora()
    {
        // Formatear la hora para que siempre muestre dos dígitos
        string horaFormateada = HoraActual.ToString("00");
        string minutoFormateado = ((int)MinutoActual).ToString("00");
        TextoHora.text = horaFormateada + ":" + minutoFormateado;
    }

    void GuardarTiempo()
    {
        // Guardar los datos en PlayerPrefs
        PlayerPrefs.SetString("DiaActual", DiaActual);
        PlayerPrefs.SetInt("HoraActual", HoraActual);
        PlayerPrefs.SetFloat("MinutoActual", MinutoActual);

        // Asegurar que los datos se guardan
        PlayerPrefs.Save();
    }
}
