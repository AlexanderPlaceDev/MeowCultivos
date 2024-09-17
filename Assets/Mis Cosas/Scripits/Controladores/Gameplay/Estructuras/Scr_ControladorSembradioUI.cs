using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Scr_ControladorSembradioUI : MonoBehaviour
{
    [SerializeField] int ID;
    [SerializeField] Scr_CreadorObjetos[] ObjetosQuePlanta;
    [SerializeField] Image[] Iconos;
    [SerializeField] Sprite[] Sprites;
    [SerializeField] Sprite[] SpritesAbono;
    [SerializeField] GameObject Abono;
    [SerializeField] Image Semilla;
    [SerializeField] Image Producto;
    [SerializeField] GameObject[] Botones;
    [SerializeField] Color ColorBoton;
    [SerializeField] bool Regado;
    [SerializeField] bool Abonado;
    [SerializeField] bool AbonadoPlus;
    Scr_Inventario Inventario;

    Scr_CreadorObjetos SemillaPlantada = null;
    int SemillaActual = 0;
    Sprite Vacio;
    void Start()
    {
        Vacio = Producto.GetComponent<Image>().sprite;
        Inventario = GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_Inventario>();

        if (PlayerPrefs.GetString("Plantado" + ID, "No") == "Si")
        {
            SemillaActual = PlayerPrefs.GetInt("SemillaPlantada", 0);
            Producto.sprite = ObjetosQuePlanta[SemillaActual].Icono;
            SemillaPlantada = ObjetosQuePlanta[SemillaActual];

        }


        if (PlayerPrefs.GetString("SembradioRegado:" + ID, "No") == "Si")
        {
            Regado = true;
            Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[1];
        }

        if (PlayerPrefs.GetString("SembradioAbonado:" + ID, "No") == "Si")
        {
            Abonado = true;

            if (Regado)
            {
                Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[4];
            }
            else
            {
                Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[2];
            }
        }

        if (PlayerPrefs.GetString("SembradioAbonadoPlus:" + ID, "No") == "Si")
        {
            AbonadoPlus = true;
            if (Regado)
            {
                Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[5];
            }
            else
            {
                Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[2];
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
            if (Botones[0].activeSelf || Botones[1].activeSelf || Botones[2].activeSelf || transform.GetChild(1).GetChild(6).gameObject.activeSelf)
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

            Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[1];
            if (Abonado)
            {
                Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[4];
            }
            if (AbonadoPlus)
            {
                Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[5];
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
                        Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[4];
                    }
                    else
                    {
                        Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[2];
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
                        Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[5];
                    }
                    else
                    {
                        Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[3];
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
        Regado = false;
        Abonado = false;
        AbonadoPlus = false;
        PlayerPrefs.DeleteKey("SembradioRegado:" + ID);
        PlayerPrefs.DeleteKey("SembradioAbonadoPlus:" + ID);
        PlayerPrefs.DeleteKey("SembradioAbonado:" + ID);
        PlayerPrefs.DeleteKey("Plantado" + ID);
        PlayerPrefs.DeleteKey("SemillaPlantada" + ID);
        Iconos[0].sprite = Sprites[0];
        Iconos[1].sprite = Sprites[0];
        SemillaPlantada = null;
        Producto.GetComponent<Image>().sprite = Vacio;
        Abono.GetComponent<SpriteRenderer>().sprite = SpritesAbono[0];

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
}
