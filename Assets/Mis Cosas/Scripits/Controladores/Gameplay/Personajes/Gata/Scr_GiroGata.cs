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

    private void OnEnable()
    {
        if (!Control) 
        {
            GameObject.Find("Camara 360").GetComponent<CinemachineVirtualCamera>().Follow= Gata;
        }
        else
        {
            GameObject.Find("Camara 360").GetComponent<CinemachineVirtualCamera>().Follow = CabezaGata;
        }
    }
    void FixedUpdate()
    {
        float Hor=Input.GetAxisRaw("Horizontal");
        if(Hor!=0)
        {
            GetComponent<Transform>().Rotate(Vector3.up,1*Hor*velocidad*Time.deltaTime);
        }  
    }
}
