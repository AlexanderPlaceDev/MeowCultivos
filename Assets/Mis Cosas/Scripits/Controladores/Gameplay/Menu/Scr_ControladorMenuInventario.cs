using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorMenuInventario : MonoBehaviour
{
    [SerializeField] Transform[] ObjetosDelInventario;
    [SerializeField] float VelocidadSlider;
    float ValorSlider = 0;
    bool InventarioActualizado;

    GameObject Gata;
    void Start()
    {
        Gata = GameObject.Find("Gata");
    }

    private void Update()
    {
        if (ObjetosDelInventario[0].gameObject.activeSelf)
        {
            ScrollBar();
            ActualizarInventario();

        }
        else
        {
            InventarioActualizado = false;
        }
    }

    void ScrollBar()
    {
        int casillasActivas = 0;
        foreach (Transform casilla in ObjetosDelInventario[0].GetChild(0).GetComponentInChildren<Transform>())
        {
            if (casilla.gameObject.activeSelf)
                casillasActivas++;
        }

        float cantFilas = Mathf.Ceil(((float)casillasActivas / 3) - 1);

        ObjetosDelInventario[0].GetChild(1).GetComponent<Scrollbar>().size = cantFilas < 2 ? 1 : 1 / cantFilas;

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
        float nuevoTop = Mathf.Lerp(topActual, ValorSlider * -300 * (cantFilas - 1), Time.deltaTime * VelocidadSlider); ;
        grid.padding = new RectOffset(grid.padding.left, grid.padding.right, Mathf.RoundToInt(nuevoTop), grid.padding.bottom);
    }

    void ActualizarInventario()
    {
        if (!InventarioActualizado)
        {
            int i = 0;
            foreach (int cantidad in Gata.transform.GetChild(6).GetComponent<Scr_Inventario>().Cantidades)
            {
                Transform casilla = ObjetosDelInventario[0].GetChild(0).GetChild(i);
                casilla.gameObject.SetActive(cantidad != 0);

                if (cantidad != 0)
                    casilla.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = cantidad.ToString();

                i++;
            }
            InventarioActualizado = true;
        }
    }
    public void ActualizarDescripcionInventario(string numeroyEntrada)
    {
        int numeroDeObjeto = int.Parse(numeroyEntrada.Split('-')[0]);
        Transform descripcion = ObjetosDelInventario[0].GetChild(2);

        if (numeroyEntrada.EndsWith("0"))
            descripcion.gameObject.SetActive(false);
        else
        {
            descripcion.gameObject.SetActive(true);
            descripcion.GetChild(0).GetComponent<TextMeshProUGUI>().text = Gata.transform.GetChild(6).GetComponent<Scr_Inventario>().Objetos[numeroDeObjeto].Nombre;
            descripcion.GetChild(1).GetComponent<TextMeshProUGUI>().text = Gata.transform.GetChild(6).GetComponent<Scr_Inventario>().Objetos[numeroDeObjeto].Descripcion;
        }
    }
}
