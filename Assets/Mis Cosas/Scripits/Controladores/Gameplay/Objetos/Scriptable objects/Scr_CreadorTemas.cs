using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Tema", order = 0)]
public class Scr_CreadorTemas : ScriptableObject
{
    public Color BarrasMenu1;
    public Color BarrasMenu2;
    public Color FondoMenu;
    public Color AreaDiaMenu;
    [Space]
    public Color BarrasInventario1;
    public Color BarrasInventario2;
    public Color FondoInventario;
    public Color AreaDiaInventario;
}
