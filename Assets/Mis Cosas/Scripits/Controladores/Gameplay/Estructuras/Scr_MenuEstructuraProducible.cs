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
    [SerializeField] private string HabilidadMejoraVelocidad;

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

    [Header("En caso de tener varias estructuras")]
    [SerializeField] string PlanoRequerido;
    [SerializeField] GameObject BotonEstructuraPrimaria;
    [SerializeField] Sprite ImagenBotonEstructuraPrimaria;
    [SerializeField] Sprite ImagenIconoEstructuraPrimaria;
    [SerializeField] GameObject BotonEstructuraSecundaria;
    [SerializeField] Sprite ImagenBotonEstructuraSecundaria;
    [SerializeField] Sprite ImagenIconoEstructuraSecundaria;
    [SerializeField] Image PanelObjetos;
    [SerializeField] Sprite PanelObjetosPrincipal;
    [SerializeField] Sprite PanelObjetosSecundario;
    [SerializeField] Image PanelInformacion;
    [SerializeField] Sprite PanelInformacionPrincipal;
    [SerializeField] Sprite PanelInformacionsecundario;
    [SerializeField] Image PanelProductos;
    [SerializeField] Sprite PanelProductoPrincipal;
    [SerializeField] Sprite PanelProductoSecundario;
    [SerializeField] GameObject[] ObjetosPrincipales;
    [SerializeField] GameObject[] ObjetosSecundarios;
    [SerializeField] GameObject BarraCarga;
    [SerializeField] Sprite CargaPrincipal;
    [SerializeField] Sprite CargaSombraPrincipal;
    [SerializeField] Sprite CargaSecundario;
    [SerializeField] Sprite CargaSombraSecundario;
    private int estructuraActual = 0;
    [SerializeField] private Scr_ActivadorMenuEstructuraCircular Activador;

    private Image Carga;
    private int ObjetoActual = 0;
    private float TiempoProduciendo = 0;
    private int cantidadAProducir = 0;
    private int cantidadProducida = 0;
    public bool Produciendo = false;

    private Scr_Inventario inventario;

    void Awake()
    {
        inventario = GameObject.Find("Gata")
            .transform.GetChild(7)
            .GetComponent<Scr_Inventario>();
    }

    void Start()
    {


        Carga = Barra.transform.GetChild(1).GetComponent<Image>();
        Cargar();

        // 🔹 FORZAR estado visual correcto al abrir la UI
        if (cantidadProducida > 0)
            ActualizarObjetoCreadoVisual();
        else
            LimpiarObjetoCreadoUI();
    }

    private void OnEnable()
    {
        if (PlanoRequerido != "")
        {
            if (PlayerPrefs.GetString("Habilidad:" + PlanoRequerido, "No") == "Si")
            {
                BotonEstructuraSecundaria.gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (gameObject.name != "Aserradero")
            ObjetoEspecial.SetActive(cantidadAProducir > 0);

        ActualizarDatos();
        Producir();
    }

    private void Producir()
    {
        if (cantidadAProducir > 0)
        {
            if (gameObject.name == "Aserradero")
                transform.GetChild(0).GetComponent<Scr_GirarObjeto>().enabled = true;

            TiempoProduciendo += Time.deltaTime *
                (PlayerPrefs.GetString("Habilidad:" + HabilidadMejoraVelocidad, "No") == "Si" ? 2 : 1);

            Carga.fillAmount = TiempoProduciendo / ObjetosQueProduce[ObjetoActual].TiempoDeProduccion;

            if (Carga.fillAmount >= 1)
            {
                CambiarDialogos();
                GenerarObjeto();
                cantidadAProducir--;
                TiempoProduciendo = 0;
                Carga.fillAmount = 0;
                Guardar();
            }
        }
        else
        {
            if (gameObject.name == "Aserradero")
                transform.GetChild(0).GetComponent<Scr_GirarObjeto>().enabled = false;

            TiempoProduciendo = 0;
        }
    }

    private void CambiarDialogos()
    {
        DialogoCarga.text =
            $"{DialogosIniciales[UnityEngine.Random.Range(0, DialogosIniciales.Length)]} " +
            $"{DialogosMedios[UnityEngine.Random.Range(0, DialogosMedios.Length)]} " +
            $"{DialogosFinales[UnityEngine.Random.Range(0, DialogosFinales.Length)]}";
    }

    private void GenerarObjeto()
    {
        cantidadProducida += ObjetosQueProduce[ObjetoActual].CantidadProducto;
        ActualizarObjetoCreadoVisual();
    }

    private void ActualizarObjetoCreadoVisual()
    {
        ObjetoCreado.SetActive(true);
        ObjetoCreado.transform.GetChild(0).GetComponent<Image>().sprite =
            ObjetosQueProduce[ObjetoActual].Icono;

        ObjetoCreado.transform.GetChild(1)
            .GetChild(0)
            .GetComponent<TextMeshProUGUI>()
            .text = cantidadProducida.ToString();
    }

    private void ActualizarDatos()
    {
        var objetoActual = ObjetosQueProduce[ObjetoActual];

        TextoNombre.text = objetoActual.Nombre;
        TextoDescripcion.text = objetoActual.Descripcion;
        TextoSegundos.text =
            (PlayerPrefs.GetString("Habilidad:" + HabilidadMejoraVelocidad, "No") == "Si"
            ? objetoActual.TiempoDeProduccion / 2
            : objetoActual.TiempoDeProduccion) + "s";

        ObjetosInfo.transform.GetChild(ObjetoActual).GetComponent<Image>().color = Color.white;

        for (int i = 0; i < CasillasMateriales.Length; i++)
        {
            var material = objetoActual.MaterialesDeProduccion[i];
            if (material != null)
            {
                CasillasMateriales[i].SetActive(true);
                CasillasMateriales[i].transform.GetChild(0)
                    .GetChild(0)
                    .GetComponent<Image>().sprite = material.Icono;

                var texto = CasillasMateriales[i].transform.GetChild(1)
                    .GetChild(0)
                    .GetComponent<TextMeshProUGUI>();

                texto.text = objetoActual.CantidadMaterialesDeProduccion[i].ToString();

                int index = Array.IndexOf(inventario.Objetos, material);
                int cantidadInv = index >= 0 ? inventario.Cantidades[index] : 0;

                texto.color = cantidadInv < objetoActual.CantidadMaterialesDeProduccion[i]
                    ? Color.red
                    : Color.white;
            }
            else
            {
                CasillasMateriales[i].SetActive(false);
            }
        }

        TextoCantidad.text = cantidadAProducir.ToString();

        var barraRect = Barra.GetComponent<RectTransform>();
        float y = barraRect.anchoredPosition.y;

        if (cantidadAProducir > 0 && y == -755)
        {
            Tween.UIAnchoredPosition3DY(barraRect, -275, DuracionBarra);
            Produciendo = true;
        }
        else if (cantidadAProducir == 0 && y == -275)
        {
            Tween.UIAnchoredPosition3DY(barraRect, -755, DuracionBarra);
            Produciendo = false;
        }
    }

    public void Flechas(bool Aumenta)
    {
        if (Aumenta)
        {
            if (cantidadAProducir + cantidadProducida < 99 &&
                TieneMaterialesNecesarios() &&
                ObtenerCantidadMinima() > 0)
            {
                if (cantidadAProducir == 0) CambiarDialogos();
                cantidadAProducir++;

                for (int i = 0; i < ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion.Length; i++)
                {
                    var mat = ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion[i];
                    if (mat != null)
                        inventario.QuitarObjeto(mat.Nombre,
                            ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i], false);
                }

                Guardar();
            }
        }
        else if (cantidadAProducir > 0)
        {
            cantidadAProducir--;

            for (int i = 0; i < ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion.Length; i++)
            {
                var mat = ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion[i];
                if (mat != null)
                    inventario.AgregarObjeto(mat.Nombre,
                        ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i], false, false);
            }

            Guardar();
        }
    }

    private bool TieneMaterialesNecesarios()
    {
        for (int i = 0; i < ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion.Length; i++)
        {
            var mat = ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion[i];
            if (mat == null) continue;

            int index = Array.IndexOf(inventario.Objetos, mat);
            if (index < 0 ||
                inventario.Cantidades[index] <
                ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i])
                return false;
        }
        return true;
    }

    public void RecolectarItem()
    {
        if (cantidadProducida <= 0)
            return;

        inventario.AgregarObjeto(
            ObjetosQueProduce[ObjetoActual].Nombre,
            cantidadProducida,
            true,
            true
        );

        cantidadProducida = 0;
        LimpiarObjetoCreadoUI();
        Guardar();
    }

    public void EntraBoton(Image Boton) => Boton.color = ColoresBotones[0];
    public void SaleBoton(Image Boton) => Boton.color = ColoresBotones[1];

    public void SeleccionarItem(int Numero)
    {
        if (cantidadAProducir == 0 && cantidadProducida == 0)
        {
            foreach (Image img in ObjetosInfo.GetComponentsInChildren<Image>())
                if (!img.gameObject.name.StartsWith("I"))
                    img.color = ColoresBotones[1];

            ObjetosInfo.GetComponent<Image>().color = Color.white;
            ObjetoActual = Numero;
            Guardar();
        }
    }

    private int ObtenerCantidadMinima()
    {
        return ObjetosQueProduce[ObjetoActual].MaterialesDeProduccion
            .Select((m, i) =>
                m == null ? int.MaxValue :
                inventario.Cantidades[Array.IndexOf(inventario.Objetos, m)] /
                ObjetosQueProduce[ObjetoActual].CantidadMaterialesDeProduccion[i])
            .Min();
    }

    public void Guardar()
    {
        PlayerPrefs.SetInt(name + "_ObjetoActual", ObjetoActual);
        PlayerPrefs.SetInt(name + "_CantidadAProducir", cantidadAProducir);
        PlayerPrefs.SetInt(name + "_CantidadProducida", cantidadProducida);
        PlayerPrefs.SetFloat(name + "_TiempoProduciendo", TiempoProduciendo);
        PlayerPrefs.SetFloat(name + "_BarraProgreso", Carga.fillAmount);
        PlayerPrefs.SetInt(name + "_Produciendo", Produciendo ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void Cargar()
    {
        ObjetoActual = PlayerPrefs.GetInt(name + "_ObjetoActual", 0);
        cantidadAProducir = PlayerPrefs.GetInt(name + "_CantidadAProducir", 0);
        cantidadProducida = PlayerPrefs.GetInt(name + "_CantidadProducida", 0);
        TiempoProduciendo = PlayerPrefs.GetFloat(name + "_TiempoProduciendo", 0);
        Carga.fillAmount = PlayerPrefs.GetFloat(name + "_BarraProgreso", 0);
        Produciendo = PlayerPrefs.GetInt(name + "_Produciendo", 0) == 1;
    }

    private void LimpiarObjetoCreadoUI()
    {
        ObjetoCreado.SetActive(false);
        ObjetoCreado.transform.GetChild(1)
            .GetChild(0)
            .GetComponent<TextMeshProUGUI>()
            .text = "0";
    }

    public void DecidirCambiarOSalir(int NumEstructura)
    {

        if (Produciendo || cantidadAProducir > 0)
        {
            return;
        }


        if (NumEstructura == estructuraActual)
        {
            Activador.Salir();
        }
        else
        {
            estructuraActual = NumEstructura;
            ActualizarEstructura();
        }
    }

    private void ActualizarEstructura()
    {
        if (estructuraActual == 0)
        {
            BotonEstructuraPrimaria.GetComponent<Image>().sprite = ImagenBotonEstructuraPrimaria;
            BotonEstructuraPrimaria.transform.GetChild(0).gameObject.SetActive(true);
            BotonEstructuraSecundaria.GetComponent<Image>().sprite = ImagenIconoEstructuraSecundaria;
            BotonEstructuraSecundaria.transform.GetChild(0).gameObject.SetActive(false);


            PanelInformacion.sprite = PanelInformacionPrincipal;
            PanelObjetos.sprite = PanelObjetosPrincipal;
            PanelProductos.sprite = PanelProductoPrincipal;
            PanelProductos.transform.GetChild(1).GetComponent<Image>().sprite = PanelProductoPrincipal;
            BarraCarga.transform.GetChild(0).GetComponent<Image>().sprite = CargaSombraPrincipal;
            BarraCarga.transform.GetChild(1).GetComponent<Image>().sprite = CargaPrincipal;

            foreach (GameObject Objeto in ObjetosPrincipales)
            {
                Objeto.SetActive(true);
            }
            foreach (GameObject Objeto in ObjetosSecundarios)
            {
                Objeto.SetActive(false);
            }
        }
        else
        {
            BotonEstructuraPrimaria.GetComponent<Image>().sprite = ImagenIconoEstructuraPrimaria;
            BotonEstructuraPrimaria.transform.GetChild(0).gameObject.SetActive(false);
            BotonEstructuraSecundaria.GetComponent<Image>().sprite = ImagenBotonEstructuraSecundaria;
            BotonEstructuraSecundaria.transform.GetChild(0).gameObject.SetActive(true);

            PanelInformacion.sprite = PanelInformacionsecundario;
            PanelObjetos.sprite = PanelObjetosSecundario;
            PanelProductos.sprite = PanelProductoSecundario;
            PanelProductos.transform.GetChild(1).GetComponent<Image>().sprite = PanelProductoSecundario;
            BarraCarga.transform.GetChild(0).GetComponent<Image>().sprite = CargaSombraSecundario;
            BarraCarga.transform.GetChild(1).GetComponent<Image>().sprite = CargaSecundario;

            foreach (GameObject Objeto in ObjetosPrincipales)
            {
                Objeto.SetActive(false);
            }

            foreach (GameObject Objeto in ObjetosSecundarios)
            {
                Objeto.SetActive(true);
            }
        }

        for (int i = 0; i < ObjetosInfo.transform.childCount; i++)
        {
            Transform hijo = ObjetosInfo.transform.GetChild(i);

            if (hijo.gameObject.activeInHierarchy)
            {
                SeleccionarItem(i);
                break;
            }
        }

    }


}
