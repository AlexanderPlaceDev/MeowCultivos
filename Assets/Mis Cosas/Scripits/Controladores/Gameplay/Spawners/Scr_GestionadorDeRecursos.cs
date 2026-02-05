using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Scr_GestionadorDeRecursos : MonoBehaviour
{
    // ================= INTERACCIÓN =================

    [Header("Interacción")]
    [SerializeField] private float radioInteraccion = 10f;
    [SerializeField] private float intervaloChequeo = 0.15f;
    [SerializeField] private bool UsaHacha = true;

    private Transform gata;
    private GameObject herramienta;
    private Scr_Recurso arbolActivo;
    private float timerInteraccion;

    // ================= RESPAWN =================

    [Header("Respawn")]
    [SerializeField] private float tiempoMinRespawn = 5f;
    [SerializeField] private float tiempoMaxRespawn = 10f;

    // ================= CRECIMIENTO =================

    [Header("Crecimiento")]
    [Tooltip("Distribución porcentual por etapa (ej: 20,35,45)")]
    [SerializeField] private int[] ProbabilidadesCrecimiento;
    [SerializeField] private float tiempoMinCrecimiento = 10f;
    [SerializeField] private float tiempoMaxCrecimiento = 20f;

    private List<Scr_Recurso> Recursos = new();
    private Dictionary<Scr_Recurso, Coroutine> procesos = new();

    // ===== persistencia de tiempo =====
    private Dictionary<Scr_Recurso, float> tiempoObjetivo = new();
    private Dictionary<Scr_Recurso, float> tiempoInicio = new();

    // ================= UNITY =================

    private void Awake()
    {
        foreach (Transform hijo in transform)
        {
            var sp = hijo.GetComponent<Scr_Recurso>();
            if (sp != null)
                Recursos.Add(sp);
        }

        gata = GameObject.Find("Gata").transform;

        herramienta = gata
            .GetChild(0).GetChild(0).GetChild(0).GetChild(1)
            .GetChild(0).GetChild(1).GetChild(0).GetChild(0)
            .GetChild(0).GetChild(2).gameObject;

        herramienta.SetActive(false);
    }

    private void Start()
    {
        InicializarPrimerSpawn();

        foreach (var r in Recursos)
        {
            if (r.etapaActual == 0)
                IniciarRespawn(r);
            else
                IniciarCrecimiento(r);
        }
    }

    private void Update()
    {
        timerInteraccion -= Time.deltaTime;
        if (timerInteraccion <= 0f)
        {
            ActualizarArbolActivo();
            timerInteraccion = intervaloChequeo;
        }
    }

    // ================= INTERACCIÓN =================

    private void ActualizarArbolActivo()
    {
        Scr_Recurso candidato = null;
        float distMin = radioInteraccion * radioInteraccion;
        Vector3 posGata = gata.position;

        foreach (var arbol in Recursos)
        {
            if (arbol.etapaActual <= 0)
                continue;

            float sqrDist = (arbol.transform.position - posGata).sqrMagnitude;
            if (sqrDist <= distMin)
            {
                distMin = sqrDist;
                candidato = arbol;
            }
        }

        if (candidato != arbolActivo)
        {
            DesactivarInteraccion();
            arbolActivo = candidato;

            if (arbolActivo != null)
                ActivarInteraccion(arbolActivo);
        }
    }

    private void ActivarInteraccion(Scr_Recurso Recurso)
    {
        string habilidadNecesaria = Recurso.GetHabilidadRequerida();
        if (!JugadorTieneHabilidad(habilidadNecesaria))
        {
            DesactivarInteraccion();
            return;
        }

        Transform ui = gata.GetChild(3);
        ui.gameObject.SetActive(true);

        ui.GetChild(0).GetChild(0)
            .GetComponent<TextMeshProUGUI>().text = Recurso.tecla;

        ui.GetChild(0)
            .GetComponent<Image>().sprite = Recurso.teclaIcono;

        ui.GetChild(1)
            .GetComponent<Image>().sprite = Recurso.icono;

        herramienta.SetActive(true);

        herramienta.transform.GetChild(0).gameObject.SetActive(Recurso.GetUsaHacha());
        herramienta.transform.GetChild(1).gameObject.SetActive(Recurso.GetUsaPico());

        var anim = gata.GetComponent<Scr_ControladorAnimacionesGata>();
        anim.PuedeTalar = true;
        anim.HabilidadUsando = habilidadNecesaria;
    }

    private void DesactivarInteraccion()
    {
        gata.GetChild(3).gameObject.SetActive(false);
        herramienta.SetActive(false);

        var anim = gata.GetComponent<Scr_ControladorAnimacionesGata>();
        anim.PuedeTalar = false;
        anim.HabilidadUsando = "";

        arbolActivo = null;
    }

    private bool JugadorTieneHabilidad(string habilidad)
    {
        if (string.IsNullOrEmpty(habilidad))
            return true;

        return PlayerPrefs.GetString("Habilidad:" + habilidad, "No") == "Si";
    }

    // ================= MUERTE =================

    public void OnSpawnerMuerto(Scr_Recurso Recurso)
    {
        if (Recurso == null)
            return;

        DetenerProceso(Recurso);
        IniciarRespawn(Recurso);
    }

    // ================= RESPAWN =================

    private void IniciarRespawn(Scr_Recurso Recurso)
    {
        DetenerProceso(Recurso);

        float tiempo = CargarTiempo(KeyRespawn(Recurso));
        if (tiempo <= 0f)
            tiempo = Random.Range(tiempoMinRespawn, tiempoMaxRespawn);

        tiempoObjetivo[Recurso] = tiempo;
        tiempoInicio[Recurso] = Time.time;

        procesos[Recurso] = StartCoroutine(RespawnCoroutine(Recurso));
    }

    private IEnumerator RespawnCoroutine(Scr_Recurso Recurso)
    {
        yield return new WaitForSeconds(tiempoObjetivo[Recurso]);

        PlayerPrefs.DeleteKey(KeyRespawn(Recurso));

        Recurso.AplicarEtapa(1);

        if (Recurso.GetCantidadEtapas() > 1)
            IniciarCrecimiento(Recurso);

        procesos.Remove(Recurso);
    }

    // ================= CRECIMIENTO =================

    private void IniciarCrecimiento(Scr_Recurso Recurso)
    {
        DetenerProceso(Recurso);

        float tiempo = CargarTiempo(KeyCrecimiento(Recurso));
        if (tiempo <= 0f)
            tiempo = Random.Range(tiempoMinCrecimiento, tiempoMaxCrecimiento);

        tiempoObjetivo[Recurso] = tiempo;
        tiempoInicio[Recurso] = Time.time;

        procesos[Recurso] = StartCoroutine(CrecimientoCoroutine(Recurso));
    }

    private IEnumerator CrecimientoCoroutine(Scr_Recurso Recurso)
    {
        int maxEtapa = Recurso.GetCantidadEtapas();

        while (Recurso != null && Recurso.etapaActual > 0)
        {
            if (Recurso.etapaActual >= maxEtapa)
                break;

            yield return new WaitForSeconds(tiempoObjetivo[Recurso]);

            PlayerPrefs.DeleteKey(KeyCrecimiento(Recurso));

            int siguiente = Recurso.etapaActual + 1;
            if (PermitirEtapa(siguiente))
                Recurso.AplicarEtapa(siguiente);

            tiempoObjetivo[Recurso] =
                Random.Range(tiempoMinCrecimiento, tiempoMaxCrecimiento);
            tiempoInicio[Recurso] = Time.time;
        }

        procesos.Remove(Recurso);
    }

    // ================= GUARDADO =================

    private void DetenerProceso(Scr_Recurso Recurso)
    {
        // ⛔ Si no hay datos de tiempo, no hay nada que guardar
        if (!tiempoObjetivo.ContainsKey(Recurso) || !tiempoInicio.ContainsKey(Recurso))
            return;

        float transcurrido = Time.time - tiempoInicio[Recurso];
        float restante = Mathf.Max(0f, tiempoObjetivo[Recurso] - transcurrido);

        if (Recurso.etapaActual == 0)
            PlayerPrefs.SetFloat(KeyRespawn(Recurso), restante);
        else
            PlayerPrefs.SetFloat(KeyCrecimiento(Recurso), restante);

        // 🟢 StopCoroutine SOLO si existe y no es null
        if (procesos.TryGetValue(Recurso, out var c) && c != null)
            StopCoroutine(c);

        procesos.Remove(Recurso);
    }


    private void OnApplicationQuit()
    {
        foreach (var r in new List<Scr_Recurso>(procesos.Keys))
            DetenerProceso(r);

        PlayerPrefs.Save();
    }

    // ================= KEYS =================

    private string KeyRespawn(Scr_Recurso r) =>
        "Spawner_RespawnTime_" + r.gameObject.name;

    private string KeyCrecimiento(Scr_Recurso r) =>
        "Spawner_GrowTime_" + r.gameObject.name;

    private float CargarTiempo(string key)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : 0f;
    }

    // ================= PROBABILIDADES =================

    private bool PermitirEtapa(int etapaDestino)
    {
        int index = etapaDestino - 1;
        if (index < 0 || index >= ProbabilidadesCrecimiento.Length)
            return false;

        if (ProbabilidadesCrecimiento[index] <= 0)
            return true;

        int vivos = ContarVivos();
        if (vivos == 0)
            return false;

        int enDestino = ContarEnEtapa(etapaDestino);

        int objetivo = Mathf.RoundToInt(
            vivos * (ProbabilidadesCrecimiento[index] / 100f)
        );

        return enDestino < objetivo;
    }

    private int ContarVivos()
    {
        int vivos = 0;
        foreach (var r in Recursos)
            if (r.etapaActual > 0)
                vivos++;
        return vivos;
    }

    private int ContarEnEtapa(int etapa)
    {
        int c = 0;
        foreach (var r in Recursos)
            if (r.etapaActual == etapa)
                c++;
        return c;
    }

    // ================= INICIAL =================

    private void InicializarPrimerSpawn()
    {
        if (ProbabilidadesCrecimiento == null || ProbabilidadesCrecimiento.Length == 0)
            return;

        List<Scr_Recurso> sinDatos = new();

        foreach (var r in Recursos)
            if (!r.TieneDatosGuardados)
                sinDatos.Add(r);

        int total = sinDatos.Count;
        if (total == 0)
            return;

        int etapas = ProbabilidadesCrecimiento.Length;
        int[] cupos = new int[etapas];

        int asignados = 0;

        for (int i = 0; i < etapas; i++)
        {
            cupos[i] = Mathf.FloorToInt(
                total * (ProbabilidadesCrecimiento[i] / 100f)
            );
            asignados += cupos[i];
        }

        int sobrantes = total - asignados;
        int idx = 0;

        while (sobrantes > 0)
        {
            cupos[idx]++;
            sobrantes--;
            idx = (idx + 1) % etapas;
        }

        for (int etapa = 0; etapa < etapas; etapa++)
        {
            for (int i = 0; i < cupos[etapa] && sinDatos.Count > 0; i++)
            {
                int index = Random.Range(0, sinDatos.Count);
                sinDatos[index].AplicarEtapa(etapa + 1);
                sinDatos.RemoveAt(index);
            }
        }
    }

#if UNITY_EDITOR
    [Header("DEBUG TIEMPOS (solo editor)")]
    [SerializeField] private List<string> debugTiempos = new();
#endif

    private void LateUpdate()
    {
#if UNITY_EDITOR
        debugTiempos.Clear();

        foreach (var kvp in tiempoObjetivo)
        {
            var r = kvp.Key;

            if (!tiempoInicio.ContainsKey(r))
                continue;

            float restante = Mathf.Max(
                0f,
                tiempoObjetivo[r] - (Time.time - tiempoInicio[r])
            );

            string tipo = r.etapaActual == 0 ? "RESPAWN" : "CRECIMIENTO";

            debugTiempos.Add(
                $"{r.name} | {tipo} | {restante:0.0}s"
            );
        }
#endif
    }

}
