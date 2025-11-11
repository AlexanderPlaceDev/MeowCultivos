using System;
using System.Collections.Generic;
using TMPro;
using Unity.Android.Types;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Experimental;
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
    [SerializeField] GameObject Arma;
    [SerializeField] GameObject[] Armas;
    [Header("Habilidades")]//para acomodar visaulmente los iconos de las habilidades
    [SerializeField] Sprite IconoVacio;
    [SerializeField] GameObject HabilidadTemporal;
    [SerializeField] GameObject Habilidad1;
    [SerializeField] GameObject Habilidad2;
    [SerializeField] GameObject HabilidadEspecial;
    [SerializeField] TextMeshProUGUI BarraHabilidadTemporal;
    public bool PuedeSeleccionarH = false;
    [Header("Alerta")]
    [SerializeField] GameObject Alerta;
    [SerializeField] GameObject PanelAlerta;
    [Header("Espacios Armas")]
    [SerializeField] GameObject[] EspaciosArmas;
    public List<Scr_CreadorHabilidadesBatalla> HabilidadesMostrar;
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

    public Transform contentPanel;
    public GameObject BotonPocion;
    Scr_CreadorHabilidadesBatalla HabT;
    Scr_CreadorHabilidadesBatalla Hab1;
    Scr_CreadorHabilidadesBatalla Hab2;
    Scr_CreadorHabilidadesBatalla HabE;


    [Header("CambioHC")]//para acomodar visaulmente los iconos de las habilidades y comsimibles
    public int Habmostrar = 0;
    public GameObject[] HabilidadesUI;
    public GameObject[] Secciones;
    public bool PocionSelec=false;
    public GameObject PocionIcono;
    public Sprite PocionVacio;
    public int NoPocion=-2;
    void Start()
    {
        ControladorBatalla = GetComponent<Scr_ControladorBatalla>();
        BuscarSingleton();
    }

    void Update()
    {
        ActualizarArma();
    }

    private void BuscarSingleton()
    {
        if (Singleton == null)
        {
            Singleton = GameObject.Find("Singleton");
        }

        datos = Singleton.GetComponent<Scr_DatosArmas>();
        MostrarHabilidadGuardada();
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

        //Actualiza Nombre, Descripcion y Tipo
        Nombretxt.text = DatosArma.Nombre;
        Color colorSuperior = DatosArma.Color;
        VertexGradient gradienteActual = Nombretxt.colorGradient;
        gradienteActual.topLeft = colorSuperior;
        gradienteActual.topRight = colorSuperior;
        Nombretxt.colorGradient = gradienteActual;
        Descripciontxt.text = DatosArma.Descripcion;
        Tipotxt.text = DatosArma.Tipo;


        //MostrarHabilidad();

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

    public void MostrarHabilidadGuardada()
    {

        DatosArma = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[ArmaActual];
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
            HabilidadTemporal.transform.GetChild(2).gameObject.SetActive(false);
            BarraHabilidadTemporal.text = "";
        }
        else
        {
            HabilidadTemporal.transform.GetChild(2).gameObject.SetActive(true);
            HabilidadTemporal.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = HabT.Icono;
            BarraHabilidadTemporal.text = $"< {us}/ {HabT.Usos}";
        }
        Habilidad1.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = Hab1.Icono;
        Habilidad1.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = Hab1.Nombre;
        Habilidad2.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = Hab2.Icono;
        Habilidad2.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = Hab2.Nombre;
        HabilidadEspecial.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = HabE.Icono;
        HabilidadEspecial.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = HabE.Nombre;
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
        MostrarHabilidadGuardada();

        EsconderFlechasHabilidades();
        NoPocion = -2;
        PocionSelec = false;
        ChecarPocionActual();
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
        MostrarHabilidadGuardada();

        EsconderFlechasHabilidades();
        NoPocion = -2;
        PocionSelec = false;
        ChecarPocionActual();
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
        EsconderFlechasHabilidades();
        checarUsosHabilidad();
        CanvasSeleccionDeArmas.SetActive(false);
        CanvasGameplay.SetActive(true);
        ObjetosArmas.SetActive(true);
        ControladorBatalla.Guardar_Pocion();
        ControladorBatalla.GuardarHabilidadesArma(DatosArma.Nombre);
        // Obtener nombres de habilidades desde el controlador
        string ht = ControladorBatalla.HabilidadT;
        string h1 = ControladorBatalla.Habilidad1;
        string h2 = ControladorBatalla.Habilidad2;
        string hE = ControladorBatalla.HabilidadEspecial;
        // Buscar habilidades usando el script de datos
        HabT = datos.BuscarHabilidadTemporalPorNombre(ht);
        Hab1 = datos.BuscarHabilidadPermanentePorNombre(h1);
        Hab2 = datos.BuscarHabilidadPermanentePorNombre(h2);
        HabE = datos.BuscarHabilidadPermanentePorNombre(hE);

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
        if (resultado <= 0)
        {
            ControladorBatalla.HabilidadT = "Nada";
            ControladorBatalla.usosHabilidad = 0;
        }
        else
        {
            ControladorBatalla.usosHabilidad = resultado;
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



    //****************************************
    //Seccion de Habiliades y Pociones

    //Cosigue las habilidades Disponibles Siempre Y cuando este habilitado

    public void EsconderFlechasHabilidades()
    {
        for (int i = 0; i < 4; i++)
        {
            HabilidadesUI[i].transform.GetChild(0).gameObject.SetActive(false);
            HabilidadesUI[i].transform.GetChild(1).gameObject.SetActive(false);
        }

        HabilidadesUI[3].transform.GetChild(3).gameObject.SetActive(true);
        HabilidadesUI[4].transform.GetChild(1).gameObject.SetActive(true);
    }
    //Cosigue las habilidades Normales en Habilidad1
    public void checarHabilidades1()
    {
        EsconderFlechasHabilidades();
        HabilidadesMostrar.Clear();
        if (!PuedeSeleccionarH) return;
        for (int i = 0; i < datos.HabilidadesPermanentes.Length; i++)
        {
            if (datos.HabilidatPDesbloqueadas[i] && datos.HabilidadesPermanentes[i].Nombre != Habilidad2.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text)
            {
                if (datos.HabilidadesPermanentes[i].Arma == DatosArma.Nombre || datos.HabilidadesPermanentes[i].Arma == "Todos")
                {
                    if (datos.HabilidadesPermanentes[i].Tipo == "Normal" || datos.HabilidadesPermanentes[i].Tipo == "Pasiva")
                    {
                        HabilidadesMostrar.Add(datos.HabilidadesPermanentes[i]);
                        if (datos.HabilidadesPermanentes[i] == Habilidad1)
                        {
                            Habmostrar = HabilidadesMostrar.Count - 1;
                        }
                    }
                }
            }
        }
        if (HabilidadesMostrar.Count>1)
        {
            HabilidadesUI[0].transform.GetChild(0).gameObject.SetActive(true);
            HabilidadesUI[0].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    //Cosigue las habilidades Normales en Habilidad2
    public void checarHabilidades2()
    {
        EsconderFlechasHabilidades();
        HabilidadesMostrar.Clear();
        if (!PuedeSeleccionarH) return;
        for (int i = 0; i < datos.HabilidadesPermanentes.Length; i++)
        {
            if (datos.HabilidatPDesbloqueadas[i] && datos.HabilidadesPermanentes[i].Nombre != Habilidad1.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text)
            {
                if (datos.HabilidadesPermanentes[i].Arma == DatosArma.Nombre || datos.HabilidadesPermanentes[i].Arma == "Todos")
                {
                    if (datos.HabilidadesPermanentes[i].Tipo == "Normal" || datos.HabilidadesPermanentes[i].Tipo == "Pasiva")
                    {
                        HabilidadesMostrar.Add(datos.HabilidadesPermanentes[i]);
                        if (datos.HabilidadesPermanentes[i] == Habilidad2)
                        {
                            Habmostrar = HabilidadesMostrar.Count - 1;
                        }
                    }
                }
            }
        }
        if (HabilidadesMostrar.Count > 1)
        {
            HabilidadesUI[1].transform.GetChild(0).gameObject.SetActive(true);
            HabilidadesUI[1].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    //Cosigue las habilidades Normales en Ulti
    public void checarHabilidadeEspeciales()
    {
        EsconderFlechasHabilidades();
        HabilidadesMostrar.Clear();
        if (!PuedeSeleccionarH) return;
        for (int i = 0; i < datos.HabilidadesPermanentes.Length; i++)
        {
            if (datos.HabilidatPDesbloqueadas[i])
            {
                if (datos.HabilidadesPermanentes[i].Arma == DatosArma.Nombre || datos.HabilidadesPermanentes[i].Arma == "Todos")
                {
                    if (datos.HabilidadesPermanentes[i].Tipo == "Especial")
                    {
                        HabilidadesMostrar.Add(datos.HabilidadesPermanentes[i]);
                        if (datos.HabilidadesPermanentes[i] == HabilidadEspecial)
                        {
                            Habmostrar = HabilidadesMostrar.Count - 1;
                        }
                    }
                }
            }
        }
        if (HabilidadesMostrar.Count > 1)
        {
            HabilidadesUI[2].transform.GetChild(0).gameObject.SetActive(true);
            HabilidadesUI[2].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    //Cosigue las habilidades Normales en Gadgets
    public void checarHabilidadesTemporales()
    {
        EsconderFlechasHabilidades();
        HabilidadesMostrar.Clear();
        if (!PuedeSeleccionarH) return;
        if (HabilidadTemporal.transform.GetChild(2).gameObject.activeSelf) return;
        int habActual = 0;
        for (int i = 0; i < datos.HabilidadesTemporales.Length; i++)
        {
            if (datos.HabilidatTDesbloqueadas[i])
            {
                if (datos.HabilidadesTemporales[i].Arma == DatosArma.Nombre || datos.HabilidadesTemporales[i].Arma == "Todos")
                {
                    HabilidadesMostrar.Add(datos.HabilidadesTemporales[i]);
                    if (datos.HabilidadesTemporales[i] == HabilidadTemporal)
                    {
                        habActual = HabilidadesMostrar.Count - 1;
                    }
                }
            }
        }
        if (HabilidadTemporal.transform.GetChild(2).gameObject.activeSelf)
        {
            Habmostrar = habActual;
        }
        else
        {
            Habmostrar = 0;
        }
        if (HabilidadesMostrar.Count > 1)
        {
            HabilidadesUI[3].transform.GetChild(0).gameObject.SetActive(true);
            HabilidadesUI[3].transform.GetChild(1).gameObject.SetActive(true);
            HabilidadesUI[3].transform.GetChild(3).gameObject.SetActive(false);
            HabilidadesUI[4].transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    //cambia de seccion de lado Izquierda
    public void cambiarHabilidadIzq(int Seccion)
    {
        Habmostrar--;
        if (Habmostrar < 0)
        {
            Habmostrar= HabilidadesMostrar.Count - 1;
        }

        checarHabilidad(Seccion);
    }
    //cambia de seccion de lado derecha
    public void cambiarHabilidadDer(int Seccion)
    {
        Habmostrar++;
        if (Habmostrar > (HabilidadesMostrar.Count-1))
        {
            Habmostrar = 0;
        }
        checarHabilidad(Seccion);
    }
    public void checarHabilidad(int Seccion)
    {
        switch (Seccion)
        {
            case 0:
                Habilidad1.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = HabilidadesMostrar[Habmostrar].Icono;
                Habilidad1.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = HabilidadesMostrar[Habmostrar].Nombre;
                Hab1 = HabilidadesMostrar[Habmostrar];
                ControladorBatalla.Habilidad1 = HabilidadesMostrar[Habmostrar].Nombre;
                break;
            case 1:
                Habilidad2.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = HabilidadesMostrar[Habmostrar].Icono;
                Habilidad2.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = HabilidadesMostrar[Habmostrar].Nombre;
                Hab2 = HabilidadesMostrar[Habmostrar];
                ControladorBatalla.Habilidad2 = HabilidadesMostrar[Habmostrar].Nombre;
                break;
            case 2:
                HabilidadEspecial.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = HabilidadesMostrar[Habmostrar].Icono;
                HabilidadEspecial.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = HabilidadesMostrar[Habmostrar].Nombre;
                HabE = HabilidadesMostrar[Habmostrar];
                ControladorBatalla.HabilidadEspecial = HabilidadesMostrar[Habmostrar].Nombre;
                break;
            case 3:
                HabilidadTemporal.transform.GetChild(2).gameObject.SetActive(true);
                HabilidadTemporal.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = HabilidadesMostrar[Habmostrar].Icono;
                BarraHabilidadTemporal.text = $"< {HabilidadesMostrar[Habmostrar].Usos} / {HabilidadesMostrar[Habmostrar].Usos}";
                HabT = HabilidadesMostrar[Habmostrar];
                ControladorBatalla.HabilidadT = HabilidadesMostrar[Habmostrar].Nombre;
                break;
        }
        MostrarHabilidadSeleccionada(Seccion);
        //MostrarHabilidadLocal();

    }
    public void checarDesciciion(int Seccion)
    {
        switch (Seccion)
        {
            case 0:
                Secciones[2].transform.GetChild(2).gameObject.GetComponent<Image>().sprite = HabilidadesMostrar[Habmostrar].Icono;
                Secciones[2].transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = HabilidadesMostrar[Habmostrar].Nombre;
                break;
            case 1:
                //aqui potis
                break;
        }
        //MostrarHabilidadLocal();

    }
    public void MostrarPanelSeccion()
    {
        Secciones[0].SetActive(false);
        Secciones[1].SetActive(true);
        Secciones[2].SetActive(false);
        ChecarPociones();
    }

    public void ChecarPociones()
    {
        PocionIcono.GetComponent<Image>().sprite = null;
        ControladorBatalla.Pocion = "";
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);
        crearVacio();
        for (int i = 0; i < datos.Pociones.Length; i++)
        {
            if (datos.CantidadPociones[i] > 0)
            {
                GameObject obj = Instantiate(BotonPocion, contentPanel);
                obj.GetComponent<Image>().sprite = datos.Pociones[i].Icono;
                Boton_pocion poc = obj.GetComponent<Boton_pocion>();
                //poc.NombrePocion= datos.Pociones[i].Tipo.ToString();
                poc.No = i;
            }
        }
    }
    private void crearVacio()
    {
        GameObject obj = Instantiate(BotonPocion, contentPanel);
        obj.GetComponent<Image>().sprite = PocionVacio;
        Boton_pocion poc = obj.GetComponent<Boton_pocion>();
        //poc.NombrePocion= datos.Pociones[i].Tipo.ToString();
        poc.No = -1;
    }
    public void ChecarPocionActual()
    {
        foreach (Transform child in contentPanel)
        {
            child.gameObject.GetComponent<Boton_pocion>().Boton_Exit();
        }
    }
    public void MostrarPocion(int No)
    {
        MostrarDescipcion();
        if (No>=0)
        {
            Secciones[2].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = datos.Pociones[No].Nombre;
            Secciones[2].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = datos.Pociones[No].Descripcion;
        }
        else
        {
            Secciones[2].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Nada";
            Secciones[2].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "Ningina Pocion";
        }
    }

    public void IconoConsumible()
    {
        if(NoPocion >= 0)
        {
            PocionIcono.SetActive(true);
            PocionIcono.GetComponent<Image>().sprite = datos.Pociones[NoPocion].Icono;
            ControladorBatalla.Pocion = datos.Pociones[NoPocion].Nombre;
        }
        else
        {
            PocionIcono.SetActive(false);
            PocionIcono.GetComponent<Image>().sprite = null;
            ControladorBatalla.Pocion = "";
        }
    }
    public void MostrarHabilidades()
    {
        Secciones[0].SetActive(true);
        Secciones[1].SetActive(false);
        Secciones[2].SetActive(false);
    }
    public void MostrarDescipcion()
    {
        Secciones[2].SetActive(true);
        Arma.SetActive(false);
    }
    public void EsconderDescipcion()
    {
        Secciones[2].SetActive(false);
        Arma.SetActive(true);
    }
    public void MostrarHabilidadSeleccionada(int Seccion)
    {
        MostrarDescipcion();
        switch (Seccion)
        {
            case 0:
                Secciones[2].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = Hab1.Nombre;
                Secciones[2].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Hab1.Descripcion;
                break;
            case 1:
                Secciones[2].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = Hab2.Nombre;
                Secciones[2].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Hab2.Descripcion;
                break;
            case 2:
                Secciones[2].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = HabE.Nombre;
                Secciones[2].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = HabE.Descripcion;
                break;
            case 3:
                if (HabilidadTemporal.transform.GetChild(2).gameObject.activeSelf)
                {
                    Secciones[2].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = HabT.Nombre;
                    Secciones[2].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = HabT.Descripcion;
                }
                else
                {
                    EsconderDescipcion();
                }
                break;
        }
    }
    public void ConfirmarConsumible()
    {
        IconoConsumible();
        MostrarHabilidades();
    }
}
