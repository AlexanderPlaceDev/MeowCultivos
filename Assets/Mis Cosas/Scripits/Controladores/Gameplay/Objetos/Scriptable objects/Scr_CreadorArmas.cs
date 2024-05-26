using UnityEngine;

[CreateAssetMenu(fileName = "NuevaArma", menuName = "Crear Objeto/Arma")]
public class Scr_CreadorArmas : ScriptableObject
{
    public Sprite Icono;
    public Color Color;
    public string Tipo;
    public string Nombre;
    public string Descripcion;
    public int Cadencia;
    public int Capacidad;
    public int CapacidadTotal;
    public int Rango;
    public int Daño;
    public int Velocidad;
    public bool DobleMano;

}