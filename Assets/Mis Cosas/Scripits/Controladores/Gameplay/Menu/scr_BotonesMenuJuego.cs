using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_BotonesMenuJuego : MonoBehaviour
{
    [SerializeField] GameObject[] Botones;
    [SerializeField] Color Seleccionado;

    private void Update()
    {

    }
    public void CambiarFormaBoton(string Boton)
    {
        if (Boton[1] == '0')
        {
            Debug.Log(Botones[int.Parse(Boton[0].ToString())].gameObject.name);
            Botones[int.Parse(Boton[0].ToString())].transform.GetChild(0).GetComponent<Image>().color = Seleccionado;

        }
        else
        {
            Botones[int.Parse(Boton[0].ToString())].transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }

    }

    public void BotonInventario()
    {
        GetComponent<Scr_ControladorMenuJuego>().CambiarMenu(2,true);
    }

}
