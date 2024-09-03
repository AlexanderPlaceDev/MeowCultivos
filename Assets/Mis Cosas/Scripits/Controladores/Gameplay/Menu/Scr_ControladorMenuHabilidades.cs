using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scr_ControladorMenuHabilidades : MonoBehaviour
{
    [SerializeField] GameObject Arbol;
    [SerializeField] float moveSpeed;
    [SerializeField] float scaleFactor;
    [SerializeField] GameObject[] Botones;
    [SerializeField] GameObject ObjetoHabilidadSeleccionada;
    public GameObject BotonActual;
    public string HabilidadActual;
    GameObject BotonSeleccionado;
    [SerializeField] Scr_CreadorHabilidades[] Habilidades;
    [SerializeField] TextMeshProUGUI Puntos;
    [SerializeField] GameObject BotonAceptar;

    string HabilidadSeleccionada;
    bool YaSelecciono = false;

    private RectTransform arbolRectTransform;

    private void Awake()
    {
        moveSpeed = moveSpeed * 1000;
        arbolRectTransform = Arbol.GetComponent<RectTransform>();

        foreach (GameObject Boton in Botones)
        {
            if (PlayerPrefs.GetString("Habilidad:" + Boton.name, "No") == "Si")
            {
                Boton.GetComponent<Image>().color = Color.white;
                Boton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            }
        }

        foreach (Scr_CreadorHabilidades Habilidad in Habilidades)
        {
            if (PlayerPrefs.GetString("Habilidad:" + Habilidad.NombreBoton, "No") == "Si")
            {
                ActivarBarra(Habilidad.NombresBarrasCarga);
            }
        }
    }

    void Start()
    {

    }

    void Update()
    {
        Puntos.text = PlayerPrefs.GetInt("PuntosDeHabilidad", 0).ToString();
        SeleccionarHabilidad();

        MoverYEscalarArbol();
        if (HabilidadActual != null || HabilidadActual != "")
        {
            ActualizarHabilidad();
        }



    }

    private void SeleccionarHabilidad()
    {
        // Añade una verificación para asegurarte de que no se vuelva a seleccionar de inmediato
        if (HabilidadSeleccionada == null && HabilidadActual != "" && Input.GetKeyDown(KeyCode.Mouse0) && !YaSelecciono && ComprobarHabilidadAnterior())
        {
            YaSelecciono = true;
            BotonSeleccionado = BotonActual;
            HabilidadSeleccionada = HabilidadActual;
            Debug.Log("Habilidad Seleccionada");

            foreach (Scr_CreadorHabilidades Habilidad in Habilidades)
            {
                if (Habilidad.NombreBoton == HabilidadActual)
                {
                    if (PlayerPrefs.GetInt("PuntosDeHabilidad", 0) >= Habilidad.Costo && PlayerPrefs.GetString("Habilidad:" + HabilidadActual, "No") == "No")
                    {
                        BotonAceptar.SetActive(true);
                    }
                    else
                    {
                        BotonAceptar.SetActive(false);
                    }
                }
            }

            ObjetoHabilidadSeleccionada.SetActive(true);
        }
    }
    public void RechazarHabilidad()
    {
        Debug.Log("Rechaza Habilidad");

        // Desactiva la selección de habilidad y resetea la variable de control
        YaSelecciono = false;

        // Asegúrate de que la UI relacionada con la selección de habilidad se oculte
        ObjetoHabilidadSeleccionada.SetActive(false);

        // Asegúrate de que la habilidad seleccionada se deseleccione
        HabilidadSeleccionada = null;
    }
    private void ActualizarHabilidad()
    {
        if (BotonActual != null)
        {
            ObjetoHabilidadSeleccionada.transform.GetChild(0).GetComponent<Image>().sprite = BotonActual.GetComponent<Image>().sprite;
            ObjetoHabilidadSeleccionada.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = BotonActual.transform.GetChild(0).GetComponent<Image>().sprite;
            foreach (Scr_CreadorHabilidades Habilidad in Habilidades)
            {
                if (Habilidad.NombreBoton == BotonActual.name)
                {
                    ObjetoHabilidadSeleccionada.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Habilidad.Nombre;
                    ObjetoHabilidadSeleccionada.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Habilidad.Descripcion;
                    ObjetoHabilidadSeleccionada.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Costo: " + Habilidad.Costo;

                }
            }
        }
    }

    private void MoverYEscalarArbol()
    {
        // Cambiar tamaño del árbol con la rueda del mouse
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            if (arbolRectTransform.localScale.x > 0f || scrollInput > 0)
            {
                arbolRectTransform.localScale += Vector3.one * scrollInput * scaleFactor;
            }

        }

        // Mover el árbol cuando se presiona la rueda del mouse
        if (Input.GetMouseButton(2))
        {
            float moveX = Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
            float moveY = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;
            arbolRectTransform.anchoredPosition += new Vector2(moveX, moveY);
        }
    }

    public void EntraHabilidad(string Habilidad)
    {
        HabilidadActual = Habilidad;

        foreach (GameObject Boton in Botones)
        {
            if (Boton.gameObject.name == HabilidadActual)
            {
                BotonActual = Boton;
                if (ComprobarHabilidadAnterior())
                {
                    Boton.GetComponent<Image>().color = Color.white;
                    Boton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    Boton.transform.GetChild(1).gameObject.SetActive(true);
                }

                break;
            }
        }
    }

    public void SaleHabilidad()
    {
        HabilidadActual = "";
        if (BotonActual != null)
        {
            BotonActual.transform.GetChild(1).gameObject.SetActive(false);
            if (PlayerPrefs.GetString("Habilidad:" + BotonActual.name, "No") == "No")
            {
                BotonActual.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
                BotonActual.transform.GetChild(0).GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            }
            BotonActual = null;

        }


    }

    public void ComprarHabilidad()
    {

        if (HabilidadSeleccionada != null)
        {
            foreach (Scr_CreadorHabilidades Habilidad in Habilidades)
            {
                Debug.Log(Habilidad.NombreBoton + HabilidadSeleccionada);
                if (Habilidad.NombreBoton == HabilidadSeleccionada)
                {
                    int puntosActuales = PlayerPrefs.GetInt("PuntosDeHabilidad", 0);
                    int puntosRestantes = puntosActuales - Habilidad.Costo;

                    // Asegúrate de que los puntos no sean negativos
                    if (puntosRestantes >= 0)
                    {
                        PlayerPrefs.SetInt("PuntosDeHabilidad", puntosRestantes);
                    }
                    else
                    {
                        Debug.LogWarning("No tienes suficientes puntos para comprar esta habilidad.");
                        return;
                    }
                }
            }

            if (PlayerPrefs.GetString("Habilidad:" + HabilidadSeleccionada, "No") == "No")
            {


                PlayerPrefs.SetString("Habilidad:" + HabilidadSeleccionada, "Si");
                BotonSeleccionado.GetComponent<Image>().color = Color.white;
                BotonSeleccionado.transform.GetChild(0).GetComponent<Image>().color = Color.white;

                // Desactiva la selección de habilidad y resetea la variable de control
                YaSelecciono = false;

                // Asegúrate de que la UI relacionada con la selección de habilidad se oculte
                ObjetoHabilidadSeleccionada.SetActive(false);

                // Asegúrate de que la habilidad seleccionada se deseleccione
                HabilidadSeleccionada = null;
            }
        }
    }

    bool ComprobarHabilidadAnterior()
    {
        foreach (Scr_CreadorHabilidades Habilidad in Habilidades)
        {

            if (Habilidad.NombreBoton == BotonActual.name)
            {

                // Si la habilidad anterior es una cadena vacía, se permite seleccionar la habilidad actual
                if (Habilidad.HabilidadesAnteriores.Length == 0)
                {
                    Debug.Log("1");
                    return true;
                }

                foreach (string HabilidadAnterior in Habilidad.HabilidadesAnteriores)
                {
                    // Comprobar si la habilidad anterior ha sido comprada
                    if (PlayerPrefs.GetString("Habilidad:" + HabilidadAnterior, "No") == "Si")
                    {
                        Debug.Log("2");
                        return true;
                    }
                    else
                    {
                        Debug.Log("3");
                        return false;
                    }
                }
            }
        }

        // Si no se encuentra la habilidad actual en la lista de habilidades, se retorna false
        Debug.Log("No se encontró una habilidad coincidente para el botón actual");
        return false;
    }

    void ActivarBarra(string[] NombresBarra)
    {
        // Obtén todos los transform de los hijos (incluyendo descendientes)
        Transform[] todasLasBarras = Arbol.GetComponentsInChildren<Transform>();

        foreach (string NombreBarra in NombresBarra)
        {
            foreach (Transform Barra in todasLasBarras)
            {
                if (Barra.gameObject.name == NombreBarra)
                {
                    // Asegúrate de que estás accediendo al hijo correcto y llenando la barra
                    Image imagenBarra = Barra.GetChild(0).GetComponent<Image>();
                    if (imagenBarra != null)
                    {
                        imagenBarra.fillAmount = 1;
                    }
                    else
                    {
                        Debug.LogWarning($"No se encontró un componente Image en {Barra.gameObject.name}");
                    }
                }
            }
        }
    }
}