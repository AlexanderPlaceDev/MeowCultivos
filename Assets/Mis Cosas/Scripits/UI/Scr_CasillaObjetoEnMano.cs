using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Scr_CasillaObjetoEnMano : MonoBehaviour
{
    public Sprite Vacio;
    public Image CasillaEncontrada;
    public bool EncontroEspacio;


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Image>().sprite==Vacio || other.GetComponent<Image>().sprite==GetComponent<Image>().sprite)
        {
            EncontroEspacio=true;
            CasillaEncontrada=other.GetComponent<Image>();
        }else
        {
            EncontroEspacio=false;
            CasillaEncontrada=null;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
            CasillaEncontrada=null;
            EncontroEspacio=false;
    }
}
