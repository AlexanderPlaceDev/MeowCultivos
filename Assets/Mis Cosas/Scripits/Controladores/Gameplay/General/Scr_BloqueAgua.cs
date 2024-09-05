using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    void Start()
    {
        gata = GameObject.Find("Gata").GetComponent<Transform>();
        Herramienta = gata.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
    }

    void Update()
    {

        if (!recolectando)
        {
            // Si se acerca, se encienden los iconos
            if (EstaDentro && PlayerPrefs.GetString("Habilidad:" + Habilidad, "No") == "Si" && !string.IsNullOrEmpty(Habilidad))
            {
                estaLejos = false;
                ActivarUI();
                SpawnearHerramienta();
                if (gata.GetChild(0).GetComponent<Animator>().GetBool("Regar"))
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
        gata.GetChild(0).GetComponent<Animator>().speed = 1;

        recolectando = false;
        gata.GetComponent<Scr_ControladorAnimacionesGata>().Regando = false;

        DarObjeto();
    }

    void DarObjeto()
    {
        PlayerPrefs.SetInt("CantidadAgua", PlayerPrefs.GetInt("CantidadAguaMaxima", 10));
    }


    void ActivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRegar = true;
        gata.GetChild(2).gameObject.SetActive(true);
        gata.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = tecla;
        gata.GetChild(2).GetChild(0).GetComponent<Image>().sprite = teclaIcono;
        gata.GetChild(2).GetChild(1).GetComponent<Image>().sprite = icono;
    }

    void DesactivarUI()
    {
        gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRegar = false;
        gata.GetChild(2).gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
     
        if(other.tag == "Gata")
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
            EstaDentro = false;
        }
    }

    void SpawnearHerramienta()
    {
        Herramienta.SetActive(true);
        Herramienta.transform.GetChild(2).gameObject.SetActive(true);
    }
}
