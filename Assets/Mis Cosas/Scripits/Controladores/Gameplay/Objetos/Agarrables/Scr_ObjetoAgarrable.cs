using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ObjetoAgarrable : MonoBehaviour
{
    [Header("Datos del objeto")]
    public string Nombre;
    public int Cantidad;
    public bool[] Forma;
    public Sprite[] Iconos;

    [Header("Datos Generales")]
    [SerializeField] Sprite Icono;
    [SerializeField] Sprite Tecla;
    [SerializeField] float Distancia;
    [SerializeField] float VelocidadGiro;
    [SerializeField] float AlturaPanel;
    bool EstaLejos;
    bool Recolectando;
    Transform Gata;
    Scr_ObjetoEnMano ObjetoEnMano;

    void Start()
    {
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        ObjetoEnMano = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(1).GetComponent<Scr_ObjetoEnMano>();
    }

    void Update()
    {
        if (!Recolectando)
        {
            //Si se Acerca se prenden los iconos
            if (Vector3.Distance(Gata.position, transform.position) < Distancia)
            {
                EstaLejos = false;
                transform.GetChild(1).gameObject.SetActive(true);
                Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = true;
                Gata.GetChild(4).GetChild(0).GetComponent<SpriteRenderer>().sprite = Tecla;
                Gata.GetChild(4).GetChild(1).GetComponent<SpriteRenderer>().sprite = Icono;
                Gata.GetChild(4).gameObject.SetActive(true);

                //Si esta recolectando se gira y espera a que termine
                if (Gata.GetChild(0).GetComponent<Animator>().GetBool("Recolectar"))
                {
                    Gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = true;
                    Recolectando = true;

                    Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
                    Gata.GetChild(4).gameObject.SetActive(false);
                    StartCoroutine(Esperar());
                }

            }
            else
            {
                if (!EstaLejos)
                {
                    Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
                    transform.GetChild(1).gameObject.SetActive(false);
                    Gata.GetChild(4).gameObject.SetActive(false);
                    EstaLejos = true;
                }

            }
        }

        if (Recolectando)
        {
            Quaternion Objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, Gata.position.y, transform.position.z) - Gata.position);
            Gata.rotation = Quaternion.RotateTowards(Gata.rotation, Objetivo, VelocidadGiro * Time.deltaTime);
        }
    }

    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(5.22f);
        Recolectando = false;
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = false;
        DarItem();
        Destroy(gameObject);
    }

    void DarItem()
    {
        if (ObjetoEnMano.Nombre == Nombre || ObjetoEnMano.Nombre == "")
        {
            ObjetoEnMano.Cantidad += Cantidad;
            ObjetoEnMano.Nombre = Nombre;
            ObjetoEnMano.Forma = Forma;
            ObjetoEnMano.Iconos = Iconos;
        }
    }
}


