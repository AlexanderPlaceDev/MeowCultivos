using System.Collections;
using UnityEngine;

public class Scr_InterruptorLuz : MonoBehaviour
{
    [SerializeField] bool EsTimer;
    [SerializeField] int[] HorasEncendidoYApagado;

    [SerializeField] bool Titila;
    [SerializeField] float[] TiempoEntreTitilado;

    [SerializeField] Scr_ControladorTiempo Tiempo;

    float TiempoParaTitilar;
    float Contador;
    bool Titilando = false;

    [SerializeField] float IntensidadMinima = 0.1f; // Intensidad mínima a la que bajará la luz
    [SerializeField] float DuracionTitilado = 0.5f; // Duración de la transición completa (bajada y subida)

    private Light luz;
    private float IntensidadOriginal;

    void Start()
    {
        luz = GetComponent<Light>();
        IntensidadOriginal = luz.intensity; // Guardamos la intensidad original de la luz
        TiempoParaTitilar = Random.Range(TiempoEntreTitilado[0], TiempoEntreTitilado[1]);
    }

    void Update()
    {
        if (EsTimer)
        {
            // Si la hora actual es entre las 20:00 (8:00 PM) y las 8:00 AM del día siguiente
            if (Tiempo.HoraActual >= HorasEncendidoYApagado[1] || Tiempo.HoraActual < HorasEncendidoYApagado[0])
            {
                // Encender la luz entre las 20:00 PM (HorasEncendidoYApagado[1]) y las 8:00 AM (HorasEncendidoYApagado[0])
                if (!luz.enabled)
                {
                    luz.enabled = true;
                }
            }
            else
            {
                // Apagar la luz entre las 8:00 AM y las 20:00 PM
                if (luz.enabled)
                {
                    StopAllCoroutines();
                    luz.enabled = false;
                }
            }
        }

        // Lógica para titilar la luz si está encendida
        if (Titila && luz.enabled)
        {
            if (!Titilando && Contador < TiempoParaTitilar)
            {
                Contador += Time.deltaTime;
            }
            else
            {
                StartCoroutine(Titilar());
            }
        }
    }







    IEnumerator Titilar()
    {
        Titilando = true;

        // Suaviza la transición de la intensidad
        yield return StartCoroutine(SuavizarIntensidad(IntensidadMinima, DuracionTitilado / 2));

        // Suaviza la vuelta a la intensidad original
        yield return StartCoroutine(SuavizarIntensidad(IntensidadOriginal, DuracionTitilado / 2));

        Titilando = false;
        Contador = 0;
        TiempoParaTitilar = Random.Range(TiempoEntreTitilado[0], TiempoEntreTitilado[1]);
    }

    // Corutina para suavizar la transición de la intensidad de la luz
    IEnumerator SuavizarIntensidad(float intensidadObjetivo, float duracion)
    {
        float intensidadInicial = luz.intensity;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            luz.intensity = Mathf.Lerp(intensidadInicial, intensidadObjetivo, tiempoTranscurrido / duracion);
            yield return null; // Espera hasta el próximo frame
        }

        // Asegurarse de que la intensidad final se ajuste perfectamente
        luz.intensity = intensidadObjetivo;
    }
}
