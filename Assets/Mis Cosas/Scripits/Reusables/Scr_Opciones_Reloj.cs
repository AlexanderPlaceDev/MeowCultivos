using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scr_Opciones_Reloj : MonoBehaviour
{
    [SerializeField] Color[] Colores;
    bool EstaAdentro;
    private void Start()
    {
    }

    private void Update()
    {
        if (Colores.Length > 0)
        {
            if (EstaAdentro)
            {
                GetComponent<TextMeshProUGUI>().color = Colores[0];
            }
            else
            {
                GetComponent<TextMeshProUGUI>().color = Colores[1];
            }
        }

    }

    public void Entrar()
    {
        EstaAdentro = true;
    }

    public void Salir()
    {
        EstaAdentro = false;
    }


}
