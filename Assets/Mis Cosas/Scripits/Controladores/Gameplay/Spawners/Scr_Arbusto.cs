using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_Arbusto : MonoBehaviour
{

    [SerializeField] Sprite Icono;
    [SerializeField] string Tecla;
    [SerializeField] Sprite TeclaIcono;
    [SerializeField] Material[] Tipos;
    int TipoActual = 0;
    [SerializeField] float Distancia;
    [SerializeField] float VelocidadGiro;
    [SerializeField] float Duracion;
    [SerializeField] Scr_CreadorObjetos[] ObjetosQueDa;
    [SerializeField] int[] MinimoMaximo;
    bool Recolectando;
    bool TieneMoras;
    bool EstaLejos;

    Transform Gata;

    void Start()
    {
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        TipoActual = Random.Range(0, 4);
        GetComponent<MeshRenderer>().material = Tipos[TipoActual];
        if (TipoActual > 0)
        {
            TieneMoras = true;
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
                Gata.GetChild(2).gameObject.SetActive(true);
                Gata.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Tecla;
                Gata.GetChild(2).GetChild(0).GetComponent<Image>().sprite = TeclaIcono;
                Gata.GetChild(2).GetChild(1).GetComponent<Image>().sprite = Icono;

                //Si esta recolectando se gira y espera a que termine
                if (Gata.GetChild(0).GetComponent<Animator>().GetBool("Recolectar"))
                {
                    Gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = true;
                    Recolectando = true;

                    Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
                    StartCoroutine(Esperar());
                }
            }
            else
            {
                if (!EstaLejos)
                {
                    Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeRecolectar = false;
                    Gata.GetChild(2).gameObject.SetActive(false);
                    EstaLejos = true;
                }

            }
        }

        if (Recolectando)
        {
            Gata.GetChild(2).gameObject.SetActive(false);
            Quaternion Objetivo = Quaternion.LookRotation(new Vector3(transform.position.x, Gata.position.y, transform.position.z) - Gata.position);
            Gata.rotation = Quaternion.RotateTowards(Gata.rotation, Objetivo, VelocidadGiro * Time.deltaTime);
        }
    }

    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(5.22f);
        Recolectando = false;
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().Recolectando = false;
        if (TieneMoras)
        {
            DarMoras();
            DarFibra();
            TieneMoras = false;

        }
        TipoActual = 0;
        GetComponent<MeshRenderer>().material = Tipos[TipoActual];

    }

    void DarMoras()
    {
        int cantidad = Random.Range(MinimoMaximo[0], MinimoMaximo[1]);
        Gata.GetChild(6).GetComponent<Scr_Inventario>().Cantidades[ObjetosQueDa[TipoActual].ID] += cantidad;
        Scr_ObjetosAgregados Controlador = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<Scr_ObjetosAgregados>();
        if (Controlador.Lista.ToArray().Length == 0)
        {
            Controlador.Lista.Add(ObjetosQueDa[TipoActual]);
            Controlador.Cantidades.Add(cantidad);
        }
        else
        {
            if (Controlador.Lista.Contains(ObjetosQueDa[TipoActual]))
            {
                for (int i = 0; i < Controlador.Lista.Count; i++)
                {
                    if (Controlador.Lista[i] == ObjetosQueDa[0])
                    {
                        Controlador.Cantidades[i] += cantidad;
                        if (i <= 3)
                        {
                            Controlador.Tiempo[i] = 2;
                        }
                    }
                }
            }
            else
            {
                Controlador.Lista.Add(ObjetosQueDa[TipoActual]);
                Controlador.Cantidades.Add(cantidad);
            }
        }
    }

    void DarFibra()
    {
        int cantidad = Random.Range(MinimoMaximo[0], MinimoMaximo[1]);
        Gata.GetChild(6).GetComponent<Scr_Inventario>().Cantidades[ObjetosQueDa[0].ID] += cantidad;
        Scr_ObjetosAgregados Controlador = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<Scr_ObjetosAgregados>();
        if (Controlador.Lista.ToArray().Length == 0)
        {
            Controlador.Lista.Add(ObjetosQueDa[0]);
            Controlador.Cantidades.Add(cantidad);
        }
        else
        {
            if (Controlador.Lista.Contains(ObjetosQueDa[0]))
            {
                for (int i = 0; i < Controlador.Lista.Count; i++)
                {
                    if (Controlador.Lista[i] == ObjetosQueDa[0])
                    {
                        Controlador.Cantidades[i] += cantidad;
                        if (i <= 3)
                        {
                            Controlador.Tiempo[i] = 2;
                        }
                    }
                }
            }
            else
            {
                Controlador.Lista.Add(ObjetosQueDa[0]);
                Controlador.Cantidades.Add(cantidad);
            }
        }
    }

}
