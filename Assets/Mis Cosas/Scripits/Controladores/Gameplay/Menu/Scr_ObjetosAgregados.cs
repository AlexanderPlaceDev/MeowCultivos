using PrimeTween;
using System.Collections;
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
    void Start()
    {

    }

    void Update()
    {
        if (Lista.Count > 0)
        {
            int ObjetoActual = 0;
            foreach (Scr_CreadorObjetos Objeto in Lista)
            {
                if (ObjetoActual == 4)
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

            if (Tiempo[i] <= 0)
            {
                Icono.GetComponent<Image>().sprite = null;
                Lista.RemoveAt(0);
                Cantidades.RemoveAt(0);
                Tiempo[i] = 2;
            }
            i++;
        }
    }
}
