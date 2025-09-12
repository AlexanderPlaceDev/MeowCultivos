using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Scr_DatosArmas : MonoBehaviour
{
    public Scr_CreadorArmas[] TodasLasArmas;
    public bool[] ArmasDesbloqueadas;

    public Scr_CreadorHabilidadesBatalla[] HabilidadesTemporales;
    public bool[] HabilidatTDesbloqueadas;
    public int[] UsosHabilidadesT;

    public Scr_CreadorHabilidadesBatalla[] HabilidadesPermanentes;
    public bool[] HabilidatPDesbloqueadas;

    void Start()
    {
        for (int i = 1; i < TodasLasArmas.Length; i++)
        {
            if (PlayerPrefs.GetString("Arma" + TodasLasArmas[i].Nombre, "No") == "Si")
            {
                ArmasDesbloqueadas[i] = true;

            }
            else
            {
                ArmasDesbloqueadas[i] = false;
            }
        }

        for (int i = 0; i < HabilidadesTemporales.Length; i++)
        {
            if (PlayerPrefs.GetString("Habilidad" + HabilidadesTemporales[i].Nombre, "No") == "Si")
            {
                HabilidatTDesbloqueadas[i] = true;

            }
            else
            {
                HabilidatTDesbloqueadas[i] = false;
            }
        }

        for (int i = 0; i < HabilidadesPermanentes.Length; i++)
        {
            if (PlayerPrefs.GetString("Habilidad" + HabilidadesPermanentes[i].Nombre, "No") == "Si")
            {
                HabilidatPDesbloqueadas[i] = true;

            }
            else
            {
                HabilidatPDesbloqueadas[i] = false;
            }
        }
    }

    public void DesbloquearArma(string Nombre)
    {
        PlayerPrefs.SetString("Arma" + Nombre, "Si");

        for (int i = 1; i < TodasLasArmas.Length; i++)
        {
            if (TodasLasArmas[i].Nombre == Nombre)
            {
                ArmasDesbloqueadas[i] = true;
            }
        }
    }
    //encuentra la habilidar por nombre
    public Scr_CreadorHabilidadesBatalla BuscarHabilidadTemporalPorNombre(string nombre)
    {
        foreach (var habilidad in HabilidadesTemporales)
        {
            if (habilidad.Nombre == nombre) // Asegúrate de que sea 'Nombre' o 'nombre' según el campo real
            {
                return habilidad;
            }
        }

        return null; // No se encontró
    }


    //encuentra la habilidar por nombre
    public Scr_CreadorHabilidadesBatalla BuscarHabilidadPermanentePorNombre(string nombre)
    {
        foreach (var habilidad in HabilidadesPermanentes)
        {
            if (habilidad.Nombre == nombre) // Asegúrate de que sea 'Nombre' o 'nombre' según el campo real
            {
                return habilidad;
            }
        }

        return null; // No se encontró
    }
}
