using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

public class Scr_Arbusto : MonoBehaviour
{

    [SerializeField] Sprite Icono;
    [SerializeField] Sprite Tecla;
    [SerializeField] Material[] Tipos;
    [SerializeField] float Distancia;
    [SerializeField] float VelocidadGiro;
    [SerializeField] float Duracion;
    int CantMoras;
    bool Recolectando;
    bool TieneMoras;
    bool EstaLejos;
    string Tipo;

    Transform Gata;

    void Start()
    {
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        int r = Random.Range(0, 4);
        GetComponent<MeshRenderer>().material = Tipos[r];
        if (r > 0)
        {
            TieneMoras = true;
            CantMoras = Random.Range(1, 5);
            Tipo = Tipos[r].name;
        }
    }

    void Update()
    {
        //Verificar si tiene moras y no esta recolectando
        if (TieneMoras && !Recolectando)
        {

            //Si se Acerca se prenden los iconos
            if (Vector3.Distance(Gata.position, transform.position) < Distancia)
            {
                EstaLejos = false;
                Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = true;

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
        TieneMoras = false;
        Recolectando = false;
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = false;
        DarMoras();
        DarFibra();
    }

    void DarMoras()
    {
        
    }

    void DarFibra()
    {
        
    }

}
