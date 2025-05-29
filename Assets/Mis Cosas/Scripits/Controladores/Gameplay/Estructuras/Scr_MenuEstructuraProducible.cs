using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Unity.VisualScripting;

public class Scr_MenuEstructuraProducible : MonoBehaviour
{
    [Header("Datos Generales")]
    [SerializeField] private GameObject ObjetoEspecial;
    [SerializeField] private float velocidadRotacionFuego;
    [SerializeField] private Scr_CreadorObjetos[] ObjetosQueProduce;
    [SerializeField] private GameObject ObjetosInfo;
    [SerializeField] private string[] DialogosIniciales;
    [SerializeField] private string[] DialogosMedios;
    [SerializeField] private string[] DialogosFinales;
    [SerializeField] private Color[] ColoresBotones;
    [SerializeField] private GameObject[] Botones;

    [Header("Objetos del menú")]
    [SerializeField] private TextMeshProUGUI TextoNombre;
    [SerializeField] private TextMeshProUGUI TextoDescripcion;
    [SerializeField] private TextMeshProUGUI TextoSegundos;
    [SerializeField] private GameObject[] CasillasMateriales;
    [SerializeField] private TextMeshProUGUI TextoCantidad;
    [SerializeField] private TextMeshProUGUI DialogoCarga;
    [SerializeField] private GameObject Barra;
    [SerializeField] private float DuracionBarra;
    [SerializeField] private GameObject ObjetoCreado;

    private Image Carga;
    private int ObjetoActual = 0;
    private float TiempoProduciendo = 0;
    private int cantidadAProducir = 0;
    private int cantidadProducida = 0;
    public bool Produciendo=false;

    void Start()
    {
        Carga = Barra.transform.GetChild(1).GetComponent<Image>();
    }

    void Update()
    {
        if (gameObject.name != "Aserradero")
        {
            ObjetoEspecial.SetActive(cantidadAProducir > 0);
        }
        ActualizarDatos();
        Producir();
    }

    private void Producir()
    {
        if (cantidadAProducir > 0)
        {
            if (gameObject.name == "Aserradero")
            {
                transform.GetChild(0).GetComponent<Scr_GirarObjeto>().enabled = true;
            }
            TiempoProduciendo += Time.deltaTime;
            Carga.fillAmount = TiempoProduciendo / ObjetosQueProduce[ObjetoActual].TiempoDeProduccion;

            if (Carga.fillAmount >= 1)
            {
                CambiarDialogos();
                GenerarObjeto();
                cantidadAProducir--;
                TiempoProduciendo = 0;
                Carga.fillAmount = 0;
            }
        }
        else
        {
            if (gameObject.name == "Aserradero")
            {
                transform.GetChild(0).GetComponent<Scr_GirarObjeto>().enabled = false;
            }
            TiempoProduciendo = 0;
        }
    }

    private void CambiarDialogos()
    {
        DialogoCarga.text = $"{DialogosIniciales[UnityEngine.Random.Range(0, DialogosIniciales.Length)]} {DialogosMedios[UnityEngine.Random.Range(0, DialogosMedios.Length)]} {DialogosFinales[UnityEngine.Random.Range(0, DialogosFinales.Length)]}";
    }

    private void GenerarObjeto()
    {
        cantidadProducida += ObjetosQueProduce[ObjetoActual].CantidadProducto;
        var objetoTransform = ObjetoCreado.transform;
        objetoTransform.GetChild(0).GetComponent<Image>().sprite = ObjetosQueProduce[ObjetoActual].Icono;
        objetoTransform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = cantidadProducida.ToString();
    }

    private void ActualizarDatos()
    {
        var objetoActual = ObjetosQueProduce[ObjetoActual];
        TextoNombre.text = objetoActual.Nombre;
        TextoDescripcion.text = objetoActual.Descripcion;
        TextoSegundos.text = $"{objetoActual.TiempoDeProduccion}s";
        ObjetosInfo.transform.GetChild(ObjetoActual).GetComponent<Image>().color = Color.white;


        for (int i = 0; i < CasillasMateriales.Length; i++)
        {
            var material = objetoActual.MaterialesDeProduccion[i];
            if (material != null)
            {
                var casilla = CasillasMateriales[i];
                casilla.SetActive(true);
                casilla.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = material.Icono;
                var cantidadText = casilla.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
                cantidadText.text = objetoActual.CantidadMaterialesDeProduccion[i].ToString();

                if (GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_Inventario>() !=null)
                {
                    var inventario = GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_Inventario>();
                    var cantidadEnInventario = inventario.Cantidades[Array.IndexOf(inventario.Objetos, material)];
                    cantidadText.color = cantidadEnInventario < objetoActual.CantidadMaterialesDeProduccion[i] ? Color.red : Color.white;
                }
            }
            else
            {
                CasillasMateriales[i].SetActive(false);
            }
        }

        TextoCantidad.text = cantidadAProducir.ToString();
        ObjetoCreado.SetActive(cantidadProducida > 0);

        var barraRect = Barra.GetComponent<RectTransform>();
        var barraPositionY = barraRect.anchoredPosition.y;
        if (cantidadAProducir > 0 && barraPositionY == -755)
        {
            Tween.UIAnchoredPosition3DY(barraRect, -275, DuracionBarra);
            Produciendo = true;
        }
        else if (cantidadAProducir == 0 && barraPositionY == -275)
        {
            Tween.UIAnchoredPosition3DY(barraRect, -755, DuracionBarra);
            Produciendo = false;
        }
    }

