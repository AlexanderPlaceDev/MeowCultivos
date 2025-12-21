using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_NuevoRango : MonoBehaviour
{
    private Image Poste;
    private Image Bandera;
    private TextMeshProUGUI Texto;
    private TextMeshProUGUI Nombre;

    // Datos recibidos desde fuera
    private string ramaActual;
    private int rangoActual;

    void Awake()
    {
        Poste = GetComponent<Image>();
        Bandera = transform.GetChild(0).GetComponent<Image>();
        Texto = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Nombre = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Llamar desde otros scripts
    /// </summary>
    public void MostrarRango(string Rama, int Rango)
    {
        ramaActual = Rama;
        rangoActual = Rango;

        StopAllCoroutines();

        gameObject.SetActive(true);

        // Configuración visual
        SetAlpha(1f);
        Bandera.color = ObtenerColorRama(ramaActual);
        Nombre.text = ObtenerNombreRango(ramaActual, rangoActual);

        StartCoroutine(SecuenciaFade());
    }

    private IEnumerator SecuenciaFade()
    {
        // Visible 2 segundos
        yield return new WaitForSeconds(2f);

        float duracionFade = 2f;
        float tiempo = 0f;

        while (tiempo < duracionFade)
        {
            float alpha = Mathf.Lerp(1f, 0f, tiempo / duracionFade);
            SetAlpha(alpha);
            tiempo += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f);

        // Espera hasta el segundo 6
        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        SetImageAlpha(Poste, alpha);
        SetImageAlpha(Bandera, alpha);
        SetTMPAlpha(Texto, alpha);
        SetTMPAlpha(Nombre, alpha);
    }

    private void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;

        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    private void SetTMPAlpha(TextMeshProUGUI tmp, float alpha)
    {
        if (tmp == null) return;

        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }

    private Color ObtenerColorRama(string Rama)
    {
        switch (Rama)
        {
            case "Naturaleza":
                return Color.green;
            case "Industrial":
                return Color.blue;
            case "Tecnica":
                return new Color(1f, 0.5f, 0f); // Naranja
            case "Arsenal":
                return Color.red;
            default:
                return Color.white;
        }
    }

    private string ObtenerNombreRango(string Rama, int Rango)
    {
        switch (Rama)
        {
            case "Naturaleza":
                switch (Rango)
                {
                    case 1: return "CAMPISTA";
                    case 2: return "GRANJERO";
                    case 3: return "ARTESANO";
                    case 4: return "GUARDA BOSQUES";
                    default: return "Sin Rango";
                }

            case "Industrial":
                switch (Rango)
                {
                    case 1: return "APRENDIZ";
                    case 2: return "AFICIONADO";
                    case 3: return "TECNICO";
                    case 4: return "INGENIERO";
                    default: return "Sin Rango";
                }

            case "Tecnica":
                switch (Rango)
                {
                    case 1: return "AVENTURERO";
                    case 2: return "EXPLORADOR";
                    case 3: return "PIONERO";
                    case 4: return "ESPECIALISTA";
                    default: return "Sin Rango";
                }

            case "Arsenal":
                switch (Rango)
                {
                    case 1: return "RECLUTA";
                    case 2: return "SOLDADO";
                    case 3: return "CAPITAN";
                    default: return "Sin Rango";
                }

            default:
                return "Sin Rango";
        }
    }
}
