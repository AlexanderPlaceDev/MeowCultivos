using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ObjetosAgregados : MonoBehaviour
{
    public List<Scr_CreadorObjetos> Lista = new List<Scr_CreadorObjetos>();
    public List<int> Cantidades = new List<int>();
    [SerializeField] GameObject[] Iconos;
    public float[] Tiempo; // inicializado en Start en función de Iconos.Length

    [SerializeField] private Scr_Inventario Inventario;

    [SerializeField] private GameObject canvasXP;
    [SerializeField] private GameObject canvasDinero;
    private TextMeshProUGUI XPText;
    private TextMeshProUGUI DineroText;
    private Animator XPAnimator;
    private Animator DineroAnimator;
    private AudioSource dineroAudioSource;
    private AudioSource xpAudioSource;
    private int xpPendiente = 0;

    // Guarda índices de los objetos que ya dieron XP
    private HashSet<int> xpOtorgada = new HashSet<int>();
    Scr_DatosSingletonBatalla Singleton;


    void Start()
    {
        Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();


        // Asegurar que Tiempo tenga el tamaño correcto
        if (Iconos != null)
        {
            if (Tiempo == null || Tiempo.Length != Iconos.Length)
            {
                Tiempo = new float[Iconos.Length];
                for (int t = 0; t < Tiempo.Length; t++) Tiempo[t] = 2f; // valor por defecto
            }
        }

        if (Inventario == null)
        {
            Inventario = FindObjectOfType<Scr_Inventario>();
            if (Inventario == null)
                Debug.LogWarning("No se encontró Scr_Inventario en la escena. No se actualizará el inventario al consumir objetos.");
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

        if (Singleton != null)
            ProcesarRecompensasSingleton();
        MostrarObjetosEnCanvas();

        if (Iconos == null || Iconos.Length == 0) return;

        List<int> expiradosIndices = new List<int>();
        int maxSlots = Iconos.Length;

        // 1️⃣ Recorremos slots visibles
        for (int i = 0; i < maxSlots; i++)
        {
            if (i >= Lista.Count || Lista[i] == null)
            {
                // limpiar visual
                var imgComp = Iconos[i].GetComponent<Image>();
                if (imgComp != null) imgComp.sprite = null;
                var txtChild = Iconos[i].transform.childCount > 0 ? Iconos[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>() : null;
                if (txtChild != null) txtChild.text = "";
                Tiempo[i] = 2f;
                continue;
            }

            // Reducir tiempo si no ha llegado a 0 aún
            if (Tiempo[i] > 0f)
            {
                Tiempo[i] -= Time.deltaTime;
                if (Tiempo[i] < 0f) Tiempo[i] = 0f; // clamp a 0 exacto
            }

            float alpha = Mathf.Clamp01(Tiempo[i] / 2f);
            var image = Iconos[i].GetComponent<Image>();
            if (image != null) image.color = new Color(1f, 1f, 1f, alpha);
            var cantidadText = Iconos[i].transform.childCount > 0 ? Iconos[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>() : null;
            if (cantidadText != null) cantidadText.color = new Color(1f, 1f, 1f, alpha);

            // Si ya llegó a cero, registrar índice para eliminar
            if (Tiempo[i] == 0f)
            {
                expiradosIndices.Add(i);
            }
        }

        // 2️⃣ Si hay expirados, procesarlos una sola vez
        if (expiradosIndices.Count > 0)
        {
            List<(Scr_CreadorObjetos item, int cantidad)> expirados = new List<(Scr_CreadorObjetos, int)>();

            // Guardar snapshot de los ítems expirados
            foreach (int idx in expiradosIndices)
            {
                if (idx < Lista.Count)
                {
                    var item = Lista[idx];
                    int cant = idx < Cantidades.Count ? Cantidades[idx] : 0;
                    expirados.Add((item, cant));
                }
            }

            // 🔹 Eliminar los expirados de las listas ANTES de modificar el inventario
            expiradosIndices.Sort((a, b) => b.CompareTo(a)); // eliminar de atrás hacia adelante
            foreach (int idx in expiradosIndices)
            {
                if (idx < Lista.Count) Lista.RemoveAt(idx);
                if (idx < Cantidades.Count) Cantidades.RemoveAt(idx);

                // Reacomodar los tiempos visuales
                for (int t = idx; t < Tiempo.Length - 1; t++)
                    Tiempo[t] = Tiempo[t + 1];
                Tiempo[Tiempo.Length - 1] = 2f;
            }

            // 3️⃣ Agregar los expirados al inventario solo una vez
            foreach (var e in expirados)
            {
                if (Inventario != null && e.item != null)
                {
                    Inventario.AgregarObjeto(e.cantidad, e.item.Nombre);
                }
            }

        }
    }

    private void ProcesarRecompensasSingleton()
    {
        if (Singleton == null) return;

        var listaObjs = Singleton.ObjetosRecompensa;
        var listaCants = Singleton.CantidadesRecompensa;

        if (listaObjs == null || listaCants == null) return;
        if (listaObjs.Count == 0 || listaCants.Count == 0) return;

        // Asegurarse que tienen el mismo tamaño
        int cantidad = Mathf.Min(listaObjs.Count, listaCants.Count);
        if (cantidad == 0) return;

        for (int i = 0; i < cantidad; i++)
        {
            var obj = listaObjs[i];
            int cant = listaCants[i];

            if (obj == null || cant <= 0)
                continue;

            // 1️⃣ Agregar al inventario real
            Inventario?.AgregarObjeto(cant, obj.Nombre);

            // 2️⃣ Añadir al canvas (tu sistema actual)
            Lista.Add(obj);
            Cantidades.Add(cant);

            // Asignar un timer al nuevo ítem visual
            if (Tiempo != null && Lista.Count - 1 < Tiempo.Length)
                Tiempo[Lista.Count - 1] = 2f;
        }

        // 3️⃣ Limpiar completamente las listas del Singleton
        listaObjs.Clear();
        listaCants.Clear();
    }


    public void AgregarExperiencia(int cantidadXP)
    {
        xpPendiente += cantidadXP;

        int xpActual = PlayerPrefs.GetInt("XPActual", 0) + cantidadXP;
        PlayerPrefs.SetInt("XPActual", xpActual);

        XPText.text = "XP + " + xpPendiente;

        if (XPAnimator != null)
            XPAnimator.Play("Desaparecer");

        CancelInvoke(nameof(ResetXPVisual));
        Invoke(nameof(ResetXPVisual), 0.2f);
    }

    void ResetXPVisual()
    {
        xpPendiente = 0;
    }


    public void AgregarDinero(int cantidad)
    {
        if (canvasDinero == null) return;

        int dineroActual = PlayerPrefs.GetInt("Dinero", 0) + cantidad;
        PlayerPrefs.SetInt("Dinero", dineroActual);

        if (DineroText != null) DineroText.text = "+$" + cantidad.ToString("N0");

        if (DineroAnimator != null && !DineroAnimator.GetCurrentAnimatorStateInfo(0).IsName("Desaparecer"))
        {
            DineroAnimator.Play("Desaparecer");
            if (dineroAudioSource != null) dineroAudioSource.Play();
        }
    }

    private void MostrarObjetosEnCanvas()
    {
        for (int k = 0; k < Iconos.Length; k++)
        {
            var image = Iconos[k].GetComponent<Image>();
            var txt = Iconos[k].transform.childCount > 0
                ? Iconos[k].transform.GetChild(0).GetComponent<TextMeshProUGUI>()
                : null;

            if (k < Lista.Count && Lista[k] != null)
            {
                var item = Lista[k];
                int cantidad = Cantidades[k];

                image.sprite = item.Icono;
                if (txt != null) txt.text = "+" + cantidad;

                float alpha = Mathf.Clamp01(Tiempo[k] / 2f);
                image.color = new Color(1f, 1f, 1f, alpha);
                if (txt != null) txt.color = new Color(1f, 1f, 1f, alpha);

                // 🧩 Dar XP solo una vez por ítem visible
                if (!xpOtorgada.Contains(k) && item.XPRecolecta > 0)
                {
                    AgregarExperiencia(item.XPRecolecta * cantidad);
                    xpOtorgada.Add(k); // Marcar como otorgado
                }
            }
            else
            {
                if (image != null) image.sprite = null;
                if (txt != null) txt.text = "";
            }
        }

        // 🔹 Limpiar índices de XP otorgada que ya no están en lista
        xpOtorgada.RemoveWhere(i => i >= Lista.Count);
    }
}
