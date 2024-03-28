using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

public class Scr_ControladorMenuJuego : MonoBehaviour
{
    [Header("Datos Generales")]
    public bool EstaEnMenu = false;
    public bool EstaEnMenuPrincipal = true;
    [SerializeField] Color[] ColoresActuales;

    [Header("Datos del menu")]
    [SerializeField] GameObject Menu;
    [SerializeField] Image[] ObjetosUI;
    [SerializeField] Transform[] ObjetosDelMenu;
    [SerializeField] Scr_CreadorTemas TemaActual;

    [Header("Datos del inventario")]
    [SerializeField] Transform[] ObjetosDelInventario;



    GameObject Gata;
    public int MenuActual = 0;
    public float ContAnimacion = 0;
    public bool Cerrando = false;
    public bool Esperando = false;




    void Start()
    {
        Gata = GameObject.Find("Gata");
    }

    void Update()
    {
        //Para Abrir y cerrar el menu
        if (EstaEnMenu)
        {
            Gata.GetComponent<Scr_Movimiento>().enabled = false;
            Gata.GetComponent<Scr_GiroGata>().enabled = false;


            if (Input.GetKeyDown(KeyCode.Tab) && !Esperando)
            {
                if (MenuActual == 0)
                {
                    Cerrando = true;
                    Cerrar();
                }
            }

            if (Cerrando)
            {
                ContAnimacion += Time.deltaTime;
            }

            if (Esperando)
            {
                ContAnimacion += Time.deltaTime;
                CambiarColores();
            }

            CambiarMenus();
        }
        else
        {
            Gata.GetComponent<Scr_Movimiento>().enabled = true;
            Gata.GetComponent<Scr_GiroGata>().enabled = true;


            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Menu.SetActive(true);
                Abrir();
                EstaEnMenu = true;
            }
        }
    }

    void Abrir()
    {
        Menu.GetComponent<Animator>().Play("Abrir");
    }

    void Cerrar()
    {
        Menu.GetComponent<Animator>().Play("Cerrar");

    }

    public void BotonInventario()
    {
        MenuActual = 2;
        Cerrando = true;
        EstaEnMenuPrincipal = false;
        Cerrar();

    }

    public void BotonRegresar()
    {
        MenuActual = 0;
        Cerrando = true;
        Cerrar();
    }

    void CambiarMenus()
    {
        if (ContAnimacion >= 1 && MenuActual == 0)
        {
            ContAnimacion = 0;
            Cerrando = false;
            if (EstaEnMenuPrincipal)
            {
                StopAllCoroutines();
                Menu.SetActive(false);
                EstaEnMenu = false;
            }
            else
            {
                foreach (Transform objeto in ObjetosDelInventario)
                {
                    objeto.gameObject.SetActive(false);
                }
                foreach (Transform objeto in ObjetosDelMenu)
                {
                    objeto.gameObject.SetActive(true);
                }

                ObjetosUI[4].color = TemaActual.ColoresMenu[4];
                ObjetosUI[5].color = TemaActual.ColoresMenu[5];

                Debug.Log("Entra");
                if (!Esperando)
                {
                    Esperando = true;
                    Debug.Log("Entra2");
                    StartCoroutine(Esperar(1));
                }
            }


        }

        if (ContAnimacion >= 1 && MenuActual == 2)
        {
            Cerrando = false;
            ContAnimacion = 0;

            foreach (Transform objeto in ObjetosDelInventario)
            {
                objeto.gameObject.SetActive(true);
            }
            foreach (Transform objeto in ObjetosDelMenu)
            {
                objeto.gameObject.SetActive(false);
            }
            ObjetosUI[4].color = TemaActual.ColoresInventario[4];
            ObjetosUI[5].color = TemaActual.ColoresInventario[5];

            if (!Esperando)
            {
                Esperando = true;
                Debug.Log("Entra1");
                StartCoroutine(Esperar(1));
            }
        }
    }

    void CambiarColores()
    {

        switch (MenuActual)
        {
            case 0:
                {
                    for (int i = 0; i < ObjetosUI.Length - 2; i++)
                    {
                        ObjetosUI[i].color = Color.Lerp(ColoresActuales[i], TemaActual.ColoresMenu[i], ContAnimacion);
                    }
                    break;
                }
            case 2:
                {
                    for (int i = 0; i < ObjetosUI.Length - 2; i++)
                    {
                        ObjetosUI[i].color = Color.Lerp(ColoresActuales[i], TemaActual.ColoresInventario[i], ContAnimacion);
                    }
                    break;
                }
        }

    }

    IEnumerator Esperar(float Tiempo)
    {
        yield return new WaitForSeconds(Tiempo);

        switch (MenuActual)
        {
            case 0 :
                {
                    EstaEnMenuPrincipal = true;
                    ColoresActuales = TemaActual.ColoresMenu;
                    break;
                }

            case 2:
                {
                    ColoresActuales = TemaActual.ColoresInventario;
                    break;
                }
        }

        Esperando = false;
        ContAnimacion = 0;
        Abrir();
    }
}
