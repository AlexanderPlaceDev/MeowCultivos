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



    int DialogoGuardado = -1;

    private void Start()
    {
        Dialogo = GetComponent<Scr_Dialogos>();
    }

    private void Update()
    {
        if (DialogoGuardado != DialogoActual)
        {
            Debug.Log("Guardando Dialogo");
            DialogoGuardado = DialogoActual;
            if (Nombre != null && Nombre!="")
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
                }
                else
                {
                    Dialogo.Lineas = Dialogos[DialogoActual].Lineas;
                    if(Dialogo.Comenzo && Dialogo.Texto.text == "")
                    {
                        Dialogo.IniciarDialogo();
                    }
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
                                    }
                                    ControladorMisiones.MisionActual = null;
                                    ControladorMisiones.MisionCompleta = false;
                                    ControladorMisiones.TeclasPresionadas = null;
                                    Dialogo.StopAllCoroutines();
                                    DialogoActual++;
                                    Dialogo.YaLeido = false;
                                    Dialogo.Texto.text = "";
                                    Dialogo.Lineas = Dialogos[DialogoActual].Lineas;
                                    Dialogo.IniciarDialogo();
                                }



                            }
                            else
                            {

                                GameObject.Find("Gata").GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
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

    

}
