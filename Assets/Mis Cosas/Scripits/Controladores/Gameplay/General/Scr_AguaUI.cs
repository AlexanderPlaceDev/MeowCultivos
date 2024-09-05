using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_AguaUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextoCantidad;

    void Start()
    {

    }

    void Update()
    {
        if (PlayerPrefs.GetString("Habilidad:Regadera", "No") == "Si")
        {
            if (PlayerPrefs.GetInt("CantidadAgua", 0) > 0)
            {

                if (!transform.GetChild(0).gameObject.activeSelf)
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    TextoCantidad.text = PlayerPrefs.GetInt("CantidadAgua", 0) + "/" + PlayerPrefs.GetInt("CantidadAguaMaxima", 10);
                }
            }
            else
            {
                    transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
