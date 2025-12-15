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
    private bool primerSpawnCompletado = false;


    // ================= UNITY =================

    private void Awake()
    {
        foreach (Transform hijo in transform)
        {
            var sp = hijo.GetComponent<Scr_Recurso>();
            if (sp != null)
                Recursos.Add(sp);
        }

        // Gata
        gata = GameObject.Find("Gata").transform;

        // Herramienta (ruta original que usabas)
        herramienta = gata
            .GetChild(0).GetChild(0).GetChild(0).GetChild(1)
            .GetChild(0).GetChild(1).GetChild(0).GetChild(0)
            .GetChild(0).GetChild(2).gameObject;

        herramienta.SetActive(false);
    }

    private void Start()
    {
        InicializarPrimerSpawn();

        foreach (var Recurso in Recursos)
        {
            if (Recurso.etapaActual == 0)
                IniciarRespawn(Recurso);
            else
                IniciarCrecimiento(Recurso);
        }
    }

    private void Update()
    {
        // -------- INTERACCIÓN --------
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
        bool puedeUsar = JugadorTieneHabilidad(habilidadNecesaria);

        // ❌ SI NO TIENE HABILIDAD → NO HAY INTERACCIÓN VISUAL
        if (!puedeUsar)
        {
            DesactivarInteraccion();
            return;
        }

        // ===== UI =====
        Transform ui = gata.GetChild(3);
        ui.gameObject.SetActive(true);

        ui.GetChild(0).GetChild(0)
            .GetComponent<TextMeshProUGUI>().text = Recurso.tecla;

        ui.GetChild(0)
            .GetComponent<Image>().sprite = Recurso.teclaIcono;

        ui.GetChild(1)
            .GetComponent<Image>().sprite = Recurso.icono;

        // ===== HERRAMIENTA =====
        herramienta.SetActive(true);

        herramienta.transform.GetChild(0).gameObject.SetActive(
            Recurso.GetUsaHacha()
        );
        herramienta.transform.GetChild(1).gameObject.SetActive(
            Recurso.GetUsaPico()
        );

        // ===== ANIMACIÓN =====
        var anim = gata.GetComponent<Scr_ControladorAnimacionesGata>();
        anim.PuedeTalar = true;
        anim.HabilidadUsando = habilidadNecesaria;
    }


    private bool JugadorTieneHabilidad(string habilidad)
    {
        if (string.IsNullOrEmpty(habilidad))
            return true; // Etapas sin requisito

        return PlayerPrefs.GetString("Habilidad:" + habilidad, "No") == "Si";
    }



    private void DesactivarInteraccion()
    {
        // UI off
        gata.GetChild(3).gameObject.SetActive(false);

        // Herramienta off
        herramienta.SetActive(false);

        // Reset animación
        var anim = gata.GetComponent<Scr_ControladorAnimacionesGata>();
        anim.PuedeTalar = false;
        anim.HabilidadUsando = "";

        arbolActivo = null;
    }



    // ================= INICIAL =================

    private void InicializarPrimerSpawn()
    {
        if (ProbabilidadesCrecimiento == null || ProbabilidadesCrecimiento.Length == 0)
        {
            Debug.LogError("ProbabilidadesCrecimiento no está configurado. " +gameObject.name);
            return;
        }

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

        // 1️⃣ Cupos base
        for (int i = 0; i < etapas; i++)
        {
            cupos[i] = Mathf.FloorToInt(
                total * (ProbabilidadesCrecimiento[i] / 100f)
            );
            asignados += cupos[i];
        }

        // 2️⃣ Repartir sobrantes (solo si hay etapas)
        int sobrantes = total - asignados;
        int idx = 0;

        while (sobrantes > 0)
        {
            cupos[idx]++;
            sobrantes--;
            idx++;

            if (idx >= etapas)
                idx = 0;
        }

        // 3️⃣ Asignar recursos
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
        procesos[Recurso] = StartCoroutine(RespawnCoroutine(Recurso));
    }

    private IEnumerator RespawnCoroutine(Scr_Recurso Recurso)
    {
        yield return new WaitForSeconds(
            Random.Range(tiempoMinRespawn, tiempoMaxRespawn)
        );

        Recurso.AplicarEtapa(1);

        // 🔴 Solo iniciar crecimiento si hay más de una etapa
        if (Recurso.GetCantidadEtapas() > 1)
            IniciarCrecimiento(Recurso);

        procesos.Remove(Recurso);
    }



    // ================= CRECIMIENTO =================

    private void IniciarCrecimiento(Scr_Recurso Recurso)
    {
        DetenerProceso(Recurso);
        procesos[Recurso] = StartCoroutine(CrecimientoCoroutine(Recurso));
    }

    private IEnumerator CrecimientoCoroutine(Scr_Recurso Recurso)
    {
        int maxEtapa = Recurso.GetCantidadEtapas();

        while (Recurso != null && Recurso.etapaActual > 0)
        {
            if (Recurso.etapaActual >= maxEtapa)
                break;

            yield return new WaitForSeconds(
                Random.Range(tiempoMinCrecimiento, tiempoMaxCrecimiento)
            );

            int siguiente = Recurso.etapaActual + 1;

            if (PermitirEtapa(siguiente))
                Recurso.AplicarEtapa(siguiente);
        }

        procesos.Remove(Recurso);
    }


    // ================= PROBABILIDADES =================

    private bool PermitirEtapa(int etapaDestino)
    {
        int index = etapaDestino - 1;
        if (index < 0 || index >= ProbabilidadesCrecimiento.Length)
            return false;

        // 🔑 Si esta etapa no tiene peso poblacional, no bloquear
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


    // ================= UTIL =================

    private void DetenerProceso(Scr_Recurso Recurso)
    {
        if (!procesos.TryGetValue(Recurso, out var c))
            return;

        if (c != null)
            StopCoroutine(c);

        procesos.Remove(Recurso);
    }

}
