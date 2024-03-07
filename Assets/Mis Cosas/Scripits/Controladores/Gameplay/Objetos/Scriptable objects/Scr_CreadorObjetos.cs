using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Item", order = 0)]
public class Scr_CreadorObjetos : ScriptableObject 
{
    public string Nombre;
    public int Tamaño;
    public bool[] Forma;
    public Sprite[] Iconos;

}



