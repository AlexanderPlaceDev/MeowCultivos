using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ActivadorDialogos : MonoBehaviour
{

    bool EstaAdentro = false;
    [SerializeField] GameObject Panel;
    [SerializeField] GameObject[] Iconos;
    [SerializeField] GameObject Camara;
    [SerializeField] GameObject CamaraGata;

    private void Start()
    {
        CamaraGata = GameObject.Find("Camara 360");
    }

    void Update()
    {
        if (EstaAdentro)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Iconos[0].SetActive(false);
                Iconos[1].SetActive(false);
                Camara.SetActive(true);
                CamaraGata.SetActive(false);
                if (!Panel.gameObject.activeSelf)
                {
                    Panel.SetActive(true);
                    GetComponent<Scr_SistemaDialogos>().EnPausa = false;
                }

                if (!GetComponent<Scr_SistemaDialogos>().EnPausa)
                {
                    if (GetComponent<Scr_SistemaDialogos>().Leyendo)
                    {
                        GetComponent<Scr_SistemaDialogos>().SaltarDialogo();
                    }
                    else
                    {
                        GetComponent<Scr_SistemaDialogos>().SiguienteLetra();
                    }
                }
            }

            if (GetComponent<Scr_SistemaDialogos>().Leido && !GetComponent<Scr_SistemaDialogos>().Leyendo)
            {
                Camara.SetActive(false);
                CamaraGata.SetActive(true);
                Iconos[0].SetActive(true);
                Iconos[1].SetActive(true);
            }
        }
        else
        {
            GetComponent<Scr_SistemaDialogos>().EnPausa = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gata")
        {
            Debug.Log("Entra");
            Iconos[0].SetActive(true);
            Iconos[1].SetActive(true);
            EstaAdentro = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Gata")
        {
            Debug.Log("Sale");
            Iconos[0].SetActive(false);
            Iconos[1].SetActive(false);
            EstaAdentro = false;
        }
    }
}
