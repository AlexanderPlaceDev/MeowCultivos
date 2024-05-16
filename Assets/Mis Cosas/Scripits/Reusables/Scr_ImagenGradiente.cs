using UnityEngine;
using UnityEngine.UI;

public class Scr_ImagenGradiente : MonoBehaviour
{
    public Color topColor = Color.white;
    public Color bottomColor = Color.black;

    private Image image;

    void Start()
    {
        // Obtener el componente Image adjunto al objeto
        image = GetComponent<Image>();

        // Aplicar el degradado
        ApplyGradient();
    }

    void ApplyGradient()
    {
        // Crear un nuevo degradado
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(topColor, 0.0f), new GradientColorKey(bottomColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );

        // Crear una textura para el degradado
        Texture2D texture = new Texture2D(1, 2);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixel(0, 0, topColor);
        texture.SetPixel(0, 1, bottomColor);
        texture.Apply();

        // Aplicar la textura al material de la imagen
        Material material = new Material(image.material);
        material.SetTexture("_MainTex", texture);
        image.material = material;
    }

    // Opcional: Actualizar el degradado en tiempo de ejecución
    public void UpdateGradient(Color newTopColor, Color newBottomColor)
    {
        topColor = newTopColor;
        bottomColor = newBottomColor;
        ApplyGradient();
    }
}
