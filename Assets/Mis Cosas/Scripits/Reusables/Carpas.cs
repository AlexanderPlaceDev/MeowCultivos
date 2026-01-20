using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Scr_Movimiento;
public class Carpas : MonoBehaviour
{

    [SerializeField] Sprite IconoTecla;
    [SerializeField] string Letra;
    [SerializeField] Sprite Icono;

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
    public GameObject radio;
    public GameObject Reloj;
    GameObject Canvas;

    public bool dentroHorario;

    PlayerInput playerInput;
    private InputAction Interactuar;
    InputIconProvider IconProvider;
    private Sprite iconoActualInteractuar = null;
    private string textoActualInteractuar = "";
    ChecarInput Checar_input;
    // Start is called before the first frame update
    void Start()
    {
        cam = carpaUI.GetComponent<CambioTiempo>();
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        ControlT = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
        //radio = transform.GetChild(3).gameObject;
        //Reloj = transform.GetChild(1).gameObject;
        Canvas = GameObject.Find("Canvas");
        Checar_input = GameObject.Find("Singleton").GetComponent<ChecarInput>();
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Interactuar = playerInput.actions["Interactuar"];
    }

    // Update is called once per frame
    void Update()
    {
        if (EstaEnRango)
        {
            Actualizar_icono();
        }
        if (Interactuar.IsPressed() && !openUI && EstaEnRango)
        {
            StartCoroutine(AparecerUI(1f));
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
            Checar_input.CammbiarAction_UI();
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
        Checar_input.CammbiarAction_Player();
        StartCoroutine(EsconderUI(.5f));
    }
    IEnumerator AparecerUI(float dur)
    {
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
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
        //PlayerPrefs.GetString("Habilidad:" + "Radio", "No") == "Si"
        //PlayerPrefs.GetInt("Estructura" + indiceReal, 0) == 1;
        if (PlayerPrefs.GetInt("Estructura" + 8, 0) == 1)
        {
            Tiene_Radio = true; 
            radio.SetActive(true);
        }
        else
        {
            Tiene_Radio = false;
            radio.SetActive(false);
        }

    }
    private void activarDespertador()
    {
        //PlayerPrefs.GetString("Habilidad:" + "Despertador", "No") == "Si"
        //PlayerPrefs.GetInt("Estructura" + 9, 0) == 1
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
            if (RelojText != null)
            {
                RelojText.text = ControlT.HoraActual + ":" + ControlT.MinutoActual;
            }
        }
        else
        {
            Reloj.SetActive(false);
        }
    }

    public bool ChecarLunaRoja()
    {
        if(ControlT.ClimaSemanal.Count > 0 && ControlT.ClimaSemanal[ControlT.ConseguirDia()].ToString() == "LunaRoja" && ControlT.EstaActivoClima)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Actualizar_icono()
    {
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = true;
        Gata.GetChild(3).gameObject.SetActive(true);

        Gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = Icono;


        Gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
        Gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);

        IconProvider.ActualizarIconoUI(Interactuar, Gata.GetChild(3).GetChild(0), ref iconoActualInteractuar, ref textoActualInteractuar,true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            if (dentroHorario && !ChecarLunaRoja())
            {
                //Debug.LogError(ContolT.HoraActual > HoraDeSiesta);
                EstaEnRango = true;
                Gata.GetChild(3).gameObject.SetActive(true);
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
