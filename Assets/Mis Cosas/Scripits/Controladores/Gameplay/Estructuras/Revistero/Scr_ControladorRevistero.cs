using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_ControladorRevistero : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextoTuDinero;
    [SerializeField] TextMeshProUGUI TextoInserta;
    [SerializeField] TextMeshProUGUI TextoDinero;
    [SerializeField] TextMeshProUGUI TextoDescripcion;
    [SerializeField] GameObject UI;
    [SerializeField] private int[] PrecioDeRangos;
    [SerializeField] GameObject BotonAceptar;

    void Start()
    {
        ActualizarDineroYPrecio();
    }

    void Update()
    {
        if (!UI.gameObject.activeSelf)
            return;

        if (PlayerPrefs.GetInt("Rango Barra Tecnica6", 0) < PrecioDeRangos.Length)
        {
            ActualizarDineroYPrecio();
        }
        else
        {
            TextoDinero.gameObject.SetActive(false);
            TextoInserta.gameObject.SetActive(false);
            TextoDescripcion.text = "Ya no quedan libros utiles";
            BotonAceptar.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.SetInt("Dinero", PlayerPrefs.GetInt("Dinero", 0) + 1000);
        }
    }

    public void CerrarUI()
    {
        UI.SetActive(false);
        GetComponent<Scr_ActivadorMenuEstructuraFijo>().CerrarTablero();
    }

    public void Comprar()
    {
        int rangoInterno = PlayerPrefs.GetInt("Rango Barra Tecnica6", 0);
        int dineroActual = PlayerPrefs.GetInt("Dinero", 0);

        if (rangoInterno >= PrecioDeRangos.Length)
            return;

        int precio = PrecioDeRangos[rangoInterno];

        if (dineroActual < precio)
            return;

        // Resta correcta
        PlayerPrefs.SetInt("Dinero", dineroActual - precio);

        // Sube rango interno
        PlayerPrefs.SetInt("Rango Barra Tecnica6", rangoInterno + 1);

        CerrarUI();

        GameObject.Find("Gata")
            .transform.GetChild(6)
            .GetComponent<Scr_ControladorMenuHabilidades>()
            .ActualizarBarrasPorRango();

        // Mostrar rango VISUAL (interno + 1)
        GameObject.Find("Canvas").transform.GetChild(10).gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.GetChild(10)
            .GetComponent<Scr_NuevoRango>()
            .MostrarRango("Tecnica", rangoInterno + 1);
    }

    void ActualizarDineroYPrecio()
    {
        int rangoInterno = PlayerPrefs.GetInt("Rango Barra Tecnica6", 0);
        int dinero = PlayerPrefs.GetInt("Dinero", 0);

        TextoTuDinero.text = "$" + dinero;

        if (rangoInterno >= PrecioDeRangos.Length)
            return;

        int precio = PrecioDeRangos[rangoInterno];

        TextoDinero.text = "$" + precio;
        BotonAceptar.SetActive(dinero >= precio);
    }
}
