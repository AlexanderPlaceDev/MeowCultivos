using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using Unity.VisualScripting;

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
    [SerializeField] GameObject Medallas;
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
    public void SeleccionarHabilidad()
    {
        // Añade una verificación para asegurarte de que no se vuelva a seleccionar de inmediato
        if (HabilidadActual != "" && !YaSelecciono)
        {
            Debug.Log("Entra");
            YaSelecciono = true;
            BotonSeleccionado = BotonActual;



            Debug.Log("Habilidad Seleccionada" + BotonSeleccionado);

            foreach (Scr_CreadorHabilidades Habilidad in Habilidades)
            {
                if (Habilidad.NombreBoton == HabilidadActual)
                {
                    ObjetoHabilidadSeleccionada.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Habilidad.Icono;
                    ObjetoHabilidadSeleccionada.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Habilidad.Nombre;
                    ObjetoHabilidadSeleccionada.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Habilidad.Descripcion;
                    ObjetoHabilidadSeleccionada.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Costo:" + Habilidad.Costo;


                    //Actualiza los items necesarios
                    if (Habilidad.RequiereItems)
                    {
                        ObjetoHabilidadSeleccionada.transform.GetChild(4).gameObject.SetActive(true);
                        int c = 0;
                        foreach (Transform Hijo in ObjetoHabilidadSeleccionada.transform.GetChild(4).GetComponentInChildren<Transform>())
                        {
                            if (c < Habilidad.CantidadesRequeridas.Length)
                            {
                                Hijo.gameObject.SetActive(true);
                                Hijo.GetComponent<Image>().sprite = Habilidad.ItemsRequeridos[c].IconoInventario;
                                Hijo.GetChild(0).GetComponent<TextMeshProUGUI>().text = Habilidad.CantidadesRequeridas[c].ToString();

                            }
                            else
                            {
                                Hijo.gameObject.SetActive(false);
                            }
                            c++;
                        }
                    }
                    else
                    {
                        ObjetoHabilidadSeleccionada.transform.GetChild(4).gameObject.SetActive(false);
                    }

                    if (Habilidad.RequiereMedallas)
                    {
                        Medallas.SetActive(true);
                        switch (PlayerPrefs.GetInt("Rango " + Habilidad.NombreBoton, 0))
                        {
                            case 0:
                                {
                                    Medallas.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
                                    break;
                                }
                            case 1:
                                {
                                    Medallas.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
                                    break;
                                }
                            case 2:
                                {
                                    Medallas.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
                                    break;
                                }
                            case 3:
                                {
                                    Medallas.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
                                    break;
                                }
                            case 4:
                                {
                                    Medallas.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
                                    Medallas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
                                    Medallas.transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
                                    break;
                                }
                        }
                    }


                    //Activa o desactiva el boton Aceptar
                    if (PlayerPrefs.GetInt("PuntosDeHabilidad", 0) >= Habilidad.Costo && PlayerPrefs.GetString("Habilidad:" + HabilidadActual, "No") == "No")
                    {
                        //Regresa al alpha original en caso de no tenerla comprada
                        Color color = BotonSeleccionado.GetComponent<Image>().color;
                        color.a = 100f / 255f;
                        BotonSeleccionado.GetComponent<Image>().color = color;
                        //En caso de requerir itmes actualiza la info de estos
                        if (Habilidad.RequiereItems)
                        {
                            Scr_Inventario Inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();

                            bool Pasa = true; // Lo inicializamos en true y lo ponemos en false si falta algo
                            for (int N1 = 0; N1 < Habilidad.ItemsRequeridos.Length; N1++)
                            {
                                Scr_CreadorObjetos itemRequerido = Habilidad.ItemsRequeridos[N1];
                                bool encontrado = false;

                                for (int N = 0; N < Inventario.Objetos.Length; N++)
                                {
                                    if (itemRequerido == Inventario.Objetos[N])
                                    {
                                        encontrado = true;
                                        if (Inventario.Cantidades[N] >= Habilidad.CantidadesRequeridas[N1])
                                        {
                                            ObjetoHabilidadSeleccionada.transform.GetChild(4).GetChild(N1).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                                        }
                                        else
                                        {
                                            ObjetoHabilidadSeleccionada.transform.GetChild(4).GetChild(N1).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                                            Pasa = false; // No tienes la cantidad suficiente
                                        }
                                        break;
                                    }
                                }

                                if (!encontrado) // No lo tienes en el inventario
                                {
                                    Pasa = false;
                                    ObjetoHabilidadSeleccionada.transform.GetChild(4).GetChild(N1).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                                }
                            }

                            BotonAceptar.SetActive(Pasa);
                        }
                        else
                        {
                            BotonAceptar.SetActive(true);
                        }

                    }
                    else
                    {
                        BotonAceptar.SetActive(false);
                    }
                    break;
                }
            }

            ObjetoHabilidadSeleccionada.SetActive(true);
        }
    }
    public void ComprarHabilidad()
    {
        Debug.Log($"[DEBUG] Intentando comprar: {HabilidadActual}");

        if (string.IsNullOrEmpty(HabilidadActual))
        {
            Debug.LogWarning("[DEBUG] No hay habilidad seleccionada para comprar.");
            return;
        }

        foreach (Scr_CreadorHabilidades habilidad in Habilidades)
        {
            if (habilidad.NombreBoton == HabilidadActual)
            {
                int puntosActuales = PlayerPrefs.GetInt("PuntosDeHabilidad", 0);

                Debug.Log($"[DEBUG] Puntos actuales: {puntosActuales}, Costo: {habilidad.Costo}");

                if (puntosActuales >= habilidad.Costo && PlayerPrefs.GetString("Habilidad:" + HabilidadActual, "No") == "No")
                {
                    //Quitar items
                    if (habilidad.RequiereItems)
                    {
                        int Obj = 0;
                        foreach (Scr_CreadorObjetos Objeto in habilidad.ItemsRequeridos)
                        {
                            GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>().QuitarObjeto(habilidad.CantidadesRequeridas[Obj], habilidad.ItemsRequeridos[Obj].Nombre);
                            Obj++;
                        }
                    }

                    // Guardar la compra
                    PlayerPrefs.SetString("Habilidad:" + HabilidadActual, "Si");

                    // Restar puntos
                    puntosActuales -= habilidad.Costo;
                    PlayerPrefs.SetInt("PuntosDeHabilidad", puntosActuales);

                    // Guardar cambios en disco
                    PlayerPrefs.Save();

                    Debug.Log($"[DEBUG] Habilidad '{habilidad.NombreBoton}' comprada. Puntos restantes: {puntosActuales}");

                    // Actualizar barras
                    ActualizarBarrasPorRango();

                    // Ocultar el panel de selección
                    YaSelecciono = false;
                    ObjetoHabilidadSeleccionada.SetActive(false);
                    BotonAceptar.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("[DEBUG] No tienes puntos suficientes o la habilidad ya está comprada.");
                }

                return; // Salir del foreach
            }
        }

        YaSelecciono = false;
        BotonActual = null;
        HabilidadActual = "";
        ObjetoHabilidadSeleccionada.SetActive(false);
        BotonAceptar.SetActive(false);

    }
    public void RechazarHabilidad()
    {
        Debug.Log("Rechaza Habilidad");

        YaSelecciono = false;
        BotonActual = null;
        HabilidadActual = "";
        ObjetoHabilidadSeleccionada.SetActive(false);

    }
    public void EntraHabilidad(GameObject boton)
    {
        if (YaSelecciono) return;

        Debug.Log($"[DEBUG] EntraHabilidad() → Botón: {boton.name}");

        // Verificar si está desbloqueado por rango
        if (boton.GetComponent<Image>().color == Color.black)
        {
            Debug.Log("[DEBUG] Botón fuera de rango, no se cambia alpha.");
            return; // No hacer nada si está bloqueado
        }

        BotonActual = boton;
        HabilidadActual = boton.name;

        // Cambiar alpha de la imagen a 255 (totalmente visible)
        Image imagenBoton = boton.GetComponent<Image>();
        if (imagenBoton != null)
        {
            Color color = imagenBoton.color;
            color.a = 1f;
            imagenBoton.color = color;
        }

        Debug.Log($"[DEBUG] HabilidadActual asignada: {HabilidadActual}, BotonActual: {BotonActual.name}");
    }

    public void SaleHabilidad()
    {
        if (YaSelecciono) return;

        Debug.Log($"[DEBUG] SaleHabilidad() → Saliendo de {HabilidadActual}");

        HabilidadActual = "";
        if (BotonActual != null)
        {
            if (PlayerPrefs.GetString("Habilidad:" + BotonActual.name, "No") == "No")
            {
                Image imagenBoton = BotonActual.GetComponent<Image>();
                if (imagenBoton != null)
                {
                    Color color = imagenBoton.color;
                    color.a = 100f / 255f;
                    imagenBoton.color = color;
                }
            }

            Debug.Log($"[DEBUG] Botón {BotonActual.name} reseteado.");
            BotonActual = null;
        }
    }
    public void ActualizarBarrasPorRango()
    {
        foreach (GameObject Barra in Barras)
        {
            int rango = PlayerPrefs.GetInt("Rango " + Barra.name, 0);
            int hijosTotales = Barra.transform.GetChild(0).childCount;

            int totalBotones = 0;

            // EXCEPCIÓN: Arsenal3 tiene una habilidad gratis al inicio
            if (Barra.name == "Barra Arsenal3")
            {
                // Primer botón siempre desbloqueado
                totalBotones = 1 + (rango * 3);
                // Ajusta el 3 si cada rango desbloquea otra cantidad
            }
            else
            {
                // Método normal para las demás ramas
                int CantBotones = int.Parse(Barra.name[Barra.name.Length - 1].ToString());
                totalBotones = CantBotones * rango;
            }

            for (int i = 0; i < hijosTotales; i++)
            {
                Image boton = Barra.transform.GetChild(0).GetChild(i).GetComponent<Image>();

                if (i < totalBotones)
                {
                    string nombreHabilidad = boton.name;
                    bool estaComprada = PlayerPrefs.GetString("Habilidad:" + nombreHabilidad, "No") == "Si";

                    if (estaComprada)
                        boton.color = Color.white;
                    else
                    {
                        Color c = Color.white;
                        c.a = 100f / 255f;
                        boton.color = c;
                    }
                }
                else
                {
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