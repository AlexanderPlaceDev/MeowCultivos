using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TiendaScroller : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform content; // El objeto con VerticalLayoutGroup
    public Slider slider;          // El slider que controlará el desplazamiento
    public float espacioVisible = 400f; // Altura del área visible en Y (ajústala al tamaño real)

    private List<RectTransform> hijos = new();
    private float alturaTotal;

    void Start()
    {
        // Guardar todos los hijos actuales
        hijos.Clear();
        foreach (Transform t in content)
        {
            hijos.Add(t.GetComponent<RectTransform>());
        }

        // Calcular altura total del contenido
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        alturaTotal = content.rect.height;

        // Escuchar el cambio del slider
        slider.onValueChanged.AddListener(OnSliderChanged);
        OnSliderChanged(slider.value);
    }

    void OnSliderChanged(float value)
    {
        // Calcula el desplazamiento vertical (top en negativo)
        float desplazamiento = Mathf.Lerp(0f, alturaTotal - espacioVisible, value);
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, desplazamiento);

        ActualizarVisibilidad();
    }

    void ActualizarVisibilidad()
    {
        foreach (RectTransform hijo in hijos)
        {
            // Posición Y del hijo relativa al content
            float yLocal = hijo.anchoredPosition.y - content.anchoredPosition.y;

            // Si la parte inferior del hijo está por encima de 0 (fuera de la vista superior), desactivar
            if (yLocal + hijo.rect.height < 0 || yLocal > espacioVisible)
            {
                ActivarComponentesHijo(hijo.gameObject, false);
            }
            else
            {
                ActivarComponentesHijo(hijo.gameObject, true);
            }
        }
    }

    void ActivarComponentesHijo(GameObject hijo, bool activo)
    {
        // Desactiva iconos, textos, etc. (sin desactivar el GameObject completo)
        foreach (var r in hijo.GetComponentsInChildren<Renderer>(true))
            r.enabled = activo;
        foreach (var img in hijo.GetComponentsInChildren<Image>(true))
            img.enabled = activo;
        foreach (var txt in hijo.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
            txt.enabled = activo;
    }
}
