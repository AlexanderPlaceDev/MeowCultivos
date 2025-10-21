using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PrimeTween;
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

    [Header("Titulo")]
    [SerializeField] GameObject Titulo;
    [SerializeField] float TiempoRebote;
    [SerializeField] bool UsaFormaEspecifica;
    [SerializeField] Ease Forma;
    [HideInInspector] public bool EstaEnOpciones = false;
    Tween tween;

    [Header("Skybox")]
    [SerializeField] float velocidadRotacion = 1.0f;

    [Header("Shake")]
    [SerializeField] float FuerzaShake;

    [Header("Version")]
    [SerializeField] TextMeshProUGUI Version;

    float rotacionActual;
    float TiempoShake = 0;
    int r;


    void Start()
    {

        if (!UsaFormaEspecifica)
        {
            RandomEase();
        }
        r = Random.Range(2, 10);
        rotacionActual = RenderSettings.skybox.GetFloat("_Rotation");
        Version.text = "Ver " + Application.version;


        //InicializarOpciones
        if (PlayerPrefs.HasKey("Volumen"))
        {
            SliderVolumen_General.value = PlayerPrefs.GetInt("Volumen", 100);
        }
        if (PlayerPrefs.HasKey("Volumen_Musica"))
        {
            SliderVolumen_Musica.value = PlayerPrefs.GetInt("Volumen_Musica", 50);
        }
        if (PlayerPrefs.HasKey("Volumen_Ambiente"))
        {
            SliderVolumen_Ambiental.value = PlayerPrefs.GetInt("Volumen_Ambiente", 20);
        }
        if (PlayerPrefs.HasKey("Volumen_Combate"))
        {
            SliderVolumen_Combate.value = PlayerPrefs.GetInt("Volumen_Combate", 50);
        }
        if (PlayerPrefs.HasKey("Brillo"))
        {
            SliderBrillo.value = PlayerPrefs.GetInt("Brillo", 50);
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

            TextoVolumen_General.text = "Volumen General " + (int)SliderVolumen_General.value + " %";
            TextoVolumen_Musica.text = "Musica "+(int)SliderVolumen_Musica.value + " %";
            TextoVolumen_Ambiental.text = "Ambiental " + (int)SliderVolumen_Ambiental.value + " %";
            TextoVolumen_Combate.text = "Combate" + (int)SliderVolumen_Combate.value + " %";
            TextoBrillo.text =(int)SliderBrillo.value + " %";
        }
    }

    public void GuardarOpciones()
    {
        PlayerPrefs.SetInt("Volumen", (int)SliderVolumen_General.value);
        PlayerPrefs.SetInt("Volumen_Musica", (int)SliderVolumen_Musica.value);
        PlayerPrefs.SetInt("Volumen_Ambiente", (int)SliderVolumen_Ambiental.value);
        PlayerPrefs.SetInt("Volumen_Combate", (int)SliderVolumen_Combate.value);
        PlayerPrefs.SetInt("Brillo", (int)SliderBrillo.value);
        Panel.SetActive(false);
    }
    public void ReiniciarOpciones()
    {
        SliderVolumen_Musica.value = 50;
        SliderBrillo.value = 50;
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
    }


    public void aparecerBrillo()
    {
        Panel_Opciones.SetActive(false);
        Panel_Volumenes.SetActive(false);
        Panel_Brillo.SetActive(true);
    }

    public void aparecerOpciones()
    {
        Panel_Opciones.SetActive(true);
        Panel_Volumenes.SetActive(false);
        Panel_Brillo.SetActive(false);
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



}
