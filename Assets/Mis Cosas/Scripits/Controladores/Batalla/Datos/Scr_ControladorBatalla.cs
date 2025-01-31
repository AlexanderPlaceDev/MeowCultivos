using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scr_ControladorBatalla : MonoBehaviour
{
    [Header("Panel Final")]
    [SerializeField] Color[] ColoresBoton;
    [SerializeField] TextMeshProUGUI TextoNivel;
    [SerializeField] TextMeshProUGUI TextoSiguienteNivel;
    [SerializeField] Image Barra;

    public GameObject ArmaActual;
    public string Habilidad1;
    public string Habilidad2;
    public string HabilidadEspecial;

    [SerializeField] TextMeshProUGUI NumeroCuenta;
    [SerializeField] GameObject Mirilla;

    private float Cuenta = 4;
    [SerializeField] TextMeshProUGUI TextoMinutos;
    [SerializeField] int Minutos = 2;
    [SerializeField] TextMeshProUGUI TextoSegundos;
    [SerializeField] float Segundos = 60;
    private bool ComenzarCuenta = false;
    private bool ComenzoTiempo = false;

    [SerializeField] private GameObject PanelFinal;
    [SerializeField] Animator[] BarrasNegras;
    [SerializeField] GameObject CirculoCarga;

    public List<GameObject> Enemigos = new List<GameObject>();
    private bool DioRecompensa = false;

    [SerializeField] GameObject Vidas;
    [SerializeField] GameObject BarraVida;
    [SerializeField] GameObject BarraVidaAmarilla;
    [SerializeField] GameObject BarraVidaRoja;
    [SerializeField] GameObject BarraVidaVacia;
    [SerializeField] TextMeshProUGUI TextoVidas;
    [SerializeField] float VidaMaxima;
    public float VidaAnterior = 3;
    public float VidaActual = 3;
    public float PuntosActualesHabilidad = 0;

    private Scr_DatosSingletonBatalla Singleton;

    void Start()
    {
        Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
        Habilidad1 = PlayerPrefs.GetString("Habilidad1", "Ojo");
        Habilidad2 = PlayerPrefs.GetString("Habilidad2", "Rugido");
        HabilidadEspecial = PlayerPrefs.GetString("Habilidad2", "Garras");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            PlayerPrefs.DeleteAll();
        }

        Comienzo();
        Tiempo();
        RemoverEnemigosMuertos();
        ActualizarVida();
        Terminar();
    }

    private void ActualizarVida()
    {
        if (VidaActual != VidaAnterior)
        {
            // Actualizar el valor de VidaAnterior
            VidaAnterior = VidaActual;

            TextoVidas.text = VidaActual.ToString() + "/" + VidaMaxima.ToString();

            // Destruir los hijos actuales de "Vidas"
            foreach (Transform hijo in Vidas.GetComponentInChildren<Transform>())
            {
                Destroy(hijo.gameObject);
            }

            // Crear las nuevas barras de vida según VidaActual
            for (int i = 0; i < VidaMaxima; i++)
            {
                if (i < VidaActual)
                {
                    // Instancia el objeto "BarraVida" como hijo de "Vidas"
                    if (VidaActual > VidaMaxima / 100 * 50)
                    {
                        GameObject nuevaBarra = Instantiate(BarraVida, Vidas.transform);
                    }
                    else
                    {
                        if (VidaActual > VidaMaxima / 100 * 20)
                        {
                            GameObject nuevaBarra = Instantiate(BarraVidaAmarilla, Vidas.transform);
                        }
                        else
                        {
                            GameObject nuevaBarra = Instantiate(BarraVidaRoja, Vidas.transform);
                        }
                    }
                }
                else
                {
                    GameObject nuevaBarra = Instantiate(BarraVidaVacia, Vidas.transform);
                }
            }

        }
    }

    private void RemoverEnemigosMuertos()
    {
        Enemigos.RemoveAll(enemigo => enemigo == null);
    }

    public void CuentaAtras()
    {
        List<GameObject> Spawners = new List<GameObject>();
        var mapa = GameObject.Find("Mapa").transform;

        foreach (Transform child in mapa)
        {
            if (child.name == Singleton.NombreMapa)
            {
                child.gameObject.SetActive(true);
                foreach (Transform objeto in child)
                {
                    if (objeto.name.Contains("Spawner"))
                    {
                        Spawners.Add(objeto.gameObject);
                    }
                }
            }
        }

        GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>().BuildNavMesh();
        NumeroCuenta.gameObject.SetActive(true);
        ComenzarCuenta = true;

        for (int i = 0; i < Singleton.CantidadDeEnemigos; i++)
        {
            int pos = UnityEngine.Random.Range(0, Spawners.Count);

            // Obtener posición del Spawner y añadir una variación aleatoria
            Vector3 spawnPosition = Spawners[pos].transform.position;
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
            spawnPosition += offset;

            // Ajustar la posición al NavMesh más cercano
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(spawnPosition, out navHit, 2f, NavMesh.AllAreas))
            {
                spawnPosition = navHit.position;
            }

            // Instanciar enemigo en la posición ajustada
            GameObject enemigo = Instantiate(Singleton.Enemigo, spawnPosition, Quaternion.identity);
            Enemigos.Add(enemigo);

            // Verificar y habilitar NavMeshAgent
            NavMeshAgent agent = enemigo.GetComponent<NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.enabled = true;
            }
            else
            {
                Debug.LogWarning("El enemigo no está en el NavMesh.");
            }
        }
    }

    private void Comienzo()
    {
        if (ComenzarCuenta)
        {
            if (Cuenta > 0)
            {
                Cuenta -= Time.deltaTime;
                NumeroCuenta.text = Cuenta < 1 ? "Pelea" : ((int)Cuenta).ToString();
            }
            else
            {
                NumeroCuenta.gameObject.SetActive(false);
                ComenzarCuenta = false;
                Cuenta = 4;
                ActivarControles(true);
                ComenzoTiempo = true;

                foreach (GameObject enemigo in Enemigos)
                {
                    enemigo.GetComponent<NavMeshAgent>().enabled = true;
                    enemigo.GetComponent<CapsuleCollider>().enabled = true;
                }
            }
        }
    }

    private void Tiempo()
    {
        if (ComenzoTiempo)
        {
            Mirilla.SetActive(true);

            // Verifica que el tiempo sea mayor a cero antes de restar
            if (Minutos > 0 || Segundos > 0)
            {
                Segundos -= Time.deltaTime;

                // Si los segundos llegan a cero, restamos un minuto y reiniciamos los segundos a 59
                if (Segundos < 0)
                {
                    Segundos = 59;
                    Minutos--;

                    // Verifica si los minutos también se agotaron
                    if (Minutos < 0)
                    {
                        Minutos = 0;
                        Segundos = 0;
                    }
                }

                // Actualiza el texto del temporizador en la interfaz
                TextoSegundos.text = Segundos >= 10 ? ((int)Segundos).ToString() : "0" + ((int)Segundos).ToString();
                TextoMinutos.text = Minutos.ToString();
            }
            else
            {
                // Si el tiempo ha llegado a cero, se detiene el conteo y finaliza la batalla
                ComenzoTiempo = false;
                FinalizarBatalla(false);
            }
        }
    }

    private void Terminar()
    {
        if (VidaActual <= 0)
        {
            VidaActual = Mathf.Max(VidaActual, 0);
            ComenzoTiempo = false;
            FinalizarBatalla(false);
        }
        else if (Enemigos.Count <= 0 && ComenzoTiempo)
        {
            ComenzoTiempo = false;
            FinalizarBatalla(true);
        }
    }

    private void FinalizarBatalla(bool Gano)
    {
        ActivarControles(false);
        if (Gano && !DioRecompensa)
        {
            DioRecompensa = true;
            DarRecompensa();
        }

        if (PlayerPrefs.GetInt("XPActual", 0) >= PlayerPrefs.GetInt("XPSiguiente", 10))
        {
            PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual", 0) - PlayerPrefs.GetInt("XPSiguiente", 10));
            PlayerPrefs.SetInt("Nivel", PlayerPrefs.GetInt("Nivel", 0) + 1);
            PlayerPrefs.SetInt("XPSiguiente", PlayerPrefs.GetInt("XPSiguiente", 10) * 2);
            PlayerPrefs.SetInt("PuntosDeHabilidad", PlayerPrefs.GetInt("PuntosDeHabilidad", 0) + 3);
        }

        TextoNivel.text = PlayerPrefs.GetInt("Nivel", 0).ToString();
        TextoSiguienteNivel.text = "Siguiente Nivel: " + PlayerPrefs.GetInt("XPActual", 0) + "/" + PlayerPrefs.GetInt("XPSiguiente", 10);
        Barra.fillAmount = (float)PlayerPrefs.GetInt("XPActual", 0) / PlayerPrefs.GetInt("XPSiguiente", 10);

        PanelFinal.SetActive(true);
    }

    private void DarRecompensa()
    {
        var enemigo = Singleton.Enemigo.GetComponent<Scr_Enemigo>();
        if (enemigo.Drops.Length != enemigo.Probabilidades.Length)
        {
            Debug.LogError("La longitud de Drops y Probabilidades no coincide.");
            return;
        }

        var recompensasDict = new Dictionary<Scr_CreadorObjetos, int>();
        for (int j = 0; j < Singleton.CantidadDeEnemigos; j++)
        {
            for (int i = 0; i < enemigo.Drops.Length; i++)
            {
                if (UnityEngine.Random.Range(0, 100) <= enemigo.Probabilidades[i])
                {
                    if (recompensasDict.ContainsKey(enemigo.Drops[i]))
                    {
                        recompensasDict[enemigo.Drops[i]] += 1;
                    }
                    else
                    {
                        recompensasDict.Add(enemigo.Drops[i], 1);
                    }
                }
                PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual") + UnityEngine.Random.Range(enemigo.XPMinima, enemigo.XPMaxima));
            }
        }

        var datos = Singleton;
        datos.ObjetosRecompensa.Clear();
        datos.CantidadesRecompensa.Clear();

        int index = 0;
        foreach (var kvp in recompensasDict)
        {
            datos.ObjetosRecompensa.Add(kvp.Key);
            datos.CantidadesRecompensa.Add(kvp.Value);

            Transform rewardUI = PanelFinal.transform.GetChild(0).GetChild(6).GetChild(index);
            rewardUI.GetComponent<Image>().sprite = kvp.Key.IconoInventario;
            rewardUI.gameObject.SetActive(true);
            rewardUI.GetChild(0).gameObject.SetActive(true);
            rewardUI.GetChild(0).GetComponent<TextMeshProUGUI>().text = kvp.Value.ToString();

            index++;
        }

        if (datos.ObjetosRecompensa.Count != datos.CantidadesRecompensa.Count)
        {
            Debug.LogError("La cantidad de objetos y recompensas no coincide.");
        }
    }

    private void ActivarControles(bool activar)
    {
        Cursor.visible = !activar;  // Visible cuando no está en combate
        Cursor.lockState = activar ? CursorLockMode.Locked : CursorLockMode.None;

        var camara = Camera.main;
        GetComponent<Scr_ControladorArmas>().enabled = activar;
        camara.GetComponent<Scr_GirarCamaraBatalla>().enabled = activar;
        camara.transform.parent.GetComponent<Rigidbody>().useGravity = activar;
        camara.transform.parent.GetComponent<Scr_Movimiento>().enabled = activar;
    }

    public void BotonAceptar()
    {
        foreach (Animator anim in BarrasNegras)
        {
            anim.Play("Cerrar");
        }
        StartCoroutine(EsperarCierre());
        PanelFinal.GetComponent<Animator>().Play("Cerrar");
    }

    public void CambiarColorAceptar(bool Entra)
    {
        PanelFinal.transform.GetChild(0).GetChild(5).GetComponent<Image>().color = Entra ? ColoresBoton[0] : ColoresBoton[1];
    }

    IEnumerator EsperarCierre()
    {
        yield return new WaitForSeconds(1);
        CirculoCarga.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(2);
    }
}
