using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class Scr_ControladorMenuHabilidades : MonoBehaviour
{
    [SerializeField] GameObject Arbol;
    [SerializeField] GameObject[] Ramas;
    [SerializeField] string RamaActual;
    [SerializeField] GameObject[] Barras;
    [SerializeField] float DuracionTransicion;
    [SerializeField] Ease TipoTransicionPaneles;
    [SerializeField] Ease TipoTransicionBotonesIn;
    [SerializeField] Ease TipoTransicionBotonesOut;
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject ObjetoHabilidadSeleccionada;
    public GameObject BotonActual;
    public string HabilidadActual;
    GameObject BotonSeleccionado;
    [SerializeField] Scr_CreadorHabilidades[] Habilidades;
    [SerializeField] TextMeshProUGUI Puntos;
    [SerializeField] GameObject BotonAceptar;

    bool YaSelecciono = false;

    private RectTransform arbolRectTransform;

    private void Awake()
    {
        moveSpeed = moveSpeed * 1000;
        arbolRectTransform = Arbol.GetComponent<RectTransform>();

        ActualizarBarrasPorRango();
    }

    void Start()
    {

    }

    void Update()
    {
        Puntos.text = PlayerPrefs.GetInt("PuntosDeHabilidad", 0).ToString();
        SeleccionarHabilidad();
        InputPruebas();
    }
    public void SeleccionarRama(string NombreRama)
    {
        RamaActual = NombreRama;

        switch (RamaActual)
        {
            case "Naturaleza":
                {
                    Debug.Log("Selecciona Rama");
                    //Escala Fondo
                    Tween.LocalScaleY(Barras[0].transform.GetChild(0), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Escala X
                    Tween.LocalScale(Ramas[0].transform.GetChild(2), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Escala Textos Rango
                    Tween.LocalScale(Ramas[0].transform.GetChild(3), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Posicion TextosRango
                    Tween.LocalPositionY(Ramas[0].transform.GetChild(3), -75, DuracionTransicion, TipoTransicionPaneles, default);
                    //Escala Fondo Planos
                    Tween.LocalScale(Ramas[0].transform.GetChild(0), new Vector3(4, 4, 1), DuracionTransicion, TipoTransicionPaneles, default);
                    //Posicion Y Escala Arbol
                    Tween.LocalScale(Arbol.transform, new Vector3(1.5f, 1.5f, 1.5f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, new Vector3(-1105, -110, 0), DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }
            case "Industrial":
                {
                    Debug.Log("Selecciona Rama");
                    //Escala Fondo
                    Tween.LocalScaleY(Barras[2].transform.GetChild(0), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Escala X
                    Tween.LocalScale(Ramas[2].transform.GetChild(1), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Escala Textos Rango
                    Tween.LocalScale(Ramas[2].transform.GetChild(2), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Posicion TextosRango
                    Tween.LocalPositionY(Ramas[2].transform.GetChild(2), -75, DuracionTransicion, TipoTransicionPaneles, default);
                    //Posicion Y Escala Arbol
                    Tween.LocalScale(Arbol.transform, new Vector3(1.5f, 1.5f, 1.5f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, new Vector3(1105, -110, 0), DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }
            case "Tecnica":
                {
                    Debug.Log("Selecciona Rama");
                    //Escala Fondo
                    Tween.LocalScaleY(Barras[1].transform.GetChild(0), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Escala X
                    Tween.LocalScale(Ramas[1].transform.GetChild(1), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Escala Textos Rango
                    Tween.LocalScale(Ramas[1].transform.GetChild(2), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Posicion TextosRango
                    Tween.LocalPositionY(Ramas[1].transform.GetChild(2), -440, DuracionTransicion, TipoTransicionPaneles, default);
                    //Posicion Y Escala Arbol
                    Tween.LocalScale(Arbol.transform, new Vector3(1.5f, 1.5f, 1.5f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, new Vector3(-850, 890, 0), DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }
            case "Arsenal":
                {
                    Debug.Log("Selecciona Rama");
                    //Escala Fondo
                    Tween.LocalScaleY(Barras[3].transform.GetChild(0), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Escala X
                    Tween.LocalScale(Ramas[3].transform.GetChild(1), 1, DuracionTransicion, TipoTransicionPaneles, default);
                    //Posicion Y Escala Arbol
                    Tween.LocalScale(Arbol.transform, new Vector3(1.5f, 1.5f, 1.5f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, new Vector3(850, 780, 0), DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }
        }

    }
    public void CerrarRama(string NombreRama)
    {
        switch (RamaActual)
        {
            case "Naturaleza":
                {
                    Debug.Log("Selecciona Rama");
                    //Escala Fondo
                    Tween.LocalScaleY(Barras[0].transform.GetChild(0), 0, DuracionTransicion, TipoTransicionBotonesIn, default);
                    //Escala X
                    Tween.LocalScale(Ramas[0].transform.GetChild(2), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    //Escala Textos Rango
                    Tween.LocalScale(Ramas[0].transform.GetChild(3), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    //Posicion TextosRango
                    Tween.LocalPositionY(Ramas[0].transform.GetChild(3), 180, DuracionTransicion, TipoTransicionBotonesOut, default);
                    //Escala Fondo Planos
                    Tween.LocalScale(Ramas[0].transform.GetChild(0), new Vector3(0, 0, 1), DuracionTransicion, TipoTransicionBotonesIn, default);
                    //Posicion Y Escala Arbol
                    Tween.LocalScale(Arbol.transform, new Vector3(1f, 1f, 1f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, Vector3.zero, DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }
            case "Industrial":
                {

                    Debug.Log("Selecciona Rama");
                    //Escala Fondo
                    Tween.LocalScaleY(Barras[2].transform.GetChild(0), 0, DuracionTransicion, TipoTransicionBotonesIn, default);
                    //Escala X
                    Tween.LocalScale(Ramas[2].transform.GetChild(1), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    //Escala Textos Rango
                    Tween.LocalScale(Ramas[2].transform.GetChild(2), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    //Posicion TextosRango
                    Tween.LocalPositionY(Ramas[2].transform.GetChild(2), 180, DuracionTransicion, TipoTransicionBotonesOut, default);
                    //Posicion Y Escala Arbol
                    Tween.LocalScale(Arbol.transform, new Vector3(1f, 1f, 1f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, Vector3.zero, DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }
            case "Tecnica":
                {
                    Debug.Log("Selecciona Rama");
                    //Escala Fondo
                    Tween.LocalScaleY(Barras[1].transform.GetChild(0), 0, DuracionTransicion, TipoTransicionBotonesIn, default);
                    //Escala X
                    Tween.LocalScale(Ramas[1].transform.GetChild(1), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    //Escala Textos Rango
                    Tween.LocalScale(Ramas[1].transform.GetChild(2), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    //Posicion TextosRango
                    Tween.LocalPositionY(Ramas[1].transform.GetChild(2), -180, DuracionTransicion, TipoTransicionBotonesOut, default);
                    //Posicion Y Escala Arbol
                    Tween.LocalScale(Arbol.transform, new Vector3(1f, 1f, 1f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, Vector3.zero, DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }
            case "Arsenal":
                {
                    Debug.Log("Selecciona Rama");
                    //Escala Fondo
                    Tween.LocalScaleY(Barras[3].transform.GetChild(0), 0, DuracionTransicion, TipoTransicionBotonesIn, default);
                    //Escala X
                    Tween.LocalScale(Ramas[3].transform.GetChild(1), 0, DuracionTransicion, TipoTransicionBotonesOut, default);
                    //Posicion Y Escala Arbol
                    Tween.LocalScale(Arbol.transform, new Vector3(1f, 1f, 1f), DuracionTransicion, TipoTransicionPaneles, default);
                    Tween.LocalPosition(Arbol.transform, Vector3.zero, DuracionTransicion, TipoTransicionPaneles, default);
                    break;
                }
        }
        RamaActual = "";
    }
    private void SeleccionarHabilidad()
    {
        // Añade una verificación para asegurarte de que no se vuelva a seleccionar de inmediato
        if (HabilidadActual != "" && Input.GetKeyDown(KeyCode.Mouse0) && !YaSelecciono)
        {
            YaSelecciono = true;
            BotonSeleccionado = BotonActual;
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
    public void EntraHabilidad(string Habilidad)
    {
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

    public void ActualizarBarrasPorRango()
    {
        foreach (GameObject Barra in Barras)
        {
            int rango = PlayerPrefs.GetInt("Rango " + Barra.name, 0);
            int CantBotones = int.Parse(Barra.name[Barra.name.Length - 1].ToString());
            int totalBotones = CantBotones * rango;

            int hijosTotales = Barra.transform.GetChild(0).childCount;

            for (int i = 0; i < hijosTotales; i++)
            {
                Image boton = Barra.transform.GetChild(0).GetChild(i).GetComponent<Image>();

                if (i < totalBotones)
                {
                    // Botón dentro del rango → blanco
                    boton.color = Color.white;
                }
                else
                {
                    // Botón fuera del rango → negro
                    boton.color = Color.black;
                }
            }
        }
    }


    public void InputPruebas()
    {
        //Prueba de rango
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            Debug.Log("Borra rangos de todas las ramas");
            foreach (GameObject barra in Barras)
            {
                string key = "Rango " + barra.name;
                PlayerPrefs.DeleteKey(key);
                Debug.Log($"Eliminado: {key}");
            }
            ActualizarBarrasPorRango();
        }

        // Verificar si se presiona Keypad2, Keypad3, Keypad4, Keypad5
        for (int i = 0; i < Barras.Length; i++)
        {
            KeyCode keyPadNumber = KeyCode.Keypad0 + (i + 1); // Keypad1 = Rama[0], Keypad2 = Rama[1] ...

            if (Input.GetKey(keyPadNumber))
            {
                string key = "Rango " + Barras[i].name;
                int rangoActual = PlayerPrefs.GetInt(key, 0);

                // Suma rango si además presiona +
                if (Input.GetKeyDown(KeyCode.KeypadPlus))
                {
                    rangoActual++;
                    PlayerPrefs.SetInt(key, rangoActual);
                    Debug.Log($"Rama {i + 1} aumenta: {key}, Rango actual: {rangoActual}");
                    ActualizarBarrasPorRango();
                }

                // Resta rango si además presiona -
                if (Input.GetKeyDown(KeyCode.KeypadMinus))
                {
                    rangoActual = Mathf.Max(0, rangoActual - 1); // Evitar rangos negativos
                    PlayerPrefs.SetInt(key, rangoActual);
                    Debug.Log($"Rama {i + 1} resta: {key}, Rango actual: {rangoActual}");
                    ActualizarBarrasPorRango();
                }
            }
        }
    }

}