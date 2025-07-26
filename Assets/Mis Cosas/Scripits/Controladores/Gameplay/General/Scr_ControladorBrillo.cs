using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorBrillo : MonoBehaviour
{
    private Image overlay;
    private float brilloActual = -1f;

    void Start()
    {
        overlay = GetComponent<Image>();
    }

    void LateUpdate()
    {
        // Obtener brillo guardado (0-100)
        int brilloGuardado = PlayerPrefs.GetInt("Brillo", 50);

        // Calcular alpha:
        // Brillo 100% → Alpha 0
        // Brillo 0% → Alpha 200/255 ≈ 0.78
        float alphaMax = 200f / 255f;
        float alpha = alphaMax * (1f - brilloGuardado / 100f);

        // Solo actualizar si cambia
        if (Mathf.Abs(alpha - brilloActual) > 0.01f)
        {
            brilloActual = alpha;
            overlay.color = new Color(0f, 0f, 0f, alpha);
        }
    }
}
