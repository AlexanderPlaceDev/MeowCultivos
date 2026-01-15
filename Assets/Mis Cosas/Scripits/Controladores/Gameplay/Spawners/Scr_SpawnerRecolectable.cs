using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_SpawnerRecolectable : MonoBehaviour
{
    [Header("Configuración del spawner")]
    [SerializeField] bool OcupaPadre;
    [SerializeField] GameObject Padre;
    [SerializeField] private Sprite icono;
    [SerializeField] private Sprite teclaIcono;
    [SerializeField] private float distancia;
    [SerializeField] private float velocidadGiro;
    [SerializeField] private Scr_CreadorObjetos objetoQueDa;
    [SerializeField] private int[] minimoMaximo;
    [SerializeField] string Habilidad;
    [SerializeField] string Habilidad2;
    [SerializeField] float[] TiempoRespawn;

    [Header("Estado del spawner")]
    private Scr_CambiadorBatalla batalla;
    public bool TieneObjeto = true;
    private bool recolectando;
    private bool estaLejos;
    private float Tiempo;
    private float TiempoRespawnAleatorio;
    private Transform gata;

    PlayerInput playerInput;
    private InputAction Recolectar;
    InputIconProvider IconProvider;
    private Sprite iconoActualRecolectar = null;
    private string textoActualRecolectar = "";
    public bool uiActiva = false;

    void Start()
    {
        gata = GameObject.Find("Gata").transform;
        TiempoRespawnAleatorio = Random.Range(TiempoRespawn[0], TiempoRespawn[1]);
        batalla = GetComponent<Scr_CambiadorBatalla>();

        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        Recolectar = playerInput.actions["Recolectar"];
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (uiActiva)
        {
            IconProvider.ActualizarIconoUI(Recolectar, gata.GetChild(3).GetChild(2), ref iconoActualRecolectar, ref textoActualRecolectar, true);
        }
    }

    void Update()
    {
        if (TieneObjeto)
        {
            float distanciaGata = Vector3.Distance(gata.position, transform.position);

            if (!recolectando)
            {
                // Activar UI si está cerca
                if (distanciaGata < distancia)
                {
                    estaLejos = false;
                    if (!uiActiva) ActivarUI();

                    // Actualizar icono continuamente
                    IconProvider.ActualizarIconoUI(Recolectar, gata.GetChild(3).GetChild(2), ref iconoActualRecolectar, ref textoActualRecolectar, true);

                    // Iniciar animación de recolección
                    if (gata.GetComponent<Animator>().GetBool("Recolectando"))
                    {
                        gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = true;
                        recolectando = true;
                        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
                        StartCoroutine(Esperar());
                    }
                }
                else if (!estaLejos)
                {
                    DesactivarUI();
                    estaLejos = true;
                }
            }
        }
        else
        {
            // Manejo de respawn
            Tiempo += Time.deltaTime;
            if (Tiempo >= TiempoRespawnAleatorio)
            {
                Tiempo = 0;
                TiempoRespawnAleatorio = Random.Range(TiempoRespawn[0], TiempoRespawn[1]);
                if (OcupaPadre && Padre.GetComponent<MeshRenderer>().enabled)
                    ActivarObjeto();
            }
            else
            {
                DesactivarObjeto();
            }
        }

        if (recolectando)
        {
            // Girar hacia el objeto mientras recolecta
            Quaternion objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, gata.position.y, transform.position.z) - gata.position);
            gata.rotation = Quaternion.RotateTowards(gata.rotation, objetivo, velocidadGiro * Time.deltaTime);
            DesactivarUI();
        }
    }

    void ActivarObjeto()
    {
        TieneObjeto = true;
        try
        {
            GetComponent<Collider>().enabled = true;
            GetComponent<MeshRenderer>().enabled = true;
        }
        catch
        {
            foreach (Transform hijo in transform)
            {
                if (hijo.GetComponent<Collider>()) hijo.GetComponent<Collider>().enabled = true;
                if (hijo.GetComponent<SkinnedMeshRenderer>()) hijo.GetComponent<SkinnedMeshRenderer>().enabled = true;
            }
        }
    }

    void DesactivarObjeto()
    {
        try
        {
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
        }
        catch
        {
            foreach (Transform hijo in transform)
            {
                if (hijo.GetComponent<Collider>()) hijo.GetComponent<Collider>().enabled = false;
                if (hijo.GetComponent<SkinnedMeshRenderer>()) hijo.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
        }
    }

    IEnumerator Esperar()
    {
        float animSpeed = 1f;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;

        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" && !string.IsNullOrEmpty(Habilidad))
            animSpeed = 2f;

        gata.GetComponent<Animator>().speed = animSpeed;

        yield return new WaitForSeconds(5.22f / animSpeed);

        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
        gata.GetComponent<Animator>().speed = 1;

        recolectando = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = false;

        if (TieneObjeto)
        {
            DarObjeto();
            TieneObjeto = false;
        }
    }

    void DarObjeto()
    {
        int cantidad = Random.Range(minimoMaximo[0], minimoMaximo[1]);
        if (PlayerPrefs.GetString("Habilidad:" + Habilidad2, "No") == "Si")
            cantidad *= 2;

        ActualizarInventario(cantidad, objetoQueDa);
    }

    void ActualizarInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        if (cantidad <= 0 || objeto == null) return;

        Scr_Inventario inventario = gata.GetChild(7).GetComponent<Scr_Inventario>();
        inventario.AgregarObjeto(objeto.Nombre, cantidad, mostrarUI: true, darXP: true);
    }

    void ActivarUI()
    {
        uiActiva = true;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = true;
        gata.GetChild(3).gameObject.SetActive(true);

        gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = icono;


        gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
        gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);
        IconProvider.ActualizarIconoUI(Recolectar, gata.GetChild(3).GetChild(0), ref iconoActualRecolectar, ref textoActualRecolectar, true);
    }

    void DesactivarUI()
    {
        uiActiva = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
        gata.GetChild(3).gameObject.SetActive(false);

        iconoActualRecolectar = null;
        textoActualRecolectar = "";

        if (PlayerPrefs.GetString("TutorialPeleas", "NO") == "SI")
        {
            gata.GetChild(3).GetChild(2).gameObject.SetActive(false);
            gata.GetChild(3).GetChild(3).gameObject.SetActive(false);
        }
    }

    
}
