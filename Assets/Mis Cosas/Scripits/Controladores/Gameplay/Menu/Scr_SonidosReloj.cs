using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_SonidosReloj : MonoBehaviour
{
    public void SonidoReloj()
    {
        GetComponent<AudioSource>().Play();
    }
}

