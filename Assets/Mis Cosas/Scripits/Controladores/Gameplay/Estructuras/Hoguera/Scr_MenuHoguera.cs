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
    [SerializeField] Scr_ControladorInventario Inventario;

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
    Scr_ObjetoEnMano ObjetoEnMano;
    int ObjetoActual = 0;
    float TiempoProduciendo = 0;
    int cantidadAProducir = 0;
    int cantidadProducida = 0;
    bool EstaDentroBoton = false;

    void Start()
    {
        Carga = Barra.transform.GetChild(1).gameObject.GetComponent<Image>();
        ObjetoEnMano = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(1).GetComponent<Scr_ObjetoEnMano>();
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
        Image[] Materiales = new Image[4];
        Materiales[0] = CasillasMateriales[0].transform.GetChild(0).GetChild(0).GetComponent<Image>();
        Materiales[1] = CasillasMateriales[1].transform.GetChild(0).GetChild(0).GetComponent<Image>();
        Materiales[2] = CasillasMateriales[2].transform.GetChild(0).GetChild(0).GetComponent<Image>();
        Materiales[3] = CasillasMateriales[3].transform.GetChild(0).GetChild(0).GetComponent<Image>();

        TextMeshProUGUI[] Cantidades = new TextMeshProUGUI[4];
        Cantidades[0] = CasillasMateriales[0].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        Cantidades[1] = CasillasMateriales[1].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        Cantidades[2] = CasillasMateriales[2].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        Cantidades[3] = CasillasMateriales[3].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();


        int MaterialActual = 0;
        foreach (Image Material in Materiales)
        {
            //Total necesario para cada objeto
            int TotalNecesario = int.Parse(Cantidades[MaterialActual].text);


            //En caso de que este activo el material
            if (Material.isActiveAndEnabled)
            {
                int ItemActual = 0;
                foreach (string Objeto in Inventario.CasillasContenido)
                {
                    if (TotalNecesario > 0)
                    {
                        //Si encuentra el material
                        if (Objeto.Contains(Material.sprite.name))
                        {
                            //En caso de tener mas de los que piden
                            if (Inventario.Cantidades[ItemActual] > TotalNecesario)
                            {
                                foreach (Image Casilla in Inventario.Casillas[ItemActual].GetComponent<Scr_CasillaInventario>().CasillasHermanas)
                                {
                                    Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] -= TotalNecesario;
                                }
                                TotalNecesario = 0;
                            }
                            else
                            {
                                //En caso de tener menos
                                if (Inventario.Cantidades[ItemActual] < TotalNecesario)
                                {
                                    bool YaResto = false;
                                    foreach (Image Casilla in Inventario.Casillas[ItemActual].GetComponent<Scr_CasillaInventario>().CasillasHermanas)
                                    {
                                        if (!YaResto)
                                        {
                                            YaResto = true;
                                            TotalNecesario -= (int)Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero];
                                        }
                                        Inventario.CasillasContenido[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = "";
                                        Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = 0;
                                    }
                                }
                                else
                                {
                                    foreach (Image Casilla in Inventario.Casillas[ItemActual].GetComponent<Scr_CasillaInventario>().CasillasHermanas)
                                    {
                                        TotalNecesario = 0;
                                        Inventario.CasillasContenido[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = "";
                                        Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = 0;
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        break;
                    }

                    ItemActual++;
                }
            }
            MaterialActual++;
        }
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
        if (ObjetoEnMano.Nombre == ObjetosQueProduce[0].Nombre || ObjetoEnMano.Nombre == "")
        {
            ObjetoEnMano.Cantidad += int.Parse(ObjetoCreado.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text);
            ObjetoEnMano.Nombre = ObjetosQueProduce[0].Nombre;
            ObjetoEnMano.Forma = ObjetosQueProduce[0].Forma;
            ObjetoEnMano.Iconos = ObjetosQueProduce[0].IconosInventario;
            cantidadProducida -= int.Parse(ObjetoCreado.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text);
            ObjetoCreado.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = cantidadProducida.ToString();
            //GetComponent<Scr_Hoguera>().Salir();
        }
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
                            if (VerificarCantidades(Objeto, ObjetosQueProduce[0].CantidadMaterialesDeProduccion[Casilla]))
                            {
                                CasillasMateriales[Casilla].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = ColoresBotones[3];
                            }
                            else
                            {
                                CasillasMateriales[Casilla].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = ColoresBotones[5];

                            }
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

        ImagenTamaño.color = ObjetosQueProduce[ObjetoActual].ColorTamaño;

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


    private bool VerificarCantidades(Scr_CreadorObjetos Objeto, int minimo)
    {
        int TotalCantidad = 0;
        int ObjetoActual = 0;
        foreach (string Casilla in Inventario.CasillasContenido)
        {
            if (Casilla.Contains(Objeto.Nombre))
            {
                TotalCantidad += Inventario.Cantidades[ObjetoActual];
            }
            ObjetoActual++;
        }

        if (TotalCantidad / Objeto.Tamaño >= minimo)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int VerificarTotales(Scr_CreadorObjetos Objeto)
    {
        int TotalCantidad = 0;
        int ObjetoActual = 0;
        foreach (string Casilla in Inventario.CasillasContenido)
        {
            if (Casilla.Contains(Objeto.Nombre))
            {
                TotalCantidad += Inventario.Cantidades[ObjetoActual];
            }
            ObjetoActual++;
        }

        return TotalCantidad / Objeto.Tamaño;

    }

    public void BotonMitad()
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
                            Cantidades[Casilla] = VerificarTotales(Objeto) / ObjetosQueProduce[0].CantidadMaterialesDeProduccion[Casilla];
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
        if (cantidadAProducir == 0)
        {
            CambiarDialogos();
        }
        cantidadAProducir = (int)RedondearHaciaArribaCon0_5((float)CantMinima / 2);
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
                            Cantidades[Casilla] = VerificarTotales(Objeto) / ObjetosQueProduce[0].CantidadMaterialesDeProduccion[Casilla];
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
