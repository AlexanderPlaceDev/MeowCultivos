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

    [Header("Debug (solo lectura)")]
    [SerializeField] private float tiempoActualRespawn;
    [SerializeField] private float tiempoRestanteRespawn;
    [SerializeField] private bool empiezaSinObjeto = false;



    PlayerInput playerInput;
    private InputAction Recolectar;
    InputIconProvider IconProvider;
    private Sprite iconoActualSpawn = null;
    private string textoActualSpawn = "";
    public bool uiActiva = false;

    private string KeyTieneObjeto => "Recolectable_Tiene_" + gameObject.name;
    private string KeyRespawn => "Recolectable_Respawn_" + gameObject.name;
    private string KeyRespawnObjetivo => "Recolectable_RespawnObjetivo_" + gameObject.name;


    void Start()
    {
        gata = GameObject.Find("Gata").transform;
        batalla = GetComponent<Scr_CambiadorBatalla>();

        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        Recolectar = playerInput.actions["Recolectar"];
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();

        // ================= ESTADO INICIAL REAL =================
        bool estadoInicialTieneObjeto = false;

        if (TryGetComponent<MeshRenderer>(out var mr))
        {
            estadoInicialTieneObjeto = mr.enabled;
        }
        else
        {
            foreach (Transform h in transform)
            {
                if (h.GetComponent<Renderer>()?.enabled == true)
                {
                    estadoInicialTieneObjeto = true;
                    break;
                }
            }
        }

        // ================= CARGA / INIT PERSISTENTE =================
        bool tieneSave = PlayerPrefs.HasKey(KeyTieneObjeto);

        if (!tieneSave)
        {
            // PRIMER ARRANQUE
            TieneObjeto = estadoInicialTieneObjeto;

            if (TieneObjeto)
            {
                ActivarObjeto();
            }
            else
            {
                TieneObjeto = false;
                Tiempo = 0f;
                TiempoRespawnAleatorio = Random.Range(TiempoRespawn[0], TiempoRespawn[1]);

                PlayerPrefs.SetInt(KeyTieneObjeto, 0);
                PlayerPrefs.SetFloat(KeyRespawn, Tiempo);
                PlayerPrefs.SetFloat(KeyRespawnObjetivo, TiempoRespawnAleatorio);
                PlayerPrefs.Save();

                DesactivarObjeto();
            }
        }
        else
        {
            TieneObjeto = PlayerPrefs.GetInt(KeyTieneObjeto) == 1;

            if (TieneObjeto)
            {
                ActivarObjeto();
            }
            else
            {
                Tiempo = PlayerPrefs.GetFloat(KeyRespawn, 0f);

                // 🔒 CREAR Y GUARDAR EL OBJETIVO SI NO EXISTE
                if (!PlayerPrefs.HasKey(KeyRespawnObjetivo))
                {
                    TiempoRespawnAleatorio = Random.Range(TiempoRespawn[0], TiempoRespawn[1]);
                    PlayerPrefs.SetFloat(KeyRespawnObjetivo, TiempoRespawnAleatorio);
                    PlayerPrefs.Save();
                }
                else
                {
                    TiempoRespawnAleatorio = PlayerPrefs.GetFloat(KeyRespawnObjetivo);
                }

                DesactivarObjeto();
            }
        }

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
            IconProvider.ActualizarIconoUI(Recolectar, gata.GetChild(3).GetChild(2), ref iconoActualSpawn, ref textoActualSpawn, true);
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
                    IconProvider.ActualizarIconoUI(Recolectar, gata.GetChild(3).GetChild(2), ref iconoActualSpawn, ref textoActualSpawn, true);

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
            Tiempo += Time.deltaTime;

            tiempoActualRespawn = Tiempo;
            tiempoRestanteRespawn = Mathf.Max(0f, TiempoRespawnAleatorio - Tiempo);

            // 🔒 Persistir tiempo transcurrido
            PlayerPrefs.SetFloat(KeyRespawn, Tiempo);

            if (Tiempo >= TiempoRespawnAleatorio)
            {
                Tiempo = 0;
                tiempoActualRespawn = 0;
                tiempoRestanteRespawn = 0;

                TiempoRespawnAleatorio = Random.Range(TiempoRespawn[0], TiempoRespawn[1]);
                PlayerPrefs.SetFloat(KeyRespawnObjetivo, TiempoRespawnAleatorio);

                ActivarObjeto();

                PlayerPrefs.SetInt(KeyTieneObjeto, 1);
                PlayerPrefs.DeleteKey(KeyRespawn);
                PlayerPrefs.Save();
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

            PlayerPrefs.SetInt(KeyTieneObjeto, 0);
            PlayerPrefs.SetFloat(KeyRespawn, Tiempo);
            PlayerPrefs.SetFloat(KeyRespawnObjetivo, TiempoRespawnAleatorio);
            PlayerPrefs.Save();

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
        iconoActualSpawn = null;
        textoActualSpawn = "";
        IconProvider.ActualizarIconoUI(Recolectar, gata.GetChild(3).GetChild(0), ref iconoActualSpawn, ref textoActualSpawn, true);
    }

    void DesactivarUI()
    {
        uiActiva = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
        gata.GetChild(3).gameObject.SetActive(false);

        iconoActualSpawn = null;
        textoActualSpawn = "";

        if (PlayerPrefs.GetString("TutorialPeleas", "NO") == "SI")
        {
            gata.GetChild(3).GetChild(2).gameObject.SetActive(false);
            gata.GetChild(3).GetChild(3).gameObject.SetActive(false);
        }
    }

    
}
