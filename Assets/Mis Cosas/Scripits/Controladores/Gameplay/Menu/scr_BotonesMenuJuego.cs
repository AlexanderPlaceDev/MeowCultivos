using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_BotonesMenuJuego : MonoBehaviour
{
    [SerializeField] GameObject[] Botones;
    [SerializeField] Color Seleccionado;
    [SerializeField] Animator Asistente;

    private void Update()
    {

    }
    public void CambiarFormaBoton(string Boton)
    {
        if (Boton[1] == '0')
        {
            Debug.Log(Botones[int.Parse(Boton[0].ToString())].gameObject.name);
            Botones[int.Parse(Boton[0].ToString())].transform.GetChild(0).GetComponent<Image>().color = Seleccionado;
            switch (Boton[0])
            {
                case '0':
                    {
                        Asistente.GetComponent<Animator>().Play("Habilidades");
                        break;
                    }

                case '1':
                    {
                        Asistente.GetComponent<Animator>().Play("Inventario");
                        break;
                    }
                case '2':
                    {
                        Asistente.GetComponent<Animator>().Play("Guia");
                        break;
                    }
            }

        }
        else
        {
            Botones[int.Parse(Boton[0].ToString())].transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }

    }
}
