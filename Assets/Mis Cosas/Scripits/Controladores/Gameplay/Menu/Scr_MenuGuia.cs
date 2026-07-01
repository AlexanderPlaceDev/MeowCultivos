using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_MenuGuia : MonoBehaviour
{
    public int MenuActual = 0;
    bool MenuActualizado = false;

    GameObject MenuGuia;
    GameObject Area;

    [SerializeField] GameObject[] ObjetosMenu1;
    [SerializeField] GameObject[] ObjetosMenu2;
    [SerializeField] GameObject[] ObjetosMenu3;
    [SerializeField] GameObject[] ObjetosMenu4;
    [SerializeField] GameObject[] ObjetosMenu5;
    [SerializeField] GameObject[] ObjetosMenu6;

    [SerializeField] float Tamaños;

    [SerializeField] Scrollbar Scroll;

    [SerializeField] float AlturaVisible = 1000f;

    [SerializeField] GameObject IconoSeleccion;

    [SerializeField]
    TextMeshProUGUI[] TextosMenus;

    [SerializeField]
    Color[] ColoresTextos;

    VerticalLayoutGroup layout;

    void Start()
    {
        MenuGuia = GameObject.Find("Canvas").transform.GetChild(3).GetChild(11).gameObject;
        Area = MenuGuia.transform.GetChild(1).GetChild(1).gameObject;

        layout = Area.GetComponent<VerticalLayoutGroup>();

        Scroll.onValueChanged.AddListener(ActualizarScroll);
    }

    void Update()
    {
        if (!MenuGuia.activeSelf)
            return;

        if (!MenuActualizado)
        {
            layout.padding.top = 0;
            Scroll.value = 0;

            switch (MenuActual)
            {
                case 0:
                    IconoSeleccion.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, 400);
                    CargarMenu(ObjetosMenu1);
                    break;

                case 1:
                    IconoSeleccion.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, 250);
                    CargarMenu(ObjetosMenu2);
                    break;

                case 2:
                    IconoSeleccion.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, 100);
                    CargarMenu(ObjetosMenu3);
                    break;

                case 3:
                    IconoSeleccion.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, -50);
                    CargarMenu(ObjetosMenu4);
                    break;

                case 4:
                    IconoSeleccion.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, -200);
                    CargarMenu(ObjetosMenu5);
                    break;

                case 5:
                    IconoSeleccion.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, -350);
                    CargarMenu(ObjetosMenu6);
                    break;
            }

            ActualizarColoresMenus();

            MenuActualizado = true;
        }
    }

    void CargarMenu(GameObject[] objetos)
    {
        //Eliminar hijos
        foreach (Transform hijo in Area.transform)
        {
            DestroyImmediate(hijo.gameObject);
        }

        //Instanciar nuevos objetos
        foreach (GameObject obj in objetos)
        {
            Instantiate(obj, Area.transform);
        }

        //Forzar actualización del layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(Area.GetComponent<RectTransform>());

        //Calcular altura total
        Tamaños = 0;

        for (int i = 0; i < Area.transform.childCount; i++)
        {
            RectTransform rt = Area.transform.GetChild(i).GetComponent<RectTransform>();
            Tamaños += rt.rect.height;
        }

        if (Area.transform.childCount > 1)
        {
            Tamaños += layout.spacing * (Area.transform.childCount - 1);
        }

        //Ajustar tamaño del handle
        Scroll.size = Mathf.Clamp01(AlturaVisible / Tamaños);

        //Actualizar posición
        ActualizarScroll(0);
    }

    void ActualizarScroll(float value)
    {
        float maxTop = Mathf.Max(0, Tamaños - AlturaVisible);

        RectOffset padding = layout.padding;
        padding.top = -Mathf.RoundToInt(value * maxTop);
        layout.padding = padding;

        LayoutRebuilder.ForceRebuildLayoutImmediate(Area.GetComponent<RectTransform>());
    }

    void ActualizarColoresMenus()
    {
        for (int i = 0; i < TextosMenus.Length; i++)
        {
            if (i == MenuActual)
            {
                TextosMenus[i].color = ColoresTextos[0];
            }
            else
            {
                TextosMenus[i].color = ColoresTextos[1];
            }
        }
    }
}