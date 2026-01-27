using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ActivadorCinematica : MonoBehaviour
{
    [SerializeField] GameObject Elementos;
    [SerializeField] string Cinematica;
    [SerializeField] string CinematicaAnterior;
    private bool EstaEnCinematica;
    private GameObject Gata;
    private Scr_ControladorTiempo Tiempo;

    private void Awake()
    {
        Tiempo = FindObjectOfType<Scr_ControladorTiempo>();

        if (Tiempo == null)
            Debug.LogError("No se encontró Scr_ControladorTiempo");

        // Si la cinemática ya fue reproducida, desactivar el activador completo
        if (PlayerPrefs.GetString("Cinematica " + Cinematica, "No") == "Si")
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }

        if (PlayerPrefs.GetString("Cinematica " + CinematicaAnterior, "No") == "Si")
        {
            gameObject.GetComponent<Collider>().enabled = true;
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (EstaEnCinematica) return;

        if (other.CompareTag("Gata") && PlayerPrefs.GetString("Cinematica " + Cinematica, "No") == "No")
        {
            Gata = other.gameObject;
            Elementos.SetActive(true);
            EstaEnCinematica = true;

            if (Gata.TryGetComponent(out Scr_ControladorAnimacionesGata animControl))
            {
                animControl.EstaEnCinematica = true;
                animControl.PuedeCaminar = false;
            }

            //Desactivar camara principal
            GameObject.Find("Cosas Inutiles").transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (EstaEnCinematica && Gata != null)
        {
            var animControl = Gata.GetComponent<Scr_ControladorAnimacionesGata>();
            if (animControl != null)
            {
                animControl.PuedeCaminar = false;
            }
        }
    }

    /// <summary>
    /// Método que se debe llamar desde un SIGNAL para finalizar la cinemática.
    /// </summary>
    public void FinalizarCinematicaDesdeSignal()
    {
        if (Gata == null) return;

        // ✅ Guardar la cinemática como completada
        PlayerPrefs.SetString("Cinematica " + Cinematica, "Si");

        PlayerPrefs.SetString("DiaCinematica:" + gameObject.transform.parent.parent.name, Tiempo.DiaActual);
        PlayerPrefs.SetInt("HoraCinematica:" + gameObject.transform.parent.parent.name, Tiempo.HoraActual);
        Debug.Log("Guarda desde: " + gameObject.name);

        Scr_ActivadorElementos ActivadorPadre = transform.parent.GetComponent<Scr_ActivadorElementos>();

        if (ActivadorPadre != null)
        {
            if (string.IsNullOrEmpty(ActivadorPadre.CinematicaSiguiente) && ActivadorPadre.UsaEventoGeneral)
            {
                Debug.Log("Desactiva evento");
                GameObject.Find("EventosGenerales").GetComponent<Controlador_EventosGenerales>().DesactivarEvento(transform.parent.GetComponent<Scr_ActivadorElementos>().NombreEventoGeneral);
            }
        }

        PlayerPrefs.Save();


        // ✅ Reactivar movimiento y cámara
        if (Gata.TryGetComponent(out Scr_ControladorAnimacionesGata animControl))
        {
            animControl.EstaEnCinematica = false;
            animControl.PuedeCaminar = true;
        }

        var movimiento = Gata.GetComponent<Scr_Movimiento>();
        if (movimiento != null)
        {
            movimiento.enabled = true;
        }


        Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = 1;

        Debug.Log("ActivaPrincipal");
        //Activar camara principal
        GameObject.Find("Cosas Inutiles").transform.GetChild(2).gameObject.SetActive(true);

        EstaEnCinematica = false;
        // ✅ Desactivar elementos y activador
        gameObject.GetComponent<Collider>().enabled = false; // Desactiva el activador para no volver a activarlo

    }
}
