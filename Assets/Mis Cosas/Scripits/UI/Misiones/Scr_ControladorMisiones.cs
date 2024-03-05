using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Scr_ControladorMisiones : MonoBehaviour
{

    Scr_ControladorInventario Inventario;

    public Scr_CreadorMisiones MisionActual;

    public bool MisionCompleta;

    public bool[] TeclasPresionadas;

    void Start()
    {
        Inventario = GameObject.Find("Gata").transform.GetChild(3).GetComponent<Scr_ControladorInventario>();
    }

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

                case "Conseguir":
                    {
                        int ObjetosCumplidos = 0;
                        int ObjetoActual = 0;
                        foreach (string ObjetoNecesario in MisionActual.Objetos)
                        {
                            int TotalDeObjetos = 0;
                            int CasillaActual=0;
                            foreach (string Item in Inventario.CasillasContenido)
                            {
                                if (Item.Contains(ObjetoNecesario))
                                {
                                    TotalDeObjetos += (int)Inventario.Cantidades[CasillaActual];
                                }
                                CasillaActual++;
                            }

                            if (TotalDeObjetos / MisionActual.Tamaños[ObjetoActual] >= MisionActual.Cantidades[ObjetosCumplidos])
                            {
                                ObjetosCumplidos++;
                            }
                            else
                            {
                                break;
                            }

                            ObjetoActual++;
                        }

                        if (ObjetosCumplidos >= MisionActual.Cantidades.Length)
                        {
                            MisionCompleta = true;
                        }
                        break;
                    }
            }
        }
    }
}
