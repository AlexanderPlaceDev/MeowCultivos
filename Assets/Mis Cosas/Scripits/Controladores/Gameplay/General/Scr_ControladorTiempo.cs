﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorTiempo : MonoBehaviour
{
    public int DuracionDelDiaPorHora; // Duración en segundos de una hora en el juego
    public string DiaActual = "LUN"; // Día inicial del juego
    public int HoraActual = 10; // Hora inicial, por ejemplo, 6:00 AM
    public float MinutoActual = 0; // Minuto actual, avanza en intervalos de 10 minutos
    [SerializeField] Image ImagenClima;
    [SerializeField] int[] HorasClima; // Horas en las que cambia el icono del clima
    [SerializeField] Sprite[] IconosClima; // Iconos del clima para representar visualmente
    [SerializeField] TextMeshProUGUI TextoFecha; // Texto que muestra la fecha actual
    [SerializeField] TextMeshProUGUI TextoHora; // Texto que muestra la hora actual
    [SerializeField] Light Sol; // Direccional Light que representa el sol
    [SerializeField] Color[] ColoresDelDia; // Array de colores para cada parte del día

    // Variables para el Skybox
    public Material SkyboxDia; // Skybox para el día
    public Material SkyboxNoche; // Skybox para la noche
    public int HoraInicioDia = 6; // Hora en la que empieza el día (6 AM)
    public int HoraInicioNoche = 18; // Hora en la que empieza la noche (6 PM)

    private float rotacionSkybox = 0f; // Controlar la rotación del Skybox

    private float tiempoTranscurrido = 0; // Controla el tiempo que pasa entre frames
    private int[] intervalosMinutos = { 0, 10, 20, 30, 40, 50 }; // Intervalos de 10 minutos

    public int DineroRecompensa; //Dinero a entregar por la caja de venta

    public enum climas
    {
        Despejado,
        Nublado,
        Lluvioso,
        Nevado,
        Tormenta
    }
    public List<climas> ClimaSemanal;
    public List<int> ProbabilidadesClimaSemanal = new List<int>(); // Probabilidad de que ese clima fuera elegido

    void Awake()
    {
        // Cargar la fecha y hora desde PlayerPrefs si ya existen
        DiaActual = PlayerPrefs.GetString("DiaActual", "LUN");
        HoraActual = PlayerPrefs.GetInt("HoraActual", 11);
        MinutoActual = PlayerPrefs.GetFloat("MinutoActual", 0);
        DineroRecompensa = PlayerPrefs.GetInt("DineroCajaVenta", 0);

        ActualizarTextoFecha();
        ActualizarTextoHora();

        // Actualizar el icono del clima al iniciar
        ActualizarIconoClima();

        // Actualizar el Skybox inicial
        ActualizarSkybox();
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

            // Actualizar el icono del clima
            ActualizarIconoClima();
        }

        // Cambia el color del sol dependiendo de la hora del día
        ActualizarColorSol();

        // Cambia el skybox dependiendo de si es de día o de noche
        ActualizarSkybox();

        // Actualizar la rotación del Skybox
        RotarSkybox();

        EntregarDineroCajaVenta();
    }
    public void LimpiarClimaSemanal()
    {
        ClimaSemanal.Clear();
        ProbabilidadesClimaSemanal.Clear();
    }
    public void climaSemanal()
    {
        if (ClimaSemanal.Count == 0)
        {
            System.Random random = new System.Random();

            for (int i = 0; i < 7; i++)
            {
                // Elegir un valor aleatorio del enum
                int climaIndex = random.Next(0, System.Enum.GetValues(typeof(climas)).Length);
                climas climaAleatorio = (climas)climaIndex;
                ProbabilidadesClimaSemanal.Add(random.Next(0, 100));
                ClimaSemanal.Add(climaAleatorio);
            }
        }
    }
    public string checarDia()
    {
        string[] dias = { "LUN", "MAR", "MIE", "JUE", "VIE", "SAB", "DOM" };
        int diaActualIndex = System.Array.IndexOf(dias, DiaActual);

        string diaManana = dias[(diaActualIndex + 1) % dias.Length];
        return diaManana;
    }

    public void EntregarDineroCajaVenta()
    {
        if (DineroRecompensa != 0)
        {
            PlayerPrefs.SetInt("DineroCajaVenta", DineroRecompensa);
            if (HoraActual == 0 && MinutoActual == 0)
            {

                GameObject.Find("ObjetosAgregados").GetComponent<Scr_ObjetosAgregados>().AgregarDinero(DineroRecompensa);
                DineroRecompensa = 0;
                PlayerPrefs.DeleteKey("DineroCajaVenta");
            }
        }
    }

    public void Descansar(float m, int h)
    {
        tiempoTranscurrido = 0;
        MinutoActual = m;
        HoraActual = h;
        ActualizarDia();

        ActualizarTextoFecha();
        ActualizarTextoHora();
        
        GuardarTiempo();
        ActualizarIconoClima();
    }
    void ActualizarMinuto()
    {
        int indexMinutos = System.Array.IndexOf(intervalosMinutos, (int)MinutoActual);
        if (indexMinutos < intervalosMinutos.Length - 1)
        {
            MinutoActual = intervalosMinutos[indexMinutos + 1];
        }
        else
        {
            MinutoActual = 0;
            ActualizarHora();
        }
    }

    void ActualizarHora()
    {
        HoraActual++;
        if (HoraActual >= 24)
        {
            HoraActual = 0;
            ActualizarDia();
        }
    }

    void ActualizarDia()
    {
        string[] dias = { "LUN", "MAR", "MIE", "JUE", "VIE", "SAB", "DOM" };
        int diaActualIndex = System.Array.IndexOf(dias, DiaActual);
        DiaActual = dias[(diaActualIndex + 1) % dias.Length];
    }

    void ActualizarTextoFecha()
    {
        TextoFecha.text = DiaActual;
    }

    void ActualizarTextoHora()
    {
        string horaFormateada = HoraActual.ToString("00");
        string minutoFormateado = ((int)MinutoActual).ToString("00");
        TextoHora.text = horaFormateada + ":" + minutoFormateado;
    }

    void GuardarTiempo()
    {
        PlayerPrefs.SetString("DiaActual", DiaActual);
        PlayerPrefs.SetInt("HoraActual", HoraActual);
        PlayerPrefs.SetFloat("MinutoActual", MinutoActual);
        PlayerPrefs.Save();
    }

    void ActualizarColorSol()
    {
        float horaFraccionada = HoraActual + (MinutoActual / 60f);
        int indiceColorActual = Mathf.FloorToInt(horaFraccionada / (24f / ColoresDelDia.Length));
        int indiceSiguienteColor = (indiceColorActual + 1) % ColoresDelDia.Length;
        float t = (horaFraccionada % (24f / ColoresDelDia.Length)) / (24f / ColoresDelDia.Length);
        Color colorActual = Color.Lerp(ColoresDelDia[indiceColorActual], ColoresDelDia[indiceSiguienteColor], t);
        Sol.color = colorActual;
    }

    void ActualizarIconoClima()
    {
        if (HorasClima.Length != IconosClima.Length || HorasClima.Length == 0)
        {
            Debug.LogError("El tamaño de HorasClima y IconosClima no coincide o están vacíos.");
            return;
        }

        float horaFraccionada = HoraActual + (MinutoActual / 60f);
        int indiceIcono = 0;
        for (int i = 0; i < HorasClima.Length; i++)
        {
            if (HoraActual >= HorasClima[i])
            {
                indiceIcono = i;
            }
            else
            {
                break;
            }
        }

        indiceIcono = Mathf.Clamp(indiceIcono, 0, IconosClima.Length - 1);
        ImagenClima.sprite = IconosClima[indiceIcono];
    }

    // Función para cambiar el skybox entre día y noche
    void ActualizarSkybox()
    {
        if (HoraActual >= HoraInicioDia && HoraActual < HoraInicioNoche)
        {
            RenderSettings.skybox = SkyboxDia; // Cambiar al skybox de día
        }
        else
        {
            RenderSettings.skybox = SkyboxNoche; // Cambiar al skybox de noche
        }
    }

    // Función para rotar el skybox suavemente
    void RotarSkybox()
    {
        // Rotar el skybox un poco cada frame
        rotacionSkybox += Time.deltaTime * 1f; // Cambia el valor de 1f para ajustar la velocidad de rotación
        RenderSettings.skybox.SetFloat("_Rotation", rotacionSkybox);
    }
}
