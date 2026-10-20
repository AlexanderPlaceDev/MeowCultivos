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

    // ================= CATEGORÍA DEL RECURSO =================

    [Header("Categoría del Recurso")]
    [Tooltip("0 = Automático (usa etapaActual); 1 = Chico; 2 = Mediano; 3 = Grande")]
    [Min(0)]
    public int CategoriaTamanoRecurso = 0;

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
        if (etapaActual <= 0 || VidaActual <= 0 || cantidad <= 0)
            return;

        // Los recursos que usan hacha o pico calculan el daño según la herramienta.
        // Los demás recursos conservan el daño que recibían originalmente.
        if (GetUsaHacha() || GetUsaPico())
        {
            int danioHerramienta = CalcularDanioHerramienta();

            // Si no tiene la herramienta necesaria para esta etapa, no causa daño.
            if (danioHerramienta <= 0)
                return;

            cantidad *= danioHerramienta;
        }

        VidaActual = Mathf.Max(VidaActual - cantidad, 0);

        // MUERTE INMEDIATA
        if (VidaActual == 0)
            ProcesarMuerte();
    }

    private int CalcularDanioHerramienta()
    {
        // La categoría manual permite que un recurso con una sola etapa real
        // se considere chico, mediano o grande para calcular el daño.
        // Si es 0, se usa la etapa real (por ejemplo, árboles que crecen).
        int categoria = CategoriaTamanoRecurso > 0
            ? CategoriaTamanoRecurso
            : etapaActual;

        // Solo hay progresión definida para las categorías 1, 2 y 3.
        if (categoria < 1 || categoria > 3)
            return 1;

        int nivelHerramienta = 0; // 0 = ninguna, 1 = madera, 2 = piedra, 3 = oro, 4 = extraterrita

        if (GetUsaHacha())
        {
            if (PlayerPrefs.GetString("Habilidad:Hacha Extraterrita", "No") == "Si")
                nivelHerramienta = 4;
            else if (PlayerPrefs.GetString("Habilidad:Hacha Oro", "No") == "Si")
                nivelHerramienta = 3;
            else if (PlayerPrefs.GetString("Habilidad:Hacha Piedra", "No") == "Si")
                nivelHerramienta = 2;
            else if (PlayerPrefs.GetString("Habilidad:Hacha Madera", "No") == "Si")
                nivelHerramienta = 1;
        }

        if (GetUsaPico())
        {
            int nivelPico = 0;

            if (PlayerPrefs.GetString("Habilidad:Pico Extraterrita", "No") == "Si")
                nivelPico = 4;
            else if (PlayerPrefs.GetString("Habilidad:Pico Oro", "No") == "Si")
                nivelPico = 3;
            else if (PlayerPrefs.GetString("Habilidad:Pico Piedra", "No") == "Si")
                nivelPico = 2;
            else if (PlayerPrefs.GetString("Habilidad:Pico Madera", "No") == "Si")
                nivelPico = 1;

            nivelHerramienta = Mathf.Max(nivelHerramienta, nivelPico);
        }

        // Golpes requeridos por etapa y nivel de herramienta.
        int golpesNecesarios = 0;

        switch (categoria)
        {
            case 1: // Chico: madera 3, piedra 2, oro/extraterrita 1
                if (nivelHerramienta == 1) golpesNecesarios = 3;
                else if (nivelHerramienta == 2) golpesNecesarios = 2;
                else if (nivelHerramienta >= 3) golpesNecesarios = 1;
                break;

            case 2: // Mediano: piedra 3, oro 2, extraterrita 1
                if (nivelHerramienta == 2) golpesNecesarios = 3;
                else if (nivelHerramienta == 3) golpesNecesarios = 2;
                else if (nivelHerramienta >= 4) golpesNecesarios = 1;
                break;

            case 3: // Grande: oro 3, extraterrita 2
                if (nivelHerramienta == 3) golpesNecesarios = 3;
                else if (nivelHerramienta >= 4) golpesNecesarios = 2;
                break;
        }

        if (golpesNecesarios <= 0)
            return 0;

        // Calcula el daño necesario para alcanzar la cantidad de golpes deseada.
        // Ejemplo: 9 de vida / 2 golpes = 5 de daño por golpe (redondeo hacia arriba).
        int vidaMaximaRecurso = VidaMaxima > 0
            ? VidaMaxima
            : VidaBase * etapaActual;
        return Mathf.CeilToInt((float)vidaMaximaRecurso / golpesNecesarios);
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

        inventario.AgregarObjeto(
            objeto.Nombre,
            cantidad,
            mostrarUI: true,
            darXP: true
        );
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