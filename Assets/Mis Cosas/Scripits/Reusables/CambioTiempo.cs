using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CambioTiempo : MonoBehaviour
{

    public Carpas carpa;

    public GameObject PanelCambio;
    public Scr_ControladorTiempo ControlT;
    public GameObject camb;
    public GameObject Radio;
    public Controlador_EventosGenerales EventosGenerales;
    public TextMeshProUGUI Dia;
    public GameObject IconoEvento;
    public GameObject IconoClima;
    public Sprite[] IconosEvento;
    public Sprite[] IconosClimas;
    public bool Puede_Ajustar;//si puede ajustar la hora
    public bool Tiene_Radio;//si puede ver el radio
    public TMP_InputField Hora;
    public TMP_InputField Minuto;

    public int HoraPredeterminada = 8;
    public int MinutoPredeterminada = 0;

    private int minValueH = 0;
    private int maxValueH = 24;

    private int minValueM = 0;
    private int maxValueM = 60;
    // Start is called before the first frame update
    void Start()
    {
        Hora.text = HoraPredeterminada.ToString();
        Minuto.text = MinutoPredeterminada.ToString();
        ControlT = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
        EventosGenerales = GameObject.Find("EventosGenerales").GetComponent<Controlador_EventosGenerales>();
        cabRadio();
        activarDespetador();
    }

    private void activarRadio()
    {
        if (PlayerPrefs.GetString("Habilidad:" + "Radio", "No") == "Si")
        {
            Tiene_Radio = true;
        }
        else
        {
            Tiene_Radio = false;
        }

    }
    private void activarDespetador()
    {
        if (PlayerPrefs.GetString("Habilidad:" + "Despertador", "No") == "Si")
        {
            Puede_Ajustar = true;
        }
        else
        {
            Puede_Ajustar = false;
        }
        cabTiempo();
    }
    public void cabRadio()
    {
        activarRadio();
        ControlT = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
        if (Tiene_Radio)
        {
            Radio.SetActive(true);
            Radio_not();
        }
    }
    public void cabTiempo()
    {
        if (Puede_Ajustar)
        {
            camb.SetActive(true);
            Hora.interactable = false;
            Minuto.interactable = false;
        }
        else
        {
            camb.SetActive(false);
            Hora.interactable = true;
            Minuto.interactable = true;
        }
    }
    //checa el radio
    public void Radio_not()
    {
        string diasemanana = ControlT.checarSiguienteDia();
        bool HayEvento=false;
        MostrarDia(diasemanana);
        //Debug.LogWarning(EventosGenerales.eventos[0].diasActivo[0]);
        for (int i = 0; i < EventosGenerales.eventos[0].diasActivo.Length; i++) 
        {
            if (diasemanana == EventosGenerales.eventos[0].diasActivo[i])
            {
                IconoEvento.GetComponent<Image>().sprite = IconosEvento[1];
                HayEvento = true;
                break;
            }
        }
        if (!HayEvento)
        {
            IconoEvento.GetComponent<Image>().sprite = IconosEvento[0];
        }
        MostrarClima(ControlT.DiaActual.ToString());
    }

    private void MostrarDia(string dia)
    {
        //"LUN", "MAR", "MIE", "JUE", "VIE", "SAB", "DOM" 
        switch (dia)
        {
            case "LUN":
                Dia.text = "LUNES";
                break;
            case "MAR":
                Dia.text = "MARTES";
                break;
            case "MIE":
                Dia.text = "MIERCOLES";
                break;
            case "JUE":
                Dia.text = "JUEVES";
                break;
            case "VIE":
                Dia.text = "VIERNES";
                break;
            case "SAB":
                Dia.text = "SABADO";
                break;
            case "DOM":
                Dia.text = "DOMINGO";
                break;
        }
    }
    private void MostrarClima(string dia)
    {
        switch (dia)
        {
            case "Soleado":
                IconoClima.GetComponent<Image>().sprite = IconosClimas[0];
                break;
            case "Nublado":
                IconoClima.GetComponent<Image>().sprite = IconosClimas[1];
                break;
            case "Lluvioso":
                IconoClima.GetComponent<Image>().sprite = IconosClimas[2];
                break;
            case "Vientoso":
                IconoClima.GetComponent<Image>().sprite = IconosClimas[3];
                break;
            default:
                IconoClima.GetComponent<Image>().sprite = IconosClimas[0];
                break;
        }
    }
    

    public void aumentH()
    {
        int hA = ObtenerHoraActual();
        if (hA == maxValueH)
        {
            hA = minValueH;
        }
        else
        {
            hA = Mathf.Clamp(hA + 1, minValueH, maxValueH);
        }
        Hora.text = hA.ToString();
    }
    public void disminuirH()
    {
        int hA = ObtenerHoraActual();
        if (hA == minValueH)
        {
            hA = maxValueH;
        }
        else 
        {
            hA = Mathf.Clamp(hA - 10, minValueH, maxValueH);
        }
        Hora.text = hA.ToString();
    }

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
        /*
        int ha = ObtenerHoraActual();
        if (ha==24)
        {
            aumentH();
        }*/
        Minuto.text = hM.ToString();
    }
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
        /*
        int ha = ObtenerHoraActual();
        if (ha == 0)
        {
            disminuirH();
        }*/
        Minuto.text = hM.ToString();
    }
    private int ObtenerHoraActual()
    {
        if (int.TryParse(Hora.text, out int valor))
            return valor;
        else
            return minValueH; 
    }
    private int ObtenerMiuntoActual()
    {
        if (int.TryParse(Minuto.text, out int valor))
            return valor;
        else
            return minValueM; 
    }

    public void Duerme()
    {
        if (Puede_Ajustar)
        {
            ControlT.Descansar(ObtenerMiuntoActual(), ObtenerHoraActual());
        }
        else
        {
            ControlT.Descansar(MinutoPredeterminada, HoraPredeterminada);
        }

        cerrarUI();
    }

    public void cerrarUI()
    {
        //IconoEvento.SetActive(false);
        //PanelCambio.SetActive(false);
        if (carpa != null)
        {
            carpa.CerrarCarpa();
        }

    }
    public void checarTiempo()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
