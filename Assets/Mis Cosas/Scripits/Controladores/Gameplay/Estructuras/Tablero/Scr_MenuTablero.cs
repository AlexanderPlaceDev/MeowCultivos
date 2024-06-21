using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_MenuTablero : MonoBehaviour
{
    [Header("Estructuras")]
    [SerializeField] Scr_CreadorEstructuras[] Estructuras;
    [SerializeField] GameObject[] ObjEstructuras;


    [Header("Menu")]
    [SerializeField] TextMeshProUGUI Nombre;
    [SerializeField] TextMeshProUGUI Descripcion;
    [SerializeField] public GameObject[] Botones;
    [SerializeField] public Color32[] ColoresBotones;
    [SerializeField] GameObject ImagenConstruido;
    [SerializeField] Image Imagen;
    [SerializeField] Sprite[] TodosLosMateriales;
    [SerializeField] Image[] Materiales;
    [SerializeField] TextMeshProUGUI[] Cantidades;

    [Header("Inventario")]
    [SerializeField] Scr_Inventario Inventario;
    public int EstructuraActual = 0;
    void Start()
    {
        ActivarEstructuras();
        EstructuraActual = PlayerPrefs.GetInt("EstructuraTablero", 0);
    }

    void Update()
    {
        if (GetComponent<Scr_Tablero>().EstaDentro)
        {
            ActualizarEstructura();
            GetComponent<Scr_EventosGuardado>().GuardarTablero(EstructuraActual);
        }
    }

    //Aqui se actualizan los datos de la estructura
    void ActualizarEstructura()
    {

        //Actualiza si esta comprada la estructura
        if (PlayerPrefs.GetInt("Estructura" + EstructuraActual, 0) == 1)
        {
            Botones[3].SetActive(false);
            ImagenConstruido.SetActive(true);
        }
        else
        {
            Botones[3].SetActive(true);
            ImagenConstruido.SetActive(false);
        }


        //Para cada estructura en la lista
        for (int i = 0; i < Estructuras.Length; i++)
        {
            //En caso de ser la estructura actual
            if (i == EstructuraActual)
            {
                //Actualiza los datos
                Nombre.text = Estructuras[i].Nombre;
                Descripcion.text = Estructuras[i].Descripcion;
                Imagen.sprite = Estructuras[i].Imagen;

                //Actualiza los materiales
                //Para cada material
                foreach (Sprite Imagen in TodosLosMateriales)
                {
                    //Para cada material en el tablero
                    for (int j = 0; j < Materiales.Length; j++)
                    {
                        //En caso de los materiales vacios
                        if (j >= Estructuras[i].Materiales.Length)
                        {
                            Materiales[j].gameObject.SetActive(false);
                        }
                        else
                        {
                            //En caso de los materiales necesarios
                            if (Imagen.name.Contains(Estructuras[i].Materiales[j].Nombre))
                            {
                                Materiales[j].gameObject.SetActive(true);
                                Materiales[j].sprite = Imagen;
                                Cantidades[j].text = Estructuras[i].Cantidades[j].ToString();
                            }
                        }
                    }
                }
                break;
            }
        }
    }

    public void EntraColorBoton(int ID)
    {
        //El primero es el ID y el segundo es el color
        if (GetComponent<Scr_Tablero>().EstaDentro)
        {
            Botones[ID].GetComponent<Image>().color = Color.white;
            if (ID == 3)
            {
                if (CalcularObjetos(Estructuras[EstructuraActual].Materiales, Estructuras[EstructuraActual].Cantidades))
                {
                    Botones[ID].GetComponent<Image>().color = Color.green; Botones[ID].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
                }
                else
                {
                    Botones[ID].GetComponent<Image>().color = Color.red; Botones[ID].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                }
            }
            if (Botones[ID].transform.childCount > 0)
            {
                if (Botones[ID].transform.childCount > 2)
                {
                    if (Botones[ID].transform.GetChild(1).GetComponent<TextMeshProUGUI>() != null)
                    {
                        Botones[ID].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.white;
                    }
                }

            }
        }
    }
    public void SaleColorBoton(int ID)
    {
        //El primero es el ID y el segundo es el color
        if (GetComponent<Scr_Tablero>().EstaDentro)
        {
            Botones[ID].GetComponent<Image>().color = ColoresBotones[0];
            if (ID == 3) { Botones[ID].GetComponent<Image>().color = Color.white; Botones[ID].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white; }
            if (Botones[ID].transform.childCount > 0)
            {
                if (Botones[ID].transform.childCount > 2)
                {
                    if (Botones[ID].transform.GetChild(1).GetComponent<TextMeshProUGUI>() != null)
                    {
                        Botones[ID].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = ColoresBotones[1];
                    }
                }
            }

        }
    }

    public void BotonComprar()
    {
        if (Botones[3].GetComponent<Image>().color == Color.green)
        {
            PlayerPrefs.SetInt("Estructura" + EstructuraActual, 1);
            QuitarObjetos(Estructuras[EstructuraActual].Materiales, Estructuras[EstructuraActual].Cantidades);
            ActualizarEstructura();
            ActivarEstructuras();
        }
    }

    private void ActivarEstructuras()
    {
        int i = 0;
        foreach (GameObject Estructura in ObjEstructuras)
        {
            if (PlayerPrefs.GetInt("Estructura" + i, 0) == 1)
            {
                Estructura.SetActive(true);
            }
            i++;
        }
    }

    private bool CalcularObjetos(Scr_CreadorObjetos[] ObjetosNecesarios, int[] CantidadesNecesarias)
    {
        int encontrados = 0;
        int i = 0;
        foreach (Scr_CreadorObjetos Objeto in ObjetosNecesarios)
        {
            int j = 0;
            foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
            {
                if (Item.Nombre == Objeto.Nombre)
                {
                    if (Inventario.Cantidades[j] >= CantidadesNecesarias[i])
                    {
                        encontrados++;
                    }
                }
                j++;
            }
            i++;
        }

        if (encontrados >= ObjetosNecesarios.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void QuitarObjetos(Scr_CreadorObjetos[] ObjetosNecesarios, int[] CantidadesNecesarias)
    {
        int i = 0;
        foreach (Scr_CreadorObjetos Objeto in ObjetosNecesarios)
        {
            int j = 0;
            foreach (Scr_CreadorObjetos Item in Inventario.Objetos)
            {
                if (Item.Nombre == Objeto.Nombre)
                {

                    Inventario.Cantidades[j] -= CantidadesNecesarias[i];
                }
                j++;
            }
            i++;
        }
    }
}
