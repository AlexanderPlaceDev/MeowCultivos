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
    public Scr_ControladorTiempo ControlT;
    public GameObject carpaUI;
    public CambioTiempo cam;
    Transform Gata;
    public bool openUI = false;
    bool EstaEnRango = false;
    public int HoraDeSiesta=19;
    GameObject radio;
    GameObject Canvas;
    // Start is called before the first frame update
    void Start()
    {
        cam = carpaUI.GetComponent<CambioTiempo>();
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        ControlT = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
        radio = transform.GetChild(3).gameObject;
        Canvas = GameObject.Find("Canvas");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !openUI && EstaEnRango)
        {
            Debug.Log("carpita");
            carpaUI.SetActive(true);
            cam.carpa = this;
            cam.cabTiempo();
            cam.cabRadio();
            StartCoroutine(AparecerUI(1.5f));
        }


        if ((PlayerPrefs.GetString("Habilidad:Despertador", "No") == "Si") && !transform.GetChild(1).gameObject.activeSelf)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            cam.Puede_Ajustar = true;
        }
        activarRadio();
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
        openUI = true;
    }
    IEnumerator EsconderUI(float dur)
    {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            if (ControlT.HoraActual > HoraDeSiesta)
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
