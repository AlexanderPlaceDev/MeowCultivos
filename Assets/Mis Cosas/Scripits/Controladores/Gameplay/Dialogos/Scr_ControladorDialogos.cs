using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scr_ControladorDialogos : MonoBehaviour
{

    [SerializeField] string Nombre;
    public int DialogoActual;
    public Scr_CreadorDialogos[] Dialogos;

    Scr_Dialogos Dialogo;
    [SerializeField] Scr_ControladorMisiones ControladorMisiones;
    [SerializeField] Scr_Radio Radio;

    Scr_ControladorInventario Inventario;


    int DialogoGuardado = -1;

    private void Start()
    {
        Dialogo = GetComponent<Scr_Dialogos>();
        try
        {
            Inventario = GameObject.Find("Gata").transform.GetChild(3).GetComponent<Scr_ControladorInventario>();
        }
        catch
        {
            Debug.Log("No existe la Gata 3D");
        }
    }

    private void Update()
    {
        if (DialogoGuardado != DialogoActual)
        {
            Debug.Log("Guardando Dialogo");
            DialogoGuardado = DialogoActual;
            if (Nombre != null)
            {
                GetComponent<Scr_EventosGuardado>().EventoDialogo(DialogoActual, Nombre);
            }
        }


        //En caso de tener un dialogo o mas
        if (Dialogos.Length > 0)
        {
            //En caso de que el dialogo se muestre una sola vez
            if (Dialogos[DialogoActual].EsUnico)
            {
                //Si ya termino de leer
                if (Dialogo.YaLeido)
                {
                    if (Radio != null)
                    {
                        Radio.Lineas = Dialogos[DialogoActual].LineasRadio;
                    }
                    if (!Dialogos[DialogoActual].EsCinematica)
                    {
                        Dialogo.Lineas = Dialogos[DialogoActual].Lineas;
                        DialogoActual++;
                        Dialogo.YaLeido = false;
                    }
                }
                else
                {
                    Dialogo.Lineas = Dialogos[DialogoActual].Lineas;
                }
            }
            else
            {
                //En caso de ser una mision
                if (Dialogos[DialogoActual].EsMision)
                {
                    if (Dialogo.YaLeido)
                    {
                        if (Radio != null)
                        {
                            Radio.Lineas = Dialogos[DialogoActual].LineasRadio;
                        }
                        if (ControladorMisiones != null)
                        {

                            //En caso de que este completa
                            if (ControladorMisiones.MisionCompleta)
                            {
                                if (Dialogo.Comenzo)
                                {
                                    if (ControladorMisiones.MisionActual.QuitaObjetos)
                                    {
                                        QuitarObjetos();
                                    }
                                    ControladorMisiones.MisionActual = null;
                                    ControladorMisiones.MisionCompleta = false;
                                    ControladorMisiones.TeclasPresionadas = null;
                                    DialogoActual++;
                                    Dialogo.Lineas = Dialogos[DialogoActual].Lineas;
                                    Dialogo.YaLeido = false;
                                    Dialogo.StopAllCoroutines();
                                    Dialogo.Texto.text = "";
                                    Dialogo.IniciarDialogo();
                                }



                            }
                            else
                            {
                                Dialogo.Lineas = Dialogos[DialogoActual].Lineas;
                                ControladorMisiones.MisionActual = Dialogos[DialogoActual].Mision;
                                if (Dialogo.Comenzo)
                                {
                                    ControladorMisiones.TeclasPresionadas = null;
                                }
                            }

                        }



                    }
                    else
                    {
                        Dialogo.Lineas = Dialogos[DialogoActual].Lineas;
                    }

                }
                else
                {
                    if (Dialogo.YaLeido)
                    {
                        if (Radio != null)
                        {
                            Radio.Lineas = Dialogos[DialogoActual].LineasRadio;
                        }
                    }
                    Dialogo.Lineas = Dialogos[DialogoActual].Lineas;
                }
            }
        }
    }

    void QuitarObjetos()
    {
        Scr_CreadorMisiones MisionActual = Dialogos[DialogoActual].Mision;
        int ObjetoActual = 0;
        //para cada objeto de la mision
        foreach (string ObjetoNecesario in MisionActual.Objetos)
        {
            int CasillaActual = 0;
            int TotalNecesario = MisionActual.Cantidades[ObjetoActual];
            //para cada item en el inventario
            foreach (string Item in Inventario.CasillasContenido)
            {
                //si el total necesario es mayor a cero
                if (TotalNecesario > 0)
                {
                    if (Item.Contains(ObjetoNecesario))
                    {
                        //En caso de tener mas de los que se piden
                        if (Inventario.Cantidades[CasillaActual] > TotalNecesario)
                        {
                            foreach (Image Casilla in Inventario.Casillas[CasillaActual].GetComponent<Scr_CasillaInventario>().CasillasHermanas)
                            {
                                Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] -= TotalNecesario;
                            }
                            TotalNecesario = 0;
                        }
                        else
                        {
                            //En caso de tener menos de los necesarios
                            if (Inventario.Cantidades[CasillaActual] < TotalNecesario)
                            {
                                bool YaResto = false;
                                foreach (Image Casilla in Inventario.Casillas[CasillaActual].GetComponent<Scr_CasillaInventario>().CasillasHermanas)
                                {
                                    if (!YaResto)
                                    {
                                        YaResto = true;
                                        TotalNecesario -= (int)Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero];
                                    }
                                    Inventario.CasillasContenido[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = "";
                                    Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = 0;
                                }
                            }
                            else
                            {
                                foreach (Image Casilla in Inventario.Casillas[CasillaActual].GetComponent<Scr_CasillaInventario>().CasillasHermanas)
                                {
                                    TotalNecesario = 0;
                                    Inventario.CasillasContenido[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = "";
                                    Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    break;
                }


                CasillaActual++;
            }
            ObjetoActual++;
        }
    }

}
