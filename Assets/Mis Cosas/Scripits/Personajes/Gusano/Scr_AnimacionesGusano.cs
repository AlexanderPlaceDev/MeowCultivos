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
        
        if(GetComponent<Scr_Dialogos>().Comenzo)
        {
            GetComponent<Animator>().SetBool("Hablando",true);
        }else
        {
            GetComponent<Animator>().SetBool("Hablando",false);
        }
        
    }
}
