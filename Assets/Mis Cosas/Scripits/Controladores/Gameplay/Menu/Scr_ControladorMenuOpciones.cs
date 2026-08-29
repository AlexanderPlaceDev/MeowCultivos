using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Scr_ControladorMenuOpciones : MonoBehaviour
{
    [Header("Opciones")]
    [SerializeField]
    public GameObject Panel;

    public GameObject Panel_Opciones;
    public GameObject Panel_Volumenes;

    public GameObject Panel_Brillo;
    public GameObject Panel_SencibilidadModos;
    public GameObject Panel_Sencibilidad;
    public GameObject Panel_Sensibilidad_joystick;
    public GameObject Panel_Sensibilidad_Mouse;

    public GameObject Guardar;
    public GameObject Reiniciar;
    [Header("Volumen")]
    private int Op = 0;
    [SerializeField] TextMeshProUGUI TextoBrillo;
    [SerializeField] Slider SliderBrillo;
    [SerializeField] TextMeshProUGUI TextoVolumen_General;
    [SerializeField] Slider SliderVolumen_General;
    [SerializeField] TextMeshProUGUI TextoVolumen_Musica;
    [SerializeField] Slider SliderVolumen_Musica;
    [SerializeField] TextMeshProUGUI TextoVolumen_Ambiental;
    [SerializeField] Slider SliderVolumen_Ambiental;
    [SerializeField] TextMeshProUGUI TextoVolumen_Combate;
    [SerializeField] Slider SliderVolumen_Combate;
    [Header("Sensibilidad")]
    private int SesnsMod=0;
    public GameObject Rotacion;
    public GameObject Panel_Sensibilidad_Camara;
    public GameObject[] Opciones_sensibilidad_mouse;
    public GameObject[] Opciones_sensibilidad_joystick;
    [SerializeField] TextMeshProUGUI TextoSencibilidad_Mouse;
    [SerializeField] Slider SliderSencibilidad_Mouse;
    [SerializeField] TextMeshProUGUI TextoSencibilidad_MouseH;
    [SerializeField] Slider SliderSencibilidad_MouseH;
    [SerializeField] TextMeshProUGUI TextoSencibilidad_MouseV;
    [SerializeField] Slider SliderSencibilidad_MouseV;
    [SerializeField] TextMeshProUGUI TextoSencibilidad_joystick;
    [SerializeField] Slider SliderSencibilidad_joystick;
    [SerializeField] TextMeshProUGUI TextoSencibilidad_joystickH;
    [SerializeField] Slider SliderSencibilidad_joystickH;
    [SerializeField] TextMeshProUGUI TextoSencibilidad_joystickV;
    [SerializeField] Slider SliderSencibilidad_joystickV;
    [SerializeField] TextMeshProUGUI TextoSencibilidad_Camara;
    [SerializeField] Slider SliderSencibilidad_Camara;


    InputIconProvider inputIconProvider;
    void Start()
    {
        inputIconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        aparecerOpciones();
    }

    void Update()
    {
        ActualizarOpciones();
    }

    void ActualizarOpciones()
    {
        if (Panel.activeSelf)
        {
            TextoVolumen_General.text = (int)SliderVolumen_General.value + " %";
            TextoVolumen_Musica.text = (int)SliderVolumen_Musica.value + " %";
            TextoVolumen_Ambiental.text = (int)SliderVolumen_Ambiental.value + " %";
            TextoVolumen_Combate.text = (int)SliderVolumen_Combate.value + " %";
            TextoBrillo.text = (int)SliderBrillo.value + " %";
            TextoSencibilidad_Mouse.text = (int)SliderSencibilidad_Mouse.value + "%";
            TextoSencibilidad_MouseH.text = (int)SliderSencibilidad_MouseH.value + "%";
            TextoSencibilidad_MouseV.text = (int)SliderSencibilidad_MouseV.value + "%";
            TextoSencibilidad_joystick.text = (int)SliderSencibilidad_joystick.value + "%";
            TextoSencibilidad_joystickH.text = (int)SliderSencibilidad_joystickH.value + "%";
            TextoSencibilidad_joystickV.text = (int)SliderSencibilidad_joystickV.value + "%";
            TextoSencibilidad_Camara.text = (int)SliderSencibilidad_Camara.value + "%";
        }
    }

    public void GuardarOpciones()
    {
        if (Op == 0)
        {
            PlayerPrefs.SetInt("Volumen", (int)SliderVolumen_General.value);
            PlayerPrefs.SetInt("Volumen_Musica", (int)SliderVolumen_Musica.value);
            PlayerPrefs.SetInt("Volumen_Ambiente", (int)SliderVolumen_Ambiental.value);
            PlayerPrefs.SetInt("Volumen_Combate", (int)SliderVolumen_Combate.value);

            foreach (Scr_AsignacionDeVolumen audio in FindObjectsOfType<Scr_AsignacionDeVolumen>())
            {
                audio.AsignarVolumen();
            }
        }
        else if (Op == 1)
        {
            PlayerPrefs.SetInt("Brillo", (int)SliderBrillo.value);
        }
        else if (Op == 2)
        {
            if (Panel_Sensibilidad_Mouse.activeSelf)
            {
                guardarMouse();
            }
            else if (Panel_Sensibilidad_joystick.activeSelf)
            {
                guardarjoystick();
            }
            else if (Panel_Sensibilidad_Camara.activeSelf)
            {
                PlayerPrefs.SetInt("Velocidad_de_camara", (int)SliderSencibilidad_Camara.value);
            }
        }
        PlayerPrefs.Save();
        Panel.SetActive(false);
    }
    private void guardarMouse()
    {
        if (SesnsMod == 0)
        {
            PlayerPrefs.SetInt("Sensibilidad_MouseT", (int)SliderSencibilidad_Mouse.value);
        }
        else
        {

            PlayerPrefs.SetInt("Sensibilidad_MouseP", (int)SliderSencibilidad_Mouse.value);
            PlayerPrefs.SetInt("Sensibilidad_MouseHP", (int)SliderSencibilidad_MouseH.value);
            PlayerPrefs.SetInt("Sensibilidad_MouseVP", (int)SliderSencibilidad_MouseV.value);
        }
    }
    private void guardarjoystick()
    {
        if (SesnsMod == 0)
        {
            PlayerPrefs.SetInt("Sensibilidad_joystickT", (int)SliderSencibilidad_joystick.value);
        }
        else
        {
            PlayerPrefs.SetInt("Sensibilidad_joystickP", (int)SliderSencibilidad_joystick.value);
            PlayerPrefs.SetInt("Sensibilidad_joystickHP", (int)SliderSencibilidad_joystickH.value);
            PlayerPrefs.SetInt("Sensibilidad_joystickVP", (int)SliderSencibilidad_joystickV.value);
        }
    }
    public void ReiniciarOpciones()
    {
        if (Op == 0)
        {
            SliderVolumen_General.value = 100;
            SliderVolumen_Musica.value = 50;
            SliderVolumen_Ambiental.value = 20;
            SliderVolumen_Combate.value = 50;
        }
        else if (Op == 1)
        {
            SliderBrillo.value = 50;
        }
        else if (Op == 2)
        {
            SliderSencibilidad_Mouse.value = 30;
            SliderSencibilidad_MouseH.value = 30;
            SliderSencibilidad_MouseV.value = 30;
            SliderSencibilidad_joystick.value = 30;
            SliderSencibilidad_joystickH.value = 30;
            SliderSencibilidad_joystickV.value = 30;
        }
    }
    public void Aparecer_Opciones()
    {
        Guardar.SetActive(true);
        Reiniciar.SetActive(true);
    }
    public void aparecerVolumenes()
    {
        Panel_Opciones.SetActive(false);
        Panel_Volumenes.SetActive(true);
        Panel_Brillo.SetActive(false);
        Panel_Sencibilidad.SetActive(false);
        Panel_SencibilidadModos.SetActive(false);
        Op = 0;
        SliderVolumen_General.value = PlayerPrefs.GetInt("Volumen", 100);
        SliderVolumen_Musica.value = PlayerPrefs.GetInt("Volumen_Musica", 50);
        SliderVolumen_Ambiental.value = PlayerPrefs.GetInt("Volumen_Ambiente", 20);
        SliderVolumen_Combate.value = PlayerPrefs.GetInt("Volumen_Combate", 50);
        Aparecer_Opciones();
    }


    public void aparecerBrillo()
    {
        Panel_Opciones.SetActive(false);
        Panel_Volumenes.SetActive(false);
        Panel_Brillo.SetActive(true);
        Panel_Sencibilidad.SetActive(false);
        Panel_SencibilidadModos.SetActive(false);
        Op = 1;
        SliderBrillo.value = PlayerPrefs.GetInt("Brillo", 50);
        Aparecer_Opciones();
    }
    public void aparecerSencibilidad()
    {
        Panel_Opciones.SetActive(false);
        Panel_Volumenes.SetActive(false);
        Panel_Brillo.SetActive(false);
        Panel_Sencibilidad.SetActive(false);
        Panel_SencibilidadModos.SetActive(true);
        Panel_Sensibilidad_Camara.SetActive(false);
        Op = 2;
        Panel_Sensibilidad_joystick.SetActive(false);
        Panel_Sensibilidad_Mouse.SetActive(false);
    }

    public void aparecerSensmods(int I)
    {
        SesnsMod = I;
        Panel_Sencibilidad.SetActive(true);
        Panel_SencibilidadModos.SetActive(false);
        detectarobjeto();
        Aparecer_Opciones();
        if (SesnsMod == 0)
        {
            Rotacion.SetActive(true);
        }
        else
        {
            Rotacion.SetActive(false);
        }
    }
    public void aparecerOpciones()
    {
        Panel_Opciones.SetActive(true);
        Panel_Volumenes.SetActive(false);
        Panel_Brillo.SetActive(false);
        Panel_Sencibilidad.SetActive(false);
        Panel_SencibilidadModos.SetActive(false);
        Guardar.SetActive(false);
        Reiniciar.SetActive(false);
        Op = -1;
    }

    private void detectarobjeto()
    {
        if (inputIconProvider.UsandoGamepad())
        {
            Aparecer_joystick();
        }
        else
        {
            Aparecer_Mouse();
        }
    }

    public void Aparecer_joystick()
    {
        Panel_Sensibilidad_joystick.SetActive(true);
        Panel_Sensibilidad_Mouse.SetActive(false);
        Panel_Sensibilidad_Camara.SetActive(false);
        cargarjoystick();
        /*
        SliderSencibilidad_joystick.value = PlayerPrefs.GetInt("Sensibilidad_joystick", 30);
        SliderSencibilidad_joystickH.value = PlayerPrefs.GetInt("Sensibilidad_joystickH", 30);
        SliderSencibilidad_joystickV.value = PlayerPrefs.GetInt("Sensibilidad_joystickV", 30);
        */
    }
    public void Aparecer_Mouse()
    {

        Panel_Sensibilidad_joystick.SetActive(false);
        Panel_Sensibilidad_Mouse.SetActive(true);
        Panel_Sensibilidad_Camara.SetActive(false);
        cargararMouse();
        /*
        SliderSencibilidad_Mouse.value = PlayerPrefs.GetInt("Sensibilidad_Mouse", 30);
        SliderSencibilidad_MouseH.value = PlayerPrefs.GetInt("Sensibilidad_MouseH", 30);
        SliderSencibilidad_MouseV.value = PlayerPrefs.GetInt("Sensibilidad_MouseV", 30);
        */
    }
    public void aparecerCamara()
    {
        Panel_Sensibilidad_joystick.SetActive(false);
        Panel_Sensibilidad_Mouse.SetActive(false);
        Panel_Sensibilidad_Camara.SetActive(true);

        SliderSencibilidad_Camara.value = PlayerPrefs.GetInt("Velocidad_de_camara", 50);
    }
    private void cargararMouse()
    {
        if (SesnsMod == 0)
        {
            SliderSencibilidad_Mouse.value = PlayerPrefs.GetInt("Sensibilidad_MouseT", 30);
            Opciones_sensibilidad_mouse[0].SetActive(true);
            Opciones_sensibilidad_mouse[1].SetActive(false);
            Opciones_sensibilidad_mouse[2].SetActive(false);
        }
        else
        {
            SliderSencibilidad_Mouse.value = PlayerPrefs.GetInt("Sensibilidad_MouseP", 30);
            SliderSencibilidad_MouseH.value = PlayerPrefs.GetInt("Sensibilidad_MouseHP", 30);
            SliderSencibilidad_MouseV.value = PlayerPrefs.GetInt("Sensibilidad_MouseVP", 30);
            Opciones_sensibilidad_mouse[0].SetActive(true);
            Opciones_sensibilidad_mouse[1].SetActive(true);
            Opciones_sensibilidad_mouse[2].SetActive(true);
        }
    }
    private void cargarjoystick()
    {
        if (SesnsMod == 0)
        {
            SliderSencibilidad_joystick.value = PlayerPrefs.GetInt("Sensibilidad_joystickT", 30);
            Opciones_sensibilidad_joystick[0].SetActive(true);
            Opciones_sensibilidad_joystick[1].SetActive(false);
            Opciones_sensibilidad_joystick[2].SetActive(false);
        }
        else
        {
            SliderSencibilidad_joystick.value = PlayerPrefs.GetInt("Sensibilidad_joystickP", 30);
            SliderSencibilidad_joystickH.value = PlayerPrefs.GetInt("Sensibilidad_joystickHP", 30);
            SliderSencibilidad_joystickV.value = PlayerPrefs.GetInt("Sensibilidad_joystickVP", 30);
            Opciones_sensibilidad_joystick[0].SetActive(true);
            Opciones_sensibilidad_joystick[1].SetActive(true);
            Opciones_sensibilidad_joystick[2].SetActive(true);
        }
    }
}
