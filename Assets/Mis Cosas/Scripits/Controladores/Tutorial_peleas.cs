using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_peleas : MonoBehaviour
{

    private Scr_SistemaDialogos sistemaDialogos;
    public GameObject Dialogo;
    public GameObject Fondo;

    public GameObject Peleas;

    public string[] NombreNPC;
    public Color[] ColorDialogo;
    public TextMeshProUGUI Mensaje;

    public float velocidad = 3f; // Velocidad del parpadeo
    public bool ColorNormal = false;
    public Color colorA = Color.white;
    public Color colorB = Color.black;

    public Color ColorAceptarNormal;
    public GameObject[] Botones;
    Scr_ControladorBatalla ControladorBatalla;

    public GameObject[] BotonesPelea;
    public float interval = 0.5f;
    private float timer = 0f;
    private Coroutine parpadeo;
    // Start is called before the first frame update
    void Start()
    {
        sistemaDialogos = GetComponent<Scr_SistemaDialogos>();
        if (PlayerPrefs.GetString("TutorialPeleas", "NO")=="SI")
        {
            gameObject.SetActive(false);
            Fondo.SetActive(false);
        }
        else
        {
            IniciarDialogo();
            ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        }

    }

    public void IniciarDialogo()
    {
        Debug.LogError(sistemaDialogos.DialogoActual);
        ChecarDialogo();
        //ApagarBotonesHabilidades();
        if (sistemaDialogos.DialogoActual<= 1)
        {
            Dialogo.SetActive(true);
            Fondo.SetActive(true);
        }
        else if (sistemaDialogos.DialogoActual == 3)
        {
            if (!Peleas.activeSelf)
            {
                sistemaDialogos.DialogoActual = 2;
            }
        }
        else if(sistemaDialogos.DialogoActual == 4)
        {
            Dialogo.SetActive(false);
            ControladorBatalla.IniciarCuentaRegresiva(true);
            gameObject.SetActive(false);
            return;
        }

        // Si el panel está cerrado, pausamos el sistema de diálogo
        if (sistemaDialogos != null && !Dialogo.activeSelf)
            sistemaDialogos.EnPausa = true;
        if (sistemaDialogos.Leyendo)
            sistemaDialogos.SaltarDialogo();
        else
            sistemaDialogos.IniciarDialogoTuto();
    }

    public void ComenzarPelea()
    {
        sistemaDialogos.DialogoActual = 3;
        sistemaDialogos.LineaActual = 0;
        sistemaDialogos.DialogoArecibir = sistemaDialogos.Dialogos[3];
        IniciarDialogo();
    }
    // Update is called once per frame
    void Update()
    {
        ChecarDialogo();
    }
    private void CambiarNPC(int n)
    {
        Mensaje.text= NombreNPC[n];
        Dialogo.GetComponent<Image>().color = ColorDialogo[n];
    }
    //Si se cambia el dilogo se tiene que cambiar el orden si o todo esta funcion
    public void ChecarDialogo()
    {
        if (sistemaDialogos.DialogoActual > 0)
        {
            CambiarNPC(1);

            if (Fondo.activeSelf)
            {
                Fondo.SetActive(false);
            }
        }

        if (sistemaDialogos.DialogoActual == 1 && sistemaDialogos.LineaActual == 2)
        {
            Botones[0].SetActive(true);
            ParpadearBotonHabilidades(Botones[0].GetComponent<Image>());
        }
        else if (sistemaDialogos.DialogoActual == 1 && sistemaDialogos.LineaActual == 3)
        {
            Botones[0].SetActive(false);
            ParpadearBotonHabilidades(Botones[1].GetComponent<Image>());
            ParpadearBotonHabilidades(Botones[2].GetComponent<Image>());
            ParpadearBotonHabilidades(Botones[3].GetComponent<Image>());
        }
        else if (sistemaDialogos.DialogoActual == 1 && sistemaDialogos.LineaActual == 4)
        {
            Botones[0].SetActive(false);
            ParpadearBotonHabilidades(Botones[1].GetComponent<Image>());
            ParpadearBotonHabilidades(Botones[2].GetComponent<Image>());
            DejarBotonHabilidades(Botones[3].GetComponent<Image>());
        }
        else if (sistemaDialogos.DialogoActual == 1 && sistemaDialogos.LineaActual == 5)
        {
            Botones[0].SetActive(false);
            DejarBotonHabilidades(Botones[1].GetComponent<Image>());
            DejarBotonHabilidades(Botones[2].GetComponent<Image>());
            ParpadearBotonHabilidades(Botones[3].GetComponent<Image>());
        }
        else if (sistemaDialogos.DialogoActual == 1 && sistemaDialogos.LineaActual == 6)
        {
            Botones[0].SetActive(false);
            ParpadearBotonHabilidades(Botones[4].GetComponent<Image>());
            DejarBotonHabilidades(Botones[3].GetComponent<Image>());
        }
        else if (sistemaDialogos.DialogoActual == 1 && sistemaDialogos.LineaActual == 7)
        {
            Botones[0].SetActive(false);
            ParpadearBotonHabilidades(Botones[5].GetComponent<Image>());
            DejarBotonHabilidades(Botones[4].GetComponent<Image>());
        }
        else if (sistemaDialogos.DialogoActual == 2 && sistemaDialogos.LineaActual == 0)
        {
            Botones[0].SetActive(false);
            ParpadearBotonAceptar(Botones[6].GetComponent<Image>());
            DejarBotonHabilidades(Botones[5].GetComponent<Image>());
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 0)
        {
            PrenderBoton(BotonesPelea[0]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 1)
        {
            DejarBotonON(BotonesPelea[0]);
            PrenderBoton(BotonesPelea[1]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 2)
        {
            DejarBotonON(BotonesPelea[1]);
            PrenderBoton(BotonesPelea[2]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 3)
        {
            DejarBotonON(BotonesPelea[2]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 4)
        {
            PrenderBoton(BotonesPelea[3]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 5)
        {
            DejarBotonON(BotonesPelea[3]);
            PrenderBoton(BotonesPelea[4]);
            PrenderBoton(BotonesPelea[5]);
            PrenderBoton(BotonesPelea[6]);
            PrenderBoton(BotonesPelea[7]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 6)
        {
            DejarBotonON(BotonesPelea[4]);
            DejarBotonON(BotonesPelea[5]);
            DejarBotonON(BotonesPelea[6]);
            DejarBotonON(BotonesPelea[7]);
            PrenderBoton(BotonesPelea[8]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 7)
        {
            PrenderBoton(BotonesPelea[8]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 8)
        {
            DejarBotonON(BotonesPelea[8]);
            PrenderBoton(BotonesPelea[9]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 9)
        {
            DejarBotonON(BotonesPelea[9]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 10)
        {
            DejarBotonON(BotonesPelea[9]);
            PrenderBoton(BotonesPelea[10]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 11)
        {
            DejarBotonON(BotonesPelea[10]);
            PrenderBoton(BotonesPelea[11]);
        }
        else if (sistemaDialogos.DialogoActual == 3 && sistemaDialogos.LineaActual == 12)
        {
            DejarBotonON(BotonesPelea[11]);
        }
    }

    public void ApagarBotonesHabilidades()
    {
        if (!ColorNormal) return;
        
        for (int i=0; i<Botones.Length; i++)
        {
            float t = (Mathf.Sin(Time.time * velocidad) + 1f) / 2f;
            Botones[0].GetComponent<Image>().color = colorA;
        }
        Botones[0].SetActive(false);
        ColorNormal = true;
    }
    public void ParpadearBotonHabilidades(Image BotonImage)
    {
        float t = (Mathf.Sin(Time.time * velocidad) + 1f) / 2f;
        BotonImage.color = Color.Lerp(colorA, colorB, t);
        ColorNormal = false;
    }

    public void DejarBotonHabilidades(Image BotonImage)
    {
        BotonImage.color = colorA;
    }
    public void ParpadearBotonAceptar(Image BotonImage)
    {
        float t = (Mathf.Sin(Time.time * velocidad) + 1f) / 2f;
        BotonImage.color = Color.Lerp(ColorAceptarNormal, colorB, t);
        ColorNormal = false;
    }

    public void PrenderBoton(GameObject Boton)
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            Boton.SetActive(!Boton.activeSelf); // alterna
            timer = 0f;
        }
    }
    public void DejarBotonON(GameObject Boton)
    {
        if (!Boton.activeSelf)
        {
            Boton.SetActive(true);
        }
    }
    public void DejarBotonOFF(GameObject Boton)
    {
        if (Boton.activeSelf)
        {
            Boton.SetActive(false);
        }
    }
}
