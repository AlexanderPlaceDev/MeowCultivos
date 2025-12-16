using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_DatosSingletonBatalla : MonoBehaviour
{
    public GameObject Enemigo;
    public GameObject Fruta;
    public string NombreMapa;
    public string Mision;
    public string NombreFruta;
    public Color ColorMision;
    public string Complemento;
    public string Item;
    public Color ColorItem;
    public string Habilidad1;
    public string Habilidad2;
    public string HabilidadEspecial;
    public Color Luz;
    public int HoraActual;
    public Material SkyBoxDia;
    public Material SkyBoxNoche;
    public int Pista;
    public enum Modo { Pelea, Recoleccion,  Defensa}
    public Modo ModoSeleccionado;
    [Header("Recompensa")]
    public List<Scr_CreadorObjetos> ObjetosRecompensa = new List<Scr_CreadorObjetos>();
    public List<int> CantidadesRecompensa= new List<int>();
    public List<string> EnemigosCazados;
    public List<int> CantidadCazados;
}
