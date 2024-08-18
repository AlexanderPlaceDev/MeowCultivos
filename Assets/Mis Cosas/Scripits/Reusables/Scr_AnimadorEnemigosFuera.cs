using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_AnimadorEnemigosFuera : MonoBehaviour
{

    Animator Animador;
    Scr_MovimientoEnemigosFuera Movimiento;
    public string Iddle;
    public string Mover;
    void Start()
    {
        Animador = GetComponent<Animator>();
        Movimiento = GetComponent<Scr_MovimientoEnemigosFuera>();
    }

    void Update()
    {
        if (Animador.GetCurrentAnimatorStateInfo(0).IsName(Iddle) && Movimiento.SeEstaMoviendo)
        {
            Animador.Play(Mover);
        }
    }
}

