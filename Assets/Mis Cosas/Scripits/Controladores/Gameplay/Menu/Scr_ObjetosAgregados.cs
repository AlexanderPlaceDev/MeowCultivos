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
    // LISTAS
    // =========================
    public List<Scr_CreadorObjetos> Lista = new List<Scr_CreadorObjetos>();
    public List<int> Cantidades = new List<int>();
    public List<bool> FueExcedente = new List<bool>();


    [SerializeField] private GameObject[] Iconos;
    public float[] Tiempo;

    // =========================
    // REFERENCIAS
    // =========================
    [SerializeField] private Scr_Inventario Inventario;
    private Scr_ControladorTiempo ControladorTiempo;
    private Scr_DatosSingletonBatalla Singleton;

    // =========================
    // UI
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

    // 🔹 DINERO
    public int DineroPendiente = 0;          // dinero diferido
    private string DiaDineroOtorgado = "";

    private HashSet<int> xpOtorgada = new HashSet<int>();

    // =========================
    // UNITY
    // =========================
    void Start()
    {
        Singleton = GameObject.Find("Singleton")?.GetComponent<Scr_DatosSingletonBatalla>();
        ControladorTiempo = GameObject.Find("Controlador Tiempo")?.GetComponent<Scr_ControladorTiempo>();

        // Cargar persistencia
        DineroPendiente = PlayerPrefs.GetInt(PREF_DINERO_PENDIENTE, 0);
        DiaDineroOtorgado = PlayerPrefs.GetString(PREF_DIA_DINERO_OTORGADO, "");

        // Inicializar tiempos visuales
        if (Iconos != null)
        {
            Tiempo = new float[Iconos.Length];
            for (int i = 0; i < Tiempo.Length; i++)
                Tiempo[i] = 2f;
        }

        if (Inventario == null)
            Inventario = FindObjectOfType<Scr_Inventario>();

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

        if (Singleton != null)
            ProcesarRecompensasSingleton();

        MostrarObjetosEnCanvas();
        ActualizarTimers();
    }

    // =========================
    // DINERO DIFERIDO
    // =========================
    private void DarDineroPendiente()
    {
        if (ControladorTiempo == null)
            return;

        // Solo a las 00:00
        if (ControladorTiempo.HoraActual != 0)
            return;

        if (DineroPendiente <= 0)
            return;

        string diaActual = ControladorTiempo.DiaActual;

        // Ya se entregó hoy
        if (DiaDineroOtorgado == diaActual)
            return;

        // 🔹 GUARDAR EL VALOR ANTES DE RESET
        int dineroEntregado = DineroPendiente;

        int dineroActual = PlayerPrefs.GetInt(PREF_DINERO, 0);
        dineroActual += dineroEntregado;

        PlayerPrefs.SetInt(PREF_DINERO, dineroActual);

        // Reset correcto
        DineroPendiente = 0;
        PlayerPrefs.SetInt(PREF_DINERO_PENDIENTE, 0);

        DiaDineroOtorgado = diaActual;
        PlayerPrefs.SetString(PREF_DIA_DINERO_OTORGADO, diaActual);

        PlayerPrefs.Save();

        // ✅ Feedback visual correcto
        if (DineroText != null)
            DineroText.text = "+$" + dineroEntregado.ToString("N0");

        DineroAnimator?.Play("Desaparecer");
        dineroAudioSource?.Play();
    }


    // =========================
    // DINERO INMEDIATO (SE CONSERVA)
    // =========================
    public void AgregarDinero(int cantidad)
    {
        if (cantidad <= 0) return;

        int dineroActual = PlayerPrefs.GetInt(PREF_DINERO, 0);
        dineroActual += cantidad;

        PlayerPrefs.SetInt(PREF_DINERO, dineroActual);
        PlayerPrefs.Save();

        if (DineroText != null)
            DineroText.text = "+$" + cantidad.ToString("N0");

        if (DineroAnimator != null &&
            !DineroAnimator.GetCurrentAnimatorStateInfo(0).IsName("Desaparecer"))
        {
            DineroAnimator.Play("Desaparecer");
            dineroAudioSource?.Play();
        }
    }

    // =========================
    // XP
    // =========================
    public void AgregarExperiencia(int cantidadXP)
    {
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
    // RECOMPENSAS
    // =========================
    private void ProcesarRecompensasSingleton()
    {
        if (Singleton == null)
            return;

        if (Singleton.ObjetosRecompensa == null ||
            Singleton.CantidadesRecompensa == null)
            return;

        int count = Mathf.Min(
            Singleton.ObjetosRecompensa.Count,
            Singleton.CantidadesRecompensa.Count
        );

        if (count == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            Scr_CreadorObjetos obj = Singleton.ObjetosRecompensa[i];
            int cant = Singleton.CantidadesRecompensa[i];

            if (obj == null || cant <= 0)
                continue;

            if (Inventario == null)
            {
                Debug.LogError("Inventario es NULL en Scr_ObjetosAgregados");
                return;
            }

            int cantidadAgregada = Inventario.AgregarObjeto(cant, obj.Nombre);

            // SI NO SE AGREGÓ NADA, NO SE CREA UI POSITIVA
            if (cantidadAgregada <= 0)
            {
                Lista.Add(obj);
                Cantidades.Add(cant);
                FueExcedente.Add(true);
            }
            else
            {
                Lista.Add(obj);
                Cantidades.Add(cantidadAgregada);
                FueExcedente.Add(false);
            }

            if (Tiempo != null && Lista.Count - 1 < Tiempo.Length)
                Tiempo[Lista.Count - 1] = 2f;


        }

        Singleton.ObjetosRecompensa.Clear();
        Singleton.CantidadesRecompensa.Clear();
    }


    // =========================
    // VISUAL
    // =========================
    private void MostrarObjetosEnCanvas()
    {
        for (int i = 0; i < Iconos.Length; i++)
        {
            var img = Iconos[i].GetComponent<Image>();
            TextMeshProUGUI txt = null;

            if (Iconos[i].transform.childCount > 0)
                txt = Iconos[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            if (i < Lista.Count)
            {
                var item = Lista[i];
                int cantidad = (i < Cantidades.Count) ? Cantidades[i] : 0;

                bool excedente = (i < FueExcedente.Count) && FueExcedente[i];

                // Sprite
                img.sprite = item.Icono;

                // TEXTO + COLOR
                if (txt != null)
                {
                    if (excedente)
                    {
                        txt.text = "-" + cantidad;
                        txt.color = Color.red;
                    }
                    else
                    {
                        txt.text = "+" + cantidad;
                        txt.color = Color.white;
                    }
                }

                // Fade
                float alpha = Mathf.Clamp01(Tiempo[i] / 2f);
                img.color = new Color(1f, 1f, 1f, alpha);

                if (txt != null)
                {
                    Color baseColor = excedente ? Color.red : Color.white;
                    txt.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                }

                // XP SOLO SI NO FUE EXCEDENTE
                if (!excedente &&
                    !xpOtorgada.Contains(i) &&
                    item.XPRecolecta > 0)
                {
                    AgregarExperiencia(item.XPRecolecta * cantidad);
                    xpOtorgada.Add(i);
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

        // Limpieza de seguridad
        while (FueExcedente.Count > Lista.Count)
            FueExcedente.RemoveAt(FueExcedente.Count - 1);

        xpOtorgada.RemoveWhere(i => i >= Lista.Count);
    }


    private void ActualizarTimers()
    {
        for (int i = 0; i < Tiempo.Length; i++)
        {
            if (i >= Lista.Count) continue;

            Tiempo[i] -= Time.deltaTime;
            if (Tiempo[i] < 0f) Tiempo[i] = 0f;
        }
    }

    // =========================
    // ACUMULAR DINERO DIFERIDO
    // =========================
    public void AgregarDineroPendiente(int cantidad)
    {
        if (cantidad <= 0)
            return;

        DineroPendiente += cantidad;

        PlayerPrefs.SetInt(PREF_DINERO_PENDIENTE, DineroPendiente);
        PlayerPrefs.Save();
    }

}
