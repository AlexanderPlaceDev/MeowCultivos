using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_SpawnerFruta : MonoBehaviour
{
    [SerializeField] int MinutosDuracion;
    [SerializeField] float MinutosRestantes = 0;
    [SerializeField] int SegundosDuracion = 0;
    [SerializeField] float SegundosRestantes = 0;
    bool Creando = false;

    void Awake()
    {
        // Recuperar tiempos guardados
        MinutosRestantes = PlayerPrefs.GetInt("MinutosRestantes:" + gameObject.name, MinutosDuracion);
        SegundosRestantes = PlayerPrefs.GetFloat("SegundosRestantes:" + gameObject.name, SegundosDuracion);

        // Restaurar estado de los hijos
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.GetComponent<Scr_SpawnerRecolectable>() != null)
            {
                string key = "MeshRendererState:" + gameObject.name + child.gameObject.name;
                if (PlayerPrefs.HasKey(key))
                {
                    bool isActive = PlayerPrefs.GetInt(key) == 1;
                    child.GetComponent<MeshRenderer>().enabled = isActive;
                    child.GetComponent<Scr_SpawnerRecolectable>().TieneObjeto = isActive;
                    Creando = true;
                }
            }
        }
    }

    void Update()
    {
        if (Creando)
        {
            if (MinutosRestantes <= 0 && SegundosRestantes <= 0)
            {
                RespawnObjeto();
            }
            else
            {
                ActualizarTemporizador();
            }
        }
        else
        {
            VerificarEstadoObjetos();
        }
    }

    void ActualizarTemporizador()
    {
        if (SegundosRestantes > 0)
        {
            SegundosRestantes -= Time.deltaTime;
            PlayerPrefs.SetFloat("SegundosRestantes:" + gameObject.name, SegundosRestantes);
        }
        else
        {
            if (MinutosRestantes > 0)
            {
                MinutosRestantes--;
                SegundosRestantes = 59;
                PlayerPrefs.SetInt("MinutosRestantes:" + gameObject.name, (int)MinutosRestantes);
                PlayerPrefs.SetFloat("SegundosRestantes:" + gameObject.name, SegundosRestantes);
            }
        }
    }

    void RespawnObjeto()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.GetComponent<Scr_SpawnerRecolectable>() != null && !child.GetComponent<MeshRenderer>().enabled)
            {
                child.GetComponent<MeshRenderer>().enabled = true;
                child.GetComponent<Scr_SpawnerRecolectable>().TieneObjeto = true;
                Creando = false;

                // Guardar el estado activado del MeshRenderer
                PlayerPrefs.SetInt("MeshRendererState:" + gameObject.name + child.gameObject.name, 1);
                break;
            }
        }
    }

    void VerificarEstadoObjetos()
    {
        int childCount = transform.childCount;
        int objetosDesactivados = 0;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.GetComponent<MeshRenderer>().enabled == false)
            {
                objetosDesactivados++;
                if (!Creando) // Solo cambiar el estado si aún no está en el modo de creación
                {
                    Creando = true;
                    MinutosRestantes = MinutosDuracion;
                    SegundosRestantes = SegundosDuracion;

                    // Guardar el estado desactivado del MeshRenderer
                    PlayerPrefs.SetInt("MeshRendererState:" + gameObject.name + child.gameObject.name, 0);
                    PlayerPrefs.SetString("CantidadObjetos:" + gameObject.name, objetosDesactivados.ToString());
                }
            }
        }
    }

}
