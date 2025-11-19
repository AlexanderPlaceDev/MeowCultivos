using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Habilidad", order = 0)]
public class Scr_CreadorHabilidades : ScriptableObject
{
    public string Nombre;
    public Sprite Icono;
    public string NombreBoton;
    public string Descripcion;
    public int Costo;
    public bool RequiereItems;
    public Scr_CreadorObjetos[] ItemsRequeridos;
    public bool RequiereMedallas;
    public int[] CantidadesRequeridas;
}
