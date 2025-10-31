using System;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorUIBatalla : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] Color[] ColoresBotones;
    [Header("Datos Armas")]
    public int ArmaActual = 0;
    [SerializeField] GameObject FlechaDerechaArma;
    [SerializeField] GameObject FlechaIzquierdaArma;
    [SerializeField] TextMeshProUGUI Tipotxt;
    [SerializeField] TextMeshProUGUI Nombretxt;
    [SerializeField] TextMeshProUGUI Descripciontxt;
    [SerializeField] GameObject[] Armas;
    [Header("Habilidades")]//para acomodar visaulmente los iconos de las habilidades
    [SerializeField] Sprite IconoVacio;
    [SerializeField] Image HabilidadTemporal;
    [SerializeField] Image Habilidad1;
    [SerializeField] Image Habilidad2;
    [SerializeField] Image HabilidadEspecial;
    [SerializeField] Image BarraHabilidadTemporal;
    [SerializeField] GameObject usosHabilidadT;
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
    [SerializeField] GameObject IconoHabilidad1;
    [SerializeField] GameObject IconoHabilidad2;
    [SerializeField] GameObject IconoHabilidadE;

    [Header("Generales")]
    public int BotonActual = -1;
    GameObject Singleton;
    Scr_DatosArmas datos;
    Scr_ControladorBatalla ControladorBatalla;
    Scr_CreadorArmas DatosArma;


    Scr_CreadorHabilidadesBatalla HabT;
    Scr_CreadorHabilidadesBatalla Hab1;
    Scr_CreadorHabilidadesBatalla Hab2;
    Scr_CreadorHabilidadesBatalla HabE;
    void Start()
    {
        ControladorBatalla = GetComponent<Scr_ControladorBatalla>();

    }

    void Update()
    {
        BuscarSingleton();
        ActualizarArma();
    }

    private void BuscarSingleton()
    {
        if (Singleton == null)
        {
            Singleton = GameObject.Find("Singleton");
        }

        datos = Singleton.GetComponent<Scr_DatosArmas>();
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
        DatosArma = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[ArmaActual];
        //Actualiza Nombre, Descripcion y Tipo
        Nombretxt.text = DatosArma.Nombre;
        Color colorSuperior = DatosArma.Color;
        VertexGradient gradienteActual = Nombretxt.colorGradient;
        gradienteActual.topLeft = colorSuperior;
        gradienteActual.topRight = colorSuperior;
        Nombretxt.colorGradient = gradienteActual;
        Descripciontxt.text = DatosArma.Descripcion;
        Tipotxt.text = DatosArma.Tipo;

        ControladorBatalla.ConseguirHabilidadesArma(DatosArma.Nombre);
        // Guardar referencias una sola vez
        

        // Obtener nombres de habilidades desde el controlador
        string ht = ControladorBatalla.HabilidadT;
        string h1 = ControladorBatalla.Habilidad1;
        string h2 = ControladorBatalla.Habilidad2;
        string hE = ControladorBatalla.HabilidadEspecial;
        int us = ControladorBatalla.usosHabilidad;
        // Buscar habilidades usando el script de datos
        HabT = datos.BuscarHabilidadTemporalPorNombre(ht);
        Hab1 = datos.BuscarHabilidadPermanentePorNombre(h1);
        Hab2 = datos.BuscarHabilidadPermanentePorNombre(h2);
        HabE = datos.BuscarHabilidadPermanentePorNombre(hE);


        if (ht == "Nada")
        {
            HabilidadTemporal.sprite = IconoVacio;
            usosHabilidadT.SetActive(false);
        }
        else
        {
            usosHabilidadT.SetActive(true);
            HabilidadTemporal.sprite = HabT.Icono;
            BarraHabilidadTemporal.fillAmount = us / HabT.Usos;
        }
        Habilidad1.sprite=Hab1.Icono;
        Habilidad2.sprite=Hab2.Icono;
        HabilidadEspecial.sprite=HabE.Icono;

        //Actualizar Imagen
        if (!Armas[ArmaActual].activeSelf)
        {
            foreach (GameObject Arma in Armas)
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

    public void EntraAlerta()
    {
        AlertaAdentro = true;
    }

    public void SaleAlerta()
    {
        AlertaAdentro = false;
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
                    FlechaIzquierdaArma.GetComponent<Image>().color = ColoresBotones[1];
                    break;
                }

            case "NoIzquierda":
                {
                    FlechaIzquierdaArma.GetComponent<Image>().color = ColoresBotones[0];
                    break;
                }

            case "SiDerecha":
                {
                    FlechaDerechaArma.GetComponent<Image>().color = ColoresBotones[1];
                    break;
                }

            case "NoDerecha":
                {
                    FlechaDerechaArma.GetComponent<Image>().color = ColoresBotones[0];
                    break;
                }
        }
    }

    public void AceptarBatalla()
    {
        checarUsosHabilidad();
        CanvasSeleccionDeArmas.SetActive(false);
        CanvasGameplay.SetActive(true);
        ObjetosArmas.SetActive(true);
        IconoHabilidad1.transform.GetChild(0).GetComponent<Image>().sprite = Hab1.Icono;
        IconoHabilidad2.transform.GetChild(0).GetComponent<Image>().sprite = Hab2.Icono;
        IconoHabilidadE.transform.GetChild(0).GetComponent<Image>().sprite = HabE.Icono;
        if (Hab1.Tipo == "Pasiva")
        {
            IconoHabilidad1.transform.GetChild(1).gameObject.SetActive(false);
        }
        if (Hab2.Tipo == "Pasiva")
        {
            IconoHabilidad2.transform.GetChild(1).gameObject.SetActive(false);
        }
        if (HabE.Tipo == "Pasiva")
        {
            IconoHabilidadE.transform.GetChild(1).gameObject.SetActive(false);
        }
        ControladorBatalla.IniciarCuentaRegresiva();
        ControladorBatalla.ArmaActual = Armas[ArmaActual];
        GetComponent<Scr_ControladorArmas>().ArmaActual = ArmaActual;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void checarUsosHabilidad()
    {
        int resultado = ControladorBatalla.usosHabilidad - 1;
        PlayerPrefs.SetInt(DatosArma.Nombre + "Usos", resultado);
        if (resultado <= 0)
        {
            PlayerPrefs.SetString(DatosArma.Nombre + "HT", "Nada");
        }
        
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
