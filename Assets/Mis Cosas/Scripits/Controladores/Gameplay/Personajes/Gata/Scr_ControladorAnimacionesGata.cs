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

    public bool EstaEnCinematica = false;


    public string HabilidadUsando;
    void Start()
    {
        Anim = GetComponent<Animator>();
        Mov = GetComponent<Scr_Movimiento>();
    }

    bool _estadoCinematicaAnterior = false;

    void Update()
    {
        if (EstaEnCinematica)
        {
            // Solo ejecutar esto UNA VEZ al entrar a la cinemática
            if (!_estadoCinematicaAnterior)
            {
                DetenerGata();

                _estadoCinematicaAnterior = true; // Marcar que ya hicimos esto
            }

            return; // No ejecutar el resto de Update mientras sea cinemática
        }

        // ← Si salimos de cinemática, reiniciamos la bandera
        if (_estadoCinematicaAnterior)
        {
            _estadoCinematicaAnterior = false;
        }

        // Lo demás de tu Update...
        Inputs();

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
            Anim.SetBool("Caminando", false);
            Anim.SetBool("Corriendo", false);
            Anim.SetBool("Retrocediendo", false);
        }

        GetComponent<Scr_Movimiento>().enabled = PuedeCaminar;
        GetComponent<Scr_GiroGata>().enabled = PuedeCaminar;
    }

    public void DetenerGata()
    {
        Talando = false;
        Recolectando = false;
        Regando = false;

        Anim.SetBool("Talando", false);
        Anim.SetBool("Recolectando", false);
        Anim.SetBool("Regando", false);
        Anim.SetBool("Caminando", false);
        Anim.SetBool("Corriendo", false);
        Anim.SetBool("Retrocediendo", false);

        Mov.Estado = Scr_Movimiento.Estados.Quieto;
        PuedeCaminar = false;

        GetComponent<Scr_Movimiento>().enabled = false;
        GetComponent<Scr_GiroGata>().enabled = false;
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
        Recolectando = false;
    }

    IEnumerator EsperarRegar()
    {
        int tiempo = PlayerPrefs.GetString("Habilidad:Guante", "No") == "Si" ? 2 : 1;
        yield return new WaitForSeconds(TiempoRegar / tiempo);
        Regando = false;
    }

    // 🎬 Llamada desde señal para reproducir animación por nombre
    public void ReproducirAnimacionDesdeCinematica(string nombreAnimacion)
    {
        if (!EstaEnCinematica || string.IsNullOrEmpty(nombreAnimacion)) return;

        Anim.Play(nombreAnimacion, 0, 0f);
    }

    // ✅ Activar un bool desde una señal del Timeline
    // ✅ Activar un bool desde una señal del Timeline (y desactivar los demás)
    public void ActivarBoolDesdeCinematica(string nombre)
    {


        if (!EstaEnCinematica) return;

        if (nombre != "")
        {
            // Desactiva todos los bools relevantes
            Anim.SetBool("Talando", false);
            Anim.SetBool("Recolectando", false);
            Anim.SetBool("Regando", false);
            Anim.SetBool("Caminando", false);
            Anim.SetBool("Corriendo", false);
            Anim.SetBool("Retrocediendo", false);

            // Activa solo el que pediste
            Anim.SetBool(nombre, true);

            // También actualiza las variables internas si aplica
            Talando = nombre == "Talando";
            Recolectando = nombre == "Recolectando";
            Regando = nombre == "Regando";
        }
        else
        {
            // Desactiva todos los bools relevantes
            Anim.SetBool("Talando", false);
            Anim.SetBool("Recolectando", false);
            Anim.SetBool("Regando", false);
            Anim.SetBool("Caminando", false);
            Anim.SetBool("Corriendo", false);
            Anim.SetBool("Retrocediendo", false);
        }


    }


    // ❌ Desactivar un bool desde una señal del Timeline
    public void DesactivarBoolDesdeCinematica(string nombre)
    {
        if (!EstaEnCinematica || string.IsNullOrEmpty(nombre)) return;

        Anim.SetBool(nombre, false);
    }
}
