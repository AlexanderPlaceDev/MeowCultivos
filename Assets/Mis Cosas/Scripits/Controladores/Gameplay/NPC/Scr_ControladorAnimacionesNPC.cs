using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorAnimacionesNPC : MonoBehaviour
{

    Scr_SistemaDialogos Dialogos;
    Scr_ActivadorDialogos ActivadorDialogos;
    public bool Hablando;
    public bool Caminando;
    Animator Anim;

    void Start()
    {
        try
        {
            Dialogos = GetComponent<Scr_SistemaDialogos>();
            ActivadorDialogos = GetComponent<Scr_ActivadorDialogos>();
        }
        catch
        {
            Debug.Log(gameObject.name + " No Tiene Dialogos")
            ;
        }
        Anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (ActivadorDialogos != null)
        {
            if (ActivadorDialogos.ViendoMisiones)
            {
                Hablando = true;
            }
        }
        if (Dialogos.Leyendo)
        {
            Hablando = true;
        }
        else
        {
            Hablando = false;
        }



        if (Hablando)
        {
            Anim.SetBool("Hablando", true);
        }
        else
        {
            if (!GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).gameObject.activeSelf)
            {
                Anim.SetBool("Hablando", false);
            }
        }

        if (Caminando)
        {
            Anim.SetBool("Caminando", true);
        }
        else
        {
            Anim.SetBool("Caminando", false);
        }

    }
}
