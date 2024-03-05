using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Windows;
using UnityEngine.SceneManagement;

public class Scr_ConsolaComandos : MonoBehaviour
{

    string PalabraClave = "truco";
    string Palabra = "";
    TextMeshProUGUI Texto;
    void Start()
    {
        Texto = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Validar();
        if (Palabra.ToLower() == PalabraClave)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            Palabra = "";
        }
        if(UnityEngine.Input.GetKeyDown(KeyCode.Return)) 
        {
            transform.GetChild(0).gameObject.SetActive(false);
            ValidarCodigo();
            Palabra = "";
        }
        Texto.text = Palabra;

    }

    void Validar()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Backspace))
        {
            if (Palabra.Length > 0)
            {
                Palabra = Palabra.Remove(Palabra.Length - 1);
            }
        }
        else
        {
            string Tecla = UnityEngine.Input.inputString.ToString();
            if (Regex.IsMatch(Tecla, @"^[a-zA-Z0-9_+-=]+$"))
            {
                Palabra += Tecla;
            }
        }
    }

    void ValidarCodigo()
    {
        switch (Palabra.ToLower())
        {
            case "borrar.datos":
                {
                    Debug.Log("Datos Borrados");
                    PlayerPrefs.DeleteAll();
                    SceneManager.LoadScene(2);
                    break;
                }
        }
    }
}
