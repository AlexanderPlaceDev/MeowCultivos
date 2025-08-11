using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/MisionData")]
public class MisionesData : ScriptableObject
{
    public Scr_CreadorMisiones[] Mision;

    public Scr_CreadorMisiones GetByName(string name)
    {
        foreach (var a in Mision)
        {
            if (a.TituloMision == name)
                return a;
        }

        Debug.LogWarning("Mision no encontrado: " + name);
        return null;
    }

    public int Count => Mision?.Length ?? 0;
}
