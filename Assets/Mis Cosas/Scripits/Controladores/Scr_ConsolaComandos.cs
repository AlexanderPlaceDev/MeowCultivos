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
    [SerializeField] Scr_Inventario Inventario;
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

                    int Vol = PlayerPrefs.GetInt("Volumen", 70);
                    int VolM = PlayerPrefs.GetInt("Volumen_Musica", 70);
                    int VolA = PlayerPrefs.GetInt("Volumen_Ambiente", 70);
                    int VolC = PlayerPrefs.GetInt("Volumen_Combate", 70);
                    int Brillo = PlayerPrefs.GetInt("Brillo", 70);
                    // Borrar todos los datos guardados
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.SetString("Partida", "SI");


                    PlayerPrefs.SetInt("Volumen", Vol);
                    PlayerPrefs.SetInt("Volumen_Musica", VolM);
                    PlayerPrefs.SetInt("Volumen_Ambiente", VolA);
                    PlayerPrefs.SetInt("Volumen_Combate", VolC);
                    PlayerPrefs.SetInt("Brillo", Brillo);


                    PlayerPrefs.Save();
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
                    PlayerPrefs.SetString("Cinematica " + "Marvin", "No");
                    GameObject.Find("Gata").transform.GetChild(5).GetComponent<Scr_CargadorGuardado>().Personajes[1].SetActive(false);
                    break;
                }
            case "borrar.cinematica=2":
                {
                    Debug.Log("Cinemagica Borrada");
                    PlayerPrefs.SetString("Cinematica " + "Bony", "No");
                    GameObject.Find("Gata").transform.GetChild(5).GetComponent<Scr_CargadorGuardado>().Personajes[1].SetActive(false);
                    break;
                }
            
            case "borrar.cinematica=3":
                {
                    Debug.Log("Cinemagica Borrada");
                    PlayerPrefs.SetString("Cinematica " + "Presentacion Rex", "No");
                    break;
                }
            case "borrar.cinematica=4":
                {
                    Debug.Log("Cinemagica Borrada");
                    PlayerPrefs.SetString("Cinematica " + "Ovni", "No");
                    break;
                }
            case "borrar.estructura=0":
                {
                    Debug.Log("Estructura Borrada");
                    PlayerPrefs.SetInt("Estructura0",0);
                    break;
                }

            case "+3":
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
            case "rico":
                {
                    for(int i = 0; i < Inventario.Cantidades.Length; i++)
                    {
                        Inventario.Cantidades[i] = 100;
                    }
                    break;
                }
            case "azar":
                {
                    for (int i = 0; i < Inventario.Cantidades.Length; i++)
                    {
                        Inventario.Cantidades[i] = Random.Range(1,21);
                    }
                    break;
                }
            case "t8":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().HoraActual = 8;
                    break;
                }
            case "t12":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().HoraActual = 12;
                    break;
                }
            case "t18":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().HoraActual = 18;
                    break;
                }
            case "t22":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().HoraActual = 22;
                    break;
                }
            case "d1":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().DiaActual="LUN";
                    break;
                }
            case "d2":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().DiaActual = "MAR";
                    break;
                }
            case "d3":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().DiaActual = "MIE";
                    break;
                }
            case "d4":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().DiaActual = "JUE";
                    break;
                }
            case "d5":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().DiaActual = "VIE";
                    break;
                }
            case "d6":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().DiaActual = "SAB";
                    break;
                }
            case "d7":
                {
                    GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().DiaActual = "DOM";
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
