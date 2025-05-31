using System.Collections;
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
        Anim = transform.GetComponent<Animator>();
        Mov = GetComponent<Scr_Movimiento>();
    }

    void Update()
    {
        Inputs();

        // Actualizar parámetros del Animator
        Anim.SetBool("Talando", Talando);
        Anim.SetBool("Recolectando", Recolectando);
        Anim.SetBool("Regando", Regando);

        if (!Talando && !Recolectando && !Regando)
        {
            Anim.SetBool("Caminando", Mov.Estado == Scr_Movimiento.Estados.Caminar);
            Anim.SetBool("Corriendo", Mov.Estado == Scr_Movimiento.Estados.Correr);
            Anim.SetBool("Retrocediendo", Mov.Estado == Scr_Movimiento.Estados.Retroceder);
        }
        else
        {
            // Detener movimiento si se está en alguna acción
            Anim.SetBool("Caminando", false);
            Anim.SetBool("Caminando", false);
            Anim.SetBool("Retrocediendo", false);
        }

        // Habilitar o deshabilitar movimiento según estado
        
        GetComponent<Scr_Movimiento>().enabled = PuedeCaminar;
        GetComponent<Scr_GiroGata>().enabled = PuedeCaminar;
    }

    void Inputs()
    {
        if (!Talando && !Recolectando && !Regando)
        {
            if (PuedeTalar && Input.GetKeyDown(Talar))
            {
                Talando = true;
                StartCoroutine(EsperarTalar());
            }

            if (PuedeRecolectar && Input.GetKeyDown(Recolectar))
            {
                Recolectando = true;
                StartCoroutine(EsperarRecolectar());
            }

            if (PuedeRegar && Input.GetKeyDown(Regar))
            {
                Regando = true;
                StartCoroutine(EsperarRegar());
            }
        }
    }

    IEnumerator EsperarTalar()
    {
        yield return new WaitForSeconds(TiempoTalar);
        Talando = false;
    }

    IEnumerator EsperarRecolectar()
    {
        int tiempo = PlayerPrefs.GetString("Habilidad:Guante", "No") == "Si" ? 2 : 1;
        yield return new WaitForSeconds(TiempoRecoleccion / tiempo);
        Debug.Log("Terminó de recolectar");
        Recolectando = false;
    }

    IEnumerator EsperarRegar()
    {
        int tiempo = PlayerPrefs.GetString("Habilidad:Guante", "No") == "Si" ? 2 : 1;
        yield return new WaitForSeconds(TiempoRegar / tiempo);
        Debug.Log("Terminó de regar");
        Regando = false;
    }
}
