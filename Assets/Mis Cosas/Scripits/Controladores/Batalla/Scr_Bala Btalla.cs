using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_BalaBtalla : MonoBehaviour
{

    public float Daño;
    GameObject Controlador;

    private void OnEnable()
    {
        Controlador = GameObject.Find("Controlador");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gata")
        {
            Tween.ShakeCamera(Camera.main, 3);
            Scr_ControladorBatalla batalla = Controlador.GetComponent<Scr_ControladorBatalla>();

            if (batalla.VidaActual >= Daño)
            {
                batalla.VidaActual -= Daño;
            }
            else
            {
                batalla.VidaActual = 0; // 🔹 Evita valores negativos
            }
        }
        Destroy(gameObject);
    }
}
