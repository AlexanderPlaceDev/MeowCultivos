using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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

    public SCR_Pociones[] Pociones;
    public int[] CantidadPociones;

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
            if (i == 0)
            {
                HabilidatPDesbloqueadas[i] = true;
            }
            else if (PlayerPrefs.GetString("Habilidad" + HabilidadesTemporales[i].Nombre, "No") == "Si")
            {
                HabilidatTDesbloqueadas[i] = true;

            }
            else
            {
                HabilidatTDesbloqueadas[i] = true;
            }

            UsosHabilidadesT[i] = PlayerPrefs.GetInt("UsoTemporal" + HabilidadesTemporales[i].Nombre, 0);
        }

        for (int i = 0; i < HabilidadesPermanentes.Length; i++)
        {
            if (PlayerPrefs.GetString("Habilidad" + HabilidadesPermanentes[i].Nombre, "No") == "Si")
            {
                HabilidatPDesbloqueadas[i] = true;

            }
            else
            {
                HabilidatPDesbloqueadas[i] = true;
            }
        }

        for (int i = 0; i < Pociones.Length; i++)
        {
            CantidadPociones[i] = PlayerPrefs.GetInt("Pociones" + Pociones[i].Nombre, 1);
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

    public void guardarHabilidades()
    {
        guardarHabilidadesTemporales();
        guardarHabilidadesPermanentes();
    }
    public void guardarHabilidadesTemporales()
    {
        for (int i = 0; i < HabilidadesTemporales.Length; i++)
        {
            if (HabilidatTDesbloqueadas[i])
            {
                PlayerPrefs.SetString("Habilidad" + HabilidadesTemporales[i].Nombre, "Si");

            }
            else
            {
                PlayerPrefs.SetString("Habilidad" + HabilidadesTemporales[i].Nombre, "No");
            }
            PlayerPrefs.SetInt("UsoTemporal" + HabilidadesTemporales[i].Nombre, UsosHabilidadesT[i]);
        }
    }
    public void guardarHabilidadesPermanentes()
    {
        for (int i = 0; i < HabilidadesPermanentes.Length; i++)
        {
            if (HabilidatPDesbloqueadas[i])
            {
                PlayerPrefs.SetString("Habilidad" + HabilidadesPermanentes[i].Nombre, "Si");

            }
            else
            {
                PlayerPrefs.SetString("Habilidad" + HabilidadesPermanentes[i].Nombre, "No");
            }
        }
    }

    public void guardarHabilidadesPociones()
    {
        for (int i = 0; i < Pociones.Length; i++)
        {
            PlayerPrefs.SetInt("Pociones" + Pociones[i].Nombre, CantidadPociones[i]);
        }
    }
    public void AgregarUsosTemporales(string Nombre)
    {

        for (int i = 1; i < HabilidadesTemporales.Length; i++)
        {
            if (HabilidadesTemporales[i].Nombre == Nombre)
            {
                UsosHabilidadesT[i]++;
                break;
            }
        }
    }

    public void QuitarUsosTemporales(string Nombre)
    {

        for (int i = 1; i < HabilidadesTemporales.Length; i++)
        {
            if (HabilidadesTemporales[i].Nombre == Nombre)
            {
                UsosHabilidadesT[i]--;
                break;
            }
        }
    }
    public void QuitarCanidadPociones(string Nombre)
    {
        for (int i = 0; i < Pociones.Length; i++)
        {
            if (Pociones[i].Nombre == Nombre)
            {
                CantidadPociones[i]--;
                break;
            }
        }
        guardarHabilidadesPociones();
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

    public int BuscarUSoHabilidadTemporalPorNombre(string nombre)
    {
        for(int i = 0; i<HabilidadesTemporales.Length; i++)
        {
            if (HabilidadesTemporales[i].Nombre == nombre) // Asegúrate de que sea 'Nombre' o 'nombre' según el campo real
            {
                return i;
            }
        }

        return 0; // No se encontró
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
