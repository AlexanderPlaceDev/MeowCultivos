using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_AnimadorCinematicasPersonajes : MonoBehaviour
{
    public bool esSuave = false; // Controla si usamos transici�n suave
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Animar(string NombreAnimacion)
    {
        if (!esSuave)
        {
            // M�todo normal: reproducir directamente
            animator.Play(NombreAnimacion);
        }
        else
        {
            // M�todo suave: activar bools en el Animator
            DesactivarTodosLosBools();

            // Activar solo el bool correspondiente al nombre recibido
            animator.SetBool(NombreAnimacion, true);
        }
    }

    private void DesactivarTodosLosBools()
    {
        // Recorremos todos los par�metros del Animator
        foreach (AnimatorControllerParameter parametro in animator.parameters)
        {
            if (parametro.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parametro.name, false);
            }
        }
    }
}
