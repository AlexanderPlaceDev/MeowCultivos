using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
    [SerializeField] Image Semilla;
    [SerializeField] Image Producto;
    [SerializeField] GameObject[] Botones;
    [SerializeField] Color ColorBoton;
    [SerializeField] bool Regado;
    [SerializeField] bool Abonado;
    [SerializeField] bool AbonadoPlus;
    Scr_Inventario Inventario;

    public Scr_CreadorObjetos SemillaPlantada = null;
    int SemillaActual = 0;
    int DiasPlantado = 0;
    string diaAnterior = "";
    Sprite Vacio;
    void Start()
    {
        Vacio = Producto.GetComponent<Image>().sprite;
        Inventario = GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_Inventario>();
        DiasPlantado = PlayerPrefs.GetInt("DiasPlantado:" + ID, 0);
        diaAnterior = PlayerPrefs.GetString("DiaAnterior:" + ID, "LUN");

        if (PlayerPrefs.GetString("Plantado" + ID, "No") == "Si")
        {
            SemillaActual = PlayerPrefs.GetInt("SemillaPlantada" + ID, 0);
            Producto.sprite = ObjetosQuePlanta[SemillaActual].Icono;
            SemillaPlantada = ObjetosQuePlanta[SemillaActual];

        }

        if (PlayerPrefs.GetString("SembradioRegado:" + ID, "No") == "Si")
        {
            Regado = true;
            Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[1];
        }

        if (PlayerPrefs.GetString("SembradioAbonado:" + ID, "No") == "Si")
        {
            Abonado = true;

            if (Regado)
            {
                Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[4];
            }
            else
            {
                Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[2];
            }
        }

        if (PlayerPrefs.GetString("SembradioAbonadoPlus:" + ID, "No") == "Si")
        {
            AbonadoPlus = true;
            if (Regado)
            {
                Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[5];
            }
            else
            {
                Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[2];
            }
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
        if (!GetComponent<Scr_ActivadorMenuEstructuraFijo>().EstaDentro)
        {
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

        ActualizarPlantaInicial();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Scr_ActivadorMenuEstructuraFijo>().EstaDentro)
        {
            if (!Regado && !Botones[0].activeSelf && PlayerPrefs.GetInt("CantidadAgua", 0) >= 2 && SemillaPlantada == null)
            {
                Botones[0].SetActive(true);
                Botones[0].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("CantidadAgua", 0).ToString();
            }
            int posicion = 0;
            foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
            {
                if (Item.Nombre == "Abono")
                {
                    if (Inventario.Cantidades[posicion] > 0 && SemillaPlantada == null)
                    {
                        if (!Abonado && !AbonadoPlus && !Botones[1].activeSelf)
                        {
                            Botones[1].SetActive(true);
                            Botones[1].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Inventario.Cantidades[posicion].ToString();
                        }
                    }
                }
                posicion++;
            }
            posicion = 0;
            foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
            {
                if (Item.Nombre == "Abono+")
                {
                    if (Inventario.Cantidades[posicion] > 0 && SemillaPlantada == null)
                    {
                        if (!Abonado && !AbonadoPlus && !Botones[2].activeSelf)
                        {
                            Botones[2].SetActive(true);
                            Botones[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Inventario.Cantidades[posicion].ToString();
                        }
                    }
                }
                posicion++;
            }

            Semilla.sprite = ObjetosQuePlanta[SemillaActual].Icono;
            posicion = 0;
            foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
            {
                if (Item == ObjetosQuePlanta[SemillaActual])
                {
                    Semilla.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Inventario.Cantidades[posicion].ToString();

                }
                posicion++;
            }

            if (SemillaPlantada == null)
            {
                transform.GetChild(1).GetChild(6).gameObject.SetActive(true);
                transform.GetChild(1).GetChild(7).gameObject.SetActive(true);
                transform.GetChild(1).GetChild(8).gameObject.SetActive(true);
                transform.GetChild(1).GetChild(11).gameObject.SetActive(true);
            }


            transform.GetChild(1).GetChild(9).gameObject.SetActive(true);
            transform.GetChild(1).GetChild(10).gameObject.SetActive(true);

        }
        else
        {
            if (Botones[0].activeSelf || Botones[1].activeSelf || Botones[2].activeSelf || transform.GetChild(1).GetChild(6).gameObject.activeSelf || transform.GetChild(1).GetChild(10).gameObject.activeSelf)
            {
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
        }

        ActualizarPlanta();
    }

    private void ActualizarPlanta()
    {
        string diaActual = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().DiaActual;

        // Verificamos si ha cambiado el día
        if (diaActual != diaAnterior)
        {
            if (SemillaPlantada != null)
            {
                // Incrementamos los días plantados solo si la semilla está plantada
                DiasPlantado++;
                PlayerPrefs.SetInt("DiasPlantado:" + ID, DiasPlantado);

                // Recorremos los transform hijos que representan las etapas de crecimiento de la planta
                foreach (Transform Planta in ObjetoPlanta.GetComponentInChildren<Transform>())
                {
                    // Verificamos si el nombre de la planta coincide con la semilla plantada
                    if (Planta.name == SemillaPlantada.TipoPlanta)
                    {

                        // Si aún no se ha completado el ciclo de crecimiento
                        if (DiasPlantado <= Planta.childCount)
                        {
                            // Activamos la planta principal y solo la etapa correspondiente
                            Planta.gameObject.SetActive(true);

                            for (int i = 0; i < Planta.childCount; i++)
                            {
                                // Activar solo la etapa de crecimiento correspondiente al día actual
                                Planta.GetChild(i).gameObject.SetActive(i == DiasPlantado - 1);
                            }
                        }
                        else
                        {
                            // Si se ha completado el ciclo de crecimiento, procedemos a la recolección de fruta
                            if (Regado && Barril.Cantidad == 0)
                            {
                                // Lógica de recolección de fruta
                                Barril.TipoFruta = SemillaPlantada;
                                int cantidad = Random.Range(SemillaPlantada.MinimoMaximoSembradio[0], SemillaPlantada.MinimoMaximoSembradio[1]);

                                if (Abonado) cantidad *= 2;
                                if (AbonadoPlus) cantidad *= 3;

                                Barril.Cantidad = Mathf.Min(Barril.Cantidad + cantidad, Barril.CantidadMaxima);
                            }

                            BotonBasura();
                        }
                        //En caso de ser la ultima face
                        if (DiasPlantado == Planta.childCount)
                        {
                            if (Barril.TipoFruta != null)
                            {
                                Barril.UltimoDiaPlanta = true;
                            }
                        }
                        // Detenemos el bucle tras procesar la planta correcta
                        break;
                    }
                }
            }

            // Actualizamos el día anterior en PlayerPrefs
            diaAnterior = diaActual;
            PlayerPrefs.SetString("DiaAnterior:" + ID, diaAnterior);
        }
    }




    public void BotonRegar()
    {
        if (PlayerPrefs.GetInt("CantidadAgua", 0) >= 2)
        {
            Regado = true;
            Botones[0].SetActive(false);
            Iconos[0].sprite = Sprites[1];
            PlayerPrefs.SetInt("CantidadAgua", PlayerPrefs.GetInt("CantidadAgua", 0) - 2);
            PlayerPrefs.SetString("SembradioRegado:" + ID, "Si");

            Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[1];
            if (Abonado)
            {
                Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[4];
            }
            if (AbonadoPlus)
            {
                Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[5];
            }
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
        int posicion = 0;
        foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
        {
            if (Item.Nombre == "Abono")
            {
                if (Inventario.Cantidades[posicion] > 0)
                {
                    Abonado = true;
                    Botones[1].SetActive(false);
                    Botones[2].SetActive(false);
                    Iconos[1].sprite = Sprites[1];
                    Inventario.Cantidades[posicion]--;
                    PlayerPrefs.SetString("SembradioAbonado:" + ID, "Si");

                    if (Regado)
                    {
                        Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[4];
                    }
                    else
                    {
                        Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[2];
                    }

                }
            }
            posicion++;
        }
    }

    public void BotonAbonarPlus()
    {
        int posicion = 0;
        foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
        {
            if (Item.Nombre == "Abono+")
            {
                if (Inventario.Cantidades[posicion] > 0)
                {
                    AbonadoPlus = true;
                    Botones[1].SetActive(false);
                    Botones[2].SetActive(false);
                    Iconos[1].sprite = Sprites[1];
                    Inventario.Cantidades[posicion]--;
                    PlayerPrefs.SetString("SembradioAbonadoPlus:" + ID, "Si");

                    if (Regado)
                    {
                        Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[5];
                    }
                    else
                    {
                        Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[3];
                    }
                }
            }
            posicion++;
        }
    }

    public void BotonCerrar()
    {
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
        Barril.UltimoDiaPlanta = false;
        PlayerPrefs.DeleteKey("SembradioRegado:" + ID);
        PlayerPrefs.DeleteKey("SembradioAbonadoPlus:" + ID);
        PlayerPrefs.DeleteKey("SembradioAbonado:" + ID);
        PlayerPrefs.DeleteKey("Plantado" + ID);
        PlayerPrefs.DeleteKey("SemillaPlantada" + ID);
        SemillaPlantada = null; // Aquí sí eliminamos la referencia a la semilla plantada
        Producto.sprite = Vacio;
        Abono.GetComponent<SpriteRenderer>().material = MaterialesAbono[0];

        // Reiniciamos visualmente la planta
        foreach (Transform Planta in ObjetoPlanta.transform.GetComponentInChildren<Transform>())
        {
            foreach (Transform fase in Planta.GetComponentInChildren<Transform>())
            {
                fase.gameObject.SetActive(false);
            }
            Planta.gameObject.SetActive(false);
        }

        DiasPlantado = 0;
        PlayerPrefs.DeleteKey("DiasPlantado:" + ID);
        PlayerPrefs.DeleteKey("DiaAnterior:" + ID);
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
                    Botones[0].SetActive(false);
                    Botones[1].SetActive(false);
                    Botones[2].SetActive(false);
                    transform.GetChild(1).GetChild(6).gameObject.SetActive(false);
                    transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
                    transform.GetChild(1).GetChild(8).gameObject.SetActive(false);
                    transform.GetChild(1).GetChild(11).gameObject.SetActive(false);
                    Producto.sprite = Item.Icono;
                    PlayerPrefs.SetString("Plantado" + ID, "Si");
                    PlayerPrefs.SetInt("SemillaPlantada" + ID, SemillaActual);

                }
            }
            posicion++;
        }
    }

    private void ActualizarPlantaInicial()
    {
        if (SemillaPlantada != null && DiasPlantado > 0)
        {
            // Recorremos los hijos de la planta para mostrar la etapa de crecimiento correcta
            foreach (Transform Planta in ObjetoPlanta.GetComponentInChildren<Transform>())
            {
                if (Planta.name == SemillaPlantada.TipoPlanta)
                {
                    // Activamos la planta principal
                    Planta.gameObject.SetActive(true);

                    // Activar solo la etapa de crecimiento correspondiente al día actual
                    for (int i = 0; i < Planta.childCount; i++)
                    {
                        Planta.GetChild(i).gameObject.SetActive(i == DiasPlantado - 1);
                    }

                    Debug.Log(Planta.childCount.ToString() + DiasPlantado.ToString());
                    //En caso de ser la ultima face
                    if (DiasPlantado == Planta.childCount)
                    {
                        if (Barril.TipoFruta != null)
                        {

                            Barril.UltimoDiaPlanta = true;
                        }
                    }

                    break; // Detenemos el bucle después de activar la planta correcta
                }
            }
        }
    }
}
