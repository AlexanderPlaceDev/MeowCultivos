using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Item", order = 0)]
public class Scr_CreadorObjetos : ScriptableObject 
{
    public int ID;
    public string Nombre;
    [SerializeField, TextArea(4, 6)] public string Descripcion;
    public Sprite Icono;
    public Scr_CreadorObjetos[] MaterialesDeProduccion;
    public int[] CantidadMaterialesDeProduccion;
    public int TiempoDeProduccion;

}



