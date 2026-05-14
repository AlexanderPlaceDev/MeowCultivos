using System;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] Image Rango;
    [SerializeField] Sprite[] Rangos;
    [Header("Habilidades")]//para acomodar visaulmente los iconos de las habilidades
    [SerializeField] Sprite IconoVacio;
    [SerializeField] GameObject HabilidadTemporal;
    [SerializeField] GameObject Habilidad1;
    [SerializeField] GameObject Habilidad2;
    [SerializeField] GameObject HabilidadEspecial;
    public bool newTem = false;
    public bool PuedeSeleccionarH = false;
    private bool TieneHabTemporal = false;
    private bool TieneFlechas = false;
    private bool TieneFlechasPociones = false;
    private int PocionMostrar = 0;
    [Header("Espacios Armas")]
    [SerializeField] GameObject[] EspaciosArmas;
    public List<Scr_CreadorHabilidadesBatalla> HabilidadesMostrar;
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
    public Scr_CreadorHabilidadesBatalla HabE;

    [Header("CambioHC")]//para acomodar visaulmente los iconos de las habilidades y comsimibles
    public int Habmostrar = 0;
    public GameObject[] HabilidadesUI;
    public GameObject[] Secciones;
    public bool PocionSelec = false;
    public GameObject PocionIcono;
    public Sprite PocionVacio;
    public int NoPocion = -2;
    private int GadgetMostrar = -1;

    private Tutorial_peleas Tutopeleas;
    void Start()
    {
        ControladorBatalla = GetComponent<Scr_ControladorBatalla>();
        BuscarSingleton();
        GameObject tu = GameObject.Find("Tutorial");
        if (tu != null)
        {
            Tutopeleas = tu.GetComponent<Tutorial_peleas>();
        }
        MostrarRango();
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
        CargarSeleccionGuardada();
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

    public void MostrarRango()
    {
        int e = PlayerPrefs.GetInt("Rango " + DatosArma.Nombre, 1) - 1;
        //Debug.Log(e);
        Rango.sprite = Rangos[e];
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
        int us = 0;

        if (ht != "Nada")
        {
            int index = datos.BuscarUSoHabilidadTemporalPorNombre(ht);
            us = datos.UsosHabilidadesT[index];
        }
        // Buscar habilidades usando el script de datos
        HabT = datos.BuscarHabilidadTemporalPorNombre(ht);
        Hab1 = datos.BuscarHabilidadPermanentePorNombre(h1);
        Hab2 = datos.BuscarHabilidadPermanentePorNombre(h2);
        HabE = datos.BuscarHabilidadPermanentePorNombre(hE);


        if (ht == "Nada")
        {
            HabilidadTemporal.transform.GetChild(2).gameObject.SetActive(false);
            HabilidadesUI[3].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "";
            TieneHabTemporal = false;
        }
        else
        {
            HabilidadTemporal.transform.GetChild(2).gameObject.SetActive(true);
            HabilidadTemporal.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = HabT.Icono;
            HabilidadesUI[3].transform.GetChild(3)
            .GetComponent<TextMeshProUGUI>().text =
             $"< {us} / 99";
            TieneHabTemporal = true;
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
        MostrarRango();
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
        MostrarRango();
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
        //if (!Tutopeleas.PuedeComenzar && Tutopeleas.isActiveAndEnabled) return;

        EsconderFlechasHabilidades();
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

        Debug.Log("Brinca");
        //Descontar Habilidades en el singleton
        if (newTem)
        {
            Debug.Log("aqui"+ht);
            //Aqui debo ajustar las cantidades de gadgets en el singleton
            datos.QuitarUsosTemporales(ht);

            int index = datos.BuscarUSoHabilidadTemporalPorNombre(ht);

            if (datos.UsosHabilidadesT[index] <= 0)
            {
                // Solo limpiar guardado para próximas selecciones
                PlayerPrefs.SetString("GadgetSeleccionado", "Nada");

                TieneHabTemporal = false;
                GadgetMostrar = -1;
            }
        }
        //Descontar pociones en el singleton
        if (NoPocion >= 0)
        {
            datos.CantidadPociones[NoPocion]--;

            if (datos.CantidadPociones[NoPocion] <= 0)
            {
                ControladorBatalla.Pocion = "";

                PlayerPrefs.SetString(
                    "PocionSeleccionada",
                    "");
            }
        }

        if (Tutopeleas != null && Tutopeleas.isActiveAndEnabled)
        {
            Tutopeleas.ComenzarPelea();
        }
        else
        {

            ControladorBatalla.IniciarCuentaRegresiva(false);
        }

        ControladorBatalla.ArmaActual = Armas[ArmaActual];
        ControladorBatalla.PrepararBatalla();
        GetComponent<Scr_ControladorArmas>().ArmaActual = ArmaActual;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void AceptarRecolecion()
    {
        EsconderFlechasHabilidades();
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
        if (Tutopeleas != null && Tutopeleas.isActiveAndEnabled)
        {
            Tutopeleas.ComenzarPelea();
        }
        //Descontar habilidades en el singleton
        if (newTem)
        {
            Debug.Log("Entra1");
            datos.QuitarUsosTemporales(ht);

            int index = datos.BuscarUSoHabilidadTemporalPorNombre(ht);

            if (datos.UsosHabilidadesT[index] <= 0)
            {
                // Solo limpiar guardado para próximas selecciones
                PlayerPrefs.SetString("GadgetSeleccionado", "Nada");

                TieneHabTemporal = false;
                GadgetMostrar = -1;
            }
        }
        //Descontar pociones en el singleton
        if (NoPocion >= 0)
        {
            datos.CantidadPociones[NoPocion]--;

            if (datos.CantidadPociones[NoPocion] <= 0)
            {
                ControladorBatalla.Pocion = "";

                PlayerPrefs.SetString(
                    "PocionSeleccionada",
                    "");
            }
        }
        ControladorBatalla.IniciarCuentaRegresiva(false);
        ControladorBatalla.ArmaActual = Armas[ArmaActual];
        GetComponent<Scr_ControladorArmas>().ArmaActual = ArmaActual;
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



    //****************************************
    //Seccion de Habiliades y Pociones

    //Cosigue las habilidades Disponibles Siempre Y cuando este habilitado



    public void EsconderFlechasHabilidades()
    {
        TieneFlechas = false;
        TieneFlechasPociones = false;

        ActualizarUIFlechas();
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
        if (HabilidadesMostrar.Count > 1)
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
        Debug.Log("Entró gadgets");

        HabilidadesMostrar.Clear();

        if (!PuedeSeleccionarH)
            return;

        // TOGGLE OFF
        if (TieneFlechas)
        {
            TieneFlechas = false;

            ActualizarUIFlechas();

            return;
        }

        // CERRAR POCIONES
        TieneFlechasPociones = false;

        for (int i = 0; i < datos.HabilidadesTemporales.Length; i++)
        {
            if (datos.UsosHabilidadesT[i] <= 0)
                continue;

            string armaHab =
                datos.HabilidadesTemporales[i]
                .Arma
                .Trim()
                .ToLower();

            bool compatible =
                armaHab.Contains(DatosArma.Nombre.ToLower())
                || armaHab == "todas";

            if (compatible)
            {
                HabilidadesMostrar.Add(
                    datos.HabilidadesTemporales[i]);
            }
        }

        Debug.Log("Total gadgets: " + HabilidadesMostrar.Count);

        // =========================
        // SINCRONIZAR CON GADGET ACTUAL
        // =========================

        Habmostrar = -1;

        if (TieneHabTemporal)
        {
            for (int i = 0; i < HabilidadesMostrar.Count; i++)
            {
                if (HabilidadesMostrar[i].Nombre ==
                    ControladorBatalla.HabilidadT)
                {
                    Habmostrar = i;
                    GadgetMostrar = i;
                    break;
                }
            }
        }
        else
        {
            GadgetMostrar = -1;
        }

        if (HabilidadesMostrar.Count >= 1)
        {
            TieneFlechas = true;
        }

        ActualizarUIFlechas();
    }

    //cambia de seccion de lado Izquierda
    public void cambiarHabilidadIzq(int Seccion)
    {
        if (Seccion != 3)
        {
            Habmostrar--;

            if (Habmostrar < 0)
            {
                Habmostrar = HabilidadesMostrar.Count - 1;
            }

            checarHabilidad(Seccion);
            return;
        }

        // GADGETS

        Habmostrar--;

        if (Habmostrar < -1)
        {
            Habmostrar = HabilidadesMostrar.Count - 1;
        }

        checarHabilidad(Seccion);
    }
    //cambia de seccion de lado derecha
    public void cambiarHabilidadDer(int Seccion)
    {
        if (Seccion != 3)
        {
            Habmostrar++;

            if (Habmostrar > (HabilidadesMostrar.Count - 1))
            {
                Habmostrar = 0;
            }

            checarHabilidad(Seccion);
            return;
        }

        // GADGETS

        Habmostrar++;

        if (Habmostrar > HabilidadesMostrar.Count - 1)
        {
            Habmostrar = -1;
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

                // =========================
                // NINGÚN GADGET
                // =========================

                if (Habmostrar == -1)
                {
                    HabilidadTemporal.transform.GetChild(2)
                        .gameObject.SetActive(false);

                    HabilidadesUI[3].transform.GetChild(3)
                        .GetComponent<TextMeshProUGUI>().text = "";

                    ControladorBatalla.HabilidadT = "Nada";

                    TieneHabTemporal = false;

                    GadgetMostrar = -1;

                    newTem = false;

                    GuardarSeleccionActual();

                    EsconderDescipcion();

                    ActualizarUIFlechas();

                    return;
                }

                // =========================
                // GADGET VÁLIDO
                // =========================

                HabilidadTemporal.transform.GetChild(2)
                    .gameObject.SetActive(true);

                HabilidadTemporal.transform.GetChild(2)
                    .GetComponent<Image>().sprite =
                    HabilidadesMostrar[Habmostrar].Icono;

                int index = datos.BuscarUSoHabilidadTemporalPorNombre(
                    HabilidadesMostrar[Habmostrar].Nombre);

                int usosActuales = datos.UsosHabilidadesT[index];

                HabilidadesUI[3].transform.GetChild(3)
                    .GetComponent<TextMeshProUGUI>().text =
                    $"< {usosActuales} / 99";

                HabT = HabilidadesMostrar[Habmostrar];

                ControladorBatalla.HabilidadT =
                    HabilidadesMostrar[Habmostrar].Nombre;

                GameObject.Find("Controlador").GetComponent<Scr_ControladorArmas>().EfectoTemp = HabT.Efecto;

                TieneHabTemporal = true;

                newTem = true;

                GadgetMostrar = Habmostrar;
                GuardarSeleccionActual();

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

    public void ChecarPociones()
    {
        // TOGGLE OFF
        if (TieneFlechasPociones)
        {
            TieneFlechasPociones = false;

            ActualizarUIFlechas();

            return;
        }

        // CERRAR GADGETS
        TieneFlechas = false;

        List<int> disponibles =
            ObtenerPocionesDisponibles();

        // =========================
        // SINCRONIZAR INDICE ACTUAL
        // =========================

        if (NoPocion == -1)
        {
            PocionMostrar = 0;
        }
        else
        {
            int indexActual =
                disponibles.IndexOf(NoPocion);

            if (indexActual >= 0)
            {
                PocionMostrar = indexActual;
            }
            else
            {
                PocionMostrar = 0;
            }
        }

        if (disponibles.Count > 1)
        {
            TieneFlechasPociones = true;
        }

        ActualizarUIFlechas();
    }

    private void ActualizarUIFlechas()
    {
        bool algunaSeleccionAbierta =
            TieneFlechas || TieneFlechasPociones;

        // =========================
        // GADGETS
        // =========================

        Transform gadget = HabilidadesUI[3].transform;

        // Flechas
        gadget.GetChild(0).gameObject.SetActive(TieneFlechas);
        gadget.GetChild(1).gameObject.SetActive(TieneFlechas);

        // Texto cantidad
        bool mostrarTextoGadget =
            TieneHabTemporal &&
            !algunaSeleccionAbierta;

        gadget.GetChild(3).gameObject.SetActive(
            mostrarTextoGadget);

        // =========================
        // POCIONES
        // =========================

        Transform pocion = HabilidadesUI[4].transform;

        // Flechas
        pocion.GetChild(0).gameObject.SetActive(
            TieneFlechasPociones);

        pocion.GetChild(1).gameObject.SetActive(
            TieneFlechasPociones);

        // Texto cantidad
        bool mostrarTextoPocion =
            NoPocion >= 0 &&
            !algunaSeleccionAbierta;

        pocion.GetChild(3).gameObject.SetActive(
            mostrarTextoPocion);
    }
    public void ChecarPocionActual()
    {
        foreach (Transform child in contentPanel)
        {
            Boton_pocion bot = child.gameObject.GetComponent<Boton_pocion>();
            if (bot != null)
            {
                bot.Boton_Exit();
            }

        }
    }

    private void SeleccionarPocion(int index)
    {
        NoPocion = index;

        // NADA
        if (index == -1)
        {
            HabilidadesUI[4].transform.GetChild(2)
                .GetComponent<Image>().sprite = PocionVacio;

            HabilidadesUI[4].transform.GetChild(3)
                .GetComponent<TextMeshProUGUI>().text =
                "";

            PocionIcono.SetActive(false);
            PocionIcono.GetComponent<Image>().sprite = null;

            ControladorBatalla.Pocion = "";

            GuardarSeleccionActual();

            Secciones[2].transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = "Nada";

            Secciones[2].transform.GetChild(1)
                .GetComponent<TextMeshProUGUI>().text =
                "Ninguna Pocion";

            return;
        }

        // ICONO UI
        HabilidadesUI[4].transform.GetChild(2)
            .GetComponent<Image>().sprite =
            datos.Pociones[index].Icono;

        // TEXTO UI
        HabilidadesUI[4].transform.GetChild(3)
            .GetComponent<TextMeshProUGUI>().text =
            $" {datos.CantidadPociones[index]} / 99 >";

        // ICONO GAMEPLAY
        PocionIcono.SetActive(true);

        PocionIcono.GetComponent<Image>().sprite =
            datos.Pociones[index].Icono;

        ControladorBatalla.Pocion =
            datos.Pociones[index].Nombre;

        // DESCRIPCION
        Secciones[2].transform.GetChild(0)
            .GetComponent<TextMeshProUGUI>().text =
            datos.Pociones[index].Nombre;

        Secciones[2].transform.GetChild(1)
            .GetComponent<TextMeshProUGUI>().text =
            datos.Pociones[index].Descripcion;
        GuardarSeleccionActual();
    }

    public void CambiarPocionIzq()
    {
        List<int> disponibles = ObtenerPocionesDisponibles();

        PocionMostrar--;

        if (PocionMostrar < 0)
        {
            PocionMostrar = disponibles.Count - 1;
        }

        SeleccionarPocion(disponibles[PocionMostrar]);
    }

    private List<int> ObtenerPocionesDisponibles()
    {
        List<int> disponibles = new List<int>();

        disponibles.Add(-1); // VACÍO

        for (int i = 0; i < datos.Pociones.Length; i++)
        {
            if (datos.CantidadPociones[i] > 0)
            {
                disponibles.Add(i);
            }
        }

        return disponibles;
    }

    public void CambiarPocionDer()
    {
        List<int> disponibles = ObtenerPocionesDisponibles();

        PocionMostrar++;

        if (PocionMostrar >= disponibles.Count)
        {
            PocionMostrar = 0;
        }

        SeleccionarPocion(disponibles[PocionMostrar]);
    }
    public void MostrarPocion(int No)
    {
        MostrarDescipcion();
        if (No >= 0)
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
        if (NoPocion >= 0)
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

    private void GuardarSeleccionActual()
    {
        // =========================
        // GADGET
        // =========================

        PlayerPrefs.SetString(
            "GadgetSeleccionado",
            ControladorBatalla.HabilidadT);

        // =========================
        // POCION
        // =========================

        PlayerPrefs.SetString(
         "PocionSeleccionada",
         string.IsNullOrEmpty(ControladorBatalla.Pocion)
          ? "Nada"
          : ControladorBatalla.Pocion);

        PlayerPrefs.Save();
    }

    private void CargarSeleccionGuardada()
    {
        // =========================
        // GADGET
        // =========================

        string gadgetGuardado =
            PlayerPrefs.GetString(
                "GadgetSeleccionado",
                "Nada");

        // RECONSTRUIR LISTA DE GADGETS
        HabilidadesMostrar.Clear();

        for (int i = 0; i < datos.HabilidadesTemporales.Length; i++)
        {
            if (datos.UsosHabilidadesT[i] <= 0)
                continue;

            string armaHab =
                datos.HabilidadesTemporales[i]
                .Arma
                .Trim()
                .ToLower();

            bool compatible =
                armaHab.Contains(DatosArma.Nombre.ToLower())
                || armaHab == "todas";

            if (compatible)
            {
                HabilidadesMostrar.Add(
                    datos.HabilidadesTemporales[i]);
            }
        }

        if (gadgetGuardado != "Nada")
        {
            int index =
                datos.BuscarUSoHabilidadTemporalPorNombre(
                    gadgetGuardado);

            // EXISTE Y TIENE USOS
            if (index >= 0 &&
                datos.UsosHabilidadesT[index] > 0)
            {
                HabT =
                    datos.BuscarHabilidadTemporalPorNombre(
                        gadgetGuardado);

                if (HabT != null)
                {
                    HabilidadTemporal.transform
                        .GetChild(2).gameObject.SetActive(true);

                    HabilidadTemporal.transform
                        .GetChild(2)
                        .GetComponent<Image>().sprite =
                        HabT.Icono;

                    HabilidadesUI[3].transform.GetChild(3)
                        .GetComponent<TextMeshProUGUI>().text =
                        $"< {datos.UsosHabilidadesT[index]} / 99";

                    TieneHabTemporal = true;

                    // IMPORTANTE
                    HabT = datos.BuscarHabilidadTemporalPorNombre(gadgetGuardado);

                    // ESTA ES LA QUE TE FALTABA
                    ControladorBatalla.HabilidadT = HabT.Nombre;

                    Scr_ControladorArmas controladorArmas= GameObject.Find("Controlador").GetComponent<Scr_ControladorArmas>();

                    Debug.Log("cargando:"+controladorArmas.EfectoTemp);
                    newTem = true;

                    for (int j = 0; j < HabilidadesMostrar.Count; j++)
                    {
                        if (HabilidadesMostrar[j].Nombre ==
                            gadgetGuardado)
                        {
                            GadgetMostrar = j;
                            Habmostrar = j;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            GadgetMostrar = -1;
            Habmostrar = -1;
        }

        // =========================
        // POCION
        // =========================

        string pocionGuardada =
            PlayerPrefs.GetString(
                "PocionSeleccionada",
                "Nada");

        if (pocionGuardada != "Nada")
        {
            for (int i = 0; i < datos.Pociones.Length; i++)
            {
                if (datos.Pociones[i].Nombre ==
                    pocionGuardada)
                {
                    if (datos.CantidadPociones[i] > 0)
                    {
                        SeleccionarPocion(i);

                        // =========================
                        // SINCRONIZAR INDICE
                        // =========================

                        List<int> disponibles =
                            ObtenerPocionesDisponibles();

                        PocionMostrar =
                            disponibles.IndexOf(i);
                    }

                    break;
                }
            }
        }
        else
        {
            PocionMostrar = 0;
            NoPocion = -1;
        }

        ActualizarUIFlechas();
    }
}
