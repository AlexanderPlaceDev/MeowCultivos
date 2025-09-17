using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/HabilidadBatalla", order = 0)]
public class Scr_CreadorHabilidadesBatalla : ScriptableObject
{
    public string Nombre;
    public Sprite Icono;
    public string Tipo;
    public string Descripcion;
    public bool RequiereCraftear;
    public Scr_CreadorObjetos[] ItemsRequeridos;
    public int[] CantidadesRequeridas;
    public int Usos;
    public string Arma;
}
