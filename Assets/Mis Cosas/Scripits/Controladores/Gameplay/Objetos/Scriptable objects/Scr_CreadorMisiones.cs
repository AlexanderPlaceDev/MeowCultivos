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
    public string Descripcion;
    public Scr_CreadorDialogos DialogoEnMision;
    public Scr_CreadorDialogos DialogoMisionCompleta;
    public enum prioridadM
    {
        Principal,
        Secundaria
    }
    public string Faccion;
    public prioridadM prioridad;
    public enum Tipos
    {
        Exploracion,
        Movimiento,
        Teclas,
        Recoleccion,// conseguir ciertos objetos
        Recolectar,//modo de recoleccion
        Defensa,
        Construccion,
        Caza
    }

    public Tipos Tipo;
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

    
    public bool QuitaObjetos;
    public bool DaObjetos;
    public int RecompensaDinero;
    public int RecompensaXP;
    public Scr_CreadorObjetos[] ObjetosQueDa;
    public int[] CantidadesDa;
}

