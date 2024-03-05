using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PrimeTween;

public class Scr_ControladorMenu : MonoBehaviour
{
    [SerializeField] bool EsSpawner;

    [Header("Suelos")]
    [SerializeField] GameObject[] Suelos;
    [SerializeField] Vector3 PosicionOrigen;
    [SerializeField] public float Velocidad;
    [SerializeField] bool CambiarMesh;
    GameObject SueloSpawneado;

    [Header("Creditos")]
    [SerializeField] GameObject Letrero;
    [HideInInspector] public bool ActivarCreditos = false;
    int CreditoActual = 0;

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
        if (!EsSpawner)
        {
            if (CambiarMesh)
            {
                int r = Random.Range(0, Suelos.Length);
                GetComponent<MeshFilter>().mesh = Suelos[r].GetComponent<MeshFilter>().sharedMesh;
            }

            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Velocidad, 0, 0);

        }
        else
        {
            if (!UsaFormaEspecifica)
            {
                RandomEase();
            }
            r = Random.Range(2, 10);
            rotacionActual = RenderSettings.skybox.GetFloat("_Rotation");
            Version.text = "Ver " + Application.version;
            SueloSpawneado = Instantiate(Suelos[0], PosicionOrigen, Quaternion.Euler(-90, 180, 0), null);
            SueloSpawneado.GetComponent<Rigidbody>().velocity = new Vector3(Velocidad, 0, 0);
        }
    }

    void Update()
    {
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

        if (EsSpawner)
        {

            //Suelos
            if (SueloSpawneado == null)
            {
                SueloSpawneado = Instantiate(Suelos[Random.Range(0, Suelos.Length)], PosicionOrigen, Quaternion.Euler(-90, 180, 0), null);
                SueloSpawneado.GetComponent<Rigidbody>().velocity = new Vector3(Velocidad, 0, 0);

                //Creditos
                if (ActivarCreditos && !EstaEnOpciones)
                {
                    if (SueloSpawneado.name != "Suelo3(Clone)")
                    {
                        GameObject LetreroSpawneado = Instantiate(Letrero, new Vector3(200, -40, 75), Quaternion.Euler(-90, 0, 0), SueloSpawneado.transform);
                        LetreroSpawneado.transform.localScale = new Vector3(0.5f, 0.5f, 15);
                        switch (CreditoActual)
                        {
                            case 0:
                                {
                                    LetreroSpawneado.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Director\nCarlos Alfredo\nLopez Flores";
                                    CreditoActual++;
                                    break;
                                }
                            case 1:
                                {
                                    LetreroSpawneado.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Jugabilidad\nXavier Emilio\n";
                                    CreditoActual++;
                                    break;
                                }
                            case 2:
                                {
                                    LetreroSpawneado.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Tester\nErick\nPulido Santoyo";
                                    ActivarCreditos = false;
                                    CreditoActual = 0;
                                    break;
                                }
                        }
                    }
                }

            }
            else
            {
                if (SueloSpawneado.transform.position.x <= PosicionOrigen.x - 80)
                {
                    SueloSpawneado = null;
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
            else
            {
                if (GameObject.Find("Autobus").transform.position.x == -93)
                {
                    rotacionActual += (velocidadRotacion / 2) * Time.deltaTime;
                    RenderSettings.skybox.SetFloat("_Rotation", rotacionActual);

                    if (Camera.main.transform.GetChild(1).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("New State") || Camera.main.transform.GetChild(1).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Aparecer"))
                    {
                        foreach (GameObject Suelo in GameObject.FindGameObjectsWithTag("SueloMenu"))
                        {
                            Debug.Log("Entra2");
                            Suelo.GetComponent<Rigidbody>().velocity = new Vector3(Velocidad / 2, 0, 0);
                        }
                    }

                    GameObject.Find("Autobus").transform.GetChild(0).GetComponent<Scr_GirarObjeto>().VelocidadGeneral = -0.5f;
                    GameObject.Find("Autobus").transform.GetChild(1).GetComponent<Scr_GirarObjeto>().VelocidadGeneral = -0.5f;
                    GameObject.Find("Autobus").transform.GetChild(2).GetComponent<Scr_GirarObjeto>().VelocidadGeneral = -0.5f;
                    GameObject.Find("Autobus").transform.GetChild(3).GetComponent<Scr_GirarObjeto>().VelocidadGeneral = -0.5f;
                    if (EstaEnOpciones && GameObject.Find("Canvas2").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("New State"))
                    {
                        Camera.main.transform.GetChild(1).GetComponent<Animator>().Play("Aparecer");
                    }
                }
            }

        }
        else
        {
            if (transform.position.x < -250)
            {
                Destroy(gameObject);
            }
        }

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
