using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_MenuHoguera : MonoBehaviour
{
    [Header("Datos Generales")]
    [SerializeField] GameObject Fuego;
    [SerializeField] Scr_CreadorObjetos[] ObjetosQueProduce;
    [SerializeField] string[] DialogosIniciales;
    [SerializeField] string[] DialogosMedios;
    [SerializeField] string[] DialogosFinales;
    [SerializeField] Color[] ColoresBotones;

    [Header("Objetos del menu")]
    [SerializeField] TextMeshProUGUI TextoNombre;
    [SerializeField] TextMeshProUGUI TextoDescripcion;
    [SerializeField] Image ImagenTamaño;
    [SerializeField] TextMeshProUGUI TextoSegundos;
    [SerializeField] GameObject[] CasillasMateriales;
    [SerializeField] TextMeshProUGUI TextoCantidad;
    [SerializeField] TextMeshProUGUI DialogoCarga;
    [SerializeField] Image Carga;
    [SerializeField] GameObject ObjetoCreado;

    int ObjetoActual = 0;
    float TiempoProduciendo = 0;
    int cantidadAProducir = 0;
    int cantidadProducida = 0;
    bool EstaDentroBoton = false;

    void Start()
    {

    }

    void Update()
    {
        if (cantidadAProducir > 0)
        {
            Fuego.SetActive(true);
        }
        else
        {
            Fuego.SetActive(false);
        }

        ActualizarDatos();

    }

    void ActualizarDatos()
    {
        switch (ObjetoActual)
        {
            case 0:
                {
                    TextoNombre.text = ObjetosQueProduce[0].Nombre;
                    TextoDescripcion.text = ObjetosQueProduce[0].Descripcion;
                    ImagenTamaño.sprite = ObjetosQueProduce[0].IconoTamaño;
                    TextoSegundos.text = ObjetosQueProduce[0].TiempoDeProduccion + "s";
                    int Casilla = 0;
                    foreach (Scr_CreadorObjetos Objeto in ObjetosQueProduce[0].MaterialesDeProduccion)
                    {
                        if (Objeto != null)
                        {
                            CasillasMateriales[Casilla].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                            CasillasMateriales[Casilla].transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                            CasillasMateriales[Casilla].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Objeto.Icono;
                            CasillasMateriales[Casilla].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = ObjetosQueProduce[0].CantidadMaterialesDeProduccion[Casilla].ToString();
                        }
                        else
                        {
                            CasillasMateriales[Casilla].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                            CasillasMateriales[Casilla].transform.GetChild(1).GetChild(0).gameObject.SetActive(false);

                        }

                        Casilla++;
                    }
                    TextoCantidad.text = cantidadAProducir.ToString();

                    break;
                }
        }

        if (int.Parse(ObjetoCreado.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text) > 0)
        {
            ObjetoCreado.SetActive(true);
        }
        else
        {
            ObjetoCreado.SetActive(false);
        }
    }

    public void Flechas(bool Aumenta)
    {
        if (Aumenta)
        {
            if (cantidadAProducir + cantidadProducida < 99)
            {
                cantidadAProducir++;
            }
        }
        else
        {
            if (cantidadAProducir > 0)
            {
                cantidadAProducir--;
            }
        }
    }


    public void EntraEnItem(int Objeto)
    {
        if (!EstaDentroBoton)
        {
            EstaDentroBoton = true;
            transform.GetChild(2).GetChild(0).GetChild(Objeto).GetComponent<Image>().color = ColoresBotones[0];
            ObjetoActual = Objeto;

        }
    }

    public void SaleItem()
    {
        if (EstaDentroBoton)
        {
            EstaDentroBoton = false;
            transform.GetChild(2).GetChild(0).GetChild(ObjetoActual).GetComponent<Image>().color = ColoresBotones[1];

        }
    }

}
