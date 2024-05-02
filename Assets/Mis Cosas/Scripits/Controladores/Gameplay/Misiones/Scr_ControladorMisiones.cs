using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorMisiones : MonoBehaviour
{
    public Scr_CreadorMisiones MisionActual;
    public bool MisionCompleta;

    [SerializeField] GameObject BotonesUI;

    private bool[] TeclasPresionadas;
    private float[] TiempoTeclas;

    void Update()
    {
        // Comprueba si la misión está completa
        ComprobarMision();
    }

    void ComprobarMision()
    {
        if (MisionActual == null)
        {
            MisionCompleta = false;
            return;
        }

        switch (MisionActual.Tipo)
        {
            case "Teclas":
                ActualizarMisionTeclas();
                break;
        }

        BotonesUI.SetActive(!MisionCompleta);
    }

    void ActualizarMisionTeclas()
    {
        if (TeclasPresionadas == null || TeclasPresionadas.Length != MisionActual.Teclas.Length)
        {
            TeclasPresionadas = new bool[MisionActual.Teclas.Length];
            TiempoTeclas = new float[MisionActual.Teclas.Length];
        }

        for (int i = 0; i < MisionActual.Teclas.Length; i++)
        {
            if (TiempoTeclas[i] >= 1)
            {
                TeclasPresionadas[i] = true;
            }
            else
            {
                if (Input.GetKey(MisionActual.Teclas[i]))
                {
                    TiempoTeclas[i] += Time.deltaTime;
                }
                else if (TiempoTeclas[i] > 0)
                {
                    TiempoTeclas[i] -= Time.deltaTime;
                }
            }

            // Actualiza el indicador visual del progreso de la tecla
            if (BotonesUI != null)
            {
                Image fillImage = BotonesUI.transform.GetChild(i).GetChild(1).GetComponent<Image>();
                fillImage.fillAmount = TiempoTeclas[i];
            }
        }

        // Comprueba si todas las teclas están presionadas
        MisionCompleta = System.Array.TrueForAll(TeclasPresionadas, t => t);
    }
}
