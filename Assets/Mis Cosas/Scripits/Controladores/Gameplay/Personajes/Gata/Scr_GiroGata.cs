using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Scr_GiroGata : MonoBehaviour
{
    public Transform Gata;
    public Transform CabezaGata;
    public bool CamFija;
    public float velocidad;
    Rigidbody rb;
    PlayerInput playerInput;
    public float Sensibilidad = .3f;
    private InputAction MoverHorizontal;
    private InputAction CambiarCamara;
    InputIconProvider IconProvider;
    private Sprite iconoActualCamara = null;
    private string textoActualCamara = "";
    public GameObject CamaraBoton;
    public GameObject Camara;
    public GameObject giro;

    private void OnEnable()
    {
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        MoverHorizontal = playerInput.actions["MoverHorizontal"];
        CambiarCamara = playerInput.actions["CamaraLibre"];
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        rb = GetComponent<Rigidbody>();
       

        if (PlayerPrefs.GetString("CamaraFija", "SI")== "SI")
        {
            CamFija = true;
        }
        else
        {
            CamFija = false;
        }
        checar_Control();

    }

    private void checarSensibilidad()
    {
        int valorSensMH = PlayerPrefs.GetInt("Sensibilidad_MouseH", 30);
        int valorSensMV = PlayerPrefs.GetInt("Sensibilidad_MouseV", 30);
        int SensMGeneral = PlayerPrefs.GetInt("Sensibilidad_Mouse", 100);       // valor general

        valorSensMH = Mathf.Clamp(valorSensMH, 0, 100);
        valorSensMV = Mathf.Clamp(valorSensMV, 0, 100);
        SensMGeneral = Mathf.Clamp(SensMGeneral, 0, 100);

        float SensmouseH = valorSensMH * (SensMGeneral / 100f);

        int valorSensJH = PlayerPrefs.GetInt("Sensibilidad_joystickH", 30);
        int valorSensJV = PlayerPrefs.GetInt("Sensibilidad_joystickV", 30);
        int SensJGeneral = PlayerPrefs.GetInt("Sensibilidad_joystick", 100);       // valor general

        valorSensJH = Mathf.Clamp(valorSensJH, 0, 100);
        valorSensJV = Mathf.Clamp(valorSensJV, 0, 100);
        SensJGeneral = Mathf.Clamp(SensJGeneral, 0, 100);

        float SensJoyH = valorSensJH * (SensJGeneral / 100f);

        if (IconProvider.UsandoGamepad())
        {
            Sensibilidad = SensJoyH / 100;
        }
        else
        {
            Sensibilidad = SensmouseH / 100;
        }
    }
    public void checar_Control()
    {

        if (CamFija)
        {
            GameObject.Find("Cosas Inutiles").transform.GetChild(2).GetComponent<CinemachineVirtualCamera>().Follow = CabezaGata;
            PlayerPrefs.SetString("CamaraFija", "SI");
            Camara.SetActive(true);
            giro.SetActive(false);
        }
        else
        {
            GameObject.Find("Cosas Inutiles").transform.GetChild(2).GetComponent<CinemachineVirtualCamera>().Follow = Gata;
            PlayerPrefs.SetString("CamaraFija", "NO"); 
            Camara.SetActive(false);
            giro.SetActive(true);
        }
    }
    private void Update()
    {
        IconProvider.ActualizarIconoUI(CambiarCamara, CamaraBoton.transform, ref iconoActualCamara, ref textoActualCamara, false);
        if (CambiarCamara.WasPressedThisFrame())
        {
            if (CamFija)
            {
                CamFija = false;
            }
            else
            {
                CamFija = true;
            }
            checar_Control();
        }
    }
    void FixedUpdate()
    {
        checarSensibilidad();
        float Hor = MoverHorizontal.ReadValue<float>();
        if (Hor != 0)
        {
            GetComponent<Transform>().Rotate(Vector3.up, 1 * Hor * Sensibilidad * velocidad * Time.deltaTime);
        }
    }
}
