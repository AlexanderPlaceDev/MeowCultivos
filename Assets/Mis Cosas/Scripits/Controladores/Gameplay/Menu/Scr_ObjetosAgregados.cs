using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ObjetosAgregados : MonoBehaviour
{
    public List<Scr_CreadorObjetos> Lista = new List<Scr_CreadorObjetos>();
    public List<int> Cantidades = new List<int>();
    [SerializeField] GameObject[] Iconos;
    public float[] Tiempo = { 2, 2, 2, 2 };

    // Bandera para evitar agregar objetos repetidamente
    private bool objetosAgregados = false;

    void Start()
    {
    }

    void Update()
    {
        AgregarObjetosBatalla();

        if (Lista.Count > 0)
        {
            int ObjetoActual = 0;
            foreach (Scr_CreadorObjetos Objeto in Lista)
            {
                if (ObjetoActual == 4 || Lista[ObjetoActual] == null)
                {
                    break;
                }
                else
                {
                    Iconos[ObjetoActual].GetComponent<Image>().sprite = Lista[ObjetoActual].Icono;
                    Iconos[ObjetoActual].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + Cantidades[ObjetoActual].ToString();
                }
                ObjetoActual++;
            }
        }

        int i = 0;
        foreach (GameObject Icono in Iconos)
        {
            if (Icono.GetComponent<Image>().sprite == null) { break; }

            Tiempo[i] -= Time.deltaTime;
            Icono.GetComponent<Image>().color = new Color(1, 1, 1, Tiempo[i]);
            Icono.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, Tiempo[i]);

            if (Tiempo[i] <= 0 && Lista.Count>0)
            {
                Icono.GetComponent<Image>().sprite = null;
                Lista.RemoveAt(0);
                Cantidades.RemoveAt(0);
                Tiempo[i] = 2;
            }
            i++;
        }
    }

    void AgregarObjetosBatalla()
    {
        Scr_DatosSingletonBatalla Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();

        if (!objetosAgregados && Singleton.CantidadesRecompensa.Count > 0 && Singleton.CantidadesRecompensa[0] != 0)
        {
            for (int i = 0; i < Singleton.ObjetosRecompensa.Count; i++)
            {
                if (i >= Singleton.CantidadesRecompensa.Count)
                {
                    Debug.LogError("Desajuste detectado entre ObjetosRecompensa y CantidadesRecompensa. ÍNDICE: " + i);
                    break;
                }

                Scr_CreadorObjetos Objeto = Singleton.ObjetosRecompensa[i];
                Lista.Add(Objeto);
                Cantidades.Add(Singleton.CantidadesRecompensa[i]);
                Singleton.ObjetosRecompensa[i] = null;
                Singleton.CantidadesRecompensa[i] = 0;
            }
            objetosAgregados = true;
        }
        else
        {
            if (Singleton.CantidadesRecompensa.ToArray()[0] > 0)
            {
                Singleton.CantidadesRecompensa.Clear();
                Singleton.ObjetosRecompensa.Clear();
            }
        }
    }


}
