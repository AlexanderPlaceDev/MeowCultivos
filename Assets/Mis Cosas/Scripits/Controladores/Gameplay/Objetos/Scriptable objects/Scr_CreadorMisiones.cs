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
    public string DescripcionCompleta;
    public Scr_CreadorDialogos DialogoEnMision;
    public Scr_CreadorDialogos DialogoMisionCompleta;
    public enum prioridadM
    {
        Principal,
        Secundaria
    }
    public prioridadM prioridad;
    public enum Tipos
    {
        Exploracion,
        Teclas,
        Movimiento,
        Recoleccion,//modo de recoleccion
        Recolectar,// conseguir ciertos objetos
        Defensa,
        Construccion,
        Crafteo,
        Pesca,
        Caza
    }
    public Tipos Tipo;

    public enum cazarenemigo
    {
        jaba,
        gallina,
        lobo
    }
    public Scr_CreadorObjetos[] ObjetosNecesarios;
    public int[] CantidadesQuita;
    public string[] ObjetivosACazar;
    public int[] CantidadACazar;
    public Sprite[] IconosACazar;
    public enum DireccionMovimiento
    {
        Arriba,
        Abajo,
        Izquierda,
        Derecha
    }

    public DireccionMovimiento[] DireccionesRequeridas;
    public InputActionReference AccionMovimientoH;
    public InputActionReference AccionMovimientoV;
    public KeyCode[] Teclas;
    public bool EsContinua;
    public bool QuitaObjetos;
    public bool DaObjetos;
    public int RecompensaDinero;
    public int RecompensaXP;
    public Scr_CreadorObjetos[] ObjetosQueDa;
    public int[] CantidadesDa;
    public GameObject[] objetosCostruir;
    public string TargetExplorado;
}

