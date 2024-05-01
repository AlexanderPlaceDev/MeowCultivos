using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_AnimacionesGusano : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
        if(GetComponent<Scr_SistemaDialogos>().Leyendo)
        {
            GetComponent<Animator>().SetBool("Hablando",true);
        }else
        {
            GetComponent<Animator>().SetBool("Hablando",false);
        }
        
    }
}
