using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_ActivadorMenuEstructuraCircular : MonoBehaviour
{
    [SerializeField] Sprite Icono;
    [SerializeField] Sprite IconoTecla;
    [SerializeField] string Tecla;
    [SerializeField] float Distancia;
    [SerializeField] float DuracionMaterial;
    [SerializeField] float DuracionCamara;
    public bool EstaDentro = false;

    Transform Gata;
    GameObject Camara360;
    GameObject Canvas;
    bool EstaLejos;
    float TiempoMaterial = 0;
    float TiempoCamara = 0;
    GameObject ControladorMenu;

    InputIconProvider IconProvider;
    PlayerInput playerInput;
    private InputAction Interactuar;
    private Sprite iconoActualInteractuar = null;
    private string textoActualInteractuar = "";
    void Awake()
    {
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        ControladorMenu = Gata.GetChild(6).gameObject;
        Camara360 = GameObject.Find("Camara 360");
        Canvas = GameObject.Find("Canvas");
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();

        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Interactuar = playerInput.actions["Interactuar"];
    }

    void Update()
    {
        VerificarDistancia();
        CambiarCamaras();
    }


    private void VerificarDistancia()
    {
        if (Vector3.Distance(Gata.position, transform.position) < Distancia && !EstaDentro)
        {
            EstaLejos = false;
            ActualizarIconoUI(Interactuar, Gata.GetChild(3).GetChild(0), ref iconoActualInteractuar, ref textoActualInteractuar);
        }
        if (Vector3.Distance(Gata.position, transform.position) > Distancia && !EstaLejos)
        {
            Gata.GetChild(3).gameObject.SetActive(false);
            EstaLejos = true;
        }
    }
    void ActualizarIconoUI(InputAction action, Transform uiTransform, ref Sprite iconoActual, ref string textoActual)
    {
        Gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = Icono;
        Gata.GetChild(3).gameObject.SetActive(true);


        Gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
        Gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);
        if (IconProvider.UsandoGamepad())
        {
            Sprite nuevoIcono = IconProvider.GetIcon(action);
            if (iconoActual != nuevoIcono)
            {
                uiTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                uiTransform.GetComponent<Image>().sprite = nuevoIcono;
                uiTransform.transform.localScale = new Vector3(1, 1, 1);
                iconoActual = nuevoIcono;
                textoActual = "";
            }
        }
        else
        {
            string tecla = IconProvider.GetKeyText(action);
            if (textoActual != tecla)
            {
                uiTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tecla;
                uiTransform.GetComponent<Image>().sprite = IconoTecla;
                uiTransform.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                textoActual = tecla;
                iconoActual = IconoTecla;
            }
        }
    }
    private void CambiarCamaras()
    {
        if (EstaDentro)
        {
            if (TiempoMaterial < DuracionMaterial)
            {
                Gata.GetChild(1).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                TiempoMaterial += Time.deltaTime;
                foreach (Material Mat in Gata.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials)
                {
                    CambiarMaterial(Mat);
                }
            }

            if (TiempoCamara < DuracionCamara)
            {
                TiempoCamara += Time.deltaTime;
            }
            else
            {
                transform.GetChild(2).gameObject.SetActive(true);
            }
        }

        if (!EstaDentro)
        {
            if (TiempoMaterial > 0)
            {
                TiempoMaterial -= Time.deltaTime;
                foreach (Material Mat in Gata.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials)
                {
                    CambiarMaterial(Mat);
                }
                Gata.GetChild(1).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }

            if (TiempoCamara > 0)
            {
                TiempoCamara -= Time.deltaTime;
                transform.GetChild(2).gameObject.SetActive(false);
            }
        }


        if (!EstaLejos && Interactuar.IsPressed())
        {

            if (Camara360 == null)
            {
                Camara360 = GameObject.Find("Camara 360");
            }

            EstaDentro = true;
            Camara360.SetActive(false);
            Gata.GetChild(3).gameObject.SetActive(false);// Desactiva Iconos de accion
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
            Gata.GetComponent<Scr_GiroGata>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);//Activa la camara hoguera
            transform.GetChild(1).gameObject.SetActive(true);//Activa la camara hoguera
            ControladorMenu.GetComponent<Scr_ControladorMenuGameplay>().enabled = false;// Desactiva logica reloj
            Canvas.transform.GetChild(2).gameObject.SetActive(false);//Desactiva UI reloj
            /*
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>(), -250, 1);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(1).GetComponent<RectTransform>(), 250, 1);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(2).GetComponent<RectTransform>(), -850, 1);*/
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && EstaDentro)
            {
                Salir();
            }
        }
    }

    void CambiarMaterial(Material Mat)
    {
        Mat.SetFloat("_Alpha", Mathf.Lerp(1, 0, TiempoMaterial / DuracionMaterial));

        if (Mat.GetFloat("_Alpha") == 0)
        {
            Gata.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        else
        {
            Gata.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = true;
        }


    }

    public void Salir()
    {
        EstaDentro = false;
        Camara360.SetActive(true);
        Gata.GetChild(3).gameObject.SetActive(true);
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        Gata.GetComponent<Scr_GiroGata>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        ControladorMenu.GetComponent<Scr_ControladorMenuGameplay>().enabled = true; //Activa logica del reloj
        Canvas.transform.GetChild(2).gameObject.SetActive(true);// Activa UI Reloj
        /*
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>(), 30, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(1).GetComponent<RectTransform>(), 0, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(2).GetComponent<RectTransform>(), -610, 1);*/
    }
}
