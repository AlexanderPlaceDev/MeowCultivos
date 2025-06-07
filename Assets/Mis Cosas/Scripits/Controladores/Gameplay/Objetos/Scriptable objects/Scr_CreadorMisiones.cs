using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Mision", order = 0)]
public class Scr_CreadorMisiones : ScriptableObject
{
    public string MisionName;
    public string Descripcion;
    public string DescripcionFinal;
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
        gallina
    }
    public cazarenemigo[] Objetivocaza;
    public int[] cantidad_caza;
    public KeyCode[] Teclas;
    public bool EsContinua;
    public bool QuitaObjetos;
    public bool DaObjetos;
    public Scr_CreadorObjetos[] ObjetosNecesarios;
    public Scr_CreadorObjetos[] ObjetosRecompensa;
    //public string[] ObjetosQueQuita;
    //public string[] ObjetosQueDa;
    public int[] CantidadesQuita;
    public int[] CantidadesDa;


    public string TargetExplorado;
}

