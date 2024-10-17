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
            BotonActual = 5;
        }
        else if (BotonActual > 5)
        {
            BotonActual = 0;
        }

        ActualiarNotificacion();
    }

    public void PrenderBoton(string BotonySi)
    {
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
        else
        {
            Botones[4].GetComponent<Image>().color = Colores[1];
            Botones[5].GetComponent<Image>().color = Colores[1];
            Botones[0].transform.GetChild(0).GetComponent<Image>().color = Colores[1];
            Botones[0].transform.GetChild(1).GetComponent<Image>().color = Colores[1];
            Botones[1].transform.GetChild(0).GetComponent<Image>().color = Colores[1];
            Botones[1].transform.GetChild(1).GetComponent<Image>().color = Colores[1];
            Botones[2].transform.GetChild(0).GetComponent<Image>().color = Colores[1];
            Botones[2].transform.GetChild(1).GetComponent<Image>().color = Colores[1];
            Botones[3].transform.GetChild(0).GetComponent<Image>().color = Colores[1];
            Botones[3].transform.GetChild(1).GetComponent<Image>().color = Colores[1];
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
                    Asistente.GetComponent<Animator>().Play("Salir");
                    break;
                case 3:
                    Asistente.GetComponent<Animator>().Play("Opciones");
                    break;
                case 4:
                    Asistente.GetComponent<Animator>().Play("Armas");
                    break;
                case 5:
                    Asistente.GetComponent<Animator>().Play("Guia");
                    break;
            }
            StartCoroutine(Esperar());
        }
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
                GameObject.Find("Gata").transform.GetChild(5).GetComponent<Scr_CambiadorMenus>().MenuActual = "Inventario";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Inventario";
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[1];
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[0];
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[5];
                break;
            case 1:
                Asistente.GetComponent<Animator>().Play("Habilidades");
                GameObject.Find("Gata").transform.GetChild(5).GetComponent<Scr_CambiadorMenus>().MenuActual = "Habilidades";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Habilidades";
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[2];
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[1];
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[0];
                break;
            case 2:
                Asistente.GetComponent<Animator>().Play("Salir");
                GameObject.Find("Gata").transform.GetChild(5).GetComponent<Scr_CambiadorMenus>().MenuActual = "Salir";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Salir";
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[3];
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[2];
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[1];
                break;
            case 3:
                Asistente.GetComponent<Animator>().Play("Opciones");
                GameObject.Find("Gata").transform.GetChild(5).GetComponent<Scr_CambiadorMenus>().MenuActual = "Opciones";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Opciones";
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[4];
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[3];
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[2];
                break;
            case 4:
                Asistente.GetComponent<Animator>().Play("Armas");
                GameObject.Find("Gata").transform.GetChild(5).GetComponent<Scr_CambiadorMenus>().MenuActual = "Armas";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Armas";
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[5];
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[4];
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[3];
                break;
            case 5:
                Asistente.GetComponent<Animator>().Play("Guia");
                GameObject.Find("Gata").transform.GetChild(5).GetComponent<Scr_CambiadorMenus>().MenuActual = "Guia";
                Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Guia";
                Botones[1].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[0];
                Botones[2].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[5];
                Botones[3].transform.GetChild(1).GetComponent<Image>().sprite = Iconos[4];
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
        if (PlayerPrefs.GetInt("PuntosDeHabilidad", 0) > 0)
        {
            Botones[1].transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("PuntosDeHabilidad", 0).ToString();
            Botones[2].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("PuntosDeHabilidad", 0).ToString();
            Botones[3].transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("PuntosDeHabilidad", 0).ToString();
            switch (BotonActual)
            {
                case 0:
                    SuaveActivarNotificacion(Botones[1].transform.GetChild(2).gameObject);
                    SuaveDesactivarNotificacion(Botones[2].transform.GetChild(3).gameObject);
                    SuaveDesactivarNotificacion(Botones[3].transform.GetChild(2).gameObject);
                    break;
                case 1:
                    SuaveDesactivarNotificacion(Botones[1].transform.GetChild(2).gameObject);
                    SuaveActivarNotificacion(Botones[2].transform.GetChild(3).gameObject);
                    SuaveDesactivarNotificacion(Botones[3].transform.GetChild(2).gameObject);
                    break;
                case 2:
                    SuaveDesactivarNotificacion(Botones[1].transform.GetChild(2).gameObject);
                    SuaveDesactivarNotificacion(Botones[2].transform.GetChild(3).gameObject);
                    SuaveActivarNotificacion(Botones[3].transform.GetChild(2).gameObject);
                    break;
                default:
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
