using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_ControladorOleadas : MonoBehaviour
{

    [Header("Barra Oleadas")]
    [SerializeField] GameObject[] Iconos;
    [SerializeField] Transform BarraSlider;
    [SerializeField] float TiempoEntreOleadas;
    [SerializeField] GameObject BotonOleada;
    [SerializeField] TextMeshProUGUI TextoCantidadEnemigos;
    private List<GameObject> OleadasActuales = new List<GameObject>();
    private GameObject FlechaOleadaActiva;
    private GameObject BotonAdelantarOleada;

    private float TiempoPresionAdelantar = 0f;
    private bool EstaPresionandoAdelantar = false;
    private float DuracionAdelantarOleada = 1.5f;

    private int CantEnemigosPorOleada;
    private float tiempoPresionE = 0f;
    private float duracionNecesaria = 1.5f;
    private bool estaPresionandoE = false;
    float ContTiempoEntreOleadas;
    List<GameObject> spawners = new List<GameObject>();
    public GameObject prefabEnemigo;

    [Header("Cuenta Regresiva")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip SonidoReloj;
    [SerializeField] private AudioClip SonidoPelea;
    private string ultimoTextoMostrado = "";



    private Scr_DatosSingletonBatalla singleton;


    public List<GameObject> enemigosOleada = new List<GameObject>();
    public int OleadaActual = 1;
    Scr_ControladorBatalla ControladorBatalla;


    PlayerInput playerInput;
    InputIconProvider IconProvider;
    private InputAction Interactuar;
    private InputAction Recolectar;


    private Sprite iconoActualInteractuar = null;
    private string textoActualInteractuar = "";
    void Start()
    {
        ControladorBatalla = GetComponent<Scr_ControladorBatalla>();
        singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
        prefabEnemigo = singleton.Enemigo;
        CantEnemigosPorOleada = singleton.CantidadEnemigosPorOleada;


        playerInput = singleton.GetComponent<PlayerInput>();
        IconProvider = singleton.GetComponent<InputIconProvider>();


        Interactuar = playerInput.actions["Interactuar"];
        Recolectar = playerInput.actions["Recolectar"];
        // Inicializar barra de oleadas
        var enemigo = singleton.Enemigo.GetComponent<Scr_Enemigo>();
        float dificultad = enemigo.DificultadInicial;
        float totalOleadas = enemigo.CantidadDeOleadas + 1;
        int dificultadIndex = (int)dificultad - 1;

        for (int i = 1; i <= totalOleadas; i++)
        {
            GameObject icono;
            if (enemigo.PuntoDeHuida == i)
            {
                icono = Instantiate(Iconos[5], BarraSlider.GetChild(1));
            }
            else
            {
                int indiceIcono = Mathf.Clamp(dificultadIndex, 0, Iconos.Length - 2);
                icono = Instantiate(Iconos[indiceIcono], BarraSlider.GetChild(1));
                OleadasActuales.Add(icono);
                dificultadIndex++;
            }
            icono.GetComponent<RectTransform>().localPosition = new Vector3(((80 / (totalOleadas - 1)) * (i - 1)) - 2.5f, 0, 0);

            var mapa = GameObject.Find("Mapa").transform;
            foreach (Transform child in mapa)
            {
                if (child.name == singleton.NombreMapa)
                {
                    foreach (Transform objeto in child.GetChild(1))
                    {
                        if (objeto.name.Contains("Spawner Dentro") && enemigo.SpawnDentro)
                        {
                            spawners.Add(objeto.gameObject);
                        }

                        if (objeto.name.Contains("Spawner Medio") && enemigo.SpawnMedio)
                        {
                            spawners.Add(objeto.gameObject);
                        }

                        if (objeto.name.Contains("Spawner Lejos") && enemigo.SpawnLejos)
                        {
                            spawners.Add(objeto.gameObject);
                        }

                        if (objeto.name.Contains("Spawner Distancia") && enemigo.SpawnDistancia)
                        {
                            spawners.Add(objeto.gameObject);
                        }

                    }
                }
            }
        }
    }

    private void Update()
    {
        if (ControladorBatalla.ComenzoBatalla)
        {
            if (enemigosOleada.Count > 0)
            {
                TextoCantidadEnemigos.text = (CantEnemigosPorOleada * OleadaActual) - enemigosOleada.Count + "/" + CantEnemigosPorOleada * OleadaActual;
                BotonOleada.SetActive(false);
                iconoActualInteractuar = null;
                textoActualInteractuar = "";
            }
            else
            {
                TextoCantidadEnemigos.text = "•/•";
                var enemigo = prefabEnemigo.GetComponent<Scr_Enemigo>();
                int oleadaActual = OleadaActual;
                bool siguienteEsBandera = (oleadaActual + 1 == enemigo.PuntoDeHuida);
                if (siguienteEsBandera)
                {
                    BotonOleada.SetActive(true);

                    IconProvider.ActualizarIconoUI(Interactuar, BotonOleada.transform, ref iconoActualInteractuar, ref textoActualInteractuar, false);
                    if (Interactuar.IsPressed())
                    {
                        estaPresionandoE = true;
                        tiempoPresionE += Time.deltaTime;
                        float progreso = Mathf.Clamp01(tiempoPresionE / duracionNecesaria);
                        BotonOleada.transform.GetChild(1).GetComponent<Image>().fillAmount = progreso;

                        if (tiempoPresionE >= duracionNecesaria)
                        {
                            BotonOleada.SetActive(false);
                            ControladorBatalla.NumeroCuenta.text = "";

                            // Reset
                            tiempoPresionE = 0f;
                            estaPresionandoE = false;
                            BarraSlider.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                            ControladorBatalla.ComenzoBatalla = false;
                            ControladorBatalla.FinalizarBatalla(true);
                        }

                    }
                    else
                    {
                        // Si se soltó la tecla antes de completar el tiempo
                        if (estaPresionandoE)
                        {
                            tiempoPresionE = 0f;
                            estaPresionandoE = false;
                            BotonOleada.transform.GetChild(1).GetComponent<Image>().fillAmount = 0f;
                        }
                    }
                }

            }
        }
        else
        {
            TextoCantidadEnemigos.text = "•/•";
            BotonOleada.SetActive(false);
        }
    }

    public void SpawnNuevaOleada(List<GameObject> spawners)
    {
        enemigosOleada.Clear();

        for (int i = 0; i < CantEnemigosPorOleada * OleadaActual; i++)
        {
            Debug.Log(spawners.Count);
            int pos = Random.Range(0, spawners.Count);

            // Posición con leve variación
            Vector3 spawnPosition = spawners[pos].transform.position;
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            spawnPosition += offset;

            // Asegurar que esté en NavMesh
            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                spawnPosition = navHit.position;
            }

            // Instanciar enemigo
            GameObject enemigo = Instantiate(prefabEnemigo, spawnPosition, Quaternion.identity);
            enemigosOleada.Add(enemigo);

            // Verificar agente
            NavMeshAgent agent = enemigo.GetComponent<NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.enabled = false; // se activará después de la cuenta
            }
            else
            {
                Debug.LogWarning("El enemigo no está en el NavMesh.");
            }
        }
    }

    public bool OleadaCompletada()
    {
        enemigosOleada.RemoveAll(e => e == null);
        return enemigosOleada.Count == 0;
    }

    public void IniciarPrimeraOleada()
    {
        enemigosOleada.Clear();

        for (int i = 0; i < CantEnemigosPorOleada; i++)
        {
            int pos = Random.Range(0, spawners.Count);

            // Posición con leve variación
            Vector3 spawnPosition = spawners[pos].transform.position;
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            spawnPosition += offset;

            // Asegurar que esté en NavMesh
            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                spawnPosition = navHit.position;
            }

            // Instanciar enemigo
            GameObject enemigo = Instantiate(prefabEnemigo, spawnPosition, Quaternion.identity);
            enemigosOleada.Add(enemigo);

            // Verificar agente
            NavMeshAgent agent = enemigo.GetComponent<NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.enabled = false; // se activará después de la cuenta
            }
            else
            {
                Debug.LogWarning("El enemigo no está en el NavMesh.");
            }
        }
    }

    public void ComprobarOleada()
    {
        if (prefabEnemigo == null) return;
        if (OleadaCompletada())
        {
            var enemigo = prefabEnemigo.GetComponent<Scr_Enemigo>();
            int oleadaActual = OleadaActual;
            bool siguienteEsBandera = (oleadaActual + 1 == enemigo.PuntoDeHuida);
            float tiempoActualEntreOleadas = siguienteEsBandera && enemigo.PuntoDeHuida != enemigo.CantidadDeOleadas + 1 ? TiempoEntreOleadas * 2f : TiempoEntreOleadas;

            int totalOleadas = (int)enemigo.CantidadDeOleadas;
            // Cálculo del valor inicial y objetivo del slider
            float valorInicial = (float)(oleadaActual - 1) / (totalOleadas);
            float objetivo = (float)(oleadaActual + (siguienteEsBandera && enemigo.PuntoDeHuida != enemigo.CantidadDeOleadas + 1 ? 1 : 0)) / totalOleadas;

            ContTiempoEntreOleadas += Time.deltaTime;
            float progreso = Mathf.Clamp01(ContTiempoEntreOleadas / tiempoActualEntreOleadas);

            ActualizarFlechaOleada(
            enemigo,
            tiempoActualEntreOleadas
            );

            if (OleadaActual > enemigo.CantidadDeOleadas)
            {
                ControladorBatalla.ComenzoBatalla = false;
                ControladorBatalla.FinalizarBatalla(true);
                Debug.Log("Terminar");
            }
            // Actualizar barra visual
            Slider slider = BarraSlider.GetChild(0).GetComponent<Slider>();
            slider.value = Mathf.Lerp(valorInicial, objetivo, progreso);

            // Tiempo restante entre oleadas
            float tiempoRestante = tiempoActualEntreOleadas - ContTiempoEntreOleadas;

            // Obtener texto del temporizador
            TextMeshProUGUI textoTiempo = BarraSlider.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

            if (tiempoRestante >= 4f)
            {
                textoTiempo.gameObject.SetActive(true);
                textoTiempo.text = ((int)tiempoRestante).ToString();
                ControladorBatalla.NumeroCuenta.gameObject.SetActive(false);
            }
            else if (tiempoRestante > 0f)
            {
                textoTiempo.gameObject.SetActive(false);
                ControladorBatalla.NumeroCuenta.gameObject.SetActive(true);

                // Mostrar cuenta regresiva exacta sin saltos
                int segundos = Mathf.FloorToInt(tiempoRestante);
                string textoMostrado;

                if (segundos >= 1)
                    textoMostrado = segundos.ToString();
                else
                {
                    if (OleadaActual == enemigo.CantidadDeOleadas && enemigo.PuntoDeHuida == enemigo.CantidadDeOleadas + 1)
                        textoMostrado = "¡Adios!";
                    else
                        textoMostrado = "¡Pelea!";
                }

                ControladorBatalla.NumeroCuenta.text = textoMostrado;

                // --- 🔊 Reproducir sonido cuando cambia el texto ---
                if (textoMostrado != ultimoTextoMostrado)
                {
                    if (textoMostrado == "¡Pelea!" || textoMostrado == "¡Adios!")
                    {
                        audioSource.clip = SonidoPelea;
                    }
                    else
                    {
                        audioSource.clip = SonidoReloj;
                    }

                    audioSource.Play();
                    ultimoTextoMostrado = textoMostrado;
                }
            }

            else
            {
                OcultarFlechaOleada();

                ControladorBatalla.NumeroCuenta.gameObject.SetActive(false);
                textoTiempo.gameObject.SetActive(false);
                ContTiempoEntreOleadas = 0;
                // Sincronizar la barra con el progreso de la oleada actual.
                Slider NewSlider = BarraSlider.GetChild(0).GetComponent<Slider>();
                NewSlider.value = objetivo;

                OleadaActual += siguienteEsBandera ? 2 : 1;

                if (OleadaActual <= enemigo.CantidadDeOleadas + 1)
                {
                    var mapa = GameObject.Find("Mapa").transform;
                    foreach (Transform child in mapa)
                    {
                        if (child.name == singleton.NombreMapa)
                        {
                            foreach (Transform objeto in child.GetChild(2))
                            {
                                if (objeto.name.Contains("Spawner Dentro") && enemigo.SpawnDentro)
                                {
                                    spawners.Add(objeto.gameObject);
                                }

                                if (objeto.name.Contains("Spawner Medio") && enemigo.SpawnMedio)
                                {
                                    spawners.Add(objeto.gameObject);
                                }

                                if (objeto.name.Contains("Spawner Lejos") && enemigo.SpawnLejos)
                                {
                                    spawners.Add(objeto.gameObject);
                                }

                                if (objeto.name.Contains("Spawner Distancia") && enemigo.SpawnDistancia)
                                {
                                    spawners.Add(objeto.gameObject);
                                }

                            }
                        }
                    }

                    SpawnNuevaOleada(spawners);

                    foreach (GameObject enemigoGO in enemigosOleada)
                    {
                        if (enemigoGO != null)
                        {
                            NavMeshAgent agent = enemigoGO.GetComponent<NavMeshAgent>();
                            if (agent != null)
                                agent.enabled = true;
                        }
                    }
                }
                else
                {
                    ControladorBatalla.ComenzoBatalla = false;
                    ControladorBatalla.FinalizarBatalla(true);
                    Debug.Log("Terminar");
                }
            }


        }
    }




    private void ActualizarFlechaOleada(
        Scr_Enemigo enemigo,
        float duracionEspera)
    {
        // Ocultar las flechas mientras hay enemigos en combate.
        if (enemigosOleada.Count > 0)
        {
            OcultarFlechaOleada();
            return;
        }

        // OleadasActuales NO contiene el icono de huida.
        // Por eso calculamos directamente el índice del siguiente
        // icono normal, saltando la posición de la bandera.

        int indiceIcono = OleadaActual;

        // Si la bandera ya se encuentra antes de la oleada actual,
        // descontamos su posición de la lista.
        if (enemigo.PuntoDeHuida <= OleadaActual)
        {
            indiceIcono--;
        }

        // Si no existe un icono siguiente, ocultar la flecha.
        if (indiceIcono < 0 ||
            indiceIcono >= OleadasActuales.Count)
        {
            OcultarFlechaOleada();
            return;
        }

        // El hijo 0 del icono es la flecha.
        Transform flecha =
            OleadasActuales[indiceIcono].transform.GetChild(0);

        // Cambiar de flecha solamente si cambia el icono objetivo.
        if (FlechaOleadaActiva != flecha.gameObject)
        {
            OcultarFlechaOleada();

            FlechaOleadaActiva = flecha.gameObject;
            FlechaOleadaActiva.SetActive(true);
        }

        // El hijo 0 de la flecha es el botón.
        Transform boton = flecha.GetChild(0);
        BotonAdelantarOleada = boton.gameObject;

        
         IconProvider.ActualizarIconoUI(
             Recolectar,
             boton,
              ref iconoActualInteractuar,
             ref textoActualInteractuar,
             false
         );

        // Aumentar la carga radial mientras se mantiene pulsada
        // la acción de interacción.
        if (Recolectar.IsPressed())
        {
            EstaPresionandoAdelantar = true;
            TiempoPresionAdelantar += Time.deltaTime;

            float progreso = Mathf.Clamp01(
                TiempoPresionAdelantar / DuracionAdelantarOleada
            );

            // El hijo 1 del botón contiene la imagen de carga.
            if (boton.childCount > 1)
            {
                Image carga =
                    boton.GetChild(1).GetComponent<Image>();

                if (carga != null)
                {
                    carga.fillAmount = progreso;
                }
            }

            // Al completar la carga, finalizar la espera.
            if (TiempoPresionAdelantar >= DuracionAdelantarOleada)
            {
                // Completar la espera actual.
                ContTiempoEntreOleadas = duracionEspera;

                // Completar visualmente la barra inmediatamente.
                Slider slider = BarraSlider.GetChild(0).GetComponent<Slider>();
                slider.value = slider.maxValue;

                // Ocultar la flecha y reiniciar su carga.
                OcultarFlechaOleada();
            }
        }
        else if (EstaPresionandoAdelantar)
        {
            // Si se suelta antes de completar, cancelar la carga.
            ReiniciarCargaAdelantar();
        }
    }



    private void ReiniciarCargaAdelantar()
    {
        TiempoPresionAdelantar = 0f;
        EstaPresionandoAdelantar = false;

        if (BotonAdelantarOleada != null &&
            BotonAdelantarOleada.transform.childCount > 1)
        {
            Image imagenCarga = BotonAdelantarOleada
                .transform.GetChild(1)
                .GetComponent<Image>();

            if (imagenCarga != null)
                imagenCarga.fillAmount = 0f;
        }
    }

    private void OcultarFlechaOleada()
    {
        if (FlechaOleadaActiva != null)
            FlechaOleadaActiva.SetActive(false);

        FlechaOleadaActiva = null;
        BotonAdelantarOleada = null;

        ReiniciarCargaAdelantar();
    }

}
