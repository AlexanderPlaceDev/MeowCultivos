using UnityEngine;
using System;

[System.Serializable]
public class EventoDiario
{
    public string nombreEvento;
    public GameObject objeto;         // Ej: FoodTruck, NPC, etc.
    public GameObject mapaAsociado;   // El mapa donde está (puede estar desactivado)
    public string[] diasActivo;       // Ej: {"Martes", "Jueves"}
    public int horaInicio;            // Ej: 10
    public int minutoInicio;          // Ej: 0
    public int horaFin;               // Ej: 18
    public int minutoFin;             // Ej: 0
    public AudioClip sonidoEvento;    // Sonido asociado
    [HideInInspector] public bool activoHoy = false;
    [HideInInspector] public bool sonidoReproducido = false;
}

public class Controlador_EventosGenerales : MonoBehaviour
{
    public EventoDiario[] eventos;
    public Scr_ControladorTiempo controladorTiempo;
    public AudioSource fuenteSonido;

    void Start()
    {
        // Inicialmente todos desactivados
        foreach (var e in eventos)
        {
            if (e.objeto != null)
                e.objeto.SetActive(false);
        }
    }

    void Update()
    {
        string diaActual = controladorTiempo.DiaActual;
        int hora = controladorTiempo.HoraActual;
        float minuto = controladorTiempo.MinutoActual;

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

            // Activar o desactivar el objeto según condiciones
            if (esDiaActivo && dentroHorario && mapaActivo)
            {
                if (!e.objeto.activeSelf)
                    e.objeto.SetActive(true);
                e.activoHoy = true;
            }
            else
            {
                if (e.objeto.activeSelf)
                    e.objeto.SetActive(false);

                // Reinicia el estado si cambia de día
                if (!esDiaActivo)
                {
                    e.activoHoy = false;
                    e.sonidoReproducido = false;
                }
            }
        }
    }

    public bool ChecarEvento(string diasemanana)
    {
        for (int i = 0; i < eventos[0].diasActivo.Length; i++)
        {
            if (diasemanana == eventos[0].diasActivo[i])
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
}
