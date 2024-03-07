using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Mision", order = 0)]
public class Scr_CreadorMisiones : ScriptableObject
{

    public string Nombre;
    public string Tipo;
    public KeyCode[] Teclas;
    public bool QuitaObjetos;
    public string[] Objetos;
    public int[] Cantidades;
    public int[] Tamaños;
}

