using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_MenuTablero : MonoBehaviour
{
    [Header("Estructuras")]
    [SerializeField] private Scr_CreadorEstructuras[] Estructuras;
    [SerializeField] private GameObject[] ObjEstructuras;

    private List<Scr_CreadorEstructuras> estructurasFiltradas = new List<Scr_CreadorEstructuras>();
    private List<GameObject> objEstructurasFiltradas = new List<GameObject>();

    [Header("Menú")]
    [SerializeField] private TextMeshProUGUI Nombre;
    [SerializeField] private TextMeshProUGUI Descripcion;
    [SerializeField] private GameObject[] Botones;
    [SerializeField] private Color32[] ColoresBotones;
    [SerializeField] private GameObject ImagenConstruido;
    [SerializeField] private Image Imagen;
    [SerializeField] private Image[] Materiales;
    [SerializeField] private TextMeshProUGUI[] Cantidades;

    [Header("Inventario")]
    [SerializeField] private Scr_Inventario Inventario;

    public int EstructuraActual = 0;

    private Scr_ActivadorMenuEstructuraFijo Tablero => GetComponent<Scr_ActivadorMenuEstructuraFijo>();
    private Scr_EventosGuardado EventosGuardado => GetComponent<Scr_EventosGuardado>();

    private void Start()
    {
        ActualizarEstructurasFiltradas();
        for (int i = 0; i < ObjEstructuras.Length; i++)
        {
            ObjEstructuras[i].SetActive(PlayerPrefs.GetInt("Estructura" + i, 0) == 1);
        }
        EstructuraActual = PlayerPrefs.GetInt("EstructuraTablero", 0);
    }

    private void Update()
    {
        if (Tablero.EstaDentro)
        {
            ActualizarEstructura();
            EventosGuardado.GuardarTablero(EstructuraActual);
        }
    }

    private void ActualizarEstructurasFiltradas()
    {
        estructurasFiltradas.Clear();
        objEstructurasFiltradas.Clear();

        for (int i = 0; i < Estructuras.Length; i++)
        {
            if (VerificarEstructura(Estructuras[i].HabilidadRequerida))
            {
                estructurasFiltradas.Add(Estructuras[i]);
                objEstructurasFiltradas.Add(ObjEstructuras[i]);
            }
        }
    }

    private void ActivarEstructuras()
    {
        for (int i = 0; i < ObjEstructuras.Length; i++)
        {
            ObjEstructuras[i].SetActive(PlayerPrefs.GetInt("Estructura" + i, 0) == 1);
        }
    }

    private void ActualizarEstructura()
    {
        ActualizarEstructurasFiltradas();

        if (estructurasFiltradas.Count == 0)
        {
            Debug.LogError("No hay estructuras disponibles.");
            return;
        }

        if (EstructuraActual >= estructurasFiltradas.Count)
        {
            EstructuraActual = 0;
        }

        bool estaComprada = PlayerPrefs.GetInt("Estructura" + EstructuraActual, 0) == 1;
        Botones[3].SetActive(!estaComprada);
        ImagenConstruido.SetActive(estaComprada);

        Scr_CreadorEstructuras estructura = estructurasFiltradas[EstructuraActual];
        Nombre.text = estructura.Nombre;
        Descripcion.text = estructura.Descripcion;
        Imagen.sprite = estructura.Imagen;

        Botones[1].SetActive(EstructuraActual != 0);
        Botones[2].SetActive(EstructuraActual != estructurasFiltradas.Count - 1);

        ActualizarMateriales(estructura);
    }

    private void ActualizarMateriales(Scr_CreadorEstructuras estructura)
    {
        for (int i = 0; i < Materiales.Length; i++)
        {
            if (i < estructura.Materiales.Length)
            {
                Materiales[i].gameObject.SetActive(true);
                Materiales[i].sprite = BuscarSprite(estructura.Materiales[i].Nombre);
                Cantidades[i].text = estructura.Cantidades[i].ToString();

                bool tieneMateriales = CalcularObjetos(new Scr_CreadorObjetos[] { estructura.Materiales[i] }, new int[] { estructura.Cantidades[i] });
                Cantidades[i].color = tieneMateriales ? Color.white : Color.red;
            }
            else
            {
                Materiales[i].gameObject.SetActive(false);
            }
        }
    }

    private Sprite BuscarSprite(string nombreMaterial)
    {
        foreach (var Objeto in Inventario.Objetos)
        {
            if (Objeto.name.Contains(nombreMaterial))
            {
                return Objeto.Icono;
            }
        }
        return null;
    }

    public void EntraColorBoton(int ID)
    {
        if (Tablero.EstaDentro)
        {
            CambiarColorBoton(ID, Color.white, Color.green, Color.red);
        }
    }

    public void SaleColorBoton(int ID)
    {
        if (Tablero.EstaDentro)
        {
            CambiarColorBoton(ID, ColoresBotones[0], Color.white, ColoresBotones[1]);
        }
    }

    private void CambiarColorBoton(int ID, Color colorBoton, Color colorVerde, Color colorRojo)
    {
        Image botonImage = Botones[ID].GetComponent<Image>();
        botonImage.color = colorBoton;

        if (ID == 3)
        {
            botonImage.color = Color.white;
            bool tieneMateriales = CalcularObjetos(estructurasFiltradas[EstructuraActual].Materiales, estructurasFiltradas[EstructuraActual].Cantidades);
            Color colorTexto = tieneMateriales ? colorVerde : colorRojo;
            if (colorRojo == ColoresBotones[1])
            {
                Botones[ID].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                Botones[ID].GetComponent<Image>().color = colorTexto;
                Botones[ID].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = colorTexto;
            }
        }

        if (ID == 0)
        {
            Botones[ID].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = ColoresBotones[1];
        }

        if (Botones[ID].transform.childCount > 2)
        {
            TextMeshProUGUI texto = Botones[ID].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            if (texto != null)
            {
                texto.color = colorBoton == Color.white ? colorRojo : colorVerde;
            }
        }
    }

    public void BotonComprar()
    {
        if (Botones[3].GetComponent<Image>().color == Color.green)
        {
            PlayerPrefs.SetInt("Estructura" + EstructuraActual, 1);
            QuitarObjetos(estructurasFiltradas[EstructuraActual].Materiales, estructurasFiltradas[EstructuraActual].Cantidades);
            ActualizarEstructura();
            ActivarEstructuras();
        }
    }

    public void Flechas(bool Aumenta)
    {
        ActualizarEstructurasFiltradas();

        if (Aumenta)
        {
            if (EstructuraActual < estructurasFiltradas.Count - 1)
            {
                EstructuraActual++;
            }
        }
        else
        {
            if (EstructuraActual > 0)
            {
                EstructuraActual--;
            }
        }
        ActualizarEstructura();
    }

    private bool CalcularObjetos(Scr_CreadorObjetos[] objetosNecesarios, int[] cantidadesNecesarias)
    {
        int encontrados = 0;
        for (int i = 0; i < objetosNecesarios.Length; i++)
        {
            for (int j = 0; j < Inventario.Objetos.Length; j++)
            {
                if (Inventario.Objetos[j].Nombre == objetosNecesarios[i].Nombre && Inventario.Cantidades[j] >= cantidadesNecesarias[i])
                {
                    encontrados++;
                    break;
                }
            }
        }
        return encontrados == objetosNecesarios.Length;
    }

    private void QuitarObjetos(Scr_CreadorObjetos[] objetosNecesarios, int[] cantidadesNecesarias)
    {
        for (int i = 0; i < objetosNecesarios.Length; i++)
        {
            for (int j = 0; j < Inventario.Objetos.Length; j++)
            {
                if (Inventario.Objetos[j].Nombre == objetosNecesarios[i].Nombre)
                {
                    Inventario.Cantidades[j] -= cantidadesNecesarias[i];
                    break;
                }
            }
        }
    }

    private bool VerificarEstructura(string Habilidad)
    {
        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" || string.IsNullOrEmpty(Habilidad))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
