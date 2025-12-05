using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorMenuGameplay : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    [SerializeField] float TiempoTransicion;
    [SerializeField] Scr_CreadorTemas TemaActual;
    [SerializeField] Image Fondo;
    [SerializeField] GameObject BarraIzquierda;
    [SerializeField] GameObject BarraDerecha;
    [SerializeField] TextMeshProUGUI Hora;
    [SerializeField] TextMeshProUGUI Dia;
    [SerializeField] TextMeshProUGUI Nivel;
    [SerializeField] TextMeshProUGUI XP;
    [SerializeField] TextMeshProUGUI Dinero;

    bool Esperando = false;
    bool EstaEnMenu = false;
    float TiempoDeEspera = 0;
    GameObject Gata;
    private Animator animator;


    [SerializeField] AudioClip[] Sonidos;
    [SerializeField] AudioSource Audio;
    void Start()
    {
        // Busca y guarda una referencia al objeto de la gata
        Gata = GameObject.Find("Gata");
        animator = Menu.GetComponent<Animator>();
    }

    void Update()
    {

        ActualizarInfoPrincipal(Hora, Dia, Nivel, XP, Dinero);
        if (EstaEnMenu)
        {
            // Desactiva los componentes de movimiento de la gata mientras está en el menú
            Gata.GetComponent<Scr_GiroGata>().enabled = false;
            if (Input.GetKeyDown(KeyCode.Tab) && !Esperando && !EstaReproduciendoAnimacion())
            {

                if (Menu.transform.GetChild(2).gameObject.activeSelf)
                {
                    Esperando = true;
                    Menu.GetComponent<Animator>().Play("Cerrar");
                    Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
                }
                else
                {
                    GetComponent<Scr_CambiadorMenus>().BotonRegresar();
                }

            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !Esperando && !EstaReproduciendoAnimacion())
            {
                Esperando = true;
                RestablecerColor();
                Menu.SetActive(true);
                Menu.GetComponent<Animator>().Play("Aparecer");
                Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
            }
        }

        if (Esperando)
        {
            TiempoDeEspera += Time.deltaTime;
        }

        if (TiempoDeEspera >= TiempoTransicion)
        {
            TiempoDeEspera = 0;
            Esperando = false;
            if (EstaEnMenu)
            {
                Menu.SetActive(false);
                EstaEnMenu = false;
            }
            else
            {
                EstaEnMenu = true;

            }
        }
    }

    public bool EstaReproduciendoAnimacion()
    {
        // Comprobar todas las capas del Animator
        for (int i = 0; i < animator.layerCount; i++)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(i);

            // Comprobar si el Animator está en algún estado que no sea el predeterminado
            if (stateInfo.normalizedTime < 1f && !stateInfo.IsName("Idle"))
            {
                return true;  // Si está en una animación activa
            }
        }
        return false;  // No se está reproduciendo ninguna animación activa
    }

    void RestablecerColor()
    {
        Fondo.color = TemaActual.ColoresMenu[2];

        BarraIzquierda.transform.GetChild(1).GetComponent<Image>().color = TemaActual.ColoresMenu[0];
        BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = TemaActual.ColoresMenu[1];

        BarraDerecha.transform.GetChild(1).GetComponent<Image>().color = TemaActual.ColoresMenu[0];
        BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = TemaActual.ColoresMenu[1];
    }

    void ActualizarInfoPrincipal(TextMeshProUGUI Hora, TextMeshProUGUI Dia, TextMeshProUGUI Nivel, TextMeshProUGUI XP, TextMeshProUGUI Dinero)
    {
        if (GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().MinutoActual == 0)
        {
            Hora.text = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().HoraActual + ":00";
        }
        else
        {
            Hora.text = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().HoraActual + ":" + GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().MinutoActual;
        }
        Dia.text = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().DiaActual;
        Nivel.text = "Lv." + PlayerPrefs.GetInt("Nivel", 0);
        XP.text = PlayerPrefs.GetInt("XPActual", 0) + " / " + PlayerPrefs.GetInt("XPSiguiente", 10) + " :XP";
        Dinero.text = "$" + PlayerPrefs.GetInt("Dinero", 0);
    }


    public void ReproducirSonidoBoton(int Sonido)
    {
        Audio.PlayOneShot(Sonidos[Sonido]);
    }
}
