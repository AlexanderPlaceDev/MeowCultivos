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
    [SerializeField] Scr_CreadorHabilidades[] Habilidades;
    [SerializeField] TextMeshProUGUI Puntos;

    string HabilidadSeleccionada;
    bool YaSelecciono = false;

    private RectTransform arbolRectTransform;


    void Start()
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
    }

    void Update()
    {
        Puntos.text = PlayerPrefs.GetInt("PuntosDeHabilidad", 0).ToString();
        SeleccionarHabilidad();

        if (HabilidadActual == null || HabilidadActual == "")
        {
            ActualizarArbol();
        }
        else
        {
            ActualizarHabilidad();
        }



    }

    private void SeleccionarHabilidad()
    {
        Debug.Log(HabilidadSeleccionada + HabilidadActual + YaSelecciono);
        // Añade una verificación para asegurarte de que no se vuelva a seleccionar de inmediato
        if (HabilidadSeleccionada == null && HabilidadActual != "" && Input.GetKeyDown(KeyCode.Mouse0) && !YaSelecciono)
        {
            YaSelecciono = true;
            HabilidadSeleccionada = HabilidadActual;
            Debug.Log("Habilidad Seleccionada");
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
                    ObjetoHabilidadSeleccionada.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =Habilidad.Nombre;
                    ObjetoHabilidadSeleccionada.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =Habilidad.Descripcion;
                    ObjetoHabilidadSeleccionada.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text ="Costo: "+Habilidad.Costo;

                }
            }
        }
    }

    private void ActualizarArbol()
    {
        Debug.Log("Habilidad Seleccionada");
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
                Boton.GetComponent<Image>().color = Color.white;
                Boton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                Boton.transform.GetChild(1).gameObject.SetActive(true);
                BotonActual = Boton;
                break;
            }
        }
    }

    public void SaleHabilidad()
    {
        HabilidadActual = "";
        BotonActual.transform.GetChild(1).gameObject.SetActive(false);

        if (PlayerPrefs.GetString("Habilidad:" + BotonActual.name, "No") == "No")
        {
            BotonActual.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            BotonActual.transform.GetChild(0).GetComponent<Image>().color = new Color32(50, 50, 50, 255);
        }
        BotonActual = null;
    }

    public void ComprarHabilidad()
    {
        if (BotonActual != null)
        {
            if (PlayerPrefs.GetString("Habilidad:" + BotonActual.name, "No") == "No")
            {
                PlayerPrefs.SetString("Habilidad:" + BotonActual.name, "Si");
            }
        }
    }


}
