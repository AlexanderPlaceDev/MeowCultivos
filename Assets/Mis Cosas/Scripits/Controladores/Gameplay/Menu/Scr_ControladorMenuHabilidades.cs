using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorMenuHabilidades : MonoBehaviour
{
    [SerializeField] GameObject Arbol;
    [SerializeField] float moveSpeed;
    [SerializeField] float scaleFactor;
    [SerializeField] GameObject[] Botones;
    public GameObject BotonActual;

    public string HabilidadActual;

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
        // Cambiar tamaño del árbol con la rueda del mouse
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            if (arbolRectTransform.localScale.x > 0f || scrollInput>0)
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

        if (BotonActual != null)
        {
            if (PlayerPrefs.GetString("Habilidad:" + BotonActual.name, "No") == "No" && Input.GetMouseButtonDown(0))
            {
                PlayerPrefs.SetString("Habilidad:" + BotonActual.name, "Si");
            }
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
}
