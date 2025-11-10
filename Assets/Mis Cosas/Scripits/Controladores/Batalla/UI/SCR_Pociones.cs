using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearPocion", menuName = "Crear Objeto/Pocion")]
public class SCR_Pociones : ScriptableObject
{
    public string Nombre;
    public Sprite Icono;
    public int Nivel;
    public enum TipoPocion { Curativo, Dano, Velocidad, Resitencia, Vida };
    public TipoPocion Tipo;
    public string Descripcion;
    public bool RequiereCraftear;
    public Scr_CreadorObjetos[] ItemsRequeridos;
    public int[] CantidadesRequeridas;
    public bool Permanente;
    public int Usos;
    public float Puntos;
    public Scr_Enemigo.TipoEfecfto efecto;
    public Color Color;
}
