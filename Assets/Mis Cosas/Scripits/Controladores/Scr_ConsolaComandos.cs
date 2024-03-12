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
    Scr_ObjetoEnMano ObjetoEnMano;

    [SerializeField] Scr_CreadorObjetos[] TodosLosObjetos;
    void Start()
    {
        Texto = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        ObjetoEnMano = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(1).GetComponent<Scr_ObjetoEnMano>();
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
                                if (ObjetoEnMano.Nombre == "")
                                {
                                    ObjetoEnMano.Nombre = objeto.Nombre;
                                    ObjetoEnMano.Cantidad = Resultado;
                                    ObjetoEnMano.Forma = objeto.Forma;
                                    int j = 0;
                                    for (int i = 0; i < objeto.Forma.Length; i++)
                                    {
                                        if (objeto.Forma[i])
                                        {
                                            ObjetoEnMano.Iconos[i] = objeto.IconosInventario[j];
                                            j++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //Casos Exactos
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
