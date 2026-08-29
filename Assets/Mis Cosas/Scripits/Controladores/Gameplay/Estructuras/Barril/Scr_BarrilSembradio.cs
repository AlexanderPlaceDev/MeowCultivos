using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_BarrilSembradio : MonoBehaviour
{
    [SerializeField] int ID;
    [SerializeField] private Sprite icono;
    [SerializeField] private string tecla;
    [SerializeField] private Sprite teclaIcono;
    [SerializeField] private float distancia;
    [SerializeField] string Habilidad;
    [SerializeField] private float velocidadGiro;
    [SerializeField] public Scr_CreadorObjetos TipoFruta;
    [SerializeField] public int Cantidad;
    [SerializeField] public int CantidadMaxima;
    [SerializeField] Sprite[] IconosPanel;
    [SerializeField] Scr_CreadorObjetos[] FrutasQueRecolecta;
    [SerializeField] public bool UltimoDiaPlanta;

    [SerializeField] private Scr_ControladorSembradioUI Sembradio;

    private bool recolectando;
    private bool estaLejos;
    private bool uiActiva = false;

    private Transform gata;

    PlayerInput playerInput;
    private InputAction Recolectar;
    InputIconProvider IconProvider;


    private Sprite iconoActualRecolectar = null;
    private string textoActualRecolectar = "";
    void Start()
    {
        gata = GameObject.Find("Gata").GetComponent<Transform>();

        foreach (Scr_CreadorObjetos Fruta in FrutasQueRecolecta)
        {
            if (Fruta.Nombre == PlayerPrefs.GetString("BarrilSembradio Futa:" + ID, "No"))
            {
                TipoFruta = Fruta;
            }
        }

        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Recolectar = playerInput.actions["Recolectar"];
        Cantidad = PlayerPrefs.GetInt("BarrilSembradio Cantidad:" + ID, 0);
        UltimoDiaPlanta = PlayerPrefs.GetInt(
    "BarrilSembradio UltimoDia:" + ID,
    0
) == 1;

    }

    void Update()
    {
        if (TipoFruta != null)
        {
            if (PlayerPrefs.GetString("BarrilSembradio Futa:" + ID, "No") != TipoFruta.Nombre)
            {
                PlayerPrefs.SetString("BarrilSembradio Futa:" + ID, TipoFruta.Nombre);
                PlayerPrefs.SetInt("BarrilSembradio Cantidad:" + ID, Cantidad);
            }


            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = TipoFruta.Icono;
            transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = Cantidad.ToString();

            if (Cantidad >= CantidadMaxima || UltimoDiaPlanta)
            {
                ColocarIconoPanel(true);
            }
            else
            {
                ColocarIconoPanel(false);
            }

            if (!recolectando)
            {
                // Si se acerca, se encienden los iconos
                if (Vector3.Distance(gata.position, transform.position) < distancia)
                {
                    estaLejos = false;

                    if (!uiActiva)
                    {
                        ActivarUI();
                        uiActiva = true;
                    }

                    if (gata.GetComponent<Animator>().GetBool("Recolectando"))
                    {
                        gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = true;
                        recolectando = true;
                        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
                        StartCoroutine(Esperar());
                    }
                }
                else
                {
                    if (!estaLejos)
                    {
                        DesactivarUI();

                        uiActiva = false;
                        estaLejos = true;
                    }
                }
            }
        }

        if (recolectando)
        {
            DesactivarUI();
            Quaternion objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, gata.position.y, transform.position.z) - gata.position);
            gata.rotation = Quaternion.RotateTowards(gata.rotation, objetivo, velocidadGiro * Time.deltaTime);
        }
    }

    IEnumerator Esperar()
    {
        float animSpeed = 1f; // Valor por defecto

        // Verificar si la habilidad está activa o no
        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" && !string.IsNullOrEmpty(Habilidad))
        {
            animSpeed = 2f; // Doble de velocidad si la habilidad está activa
        }
        gata.GetComponent<Animator>().speed = animSpeed;

        yield return new WaitForSeconds(5.22f / animSpeed);
        gata.GetComponent<Animator>().speed = 1;

        recolectando = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = false;
        if (TipoFruta != null)
        {
            DarObjeto();

            PlayerPrefs.DeleteKey("BarrilSembradio Futa:" + ID);
            PlayerPrefs.DeleteKey("BarrilSembradio Cantidad:" + ID);
            PlayerPrefs.DeleteKey("BarrilSembradio UltimoDia:" + ID);

            transform.GetChild(0).gameObject.SetActive(false);

            TipoFruta = null;
            Cantidad = 0;
            UltimoDiaPlanta = false;

            if (Sembradio != null)
            {
                Sembradio.CosecharPlanta();
            }
        }
    }

    public void GuardarEstado()
    {
        PlayerPrefs.SetString(
            "BarrilSembradio Futa:" + ID,
            TipoFruta != null ? TipoFruta.Nombre : "No"
        );

        PlayerPrefs.SetInt(
            "BarrilSembradio Cantidad:" + ID,
            Cantidad
        );

        PlayerPrefs.SetInt(
            "BarrilSembradio UltimoDia:" + ID,
            UltimoDiaPlanta ? 1 : 0
        );
    }

    void DarObjeto()
    {
        ActualizarInventario(Cantidad, TipoFruta);
    }

    void ActualizarInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        Scr_Inventario controlador =
            GameObject.Find("Gata")
            .transform
            .GetChild(7)
            .GetComponent<Scr_Inventario>();

        if (controlador == null)
            return;

        controlador.AgregarObjeto(
            objeto.Nombre,
            cantidad,
            true,
            true
        );
    }

    void ActivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = true;

        Transform iconosAccion = gata.GetChild(3);

        iconosAccion.gameObject.SetActive(true);

        // Icono del objeto recolectable
        iconosAccion.GetChild(1)
            .GetComponent<Image>()
            .sprite = icono;

        // Actualizar solamente la tecla principal
        IconProvider.ActualizarIconoUI(
            Recolectar,
            iconosAccion.GetChild(0),
            ref iconoActualRecolectar,
            ref textoActualRecolectar,
            true
        );
    }
    void DesactivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;

        Transform iconosAccion = gata.GetChild(3);

        // Apagar acciones secundarias
        iconosAccion.GetChild(2).gameObject.SetActive(false);
        iconosAccion.GetChild(3).gameObject.SetActive(false);

        // Apagar todo el panel
        iconosAccion.gameObject.SetActive(false);

        iconosAccion.GetChild(0).transform.localPosition =
            new Vector3(-1, 0, 0);

        iconosAccion.GetChild(1).transform.localPosition =
            new Vector3(1, 0, 0);

        iconoActualRecolectar = null;
        textoActualRecolectar = "";
    }

    public void ColocarIconoPanel(bool EnAlerta)
    {
        if (EnAlerta)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = IconosPanel[1];
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = IconosPanel[0];
        }
    }

    public void ReiniciarBarril()
    {
        // Reiniciar variables
        TipoFruta = null;
        Cantidad = 0;
        UltimoDiaPlanta = false;

        // Reiniciar estado de UI
        recolectando = false;
        uiActiva = false;
        estaLejos = true;

        // Eliminar guardado
        PlayerPrefs.DeleteKey("BarrilSembradio Futa:" + ID);
        PlayerPrefs.DeleteKey("BarrilSembradio Cantidad:" + ID);
        PlayerPrefs.DeleteKey("BarrilSembradio UltimoDia:" + ID);

        // Ocultar panel del barril
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        // Ocultar UI de recolección por seguridad
        if (gata != null)
        {
            DesactivarUI();
        }

        PlayerPrefs.Save();
    }
}
