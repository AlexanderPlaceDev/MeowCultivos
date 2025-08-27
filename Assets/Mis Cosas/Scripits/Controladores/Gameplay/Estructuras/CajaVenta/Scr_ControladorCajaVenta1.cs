using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorCajaVenta1 : MonoBehaviour
{
    public bool EntregaRegalo;
    [SerializeField] private GameObject Regalo;


    void Start()
    {
        if (PlayerPrefs.GetString("CajaVentaRegalo", "No") == "Si")
        {
            EntregaRegalo = true;
        }
    }

    void Update()
    {
        if (EntregaRegalo && !Regalo.activeSelf)
        {
            Regalo.SetActive(true);
        }
    }
}