    public void Flechas(bool Aumenta)
    {
        if (Aumenta)
        {
            if (cantidadAProducir + cantidadProducida < 99 && TieneMaterialesNecesarios() && ObtenerCantidadMinima() > 0)
            {
                if (cantidadAProducir == 0) CambiarDialogos();
                cantidadAProducir++;

                int i = 0;
                foreach (Scr_CreadorObjetos Objeto in ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion)
                {
                    if (Objeto != null)
                    {
                        QuitarObjeto(Objeto, ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i]);
                    }
                    i++;
                }
            }
        }
        else
        {
            if (cantidadAProducir > 0)
            {
                cantidadAProducir--;
                int i = 0;
                foreach (Scr_CreadorObjetos Objeto in ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion)
                {
                    if (Objeto != null)
                    {
                        DarObjeto(Objeto, ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i]);
                    }
                    i++;
                }
            }
        }
    }

    private bool TieneMaterialesNecesarios()
    {
        var inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();
        for (int i = 0; i < ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion.Length; i++)
        {
            var material = ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion[i];
            if (material != null)
            {
                int index = Array.IndexOf(inventario.Objetos, material);
                if (index == -1 || inventario.Cantidades[index] < ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void EntraBoton(Image Boton)
    {
        Boton.color = ColoresBotones[0];
    }

    public void SaleBoton(Image Boton)
    {
        Boton.color = ColoresBotones[1];
    }

    public void SeleccionarItem(int Numero)
    {
        if (cantidadAProducir == 0 && cantidadProducida == 0)
        {
            foreach (Image imagen in ObjetosInfo.GetComponentsInChildren<Image>())
            {
                if (!imagen.gameObject.name.StartsWith("I"))
                {
                    imagen.color = ColoresBotones[1];
                }
            }
            ObjetosInfo.GetComponent<Image>().color = Color.white;
            ObjetoActual = Numero;
        }

    }

    public void RecolectarItem()
    {
        DarObjeto(ObjetosQueProduce[ObjetoActual], cantidadProducida);
        GameObject.Find("ObjetosAgregados").GetComponent<Scr_ObjetosAgregados>().Lista.Add(ObjetosQueProduce[ObjetoActual]);
        GameObject.Find("ObjetosAgregados").GetComponent<Scr_ObjetosAgregados>().Cantidades.Add(cantidadProducida);
        cantidadProducida = 0;
    }

    public void BotonMitad()
    {

        if (cantidadAProducir == 0)
        {
            CambiarDialogos();
            cantidadAProducir = Mathf.CeilToInt(ObtenerCantidadMinima() / 2.0f);
            cantidadAProducir = Mathf.Min(cantidadAProducir, 99 - cantidadProducida);
            int i = 0;
            foreach (Scr_CreadorObjetos Objeto in ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion)
            {
                if (Objeto != null)
                {
                    QuitarObjeto(Objeto, ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i] * cantidadAProducir);
                }
                i++;
            }
        }
        else
        {
            if (cantidadAProducir > 1)
            {

                int i = 0;
                foreach (Scr_CreadorObjetos Objeto in ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion)
                {
                    if (Objeto != null)
                    {
                        DarObjeto(Objeto, ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i] * Mathf.FloorToInt(cantidadAProducir / 2.0f));
                    }
                    i++;
                }
                cantidadAProducir = Mathf.CeilToInt(cantidadAProducir / 2.0f);
                cantidadAProducir = Mathf.Min(cantidadAProducir, 99 - cantidadProducida);
            }
        }

    }

    public void BotonMax()
    {
        int nuevaCantidadAProducir = ObtenerCantidadMinima();
        nuevaCantidadAProducir = Mathf.Min(nuevaCantidadAProducir, 99 - cantidadProducida);

        if (cantidadAProducir == 0)
        {
            CambiarDialogos();
            cantidadAProducir = nuevaCantidadAProducir;
        }
        else
        {
            int incremento = nuevaCantidadAProducir;
            cantidadAProducir += incremento;
            cantidadAProducir = Mathf.Min(cantidadAProducir, 99 - cantidadProducida);
        }

        int i = 0;
        foreach (Scr_CreadorObjetos Objeto in ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion)
        {
            if (Objeto != null)
            {
                int cantidadNecesaria = ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i] * cantidadAProducir;
                int cantidadActual = ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i] * (cantidadAProducir - nuevaCantidadAProducir);
                QuitarObjeto(Objeto, cantidadNecesaria - cantidadActual);
            }
            i++;
        }
    }

    public void BotonBorrar()
    {
        int i = 0;
        foreach (Scr_CreadorObjetos Objeto in ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion)
        {
            if (Objeto != null)
            {
                DarObjeto(Objeto, ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i] * cantidadAProducir);
            }
            i++;
        }
        cantidadAProducir = 0;

    }

    private int ObtenerCantidadMinima()
    {
        var inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();
        var cantidadesMinimas = ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion
            .Select((material, i) => material == null ? int.MaxValue : inventario.Cantidades[Array.IndexOf(inventario.Objetos, material)] / ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i])
            .ToArray();
        return cantidadesMinimas.Min();
    }

    private void QuitarObjeto(Scr_CreadorObjetos Objeto, int cantidad)
    {
        var inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();
        int index = Array.IndexOf(inventario.Objetos, Objeto);
        if (index >= 0) inventario.Cantidades[index] -= cantidad;
    }

    private void DarObjeto(Scr_CreadorObjetos Objeto, int cantidad)
    {
        var inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();
        int index = Array.IndexOf(inventario.Objetos, Objeto);
        if (index >= 0) inventario.Cantidades[index] += cantidad;
    }
}
