using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CambioTiempo : MonoBehaviour
{
    // Referencia a la carpa para cerrar UI si es necesario
    public Carpas carpa;

    // Panel donde se ajusta el tiempo
    public GameObject PanelCambio;

    // Controlador principal del tiempo
    public Scr_ControladorTiempo ControlT;

    // Panel donde se ajusta la hora manual
    public GameObject camb;

    // Panel del radio
    public GameObject Radio;

    // Controlador de eventos diarios
    public Controlador_EventosGenerales EventosGenerales;

    // Texto donde se muestra el día siguiente
    public TextMeshProUGUI Dia;

    // Icono del evento del día siguiente
    public GameObject IconoEvento;

    // Icono del clima del día siguiente
    public GameObject IconoClima;

    // Arreglo de sprites para iconos de evento
    public Sprite[] IconosEvento;

    // Arreglo de sprites para iconos de clima
    public Sprite[] IconosClimas;

    // Permite o no ajustar la hora manualmente (habilidad "Despertador")
    public bool Puede_Ajustar;

    // Indica si el jugador tiene radio construido
    public bool Tiene_Radio;

    // Inputs para la hora y el minuto
    public TMP_InputField Hora;
    public TMP_InputField Minuto;

    // Hora predeterminada para dormir si no puede ajustar la hora
    public int HoraPredeterminada = 8;
    public int MinutoPredeterminada = 0;

    // Límites de hora
    private int minValueH = 0;
    private int maxValueH = 24;

    // Límites de minuto
    private int minValueM = 0;
    private int maxValueM = 60;

    // Coroutine para alternar entre iconos de eventos
    private Coroutine cicloEventosCoroutine;

    // Lista de iconos de eventos del día siguiente
    private List<Sprite> iconosEventosDia = new List<Sprite>();


    void Start()
    {
        // Inicializar campos de hora y minuto
        Hora.text = HoraPredeterminada.ToString();
        Minuto.text = MinutoPredeterminada.ToString();

        // Obtener referencias necesarias en escena
        ControlT = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
        EventosGenerales = GameObject.Find("EventosGenerales").GetComponent<Controlador_EventosGenerales>();

        // Configurar radio y reloj
        cabRadio();
        activarDespetador();
    }

    // Activa o desactiva el radio según estado en PlayerPrefs
    private void activarRadio()
    {
        if (PlayerPrefs.GetInt("Estructura" + 8, 0) == 1)
            Tiene_Radio = true;
        else
            Tiene_Radio = false;
    }

    // Activa o desactiva el ajuste de hora según habilidad del jugador
    private void activarDespetador()
    {
        if (PlayerPrefs.GetString("Habilidad:" + "Despertador", "No") == "Si")
            Puede_Ajustar = true;
        else
            Puede_Ajustar = false;

        cabTiempo();
    }

    // Configura la UI del radio
    public void cabRadio()
    {
        activarRadio();
        ControlT = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();

        if (Tiene_Radio)
        {
            Radio.SetActive(true);
            Radio_not(); // Cargar datos del día siguiente
        }
    }

    // Muestra u oculta la UI de ajuste manual de tiempo
    public void cabTiempo()
    {
        if (Puede_Ajustar)
        {
            // Habilidad desbloqueada → panel visible pero bloqueado
            camb.SetActive(true);
            Hora.interactable = false;
            Minuto.interactable = false;
        }
        else
        {
            // No puede ajustar la hora → panel oculto
            camb.SetActive(false);
            Hora.interactable = true;
            Minuto.interactable = true;
        }
    }

    // Muestra información del radio para el día siguiente
    public void Radio_not()
    {
        // Obtener día siguiente
        string diaSiguiente = ControlT.checarSiguienteDia();
        MostrarDia(diaSiguiente);

        // Asegurar referencia a controlador de eventos
        if (EventosGenerales == null)
        {
            EventosGenerales = GameObject.Find("EventosGenerales").GetComponent<Controlador_EventosGenerales>();
        }

        // Obtener todos los eventos del día siguiente

        List<EventoDiario> eventosManiana = EventosGenerales.ObtenerEventosDelDia(diaSiguiente);
        Debug.Log("Eventos de mañana: " + eventosManiana.Count);
        // Si hay eventos, iniciar ciclo de iconos
        if (eventosManiana.Count > 0)
        {
            iconosEventosDia.Clear();

            // Agregar iconos que sí tengan sprite asignado
            foreach (var ev in eventosManiana)
            {
                if (ev.IconoRadio != null)
                    iconosEventosDia.Add(ev.IconoRadio);
            }

            // Si ningún evento tiene icono, usar icono genérico
            if (iconosEventosDia.Count == 0)
                iconosEventosDia.Add(IconosEvento[1]);

            // Iniciar la rotación de iconos
            if (cicloEventosCoroutine != null)
                StopCoroutine(cicloEventosCoroutine);

            cicloEventosCoroutine = StartCoroutine(CicloIconosEventos());
        }
        else
        {
            // No hay eventos → icono de "sin evento"
            IconoEvento.GetComponent<Image>().sprite = IconosEvento[0];

            if (cicloEventosCoroutine != null)
                StopCoroutine(cicloEventosCoroutine);
        }

        // Obtener clima del día siguiente
        string[] dias = { "LUN", "MAR", "MIE", "JUE", "VIE", "SAB", "DOM" };
        int indiceSiguienteDia = System.Array.IndexOf(dias, diaSiguiente);
        var climaManiana = ControlT.ClimaSemanal[indiceSiguienteDia];

        MostrarClima(climaManiana.ToString());
    }

    // Coroutine que alterna iconos cada 1 segundo
    private IEnumerator CicloIconosEventos()
    {
        int index = 0;

        while (true)
        {
            if (iconosEventosDia.Count == 0)
                yield break;

            IconoEvento.GetComponent<Image>().sprite = iconosEventosDia[index];

            index++;
            if (index >= iconosEventosDia.Count)
                index = 0;

            yield return new WaitForSeconds(1f);
        }
    }

    // Muestra el nombre completo del día
    private void MostrarDia(string dia)
    {
        switch (dia)
        {
            case "LUN": Dia.text = "LUNES"; break;
            case "MAR": Dia.text = "MARTES"; break;
            case "MIE": Dia.text = "MIERCOLES"; break;
            case "JUE": Dia.text = "JUEVES"; break;
            case "VIE": Dia.text = "VIERNES"; break;
            case "SAB": Dia.text = "SABADO"; break;
            case "DOM": Dia.text = "DOMINGO"; break;
        }
    }

    // Muestra el ícono correspondiente al clima
    private void MostrarClima(string clima)
    {
        switch (clima)
        {
            case "Soleado": IconoClima.GetComponent<Image>().sprite = IconosClimas[0]; break;
            case "Nublado": IconoClima.GetComponent<Image>().sprite = IconosClimas[1]; break;
            case "Lluvioso": IconoClima.GetComponent<Image>().sprite = IconosClimas[2]; break;
            case "Vientoso": IconoClima.GetComponent<Image>().sprite = IconosClimas[3]; break;
            case "LunaRoja": IconoClima.GetComponent<Image>().sprite = IconosClimas[4]; break;
            default: IconoClima.GetComponent<Image>().sprite = IconosClimas[0]; break;
        }
    }

    // Aumentar hora manualmente
    public void aumentH()
    {
        int hA = ObtenerHoraActual();

        if (hA == maxValueH)
            hA = minValueH;
        else
            hA = Mathf.Clamp(hA + 1, minValueH, maxValueH);

        Hora.text = hA.ToString();
    }

    // Disminuir hora manualmente
    public void disminuirH()
    {
        int hA = ObtenerHoraActual();

        if (hA == minValueH)
            hA = maxValueH;
        else
            hA = Mathf.Clamp(hA - 10, minValueH, maxValueH);

        Hora.text = hA.ToString();
    }

    // Aumentar minuto manualmente
    public void aumentM()
    {
        int hM = ObtenerMiuntoActual();

        if (hM == maxValueM)
        {
            hM = minValueM;
            aumentH();
        }
        else
        {
            hM = Mathf.Clamp(hM + 10, minValueM, maxValueM);
        }

        Minuto.text = hM.ToString();
    }

    // Disminuir minuto manualmente
    public void disminuirM()
    {
        int hM = ObtenerMiuntoActual();

        if (hM == minValueM)
        {
            hM = maxValueM;
            disminuirH();
        }
        else
        {
            hM = Mathf.Clamp(hM - 1, minValueM, maxValueM);
        }

        Minuto.text = hM.ToString();
    }

    // Obtener hora del input
    private int ObtenerHoraActual()
    {
        if (int.TryParse(Hora.text, out int valor))
            return valor;
        else
            return minValueH;
    }

    // Obtener minuto del input
    private int ObtenerMiuntoActual()
    {
        if (int.TryParse(Minuto.text, out int valor))
            return valor;
        else
            return minValueM;
    }

    // Acción de dormir
    public void Duerme()
    {
        if (Puede_Ajustar)
            ControlT.Descansar(ObtenerMiuntoActual(), ObtenerHoraActual());
        else
            ControlT.Descansar(MinutoPredeterminada, HoraPredeterminada);

        cerrarUI();
    }

    // Cierra el panel y detiene animación del radio
    public void cerrarUI()
    {
        if (cicloEventosCoroutine != null)
            StopCoroutine(cicloEventosCoroutine);

        if (carpa != null)
            carpa.CerrarCarpa();
    }

    void Update()
    {
        // No contiene lógica actualmente
    }
}
