using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ActivadorLuzHoguera : MonoBehaviour
{

    [SerializeField] Light Luz;
    [SerializeField] Scr_MenuEstructuraProducible Estructura;
    void Start()
    {
        
    }

    void Update()
    {
        if (Estructura.Produciendo)
        {
            Luz.enabled = true;
        }
        else
        {
            Luz.enabled = false;
        }
    }
}
