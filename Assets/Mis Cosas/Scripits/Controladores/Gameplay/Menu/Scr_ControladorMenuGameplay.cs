using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorMenuGameplay : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    [SerializeField] float TiempoTransicion;
    [SerializeField] Scr_CreadorTemas TemaActual;
    [SerializeField] Image Fondo;
    [SerializeField] Image AreaHora;
    [SerializeField] GameObject BarraIzquierda;
    [SerializeField] GameObject BarraDerecha;
    bool Esperando = false;
    bool EstaEnMenu = false;
    float TiempoDeEspera = 0;
    GameObject Gata;
    void Start()
    {
        // Busca y guarda una referencia al objeto de la gata
        Gata = GameObject.Find("Gata");
    }

    void Update()
    {

        if (EstaEnMenu)
        {
            // Desactiva los componentes de movimiento de la gata mientras está en el menú
            Gata.GetComponent<Scr_Movimiento>().enabled = false;
            Gata.GetComponent<Scr_GiroGata>().enabled = false;

            if (Input.GetKeyDown(KeyCode.Tab) && !Esperando)
            {
                Debug.Log("Entra");

                if (GetComponent<Scr_CambiadorMenus>().MenuActual == "Menu")
                {
                    Esperando = true;
                    Menu.GetComponent<Animator>().Play("Cerrar");
                }
                else
                {
                    GetComponent<Scr_CambiadorMenus>().BotonRegresar();
                }

            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !Esperando)
            {
                Esperando = true;
                RestablecerColor();
                Menu.SetActive(true);
                Menu.GetComponent<Animator>().Play("Aparecer");
            }
        }

        if (Esperando)
        {
            TiempoDeEspera += Time.deltaTime;
        }

        if (TiempoDeEspera >= TiempoTransicion)
        {
            TiempoDeEspera = 0;
            Esperando = false;
            if (EstaEnMenu)
            {
                Menu.SetActive(false);
                EstaEnMenu = false;
            }
            else
            {
                EstaEnMenu = true;

            }
        }
    }

    void RestablecerColor()
    {
        Fondo.color = TemaActual.ColoresMenu[2];
        AreaHora.color = TemaActual.ColoresMenu[3];

        BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = TemaActual.ColoresMenu[0];
        BarraIzquierda.transform.GetChild(3).GetComponent<Image>().color = TemaActual.ColoresMenu[1];

        BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = TemaActual.ColoresMenu[0];
        BarraDerecha.transform.GetChild(3).GetComponent<Image>().color = TemaActual.ColoresMenu[1];
    }
}
