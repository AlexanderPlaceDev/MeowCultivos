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
    private void OnEnable()
    {
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        MoverHorizontal = playerInput.actions["MoverHorizontal"];
        CambiarCamara = playerInput.actions["CamaraLibre"];
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
        if (!CamFija)
        {
            GameObject.Find("Cosas Inutiles").transform.GetChild(2).GetComponent<CinemachineVirtualCamera>().Follow = Gata;
            //GetComponent<Scr_Movimiento>().UsaEjeHorizontal = false;
        }
        else
        {
            GameObject.Find("Cosas Inutiles").transform.GetChild(2).GetComponent<CinemachineVirtualCamera>().Follow = CabezaGata;
            //GetComponent<Scr_Movimiento>().UsaEjeHorizontal = false;
        }

        if (CamFija)
        {
            PlayerPrefs.SetString("CamaraFija", "SI");
        }
        else
        {
            PlayerPrefs.SetString("CamaraFija", "NO");
        }
    }
    private void Update()
    {
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
