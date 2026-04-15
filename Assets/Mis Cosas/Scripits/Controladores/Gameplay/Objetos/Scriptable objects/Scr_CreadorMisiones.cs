using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Mision", order = 0)]
public class Scr_CreadorMisiones : ScriptableObject
{
    public string TituloMision;
    public Sprite LogoMision;
    public string DescripcionEnMision;
    public string DescripcionMisionCompleta;
    public Scr_CreadorDialogos DialogoEnMision;
    public Scr_CreadorDialogos DialogoMisionCompleta;
    public enum prioridadM
    {
        Principal,
        Secundaria
    }
    public string Personaje;
    public string LugarMision;
    public prioridadM prioridad;
    public enum Tipos
    {
        Exploracion,
        Movimiento,
        Teclas,
        Recoleccion,// conseguir ciertos objetos
        Defensa,
        Construccion,
        Caza
    }

    public Tipos Tipo;
    public string LugarObjetivoExploracion;
    public enum DireccionMovimiento
    {
        Arriba,
        Abajo,
        Izquierda,
        Derecha
    }

    public DireccionMovimiento[] DireccionesRequeridas;
    public KeyCode[] Teclas;
    public Scr_CreadorObjetos[] ObjetosNecesarios;
    public int[] CantidadesQuita;
    public string[] ObjetivosACazar;
    public int[] CantidadACazar;
    public Sprite[] IconosACazar;
    public string[] EstructurasRequeridas;
    
    public int RecompensaDinero;
    public int RecompensaXP;
    public Scr_CreadorObjetos[] ObjetosQueDa;
    public int[] CantidadesQueDa;
    public Scr_CreadorObjetos[] ObjetosQueQuita;
    public int[] CantidadesQueQuita;
    public bool QuitaObjetosDesdeSignal;
    public Scr_CreadorMisiones MisionAnterior;
}

