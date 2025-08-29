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
    public GameObject Radio;
    public Controlador_FoodTruck FoodTruck;
    public TextMeshProUGUI Dia;
    public GameObject Icono_Foodtruck;
    public bool Puede_Ajustar;//si puede ajustar la hora
    public bool Tiene_Radio;//si puede ajustar la hora
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
    {/*
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
        if (CTiempo == null)
        {
            
        }*/

        Hora.text = HoraPredeterminada.ToString();
        Minuto.text = MinutoPredeterminada.ToString();
        cabTiempo();
        if (Tiene_Radio)
        {
            Radio.SetActive(true);
            Radio_not();

        }
    }

    private void OnEnable()
    {
        Radio_not();
    }

    public void Radio_not()
    {
        string diamanana = ContolT.checarDia();
        Dia.text = diamanana;
        for (int i = 0; i < FoodTruck.DiasActivo.Length; i++) 
        {
            if (diamanana == FoodTruck.DiasActivo[i])
            {
                //Debug.LogError("Eytt");
                Icono_Foodtruck.SetActive(true);
                break;
            }
        }
    }
    public void cabTiempo()
    {
        if (Puede_Ajustar)
        {
            camb.SetActive(true);
            Hora.interactable = false;
            Minuto.interactable=false;
        }
        else
        {
            camb.SetActive(false);
            Hora.interactable = true;
            Minuto.interactable = true;
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
        if (Puede_Ajustar)
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
        cerrarUI();
    }

    public void cerrarUI()
    {
        Icono_Foodtruck.SetActive(false);
        PanelCambio.SetActive(false);
    }
    public void checarTiempo()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
