using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class SpawnerActivo : MonoBehaviour
{
    public bool UsaPico;
    public bool UsaHacha;
    [SerializeField] Sprite Icono;
    [SerializeField] string Tecla;
    [SerializeField] Sprite TeclaIcono;
    [SerializeField] public int Vida;
    [SerializeField] float Distancia;
    [SerializeField] Scr_CreadorObjetos ObjetoQueDa;
    [SerializeField] int[] MinimoMaximo;


    GameObject Herramienta;
    Transform Gata;
    bool EstaLejos;


    void Start()
    {
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        Herramienta = Gata.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
    }

    void Update()
    {

        if (Vector3.Distance(Gata.position, transform.position) < Distancia && GetComponent<MeshRenderer>().enabled)
        {
            SpawnearHerramienta();
            Gata.GetChild(2).gameObject.SetActive(true);
            if (Tecla != "")
            {
                Gata.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Tecla;
            }
            else
            {
                Gata.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                Gata.GetChild(2).GetChild(0).GetComponent<Image>().sprite = TeclaIcono;
            }
            Gata.GetChild(2).GetChild(1).GetComponent<Image>().sprite = Icono;
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = true;
            EstaLejos = false;
        }
        else
        {
            if (!EstaLejos)
            {
                Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = false;
                Herramienta.SetActive(false);
                Gata.GetChild(2).gameObject.SetActive(false);
                EstaLejos = true;
            }

        }

        if (Vida <= 0)
        {
            Herramienta.SetActive(false);
            Gata.GetChild(2).gameObject.SetActive(false);
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = false;
            int cantidad = Random.Range(MinimoMaximo[0], MinimoMaximo[1]);
            Gata.GetChild(6).GetComponent<Scr_Inventario>().Cantidades[ObjetoQueDa.ID] += cantidad;
            Scr_ObjetosAgregados Controlador = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<Scr_ObjetosAgregados>();
            if (Controlador.Lista.ToArray().Length == 0)
            {
                Controlador.Lista.Add(ObjetoQueDa);
                Controlador.Cantidades.Add(cantidad);
            }
            else
            {
                if (Controlador.Lista.Contains(ObjetoQueDa))
                {
                    for (int i = 0; i < Controlador.Lista.Count; i++)
                    {
                        if (Controlador.Lista[i] == ObjetoQueDa)
                        {
                            Controlador.Cantidades[i] += cantidad;
                            if (i <= 3)
                            {
                                Controlador.Tiempo[i] = 2;
                            }
                        }
                    }
                }
            }
            Destroy(gameObject);
        }
    }


    void SpawnearHerramienta()
    {
        Herramienta.SetActive(true);
        if (UsaHacha)
        {
            Herramienta.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            Herramienta.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (UsaPico)
        {
            Herramienta.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            Herramienta.transform.GetChild(1).gameObject.SetActive(false);
        }

    }
}
