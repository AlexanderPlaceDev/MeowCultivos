using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scr_BotonesMenu : MonoBehaviour
{
    [SerializeField] Color[] Colores;
    Scr_ControladorMenu Controlador;

    bool EstaAdentro;
    private void Start()
    {
        Controlador = GameObject.Find("Controlador").GetComponent<Scr_ControladorMenu>();
    }

    private void Update()
    {
        if (Controlador.CreditosActivados && gameObject.name == "Creditos")
        {
            GetComponent<TextMeshProUGUI>().color = Colores[2];
        }
        else
        {
            if (Colores.Length > 0)
            {
                if (Controlador.EstaEnOpciones)
                {
                    GetComponent<TextMeshProUGUI>().color = Colores[2];
                }
                else
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

    public void Creditos()
    {
        if (!Controlador.CreditosActivados)
        {
            StartCoroutine(Controlador.Creditos());
        }
    }

    public void CerrarJuego()
    {
        Application.Quit();
    }

    public void Jugar()
    {
        if (!Controlador.EstaEnOpciones)
        {
            Tween.Color(GameObject.Find("Pantalla").GetComponent<SpriteRenderer>(), Color.black, 4, Ease.Default, cycles: 1);
            StartCoroutine(CambiarEscena());
        }

    }
    IEnumerator CambiarEscena()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(1);
    }




}

