using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_ControladorMenu : MonoBehaviour
{
    [Header("Nubes")]
    [SerializeField] GameObject[] Nubes;
    float Tiempo = 0;

    [Header("Creditos")]
    [SerializeField] GameObject ObjCreditos;
    public bool CreditosActivados = false;

    [Header("Opciones")]
    [SerializeField]
    public GameObject Panel;

    public GameObject Panel_Opciones;
    public GameObject Panel_Volumenes;

    public GameObject Panel_Brillo;
    public GameObject Panel_Sencibilidad;
    public GameObject Panel_Sensibilidad_joystick;
    public GameObject Panel_Sensibilidad_Mouse;
    private int Op =0;
    [SerializeField] TextMeshProUGUI TextoVolumen_General;
    [SerializeField] Slider SliderVolumen_General;
    [SerializeField] TextMeshProUGUI TextoVolumen_Musica;
    [SerializeField] Slider SliderVolumen_Musica;
    [SerializeField] TextMeshProUGUI TextoVolumen_Ambiental;
    [SerializeField] Slider SliderVolumen_Ambiental;
    [SerializeField] TextMeshProUGUI TextoVolumen_Combate;
    [SerializeField] Slider SliderVolumen_Combate;
    [SerializeField] TextMeshProUGUI TextoBrillo;
    [SerializeField] Slider SliderBrillo;
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
    [Header("Nueva Partida")]
    [SerializeField] GameObject opcioensNuevaPartida;
    [Header("Titulo")]
    [SerializeField] GameObject Titulo;
    [SerializeField] float TiempoRebote;
    [SerializeField] bool UsaFormaEspecifica;
    [SerializeField] Ease Forma;
    [HideInInspector] public bool EstaEnOpciones = false;
    Tween tween;
    [SerializeField] GameObject NuevaPartida;

    [Header("Skybox")]
    [SerializeField] float velocidadRotacion = 1.0f;

    [Header("Shake")]
    [SerializeField] float FuerzaShake;

    [Header("Version")]
    [SerializeField] TextMeshProUGUI Version;

    float rotacionActual;
    float TiempoShake = 0;
    int r;

    InputIconProvider inputIconProvider;
    void Start()
    {
        inputIconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        if (!UsaFormaEspecifica)
        {
            RandomEase();
        }
        r = Random.Range(2, 10);
        rotacionActual = RenderSettings.skybox.GetFloat("_Rotation");
        Version.text = "Ver " + Application.version;


        if (PlayerPrefs.GetString("Partida", "SI") == "SI")
        {
            NuevaPartida.SetActive(true);
        }
        else
        {
            NuevaPartida.SetActive(false);
        }
    }

    void Update()
    {
        ActualizarOpciones();
        SpawnNubes();

        if (!tween.IsAlive && Titulo != null && !EstaEnOpciones)
        {
            if (UsaFormaEspecifica)
            {
                tween = Tween.LocalScale(Titulo.transform, endValue: new Vector3(2, 2, 2), duration: TiempoRebote, Forma, cycles: 2, CycleMode.Yoyo);
            }
            else
            {
                tween = Tween.LocalScale(Titulo.transform, endValue: new Vector3(2, 2, 2), duration: TiempoRebote, Forma, cycles: 2, CycleMode.Yoyo);
            }
        }

        if (!EstaEnOpciones)
        {
            //Skybox
            rotacionActual += velocidadRotacion * Time.deltaTime;
            RenderSettings.skybox.SetFloat("_Rotation", rotacionActual);

            //Shake
            if (TiempoShake < r)
            {
                TiempoShake += Time.deltaTime;
            }
            else
            {
                Tween.ShakeCamera(Camera.main, strengthFactor: FuerzaShake);
                TiempoShake = 0;
                r = Random.Range(2, 10);
            }


        }
    }

    public IEnumerator Creditos()
    {

        CreditosActivados = true;
        ObjCreditos.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(20);
        CreditosActivados = false;
        ObjCreditos.GetComponent<Animator>().enabled = false;
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
        else if(Op==1)
        {
            PlayerPrefs.SetInt("Brillo", (int)SliderBrillo.value);
        }
        else if (Op == 2)
        {
            if (Panel_Sensibilidad_Mouse.activeSelf)
            {
                PlayerPrefs.SetInt("Sensibilidad_Mouse", (int)SliderSencibilidad_Mouse.value);
                PlayerPrefs.SetInt("Sensibilidad_MouseH", (int)SliderSencibilidad_MouseH.value);
                PlayerPrefs.SetInt("Sensibilidad_MouseV", (int)SliderSencibilidad_MouseV.value);
            }
            else
            {
                PlayerPrefs.SetInt("Sensibilidad_joystick", (int)SliderSencibilidad_joystick.value);
                PlayerPrefs.SetInt("Sensibilidad_joystickH", (int)SliderSencibilidad_joystickH.value);
                PlayerPrefs.SetInt("Sensibilidad_joystickV", (int)SliderSencibilidad_joystickV.value);
            }
        }
        PlayerPrefs.Save();
        Panel.SetActive(false);
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
    private void SpawnNubes()
    {
        if (Tiempo <= 0)
        {
            Tiempo = Random.Range(2, 15);
            GameObject Nube = Instantiate(Nubes[Random.Range(0, 4)], new Vector3(40, Random.Range(-10, 10), 0), Quaternion.identity, null);
            float Escala = Random.Range(0.5f, 1.5f);
            Nube.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(-2, -1);
            Nube.transform.localScale = new Vector3(Escala, Escala, 1);
            Nube.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-3, -7), 0, 0);
        }
        else
        {
            Tiempo -= Time.deltaTime;
        }
    }

    public void aparecerVolumenes()
    {
        Panel_Opciones.SetActive(false);
        Panel_Volumenes.SetActive(true);
        Panel_Brillo.SetActive(false);
        Panel_Sencibilidad.SetActive(false);
        Op = 0;
        SliderVolumen_General.value = PlayerPrefs.GetInt("Volumen", 100);
        SliderVolumen_Musica.value = PlayerPrefs.GetInt("Volumen_Musica", 50);
        SliderVolumen_Ambiental.value = PlayerPrefs.GetInt("Volumen_Ambiente", 20);
        SliderVolumen_Combate.value = PlayerPrefs.GetInt("Volumen_Combate", 50);
        
    }


    public void aparecerBrillo()
    {
        Panel_Opciones.SetActive(false);
        Panel_Volumenes.SetActive(false);
        Panel_Brillo.SetActive(true);
        Panel_Sencibilidad.SetActive(false);
        Op = 1;
        SliderBrillo.value = PlayerPrefs.GetInt("Brillo", 50);
    }
    public void aparecerSencibilidad()
    {
        Panel_Opciones.SetActive(false);
        Panel_Volumenes.SetActive(false);
        Panel_Brillo.SetActive(false);
        Panel_Sencibilidad.SetActive(true);
        Op = 2;
        Panel_Sensibilidad_joystick.SetActive(false);
        Panel_Sensibilidad_Mouse.SetActive(false);
        detectarobjeto();
    }
    public void aparecerOpciones()
    {
        Panel_Opciones.SetActive(true);
        Panel_Volumenes.SetActive(false);
        Panel_Brillo.SetActive(false);
        Panel_Sencibilidad.SetActive(false);
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
        SliderSencibilidad_joystick.value = PlayerPrefs.GetInt("Sensibilidad_joystick", 30);
        SliderSencibilidad_joystickH.value = PlayerPrefs.GetInt("Sensibilidad_joystickH", 30);
        SliderSencibilidad_joystickV.value = PlayerPrefs.GetInt("Sensibilidad_joystickV", 30);
    }
    public void Aparecer_Mouse()
    {

        Panel_Sensibilidad_joystick.SetActive(false);
        Panel_Sensibilidad_Mouse.SetActive(true);
        SliderSencibilidad_Mouse.value = PlayerPrefs.GetInt("Sensibilidad_Mouse", 30);
        SliderSencibilidad_MouseH.value = PlayerPrefs.GetInt("Sensibilidad_MouseH", 30);
        SliderSencibilidad_MouseV.value = PlayerPrefs.GetInt("Sensibilidad_MouseV", 30);
    }
    void RandomEase()
    {
        int r = Random.Range(0, 33);

        switch (r)
        {
            case 0:
                {
                    Forma = Ease.Default;
                    break;
                }
            case 1:
                {
                    Forma = Ease.InBack;
                    break;
                }
            case 2:
                {
                    Forma = Ease.InBounce;
                    break;
                }
            case 3:
                {
                    Forma = Ease.InCirc;
                    break;
                }
            case 4:
                {
                    Forma = Ease.InCubic;
                    break;
                }
            case 5:
                {
                    Forma = Ease.InElastic;
                    break;
                }
            case 6:
                {
                    Forma = Ease.InExpo;
                    break;
                }
            case 7:
                {
                    Forma = Ease.InOutBack;
                    break;
                }
            case 8:
                {
                    Forma = Ease.InOutBounce;
                    break;
                }
            case 9:
                {
                    Forma = Ease.InOutCirc;
                    break;
                }
            case 10:
                {
                    Forma = Ease.InOutCubic;
                    break;
                }
            case 11:
                {
                    Forma = Ease.InOutElastic;
                    break;
                }
            case 12:
                {
                    Forma = Ease.InOutExpo;
                    break;
                }
            case 13:
                {
                    Forma = Ease.InOutQuad;
                    break;
                }
            case 14:
                {
                    Forma = Ease.InOutQuart;
                    break;
                }
            case 15:
                {
                    Forma = Ease.InOutQuint;
                    break;
                }
            case 16:
                {
                    Forma = Ease.InOutSine;
                    break;
                }
            case 17:
                {
                    Forma = Ease.InQuad;
                    break;
                }
            case 18:
                {
                    Forma = Ease.InQuart;
                    break;
                }
            case 19:
                {
                    Forma = Ease.InQuint;
                    break;
                }
            case 20:
                {
                    Forma = Ease.InSine;
                    break;
                }
            case 21:
                {
                    Forma = Ease.InSine;
                    break;
                }
            case 22:
                {
                    Forma = Ease.Linear;
                    break;
                }
            case 23:
                {
                    Forma = Ease.OutBack;
                    break;
                }
            case 24:
                {
                    Forma = Ease.OutBounce;
                    break;
                }
            case 25:
                {
                    Forma = Ease.OutCirc;
                    break;
                }
            case 26:
                {
                    Forma = Ease.OutCubic;
                    break;
                }
            case 27:
                {
                    Forma = Ease.OutElastic;
                    break;
                }
            case 28:
                {
                    Forma = Ease.OutExpo;
                    break;
                }
            case 29:
                {
                    Forma = Ease.OutQuad;
                    break;
                }
            case 30:
                {
                    Forma = Ease.OutQuart;
                    break;
                }
            case 31:
                {
                    Forma = Ease.OutQuint;
                    break;
                }
            case 32:
                {
                    Forma = Ease.OutSine;
                    break;
                }
        }
    }

    public void MostarNuevaPatida()
    {
        opcioensNuevaPartida.SetActive(true);
    }

    public void EsconderNuevaPatida()
    {
        opcioensNuevaPartida.SetActive(false);
    }
}
