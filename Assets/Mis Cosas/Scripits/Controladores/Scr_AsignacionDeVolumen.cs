using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Scr_AsignacionDeVolumen : MonoBehaviour
{
    public enum CategoriaVolumen
    {
        Volumen,
        Volumen_Musica,
        Volumen_Ambiente,
        Volumen_Combate
    }

    [Header("Categoría de volumen")]
    [Tooltip("Selecciona qué PlayerPref usar para este audio")]
    public CategoriaVolumen categoria;

    private AudioSource audioSource;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        audioSource = GetComponent<AudioSource>();
        AsignarVolumen();
    }

    public void AsignarVolumen()
    {
        string nombrePref = categoria.ToString(); // el nombre del PlayerPref
        int valorPorDefecto = ObtenerValorPorDefecto(categoria);

        int valor = PlayerPrefs.GetInt(nombrePref, valorPorDefecto);   // usa el valor por defecto de esa categoría
        int volumenGeneral = PlayerPrefs.GetInt("Volumen", 100);       // valor general

        valor = Mathf.Clamp(valor, 0, 100);
        volumenGeneral = Mathf.Clamp(volumenGeneral, 0, 100);

        float volumenNormalizado = (valor / 100f) * (volumenGeneral / 100f);
        audioSource.volume = volumenNormalizado;

    }

    private int ObtenerValorPorDefecto(CategoriaVolumen categoria)
    {
        switch (categoria)
        {
            case CategoriaVolumen.Volumen: return 80;
            case CategoriaVolumen.Volumen_Musica: return 30;
            case CategoriaVolumen.Volumen_Ambiente: return 40;
            case CategoriaVolumen.Volumen_Combate: return 70;
            default: return 100;
        }
    }
}
