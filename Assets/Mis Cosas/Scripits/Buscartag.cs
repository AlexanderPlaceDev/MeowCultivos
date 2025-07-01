using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buscartag
{
    public static GameObject[] BuscarObjetosConTagInclusoInactivos(string tag)
    {
        List<GameObject> resultados = new List<GameObject>();

        // Obtener TODOS los objetos en la escena (activos e inactivos)
        GameObject[] todosLosObjetos = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in todosLosObjetos)
        {
            // Filtrar por objetos en la escena (no assets como prefabs)
            if (obj.hideFlags == HideFlags.None && obj.scene.IsValid())
            {
                if (obj.CompareTag(tag))
                {
                    resultados.Add(obj);
                }
            }
        }

        return resultados.ToArray();
    }
}
