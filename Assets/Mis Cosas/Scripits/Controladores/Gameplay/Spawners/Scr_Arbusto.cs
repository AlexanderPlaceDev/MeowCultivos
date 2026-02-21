using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_Arbusto : MonoBehaviour
{
    [Header("Configuración del Arbusto")]
    [SerializeField] private Sprite icono;
    [SerializeField] private Sprite icono2;
    [SerializeField] private string tecla;
    [SerializeField] private Sprite teclaIcono;
    [SerializeField] private Material[] tipos;
    [SerializeField] private float distancia;
    [SerializeField] private float velocidadGiro;
    [SerializeField] private Scr_CreadorObjetos[] objetosQueDa;
    [SerializeField] private int[] minimoMaximo;
    [SerializeField] string HabilidadGuante;
    [SerializeField] string HabilidadRamas;
    [SerializeField] Scr_CreadorObjetos Rama;

    [Header("Estado del Arbusto")]
    private Scr_CambiadorBatalla batalla;
    private int tipoActual = 0;
    private bool recolectando;
    private bool tieneMoras;
    private bool estaLejos;

    private Transform gata;

    PlayerInput playerInput;
    private InputAction Interactuar;
    private InputAction Recolectar;
    InputIconProvider IconProvider;
    Scr_ControladorMisiones Mis;

    // Variables por botón para evitar parpadeo
    private Sprite iconoActualRecolectar = null;
    private string textoActualRecolectar = "";
    private Sprite iconoActualInteractuar = null;
    private string textoActualInteractuar = "";

    public bool uiActiva = false;

    void Start()
    {
        gata = GameObject.Find("Gata").GetComponent<Transform>();
        tipoActual = Random.Range(0, 4);
        GetComponent<MeshRenderer>().material = tipos[tipoActual];
        tieneMoras = (tipoActual > 0);
        batalla = GetComponent<Scr_CambiadorBatalla>();
        batalla.Fruta = objetosQueDa[tipoActual].Nombre;
        batalla.Item = objetosQueDa[tipoActual].Nombre;

        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();

        Mis = GameObject.Find("ControladorMisiones").GetComponent<Scr_ControladorMisiones>();

        Interactuar = playerInput.actions["Interactuar"];
        Recolectar = playerInput.actions["Recolectar"];
    }

    void Update()
    {
        if (!tieneMoras) return;

        float distanciaGata = Vector3.Distance(gata.position, transform.position);

        if (!recolectando)
        {
            // Si estamos dentro de la distancia para recolectar
            if (distanciaGata < distancia)
            {
                // Solo activamos si no está activa
                if (!uiActiva)
                {
                    ActivarUI();
                }

                estaLejos = false;

                // Detectar interacción
                if (Interactuar.IsPressed() && !batalla.escenaCargada && Mis.HayMisionRecolectar())
                    batalla.Iniciar();

                if (gata.GetComponent<Animator>().GetBool("Recolectando"))
                {
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = true;
                    recolectando = true;
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
                    StartCoroutine(Esperar());
                }

                // Actualizamos iconos cada frame mientras estamos cerca
                ActualizarIconosUI();
            }
            else
            {
                // Si nos alejamos, solo desactivamos si no está desactivada
                if (!estaLejos)
                {
                    DesactivarUI();
                    estaLejos = true;
                }
            }
        }
        else
        {
            // Mientras recolectamos, giramos hacia el arbusto
            Quaternion objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, gata.position.y, transform.position.z) - gata.position);
            gata.rotation = Quaternion.RotateTowards(gata.rotation, objetivo, velocidadGiro * Time.deltaTime);
            DesactivarUI();
        }
    }

    IEnumerator Esperar()
    {
        float animSpeed = 1f;

        if (PlayerPrefs.GetString("Habilidad:" + HabilidadGuante, "No") == "Si" || string.IsNullOrEmpty(HabilidadGuante))
            animSpeed = 2f;

        gata.GetComponent<Animator>().speed = animSpeed;
        yield return new WaitForSeconds(5.22f / animSpeed);
        gata.GetComponent<Animator>().speed = 1f;

        recolectando = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = false;

        if (tieneMoras)
        {
            if (PlayerPrefs.GetString("Habilidad:" + HabilidadRamas, "No") == "Si" || string.IsNullOrEmpty(HabilidadRamas))
            {
                int cantidad = Random.Range(minimoMaximo[0], minimoMaximo[1]);
                ActualizarInventario(cantidad, Rama);
            }

            DarMoras();
            DarFibra();
            tieneMoras = false;
        }

        tipoActual = 0;
        GetComponent<MeshRenderer>().material = tipos[tipoActual];
    }

    void DarMoras()
    {
        int cantidad = Random.Range(minimoMaximo[0], minimoMaximo[1]);
        ActualizarInventario(cantidad, objetosQueDa[tipoActual]);
    }

    void DarFibra()
    {
        int cantidad = Random.Range(minimoMaximo[2], minimoMaximo[3]);
        ActualizarInventario(cantidad, objetosQueDa[0]);
    }

    void ActualizarInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        if (cantidad <= 0 || objeto == null) return;

        Scr_Inventario inventario = FindObjectOfType<Scr_Inventario>();
        if (inventario == null) return;

        int agregada = inventario.AgregarObjeto(objeto.Nombre, cantidad, true, true);

        Scr_DatosSingletonBatalla singleton = GameObject.Find("Singleton")?.GetComponent<Scr_DatosSingletonBatalla>();
        if (singleton == null) return;

        singleton.ObjetosRecompensa.Add(objeto);
        singleton.CantidadesRecompensa.Add(agregada > 0 ? agregada : cantidad);
    }

    void ActivarUI()
    {
        uiActiva = true;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = true;
        gata.GetChild(3).gameObject.SetActive(true);

        gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = icono;


        if (Mis.HayMisionRecolectar())
        {
            gata.GetChild(3).GetChild(2).gameObject.SetActive(true);
            gata.GetChild(3).GetChild(3).gameObject.SetActive(true);

            gata.GetChild(3).GetChild(3).GetComponent<Image>().sprite = icono2;

            gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(1, 0, 0);
            gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(3, 0, 0);
        }
        // Actualizamos ambos botones
        //ActualizarIconosUI();
    }

    void DesactivarUI()
    {
        uiActiva = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
        gata.GetChild(3).gameObject.SetActive(false);

        iconoActualRecolectar = null;
        textoActualRecolectar = "";
        iconoActualInteractuar = null;
        textoActualInteractuar = "";

        //(PlayerPrefs.GetString("TutorialPeleas", "NO") == "SI")
        if (Mis.HayMisionRecolectar())
        {
            gata.GetChild(3).GetChild(2).gameObject.SetActive(false);
            gata.GetChild(3).GetChild(3).gameObject.SetActive(false);
        }

        gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
        gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);
    }

    void ActualizarIconosUI()
    {
        //if (!uiActiva) return;
        IconProvider.ActualizarIconoUI(Recolectar, gata.GetChild(3).GetChild(0), ref iconoActualRecolectar, ref textoActualRecolectar, true);
        IconProvider.ActualizarIconoUI(Interactuar, gata.GetChild(3).GetChild(2), ref iconoActualInteractuar, ref textoActualInteractuar, true);
    }


}
