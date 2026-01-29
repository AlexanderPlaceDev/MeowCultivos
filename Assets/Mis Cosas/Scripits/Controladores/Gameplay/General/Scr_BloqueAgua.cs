using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_BloqueAgua : MonoBehaviour
{
    [Header("Configuración del spawner")]
    [SerializeField] private Sprite IconoAgua;
    [SerializeField] private Sprite IconoCaña;
    [SerializeField] private Sprite IconoExclamacion;
    [SerializeField] private bool EstaDentro;
    [SerializeField] private float velocidadGiro;

    [Header("Estado del Bloque")]
    private bool Recolectando;
    private bool Pescando;
    private bool estaLejos;
    private GameObject Herramienta;
    private Transform gata;
    private bool PausandoPesca;
    private Animator animatorGata;

    [Header("Pesca - Minijuego")]
    [SerializeField] private float tiempoMinPicada = 1f;
    [SerializeField] private float tiempoMaxPicada = 10f;
    [SerializeField] private float ventanaReaccion = 2f;
    [SerializeField] private GameObject JuegoPesca;
    private GameObject CamaraIzquierda;
    private GameObject CamaraDerecha;
    private GameObject CamaraPrincipal;

    private bool EsperandoPicada;
    private bool VentanaActiva;
    bool usandoCamaraIzquierda;
    Scr_MiniJuegoPesca miniJuego;




    PlayerInput playerInput;
    private InputAction Regar;
    private InputAction Pescar;
    InputIconProvider IconProvider;

    private Sprite iconoActualRegar = null;
    private string textoActualRegar = "";

    void Awake()
    {
        gata = GameObject.Find("Gata").GetComponent<Transform>();
        animatorGata = gata.GetComponent<Animator>();


        Herramienta = gata.GetChild(0).GetChild(0).GetChild(0).GetChild(1)
            .GetChild(0).GetChild(1).GetChild(0).GetChild(0)
            .GetChild(0).GetChild(2).gameObject;

        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();

        Regar = playerInput.actions["Regar"];
        Pescar = playerInput.actions["Talar"]; // Pescar = Caña
        CamaraDerecha = JuegoPesca.transform.GetChild(0).gameObject;
        CamaraIzquierda = JuegoPesca.transform.GetChild(1).gameObject;
        CamaraPrincipal = GameObject.Find("Cosas Inutiles").transform.GetChild(2).gameObject;
    }

    void OnEnable()
    {
        Scr_MiniJuegoPesca.OnSolicitarCambioCamara += CambiarCamaraPesca;
        Scr_MiniJuegoPesca.OnFinMiniJuego += ResolverFinPesca;

    }

    void OnDisable()
    {
        Scr_MiniJuegoPesca.OnSolicitarCambioCamara -= CambiarCamaraPesca;
        Scr_MiniJuegoPesca.OnFinMiniJuego -= ResolverFinPesca;

    }



    void Update()
    {
        if (!Recolectando && !Pescando)
        {
            if (EstaDentro)
            {
                // --------- EVALUACIÓN DE HABILIDADES ---------
                bool tieneRegadera = PlayerPrefs.GetString("Habilidad:Regadera", "No") == "Si";
                bool tieneCubeta = PlayerPrefs.GetString("Habilidad:Cubeta", "No") == "Si";
                bool tieneCaña = PlayerPrefs.GetString("Habilidad:Caña", "No") == "Si";

                bool puedeRegar = tieneRegadera || tieneCubeta;

                // Si no tiene ninguna habilidad relevante, no mostramos UI
                if (!puedeRegar && !tieneCaña)
                    return;

                estaLejos = false;

                // Activamos UI base
                if (!Pescando && !VentanaActiva)
                {
                    ActivarUI();
                }

                // --------- REGAR ---------
                if (puedeRegar)
                {
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRegar = true;
                    gata.GetChild(3).GetChild(0).gameObject.SetActive(true);

                    IconProvider.ActualizarIconoUI(
                        Regar,
                        gata.GetChild(3).GetChild(0),
                        ref iconoActualRegar,
                        ref textoActualRegar,
                        true
                    );
                }
                else
                {
                    gata.GetChild(3).GetChild(0).gameObject.SetActive(false);
                    gata.GetChild(3).GetChild(1).gameObject.SetActive(false);
                }

                // --------- PESCAR (CAÑA) ---------
                if (tieneCaña)
                {
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = true;
                    gata.GetChild(3).GetChild(2).gameObject.SetActive(true);

                    IconProvider.ActualizarIconoUI(
                        Pescar,
                        gata.GetChild(3).GetChild(2),
                        ref iconoActualRegar,
                        ref textoActualRegar,
                        true
                    );
                }
                else
                {
                    gata.GetChild(3).GetChild(2).gameObject.SetActive(false);
                    gata.GetChild(3).GetChild(3).gameObject.SetActive(false);
                }

                // --------- CENTRADO DE ICONOS ---------
                // Se ejecuta DESPUÉS de saber qué habilidades están activas

                // Caso 1: SOLO REGAR (Regadera o Cubeta)
                if (puedeRegar && !tieneCaña)
                {
                    // Centramos iconos de regar (child 0 y 1)
                    gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
                    gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);
                }
                // Caso 2: SOLO CAÑA
                else if (!puedeRegar && tieneCaña)
                {
                    // Centramos iconos de caña (child 2 y 3)
                    gata.GetChild(3).GetChild(2).transform.localPosition = new Vector3(-1, 0, 0);
                    gata.GetChild(3).GetChild(3).transform.localPosition = new Vector3(1, 0, 0);
                }
                // Caso 3: REGAR + CAÑA
                else if (puedeRegar && tieneCaña)
                {
                    // Centramos el grupo completo de 4 iconos
                    gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(1, 0, 0);
                    gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(3, 0, 0);

                    gata.GetChild(3).GetChild(2).transform.localPosition = new Vector3(-1, 0, 0);
                    gata.GetChild(3).GetChild(3).transform.localPosition = new Vector3(-3, 0, 0);
                }


                // Spawn de herramienta según habilidades
                SpawnearHerramienta();

                // --------- ANIMACIÓN DE REGAR ---------
                if (puedeRegar && gata.GetComponent<Animator>().GetBool("Regando"))
                {
                    Recolectando = true;
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRegar = false;
                    StartCoroutine(Esperar());
                }

                // --------- ANIMACIÓN DE PESCAR ---------
                if (tieneCaña && animatorGata.GetBool("Talando") && !PausandoPesca)
                {
                    Recolectando = true;
                    PausandoPesca = true;

                    gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = false;

                    StartCoroutine(EsperarPesca());
                }

            }
            else
            {
                if (!estaLejos)
                {
                    DesactivarUI();
                    estaLejos = true;
                }
            }
        }

        if (Recolectando && !VentanaActiva)
        {
            DesactivarUI();
            Quaternion objetivo = Quaternion.LookRotation(
                new Vector3(transform.position.x, gata.position.y, transform.position.z) - gata.position
            );

            gata.rotation = Quaternion.RotateTowards(
                gata.rotation,
                objetivo,
                velocidadGiro * Time.deltaTime
            );
        }
    }

    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(3f);

        gata.GetComponent<Animator>().speed = 1;
        Recolectando = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().Regando = false;

        DarAgua();
    }

    IEnumerator EsperarPesca()
    {
        Pescando = true;

        // Esperamos hasta que Talando alcance el frame deseado
        while (true)
        {
            AnimatorStateInfo info = animatorGata.GetCurrentAnimatorStateInfo(0);

            if (info.IsName("Gata_Picar") && info.normalizedTime >= 0.75f)
            {
                animatorGata.speed = 0; // congelamos pose exacta
                break;
            }

            yield return null;
        }

        StartCoroutine(EsperarPicada());
    }


    IEnumerator EsperarPicada()
    {
        EsperandoPicada = true;

        float tiempo = Random.Range(tiempoMinPicada, tiempoMaxPicada);
        yield return new WaitForSeconds(tiempo);

        StartCoroutine(VentanaDePesca());
    }

    IEnumerator VentanaDePesca()
    {
        VentanaActiva = true;

        // Activamos UI de pesca
        gata.GetChild(3).gameObject.SetActive(true);
        gata.GetChild(3).GetChild(2).gameObject.SetActive(false);

        iconoActualRegar = null;
        textoActualRegar = null;
        gata.GetChild(3).GetChild(3).gameObject.SetActive(false);
        IconProvider.ActualizarIconoUI(
                        Pescar,
                        gata.GetChild(3).GetChild(0),
                        ref iconoActualRegar,
                        ref textoActualRegar,
                        true
                    );


        // Cambiar icono a exclamación
        gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = IconoExclamacion;

        // Centramos iconos
        gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
        gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);

        bool acerto = false;
        float tiempoRestante = ventanaReaccion;

        // Ventana de input controlada
        while (tiempoRestante > 0f)
        {
            if (Pescar.IsPressed())
            {
                acerto = true;
                break;
            }

            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        VentanaActiva = false;

        // SIEMPRE resolvemos
        if (acerto)
            EntrarMiniJuegoPesca();
        else
            FallarPesca();
    }


    void EntrarMiniJuegoPesca()
    {
        CamaraPrincipal.SetActive(false);
        GameObject contenedor = JuegoPesca.transform.GetChild(2).gameObject;
        contenedor.SetActive(true);              // 🔹 PRIMERO activar

        miniJuego = contenedor.GetComponent<Scr_MiniJuegoPesca>();
        miniJuego.enabled = true;                // 🔹 luego habilitar script
        miniJuego.Resetear();                    // 🔹 y recién resetear


        AplicarCamara();
    }



    void CambiarCamaraPesca()
    {
        usandoCamaraIzquierda = !usandoCamaraIzquierda;
        AplicarCamara();
    }


    void AplicarCamara()
    {
        CamaraIzquierda.SetActive(usandoCamaraIzquierda);
        CamaraDerecha.SetActive(!usandoCamaraIzquierda);

        if (usandoCamaraIzquierda)
            AcomodarCaña(0);
        else
            AcomodarCaña(1);

    }



    void AcomodarCaña(int lado)
    {
        if (lado == 0)
        {
            JuegoPesca.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-130, 5, 0);
            JuegoPesca.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, -180f, 0f);
        }
        else
        {
            JuegoPesca.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(130, 5, 0);
            JuegoPesca.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    void FallarPesca()
    {
        Debug.Log("❌ Falló la pesca");

        // Reanudar animator
        animatorGata.speed = 1;
        animatorGata.SetBool("Talando", false);

        // Reset de estados
        Recolectando = false;
        Pescando = false;
        PausandoPesca = false;
        EsperandoPicada = false;

        // Reactivar movimiento
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;

        // Restaurar icono de caña
        gata.GetChild(3).GetChild(3).GetComponent<Image>().sprite = IconoCaña;

        // Ocultar UI
        DesactivarUI();
    }

    void ResolverFinPesca(bool gano)
    {
        // 🔹 Cámara
        CamaraIzquierda.SetActive(false);
        CamaraDerecha.SetActive(false);
        CamaraPrincipal.SetActive(true);
        JuegoPesca.transform.GetChild(2).gameObject.SetActive(false);

        // 🔹 Animator
        animatorGata.speed = 1;
        animatorGata.SetBool("Talando", false);

        // 🔹 Estados
        Recolectando = false;
        Pescando = false;
        PausandoPesca = false;
        EsperandoPicada = false;
        VentanaActiva = false;

        // 🔹 Movimiento
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;

        // 🔹 UI
        DesactivarUI();

        // 🔹 Reset minijuego

        if (gano)
        {
            Debug.Log("🎁 Dar recompensa (aquí agregas objeto)");
            // Aquí tú agregas el item
        }
        else
        {
            FallarPesca();
        }
    }



    void DarAgua()
    {
        if (PlayerPrefs.GetString("Habilidad:Regadera", "No") == "Si")
        {
            PlayerPrefs.SetInt("CantidadAgua", 50);
        }
        else if (PlayerPrefs.GetString("Habilidad:Cubeta", "No") == "Si")
        {
            PlayerPrefs.SetInt("CantidadAgua", 25);
        }
    }

    void ActivarUI()
    {
        gata.GetChild(3).gameObject.SetActive(true);
        gata.GetChild(3).GetChild(3).gameObject.SetActive(true);

        // Posiciones base (se ajustan luego según habilidades)
        gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(1, 0, 0);
        gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(3, 0, 0);

        gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = IconoAgua;
        gata.GetChild(3).GetChild(3).GetComponent<Image>().sprite = IconoCaña;
    }

    void DesactivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRegar = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = false;
        gata.GetChild(3).gameObject.SetActive(false);
        gata.GetChild(3).GetChild(2).gameObject.SetActive(false);
        gata.GetChild(3).GetChild(3).gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gata")
        {
            EstaDentro = true;
            SpawnearHerramienta();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Gata")
        {
            Herramienta.SetActive(false);
            Herramienta.transform.GetChild(2).gameObject.SetActive(false);
            Herramienta.transform.GetChild(3).gameObject.SetActive(false);
            EstaDentro = false;
        }
    }

    void SpawnearHerramienta()
    {
        Herramienta.SetActive(true);

        if (PlayerPrefs.GetString("Habilidad:Caña", "No") == "Si")
        {
            // Herramienta.transform.GetChild(3).gameObject.SetActive(true);
        }
        else if (PlayerPrefs.GetString("Habilidad:Cubeta", "No") == "Si")
        {
            Herramienta.transform.GetChild(2).gameObject.SetActive(true);
        }
    }
}
