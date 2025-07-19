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

    // 🆕 Referencias para Canvas Dinero
    [SerializeField] private GameObject canvasXP;
    [SerializeField] private GameObject canvasDinero;
    private TextMeshProUGUI XPText;
    private TextMeshProUGUI DineroText;
    private Animator XPAnimator;
    private Animator DineroAnimator;

    void Start()
    {
        // 🆕 Cachear referencias al Canvas Dinero
        if (canvasDinero != null)
        {
            DineroText = canvasDinero.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            DineroAnimator = canvasDinero.GetComponent<Animator>();
        }
        if (canvasXP != null)
        {
            XPText = canvasXP.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            XPAnimator = canvasXP.GetComponent<Animator>();
        }
    }

    void Update()
    {
        MostrarObjetosEnCanvas();

        // Gestiona el desvanecimiento de iconos
        int i = 0;
        foreach (GameObject icono in Iconos)
        {
            if (icono.GetComponent<Image>().sprite == null) break;

            Tiempo[i] -= Time.deltaTime;
            icono.GetComponent<Image>().color = new Color(1, 1, 1, Tiempo[i]);
            icono.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, Tiempo[i]);

            if (Tiempo[i] <= 0 && Lista.Count > 0)
            {
                icono.GetComponent<Image>().sprite = null;
                Lista.RemoveAt(0);
                Cantidades.RemoveAt(0);
                Tiempo[i] = 2;
            }
            i++;
        }
    }

    public void AgregarExperiencia(int cantidadXP)
    {
        int xpActual = PlayerPrefs.GetInt("XPActual", 0) + cantidadXP;
        int xpSiguiente = PlayerPrefs.GetInt("XPSiguiente", 10);
        PlayerPrefs.SetInt("XPActual", xpActual);

        if (xpActual >= xpSiguiente)
        {
            PlayerPrefs.SetInt("XPActual", 0);
            PlayerPrefs.SetInt("Nivel", PlayerPrefs.GetInt("Nivel", 0) + 1);
            PlayerPrefs.SetInt("XPSiguiente", xpSiguiente * 2);
            PlayerPrefs.SetInt("PuntosDeHabilidad", PlayerPrefs.GetInt("PuntosDeHabilidad", 0) + 3);

            XPText.text = "LV.+1";
        }
        else
        {
            XPText.text = "XP + " + cantidadXP;
        }

        if (!XPAnimator.GetCurrentAnimatorStateInfo(0).IsName("Desaparecer"))
        {
            XPAnimator.Play("Desaparecer");
        }
    }
    public void AgregarDinero(int cantidad)
    {
        if (canvasDinero == null) return;

        // Suma el dinero al total
        int dineroActual = PlayerPrefs.GetInt("Dinero", 0) + cantidad;
        PlayerPrefs.SetInt("Dinero", dineroActual);

        // Muestra el dinero ganado en el Canvas Dinero
        DineroText.text = "💰 + " + cantidad; // Puedes cambiar el emoji por "$", "₡", etc.

        if (!DineroAnimator.GetCurrentAnimatorStateInfo(0).IsName("Desaparecer"))
        {
            DineroAnimator.Play("Desaparecer");
        }
    }

    private void MostrarObjetosEnCanvas()
    {
        if (Lista.Count <= 0) return;
        int objetoActual = 0;
        foreach (Scr_CreadorObjetos objeto in Lista)
        {
            if (objetoActual >= Iconos.Length || objeto == null) break;

            Iconos[objetoActual].GetComponent<Image>().sprite = objeto.Icono;
            Iconos[objetoActual].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + Cantidades[objetoActual];
            objetoActual++;
        }
    }
}
