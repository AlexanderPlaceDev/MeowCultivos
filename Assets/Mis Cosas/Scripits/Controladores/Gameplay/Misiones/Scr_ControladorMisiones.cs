using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Scr_ControladorMisiones : MonoBehaviour
{

    public Scr_CreadorMisiones MisionActual;

    public bool MisionCompleta;

    public bool[] TeclasPresionadas;

    void Update()
    {

        //Comprueba si la mision esta completa
        ComprobarMision();
    }
    void ComprobarMision()
    {
        if (MisionActual != null)
        {
            switch (MisionActual.Tipo)
            {
                case "Teclas":
                    {
                        if (TeclasPresionadas != null && TeclasPresionadas.Length > 0)
                        {
                            for (int i = 0; i < MisionActual.Teclas.Length; i++)
                            {
                                if (Input.GetKeyDown(MisionActual.Teclas[i]))
                                {
                                    TeclasPresionadas[i] = true;
                                }
                            }

                            for (int i = 0; i < TeclasPresionadas.Length; i++)
                            {
                                if (!TeclasPresionadas[i])
                                {
                                    break;
                                }
                                if (i == TeclasPresionadas.Length - 1)
                                {
                                    MisionCompleta = true;
                                    TeclasPresionadas = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            TeclasPresionadas = new bool[MisionActual.Teclas.Length];
                        }

                        break;
                    }

            }
        }
    }
}
