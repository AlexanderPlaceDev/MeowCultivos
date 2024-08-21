using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorUIBatalla : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] Color[] ColoresBotones;
    [SerializeField] Color[] ColoresMenus;
    [SerializeField] Image[] BotonesMenus;
    [SerializeField] GameObject PanelMenus;
    [Header("Datos Armas")]
    public int ArmaActual = 1;
    [SerializeField] GameObject FlechaDerechaArma;
    [SerializeField] GameObject FlechaIzquierdaArma;
    [SerializeField] TextMeshProUGUI Tipotxt;
    [SerializeField] TextMeshProUGUI Nombretxt;
    [SerializeField] TextMeshProUGUI Descripciontxt;
    [SerializeField] TextMeshProUGUI Cadenciatxt;
    [SerializeField] TextMeshProUGUI Capacidadtxt;
    [SerializeField] TextMeshProUGUI Rangotxt;
    [SerializeField] Image RangoBarra;
    [SerializeField] TextMeshProUGUI Dañotxt;
    [SerializeField] Image DañoBarra;
    [SerializeField] TextMeshProUGUI Velocidadtxt;
    [SerializeField] Image VelocidadBarra;
    [SerializeField] GameObject[] Armas;
    [Header("Alerta")]
    [SerializeField] GameObject Alerta;
    [SerializeField] GameObject PanelAlerta;
    [Header("Espacios Armas")]
    [SerializeField] GameObject[] EspaciosArmas;
    bool AlertaAdentro = false;
    [Header("Aceptar")]
    [SerializeField] Image BotonAceptar;
    [SerializeField] Color[] ColoresAceptar;
    [SerializeField] GameObject CanvasSeleccionDeArmas;
    [SerializeField] GameObject CanvasGameplay;
    [SerializeField] GameObject ObjetosArmas;
    [SerializeField] GameObject Mapa;

    [Header("Generales")]
    public int BotonActual = -1;
    GameObject Singleton;

    void Start()
    {
    }

    void Update()
    {
        BuscarSingleton();
        ActualizarBotonesMenus();
        ActualizarAlerta();
        ActualizarArma();
        ActualizarEspaciosArmas();
    }

    

    private void ActualizarEspaciosArmas()
    {
        for(int i = 0; i <= EspaciosArmas.Length; i++)
        {
            if(PlayerPrefs.GetString("Espacio Arma" + i, "No") == "Si")
            {
                EspaciosArmas[i].SetActive(true);
            }
        };
    }

    private void BuscarSingleton()
    {
        if (Singleton == null)
        {
            Singleton = GameObject.Find("Singleton");
        }
    }

    private void ActualizarArma()
    {
        //Contar Armas Desbloqueadas

        int ArmasDesbloqueadas = 0;
        int UltimaArmaDesbloqueada = 0;

        int i = 0;
        foreach (bool Arma in Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas)
        {
            if (Arma)
            {
                ArmasDesbloqueadas++;
                UltimaArmaDesbloqueada = i;
            }
            i++;
        }
        //Actualizar Flechas
        if (ArmasDesbloqueadas > 1)
        {
            if (ArmaActual == 0)
            {
                FlechaDerechaArma.SetActive(true);
                FlechaIzquierdaArma.SetActive(false);
                ActualizarBotonArma("NoIzquierda");
            }
            if (ArmaActual == UltimaArmaDesbloqueada)
            {
                FlechaDerechaArma.SetActive(false);
                FlechaIzquierdaArma.SetActive(true);
                ActualizarBotonArma("NoDerecha");
            }
            if (ArmaActual > 0 && ArmaActual < UltimaArmaDesbloqueada)
            {
                FlechaDerechaArma.SetActive(true);
                FlechaIzquierdaArma.SetActive(true);
            }
        }
        else
        {
            FlechaDerechaArma.SetActive(false);
            FlechaIzquierdaArma.SetActive(false);
        }

        Scr_CreadorArmas DatosArma = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[ArmaActual];
        //Actualiza Nombre, Descripcion y Tipo
        Nombretxt.text = DatosArma.Nombre;
        Color colorSuperior = DatosArma.Color;
        VertexGradient gradienteActual = Nombretxt.colorGradient;
        gradienteActual.topLeft = colorSuperior;
        gradienteActual.topRight = colorSuperior;
        Nombretxt.colorGradient = gradienteActual;
        Descripciontxt.text = DatosArma.Descripcion;
        Tipotxt.text = DatosArma.Tipo;

        //Actualizar Datos
        Cadenciatxt.text = DatosArma.Cadencia + " s";
        if (DatosArma.CapacidadTotal == 0)
        {
            Capacidadtxt.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            Capacidadtxt.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            Capacidadtxt.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            Capacidadtxt.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
            Capacidadtxt.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            Capacidadtxt.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            Capacidadtxt.text = DatosArma.Capacidad + "/" + DatosArma.CapacidadTotal;
        }

        RangoBarra.fillAmount = (float)DatosArma.Alcance / 100;
        Rangotxt.text = DatosArma.Alcance + "/100";
        DañoBarra.fillAmount = (float)DatosArma.Daño / 100;
        Dañotxt.text = DatosArma.Daño + "/100";
        VelocidadBarra.fillAmount = (float)DatosArma.Velocidad / 100;
        Velocidadtxt.text = DatosArma.Velocidad + "/100";

        if (DatosArma.DobleMano)
        {
            Alerta.SetActive(true);
        }
        else
        {
            Alerta.SetActive(false);
        }

        //Actualizar Imagen
        if (!Armas[ArmaActual].activeSelf)
        {
            foreach(GameObject Arma in Armas)
            {
                Arma.SetActive(false);
            }
            Armas[ArmaActual].SetActive(true);
        }

    }

    public void EntraBotonMenu(int Boton)
    {
        BotonActual = Boton;
    }

    public void SaleBotonMenu()
    {
        BotonActual = -1;
    }

    void ActualizarBotonesMenus()
    {
        if (BotonActual != -1)
        {
            PanelMenus.SetActive(true);
            BotonesMenus[BotonActual].color = ColoresBotones[1];

            switch (BotonActual)
            {
                case 0:
                    {
                        PanelMenus.GetComponent<Image>().color = ColoresMenus[0];
                        PanelMenus.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Habilidades";
                        break;
                    }

                case 1:
                    {
                        PanelMenus.GetComponent<Image>().color = ColoresMenus[1];
                        PanelMenus.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Skins";
                        break;
                    }

                case 2:
                    {
                        PanelMenus.GetComponent<Image>().color = ColoresMenus[2];
                        PanelMenus.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Controles";
                        break;
                    }
            }

            for (int i = 0; i < BotonesMenus.Length; i++)
            {
                if (i != BotonActual)
                {
                    BotonesMenus[i].color = ColoresBotones[0];
                }
            }
        }
        else
        {
            PanelMenus.SetActive(false);
            BotonesMenus[0].color = ColoresBotones[0];
            BotonesMenus[1].color = ColoresBotones[0];
            BotonesMenus[2].color = ColoresBotones[0];
        }
    }

    public void EntraAlerta()
    {
        AlertaAdentro = true;
    }

    public void SaleAlerta()
    {
        AlertaAdentro = false;
    }

    void ActualizarAlerta()
    {
        if (AlertaAdentro)
        {
            PanelAlerta.SetActive(true);
        }
        else
        {
            PanelAlerta.SetActive(false);
        }
    }

    public void BotonArmaDerecha()
    {
        for (int i = ArmaActual + 1; i <= Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas.Length; i++)
        {
            if (Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas[i])
            {
                ArmaActual = i;
                break;

            }
        }
    }

    public void BotonArmaIzquierda()
    {
        for (int i = ArmaActual - 1; i >= 0; i--)
        {
            if (Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas[i])
            {
                ArmaActual = i;
                break;

            }
        }
    }

    public void ActualizarBotonArma(string EntrayOrientacion)
    {
        switch (EntrayOrientacion)
        {
            case "SiIzquierda":
                {
                    FlechaIzquierdaArma.transform.GetChild(0).GetComponent<Image>().color = ColoresBotones[1];
                    break;
                }

            case "NoIzquierda":
                {
                    FlechaIzquierdaArma.transform.GetChild(0).GetComponent<Image>().color = ColoresBotones[0];
                    break;
                }

            case "SiDerecha":
                {
                    FlechaDerechaArma.transform.GetChild(0).GetComponent<Image>().color = ColoresBotones[1];
                    break;
                }

            case "NoDerecha":
                {
                    FlechaDerechaArma.transform.GetChild(0).GetComponent<Image>().color = ColoresBotones[0];
                    break;
                }
        }
    }

    public void AceptarBatalla()
    {
        CanvasSeleccionDeArmas.SetActive(false);
        CanvasGameplay.SetActive(true);
        ObjetosArmas.SetActive(true);
        Mapa.SetActive(true);
        GetComponent<Scr_ControladorBatalla>().CuentaAtras();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CambiarColorBotonAceptar(bool Entra)
    {
        if (Entra)
        {
            BotonAceptar.color = ColoresAceptar[0];
            BotonAceptar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = ColoresAceptar[1];
        }
        else
        {
            BotonAceptar.color = ColoresAceptar[2];
            BotonAceptar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = ColoresAceptar[3];
        }
    }

}
