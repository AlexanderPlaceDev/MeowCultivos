using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Scr_Movimiento;
public class Carpas : MonoBehaviour
{

    [SerializeField] Sprite IconoTecla;
    [SerializeField] string Letra;


    public bool Tiene_Radio;//si puede ver el radio
    public bool Tiene_Reloj;//si puede ver el radio
    public Scr_ControladorTiempo ControlT;
    public GameObject carpaUI;
    public CambioTiempo cam;
    Transform Gata;
    public bool openUI = false;
    public bool EstaEnRango = false;
    public int HoraDeSiestaInicio = 19;
    public int HoraDeSiestaFin = 5;
    GameObject radio; 
    GameObject Reloj;
    GameObject Canvas;

    public bool dentroHorario;
    // Start is called before the first frame update
    void Start()
    {
        cam = carpaUI.GetComponent<CambioTiempo>();
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        ControlT = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
        radio = transform.GetChild(2).gameObject;
        Reloj = transform.GetChild(1).gameObject;
        Canvas = GameObject.Find("Canvas");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !openUI && EstaEnRango)
        {
            StartCoroutine(AparecerUI(1f));
        }
        if (Input.GetKeyDown(KeyCode.E) && openUI && EstaEnRango)
        {
            StartCoroutine(EsconderUI(1f));
        }

        if ((PlayerPrefs.GetString("Habilidad:Despertador", "No") == "Si") && !transform.GetChild(1).gameObject.activeSelf)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            cam.Puede_Ajustar = true;
        }
        activarRadio();
        activarDespertador();

        // Calculamos si estamos dentro del horario
        if (HoraDeSiestaInicio < HoraDeSiestaFin)
        {
            dentroHorario = ControlT.HoraActual >= HoraDeSiestaInicio && ControlT.HoraActual < HoraDeSiestaFin;
        }
        else
        {
            dentroHorario = ControlT.HoraActual >= HoraDeSiestaInicio || ControlT.HoraActual < HoraDeSiestaFin;
        }

    }

    public void CerrarCarpa()
    {
        StartCoroutine(EsconderUI(.5f));
    }
    IEnumerator AparecerUI(float dur)
    {
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>(), -200, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(1).GetComponent<RectTransform>(), 230, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(2).GetComponent<RectTransform>(), -810, 1);
        yield return new WaitForSeconds(dur);
        Debug.Log("carpita");
        carpaUI.SetActive(true);
        cam.carpa = this;
        cam.cabTiempo();
        cam.cabRadio();
        openUI = true;
    }
    IEnumerator EsconderUI(float dur)
    {
        carpaUI.SetActive(false);
        openUI = false;
        yield return new WaitForSeconds(dur);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>(), 0, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(1).GetComponent<RectTransform>(), 0, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(2).GetComponent<RectTransform>(), -610, 1);
    }
    private void activarRadio()
    {
        if(PlayerPrefs.GetString("Habilidad:" + "Radio", "No") == "Si")
        {
            Tiene_Radio = true;
        }
        else
        {
            Tiene_Radio = false;
        }
        
        if (Tiene_Radio)
        {
            radio.SetActive(true);
        }
        else
        {
            radio.SetActive(false);
        }
    }
    private void activarDespertador()
    {
        if (PlayerPrefs.GetString("Habilidad:" + "Despertador", "No") == "Si")
        {
            Tiene_Reloj = true;
        }
        else
        {
            Tiene_Reloj = false;
        }

        if (Tiene_Reloj)
        {
            Reloj.SetActive(true);
            TextMeshProUGUI RelojText = GetComponentInChildren<TextMeshProUGUI>();
            RelojText.text = ControlT.HoraActual + ":" + ControlT.MinutoActual;
        }
        else
        {
            Reloj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            if (dentroHorario)
            {
                //Debug.LogError(ContolT.HoraActual > HoraDeSiesta);
                EstaEnRango = true;
                Gata.GetChild(3).gameObject.SetActive(true);
                Gata.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Letra;
                Gata.GetChild(3).GetChild(0).GetComponent<Image>().sprite = IconoTecla;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = false;
            Gata.GetChild(3).gameObject.SetActive(false);
        }
    }
}
