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

                    // Borrar todos los datos guardados
                    PlayerPrefs.DeleteAll();

                    // Aquí vamos a activar manualmente los primeros 4 mapas
                    int numeroMapasActivos = 5; // La cantidad de mapas que queremos mantener activos
                    Scr_ControladorMapas controladorMapas = FindObjectOfType<Scr_ControladorMapas>(); // Asegúrate que el script está en la escena

                    if (controladorMapas != null && controladorMapas.EsMapa)
                    {
                        // Iteramos a través de los hijos del objeto que contiene los mapas
                        for (int i = 0; i < controladorMapas.transform.childCount; i++)
                        {
                            Transform child = controladorMapas.transform.GetChild(i);
                            GameObject mapa = child.gameObject;

                            // Si es uno de los primeros 4 mapas, lo activamos y guardamos su estado
                            if (i < numeroMapasActivos)
                            {
                                mapa.SetActive(true);
                                PlayerPrefs.SetString("MapaActivo:" + mapa.name, "Si"); // Guardamos el estado como activo
                            }
                            else
                            {
                                mapa.SetActive(false);
                                PlayerPrefs.SetString("MapaActivo:" + mapa.name, "No"); // Guardamos el estado como desactivado
                            }
                        }
                    }

                    // Otras acciones relacionadas con la eliminación de datos
                    Camera.main.transform.GetChild(0).GetComponent<Scr_BarrasNegras>().Awake();
                    Camera.main.transform.GetChild(1).GetComponent<Scr_BarrasNegras>().Awake();

                    // Recargar la escena (opcional)
                    SceneManager.LoadScene(2);
                    break;
                }
            case "borrar.cinematica=1":
                {
                    Debug.Log("Cinemagica Borrada");
                    PlayerPrefs.SetString("Cinematica " + "Bony", "No");
                    GameObject.Find("Gata").transform.GetChild(4).GetComponent<Scr_CargadorGuardado>().Personajes[1].SetActive(false);
                    break;
                }
            case "borrar.estructura=0":
                {
                    Debug.Log("Estructura Borrada");
                    PlayerPrefs.SetInt("Estructura0",0);
                    break;
                }

            case "+10":
                {
                    Debug.Log("Puntos Adquiridos");
                    PlayerPrefs.SetInt("PuntosDeHabilidad", PlayerPrefs.GetInt("PuntosDeHabilidad",0)+3);
                    break;
                }

            case "borrar.agua":
                {
                    Debug.Log("Sin Agua");
                    PlayerPrefs.SetInt("CantidadAgua", 0);
                    break;
                }
            case "agua+10":
                {
                    Debug.Log("Agrega Agua");
                    PlayerPrefs.SetInt("CantidadAgua", 10);
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
