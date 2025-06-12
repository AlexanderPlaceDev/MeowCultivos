using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Inventario : MonoBehaviour
{
    [SerializeField] public Scr_CreadorObjetos[] Objetos;
    [SerializeField] public int[] Cantidades;
    [SerializeField] int Limite;

    public Scr_ControladorMisiones ControladorMisiones;

    private void Start()
    {
        // Cargar las cantidades desde PlayerPrefs al iniciar
        for (int i = 0; i < Objetos.Length; i++)
        {
            string key = Objetos[i].Nombre + "_Cantidad";
            if (PlayerPrefs.HasKey(key))
            {
                Cantidades[i] = PlayerPrefs.GetInt(key);
            }
            else
            {
                PlayerPrefs.SetInt(key, Cantidades[i]); // Guardar inicial si no existe
            }
        }
        if (GameObject.Find("Gata"))
        {
            ControladorMisiones = GameObject.Find("Gata").transform.GetChild(4).GetComponent<Scr_ControladorMisiones>();
        }
    }

    private void Update()
    {
        // Guardar automáticamente los valores actualizados
        GuardarInventario();
    }

    public void AgregarObjeto(int Cantidad, string Nombre)
    {
        int i = 0;
        foreach (Scr_CreadorObjetos Objeto in Objetos)
        {
            if (Objeto.Nombre == Nombre)
            {
                if (Cantidades[i] + Cantidad > Limite)
                {
                    Cantidades[i] = Limite;
                }
                else
                {
                    Cantidades[i] += Cantidad;
                }
                break;
            }
            i++;
        }
        if(ControladorMisiones!= null)
        {
            ControladorMisiones.RevisarTodasLasMisionesSecundarias();
        }
    }

    public void QuitarObjeto(int Cantidad, string Nombre)
    {
        int i = 0;
        foreach (Scr_CreadorObjetos Objeto in Objetos)
        {
            if (Objeto.Nombre == Nombre)
            {
                if (Cantidades[i] > Cantidad)
                {
                    Cantidades[i] -= Cantidad;
                }
                else
                {
                    Cantidades[i] = 0;
                }
                break;
            }
            i++;
        }
    }

    private void GuardarInventario()
    {
        for (int i = 0; i < Objetos.Length; i++)
        {
            string key = Objetos[i].Nombre + "_Cantidad";
            // Comprobar si el valor actual es diferente al almacenado en PlayerPrefs
            if (PlayerPrefs.GetInt(key) != Cantidades[i])
            {
                PlayerPrefs.SetInt(key, Cantidades[i]); // Actualizar PlayerPrefs
            }
        }
    }

    private void OnApplicationQuit()
    {
        // Guardar inventario al salir de la aplicación
        GuardarInventario();
    }
}
