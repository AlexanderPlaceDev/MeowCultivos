using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_AnimadorCinematicasPersonajes : MonoBehaviour
{
   public void Animar(string NombreAnimacion)
    {

        GetComponent<Animator>().Play(NombreAnimacion);
    }
}
