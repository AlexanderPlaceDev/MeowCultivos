using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_CinematicaIntroPereodico : MonoBehaviour
{
    [SerializeField] GameObject Iconos;
    [SerializeField] GameObject PereodicoGrande;
    [SerializeField] Scr_ControladorCinematica Cinematica;
    int cont = 0;
    void Start()
    {

    }

    void Update()
    {
        if (Iconos.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (cont == 0)
                {
                    cont = 1;
                    PereodicoGrande.SetActive(true);
                }
                else
                {
                    PereodicoGrande.SetActive(false);
                    Iconos.SetActive(false);
                    Cinematica.PausaAlTerminar[3] = false;
                }
            }
        }
    }
}
