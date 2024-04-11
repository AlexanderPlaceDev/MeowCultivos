using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Scr_MenuHoguera : MonoBehaviour
{
    [Header("Datos Generales")]
    [SerializeField] GameObject Fuego;
    [SerializeField] float velocidadRotacionFuego;
    [SerializeField] Scr_CreadorObjetos[] ObjetosQueProduce;
    [SerializeField] string[] DialogosIniciales;
    [SerializeField] string[] DialogosMedios;
    [SerializeField] string[] DialogosFinales;
    [SerializeField] Color[] ColoresBotones;
    [SerializeField] GameObject[] Botones;

    [Header("Objetos del menu")]
    [SerializeField] TextMeshProUGUI TextoNombre;
    [SerializeField] TextMeshProUGUI TextoDescripcion;
    [SerializeField] Image ImagenTamaño;
    [SerializeField] TextMeshProUGUI TextoSegundos;
    [SerializeField] GameObject[] CasillasMateriales;
    [SerializeField] TextMeshProUGUI TextoCantidad;
    [SerializeField] TextMeshProUGUI DialogoCarga;
    [SerializeField] GameObject Barra;
    [SerializeField] float DuracionBarra;
    [SerializeField] GameObject ObjetoCreado;


    Image Carga;
    int ObjetoActual = 0;
    float TiempoProduciendo = 0;
    int cantidadAProducir = 0;
    int cantidadProducida = 0;
    bool EstaDentroBoton = false;

    void Start()
    {
        Carga = Barra.transform.GetChild(1).gameObject.GetComponent<Image>();
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
        Producir();

    }

    private void Producir()
    {
        if (cantidadAProducir > 0)
        {
            TiempoProduciendo += Time.deltaTime;
        }
        else
        {
            TiempoProduciendo = 0;
        }
        Carga.fillAmount = TiempoProduciendo / ObjetosQueProduce[ObjetoActual].TiempoDeProduccion;
        Carga.color = Color.Lerp(ColoresBotones[5], ColoresBotones[3], Carga.fillAmount);
        if (Carga.fillAmount >= 1)
        {
            CambiarDialogos();
            QuitarObjetos();
            GenerarObjeto();
            cantidadAProducir--;
            TiempoProduciendo = 0;
        }

    }

    private void CambiarDialogos()
    {
        DialogoCarga.text = DialogosIniciales[Random.Range(0, DialogosIniciales.Length)] + " " + DialogosMedios[Random.Range(0, DialogosMedios.Length)] + " " + DialogosFinales[Random.Range(0, DialogosFinales.Length)];
    }

    private void QuitarObjetos()
    {
        
    }

    private void GenerarObjeto()
    {
        switch (ObjetoActual)
        {
            case 0:
                {
                    cantidadProducida++;
                    ObjetoCreado.transform.GetChild(0).GetComponent<Image>().sprite = ObjetosQueProduce[0].Icono;
                    ObjetoCreado.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = cantidadProducida.ToString();
                    break;
                }
        }
    }
    public void DarItem()
    {
        
    }

    void ActualizarDatos()
    {
        switch (ObjetoActual)
        {
            case 0:
                {
                    TextoNombre.text = ObjetosQueProduce[0].Nombre;
                    TextoDescripcion.text = ObjetosQueProduce[0].Descripcion;
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


        if (cantidadAProducir > 0 && Barra.GetComponent<RectTransform>().anchoredPosition.y == -670)
        {
            Tween.UIAnchoredPosition3DY(Barra.GetComponent<RectTransform>(), -450, DuracionBarra);
        }
        if (cantidadAProducir == 0 && Barra.GetComponent<RectTransform>().anchoredPosition.y == -450)
        {
            Tween.UIAnchoredPosition3DY(Barra.GetComponent<RectTransform>(), -670, DuracionBarra);
        }

        if (cantidadAProducir > 0)
        {
            Fuego.transform.Rotate(Vector3.up, velocidadRotacionFuego * Time.deltaTime);
        }

    }

    public void Flechas(bool Aumenta)
    {
        if (Aumenta)
        {
            if (cantidadAProducir + cantidadProducida < 99)
            {
                if (cantidadAProducir == 0)
                {
                    CambiarDialogos();
                }
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

    public void EntraBoton(string ColorYNumero)
    {
        if (ColorYNumero == "01")
        {
            Botones[0].GetComponent<Image>().color = ColoresBotones[2];
        }
        if (ColorYNumero == "02")
        {
            Botones[1].GetComponent<Image>().color = ColoresBotones[2];
        }
        if (ColorYNumero == "13")
        {
            Botones[2].GetComponent<Image>().color = ColoresBotones[4];
        }
        if (ColorYNumero == "24")
        {
            Botones[3].GetComponent<Image>().color = ColoresBotones[6];
        }
        if (ColorYNumero == "25")
        {
            Botones[4].GetComponent<Image>().color = ColoresBotones[6];
        }
    }

    public void SaleBoton(string ColorYNumero)
    {
        if (ColorYNumero == "01")
        {
            Botones[0].GetComponent<Image>().color = ColoresBotones[3];
        }
        if (ColorYNumero == "02")
        {
            Botones[1].GetComponent<Image>().color = ColoresBotones[3];
        }
        if (ColorYNumero == "13")
        {
            Botones[2].GetComponent<Image>().color = ColoresBotones[5];
        }
        if (ColorYNumero == "24")
        {
            Botones[3].GetComponent<Image>().color = ColoresBotones[7];
        }
        if (ColorYNumero == "25")
        {
            Botones[4].GetComponent<Image>().color = ColoresBotones[7];
        }
    }



    public void BotonMitad()
    {
        
    }

    public void BotonMax()
    {
        if (cantidadAProducir == 0)
        {
            CambiarDialogos();
        }
        cantidadAProducir = ObtenerCantidadMinima();
        if (cantidadProducida + cantidadAProducir >= 99)
        {
            cantidadAProducir = 99 - cantidadProducida;
        }
    }

    public void BotonBorrar()
    {
        cantidadAProducir = 0;
    }

    float RedondearHaciaArribaCon0_5(float numero)
    {
        float redondeado = Mathf.Round(numero);

        // Verificar si los decimales son 0.5
        if ((numero - redondeado) == 0.5f)
        {
            // Ajustar hacia arriba
            redondeado += numero + 0.5f;
        }
        return redondeado;
    }

    int ObtenerCantidadMinima()
    {
        int[] Cantidades = new int[4];
        int CantidadMateriales = 0;
        switch (ObjetoActual)
        {
            case 0:
                {
                    int Casilla = 0;
                    foreach (Scr_CreadorObjetos Objeto in ObjetosQueProduce[0].MaterialesDeProduccion)
                    {
                        if (Objeto != null)
                        {
                            CantidadMateriales++;
                        }
                        else
                        {
                            Cantidades[Casilla] = 0;
                        }
                        Casilla++;
                    }

                    break;
                }
        }

        int CantMinima = Cantidades[0];

        int i = 0;
        foreach (int Cantidad in Cantidades)
        {
            if (i >= CantidadMateriales)
            {
                break;
            }
            if (Cantidad < CantMinima)
            {

                CantMinima = Cantidad;

            }
            i++;
        }

        return CantMinima;
    }
}
