using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Estructura", order = 0)]
public class Scr_CreadorEstructuras : ScriptableObject
{
    public string Nombre;
    public Sprite Imagen;
    public string Descripcion;
    public string[] Materiales;
    public int[] Tamaños;
    public int[] Cantidades;
}
