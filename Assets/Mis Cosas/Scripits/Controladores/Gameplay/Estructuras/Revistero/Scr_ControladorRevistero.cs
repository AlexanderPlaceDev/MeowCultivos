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

    // NUEVO: control de revistas disponibles
    // false = disponible | true = bloqueada
    [SerializeField] bool[] RevistasDisponibles = new bool[4];

    void Start()
    {
        ActualizarDineroYPrecio();
    }

    void Update()
    {
        if (!UI.gameObject.activeSelf)
            return;

        int maxComprable = ObtenerMaximoComprable();
        int rangoInterno = PlayerPrefs.GetInt("Rango Barra Tecnica6", 0);

        if (rangoInterno < maxComprable)
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
        int maxComprable = ObtenerMaximoComprable();

        if (rangoInterno >= maxComprable)
            return;

        int precio = PrecioDeRangos[rangoInterno];

        if (dineroActual < precio)
            return;

        // Resta dinero
        PlayerPrefs.SetInt("Dinero", dineroActual - precio);

        // Aumenta rango
        PlayerPrefs.SetInt("Rango Barra Tecnica6", rangoInterno + 1);

        CerrarUI();

        GameObject.Find("Gata")
            .transform.GetChild(6)
            .GetComponent<Scr_ControladorMenuHabilidades>()
            .ActualizarBarrasPorRango();

        // Mostrar rango visual
        GameObject.Find("Canvas").transform.GetChild(10).gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.GetChild(10)
            .GetComponent<Scr_NuevoRango>()
            .MostrarRango("Tecnica", rangoInterno + 1);
    }

    void ActualizarDineroYPrecio()
    {
        int rangoInterno = PlayerPrefs.GetInt("Rango Barra Tecnica6", 0);
        int dinero = PlayerPrefs.GetInt("Dinero", 0);
        int maxComprable = ObtenerMaximoComprable();

        TextoTuDinero.text = "$" + dinero;

        if (rangoInterno >= maxComprable)
            return;

        int precio = PrecioDeRangos[rangoInterno];

        TextoDinero.text = "$" + precio;
        BotonAceptar.SetActive(dinero >= precio);
    }

    // NUEVO: calcula cuántas revistas se pueden comprar
    int ObtenerMaximoComprable()
    {
        int contador = 0;

        for (int i = 0; i < RevistasDisponibles.Length; i++)
        {
            if (!RevistasDisponibles[i])
                contador++;
            else
                break;
        }

        return contador;
    }
}
