using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Scr_GiroGata : MonoBehaviour
{
    public Transform Gata;
    public Transform CabezaGata;
    public bool Control;
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
        checar_Control();
    }

    public void checar_Control()
    {
        if (!Control)
        {
            GameObject.Find("Camara 360").GetComponent<CinemachineVirtualCamera>().Follow = Gata;
            //GetComponent<Scr_Movimiento>().UsaEjeHorizontal = false;
        }
        else
        {
            GameObject.Find("Camara 360").GetComponent<CinemachineVirtualCamera>().Follow = CabezaGata;
            //GetComponent<Scr_Movimiento>().UsaEjeHorizontal = false;
        }
    }
    private void Update()
    {
        if (CambiarCamara.IsInProgress())
        {
            if (Control)
            {
                Control = false;
            }
            else
            {
                Control = true;
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
