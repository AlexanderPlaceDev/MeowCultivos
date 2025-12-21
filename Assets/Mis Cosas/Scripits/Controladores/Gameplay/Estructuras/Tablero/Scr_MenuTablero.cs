using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_MenuTablero : MonoBehaviour
{
    [Header("Estructuras")]
    [SerializeField] private Scr_CreadorEstructuras[] Estructuras;

    // 🔥 ESTA ES LA LISTA QUE SÍ USARÁS
    [SerializeField] private Estructura[] Objetos;

    private List<Scr_CreadorEstructuras> estructurasFiltradas = new List<Scr_CreadorEstructuras>();
    private List<int> indicesOriginales = new List<int>();

    private float TransicionDuracion = 3f;
    [SerializeField] private GameObject camara_tablero;
    [SerializeField] private GameObject camaraGata;

    [Header("Menú")]
    [SerializeField] private TextMeshProUGUI Nombre;
    [SerializeField] private TextMeshProUGUI Descripcion;
    [SerializeField] private GameObject[] Botones;
    [SerializeField] private Color32[] ColoresBotones;
    [SerializeField] private GameObject ImagenConstruido;
    [SerializeField] private Image Imagen;
    [SerializeField] private Image[] Materiales;
    [SerializeField] private TextMeshProUGUI[] Cantidades;
    [SerializeField] private Image Notificacion;
    [SerializeField] private GameObject PanelNotificacion;
    private bool NotificacionParpadeando = false;

    [Header("Inventario")]
    [SerializeField] private Scr_Inventario Inventario;

    public int EstructuraActual = 0;

    private Scr_ActivadorMenuEstructuraFijo Tablero => GetComponent<Scr_ActivadorMenuEstructuraFijo>();
    private Scr_EventosGuardado EventosGuardado => GetComponent<Scr_EventosGuardado>();

    [System.Serializable]
    public class Estructura
    {
        [SerializeField] public GameObject[] ObjetosEstructura;
    }

    private void Start()
    {
        ActualizarEstructurasFiltradas();

        // 🔥 Activación inicial desde PlayerPrefs usando ObjetosEstructura[]
        for (int i = 0; i < Objetos.Length; i++)
        {
            bool activo = PlayerPrefs.GetInt("Estructura" + i, 0) == 1;

            foreach (var obj in Objetos[i].ObjetosEstructura)
            {
                if (obj != null)
                    obj.SetActive(activo);
            }
        }

        EstructuraActual = PlayerPrefs.GetInt("EstructuraTablero", 0);
    }

    private void FixedUpdate()
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
        indicesOriginales.Clear();

        for (int i = 0; i < Estructuras.Length; i++)
        {
            if (Estructuras[i].PlanoRequerido == "" || VerificarEstructura(Estructuras[i].PlanoRequerido))
            {
                estructurasFiltradas.Add(Estructuras[i]);
                indicesOriginales.Add(i);
            }
        }
    }

    private void ActivarEstructuras()
    {
        // 🔥 Activación general desde PlayerPrefs
        for (int i = 0; i < Objetos.Length; i++)
        {
            bool activo = PlayerPrefs.GetInt("Estructura" + i, 0) == 1;

            foreach (var obj in Objetos[i].ObjetosEstructura)
            {
                if (obj != null)
                    obj.SetActive(activo);
            }
        }
    }

    private void CambiarACamaraDialogo()
    {
        camara_tablero.SetActive(false);
    }

    private IEnumerator EsperarCamara()
    {
        yield return new WaitForSeconds(TransicionDuracion);
        int cam = 0;
        // 🔥 Oculta el "hijo 0" de cada objeto de esta estructura
        foreach (var obj in Objetos[indicesOriginales[EstructuraActual]].ObjetosEstructura)
        {
            if (obj != null && obj.transform.childCount > 0)
            {
                if (cam == 0)
                {
                    obj.transform.GetChild(0).gameObject.SetActive(false);
                }
                cam++;
            }
        }

        camara_tablero.SetActive(true);
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
            EstructuraActual = 0;

        int indiceReal = indicesOriginales[EstructuraActual];

        bool estaComprada = PlayerPrefs.GetInt("Estructura" + indiceReal, 0) == 1;

        Botones[3].SetActive(!estaComprada);
        ImagenConstruido.SetActive(estaComprada);

        Scr_CreadorEstructuras estructura = estructurasFiltradas[EstructuraActual];

        Nombre.text = estructura.Nombre;
        Descripcion.text = estructura.Descripcion;
        Imagen.sprite = estructura.Imagen;

        if (estructura.AumentaRango)
        {
            Notificacion.gameObject.SetActive(true);

            if (!NotificacionParpadeando)
                StartCoroutine(ParpadearNotificacion());
        }
        else
        {
            NotificacionParpadeando = false;
            Notificacion.gameObject.SetActive(false);
        }

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

                bool tieneMateriales = CalcularObjetos(
                    new Scr_CreadorObjetos[] { estructura.Materiales[i] },
                    new int[] { estructura.Cantidades[i] }
                );
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
                return Objeto.Icono;
        }
        return null;
    }

    public void EntraColorBoton(int ID)
    {
        if (Tablero.EstaDentro)
            CambiarColorBoton(ID, Color.white, Color.green, Color.red);
    }

    public void SaleColorBoton(int ID)
    {
        if (Tablero.EstaDentro)
            CambiarColorBoton(ID, ColoresBotones[0], Color.white, ColoresBotones[1]);
    }

    private void CambiarColorBoton(int ID, Color colorBoton, Color colorVerde, Color colorRojo)
    {
        Image botonImage = Botones[ID].GetComponent<Image>();
        botonImage.color = colorBoton;

        if (ID == 3)
        {
            botonImage.color = Color.white;
            bool tieneMateriales = CalcularObjetos(
                estructurasFiltradas[EstructuraActual].Materiales,
                estructurasFiltradas[EstructuraActual].Cantidades
            );
            Color colorTexto = tieneMateriales ? colorVerde : colorRojo;
            if (colorRojo == ColoresBotones[1])
                Botones[ID].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            else
            {
                Botones[ID].GetComponent<Image>().color = colorTexto;
                Botones[ID].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = colorTexto;
            }
        }

        if (ID == 0)
            Botones[ID].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = ColoresBotones[1];

        if (Botones[ID].transform.childCount > 2)
        {
            TextMeshProUGUI texto = Botones[ID].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            if (texto != null)
                texto.color = colorBoton == Color.white ? colorRojo : colorVerde;
        }
    }

    public void BotonComprar()
    {
        if (Botones[3].GetComponent<Image>().color == Color.green)
        {
            int indiceReal = indicesOriginales[EstructuraActual];

            PlayerPrefs.SetInt("Estructura" + indiceReal, 1);

            Scr_CreadorEstructuras estructura = estructurasFiltradas[EstructuraActual];

            if (estructura.AumentaRango)
            {
                PlayerPrefs.SetInt("Rango Barra Naturaleza3", PlayerPrefs.GetInt("Rango Barra Naturaleza3", 0) + 1);
                PlayerPrefs.SetInt("Rango Barra Planos4", PlayerPrefs.GetInt("Rango Barra Planos4", 0) + 1);
                GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_ControladorMenuHabilidades>().ActualizarBarrasPorRango();

                GameObject.Find("Canvas").transform.GetChild(10).gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.GetChild(10).GetComponent<Scr_NuevoRango>().MostrarRango("Naturaleza",PlayerPrefs.GetInt("Rango Barra Naturaleza3",0));
            }

            QuitarObjetos(estructura.Materiales, estructura.Cantidades);

            camara_tablero.SetActive(false);

            // 🔥 ACTIVAR TODOS LOS OBJETOS DE LA ESTRUCTURA
            foreach (var obj in Objetos[indiceReal].ObjetosEstructura)
            {
                if (obj != null)
                {
                    obj.SetActive(true);

                    if (obj.transform.childCount > 0)
                        obj.transform.GetChild(0).gameObject.SetActive(true);
                }
            }

            StartCoroutine(EsperarCamara());
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
                EstructuraActual++;
        }
        else
        {
            if (EstructuraActual > 0)
                EstructuraActual--;
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
                if (Inventario.Objetos[j].Nombre == objetosNecesarios[i].Nombre &&
                    Inventario.Cantidades[j] >= cantidadesNecesarias[i])
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
        return PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" ||
               string.IsNullOrEmpty(Habilidad);
    }

    private IEnumerator ParpadearNotificacion()
    {
        NotificacionParpadeando = true;
        Color blanco = Color.white;
        Color gris = Color.gray;
        bool colorBlanco = true;

        while (NotificacionParpadeando)
        {
            Notificacion.color = colorBlanco ? blanco : gris;
            colorBlanco = !colorBlanco;
            yield return new WaitForSeconds(1f);
        }

        Notificacion.color = blanco;
    }

    public void EntraNotificacion()
    {
        PanelNotificacion.SetActive(true);
    }

    public void SaleNotificacion()
    {
        PanelNotificacion.SetActive(false);
    }
}
