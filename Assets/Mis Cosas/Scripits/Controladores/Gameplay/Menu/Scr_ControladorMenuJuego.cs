using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using Unity.VisualScripting;
using TMPro;

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
    [SerializeField] float VelocidadSlider;
    float ValorSlider = 0;
    bool InventarioActualizado;


    GameObject Gata;
    int MenuActual = 0;
    float ContAnimacion = 0;
    bool Cerrando = false;
    bool Esperando = false;




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
                    Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
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


            if (MenuActual == 2)
            {
                ScrollBar();
                ActualizarInventario();

            }

            CambiarMenus();


        }
        else
        {

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
                InventarioActualizado = false;
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

                if (!Esperando)
                {
                    Esperando = true;
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
            case 0:
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

    void ScrollBar()
    {
        int CasillasActivas = 0;
        foreach (Transform Casilla in ObjetosDelInventario[0].GetChild(0).GetComponentInChildren<Transform>())
        {
            if (Casilla.gameObject.activeSelf)
            {
                CasillasActivas++;
            }
        }

        float CantFilas = Mathf.Ceil(((float)CasillasActivas / 3) - 1);
        Debug.Log(CantFilas);
        if (CantFilas < 2)
        {
            ObjetosDelInventario[0].GetChild(1).GetComponent<Scrollbar>().size = 1;
        }
        else
        {
            ObjetosDelInventario[0].GetChild(1).GetComponent<Scrollbar>().size = 1 / CantFilas;

        }

        float scrollDelta = -Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            ValorSlider = Mathf.Clamp01(ValorSlider + scrollDelta);
            ObjetosDelInventario[0].GetChild(1).GetComponent<Scrollbar>().value = ValorSlider;
        }
        else
        {
            ValorSlider = ObjetosDelInventario[0].GetChild(1).GetComponent<Scrollbar>().value;
        }



        GridLayoutGroup grid = ObjetosDelInventario[0].GetChild(0).GetComponent<GridLayoutGroup>();

        float topActual = grid.padding.top;

        // Utilizamos Mathf.Lerp para interpolar suavemente hacia el valor de la variable
        float nuevoTop = Mathf.Lerp(topActual, ValorSlider * -300 * (CantFilas - 1), Time.deltaTime * VelocidadSlider); ;

        // Creamos un nuevo 'RectOffset' con el nuevo valor de 'top' y lo asignamos al GridLayoutGroup
        grid.padding = new RectOffset(grid.padding.left, grid.padding.right, Mathf.RoundToInt(nuevoTop), grid.padding.bottom);


    }

    void ActualizarInventario()
    {

        if (!InventarioActualizado)
        {
            int i = 0;
            foreach (int cantidad in Gata.transform.GetChild(6).GetComponent<Scr_Inventario>().Cantidades)
            {
                if (cantidad != 0)
                {
                    ObjetosDelInventario[0].GetChild(0).GetChild(i).gameObject.SetActive(true);
                    ObjetosDelInventario[0].GetChild(0).GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = cantidad.ToString();


                }
                else
                {
                    ObjetosDelInventario[0].GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
                i++;
            }
            InventarioActualizado = true;
        }
    }

    public void ActualizarDescripcionInventario(string NumeroyEntrada)
    {
        int NumeroDeObjeto = int.Parse(NumeroyEntrada.Split('-')[0]);
        if (NumeroyEntrada[NumeroyEntrada.Length-1].ToString() == 0.ToString())
        {
            ObjetosDelInventario[0].GetChild(2).gameObject.SetActive(false);

        }
        else
        {
            ObjetosDelInventario[0].GetChild(2).gameObject.SetActive(true);
            ObjetosDelInventario[0].GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = Gata.transform.GetChild(6).GetComponent<Scr_Inventario>().Objetos[NumeroDeObjeto].Nombre;
            ObjetosDelInventario[0].GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = Gata.transform.GetChild(6).GetComponent<Scr_Inventario>().Objetos[NumeroDeObjeto].Descripcion;
        }

    }
}
