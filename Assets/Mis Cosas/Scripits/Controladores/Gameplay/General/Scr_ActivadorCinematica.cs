using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ActivadorCinematica : MonoBehaviour
{
    [SerializeField] GameObject Elementos;
    [SerializeField] string Cinematica;
    private bool EstaEnCinematica;
    private GameObject Gata;

    private void Awake()
    {
        // Si la cinemática ya fue reproducida, desactivar el activador completo
        if (PlayerPrefs.GetString("Cinematica " + Cinematica, "No") == "Si")
        {
            gameObject.SetActive(false);
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
            GameObject.Find("Cosas Inutiles").transform.GetChild(3).gameObject.SetActive(false);
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
        GameObject.Find("Cosas Inutiles").transform.GetChild(3).gameObject.SetActive(true);

        EstaEnCinematica = false;
        // ✅ Desactivar elementos y activador
        gameObject.SetActive(false); // Desactiva el activador para no volver a activarlo

    }
}
