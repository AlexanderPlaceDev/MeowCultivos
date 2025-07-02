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
    public GameObject CTiempo;
    public Scr_ControladorTiempo ContolT;
    public GameObject camb;
    public bool can_C;
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
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();/*
        if (CTiempo == null)
        {
            
        }*/
        cabTiempo();
    }

    public void cambioTiempo()
    {

    }
    public void cabTiempo()
    {
        if (can_C)
        {
            camb.SetActive(true);
        }
        else
        {
            camb.SetActive(false);
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
            hA = Mathf.Clamp(hA - 1, minValueH, maxValueH);
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
            hM = Mathf.Clamp(hM + 1, minValueM, maxValueM);
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
        if (can_C)
        {
            ContolT.Descansar(ObtenerMiuntoActual(), ObtenerHoraActual());
        }
        else
        {
            ContolT.Descansar(MinutoPredeterminada, HoraPredeterminada);
        }

        if(carpa!=null)
        {
            carpa.openUI = false;
        }
        PanelCambio.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
