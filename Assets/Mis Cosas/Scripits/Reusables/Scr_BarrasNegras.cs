using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_BarrasNegras : MonoBehaviour
{
    public void Awake()
    {
        GetComponent<Animator>().Play("Abrir");
    }

    public void Start()
    {
        GetComponent<Animator>().Play("Abrir");
    }


}
