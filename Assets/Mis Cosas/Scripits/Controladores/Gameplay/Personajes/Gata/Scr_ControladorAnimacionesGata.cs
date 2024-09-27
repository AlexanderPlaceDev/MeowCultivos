using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorAnimacionesGata : MonoBehaviour
{
    public Animator Anim;
    public Scr_Movimiento Mov;

    public KeyCode Talar = KeyCode.Mouse0;
    public KeyCode Recolectar = KeyCode.F;
    public KeyCode Regar = KeyCode.F;
    public bool PuedeCaminar;
    public bool PuedeTalar;
    public bool PuedeRecolectar;
    public bool PuedeRegar;
    public float TiempoRecoleccion;
    public float TiempoRegar;
    public float TiempoTalar;

    bool Talando;
    public bool Recolectando;
    public bool Regando;

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



                if (Regando)
                {
                    Anim.SetBool("Regar", true);
                    PuedeCaminar = false;
                }
                else
                {
                    Anim.SetBool("Regar", false);
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


        if (!Talando && !Recolectando && !Regando)
        {
            if (PuedeTalar)
            {
                if (Input.GetKeyDown(Talar) && !Recolectando)
                {
                    Talando = true;
                    StartCoroutine(EsperarTalar());
                }
            }
            if (PuedeRecolectar)
            {
                if (Input.GetKeyDown(Recolectar))
                {
                    Recolectando = true;
                    StartCoroutine(EsperarRecolectar());
                }
            }
            if (PuedeRegar)
            {
                if (Input.GetKeyDown(Regar))
                {
                    Regando = true;
                    StartCoroutine(EsperarRegar());
                }
            }
        }








    }

    public IEnumerator EsperarRecolectar()
    {
        int Tiempo = 1;
        if (PlayerPrefs.GetString("Habilidad:Guante", "No") == "Si")
        {
            Tiempo = 2;
        }
        yield return new WaitForSeconds(TiempoRecoleccion / Tiempo);
        Debug.Log("Entra5");
        PuedeCaminar = true;
        Recolectando = false;
    }

    public IEnumerator EsperarRegar()
    {
        int Tiempo = 1;
        if (PlayerPrefs.GetString("Habilidad:Guante", "No") == "Si")
        {
            Tiempo = 2;
        }
        yield return new WaitForSeconds(TiempoRegar / Tiempo);
        PuedeCaminar = true;
        Regando = false;
    }
    public IEnumerator EsperarTalar()
    {
        yield return new WaitForSeconds(TiempoTalar);
        Debug.Log("Entra4");
        PuedeCaminar = true;
        Talando = false;
    }
}
