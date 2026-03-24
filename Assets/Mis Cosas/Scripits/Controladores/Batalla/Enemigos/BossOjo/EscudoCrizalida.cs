using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscudoCrizalida : MonoBehaviour
{
    [SerializeField] float Distancia;
    [SerializeField] GameObject Escudo;
    GameObject gata;
    // Start is called before the first frame update
    void Start()
    {
        gata = GameObject.Find("Personaje");
    }

    // Update is called once per frame
    void Update()
    {
        float distanciaGata = Vector3.Distance(gata.transform.position, transform.position);
        if (distanciaGata < Distancia)
        {
            Escudo.SetActive(false);
        }
        else
        {
            Escudo.SetActive(true);
        }
    }
}
