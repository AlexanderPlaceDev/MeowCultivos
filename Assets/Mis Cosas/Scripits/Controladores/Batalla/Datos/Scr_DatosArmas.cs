using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Scr_DatosArmas : MonoBehaviour
{
    public Scr_CreadorArmas[] TodasLasArmas;
    public bool[] ArmasDesbloqueadas;

    public Scr_CreadorHabilidadesBatalla[] HabilidadesTemporales;
    public int[] UsosHabilidadesT;

    public Scr_CreadorHabilidadesBatalla[] HabilidadesPermanentes;
    public bool[] HabilidatPDesbloqueadas;

    public SCR_Pociones[] Pociones;
    public int[] CantidadPociones;

    public int[] UsosHabilidadesTInicial;
    public int[] CantidadPocionesInicial;

    void Start()
    {
        ActualizarArmas();

        ActualizarHabilidades();

        VerificarConsumosBatalla();
    }

    public void ActualizarArmas()
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
    }

    public void ActualizarHabilidades()
    {
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

    //Esta funcion se usa tambien en el controlador de cinematicas
    public void DesbloquearArma(string Nombre)
    {
        Debug.Log("Se esta guardando el arma: " + Nombre);
        PlayerPrefs.SetString("Arma" + Nombre, "Si");
        PlayerPrefs.Save();

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
        guardarHabilidadesPermanentes();
        PlayerPrefs.Save();
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

        for (int i = 0; i < HabilidadesTemporales.Length; i++)
        {
            if (HabilidadesTemporales[i].Nombre == Nombre)
            {
                Debug.Log("Entra2");
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
        for (int i = 0; i < HabilidadesTemporales.Length; i++)
        {
            if (HabilidadesTemporales[i] != null)
            {
                if (HabilidadesTemporales[i].Nombre == nombre)
                {
                    return i;
                }
            }
        }

        return -1;
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

    public void SincronizarHabilidadesYPocionesDesdeInventario(
    Scr_Inventario inventario
)
    {
        // =========================
        // HABILIDADES TEMPORALES
        // =========================

        for (int i = 0; i < HabilidadesTemporales.Length; i++)
        {
            string nombre = HabilidadesTemporales[i].Nombre;

            int cantidad = 0;

            for (int j = 0; j < inventario.Objetos.Length; j++)
            {
                if (inventario.Objetos[j].Nombre == nombre)
                {
                    cantidad = inventario.Cantidades[j];
                    break;
                }
            }

            // Cantidad actual
            UsosHabilidadesT[i] = cantidad;
        }

        // =========================
        // POCIONES
        // =========================

        for (int i = 0; i < Pociones.Length; i++)
        {
            string nombre = Pociones[i].Nombre;

            int cantidad = 0;

            for (int j = 0; j < inventario.Objetos.Length; j++)
            {
                if (inventario.Objetos[j].Nombre == nombre)
                {
                    cantidad = inventario.Cantidades[j];
                    break;
                }
            }

            // Cantidad actual
            CantidadPociones[i] = cantidad;
        }

        // =========================
        // SNAPSHOT INICIAL
        // =========================

        UsosHabilidadesTInicial =
            (int[])UsosHabilidadesT.Clone();

        CantidadPocionesInicial =
            (int[])CantidadPociones.Clone();
    }

    private void VerificarConsumosBatalla()
    {
        GameObject gata = GameObject.Find("Gata");

        if (gata == null)
            return;

        Scr_Inventario inventario =
            gata.transform.GetChild(7)
            .GetComponent<Scr_Inventario>();

        if (inventario == null)
            return;

        // =========================
        // POCIONES
        // =========================

        if (CantidadPocionesInicial != null)
        {
            for (int i = 0; i < Pociones.Length; i++)
            {
                int consumidas =
                    CantidadPocionesInicial[i]
                    - CantidadPociones[i];

                if (consumidas > 0)
                {
                    inventario.QuitarObjeto(
                        Pociones[i].Nombre,
                        consumidas
                    );
                }
            }
        }

        // =========================
        // GADGETS
        // =========================

        if (UsosHabilidadesTInicial != null)
        {
            for (int i = 0; i < HabilidadesTemporales.Length; i++)
            {
                int consumidas =
                    UsosHabilidadesTInicial[i]
                    - UsosHabilidadesT[i];

                if (consumidas > 0)
                {
                    inventario.QuitarObjeto(
                        HabilidadesTemporales[i].Nombre,
                        consumidas
                    );
                }
            }
        }

        // =========================
        // LIMPIAR SNAPSHOTS
        // =========================

        CantidadPocionesInicial = null;
        UsosHabilidadesTInicial = null;
    }
}
