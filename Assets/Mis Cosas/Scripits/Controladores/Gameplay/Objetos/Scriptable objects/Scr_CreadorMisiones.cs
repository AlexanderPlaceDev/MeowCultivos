using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Mision", order = 0)]
public class Scr_CreadorMisiones : ScriptableObject
{

    public string Descripcion;
    public string DescripcionFinal;
    public enum Tipos
    {
        Exploracion,
        Teclas,
        Recoleccion,
        Caza
    }
    public Tipos Tipo;
    public KeyCode[] Teclas;
    public bool EsContinua;
    public bool QuitaObjetos;
    public bool DaObjetos;
    public string[] ObjetosQueQuita;
    public string[] ObjetosQueDa;
    public int[] CantidadesQuita;
    public int[] CantidadesDa;
}

