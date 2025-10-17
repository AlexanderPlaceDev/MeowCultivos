using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tienda_3D : MonoBehaviour
{
    public GameObject[] Obetoscompra;
    public int objetosVender;
    public int cantidad;
    public int precio;
    public float descuento = 20;
    public TextMeshProUGUI Dinero;
    public Scr_Inventario inventario;
    public GameObject FoodTruck;

    public List<int> objetosAvender;

    // ==============================
    // 🔹 Reinicio de objetos de tienda
    // ==============================
    public void nevosObjetos()
    {
        objetosAvender.Clear();
        for (int i = 0; i < objetosVender; i++)
        {
            int index = Random.Range(0, inventario.Objetos.Length - 1);
            objetosAvender.Add(index);
        }
    }
}
