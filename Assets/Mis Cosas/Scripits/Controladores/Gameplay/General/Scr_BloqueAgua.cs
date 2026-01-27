using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_BloqueAgua : MonoBehaviour
{
    [Header("Configuración del spawner")]
    [SerializeField] private Sprite icono;
    [SerializeField] private string tecla;
    [SerializeField] private Sprite teclaIcono;
    [SerializeField] private bool EstaDentro;
    [SerializeField] private float velocidadGiro;
    [SerializeField] string Habilidad;

    [Header("Estado del spawner")]
    private bool recolectando;
    private bool estaLejos;
    private GameObject Herramienta;
    private Transform gata;




    PlayerInput playerInput;
    private InputAction Regar;
    InputIconProvider IconProvider;
    private Sprite iconoActualRegar = null;
    private string textoActualRegar = "";
    void Awake()
    {
        gata = GameObject.Find("Gata").GetComponent<Transform>();



        Herramienta = gata.GetChild(0).GetChild(0).GetChild(0).GetChild(1)
                                .GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;

        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Regar = playerInput.actions["Regar"];


    }

    void Update()
    {

        if (!recolectando)
        {
            // Si se acerca, se encienden los iconos
            if (EstaDentro && ((PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" && !string.IsNullOrEmpty(Habilidad)) || PlayerPrefs.GetString("Habilidad:Cubeta", "No") == "Si"))
            {
                estaLejos = false;
                ActivarUI();
                IconProvider.ActualizarIconoUI(Regar, gata.GetChild(3).GetChild(0), ref iconoActualRegar, ref textoActualRegar, true);
                SpawnearHerramienta();
                if (gata.GetComponent<Animator>().GetBool("Regando"))
                {
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().Regando = true;
                    recolectando = true;
                    gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRegar = false;
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

        if (recolectando)
        {
            DesactivarUI();
            Quaternion objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, gata.position.y, transform.position.z) - gata.position);
            gata.rotation = Quaternion.RotateTowards(gata.rotation, objetivo, velocidadGiro * Time.deltaTime);
        }
    }

    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(3f);
        gata.GetComponent<Animator>().speed = 1;

        recolectando = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().Regando = false;

        DarObjeto();
    }

    void DarObjeto()
    {
        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si")
        {
            PlayerPrefs.SetInt("CantidadAgua", 50);
        }
        else
        {
            if (PlayerPrefs.GetString("Habilidad:Cubeta", "No") == "Si")
            {
                PlayerPrefs.SetInt("CantidadAgua", 25);
            }
        }


    }


    void ActivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRegar = true;
        gata.GetChild(3).gameObject.SetActive(true);
        gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite=icono;
    }

    void DesactivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRegar = false;
        gata.GetChild(3).gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Gata" && (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" || PlayerPrefs.GetString("Habilidad:Cubeta", "No") == "Si"))
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
            EstaDentro = false;
        }
    }

    void SpawnearHerramienta()
    {
        Herramienta.SetActive(true);

        if (PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si")
        {
            Herramienta.transform.GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            if (PlayerPrefs.GetString("Habilidad:Cubeta", "No") == "Si")
            {
                Herramienta.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
    }
}
