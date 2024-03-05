using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_Radio : MonoBehaviour
{
    [SerializeField, TextArea(4, 6)] public string[] Lineas;
    [SerializeField] public  GameObject PanelDialogo;
    [SerializeField] public TextMeshProUGUI Texto;
    [SerializeField] float VelocidadAlHablar;
    public bool Comenzo;
    int LineaActual;

    private void Update()
    {
        GetComponent<Scr_EventosGuardado>().EventoLineasRadio(Lineas);

        if(GetComponent<RectTransform>().anchoredPosition.y == -20)
        {
            if (Input.GetKeyDown(KeyCode.M) && Time.timeScale == 1)
            {
                if (!Comenzo)
                {
                    if (Lineas.Length > 0)
                    {
                        IniciarDialogo();
                    }
                }
            }
        }
        else
        {
            Comenzo = false;
            PanelDialogo.SetActive(false);
        }
        if(Lineas!=null)
        {
            if (LineaActual < Lineas.Length)
            {
                if (Texto.text == Lineas[LineaActual])
                {
                    StartCoroutine(LineaSiguiente());
                }
            }
        }
        
        

    }

    public void IniciarDialogo()
    {
        Comenzo = true;
        PanelDialogo.SetActive(true);
        LineaActual = 0;
        StartCoroutine(MostrarLinea());

    }

    public IEnumerator MostrarLinea()
    {
        Texto.text = "";
        if(Lineas.Length > 0)
        {
            foreach (char ch in Lineas[LineaActual])
            {
                Texto.text += ch;
                yield return new WaitForSeconds(VelocidadAlHablar);
            }
        }
        
    }

    public IEnumerator LineaSiguiente()
    {
        LineaActual++;
        yield return new WaitForSeconds(PlayerPrefs.GetFloat("TiempoRadio",1));
        if (LineaActual < Lineas.Length)
        {
            StartCoroutine(MostrarLinea());
        }
        else
        {
            Comenzo = false;
            PanelDialogo.SetActive(false);
        }
    }
}
