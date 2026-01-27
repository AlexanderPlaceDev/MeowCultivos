using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class EventoDiario
{
    public string nombreEvento;
    public Sprite IconoRadio;
    public GameObject objeto;         // Ej: FoodTruck, NPC, etc.
    public GameObject mapaAsociado;   // El mapa donde está (puede estar desactivado)
    public string[] diasActivo;       // Ej: {"Martes", "Jueves"}
    public int horaInicio;            // Ej: 10
    public int minutoInicio;          // Ej: 0
    public int horaFin;               // Ej: 18
    public int minutoFin;             // Ej: 0
    public AudioClip sonidoEvento;    // Sonido asociado
    [HideInInspector] public bool sonidoReproducido = false;
}


public class Controlador_EventosGenerales : MonoBehaviour
{
    public EventoDiario[] eventos;
    public Scr_ControladorTiempo controladorTiempo;
    public AudioSource fuenteSonido;

    public List<EventoDiario> ObtenerEventosDelDia(string dia)
    {
        List<EventoDiario> lista = new List<EventoDiario>();

        foreach (var e in eventos)
        {
            foreach (var d in e.diasActivo)
            {
                if (d == dia)
                {
                    lista.Add(e);
                    break;
                }
            }
        }

        return lista;
    }

    public EventoDiario ObtenerEventoPorNombre(string nombre)
    {
        foreach (var e in eventos)
        {
            if (e.nombreEvento == nombre)
                return e;
        }
        return null;
    }


    void Start()
{
    // Inicialmente todos desactivados
    foreach (var e in eventos)
    {
        if (e.objeto != null) e.objeto.SetActive(false);

        // Carga el estado del evento desde PlayerPrefs
        int desactivado = PlayerPrefs.GetInt("EventoDesactivado_" + e.nombreEvento, 0);
        if (desactivado == 1)
        {
            e.diasActivo = new string[0]; // elimina todos los días → ya nunca volverá a aparecer
            e.sonidoReproducido = false;
            if (e.objeto != null) e.objeto.SetActive(false);
        }
    }
}

    private string ultimoDia = "";


    void Update()
    {
        string diaActual = controladorTiempo.DiaActual;
        int hora = controladorTiempo.HoraActual;
        float minuto = controladorTiempo.MinutoActual;

        // --- DETECTAR CAMBIO DE DÍA ---
        if (diaActual != ultimoDia)
        {
            ultimoDia = diaActual;

            // Reiniciar estado diario de TODOS los eventos
            foreach (var e in eventos)
            {
                e.sonidoReproducido = false;
            }

            //Debug.Log("RESET DIARIO DE EVENTOS: " + diaActual);
        }
        // -------------------------------


        foreach (var e in eventos)
        {
            bool esDiaActivo = DiaValido(e.diasActivo, diaActual);
            bool dentroHorario = EnHorario(e, hora, (int)minuto);

            // El mapa puede estar inactivo, así que verificamos antes de activar el objeto
            bool mapaActivo = (e.mapaAsociado == null) || e.mapaAsociado.activeInHierarchy;

            // Reproducir sonido siempre al inicio del evento (aunque el mapa esté inactivo)
            if (esDiaActivo && dentroHorario && !e.sonidoReproducido)
            {
                if (e.sonidoEvento != null)
                {
                    fuenteSonido.clip = e.sonidoEvento;
                    fuenteSonido.Play();
                }
                e.sonidoReproducido = true;
            }

            if (e.objeto != null)
            {
                if (esDiaActivo && dentroHorario && mapaActivo)
                {
                    if (!e.objeto.activeSelf)
                        e.objeto.SetActive(true);
                }
                else
                {
                    if (e.objeto.activeSelf)
                        e.objeto.SetActive(false);

                    // Nada más aquí: el reset diario ya se maneja arriba
                }
            }
        }
    }


    public bool ChecarEvento(string DiaSiguiente)
    {
        for (int i = 0; i < eventos[0].diasActivo.Length; i++)
        {
            if (DiaSiguiente == eventos[0].diasActivo[i])
            {
                return true;
            }
        }
        return false;
    }

    bool DiaValido(string[] dias, string diaActual)
    {
        foreach (string d in dias)
        {
            if (string.Equals(d, diaActual, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    bool EnHorario(EventoDiario e, int hora, int minuto)
    {
        TimeSpan actual = new TimeSpan(hora, minuto, 0);
        TimeSpan inicio = new TimeSpan(e.horaInicio, e.minutoInicio, 0);
        TimeSpan fin = new TimeSpan(e.horaFin, e.minutoFin, 0);

        return actual >= inicio && actual <= fin;
    }

    public void DesactivarEvento(string nombreEvento)
    {
        Debug.Log("Desactivando evento: " + nombreEvento);
        var evento = ObtenerEventoPorNombre(nombreEvento);
        if (evento != null)
        {
            // Guarda el estado del evento en PlayerPrefs
            PlayerPrefs.SetInt("EventoDesactivado_" + nombreEvento, 1);
            PlayerPrefs.Save();

            evento.diasActivo = new string[0]; // elimina todos los días → ya nunca volverá a aparecer
            evento.sonidoReproducido = false; // Si hay objeto asociado, lo apagamos
            if (evento.objeto != null) evento.objeto.SetActive(false);
            Debug.Log("EVENTO DESACTIVADO PERMANENTEMENTE: " + nombreEvento);
        }
    }


}
