using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Inventario : MonoBehaviour
{
    [SerializeField] public Scr_CreadorObjetos[] Objetos;
    [SerializeField] public int[] Cantidades;
    [SerializeField] int Limite;


    void AgregarObjeto(int Cantidad, string Nombre)
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
    }

    void QuitarObjeto(int Cantidad, string Nombre)
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
}
