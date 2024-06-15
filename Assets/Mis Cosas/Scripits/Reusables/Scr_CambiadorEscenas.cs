using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_CambiadorEscenas : MonoBehaviour
{
    public AsyncOperation Operacion;

    public void CambiarEscena()
    {
        if (Operacion != null && Operacion.progress >= 0.9f)
        {
            Operacion.allowSceneActivation = true;
        }
        else
        {
            Debug.LogWarning("La escena no está completamente cargada.");
        }
    }
}
