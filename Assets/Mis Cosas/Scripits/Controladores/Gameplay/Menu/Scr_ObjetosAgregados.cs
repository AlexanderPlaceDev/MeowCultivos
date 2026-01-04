using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ObjetosAgregados : MonoBehaviour
{
    // =========================
    // PLAYER PREFS
    // =========================
    private const string PREF_DINERO = "Dinero";
    private const string PREF_DINERO_PENDIENTE = "DineroPendiente";
    private const string PREF_DIA_DINERO_OTORGADO = "DiaDineroOtorgado";

    // =========================
    // LISTAS VISUALES
    // =========================
    public List<Scr_CreadorObjetos> Lista = new();
    public List<int> Cantidades = new();
    public List<bool> FueExcedente = new();

    [SerializeField] private GameObject[] Iconos;
    public float[] Tiempo;

    // =========================
    // REFERENCIAS
    // =========================
    private Scr_ControladorTiempo ControladorTiempo;
    private Scr_DatosSingletonBatalla Singleton;

    // =========================
    // UI XP / DINERO
    // =========================
    [SerializeField] private GameObject canvasXP;
    [SerializeField] private GameObject canvasDinero;

    private TextMeshProUGUI XPText;
    private TextMeshProUGUI DineroText;
    private Animator XPAnimator;
    private Animator DineroAnimator;
    private AudioSource dineroAudioSource;
    private AudioSource xpAudioSource;

    // =========================
    // ESTADO
    // =========================
    private int xpPendiente = 0;
    public int DineroPendiente = 0;
    private string DiaDineroOtorgado = "";

    private HashSet<int> xpOtorgada = new();

    // =========================
    // UNITY
    // =========================
    void Start()
    {
        Singleton = GameObject.Find("Singleton")?.GetComponent<Scr_DatosSingletonBatalla>();
        ControladorTiempo = GameObject.Find("Controlador Tiempo")?.GetComponent<Scr_ControladorTiempo>();

        DineroPendiente = PlayerPrefs.GetInt(PREF_DINERO_PENDIENTE, 0);
        DiaDineroOtorgado = PlayerPrefs.GetString(PREF_DIA_DINERO_OTORGADO, "");

        if (Iconos != null)
        {
            Tiempo = new float[Iconos.Length];
            for (int i = 0; i < Tiempo.Length; i++)
                Tiempo[i] = 2f;
        }

        if (canvasDinero != null)
        {
            DineroText = canvasDinero.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            DineroAnimator = canvasDinero.GetComponent<Animator>();
            dineroAudioSource = canvasDinero.GetComponent<AudioSource>();
        }

        if (canvasXP != null)
        {
            XPText = canvasXP.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            XPAnimator = canvasXP.GetComponent<Animator>();
            xpAudioSource = canvasXP.GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        DarDineroPendiente();
        MostrarObjetosEnCanvas();
        ActualizarTimers();
    }

    // =========================
    // ENTRADA DESDE INVENTARIO
    // =========================
    public void RegistrarObjetoVisual(
        Scr_CreadorObjetos objeto,
        int cantidad,
        bool fueExcedente
    )
    {
        if (objeto == null || cantidad <= 0)
            return;

        Lista.Add(objeto);
        Cantidades.Add(cantidad);
        FueExcedente.Add(fueExcedente);

        if (Tiempo != null && Lista.Count - 1 < Tiempo.Length)
            Tiempo[Lista.Count - 1] = 2f;
    }

    // =========================
    // DINERO
    // =========================
    public void AgregarDinero(int cantidad)
    {
        if (cantidad <= 0) return;

        int dineroActual = PlayerPrefs.GetInt(PREF_DINERO, 0) + cantidad;
        PlayerPrefs.SetInt(PREF_DINERO, dineroActual);
        PlayerPrefs.Save();

        if (DineroText != null)
            DineroText.text = "+$" + cantidad.ToString("N0");

        DineroAnimator?.Play("Desaparecer");
        dineroAudioSource?.Play();
    }

    public void AgregarDineroPendiente(int cantidad)
    {
        if (cantidad <= 0) return;

        DineroPendiente += cantidad;
        PlayerPrefs.SetInt(PREF_DINERO_PENDIENTE, DineroPendiente);
        PlayerPrefs.Save();
    }

    private void DarDineroPendiente()
    {
        if (ControladorTiempo == null) return;
        if (ControladorTiempo.HoraActual != 0) return;
        if (DineroPendiente <= 0) return;

        string diaActual = ControladorTiempo.DiaActual;
        if (DiaDineroOtorgado == diaActual) return;

        int dineroActual = PlayerPrefs.GetInt(PREF_DINERO, 0) + DineroPendiente;
        PlayerPrefs.SetInt(PREF_DINERO, dineroActual);

        DineroPendiente = 0;
        PlayerPrefs.SetInt(PREF_DINERO_PENDIENTE, 0);

        DiaDineroOtorgado = diaActual;
        PlayerPrefs.SetString(PREF_DIA_DINERO_OTORGADO, diaActual);
        PlayerPrefs.Save();

        if (DineroText != null)
            DineroText.text = "+$" + dineroActual.ToString("N0");

        DineroAnimator?.Play("Desaparecer");
        dineroAudioSource?.Play();
    }

    // =========================
    // XP (solo visual)
    // =========================
    public void AgregarExperiencia(int cantidadXP)
    {
        if (cantidadXP <= 0) return;

        xpPendiente += cantidadXP;

        int xpActual = PlayerPrefs.GetInt("XPActual", 0) + cantidadXP;
        PlayerPrefs.SetInt("XPActual", xpActual);

        XPText.text = "XP + " + xpPendiente;
        XPAnimator?.Play("Desaparecer");

        CancelInvoke(nameof(ResetXPVisual));
        Invoke(nameof(ResetXPVisual), 0.2f);
    }

    void ResetXPVisual()
    {
        xpPendiente = 0;
    }

    // =========================
    // VISUAL
    // =========================
    private void MostrarObjetosEnCanvas()
    {
        for (int i = 0; i < Iconos.Length; i++)
        {
            var img = Iconos[i].GetComponent<Image>();
            var txt = Iconos[i].transform.childCount > 0
                ? Iconos[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>()
                : null;

            if (i < Lista.Count)
            {
                var item = Lista[i];
                int cantidad = Cantidades[i];
                bool excedente = FueExcedente[i];

                img.sprite = item.Icono;

                float alpha = Mathf.Clamp01(Tiempo[i] / 2f);
                img.color = new Color(1, 1, 1, alpha);

                if (txt != null)
                {
                    txt.text = (excedente ? "-" : "+") + cantidad;
                    Color baseColor = excedente ? Color.red : Color.white;
                    txt.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                }
            }
            else
            {
                img.sprite = null;
                img.color = Color.clear;
                if (txt != null)
                {
                    txt.text = "";
                    txt.color = Color.clear;
                }
            }
        }
    }

    // =========================
    // TIMERS
    // =========================
    private void ActualizarTimers()
    {
        for (int i = Lista.Count - 1; i >= 0; i--)
        {
            Tiempo[i] -= Time.deltaTime;

            if (Tiempo[i] <= 0f)
            {
                Lista.RemoveAt(i);
                Cantidades.RemoveAt(i);
                FueExcedente.RemoveAt(i);

                ReindexarXP(i);

                for (int t = i; t < Tiempo.Length - 1; t++)
                    Tiempo[t] = Tiempo[t + 1];

                Tiempo[Tiempo.Length - 1] = 0f;
            }
        }
    }

    private void ReindexarXP(int indiceEliminado)
    {
        HashSet<int> nuevo = new();

        foreach (int idx in xpOtorgada)
        {
            if (idx < indiceEliminado) nuevo.Add(idx);
            else if (idx > indiceEliminado) nuevo.Add(idx - 1);
        }

        xpOtorgada = nuevo;
    }
}
