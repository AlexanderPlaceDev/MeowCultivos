using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Scr_ControladorOleadas : MonoBehaviour
{

    [Header("Barra Oleadas")]
    [SerializeField] GameObject[] Iconos;
    [SerializeField] Transform BarraSlider;
    [SerializeField] float TiempoEntreOleadas;
    [SerializeField] GameObject BotonOleada;
    [SerializeField] TextMeshProUGUI TextoCantidadEnemigos;
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
    void Start()
    {
        ControladorBatalla = GetComponent<Scr_ControladorBatalla>();
        singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
        prefabEnemigo = singleton.Enemigo;
        CantEnemigosPorOleada = prefabEnemigo.GetComponent<Scr_Enemigo>().CantidadEnemigosPorOleada;
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
                dificultadIndex++;
            }
            icono.GetComponent<RectTransform>().localPosition = new Vector3(((80 / (totalOleadas - 1)) * (i - 1)) - 2.5f, 0, 0);

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

                    if (Input.GetKey(KeyCode.E))
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
                ControladorBatalla.NumeroCuenta.gameObject.SetActive(false);
                textoTiempo.gameObject.SetActive(false);
                ContTiempoEntreOleadas = 0;

                OleadaActual += siguienteEsBandera ? 2 : 1;

                if (OleadaActual <= enemigo.CantidadDeOleadas + 1)
                {
                    List<GameObject> spawners = new List<GameObject>();
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
}
