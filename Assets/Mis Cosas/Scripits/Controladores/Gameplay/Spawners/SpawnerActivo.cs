using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpawnerActivo : MonoBehaviour
{
    public bool UsaPico;
    public bool UsaHacha;
    [SerializeField] Sprite Icono;
    [SerializeField] Sprite Tecla;
    [SerializeField] public int Vida;
    [SerializeField] public GameObject Objeto;
    [SerializeField] float Distancia;
    [SerializeField, Range(0, 10)] float DistanciaSpawneo;
    [SerializeField] float AlturaSpawneo;

    GameObject Herramienta;
    Transform Gata;
    bool EstaLejos;
    bool Destruyendo = false;


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
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = true;
            EstaLejos = false;
            Gata.GetChild(4).GetChild(0).GetComponent<SpriteRenderer>().sprite = Tecla;
            Gata.GetChild(4).GetChild(1).GetComponent<SpriteRenderer>().sprite = Icono;
            Gata.GetChild(4).gameObject.SetActive(true);
        }
        else
        {
            if (!EstaLejos)
            {
                Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = false;
                Herramienta.SetActive(false);
                Gata.GetChild(4).gameObject.SetActive(false);
                EstaLejos = true;
            }

        }

        if (Vida <= 0 && !Destruyendo)
        {
            GetComponent<MeshRenderer>().enabled = false;
            Herramienta.SetActive(false);
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = false;
            StartCoroutine(SpawnearObjetos());
        }
    }

    IEnumerator SpawnearObjetos()
    {
        Destruyendo = true;

        int r = Random.Range(1, 4);

        Herramienta.SetActive(false);
        Gata.GetChild(4).gameObject.SetActive(false);
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar=false;
        for (int i = 0; i < r; i++)
        {
            GameObject Item = Instantiate(Objeto, transform.position, Quaternion.identity, null);
            Item.transform.position = Item.transform.position + new Vector3(Random.Range(0f, DistanciaSpawneo), transform.position.y + AlturaSpawneo, Random.Range(0f, DistanciaSpawneo));
            Item.transform.GetChild(0).rotation = Quaternion.Euler(Random.Range(0f, 359f), Random.Range(0f, 359f), Random.Range(0f, 359f));
        }

        if (transform.childCount > 0)
        {
            transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            float Tiempo = transform.GetChild(0).GetComponent<ParticleSystem>().main.duration;
            yield return new WaitForSeconds(Tiempo);
            Destroy(gameObject);
        }
        else
        {
            yield return new WaitForSeconds(0);
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