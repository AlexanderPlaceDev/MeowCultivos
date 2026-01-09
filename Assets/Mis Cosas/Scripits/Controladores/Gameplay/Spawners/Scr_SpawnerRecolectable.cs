using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_SpawnerRecolectable : MonoBehaviour
{
    [Header("Configuración del spawner")]
    [SerializeField] bool OcupaPadre;
    [SerializeField] GameObject Padre;
    [SerializeField] private Sprite icono;
    [SerializeField] private string tecla;
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

    void Start()
    {
        gata = GameObject.Find("Gata").GetComponent<Transform>();
        TiempoRespawnAleatorio = Random.Range(TiempoRespawn[0], TiempoRespawn[1]);
        batalla = GetComponent<Scr_CambiadorBatalla>();
    }

    void Update()
    {
        if (TieneObjeto)
        {
            if (!recolectando)
            {
                // Si se acerca, se encienden los iconos
                if (Vector3.Distance(gata.position, transform.position) < distancia)
                {
                    estaLejos = false;
                    ActivarUI();
                    if (Input.GetKeyDown(KeyCode.E) && !batalla.escenaCargada)
                    {
                        batalla.Iniciar();
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
                        estaLejos = true;
                    }
                }
            }

        }
        else
        {
            Tiempo += Time.deltaTime;
            if (Tiempo >= TiempoRespawnAleatorio)
            {
                Tiempo = 0;

                if (OcupaPadre && Padre.GetComponent<MeshRenderer>().enabled)
                {
                    TieneObjeto = true;
                    try
                    {
                        GetComponent<Collider>().enabled = true;
                        GetComponent<MeshRenderer>().enabled = true;
                    }
                    catch
                    {
                        foreach (Transform Hijo in transform.GetComponentInChildren<Transform>())
                        {
                            if (Hijo.GetComponent<Collider>())
                            {
                                Hijo.GetComponent<Collider>().enabled = true;
                            }
                            if (Hijo.GetComponent<SkinnedMeshRenderer>())
                            {
                                Hijo.GetComponent<SkinnedMeshRenderer>().enabled = true;
                            }
                        }
                    }
                }

            }
            else
            {
                try
                {
                    GetComponent<Collider>().enabled = false;
                    GetComponent<MeshRenderer>().enabled = false;
                }
                catch
                {
                    foreach (Transform Hijo in transform.GetComponentInChildren<Transform>())
                    {
                        if (Hijo.GetComponent<Collider>())
                        {
                            Hijo.GetComponent<Collider>().enabled = false;
                        }
                        if (Hijo.GetComponent<SkinnedMeshRenderer>())
                        {
                            Hijo.GetComponent<SkinnedMeshRenderer>().enabled = false;
                        }
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

        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = false;
        // Verificar si la habilidad está activa o no
        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" && !string.IsNullOrEmpty(Habilidad))
        {
            animSpeed = 2f; // Doble de velocidad si la habilidad está activa
        }
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
        {
            cantidad = cantidad * 2;
        }
        ActualizarInventario(cantidad, objetoQueDa);
    }

    void ActualizarInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        if (cantidad <= 0 || objeto == null)
            return;

        Scr_Inventario inventario = gata.GetChild(7).GetComponent<Scr_Inventario>();

        // INVENTARIO ES LA ÚNICA FUENTE DE VERDAD
        inventario.AgregarObjeto(
            objeto.Nombre,
            cantidad,
            mostrarUI: true,
            darXP: true
        );
    }



    void ActivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = true;
        gata.GetChild(3).gameObject.SetActive(true);
        gata.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = tecla;
        gata.GetChild(3).GetChild(0).GetComponent<Image>().sprite = teclaIcono;
        gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = icono;
        GameObject ui = gata.GetChild(3).GetChild(2).gameObject;
        GameObject ui2 = gata.GetChild(3).GetChild(3).gameObject;

        if (!ui.activeSelf)
        {
            ui.SetActive(false);
        }
        if (!ui2.activeSelf)
        {
            ui2.SetActive(false);
        }


        gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
        gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);
    }
    void DesactivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
        gata.GetChild(3).gameObject.SetActive(false);
        gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
        gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);
        gata.GetChild(3).GetChild(2).gameObject.SetActive(false);
        gata.GetChild(3).GetChild(3).gameObject.SetActive(false);
    }
}
