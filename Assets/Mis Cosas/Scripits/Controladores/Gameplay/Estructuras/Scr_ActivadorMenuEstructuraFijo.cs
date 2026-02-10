using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_ActivadorMenuEstructuraFijo : MonoBehaviour
{
    bool EstaEnRango;
    [SerializeField] float Duracion;
    [SerializeField] Sprite Icono;
    [SerializeField] Sprite IconoTecla;
    [SerializeField] string Letra;
    [SerializeField] Sprite Tecla;
    [SerializeField] GameObject BotonCerrar;
    [SerializeField] Color[] ColorBotones;
    [SerializeField] GameObject CanvasMenu;


    Transform Gata;
    float Tiempo = 0;
    public bool EstaDentro = false;
    GameObject Camara360;
    GameObject Canvas;
    ChecarInput Checar_input;
    PlayerInput playerInput;
    private InputAction Interactuar;
    private InputAction Cerrar;
    InputIconProvider IconProvider;
    private Sprite iconoActualInteractuar = null;
    private string textoActualInteractuar = "";
    void Start()
    {
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        Camara360 = GameObject.Find("Camara 360");
        Canvas = GameObject.Find("Canvas");
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        Checar_input = GameObject.Find("Singleton").GetComponent<ChecarInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Interactuar = playerInput.actions["Interactuar"];
        Cerrar = playerInput.actions["Cerrar"];
    }

    void Update()
    {
        if (EstaEnRango)
        {
            IconProvider.ActualizarIconoUI(Interactuar, Gata.GetChild(3).GetChild(0), ref iconoActualInteractuar, ref textoActualInteractuar, true);
        }
        if (Interactuar.WasPressedThisFrame() && EstaEnRango && !EstaDentro)
        {
            if (CanvasMenu != null)
            {
                CanvasMenu.SetActive(true);
            }
            EstaDentro = true;
            Camara360.SetActive(false);
            Gata.GetChild(2).gameObject.SetActive(false);
            Gata.GetChild(3).gameObject.SetActive(false);
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
            Gata.GetComponent<Scr_GiroGata>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>(), -200, 1);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(1).GetComponent<RectTransform>(), 230, 1);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(2).GetComponent<RectTransform>(), -810, 1);
            Checar_input.CammbiarAction_UI();
        }
        else
        {
            if ((Interactuar.WasPressedThisFrame() || Cerrar.WasPressedThisFrame()) && EstaEnRango && EstaDentro)
            {
                if (CanvasMenu != null)
                {
                    CanvasMenu.SetActive(false);
                }
                CerrarTablero();
            }

        }

        if (EstaDentro && Tiempo < Duracion)
        {
            Gata.GetChild(1).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Tiempo += Time.deltaTime;
            foreach (Material Mat in Gata.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials)
            {
                CambiarMaterial(Mat);
            }
        }

        if (!EstaDentro)
        {
            if (Tiempo > 0)
            {
                Tiempo -= Time.deltaTime;
                foreach (Material Mat in Gata.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials)
                {
                    CambiarMaterial(Mat);
                }
                Gata.GetChild(1).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = true;
            Gata.GetChild(3).gameObject.SetActive(true);

            Gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = Icono;

            IconProvider.ActualizarIconoUI(Interactuar, Gata.GetChild(3).GetChild(0), ref iconoActualInteractuar, ref textoActualInteractuar, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = false;
            Gata.GetChild(3).gameObject.SetActive(false);
            iconoActualInteractuar = null;
            textoActualInteractuar = "";
        }
    }

    void CambiarMaterial(Material Mat)
    {
        Mat.SetFloat("_Alpha", Mathf.Lerp(1, 0, Tiempo / Duracion));
        if (Mat.GetFloat("_Alpha") == 0)
        {
            GameObject.Find("Gata").transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        else
        {
            GameObject.Find("Gata").transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = true;
        }

    }

    public void CerrarTablero()
    {
        Checar_input.CammbiarAction_Player();
        EstaDentro = false;
        Camara360.SetActive(true);
        Gata.GetChild(2).gameObject.SetActive(true);
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        Gata.GetComponent<Scr_GiroGata>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(false);
        BotonCerrar.GetComponent<Image>().color = ColorBotones[0];
        if (BotonCerrar.transform.childCount>1)
        {
            BotonCerrar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = ColorBotones[1];
        }
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>(), 0, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(1).GetComponent<RectTransform>(), 0, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(2).GetComponent<RectTransform>(),-610, 1);
    }
}
