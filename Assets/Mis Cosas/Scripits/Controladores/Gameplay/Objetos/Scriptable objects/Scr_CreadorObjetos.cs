using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Item", order = 0)]
public class Scr_CreadorObjetos : ScriptableObject 
{
    public string Nombre;
    [SerializeField, TextArea(4, 6)] public string Descripcion;
    public int Tama�o;
    public Sprite IconoTama�o;
    public Color ColorTama�o;
    public bool[] Forma;
    public Sprite Icono;
    public Sprite[] IconosInventario;
    public Scr_CreadorObjetos[] MaterialesDeProduccion;
    public int[] CantidadMaterialesDeProduccion;
    public int TiempoDeProduccion;

}



