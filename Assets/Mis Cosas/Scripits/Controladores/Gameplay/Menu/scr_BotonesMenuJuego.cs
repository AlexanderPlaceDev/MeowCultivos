using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scr_BotonesMenuJuego : MonoBehaviour
{
    [SerializeField] Color[] Colores;
    [SerializeField] GameObject[] Botones;
    [SerializeField] Sprite[] Iconos;
    [SerializeField] Animator Asistente;

    public int BotonActual = 0;
    bool Esperando = false;

    private void Update()
    {
        if (BotonActual < 0)
        {
            BotonActual = 4;
        }
        else if (BotonActual > 4)
        {
            BotonActual = 0;
        }

        ActualiarNotificacion();
    }

    public void OnEnable()
    {
        ChecarMenu();
    }

    public void ChecarMenu()
    {
        switch (GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuActual)
        {
            case "Inventario":
                BotonActual = 0;
                break;
            case "Habilidades":
                BotonActual = 1;
                break;
            case "Guia":
                BotonActual = 2;
                break;
            case "Salir":
                BotonActual = 3;
                break;
            case "Opciones":
                BotonActual = 4;
                break;
            case "Armas":
                BotonActual = 5;
                break;
            case "Misiones":
                BotonActual = 6;
                break;
        }
        ChecarBotones();
    }
    public void PrenderBoton(string BotonySi)
    {
        //Primer 1 es igual a entra al boton

        if (BotonySi[0] == '1')
        {
            if (BotonySi[1] == '0')
            {
                Botones[4].GetComponent<Image>().color = Colores[0];
            }
            else
            {
                if (BotonySi[1] == '1')
                {
                    Botones[5].GetComponent<Image>().color = Colores[0];
                }
                else
                {
                    if (BotonySi[2] == '1')
                    {
                        Botones[3].transform.GetChild(0).GetComponent<Image>().color = Colores[0];
                        Botones[3].transform.GetChild(1).GetComponent<Image>().color = Colores[0];
                    }
                    else
                    {
                        if (BotonySi[2] == '2')
                        {
                            Botones[2].transform.GetChild(0).GetComponent<Image>().color = Colores[0];
                            Botones[2].transform.GetChild(1).GetComponent<Image>().color = Colores[0];
                        }
                        else
                        {
                            Botones[1].transform.GetChild(0).GetComponent<Image>().color = Colores[0];
                            Botones[1].transform.GetChild(1).GetComponent<Image>().color = Colores[0];
                        }
                    }

                }
            }
        }
        //Si es diferente sale del boton
        else
        {
            Botones[5].GetComponent<Image>().color = Colores[1];
            //Botones[6].GetComponent<Image>().color = Colores[1];
            Botones[0].transform.GetChild(0).GetComponent<Image>().color = Colores[1];
            Botones[0].transform.GetChild(1).GetComponent<Image>().color = Colores[1];
            Botones[1].transform.GetChild(0).GetComponent<Image>().color = Colores[1];
            Botones[1].transform.GetChild(1).GetComponent<Image>().color = Colores[1];
            Botones[2].transform.GetChild(0).GetComponent<Image>().color = Colores[1];
            Botones[2].transform.GetChild(1).GetComponent<Image>().color = Colores[1];
            Botones[3].transform.GetChild(0).GetComponent<Image>().color = Colores[1];
            Botones[3].transform.GetChild(1).GetComponent<Image>().color = Colores[1];
            //Botones[4].transform.GetChild(0).GetComponent<Image>().color = Colores[1];
            //Botones[4].transform.GetChild(1).GetComponent<Image>().color = Colores[1];
        }
    }

    public void ClicFlecha(bool Sube)
    {
        if (!Esperando)
        {
            Girar(Sube);

            switch (BotonActual)
            {
                case 0:
                    Asistente.GetComponent<Animator>().Play("Mochila");
                    break;
                case 1:
                    Asistente.GetComponent<Animator>().Play("Habilidades");
                    break;
                case 2:
                    Asistente.GetComponent<Animator>().Play("Guia");
                    break;
                case 3:
                    Asistente.GetComponent<Animator>().Play("Salir");
                    break;
                case 4:
                    Asistente.GetComponent<Animator>().Play("Opciones");
                    break;
                case 5:
                    Asistente.GetComponent<Animator>().Play("Armas");
                    break;
                case 6:
                    Asistente.GetComponent<Animator>().Play("Misiones");
                    break;
            }
            ChecarBotones();
        }
    }

    public void ChecarBotones()
    {
        StartCoroutine(Esperar());
    }

    public IEnumerator Esperar()
    {
        Esperando = true;
        SuaveDesactivarNotificacion(Botones[2].transform.GetChild(2).gameObject);
        yield return new WaitForSeconds(0.1f);
        SuaveActivarNotificacion(Botones[2].transform.GetChild(2).gameObject);

        // Actualizar los iconos y el texto de los botones
        switch (BotonActual)
        {
            case 0:
                Asistente.GetComponent<Animator>().Play("Inventario");
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuActual = "Inventario";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuArriba = "Habilidades";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuAbajo = "Opciones";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Inventario";
                //Boton Arriba
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[2];
                //Boton Medio
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[0];
                //Boton Abajo
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[4];
                break;
            case 1:
                Asistente.GetComponent<Animator>().Play("Habilidades");
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuActual = "Habilidades";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuArriba = "Guia";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuAbajo = "Inventario";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Habilidades";
                //Boton Arriba
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[6];
                //Boton Medio
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[2];
                //Boton Abajo
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[0];
                break;

            case 2:
                Asistente.GetComponent<Animator>().Play("Guia");
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuActual = "Guia";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuArriba = "Salir";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuAbajo = "Habilidades";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Guia";

                //Boton Arriba
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[3];
                //Boton Medio
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[1];
                //Boton Abajo
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[2];
                break;

            case 3:
                Asistente.GetComponent<Animator>().Play("Salir");
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuActual = "Salir";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuArriba = "Opciones";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuAbajo = "Guia";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Salir";
                //Boton Arriba
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[4];
                //Boton Medio
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[3];
                //Boton Abajo
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[1];
                break;
            case 4:
                Asistente.GetComponent<Animator>().Play("Opciones");
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuActual = "Opciones";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuArriba = "Inventario";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuAbajo = "Salir";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Opciones";
                //Boton Arriba
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[0];
                //Boton Medio
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[4];
                //Boton Abajo
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[3];
                break;
            case 5:
                Asistente.GetComponent<Animator>().Play("Armas");
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuActual = "Armas";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuArriba = "Guia";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuAbajo = "Opciones";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Armas";
                //Boton Arriba
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[6];
                //Boton Medio
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[5];
                //Boton Abajo
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[4];
                break;
            case 6:
                Asistente.GetComponent<Animator>().Play("Misiones");
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuActual = "Misiones";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuArriba = "Inventario";
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_CambiadorMenus>().MenuAbajo = "Armas";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Misiones";

                //Boton Arriba
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[0];
                //Boton Medio
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[6];
                //Boton Abajo
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[5];
                break;
        }

        GetComponent<Animator>().Play("PosicionDefecto");
        Debug.Log("Cambiaron Posicion");

        Esperando = false;
    }

    public void Girar(bool Sube)
    {
        if (Sube)
        {
            BotonActual++;
            GetComponent<Animator>().Play("GirarArriba");
        }
        else
        {
            BotonActual--;
            GetComponent<Animator>().Play("GirarAbajo");
        }
    }

    void ActualiarNotificacion()
    {
        Debug.Log(BotonActual + "BotonA");
        if (PlayerPrefs.GetInt("PuntosDeHabilidad", 0) > 0)
        {
            Botones[1].transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("PuntosDeHabilidad", 0).ToString();
            Botones[2].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("PuntosDeHabilidad", 0).ToString();
            Botones[3].transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("PuntosDeHabilidad", 0).ToString();
            switch (BotonActual)
            {
                //Inventario
                case 0:
                    Debug.Log("Entra0Puntos");
                    SuaveActivarNotificacion(Botones[1].transform.GetChild(2).gameObject);
                    SuaveDesactivarNotificacion(Botones[2].transform.GetChild(3).gameObject);
                    SuaveDesactivarNotificacion(Botones[3].transform.GetChild(2).gameObject);
                    break;
                    //Habilidades
                case 1:
                    Debug.Log("Entra1Puntos");
                    SuaveDesactivarNotificacion(Botones[1].transform.GetChild(2).gameObject);
                    SuaveActivarNotificacion(Botones[2].transform.GetChild(3).gameObject);
                    SuaveDesactivarNotificacion(Botones[3].transform.GetChild(2).gameObject);
                    break;
                    //Misiones
                case 2:
                    Debug.Log("Entra2Puntos");
                    SuaveDesactivarNotificacion(Botones[1].transform.GetChild(2).gameObject);
                    SuaveDesactivarNotificacion(Botones[2].transform.GetChild(3).gameObject);
                    SuaveActivarNotificacion(Botones[3].transform.GetChild(2).gameObject);
                    break;
                default:
                    Debug.Log("EntraDefaultPuntos");
                    SuaveDesactivarNotificacion(Botones[1].transform.GetChild(2).gameObject);
                    SuaveDesactivarNotificacion(Botones[2].transform.GetChild(3).gameObject);
                    SuaveDesactivarNotificacion(Botones[3].transform.GetChild(2).gameObject);
                    break;
            }
        }
        else
        {
            SuaveDesactivarNotificacion(Botones[1].transform.GetChild(2).gameObject);
            SuaveDesactivarNotificacion(Botones[2].transform.GetChild(3).gameObject);
            SuaveDesactivarNotificacion(Botones[3].transform.GetChild(2).gameObject);
        }
    }

    void SuaveActivarNotificacion(GameObject notificacion)
    {
        StartCoroutine(ActivarNotificacionGradualmente(notificacion));
    }

    void SuaveDesactivarNotificacion(GameObject notificacion)
    {
        StartCoroutine(DesactivarNotificacionGradualmente(notificacion));
    }

    IEnumerator ActivarNotificacionGradualmente(GameObject notificacion)
    {
        yield return new WaitForSeconds(0.1f);  // Pequeño retraso para suavizar la activación
        notificacion.SetActive(true);
    }

    IEnumerator DesactivarNotificacionGradualmente(GameObject notificacion)
    {
        yield return new WaitForSeconds(0.1f);  // Pequeño retraso para suavizar la desactivación
        notificacion.SetActive(false);
    }
}
