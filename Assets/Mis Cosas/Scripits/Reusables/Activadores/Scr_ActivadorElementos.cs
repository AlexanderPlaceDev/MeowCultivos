using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Scr_ActivadorElementos : MonoBehaviour
{
    [SerializeField] private bool EsCinematica;
    [SerializeField] public string NombreCinematica;
    [SerializeField] private string CinematicaPrevia;
    [SerializeField] public string CinematicaSiguiente;
    [SerializeField] public bool UsaEventoGeneral;
    [SerializeField] public string NombreEventoGeneral;
    [SerializeField] private string HabilidadNecesaria;

    // AHORA SOLO HORAS
    [SerializeField] private bool UsaHorarioFijo = true; // si false → activo todo el día

    [SerializeField] private int HoraMinima;
    [SerializeField] private int HoraMaxima;

    [Header("Condiciones extra")]
    [SerializeField] private string[] DiasValidos;
    [Range(0f, 1f)]
    [SerializeField] private float ProbabilidadDia = 1f;

    [SerializeField] private GameObject[] ElementosAActivar;

    [Header("Control de reactivación")]
    public int HorasParaReactivar = 24;

    private Scr_ControladorTiempo Tiempo;
    private Controlador_EventosGenerales Eventos;

    private string diaUltimaRevision = "";
    private bool diaPermitido = true;
    private bool prevActivado = false;

    void Start()
    {
        Tiempo = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
        Eventos = GameObject.Find("EventosGenerales").GetComponent<Controlador_EventosGenerales>();
    }

    void Update()
    {
        // HABILIDAD
        string keyHabilidad = "Habilidad:" + HabilidadNecesaria;

        bool tieneHabilidad = false;
        if (string.IsNullOrEmpty(HabilidadNecesaria))
        {
            tieneHabilidad = true;
        }
        else
        {
            tieneHabilidad = PlayerPrefs.GetString(keyHabilidad, "No") == "Si";
        }

        int horaActual = Tiempo.HoraActual;

        float rangoMin, rangoMax;

        // -------------------------------
        // ✔ SI USA EVENTO GENERAL
        // -------------------------------
        if (UsaEventoGeneral)
        {
            var evento = Eventos.ObtenerEventoPorNombre(NombreEventoGeneral);
            if (evento == null)
            {
                prevActivado = false;
                return;
            }

            // Día válido
            bool esDia = Array.Exists(evento.diasActivo, d => d == Tiempo.DiaActual);

            // Mapa activo
            bool mapaActivo = (evento.mapaAsociado == null) || evento.mapaAsociado.activeInHierarchy;

            if (!(esDia && mapaActivo))
            {
                prevActivado = false;
                return;
            }

            // SOLO HORAS
            rangoMin = evento.horaInicio;
            rangoMax = evento.horaFin;
        }
        else
        {
            // Si NO usa evento general, decidimos si usa horario o todo el día
            if (UsaHorarioFijo)
            {
                rangoMin = HoraMinima;
                rangoMax = HoraMaxima;
            }
            else
            {
                // Activo TODO EL DÍA
                rangoMin = 0;
                rangoMax = 24;
            }
        }


        // Rango horario (soporta cruce de medianoche)

        bool dentroDeRango = false;
        if (rangoMin <= rangoMax)
        {
            // Rango normal (ej. 8:00 a 17:00)
            if (horaActual >= rangoMin && horaActual <= rangoMax)
            {
                dentroDeRango = true;
            }
        }
        else
        {
            // Rango invertido (ej. 22:00 a 3:00)
            if (horaActual >= rangoMin || horaActual <= rangoMax)
            {
                dentroDeRango = true;
            }
        }

        // CINE PREVIA
        bool vioCinematicaPrevia = false;

        if (string.IsNullOrEmpty(CinematicaPrevia))
        {
            Debug.Log(5 + " " + gameObject.name);
            vioCinematicaPrevia = true;
        }
        else
        {
            Debug.Log(PlayerPrefs.GetString("Cinematica " + CinematicaPrevia, "No") + " cinematica " + CinematicaPrevia);
            if (PlayerPrefs.GetString("Cinematica " + CinematicaPrevia, "No") == "Si")
            {
                Debug.Log(7);
                vioCinematicaPrevia = true;
            }
        }

        // NO haber visto esta
        bool noHaVistoEsta = true;

        if (UsaEventoGeneral)
        {
            if (PlayerPrefs.GetString("Cinematica " + NombreCinematica, "No") == "No")
            {
                noHaVistoEsta = true;
            }
            else
            {
                noHaVistoEsta = false;
                if (CinematicaSiguiente == null)
                {
                    Eventos.DesactivarEvento(NombreEventoGeneral);
                }

            }
        }
        if (!EsCinematica)
        {
            noHaVistoEsta = true;
        }

        // --- CONTROL DE TIEMPO SOLO POR DÍA ---
        bool bloqueadoPorTiempo = false;

        if (!string.IsNullOrEmpty(CinematicaPrevia))
        {
            string ultimoDia = PlayerPrefs.GetString("DiaCinematica:" + CinematicaPrevia, "");
            int ultimaHora = PlayerPrefs.GetInt("HoraCinematica:" + CinematicaPrevia, -1);

            if (!string.IsNullOrEmpty(ultimoDia) && ultimaHora >= 0)
            {
                int horaPrevTotal = GetDayIndex(ultimoDia) * 24 + ultimaHora;
                int horaActualTotal = GetDayIndex(Tiempo.DiaActual) * 24 + Tiempo.HoraActual;

                int horasTranscurridas = horaActualTotal - horaPrevTotal;

                // Ajuste si cruzó semana
                if (horasTranscurridas < 0)
                    horasTranscurridas += 7 * 24;

                if (horasTranscurridas < HorasParaReactivar)
                    bloqueadoPorTiempo = true;
            }
        }


        // DÍA
        bool diaCorrecto = DiaEsValido(Tiempo.DiaActual);

        // Probabilidad diaria
        if (!UsaEventoGeneral && ProbabilidadDia < 1f)
        {
            if (diaUltimaRevision != Tiempo.DiaActual)
            {
                diaUltimaRevision = Tiempo.DiaActual;
                diaPermitido = UnityEngine.Random.value <= ProbabilidadDia;
            }

            diaCorrecto &= diaPermitido;
        }

        Debug.Log("Habilidad:" + tieneHabilidad + " Enrango:" + dentroDeRango + " Dia:" + diaCorrecto + " previa:" + vioCinematicaPrevia + " nohavistoesta:" + noHaVistoEsta + " Bloqueada:" + bloqueadoPorTiempo + " " + gameObject.name);

        bool activar =
            tieneHabilidad &&
            dentroDeRango &&
            diaCorrecto &&
            vioCinematicaPrevia &&
            noHaVistoEsta &&
            !bloqueadoPorTiempo;

        // Activación de objetos
        foreach (var obj in ElementosAActivar)
            if (obj != null)
                obj.SetActive(activar);

        // Guardar cinemática vista
        if (EsCinematica)
        {
            if (activar && !prevActivado)
            {
                // Guardar solo día + hora
                PlayerPrefs.SetString("DiaCinematica:" + NombreCinematica, Tiempo.DiaActual);
                PlayerPrefs.SetInt("HoraCinematica:" + NombreCinematica, Tiempo.HoraActual);

                PlayerPrefs.Save();
            }
            prevActivado = activar;
        }
    }

    // Devuelve el índice del día (LUN=0, MAR=1,... DOM=6)
    private int GetDayIndex(string dia)
    {
        string[] dias = { "LUN", "MAR", "MIE", "JUE", "VIE", "SAB", "DOM" };
        int idx = Array.IndexOf(dias, dia);
        return Mathf.Clamp(idx, 0, 6);
    }

    private bool DiaEsValido(string diaActual)
    {
        string[] dias;

        if (UsaEventoGeneral)
        {
            var evento = Eventos.ObtenerEventoPorNombre(NombreEventoGeneral);
            if (evento == null)
                return false;

            dias = evento.diasActivo;
        }
        else
        {
            dias = DiasValidos;
        }

        if (dias == null || dias.Length == 0) return true;

        return Array.Exists(dias, d => d == diaActual);
    }
}
