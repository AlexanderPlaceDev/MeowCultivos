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
        MinutosRestantes = PlayerPrefs.GetInt("MinutosRestantes:" + gameObject.name, MinutosDuracion);
        SegundosRestantes = PlayerPrefs.GetFloat("SegundosRestantes:" + gameObject.name, SegundosDuracion);

        int frutasFaltantes = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            var recolectable = child.GetComponent<Scr_SpawnerRecolectable>();

            if (recolectable == null)
                continue;

            string key = "Recolectable_Tiene_" + child.gameObject.name;

            bool tieneFruta = true;

            if (PlayerPrefs.HasKey(key))
                tieneFruta = PlayerPrefs.GetInt(key) == 1;

            child.GetComponent<MeshRenderer>().enabled = tieneFruta;
            recolectable.TieneObjeto = tieneFruta;

            if (!tieneFruta)
                frutasFaltantes++;
        }

        Creando = frutasFaltantes > 0;
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
        // Reaparecer solo una fruta
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            var recolectable = child.GetComponent<Scr_SpawnerRecolectable>();

            if (recolectable == null)
                continue;

            if (!child.GetComponent<MeshRenderer>().enabled)
            {
                child.GetComponent<MeshRenderer>().enabled = true;
                recolectable.TieneObjeto = true;

                // Actualizar el guardado del recolectable
                PlayerPrefs.SetInt("Recolectable_Tiene_" + child.gameObject.name, 1);
                PlayerPrefs.DeleteKey("Recolectable_Respawn_" + child.gameObject.name);
                PlayerPrefs.DeleteKey("Recolectable_RespawnObjetivo_" + child.gameObject.name);

                break; // Solo reaparece una fruta
            }
        }

        // Comprobar si todavía quedan frutas desaparecidas
        bool quedanFrutas = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            var recolectable = child.GetComponent<Scr_SpawnerRecolectable>();

            if (recolectable == null)
                continue;

            if (!child.GetComponent<MeshRenderer>().enabled)
            {
                quedanFrutas = true;
                break;
            }
        }

        if (quedanFrutas)
        {
            // Reiniciar el temporizador para la siguiente fruta
            MinutosRestantes = MinutosDuracion;
            SegundosRestantes = SegundosDuracion;

            PlayerPrefs.SetInt("MinutosRestantes:" + gameObject.name, MinutosDuracion);
            PlayerPrefs.SetFloat("SegundosRestantes:" + gameObject.name, SegundosDuracion);

            Creando = true;
        }
        else
        {
            // Ya reaparecieron todas
            Creando = false;

            PlayerPrefs.DeleteKey("MinutosRestantes:" + gameObject.name);
            PlayerPrefs.DeleteKey("SegundosRestantes:" + gameObject.name);
        }

        PlayerPrefs.Save();
    }

    void VerificarEstadoObjetos()
    {
        if (Creando)
            return;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            var recolectable = child.GetComponent<Scr_SpawnerRecolectable>();
            if (recolectable == null)
                continue;

            if (!child.GetComponent<MeshRenderer>().enabled)
            {
                Creando = true;
                MinutosRestantes = MinutosDuracion;
                SegundosRestantes = SegundosDuracion;

                PlayerPrefs.SetInt("MinutosRestantes:" + gameObject.name, MinutosDuracion);
                PlayerPrefs.SetFloat("SegundosRestantes:" + gameObject.name, SegundosDuracion);

                break;
            }
        }
    }

}
