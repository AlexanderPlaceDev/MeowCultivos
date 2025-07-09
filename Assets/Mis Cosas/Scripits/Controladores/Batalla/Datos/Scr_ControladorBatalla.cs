using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
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
    public float PuntosActualesHabilidad = 0;

    [SerializeField] public TextMeshProUGUI NumeroCuenta;
    [SerializeField] GameObject Mirilla;

    private float Cuenta = 4;
    private bool ComenzarCuenta = false;
    public bool ComenzoBatalla = false;

    [SerializeField] private GameObject PanelFinal;
    [SerializeField] Animator[] BarrasNegras;
    [SerializeField] GameObject CirculoCarga;

    [Header("Vida")]
    [SerializeField] GameObject Vidas;
    [SerializeField] TextMeshProUGUI TextoVidas;
    [SerializeField] float VidaMaxima;
    public float VidaAnterior = 3;
    public float VidaActual = 3;
    [SerializeField] Slider BarraVida;

    [Header("Objetivo")]
    [SerializeField] TextMeshProUGUI Mision;
    [SerializeField] TextMeshProUGUI Complemento;
    [SerializeField] TextMeshProUGUI Item;

    [Header("Barra Oleadas")]
    [SerializeField] Transform BarraSlider;
    [SerializeField] float TiempoEntreOleadas;
    float ContTiempoEntreOleadas;
    private bool PrimerSpawn = false;


    [Header("Otros")]
    [SerializeField] Light OrigenLuz;
    private Scr_DatosSingletonBatalla Singleton;
    private Scr_ControladorOleadas controladorOleadas;
    private bool DioRecompensa = false;
    void Start()
    {
        Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
        controladorOleadas = GetComponent<Scr_ControladorOleadas>();

        Mision.text = Singleton.Mision;
        Mision.color = Singleton.ColorMision;
        Complemento.text = Singleton.Complemento;
        Item.text = Singleton.Item;
        Item.color = Singleton.ColorItem;

        Habilidad1 = PlayerPrefs.GetString("Habilidad1", "Ojo");
        Habilidad2 = PlayerPrefs.GetString("Habilidad2", "Rugido");
        HabilidadEspecial = PlayerPrefs.GetString("HabilidadEspecial", "Garras");

        TextoVidas.text = VidaMaxima + "/" + VidaMaxima;
        if (Singleton.HoraActual > 7 && Singleton.HoraActual < 19)
        {
            RenderSettings.skybox = Singleton.SkyBoxDia;
        }
        else
        {
            RenderSettings.skybox = Singleton.SkyBoxNoche;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12)) PlayerPrefs.DeleteAll();

        Comienzo();
        ActualizarVida();
        Terminar();
    }

    private void ActualizarVida()
    {
        if (VidaActual != VidaAnterior)
        {
            VidaAnterior = VidaActual;
            TextoVidas.text = VidaActual + "/" + VidaMaxima;
            BarraVida.value = VidaActual / VidaMaxima;
        }
    }

    public void IniciarCuentaRegresiva()
    {
        NumeroCuenta.gameObject.SetActive(true);
        ComenzarCuenta = true;
    }

    private void Comienzo()
    {
        if (ComenzarCuenta)
        {
            if (Cuenta > 0)
            {
                Debug.Log("Comenzo la cuenta con un numero mayor a 0");
                if (Cuenta == 4)
                {
                    if (!PrimerSpawn)
                    {
                        OrigenLuz.color = Singleton.Luz;
                        PrepararBatalla();
                        controladorOleadas.IniciarPrimeraOleada();
                        PrimerSpawn = true;
                    }
                }
                Cuenta -= Time.deltaTime;
                NumeroCuenta.text = Cuenta < 1 ? "Pelea" : ((int)Cuenta).ToString();
            }
            else
            {
                Debug.Log("Comenzo la cuenta con el numero en 0");
                if (controladorOleadas.enemigosOleada.Count > 0 && ComenzarCuenta)
                {
                    foreach (GameObject Enemigo in controladorOleadas.enemigosOleada)
                    {
                        Enemigo.GetComponent<NavMeshAgent>().enabled = true;
                    }
                }
                NumeroCuenta.gameObject.SetActive(false);
                ComenzarCuenta = false;
                Cuenta = 4;
                ActivarControles(true);
                ComenzoBatalla = true;
            }
        }
    }

    private void Terminar()
    {
        if (VidaActual <= 0)
        {
            VidaActual = Mathf.Max(VidaActual, 0);
            ComenzoBatalla = false;
            FinalizarBatalla(false);
        }
        else if (ComenzoBatalla)
        {
            controladorOleadas.ComprobarOleada();
        }


    }

    public void FinalizarBatalla(bool Gano)
    {
        ActivarControles(false);
        if (Gano && !DioRecompensa)
        {
            DioRecompensa = true;
            DarRecompensa();
        }

        if (PlayerPrefs.GetInt("XPActual", 0) >= PlayerPrefs.GetInt("XPSiguiente", 10))
        {
            PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual") - PlayerPrefs.GetInt("XPSiguiente", 10));
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
        var recompensasDict = new Dictionary<Scr_CreadorObjetos, int>();
        
        int totalEnemigos = enemigo.CantidadEnemigosPorOleada * controladorOleadas.OleadaActual;
        Debug.Log(totalEnemigos + "**********");
        for (int k = 0; k < totalEnemigos; k++)
        {
            // Tiradas por enemigo individual
            for (int i = 0; i < enemigo.Drops.Length; i++)
            {
                if (Random.Range(0, 100) <= enemigo.Probabilidades[i])
                {
                    if (recompensasDict.ContainsKey(enemigo.Drops[i]))
                        recompensasDict[enemigo.Drops[i]] += 1;
                    else
                        recompensasDict[enemigo.Drops[i]] = 1;
                }
            }

            // XP individual por enemigo
            PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual") + Random.Range(enemigo.XPMinima, enemigo.XPMaxima));
        }
        
        int cazado = PlayerPrefs.GetInt("Cazado_Cantidad", 0);
        bool havecaza = false;
        if (cazado > 0)
        {
            for (int i = 0; i < cazado; i++)
            {
                string nombrecazado = PlayerPrefs.GetString("Cazado_" + i, "");
                int cantCazados = PlayerPrefs.GetInt("cazado_cant" + i, 0);
                if (!string.IsNullOrEmpty(nombrecazado) && cantCazados > 0)
                {
                    if (nombrecazado == enemigo.name)
                    {
                        Debug.Log("ya exite");
                        PlayerPrefs.SetString("Cazado_" + i, nombrecazado);
                        PlayerPrefs.SetInt("cazado_cant" + i, cantCazados + totalEnemigos);
                        havecaza = true;
                        break;
                    }
                }
            }
        }


        if (!havecaza)
        {

            Debug.Log("NO exite" + cazado);
            //Debug.Log("se caso" + enemigo.name + " num" + cazado);
            PlayerPrefs.SetString("Cazado_" + cazado, enemigo.name);
            PlayerPrefs.SetInt("cazado_cant" + cazado, totalEnemigos);
            PlayerPrefs.SetInt("Cazado_Cantidad", cazado+1);
        }
        PlayerPrefs.Save();

        // Mostrar recompensas
        var datos = Singleton;
        datos.ObjetosRecompensa.Clear();
        datos.CantidadesRecompensa.Clear();
        //datos.enemicaza = enemigo.name;
        //datos.cantcaza = totalEnemigos;
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
    }


    private void ActivarControles(bool activar)
    {
        Cursor.visible = !activar;
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
            anim.Play("Cerrar");

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

    public void PrepararBatalla()
    {
        // Activar mapa y posicionar jugador
        var mapa = GameObject.Find("Mapa").transform;

        foreach (Transform child in mapa)
        {
            if (child.name == Singleton.NombreMapa)
            {
                child.gameObject.SetActive(true);
                foreach (Transform objeto in child)
                {

                    if (objeto.name.Contains("Posicion Inicial") && controladorOleadas.OleadaActual == 1)
                    {
                        GameObject.Find("Personaje").transform.position = objeto.transform.position;
                        GameObject.Find("Personaje").transform.rotation = objeto.transform.rotation;
                    }
                }
            }
        }

        // Reconstruir NavMesh
        GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
