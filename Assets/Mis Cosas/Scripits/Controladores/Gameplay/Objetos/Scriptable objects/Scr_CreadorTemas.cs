using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Tema", order = 0)]
public class Scr_CreadorTemas : ScriptableObject
{

    public Color[] ColoresMenu;
    [Space]
    public Color[] ColoresHabilidades;
    [Space]
    public Color[] ColoresInventario;
    [Space]
    public Color[] ColoresGuia;
    [Space]
    public Color[] ColoresArmas;
    [Space]
    public Color[] ColoresOpciones;

    [Space]
    public Color[] ColoresMisiones;
}
