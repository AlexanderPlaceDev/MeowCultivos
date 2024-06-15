using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_DatosSingletonBatalla : MonoBehaviour
{
    public GameObject Enemigo;
    public int CantidadDeEnemigos;
    public GameObject Mapa;

    [Header("Recompensa")]
    public List<Scr_CreadorObjetos> ObjetosRecompensa = new List<Scr_CreadorObjetos>();
    public List<int> CantidadesRecompensa= new List<int>();
}
