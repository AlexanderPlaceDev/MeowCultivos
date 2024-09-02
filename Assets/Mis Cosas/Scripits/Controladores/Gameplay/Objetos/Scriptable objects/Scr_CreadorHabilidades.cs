using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Habilidad", order = 0)]
public class Scr_CreadorHabilidades : ScriptableObject
{
    public string Nombre;
    public string NombreBoton;
    public string Descripcion;
    public string[] HabilidadesAnteriores;
    public int Costo;
}
