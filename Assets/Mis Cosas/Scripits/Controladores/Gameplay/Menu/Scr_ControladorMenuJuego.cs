using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using UnityEngine.UI;
using Unity.Mathematics;

public class Scr_ControladorMenuJuego : MonoBehaviour
{

    public bool EstaEnMenu = false;
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject BotonesMenu;
    [SerializeField] Transform BarraIzq;
    [SerializeField] Transform BarraDer;
    [SerializeField] Transform Fondo;
    [SerializeField] Transform AreaDia;
    [SerializeField] Scr_CreadorTemas TemaActual;
    [SerializeField] float Duracion;
    float ContTiempo = 0;
    Tween tweenAbrir;
    Tween tweenCerrar;
    Tween tweenCambiarColor;
    GameObject Gata;
    bool Va = false;
    int MenuActual = 0;




    void Start()
    {
        Gata = GameObject.Find("Gata");
    }

    void Update()
    {

        if (EstaEnMenu)
        {
            Gata.GetComponent<Scr_Movimiento>().enabled = false;
            Gata.GetComponent<Scr_GiroGata>().enabled = false;
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Cerrar();
            }
            if (tweenCerrar.progress >= 0.9 && MenuActual == 0)
            {
                EstaEnMenu = false;
                CambiarMenu();
            }
        }
        else
        {
            Gata.GetComponent<Scr_Movimiento>().enabled = true;
            Gata.GetComponent<Scr_GiroGata>().enabled = true;
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                EstaEnMenu = true;
                CambiarMenu();
                Abrir();
            }
        }

        if (MenuActual == 2)
        {
            CambiarAInventario();
        }

    }


    private void CambiarMenu()
    {
        if (EstaEnMenu)
        {
            Menu.SetActive(true);

        }
        else
        {

            Menu.SetActive(false);
        }
    }

    void Abrir()
    {
        if (!tweenAbrir.IsAlive && BarraIzq.GetComponent<RectTransform>().anchoredPosition.x != -850)
        {
            tweenAbrir = Tween.UIAnchoredPositionX(BarraIzq.GetComponent<RectTransform>(), -850, Duracion, Ease.Default, 1);
            Tween.UIAnchoredPosition3DX(BarraDer.GetComponent<RectTransform>(), 850, Duracion, Ease.Default, 1);

        }
    }

    void Cerrar()
    {
        if (!tweenCerrar.IsAlive && BarraIzq.GetComponent<RectTransform>().anchoredPosition.x != -260)
        {
            tweenCerrar = Tween.UIAnchoredPositionX(BarraIzq.GetComponent<RectTransform>(), -260, Duracion, Ease.Default, 1);
            Tween.UIAnchoredPosition3DX(BarraDer.GetComponent<RectTransform>(), 245, Duracion, Ease.Default, 1);

        }
    }

    public void CambiarMenu(int NumeroMenu, bool va)
    {
        MenuActual = NumeroMenu;
        Va = va;
        Cerrar();
    }

    public void CambiarAInventario()
    {
        if (tweenCerrar.progress == 0)
        {
            if (Va)
            {
                if (ContTiempo < Duracion)
                {
                    ContTiempo += Time.deltaTime;
                }

            }
            else
            {
                if (ContTiempo > 0)
                {
                    ContTiempo -= Time.deltaTime;
                }
            }
            BarraIzq.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.BarrasMenu1, TemaActual.BarrasInventario1, ContTiempo);
            BarraIzq.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.BarrasMenu2, TemaActual.BarrasInventario2, ContTiempo);
            BarraDer.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.BarrasMenu1, TemaActual.BarrasInventario1, ContTiempo);
            BarraDer.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.BarrasMenu2, TemaActual.BarrasInventario2, ContTiempo);
            Fondo.GetComponent<Image>().color = Color.Lerp(TemaActual.FondoMenu, TemaActual.FondoInventario, ContTiempo);
            AreaDia.GetComponent<Image>().color = Color.Lerp(TemaActual.AreaDiaMenu, TemaActual.AreaDiaInventario, ContTiempo);
        }

        if (ContTiempo >= 1)
        {
            BotonesMenu.SetActive(false);
            BarraIzq.GetChild(1).gameObject.SetActive(true);
            BarraDer.GetChild(1).gameObject.SetActive(true);
            Abrir();
        }
    }
}
