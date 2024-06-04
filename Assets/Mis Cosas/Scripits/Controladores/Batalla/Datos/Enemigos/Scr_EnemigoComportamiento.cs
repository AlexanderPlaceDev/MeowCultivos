using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_EnemigoComportamiento : MonoBehaviour
{
    private Scr_Enemigo BaseEnemigo;
    Transform Jugador;

    void Start()
    {
        Jugador = Camera.main.transform;
        BaseEnemigo = GetComponent<Scr_Enemigo>();
    }

    void Update()
    {
        switch (BaseEnemigo.tipocomportamiento)
        {
            case Scr_Enemigo.TipoComportamiento.Agresivo:
                ComportamientoAgresivo();
                break;
            case Scr_Enemigo.TipoComportamiento.Miedoso:
                ComportamientoMiedoso();
                break;
            case Scr_Enemigo.TipoComportamiento.Pacifico:
                ComportamientoPacifico();
                break;
        }
    }

    void ComportamientoAgresivo()
    {
        // Comportamiento audaz: se acerca agresivamente al jugador
        BaseEnemigo.Mover();
        //Debug.Log("Distancia: " + Vector3.Distance(transform.position, Jugador.position));
        if (Vector3.Distance(transform.position, Jugador.position) < BaseEnemigo.Rango)
        {
            BaseEnemigo.Objetivo = null;
            if (BaseEnemigo.tipoenemigo == Scr_Enemigo.TipoEnemigo.Terrestre)
                BaseEnemigo.AtaqueMelee();
            else
                BaseEnemigo.AtaqueDistancia();
        }
        else
        {
            BaseEnemigo.Objetivo = Camera.main.transform;
        }
    }

    void ComportamientoMiedoso()
    {
        // Comportamiento miedoso: se aleja del jugador
        if (Vector3.Distance(transform.position, Jugador.position) < BaseEnemigo.Rango)
        {
            transform.Translate(-Vector3.forward * BaseEnemigo.Velocidad * Time.deltaTime);
        }
    }

    void ComportamientoPacifico()
    {
        // Comportamiento pacífico: no ataca, solo se mueve aleatoriamente
        transform.Translate(Vector3.forward * BaseEnemigo.Velocidad * Time.deltaTime);
    }
}
