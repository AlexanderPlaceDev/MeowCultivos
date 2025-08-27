using UnityEngine;
using System.Collections;

public class Scr_Parpadeo : MonoBehaviour
{
    [Header("Renderer del enemigo (con materiales)")]
    [SerializeField] private SkinnedMeshRenderer rendererMalla;

    [Header("Nombre del material del ojo")]
    [Tooltip("Busca un material cuyo nombre contenga este texto (ej: 'Ojo')")]
    [SerializeField] private string nombreMaterialOjo = "Ojo";

    [Header("Config. parpadeo")]
    [Tooltip("Tiempo mínimo entre parpadeos")]
    public float intervaloMin = 2f;
    [Tooltip("Tiempo máximo entre parpadeos")]
    public float intervaloMax = 5f;
    [Tooltip("Duración de cerrar el ojo (1 → 0)")]
    public float duracionCierre = 0.07f;
    [Tooltip("Duración de abrir el ojo (0 → 1)")]
    public float duracionApertura = 0.1f;

    private int indiceMaterialOjo = -1;
    private MaterialPropertyBlock mpb;
    private bool parpadeando;
    private float proximoParpadeo;

    private void Awake()
    {
        if (rendererMalla == null)
            rendererMalla = GetComponentInChildren<SkinnedMeshRenderer>();

        mpb = new MaterialPropertyBlock();

        // Buscar índice del material que tenga el nombre indicado
        indiceMaterialOjo = BuscarMaterialPorNombre(rendererMalla, nombreMaterialOjo);

        if (indiceMaterialOjo < 0)
            Debug.LogWarning($"{name}: No encontré material que contenga '{nombreMaterialOjo}'");

        ProgramarSiguienteParpadeo();
        SetAlpha(1f);
    }

    private void Update()
    {
        if (!parpadeando && Time.time >= proximoParpadeo)
            StartCoroutine(CoParpadear());
    }

    private IEnumerator CoParpadear()
    {
        parpadeando = true;

        // cerrar
        float t = 0;
        while (t < duracionCierre)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(1, 0, t / duracionCierre));
            yield return null;
        }
        SetAlpha(0);

        // abrir
        t = 0;
        while (t < duracionApertura)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0, 1, t / duracionApertura));
            yield return null;
        }
        SetAlpha(1);

        parpadeando = false;
        ProgramarSiguienteParpadeo();
    }

    private void ProgramarSiguienteParpadeo()
    {
        proximoParpadeo = Time.time + Random.Range(intervaloMin, intervaloMax);
    }

    private void SetAlpha(float valor)
    {
        if (rendererMalla == null || indiceMaterialOjo < 0) return;

        mpb.Clear();
        mpb.SetFloat("_Alpha", valor);
        rendererMalla.SetPropertyBlock(mpb, indiceMaterialOjo);
    }

    private int BuscarMaterialPorNombre(Renderer rend, string nombre)
    {
        var mats = rend.sharedMaterials;
        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i] != null && mats[i].name.Contains(nombre))
                return i;
        }
        return -1;
    }
}
