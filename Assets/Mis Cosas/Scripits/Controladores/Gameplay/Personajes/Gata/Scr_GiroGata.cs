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
        float Hor = MoverHorizontal.ReadValue<float>();
        if (Hor != 0)
        {
            GetComponent<Transform>().Rotate(Vector3.up, 1 * Hor * velocidad * Time.deltaTime);
        }
    }
}
