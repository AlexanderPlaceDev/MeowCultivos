using UnityEngine;

[CreateAssetMenu(fileName = "NuevaArma", menuName = "Crear Objeto/Arma")]
public class Scr_CreadorArmas : ScriptableObject
{
    public Sprite Icono;
    public Color Color;
    public string Tipo;
    public string Nombre;
    public string Descripcion;
    public float Cadencia;
    public int Capacidad;
    public int CapacidadTotal;
    public int Alcance;
    public int Daño;
    public float PuntosXGolpe;
    public int Velocidad;
    public bool DobleMano;
}