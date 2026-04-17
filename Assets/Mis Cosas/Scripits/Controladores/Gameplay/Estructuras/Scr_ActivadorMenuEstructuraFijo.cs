using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_ActivadorMenuEstructuraFijo : MonoBehaviour
{
    public bool EstaEnRango;
    [SerializeField] float Duracion;
    [SerializeField] Sprite Icono;
    [SerializeField] Sprite IconoTecla;
    [SerializeField] private Sprite icono2;
    [SerializeField] string Letra;
    [SerializeField] Sprite Tecla;
    [SerializeField] GameObject BotonCerrar;
    [SerializeField] Color[] ColorBotones;
    [SerializeField] GameObject CanvasMenu;

    [SerializeField] bool TieneBatalla;

    Transform Gata;
    float Tiempo = 0;
    public bool EstaDentro = false;
    GameObject Camara360;
    GameObject Canvas;
    ChecarInput Checar_input;
    PlayerInput playerInput;
    private InputAction Interactuar;
    private InputAction Cerrar;
    private InputAction Recolectar;
    InputIconProvider IconProvider;
    Scr_ControladorMisiones Mis;
    // Variables por botón para evitar parpadeo
    private Sprite iconoActualRecolectar = null;
    private string textoActualRecolectar = "";
    private Sprite iconoActualInteractuar = null;
    private string textoActualInteractuar = "";

    private Scr_CambiadorBatalla batalla;
    Scr_ControladorSembradioUI SembradioUI;

    void Awake()
    {
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        Camara360 = GameObject.Find("Camara 360");
        Canvas = GameObject.Find("Canvas");
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        Checar_input = GameObject.Find("Singleton").GetComponent<ChecarInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Interactuar = playerInput.actions["Interactuar"];
        Cerrar = playerInput.actions["Cerrar"];
        Recolectar = playerInput.actions["Recolectar"];

        Mis = GameObject.Find("ControladorMisiones").GetComponent<Scr_ControladorMisiones>();
        if (TieneBatalla)
        {
            batalla = GetComponent<Scr_CambiadorBatalla>();
            SembradioUI = GetComponent<Scr_ControladorSembradioUI>();
        }
    }

    void Update()
    {
        if (EstaEnRango)
        {
            IconProvider.ActualizarIconoUI(Interactuar, Gata.GetChild(3).GetChild(0), ref iconoActualInteractuar, ref textoActualInteractuar, true);
            if (TieneBatalla && Mis.HayMisionDefensa() && SembradioUI.SemillaPlantada != null && TieneBatalla)
            {
                IconProvider.ActualizarIconoUI(Recolectar, Gata.GetChild(3).GetChild(2), ref iconoActualRecolectar, ref textoActualRecolectar, true);
            }
        }
        if (Interactuar.WasPressedThisFrame() && EstaEnRango && !EstaDentro)
        {
            if (CanvasMenu != null)
            {
                CanvasMenu.SetActive(true);
            }
            EstaDentro = true;
            if (Camara360 == null)
            {
                Camara360 = GameObject.Find("Camara 360");
            }
            Camara360.SetActive(false);
            Gata.GetChild(2).gameObject.SetActive(false);
            Gata.GetChild(3).gameObject.SetActive(false);
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
            Gata.GetComponent<Scr_GiroGata>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>(), -200, 1);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(1).GetComponent<RectTransform>(), 230, 1);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(2).GetComponent<RectTransform>(), -800, 1);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(3).GetComponent<RectTransform>(), -800, 1);
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
        if (Recolectar.WasPressedThisFrame() && EstaEnRango && !EstaDentro && Mis.HayMisionDefensa() && SembradioUI.SemillaPlantada != null && !batalla.escenaCargada && TieneBatalla) 
        {
            batalla.Iniciar(gameObject);
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

            if (Mis.HayMisionDefensa() && TieneBatalla && SembradioUI.SemillaPlantada != null )
            {
                Gata.GetChild(3).GetChild(2).gameObject.SetActive(true);
                Gata.GetChild(3).GetChild(3).gameObject.SetActive(true);

                Gata.GetChild(3).GetChild(3).GetComponent<Image>().sprite = icono2;
                batalla.Fruta = SembradioUI.SemillaPlantada.Nombre;
                batalla.Item = SembradioUI.SemillaPlantada.Nombre;
                Gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(1, 0, 0);
                Gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(3, 0, 0);
            }
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
            Gata.GetChild(3).GetChild(2).gameObject.SetActive(false);
            Gata.GetChild(3).GetChild(3).gameObject.SetActive(false);
            iconoActualInteractuar = null;
            textoActualRecolectar = "";



            Gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
            Gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);

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
        if (CanvasMenu != null) { CanvasMenu.SetActive(false); }
        if (BotonCerrar.transform.childCount > 1)
        {
            BotonCerrar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = ColorBotones[1];
        }
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>(), 0, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(1).GetComponent<RectTransform>(), 0, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(2).GetComponent<RectTransform>(), 22.5f, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetChild(3).GetComponent<RectTransform>(), -12.5f, 1);
    }
}
