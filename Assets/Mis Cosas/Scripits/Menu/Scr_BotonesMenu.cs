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
        if (Controlador.ActivarCreditos && gameObject.name == "Creditos")
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
        if (!Controlador.ActivarCreditos)
        {

            Controlador.ActivarCreditos = true;
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
            GameObject.Find("Autobus").GetComponent<Animator>().Play("Avanzar");
            Tween.Color(GameObject.Find("Pantalla").GetComponent<SpriteRenderer>(), Color.black, 4, Ease.Default, cycles: 1);
            StartCoroutine(CambiarEscena());
        }

    }
    IEnumerator CambiarEscena()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(1);
    }

    public void Opciones()
    {
        if (!Controlador.EstaEnOpciones)
        {
            GameObject.Find("Autobus").GetComponent<Animator>().Play("Retroceder");
            Controlador.EstaEnOpciones = true;

        }
    }

    public void EntrarGuardarOpciones()
    {
        GetComponent<Image>().color = Color.white;
    }

    public void SalirGuardarOpciones()
    {
        GetComponent<Image>().color = new Color32(209, 209, 209, 180);
    }

    public void CerrarOpciones()
    {
        if (Controlador.EstaEnOpciones && Camera.main.transform.GetChild(1).GetChild(1).GetComponent<Image>().color.a>=0.62)
        {
            Camera.main.transform.GetChild(1).GetComponent<Animator>().Play("Desaparecer");
            StartCoroutine(RegresarOpciones());
        }
    }

    IEnumerator RegresarOpciones()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject Suelo in GameObject.FindGameObjectsWithTag("SueloMenu"))
        {
            Debug.Log("Entra");
            Suelo.GetComponent<Rigidbody>().velocity = new Vector3(Controlador.Velocidad, 0, 0);
        }
        GameObject.Find("Autobus").transform.GetChild(0).GetComponent<Scr_GirarObjeto>().VelocidadGeneral = -1;
        GameObject.Find("Autobus").transform.GetChild(1).GetComponent<Scr_GirarObjeto>().VelocidadGeneral = -1;
        GameObject.Find("Autobus").transform.GetChild(2).GetComponent<Scr_GirarObjeto>().VelocidadGeneral = -1;
        GameObject.Find("Autobus").transform.GetChild(3).GetComponent<Scr_GirarObjeto>().VelocidadGeneral = -1;
        GameObject.Find("Autobus").GetComponent<Animator>().Play("Regresar");
        yield return new WaitForSeconds(2);
        Camera.main.transform.GetChild(1).GetComponent<Animator>().Play("New State");
        Controlador.EstaEnOpciones = false;
    }
}

