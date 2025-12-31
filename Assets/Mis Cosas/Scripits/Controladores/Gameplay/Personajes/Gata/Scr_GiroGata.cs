using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_GiroGata : MonoBehaviour
{
    public Transform Gata;
    public Transform CabezaGata;
    public bool Control;
    public float velocidad;
    Rigidbody rb;
    private void OnEnable()
    {
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
    void FixedUpdate()
    {
        float Hor = Input.GetAxisRaw("Horizontal");
        if (Hor != 0)
        {
            GetComponent<Transform>().Rotate(Vector3.up, 1 * Hor * velocidad * Time.deltaTime);
        }
    }
}
