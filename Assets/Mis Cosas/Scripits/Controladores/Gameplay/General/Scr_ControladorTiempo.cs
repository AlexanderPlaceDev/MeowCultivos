using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorTiempo : MonoBehaviour
{
    public int DuracionDelDiaPorHora; // Duraci�n en segundos de una hora en el juego
    public string DiaActual = "LUN"; // D�a inicial del juego
    public int HoraActual = 10; // Hora inicial, por ejemplo, 6:00 AM
    public float MinutoActual = 0; // Minuto actual, avanza en intervalos de 10 minutos
    [SerializeField] Image ImagenClima;
    [SerializeField] int[] HorasClima; // Horas en las que cambia el icono del clima
    [SerializeField] Sprite[] IconosClima; // Iconos del clima para representar visualmente
    [SerializeField] TextMeshProUGUI TextoFecha; // Texto que muestra la fecha actual
    [SerializeField] TextMeshProUGUI TextoHora; // Texto que muestra la hora actual
    [SerializeField] Light Sol; // Direccional Light que representa el sol
    [SerializeField] Color[] ColoresDelDia; // Array de colores para cada parte del d�a

    // Variables para el Skybox
    public Material SkyboxDia; // Skybox para el d�a
    public Material SkyboxNoche; // Skybox para la noche
    public int HoraInicioDia = 6; // Hora en la que empieza el d�a (6 AM)
    public int HoraInicioNoche = 18; // Hora en la que empieza la noche (6 PM)

    private float rotacionSkybox = 0f; // Controlar la rotaci�n del Skybox

    private float tiempoTranscurrido = 0; // Controla el tiempo que pasa entre frames
    private int[] intervalosMinutos = { 0, 10, 20, 30, 40, 50 }; // Intervalos de 10 minutos

    void Awake()
    {
        // Cargar la fecha y hora desde PlayerPrefs si ya existen
        DiaActual = PlayerPrefs.GetString("DiaActual", "LUN");
        HoraActual = PlayerPrefs.GetInt("HoraActual", 11);
        MinutoActual = PlayerPrefs.GetFloat("MinutoActual", 0);

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

        // Si el tiempo transcurrido es mayor a la duraci�n de una "hora" en el juego
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

        // Cambia el color del sol dependiendo de la hora del d�a
        ActualizarColorSol();

        // Cambia el skybox dependiendo de si es de d�a o de noche
        ActualizarSkybox();

        // Actualizar la rotaci�n del Skybox
        RotarSkybox();
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
            Debug.LogError("El tama�o de HorasClima y IconosClima no coincide o est�n vac�os.");
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

    // Funci�n para cambiar el skybox entre d�a y noche
    void ActualizarSkybox()
    {
        if (HoraActual >= HoraInicioDia && HoraActual < HoraInicioNoche)
        {
            RenderSettings.skybox = SkyboxDia; // Cambiar al skybox de d�a
        }
        else
        {
            RenderSettings.skybox = SkyboxNoche; // Cambiar al skybox de noche
        }
    }

    // Funci�n para rotar el skybox suavemente
    void RotarSkybox()
    {
        // Rotar el skybox un poco cada frame
        rotacionSkybox += Time.deltaTime * 1f; // Cambia el valor de 1f para ajustar la velocidad de rotaci�n
        RenderSettings.skybox.SetFloat("_Rotation", rotacionSkybox);
    }
}
