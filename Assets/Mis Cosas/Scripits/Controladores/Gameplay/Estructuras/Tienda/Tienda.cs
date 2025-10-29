using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class Tienda : MonoBehaviour
{
    public GameObject personajePrefab; 
    public Transform contentPanel;
    public int objetosVender;
    public int cantidad;
    public int precio;
    public float descuento=20;
    public TextMeshProUGUI Dinero;
    private Transform Gata;
    private Scr_Inventario inventario;
    public Button but;
    public List<int> objetosAvender;
    // Start is called before the first frame update
    void Start()
    {
        Gata = GameObject.Find("Gata").transform;
        inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
        MostrarObjetos();
    }
    void OnEnable()
    {
        Gata = GameObject.Find("Gata").transform;
        inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
        MostrarObjetos();
    }
    // Update is called once per frame
    void Update()
    {
        Dinero.text = "Dinero: "+PlayerPrefs.GetInt("Dinero", 0);
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
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        List<int> numeros = Enumerable.Range(0, objetosVender).ToList();
        numeros = numeros.OrderBy(x => Random.value).ToList(); // Mezclar aleatoriamente

        int num1 = numeros[0];
        int num2 = numeros[1];

        Debug.Log("tiene descuento el lugar" + num1 + " aaaaaaa "+ num2);

        for (int i = 0; i < objetosVender; i++)
        {

            int index = Random.Range(0, inventario.Objetos.Length - 1);
            Scr_CreadorObjetos instance = inventario.Objetos[index];

            GameObject obj = Instantiate(personajePrefab, contentPanel);
            Image image = obj.transform.Find("Objetoa").GetComponent<Image>();
            TextMeshProUGUI texto = obj.GetComponentInChildren<TextMeshProUGUI>();
            Button boton = obj.GetComponentInChildren<Button>();

            image.sprite=inventario.Objetos[index].Icono;
            texto.text = $"{instance.Nombre} x{cantidad}";

            boton.onClick.RemoveAllListeners();
            int costo = precio;
            if (i==num1 || i==num2)
            {
                float desc = (100 - descuento) / 100;
                costo = (int)(costo * desc);
                Debug.Log(costo + "tiene descuento"+i );
            }

            boton.GetComponentInChildren<TextMeshProUGUI>().text = $"{costo}";
            boton.onClick.AddListener(() =>
            {
                comprarobjeto(index, costo);
            });
            if(PlayerPrefs.GetInt("Dinero", 0) >= precio)
            {
                boton.interactable = true;
            }
            else
            {
                boton.interactable = false;
            }
        }
    }

    void comprarobjeto(int index, int costo)
    {
        if (PlayerPrefs.GetInt("Dinero", 0)>= costo)
        {
            for (int i = 0; i < inventario.Objetos.Length; i++)
            {
                if (inventario.Objetos[i] == inventario.Objetos[index])
                {
                    inventario.Cantidades[index] += cantidad;
                    int newdinero= PlayerPrefs.GetInt("Dinero", 0) - precio;
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
