using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruta_drop : MonoBehaviour
{
    public enum EstadoFruta
    {
        Normal,
        Pudriendose,
        Podrido
    }
    public string Nombre;
    [Header("Tiempos")]
    public float vidaTotal = 20f;
    public float tiempoPudrirse = 14f;
    public float tiempoPodrido = 17f;

    [Header("Colores")]
    public Color colorNormal = Color.white;
    public Color colorPudriendose = Color.yellow;
    public Color colorPodrido = Color.green;

    public float tiempoActual = 0f;
    public EstadoFruta estadoActual;
    private Renderer rendererFruta;

    

    void Start()
    {
        rendererFruta = GetComponent<Renderer>();
        //CambiarEstado(EstadoFruta.Normal);
    }

    void Update()
    {
        // El tiempo SIEMPRE avanza
        tiempoActual += Time.deltaTime;

        // Cambios de estado
        if (tiempoActual >= tiempoPodrido && estadoActual != EstadoFruta.Podrido)
            CambiarEstado(EstadoFruta.Podrido);
        else if (tiempoActual >= tiempoPudrirse && estadoActual != EstadoFruta.Pudriendose)
            CambiarEstado(EstadoFruta.Pudriendose);

        // Muerte
        if (tiempoActual >= vidaTotal)
            DestruirFruta();
    }

    void CambiarEstado(EstadoFruta nuevoEstado)
    {
        estadoActual = nuevoEstado;

        switch (estadoActual)
        {
            case EstadoFruta.Normal:
                CambiarColor(colorNormal);
                break;

            case EstadoFruta.Pudriendose:
                CambiarColor(colorPudriendose);
                break;

            case EstadoFruta.Podrido:
                CambiarColor(colorPodrido);
                break;
        }
    }

    void CambiarColor(Color color)
    {
        if (rendererFruta == null) return;

        foreach (Material mat in rendererFruta.materials)
        {
            Material[] materiales = rendererFruta.materials;
            // Guardar una copia de los materiales originales
            Material[] materialesOriginales = new Material[materiales.Length];
            for (int i = 0; i < materiales.Length; i++)
            {
                materialesOriginales[i] = new Material(materiales[i]);
            }

            // Crear copias modificadas de los materiales y cambiar el _BaseColor
            Material[] materialesModificados = new Material[materiales.Length];
            for (int i = 0; i < materiales.Length; i++)
            {
                materialesModificados[i] = new Material(materiales[i]);
                materialesModificados[i].SetColor("_Base_Color", color); // Cambiar el color
            }

            // Aplicar materiales modificados
            rendererFruta.materials = materialesModificados;
        }
    }

    void DestruirFruta()
    {
        // Si está en la mano, se destruye igual
        Destroy(gameObject);
    }

    public EstadoFruta GetEstado()
    {
        return estadoActual;
    }
}
