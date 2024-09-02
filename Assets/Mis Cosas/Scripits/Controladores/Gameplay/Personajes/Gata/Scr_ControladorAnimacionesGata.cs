using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorAnimacionesGata : MonoBehaviour
{
    public Animator Anim;
    public Scr_Movimiento Mov;

    public KeyCode Talar = KeyCode.Mouse0;
    public KeyCode Recolectar = KeyCode.F;
    public bool PuedeCaminar;
    public bool PuedeTalar;
    public bool PuedeRecolectar;
    public float TiempoRecoleccion;
    public float TiempoTalar;

    bool Talando;
    public bool Recolectando;

    void Start()
    {
        Anim = gameObject.transform.GetChild(0).GetComponent<Animator>();
        Mov = GetComponent<Scr_Movimiento>();
    }

    void Update()
    {
        Inputs();


        if (Talando)
        {
            
                Anim.SetBool("Talar", true);
            PuedeCaminar = false;
            
        }
        else
        {
            Anim.SetBool("Talar", false);
            if (Recolectando)
            {
                Anim.SetBool("Recolectar", true);
                PuedeCaminar = false;
            }
            else
            {
                Anim.SetBool("Recolectar", false);
                PuedeCaminar= true;

                if (Mov.Estado == Scr_Movimiento.Estados.Caminar)
                {
                    Anim.SetBool("Caminar", true);
                }
                else
                {
                    Anim.SetBool("Caminar", false);
                }
                if (Mov.Estado == Scr_Movimiento.Estados.Correr)
                {
                    Anim.SetBool("Correr", true);
                }
                else
                {
                    Anim.SetBool("Correr", false);
                }
            }
        }

        if (PuedeCaminar)
        {
            GetComponent<Scr_Movimiento>().enabled = true;
            GetComponent<Scr_GiroGata>().enabled = true;
        }
        else
        {
            GetComponent<Scr_Movimiento>().enabled = false;
            GetComponent<Scr_GiroGata>().enabled = false;
        }

    }


    public void Inputs()
    {
        if(PuedeTalar && Time.timeScale==1)
        {
            if (Input.GetKeyDown(Talar) && !Recolectando)
            {
                Talando = true;
                StartCoroutine(EsperarTalar());
            }
        }
        
        if(PuedeRecolectar && Time.timeScale == 1)
        {
            if (Input.GetKeyDown(Recolectar) && !Talando)
            {
                Recolectando = true;
                StartCoroutine(EsperarRecolectar());
            }
        }
        
    }

    public IEnumerator EsperarRecolectar()
    {
        yield return new WaitForSeconds(TiempoRecoleccion);
        PuedeCaminar = true;
        Recolectando =false;
    }
    public IEnumerator EsperarTalar()
    {
        yield return new WaitForSeconds(TiempoTalar);
        PuedeCaminar = true;
        Talando =false;
    }
}
