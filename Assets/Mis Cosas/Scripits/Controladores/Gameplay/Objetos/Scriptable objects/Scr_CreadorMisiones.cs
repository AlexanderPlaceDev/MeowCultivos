using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Mision", order = 0)]
public class Scr_CreadorMisiones : ScriptableObject
{
    public string MisionName;
    public string Descripcion;
    public string DescripcionFinal;
    public string DialogoEnMision;
    public string DialogoMisionCompleta;
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
        Recoleccion,
        Construccion,
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
    public string[] ObjetivosACazar;
    public int[] CantidadACazar;
    public Sprite[] IconosACazar;
    public KeyCode[] Teclas;
    public bool EsContinua;
    public bool QuitaObjetos;
    public bool DaObjetos;
    public int RecompensaDinero;
    public int RecompensaXP;
    //public string[] ObjetosQueQuita;
    //public string[] ObjetosQueDa;
    public int[] CantidadesQuita;
    public int[] CantidadesDa;
    public GameObject[] objetosCostruir;
    public string TargetExplorado;
}

