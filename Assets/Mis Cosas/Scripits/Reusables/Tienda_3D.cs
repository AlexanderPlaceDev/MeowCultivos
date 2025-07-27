using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tienda_3D : MonoBehaviour
{
    //public GameObject personajePrefab;
    public GameObject[] Obetoscompra; 
    //public Transform contentPanel;
    public int objetosVender;
    public int cantidad;
    public int precio;
    public float descuento = 20;
    public TextMeshProUGUI Dinero;
    private Transform Gata;
    public   Scr_Inventario inventario;


    public List<int> objetosAvender;
    //public Button but;
    // Start is called before the first frame update
    void Start()
    {
        //Gata = GameObject.Find("Gata").transform;
        //inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
        //MostrarObjetos();
    }
    void OnEnable()
    {
        //Gata = GameObject.Find("Gata").transform;
        //inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
        MostrarObjetos();
    }
    // Update is called once per frame
    void Update()
    {
        Dinero.text = "Dinero:" + PlayerPrefs.GetInt("Dinero", 0);
    }

    //se llama cada vez que se quiera reiniciar los objetos a vender
    public void nevosObjetos()
    {
        objetosAvender.Clear();
        for (int i = 0; i < objetosVender; i++)
        {
            int index = Random.Range(0, inventario.Objetos.Length - 1);
            objetosAvender.Add(index);
        }
    }


    //Muestra los objetos
    void MostrarObjetos()
    {

        List<int> numeros = Enumerable.Range(0, Obetoscompra.Length).ToList();
        numeros = numeros.OrderBy(x => Random.value).ToList();

        int num1 = numeros[0];
        int num2 = numeros[1];

        Debug.Log("tiene descuento el lugar " + num1 + " y " + num2);
        //Debug.Log(objetosAvender.Count + "+++++++"+ Obetoscompra.Length);
        for (int i = 0; i < objetosAvender.Count; i++)
        {
            Scr_CreadorObjetos instance = inventario.Objetos[objetosAvender[i]];

            Image image = Obetoscompra[i].transform.Find("Objeto").GetComponent<Image>();
            image.sprite = instance.Icono;

            Transform comp = Obetoscompra[i].transform.Find("BComprar");

            TextMeshProUGUI texto = comp.transform.Find("Precio").GetComponent<TextMeshProUGUI>();
            EventTrigger boton = Obetoscompra[i].transform.Find("BComprar").GetComponent<EventTrigger>();

            texto.text = $"{instance.Nombre} x{cantidad}";

            boton.triggers.Clear();

            int costo = precio;
            if (i == num1 || i == num2)
            {
                float desc = (100 - descuento) / 100f;
                costo = Mathf.RoundToInt(costo * desc);
                Debug.Log(costo + " tiene descuento en el índice " + i);
            }

            boton.GetComponentInChildren<TextMeshProUGUI>().text = $"{costo}";

            // CORRECCIÓN CLAVE AQUÍ:
            int iCopia = i;
            int costoCopia = costo;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => {
                comprarobjeto(objetosAvender[iCopia], costoCopia);
            });

            boton.triggers.Add(entry);

            // Color del texto según si puede comprar o no
            if (PlayerPrefs.GetInt("Dinero", 0) >= costo)
            {
                boton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                boton.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
            }
        }
    }

    void comprarobjeto(int index, int costo)
    {
        if (PlayerPrefs.GetInt("Dinero", 0) >= costo)
        {
            for (int i = 0; i < inventario.Objetos.Length; i++)
            {
                if (inventario.Objetos[i] == inventario.Objetos[index])
                {
                    inventario.Cantidades[index] += cantidad;
                    int newdinero = PlayerPrefs.GetInt("Dinero", 0) - costo;
                    PlayerPrefs.SetInt("Dinero", newdinero);
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Jaja no money");
        }
    }
}
