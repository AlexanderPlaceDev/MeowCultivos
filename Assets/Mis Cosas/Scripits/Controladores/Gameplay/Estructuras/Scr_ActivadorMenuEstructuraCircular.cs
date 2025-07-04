using TMPro;
using PrimeTween;
using UnityEngine;
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

    void Awake()
    {
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        ControladorMenu = Gata.GetChild(6).gameObject;
        Camara360 = GameObject.Find("Camara 360");
        Canvas = GameObject.Find("Canvas");
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
            Gata.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Tecla;
            Gata.GetChild(3).GetChild(0).GetComponent<Image>().sprite = IconoTecla;
            Gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = Icono;
            Gata.GetChild(3).gameObject.SetActive(true);
        }
        if (Vector3.Distance(Gata.position, transform.position) > Distancia && !EstaLejos)
        {
            Gata.GetChild(3).gameObject.SetActive(false);
            EstaLejos = true;
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


        if (!EstaLejos && Input.GetKeyDown(KeyCode.E))
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
            transform.GetChild(1).gameObject.SetActive(true);
            ControladorMenu.GetComponent<Scr_ControladorMenuGameplay>().enabled = false;// Desactiva logica reloj
            Canvas.transform.GetChild(2).gameObject.SetActive(false);//Desactiva UI reloj
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>(), -250, 1);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(3).GetChild(1).GetComponent<RectTransform>(), 250, 1);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(3).GetChild(2).GetComponent<RectTransform>(), -850, 1);
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
        transform.GetChild(1).gameObject.SetActive(false);
        ControladorMenu.GetComponent<Scr_ControladorMenuGameplay>().enabled = true; //Activa logica del reloj
        Canvas.transform.GetChild(2).gameObject.SetActive(true);// Activa UI Reloj
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>(), 30, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(3).GetChild(1).GetComponent<RectTransform>(), 0, 1);
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(3).GetChild(2).GetComponent<RectTransform>(), -610, 1);
    }
}
