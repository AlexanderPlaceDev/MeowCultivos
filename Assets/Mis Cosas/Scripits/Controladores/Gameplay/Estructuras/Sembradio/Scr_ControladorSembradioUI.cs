using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_ControladorSembradioUI : MonoBehaviour
{
    [SerializeField] int ID;
    [SerializeField] Scr_CreadorObjetos[] ObjetosQuePlanta;
    [SerializeField] Image[] Iconos;
    [SerializeField] Sprite[] Sprites;
    [SerializeField] Material[] MaterialesAbono;
    [SerializeField] GameObject ObjetoPlanta;
    [SerializeField] Scr_BarrilSembradio Barril;
    [SerializeField] GameObject Abono;
    [SerializeField] GameObject CanvasIcono;
    [SerializeField] Image Semilla;
    [SerializeField] Image Producto;
    [SerializeField] GameObject[] Botones;
    [SerializeField] Color ColorBoton;
    [SerializeField] bool Regado;
    [SerializeField] bool Abonado;
    [SerializeField] bool AbonadoPlus;
    [SerializeField] int ReduccionAbono = 1;
    [SerializeField] int ReduccionAbonoPlus = 2;

    private TextMeshProUGUI TextoTiempo;
    Scr_Inventario Inventario;

    public Scr_CreadorObjetos SemillaPlantada = null;
    int SemillaActual = 0;
    int DiasPlantado = 0;
    string diaAnterior = "";
    Sprite Vacio;




    void Start()
    {
        Vacio = Producto.GetComponent<Image>().sprite;
        Inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();
        DiasPlantado = PlayerPrefs.GetInt("DiasPlantado:" + ID, 0);

        string diaActual = GameObject.Find("Controlador Tiempo")
            .GetComponent<Scr_ControladorTiempo>()
            .DiaActual;

        diaAnterior = PlayerPrefs.GetString(
            "DiaAnterior:" + ID,
            diaActual
        );


        if (PlayerPrefs.GetString("Plantado" + ID, "No") == "Si")
        {
            SemillaActual = PlayerPrefs.GetInt("SemillaPlantada" + ID, 0);
            Producto.sprite = ObjetosQuePlanta[SemillaActual].Icono;
            SemillaPlantada = ObjetosQuePlanta[SemillaActual];

        }

        if (PlayerPrefs.GetString("SembradioRegado:" + ID, "No") == "Si")
        {
            Regado = true;
            Abono.GetComponent<MeshRenderer>().material = MaterialesAbono[1];
        }

        if (PlayerPrefs.GetString("SembradioAbonado:" + ID, "No") == "Si")
        {
            Abonado = true;

            if (Regado)
            {
                Abono.GetComponent<MeshRenderer>().material = MaterialesAbono[4];
            }
            else
            {
                Abono.GetComponent<MeshRenderer>().material = MaterialesAbono[2];
            }
        }

        if (PlayerPrefs.GetString("SembradioAbonadoPlus:" + ID, "No") == "Si")
        {
            AbonadoPlus = true;
            if (Regado)
            {
                Abono.GetComponent<MeshRenderer>().material = MaterialesAbono[5];
            }
            else
            {
                Abono.GetComponent<MeshRenderer>().material = MaterialesAbono[2];
            }
        }

        CanvasIcono.SetActive(false);

        if (CanvasIcono.transform.childCount >= 2)
        {
            TextoTiempo = CanvasIcono.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        }

        if (Regado)
        {
            Iconos[0].sprite = Sprites[1];
            Botones[0].SetActive(false);
        }
        if (Abonado || AbonadoPlus)
        {
            Iconos[1].sprite = Sprites[1];
            Botones[1].SetActive(false);
            Botones[2].SetActive(false);
        }

        ActualizarCanvas();
        ActualizarModeloPlanta();
    }

    // Update is called once per frame
    void Update()
    {
        // =========================
        // ACTUALIZAR CANVAS
        // =========================

        ActualizarCanvas();


        // =========================
        // MENU DEL SEMBRADIO
        // =========================

        if (GetComponent<Scr_ActivadorMenuEstructuraFijo>().EstaDentro)
        {
            // =========================
            // ACTUALIZAR BOTONES
            // =========================

            ActualizarBotones();


            // =========================
            // ACTUALIZAR ICONO DE SEMILLA
            // =========================

            Semilla.sprite = ObjetosQuePlanta[SemillaActual].Icono;

            int posicion = 0;

            foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
            {
                if (Item == ObjetosQuePlanta[SemillaActual])
                {
                    Semilla.transform.GetChild(0)
                        .GetComponent<TextMeshProUGUI>()
                        .text = Inventario.Cantidades[posicion].ToString();

                    break;
                }

                posicion++;
            }


            // =========================
            // MOSTRAR / OCULTAR OPCIONES DE PLANTAR
            // =========================

            if (SemillaPlantada == null)
            {
                transform.GetChild(1).GetChild(6).gameObject.SetActive(true);
                transform.GetChild(1).GetChild(7).gameObject.SetActive(true);
                transform.GetChild(1).GetChild(8).gameObject.SetActive(true);
                transform.GetChild(1).GetChild(11).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(1).GetChild(6).gameObject.SetActive(false);
                transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
                transform.GetChild(1).GetChild(8).gameObject.SetActive(false);
                transform.GetChild(1).GetChild(11).gameObject.SetActive(false);
            }


            // =========================
            // OTRAS OPCIONES DEL TABLERO
            // =========================

            transform.GetChild(1).GetChild(9).gameObject.SetActive(true);
            transform.GetChild(1).GetChild(10).gameObject.SetActive(true);
        }
        else
        {
            // =========================
            // OCULTAR TODO AL SALIR
            // =========================

            Botones[0].SetActive(false);
            Botones[1].SetActive(false);
            Botones[2].SetActive(false);

            transform.GetChild(1).GetChild(6).gameObject.SetActive(false);
            transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
            transform.GetChild(1).GetChild(8).gameObject.SetActive(false);
            transform.GetChild(1).GetChild(9).gameObject.SetActive(false);
            transform.GetChild(1).GetChild(10).gameObject.SetActive(false);
            transform.GetChild(1).GetChild(11).gameObject.SetActive(false);
        }


        // =========================
        // ACTUALIZAR CRECIMIENTO
        // =========================

        ActualizarPlanta();
    }

    private void ActualizarPlanta()
    {
        string diaActual = GameObject.Find("Controlador Tiempo")
            .GetComponent<Scr_ControladorTiempo>()
            .DiaActual;

        // Si todavía estamos en el mismo día, no hacemos nada
        if (diaActual == diaAnterior)
            return;

        if (SemillaPlantada != null)
        {
            DiasPlantado++;

            PlayerPrefs.SetInt(
                "DiasPlantado:" + ID,
                DiasPlantado
            );

            int TiempoCrecimiento = ObtenerTiempoCrecimiento();

            // ACTUALIZAR EL MODELO
            ActualizarModeloPlanta();

            // =========================
            // TERMINÓ DE CRECER
            // =========================

            if (DiasPlantado >= TiempoCrecimiento)
            {
                if (Regado && Barril.Cantidad == 0)
                {
                    Barril.TipoFruta = SemillaPlantada;

                    int cantidad = Random.Range(
                        SemillaPlantada.MinimoMaximoSembradio[0],
                        SemillaPlantada.MinimoMaximoSembradio[1] + 1
                    );

                    Barril.Cantidad = Mathf.Min(
                        cantidad,
                        Barril.CantidadMaxima
                    );

                    Barril.UltimoDiaPlanta = true;

                    Barril.GuardarEstado();
                    
                }
            }

            ActualizarCanvas();
        }

        // Guardar que este día ya fue procesado
        diaAnterior = diaActual;

        PlayerPrefs.SetString(
            "DiaAnterior:" + ID,
            diaAnterior
        );

        PlayerPrefs.Save();
    }

    public void CosecharPlanta()
    {
        Debug.Log("Sembradio: reiniciando planta");

        // =========================
        // REINICIAR ESTADO
        // =========================

        Regado = false;
        Abonado = false;
        AbonadoPlus = false;

        DiasPlantado = 0;

        // MUY IMPORTANTE:
        // quitar la semilla
        SemillaPlantada = null;

        // =========================
        // REINICIAR BARRIL
        // =========================

        if (Barril != null)
        {
            Barril.UltimoDiaPlanta = false;
        }

        // =========================
        // PLAYER PREFS
        // =========================

        PlayerPrefs.DeleteKey("SembradioRegado:" + ID);
        PlayerPrefs.DeleteKey("SembradioAbonado:" + ID);
        PlayerPrefs.DeleteKey("SembradioAbonadoPlus:" + ID);
        PlayerPrefs.DeleteKey("Plantado" + ID);
        PlayerPrefs.DeleteKey("SemillaPlantada" + ID);
        PlayerPrefs.DeleteKey("DiasPlantado:" + ID);

        // Guardamos el día actual para evitar
        // que vuelva a procesar crecimiento inmediatamente
        string diaActual = GameObject.Find("Controlador Tiempo")
            .GetComponent<Scr_ControladorTiempo>()
            .DiaActual;

        diaAnterior = diaActual;

        PlayerPrefs.SetString(
            "DiaAnterior:" + ID,
            diaAnterior
        );

        PlayerPrefs.Save();

        // =========================
        // UI
        // =========================

        Iconos[0].sprite = Sprites[0];
        Iconos[1].sprite = Sprites[0];

        Producto.sprite = Vacio;

        if (Abono != null)
        {
            Abono.GetComponent<MeshRenderer>().material =
                MaterialesAbono[0];
        }

        // =========================
        // DESACTIVAR TODAS LAS PLANTAS
        // =========================

        foreach (Transform Planta in ObjetoPlanta.transform)
        {
            // Desactivar todas las fases
            for (int i = 0; i < Planta.childCount; i++)
            {
                Planta.GetChild(i).gameObject.SetActive(false);
            }

            // Desactivar el objeto principal de la planta
            Planta.gameObject.SetActive(false);
        }

        // =========================
        // ACTUALIZAR SISTEMA
        // =========================

        ActualizarBotones();
        ActualizarCanvas();

        Debug.Log("Sembradio: reinicio terminado");
    }


    public void BotonRegar()
    {
        if (SemillaPlantada == null)
            return;

        if (Regado)
            return;

        if (PlayerPrefs.GetInt("CantidadAgua", 0) >= 2)
        {
            Regado = true;

            PlayerPrefs.SetInt(
                "CantidadAgua",
                PlayerPrefs.GetInt("CantidadAgua", 0) - 2
            );

            PlayerPrefs.SetString("SembradioRegado:" + ID, "Si");

            Iconos[0].sprite = Sprites[1];

            Abono.GetComponent<MeshRenderer>().material = MaterialesAbono[1];

            if (Abonado)
            {
                Abono.GetComponent<MeshRenderer>().material = MaterialesAbono[4];
            }

            if (AbonadoPlus)
            {
                Abono.GetComponent<MeshRenderer>().material = MaterialesAbono[5];
            }

            ActualizarBotones();
            ActualizarCanvas();
        }
    }

    public void EntraBoton(string ID)
    {
        // Convertir el segundo carácter a un número entero
        int index = (int)char.GetNumericValue(ID[1]);

        if (ID[0] == '1')
        {
            Botones[index].GetComponent<Image>().color = Color.white;
            Botones[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            Botones[index].GetComponent<Image>().color = ColorBoton;
            Botones[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = ColorBoton;
        }
    }

    public void BotonAbonar()
    {
        if (SemillaPlantada == null) return;
        if (Abonado || AbonadoPlus) return;

        // FIX: validar que quede mínimo 1 día pendiente
        int tiempoRestante = ObtenerTiempoCrecimiento() - DiasPlantado;
        if (tiempoRestante < 1)
            return;

        int posicion = 0;
        foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
        {
            if (Item.Nombre == "Abono" && Inventario.Cantidades[posicion] > 0)
            {
                Abonado = true;
                Inventario.Cantidades[posicion]--;
                PlayerPrefs.SetString("SembradioAbonado:" + ID, "Si");
                PlayerPrefs.DeleteKey("SembradioAbonadoPlus:" + ID);
                Iconos[1].sprite = Sprites[1];
                Abono.GetComponent<MeshRenderer>().material = Regado ? MaterialesAbono[4] : MaterialesAbono[2];
                ActualizarBotones();
                ActualizarCanvas();
                break;
            }
            posicion++;
        }
    }

    public void BotonAbonarPlus()
    {
        if (SemillaPlantada == null) return;
        if (Abonado || AbonadoPlus) return;

        // FIX: validar que queden mínimo 2 días pendientes
        int tiempoRestante = ObtenerTiempoCrecimiento() - DiasPlantado;
        if (tiempoRestante < 2)
            return;

        int posicion = 0;
        foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
        {
            if (Item.Nombre == "Abono+" && Inventario.Cantidades[posicion] > 0)
            {
                AbonadoPlus = true;
                Inventario.Cantidades[posicion]--;
                PlayerPrefs.SetString("SembradioAbonadoPlus:" + ID, "Si");
                PlayerPrefs.DeleteKey("SembradioAbonado:" + ID);
                Iconos[1].sprite = Sprites[1];
                Abono.GetComponent<MeshRenderer>().material = Regado ? MaterialesAbono[5] : MaterialesAbono[3];
                ActualizarBotones();
                ActualizarCanvas();
                break;
            }
            posicion++;
        }
    }

    public void BotonCerrar()
    {
        Debug.Log("Entra");
        GetComponent<Scr_ActivadorMenuEstructuraFijo>().CerrarTablero();
    }

    public void BotonBasura()
    {
        // Reiniciamos todo el estado del sembradio y eliminamos la semilla plantada
        Regado = false;
        Abonado = false;
        AbonadoPlus = false;
        Iconos[0].sprite = Sprites[0];
        Iconos[1].sprite = Sprites[0];
        if (Barril != null)
        {
            Barril.ReiniciarBarril();
        }
        PlayerPrefs.DeleteKey("SembradioRegado:" + ID);
        PlayerPrefs.DeleteKey("SembradioAbonadoPlus:" + ID);
        PlayerPrefs.DeleteKey("SembradioAbonado:" + ID);
        PlayerPrefs.DeleteKey("Plantado" + ID);
        PlayerPrefs.DeleteKey("SemillaPlantada" + ID);
        SemillaPlantada = null; // Aquí sí eliminamos la referencia a la semilla plantada
        Producto.sprite = Vacio;
        Abono.GetComponent<MeshRenderer>().material = MaterialesAbono[0];

        // Reiniciamos visualmente la planta
        foreach (Transform Planta in ObjetoPlanta.transform)
        {
            for (int i = 0; i < Planta.childCount; i++)
            {
                Planta.GetChild(i).gameObject.SetActive(false);
            }

            Planta.gameObject.SetActive(false);
        }

        DiasPlantado = 0;
        PlayerPrefs.DeleteKey("DiasPlantado:" + ID);
        PlayerPrefs.DeleteKey("DiaAnterior:" + ID);

        ActualizarBotones();
        ActualizarCanvas();
    }


    public void FlechaDerecha()
    {
        SemillaActual = (SemillaActual + 1) % ObjetosQuePlanta.Length;
    }

    public void FlechaIzquierda()
    {
        SemillaActual = (SemillaActual == 0) ? ObjetosQuePlanta.Length - 1 : SemillaActual - 1;
    }

    public void BotonPlantar()
    {
        int posicion = 0;

        foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
        {
            if (Item == ObjetosQuePlanta[SemillaActual])
            {
                if (Inventario.Cantidades[posicion] > 0)
                {
                    Inventario.Cantidades[posicion]--;

                    SemillaPlantada = Item;

                    Producto.sprite = Item.Icono;

                    DiasPlantado = 0;

                    string diaActual = GameObject.Find("Controlador Tiempo")
                    .GetComponent<Scr_ControladorTiempo>()
                    .DiaActual;

                    diaAnterior = diaActual;

                    PlayerPrefs.SetString(
                        "DiaAnterior:" + ID,
                        diaAnterior
                    );

                    Regado = false;
                    Abonado = false;
                    AbonadoPlus = false;

                    PlayerPrefs.SetString("Plantado" + ID, "Si");
                    PlayerPrefs.SetInt("SemillaPlantada" + ID, SemillaActual);

                    PlayerPrefs.DeleteKey("SembradioRegado:" + ID);
                    PlayerPrefs.DeleteKey("SembradioAbonado:" + ID);
                    PlayerPrefs.DeleteKey("SembradioAbonadoPlus:" + ID);

                    PlayerPrefs.SetInt("DiasPlantado:" + ID, 0);
                    ActualizarModeloPlanta();

                    ActualizarBotones();
                    ActualizarCanvas();
                }
            }

            posicion++;
        }
    }


    private void ActualizarModeloPlanta()
    {
        if (SemillaPlantada == null)
            return;
        Debug.Log("No null");
        foreach (Transform Planta in ObjetoPlanta.transform)
        {
            // No es la planta que está sembrada
            if (Planta.name != SemillaPlantada.TipoPlanta)
            {
                Planta.gameObject.SetActive(false);
                continue;
            }

            // Es la planta correcta
            Planta.gameObject.SetActive(true);

            int cantidadEtapas = Planta.childCount;

            if (cantidadEtapas == 0)
                return;

            /*
             * DiasPlantado = 0 -> primera etapa
             * DiasPlantado = 1 -> primera etapa
             * DiasPlantado = 2 -> segunda etapa
             * DiasPlantado = 3 -> tercera etapa
             */

            int etapaActual = Mathf.Clamp(
                DiasPlantado - 1,
                0,
                cantidadEtapas - 1
            );

            if (Abonado)
            {
                etapaActual++;
            }
            else
            {
                if (AbonadoPlus)
                {
                    etapaActual++;
                    etapaActual++;
                }
            }
            // Cuando llega al final, mantenemos la última etapa
            if (DiasPlantado >= cantidadEtapas)
            {
                etapaActual = cantidadEtapas - 1;
            }

            for (int i = 0; i < cantidadEtapas; i++)
            {
                Planta.GetChild(i).gameObject.SetActive(
                    i == etapaActual
                );
            }

            Debug.Log(
                "Planta: " + Planta.name +
                " | Dias plantado: " + DiasPlantado +
                " | Etapa mostrada: " + etapaActual
            );

            break;
        }
    }
    private void ActualizarBotones()
    {
        // Si no hay semilla, no se puede hacer nada
        if (SemillaPlantada == null)
        {
            Botones[0].SetActive(false);
            Botones[1].SetActive(false);
            Botones[2].SetActive(false);
            return;
        }

        // =========================
        // REGAR
        // =========================
        if (!Regado && PlayerPrefs.GetInt("CantidadAgua", 0) >= 2)
        {
            Botones[0].SetActive(true);
            Botones[0].transform.GetChild(2)
               .GetComponent<TextMeshProUGUI>()
               .text = PlayerPrefs.GetInt("CantidadAgua", 0).ToString();
        }
        else
        {
            Botones[0].SetActive(false);
        }

        // =========================
        // ABONO NORMAL
        // =========================
        bool TieneAbono = false;
        bool TieneAbonoPlus = false;

        int posicion = 0;
        foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
        {
            if (Item.Nombre == "Abono" && Inventario.Cantidades[posicion] > 0)
                TieneAbono = true;

            if (Item.Nombre == "Abono+" && Inventario.Cantidades[posicion] > 0)
                TieneAbonoPlus = true;

            posicion++;
        }

        // Calculamos cuanto le queda ANTES de abonar
        int tiempoRestanteActual = ObtenerTiempoCrecimiento() - DiasPlantado;

        // Solo se puede abonar si queda minimo 2 dia y no tiene ya abono
        if (!Abonado && !AbonadoPlus && TieneAbono && tiempoRestanteActual >= 2)
        {
            Botones[1].SetActive(true);
            posicion = 0;
            foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
            {
                if (Item.Nombre == "Abono")
                {
                    Botones[1].transform.GetChild(2)
                       .GetComponent<TextMeshProUGUI>()
                       .text = Inventario.Cantidades[posicion].ToString();
                    break;
                }
                posicion++;
            }
        }
        else
        {
            Botones[1].SetActive(false);
        }

        // =========================
        // ABONO+
        // =========================
        // Solo se puede abonar plus si quedan minimo 3 dias
        if (!Abonado && !AbonadoPlus && TieneAbonoPlus && tiempoRestanteActual >= 3)
        {
            Botones[2].SetActive(true);
            posicion = 0;
            foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
            {
                if (Item.Nombre == "Abono+")
                {
                    Botones[2].transform.GetChild(2)
                       .GetComponent<TextMeshProUGUI>()
                       .text = Inventario.Cantidades[posicion].ToString();
                    break;
                }
                posicion++;
            }
        }
        else
        {
            Botones[2].SetActive(false);
        }
    }
    private void ActualizarCanvas()
    {
        if (CanvasIcono == null)
            return;

        // No hay semilla
        if (SemillaPlantada == null)
        {
            CanvasIcono.SetActive(false);
            return;
        }

        // Hay una semilla
        CanvasIcono.SetActive(true);

        // No regado
        if (!Regado)
        {
            CanvasIcono.transform.GetChild(0).gameObject.SetActive(true);
            CanvasIcono.transform.GetChild(1).gameObject.SetActive(false);
        }
        // Regado
        else
        {
            CanvasIcono.transform.GetChild(0).gameObject.SetActive(false);
            CanvasIcono.transform.GetChild(1).gameObject.SetActive(true);
        }

        ActualizarTiempoRestante();
    }

    private void ActualizarTiempoRestante()
    {
        if (SemillaPlantada == null || TextoTiempo == null)
            return;

        int TiempoTotal = ObtenerTiempoCrecimiento();

        int TiempoRestante = Mathf.Max(0, TiempoTotal - DiasPlantado);

        if (TiempoRestante <= 0)
        {
            TextoTiempo.text = "Listo";
        }
        else if (TiempoRestante == 1)
        {
            TextoTiempo.text = "1 dia";
        }
        else
        {
            TextoTiempo.text = TiempoRestante + " dias";
        }
    }

    private int ObtenerTiempoCrecimiento()
    {
        int TiempoBase = 0;

        foreach (Transform Planta in ObjetoPlanta.GetComponentInChildren<Transform>())
        {
            if (Planta.name == SemillaPlantada.TipoPlanta)
            {
                TiempoBase = Planta.childCount;
                break;
            }
        }

        if (Abonado)
            TiempoBase -= ReduccionAbono;

        if (AbonadoPlus)
            TiempoBase -= ReduccionAbonoPlus;

        return Mathf.Max(1, TiempoBase);
    }
}
