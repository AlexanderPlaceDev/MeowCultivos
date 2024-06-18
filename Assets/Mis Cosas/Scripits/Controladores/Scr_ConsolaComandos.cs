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

    [SerializeField] Scr_CreadorObjetos[] TodosLosObjetos;
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
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            ValidarCodigo();
            Palabra = "";
            transform.GetChild(0).gameObject.SetActive(false);
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
            if (Regex.IsMatch(Tecla, @"^[a-zA-Z0-9_+-=\s]+$"))
            {
                Palabra += Tecla;
            }
        }
    }

    void ValidarCodigo()
    {
        //Casos Variables
        if (Palabra.ToLower().Contains("objeto."))
        {
            Palabra = Palabra.Substring(7);
            if (Palabra.Length > 0)
            {
                string[] split = Palabra.Split("=");

                if (split.Length == 2)
                {
                    foreach (Scr_CreadorObjetos objeto in TodosLosObjetos)
                    {
                        if (objeto.Nombre.ToLower() == split[0].ToLower())
                        {
                            if (int.TryParse(split[1], out int Resultado))
                            {

                            }
                        }
                    }
                }
            }
        }

        //Casos Exactos
        switch (Palabra.ToLower().Trim())
        {
            case "borrar.datos":
                {
                    Debug.Log("Datos Borrados");
                    Camera.main.transform.GetChild(1).GetComponent<Scr_BarrasNegras>().Start();
                    Camera.main.transform.GetChild(2).GetComponent<Scr_BarrasNegras>().Start();
                    PlayerPrefs.DeleteAll();
                    SceneManager.LoadScene(2);
                    break;
                }
            case "borrar.cinematica=1":
                {
                    Debug.Log("Cinemagica Borrada");
                    PlayerPrefs.SetString("Cinematica " + "Bony", "No");
                    break;
                }
            default:
                {
                    Debug.Log("No se encontro el comando");
                    break;
                }
        }
    }
}
