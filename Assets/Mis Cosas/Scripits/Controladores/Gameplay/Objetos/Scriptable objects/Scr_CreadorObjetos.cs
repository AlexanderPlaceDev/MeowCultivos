using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Item", order = 0)]
public class Scr_CreadorObjetos : ScriptableObject 
{
    public string Nombre;
    public string TipoPlanta;
    [SerializeField, TextArea(4, 6)] public string Descripcion;
    public Sprite Icono;
    public Sprite IconoInventario;
    public Scr_CreadorObjetos[] MaterialesDeProduccion;
    public int[] CantidadMaterialesDeProduccion;
    public int CantidadProducto; 
    public int TiempoDeProduccion;
    public float ValorComposta;
    public int[] MinimoMaximoSembradio;
    public int XPRecolecta;
    public int CantidadVentaMinima;
    public int ValorVentaIndividual;
}



