using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scr_ControladorInventario : MonoBehaviour
{
    [Header("Datos Generales")]
    public Scr_CreadorObjetos[] TodosLosObjetos;
    public Image[] Casillas;
    public TextMeshProUGUI[] CasillasTexto;
    public string[] CasillasContenido;
    public float[] Cantidades;
    [SerializeField] public Scr_ObjetoEnMano ObjetoEnMano;
    [SerializeField] Sprite CasillaVacia;

    void LateUpdate()
    {
        int cont = 0;
        foreach (string Item in CasillasContenido)
        {
            foreach (Scr_CreadorObjetos Objeto in TodosLosObjetos)
            {
                if (Objeto.Nombre == Item)
                {
                    Casillas[cont].sprite = Objeto.Imagen;
                    CasillasTexto[cont].text = Cantidades[cont].ToString();
                }
            }

            if (Item == "")
            {
                Casillas[cont].sprite = CasillaVacia;
                CasillasTexto[cont].text = "";
            }
            cont++;
        }

    }

    public void AgarrarObjetoDelInventario(Image[] Casillas, bool Mitad, bool Uno)
    {


        if (ObjetoEnMano.Nombre == "")
        {

            foreach (Image Casilla in Casillas)
            {

                Scr_CasillaInventario CasillaInventario = Casilla.GetComponent<Scr_CasillaInventario>();
                ObjetoEnMano.Forma = CasillaInventario.FormaConHermanas;

                if (!Mitad && !Uno)
                {
                    //Actualizar Objeto En mano
                    ObjetoEnMano.Cantidad = Cantidades[(int)CasillaInventario.Numero];

                    if (Casillas.Length > 1)
                    {
                        ObjetoEnMano.Nombre = CasillasContenido[(int)CasillaInventario.Numero].Substring(0, CasillasContenido[(int)CasillaInventario.Numero].Length - 1);
                    }
                    else
                    {
                        ObjetoEnMano.Nombre = CasillasContenido[(int)CasillaInventario.Numero];
                    }

                    int UltimaPosicion = 0;
                    foreach (bool CasillaActiva in ObjetoEnMano.Forma)
                    {
                        if (CasillaActiva && ObjetoEnMano.Iconos[UltimaPosicion] == null)
                        {
                            ObjetoEnMano.Iconos[UltimaPosicion] = Casilla.sprite;
                            break;
                        }
                        UltimaPosicion++;
                    }

                    //Actualizar Casilla del Inventario
                    CasillasContenido[(int)CasillaInventario.Numero] = "";
                    Cantidades[(int)CasillaInventario.Numero] = 0;
                    CasillaInventario.ReiniciarCasilla();
                }
                else
                {
                    if (Mitad)
                    {
                        if (Casillas.Length > 1)
                        {
                            ObjetoEnMano.Nombre = CasillasContenido[(int)CasillaInventario.Numero].Substring(0, CasillasContenido[(int)CasillaInventario.Numero].Length - 1);
                        }
                        else
                        {
                            ObjetoEnMano.Nombre = CasillasContenido[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero];
                        }

                        int UltimaPosicion = 0;
                        foreach (bool CasillaActiva in ObjetoEnMano.Forma)
                        {
                            if (CasillaActiva && ObjetoEnMano.Iconos[UltimaPosicion] == null)
                            {
                                ObjetoEnMano.Iconos[UltimaPosicion] = Casilla.sprite;
                                break;
                            }
                            UltimaPosicion++;
                        }

                        if (Cantidades[(int)CasillaInventario.Numero] > 1)
                        {
                            float Division = Cantidades[(int)CasillaInventario.Numero] / 2;

                            if (Division == Mathf.Round(Division))
                            {
                                ObjetoEnMano.Cantidad = Cantidades[(int)CasillaInventario.Numero] / 2;
                                Cantidades[(int)CasillaInventario.Numero] = Cantidades[(int)CasillaInventario.Numero] / 2;
                            }
                            else
                            {
                                ObjetoEnMano.Cantidad = (int)(Division + 0.5f);
                                Cantidades[(int)CasillaInventario.Numero] = (int)(Division - 0.5f);
                            }
                        }
                        else
                        {
                            ObjetoEnMano.Cantidad = 1;
                            CasillasContenido[(int)CasillaInventario.Numero] = "";
                            Cantidades[(int)CasillaInventario.Numero] = 0;
                            CasillaInventario.ReiniciarCasilla();
                        }
                    }
                    else
                    {
                        if (Casillas.Length > 1)
                        {
                            ObjetoEnMano.Nombre = CasillasContenido[(int)CasillaInventario.Numero].Substring(0, CasillasContenido[(int)CasillaInventario.Numero].Length - 1);
                        }
                        else
                        {
                            ObjetoEnMano.Nombre = CasillasContenido[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero];
                        }

                        int UltimaPosicion = 0;
                        foreach (bool CasillaActiva in ObjetoEnMano.Forma)
                        {
                            if (CasillaActiva && ObjetoEnMano.Iconos[UltimaPosicion] == null)
                            {
                                ObjetoEnMano.Iconos[UltimaPosicion] = Casilla.sprite;
                                break;
                            }
                            UltimaPosicion++;
                        }

                        if (Cantidades[(int)CasillaInventario.Numero] > 1)
                        {

                            ObjetoEnMano.Cantidad = 1;
                            Cantidades[(int)CasillaInventario.Numero]--;

                        }
                        else
                        {
                            ObjetoEnMano.Cantidad = 1;
                            CasillasContenido[(int)CasillaInventario.Numero] = "";
                            Cantidades[(int)CasillaInventario.Numero] = 0;
                            CasillaInventario.ReiniciarCasilla();
                        }
                    }
                }
            }
        }
    }

    public void EliminarObjeto(Image[] Casillas)
    {
       foreach (Image Casilla in Casillas)
        {
            Scr_CasillaInventario CasillaInventario = Casilla.GetComponent<Scr_CasillaInventario>();

            CasillasContenido[(int)CasillaInventario.Numero]="";
            Cantidades[(int)CasillaInventario.Numero]=0;
            CasillaInventario.ReiniciarCasilla();
        }
    }
}
