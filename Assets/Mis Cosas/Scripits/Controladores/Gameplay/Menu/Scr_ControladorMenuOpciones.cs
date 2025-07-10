using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Scr_ControladorMenuOpciones : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI TextoVolumen;
    [SerializeField] Slider SliderVolumen;
    [SerializeField] TextMeshProUGUI TextoBrillo;
    [SerializeField] Slider SliderBrillo;

    void Start()
    {
        if (PlayerPrefs.HasKey("Volumen"))
        {
            SliderVolumen.value = PlayerPrefs.GetInt("Volumen", 50);
        }
        if (PlayerPrefs.HasKey("Brillo"))
        {
            SliderBrillo.value = PlayerPrefs.GetInt("Brillo", 50);
        }
    }

    void Update()
    {
        ActualizarOpciones();
    }

    void ActualizarOpciones()
    {

        TextoVolumen.text = (int)SliderVolumen.value + "%";
        TextoBrillo.text = (int)SliderBrillo.value + "%";
    }

    public void GuardarOpciones()
    {
        PlayerPrefs.SetInt("Volumen", (int)SliderVolumen.value);
        PlayerPrefs.SetInt("Brillo", (int)SliderBrillo.value);
    }
    public void ReiniciarOpciones()
    {
        SliderVolumen.value = 50;
        SliderBrillo.value = 50;
    }
}
