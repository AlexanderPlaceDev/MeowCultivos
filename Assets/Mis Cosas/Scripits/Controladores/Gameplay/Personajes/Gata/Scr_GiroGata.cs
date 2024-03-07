using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_GiroGata : MonoBehaviour
{
    public float velocidad;
    void FixedUpdate()
    {
        float Hor=Input.GetAxisRaw("Horizontal");
        if(Hor!=0)
        {
            GetComponent<Transform>().Rotate(Vector3.up,1*Hor*velocidad*Time.deltaTime);
        }  
    }
}
