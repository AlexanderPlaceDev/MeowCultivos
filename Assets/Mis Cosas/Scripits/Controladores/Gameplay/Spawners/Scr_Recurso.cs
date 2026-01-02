using UnityEngine;

public class Scr_Recurso : MonoBehaviour
{
    // ================= UI / TOOL =================

    [Header("UI / Tool")]
    public string tecla;
    public Sprite teclaIcono;
    public Sprite icono;
    [Header("Habilidad por Etapa")]
    [SerializeField] private string[] habilidadesPorEtapa;
    [SerializeField] private bool[] usaHachaPorEtapa;
    [SerializeField] private bool[] usaPicoPorEtapa;

    [SerializeField] private Scr_CreadorObjetos ItemQueDa;
    [SerializeField] private int[] CantidadesQueDa;

    // ================= VIDA =================

    [Header("Vida")]
    public int VidaBase = 3;
    public int VidaActual;
    public int VidaMaxima;

    // ================= ETAPAS =================

    [Header("Etapas")]
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private float tiempoVerificacion = 0.2f;

    public int etapaActual; // 0 = muerto

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private int etapaAnterior = -1;
    private int vidaAnterior = -1;
    private float timer;
    private bool muerteNotificada;

    private Scr_GestionadorDeRecursos controlador;

    private string KeyEtapa => "Spawner_Etapa_" + gameObject.name;
    private string KeyVida => "Spawner_Vida_" + gameObject.name;

    public bool TieneDatosGuardados =>
        PlayerPrefs.HasKey(KeyEtapa) && PlayerPrefs.HasKey(KeyVida);

    // ================= UNITY =================

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        controlador = GetComponentInParent<Scr_GestionadorDeRecursos>();

        if (TieneDatosGuardados)
            CargarEstado();

        ActualizarVisual(true);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            VerificarCambios();
            timer = tiempoVerificacion;
        }
    }

    // ================= DAÑO =================

    public void RecibirDanio(int cantidad)
    {
        if (etapaActual <= 0 || VidaActual <= 0)
            return;

        VidaActual = Mathf.Max(VidaActual - cantidad, 0);

        // 🔴 MUERTE INMEDIATA
        if (VidaActual == 0)
            ProcesarMuerte();
    }

    private void ProcesarMuerte()
    {
        if (etapaActual == 0)
            return;

        transform.GetChild(0).GetComponent<ParticleSystem>().Play();

        //Dar Item
        int min = CantidadesQueDa[0] * etapaActual;
        int max = CantidadesQueDa[1] * etapaActual;

        int cantidad = Random.Range(min, max + 1);

        ActualizarInventario(cantidad, ItemQueDa);

        etapaActual = 0;
        if (!muerteNotificada && controlador != null)
        {
            muerteNotificada = true;
            controlador.OnSpawnerMuerto(this);
        }

        ActualizarVisual(false);
        GuardarEstado();
    }

    void ActualizarInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        if (cantidad <= 0 || objeto == null)
            return;

        Scr_Inventario inventario = GameObject.Find("Gata")
            .transform.GetChild(7)
            .GetComponent<Scr_Inventario>();

        Scr_ObjetosAgregados controlador = GameObject.Find("Canvas")
            .transform.GetChild(4)
            .GetComponent<Scr_ObjetosAgregados>();

        // 1️⃣ Inventario decide cuánto entra realmente
        int cantidadAgregada = inventario.AgregarObjeto(cantidad, objeto.Nombre);

        // 2️⃣ UI refleja el resultado
        controlador.Lista.Add(objeto);

        if (cantidadAgregada > 0)
        {
            controlador.Cantidades.Add(cantidadAgregada);
            controlador.FueExcedente.Add(false);
        }
        else
        {
            // Inventario lleno
            controlador.Cantidades.Add(cantidad);
            controlador.FueExcedente.Add(true);
        }

        if (controlador.Tiempo != null &&
            controlador.Lista.Count - 1 < controlador.Tiempo.Length)
        {
            controlador.Tiempo[controlador.Lista.Count - 1] = 2f;
        }
    }



    // ================= CORE =================

    private void VerificarCambios()
    {
        // Vida → Etapa (ÚNICA dependencia permitida)
        if (VidaActual <= 0 && etapaActual != 0)
        {
            etapaActual = 0;

            if (!muerteNotificada && controlador != null)
            {
                muerteNotificada = true;
                controlador.OnSpawnerMuerto(this);
            }
        }

        if (etapaActual == etapaAnterior && VidaActual == vidaAnterior)
            return;

        ActualizarVisual(false);
        GuardarEstado();
    }

    // ================= VISUAL =================

    private void ActualizarVisual(bool forzado)
    {
        if (!forzado && etapaActual == etapaAnterior)
            return;

        if (etapaActual == 0)
        {
            meshRenderer.enabled = false;
            meshCollider.enabled = false;
        }
        else
        {
            int index = etapaActual - 1;
            meshFilter.sharedMesh = meshes[index];
            meshCollider.sharedMesh = meshes[index];
            meshRenderer.enabled = true;
            meshCollider.enabled = true;
        }

        etapaAnterior = etapaActual;
        vidaAnterior = VidaActual;
    }

    // ================= SAVE / LOAD =================

    private void GuardarEstado()
    {
        PlayerPrefs.SetInt(KeyEtapa, etapaActual);
        PlayerPrefs.SetInt(KeyVida, VidaActual);
    }

    private void CargarEstado()
    {
        etapaActual = PlayerPrefs.GetInt(KeyEtapa, etapaActual);

        if (etapaActual > 0)
        {
            VidaMaxima = VidaBase * etapaActual;
            VidaActual = Mathf.Min(
                PlayerPrefs.GetInt(KeyVida, VidaMaxima),
                VidaMaxima
            );
        }
        else
        {
            VidaActual = 0;
            VidaMaxima = 0;
        }
    }

    // ================= ETAPAS =================

    public int GetCantidadEtapas()
    {
        return meshes.Length;
    }

    // 🔹 SOLO el controlador llama esto
    public void AplicarEtapa(int nuevaEtapa)
    {
        etapaActual = Mathf.Clamp(nuevaEtapa, 0, meshes.Length);

        if (etapaActual == 0)
        {
            VidaActual = 0;
            VidaMaxima = 0;
            return;
        }

        VidaMaxima = VidaBase * etapaActual;
        VidaActual = VidaMaxima;
        muerteNotificada = false;
    }

    private void OnApplicationQuit()
    {
        GuardarEstado();
    }

    public string GetHabilidadRequerida()
    {
        int index = etapaActual - 1;
        if (index < 0 || index >= habilidadesPorEtapa.Length)
            return "";

        return habilidadesPorEtapa[index];
    }

    public bool GetUsaHacha()
    {
        int index = etapaActual - 1;
        return index >= 0 && index < usaHachaPorEtapa.Length && usaHachaPorEtapa[index];
    }

    public bool GetUsaPico()
    {
        int index = etapaActual - 1;
        return index >= 0 && index < usaPicoPorEtapa.Length && usaPicoPorEtapa[index];
    }

}
