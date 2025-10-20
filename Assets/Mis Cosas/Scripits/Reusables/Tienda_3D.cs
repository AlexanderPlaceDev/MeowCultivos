using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    [SerializeField] GameObject Campana;
    [SerializeField] GameObject Marco;
    [SerializeField] GameObject CamaraTienda;
    [SerializeField] GameObject CamaraVenta;
    [SerializeField] GameObject AreaObjetosCompra; //Vertical Layout Group

    public List<int> objetosAvender;

    // 🔹 Variables internas
    private bool mouseSobreCampana = false;
    private Material[] materialesCampana;
    private Material[] materialesMarco;

    void Start()
    {
        // Asegurar collider
        if (Campana != null && Campana.GetComponent<Collider>() == null)
        {
            Campana.AddComponent<BoxCollider>();
        }

        // Guardar materiales (si tiene un MeshRenderer o SkinnedMeshRenderer)
        var renderer = Campana.GetComponent<Renderer>();
        if (renderer != null)
        {
            materialesCampana = renderer.materials;
        }
        materialesMarco = Marco.GetComponent<Renderer>().materials;
    }

    void Update()
    {
        DetectarCampanaHoverYClick();
    }

    private void DetectarCampanaHoverYClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool sobreCampana = false;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == Campana)
            {
                sobreCampana = true;

                // Si hace clic
                if (Input.GetMouseButtonDown(0))
                {
                    CamaraTienda.SetActive(false);
                    CamaraVenta.SetActive(true);
                }
            }
        }

        // Cambiar materiales solo si el estado de hover cambió
        if (sobreCampana && !mouseSobreCampana)
        {
            mouseSobreCampana = true;
            CambiarIntensidadMateriales(5f);
        }
        else if (!sobreCampana && mouseSobreCampana)
        {
            mouseSobreCampana = false;
            CambiarIntensidadMateriales(1f);
        }
    }

    private void CambiarIntensidadMateriales(float valor)
    {
        if (materialesCampana == null || materialesMarco==null) return;

        foreach (var mat in materialesCampana)
        {
            if (mat.HasProperty("_Intensidad"))
            {
                mat.SetFloat("_Intensidad", valor);
            }
        }
        foreach (var mat in materialesMarco)
        {
            if (mat.HasProperty("_Intensidad"))
            {
                mat.SetFloat("_Intensidad", valor);
            }
        }
    }

    public void nevosObjetos()
    {
        objetosAvender.Clear();
        for (int i = 0; i < objetosVender; i++)
        {
            int index = Random.Range(0, inventario.Objetos.Length - 1);
            objetosAvender.Add(index);
        }
    }

    // 🔹 Función pública para volver a la cámara de tienda
    public void VolverACamaraTienda()
    {
        CamaraTienda.SetActive(true);
        CamaraVenta.SetActive(false);
    }
}
