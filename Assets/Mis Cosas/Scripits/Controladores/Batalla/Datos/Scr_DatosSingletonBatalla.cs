using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_DatosSingletonBatalla : MonoBehaviour
{
    public GameObject Enemigo;
    public string NombreMapa;
    public string Mision;
    public Color ColorMision;
    public string Complemento;
    public string Item;
    public Color ColorItem;
    public string Habilidad1;
    public string Habilidad2;
    public string HabilidadEspecial;

    [Header("Recompensa")]
    public List<Scr_CreadorObjetos> ObjetosRecompensa = new List<Scr_CreadorObjetos>();
    public List<int> CantidadesRecompensa= new List<int>();
}
