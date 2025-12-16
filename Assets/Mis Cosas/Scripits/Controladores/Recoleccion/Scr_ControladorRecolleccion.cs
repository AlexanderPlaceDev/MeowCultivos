using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Scr_ControladorRecolleccion : MonoBehaviour
{
    [Header("Barra Oleadas")]
    [SerializeField] GameObject[] Iconos;
    [SerializeField] Transform BarraSlider;
    [SerializeField] float TiempoEntreOleadas;
    [SerializeField] GameObject BotonOleada;
    [SerializeField] TextMeshProUGUI TextoCantidadFrutas;
    [SerializeField] Sprite[] Fruta; 
    [SerializeField] Sprite IconoFruta; 
    
    public bool Dropear = true;
    private int CantFrutaPorOleada;
    public int CantFrutaRecolectadas;
    public GameObject prefabFruta;
    public float TiempoDeRecoleccion=0;
    [Header("Cuenta Regresiva")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip SonidoReloj;
    [SerializeField] private AudioClip SonidoPelea;
    private string ultimoTextoMostrado = "";

    float ContTiempoEntreOleadas;

    private Scr_DatosSingletonBatalla singleton;


    public List<GameObject> enemigosOleada = new List<GameObject>();
    public int OleadaActual = 1;
    Scr_ControladorBatalla ControladorBatalla;
    [Header("Recompensas")]
    [SerializeField] public int DineroMin = 30; 
    [SerializeField] public  int DineroMax = 500;
    [SerializeField] public Scr_CreadorObjetos[] FrutasRecom;
    public Scr_CreadorObjetos FrutaRecom;

    void Start()
    {
        ControladorBatalla = GetComponent<Scr_ControladorBatalla>();
        singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
        //prefabFruta = singleton.Fruta;
        CantFrutaPorOleada = Random.Range(5,20);
    }
    
    private void Update()
    {
        if (ControladorBatalla.ComenzoBatalla)
        {
            TextoCantidadFrutas.text = CantFrutaRecolectadas + "/" + CantFrutaPorOleada;
            TiempoDeRecoleccion += Time.deltaTime;
        }
        else
        {
            TextoCantidadFrutas.text = "•/•";
        }
        if (CantFrutaRecolectadas>= CantFrutaPorOleada)
        {
            ControladorBatalla.NumeroCuenta.gameObject.SetActive(false);
            ContTiempoEntreOleadas = 0;
            ControladorBatalla.ComenzoBatalla = false;
            ControladorBatalla.FrutasRecolectadas = CantFrutaRecolectadas;
            ControladorBatalla.FinalizarBatalla(true);
            Debug.Log("Terminar");
        }
    }
    public bool OleadaCompletada()
    {
        enemigosOleada.RemoveAll(e => e == null);
        return enemigosOleada.Count == 0;
    }

    public void IniciarRecoleccion()
    {
        Transform mapa = GameObject.Find("Mapa").transform;

        foreach (Transform zona in mapa)
        {
            if (zona.name == singleton.NombreMapa)
            {
                Transform planta = zona.Find(Planta());

                if (planta != null)
                {
                    ActivarHijos(planta);
                }
                else
                {
                    Debug.Log("No hay" + Planta());
                }
            }
        }

    }
    void ActivarHijos(Transform padre)
    {
        foreach (Transform hijo in padre)
        {
            hijo.gameObject.SetActive(true);
        }
    }
    private string Planta()
    {
        switch (singleton.NombreFruta)
        {
            case "Baya Azul":
                IconoFruta = Fruta[0];
                FrutaRecom = FrutasRecom[0];
                return "Arbusto Baya Azul";
            case "Baya Amarillo":
                IconoFruta = Fruta[1];
                FrutaRecom = FrutasRecom[1];
                return "Arbusto Baya Roja";
            case "Baya Roja":
                IconoFruta = Fruta[2];
                FrutaRecom = FrutasRecom[2];
                return "Arbusto Baya Amarilla";
            case "Platano":
                IconoFruta = Fruta[3];
                FrutaRecom = FrutasRecom[3];
                return "Palmeras Platano";
            default:
                IconoFruta = Fruta[3];
                FrutaRecom = FrutasRecom[3];
                return "Palmeras Platano";
        }
    }
    public void ComprobarOleada()
    {
        /*
        if (OleadaActual > enemigo.CantidadDeOleadas)
        {
            ControladorBatalla.ComenzoBatalla = false;
            ControladorBatalla.FinalizarBatalla(true);
            Debug.Log("Terminar");
        }

        // Actualizar barra visual
        Slider slider = BarraSlider.GetChild(0).GetComponent<Slider>();
        slider.value = Mathf.Lerp(valorInicial, objetivo, progreso);*/

        // Tiempo restante entre oleadas
        float tiempoRestante = TiempoEntreOleadas - ContTiempoEntreOleadas;

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
                textoMostrado = "¡Recolecta!";
            }

            ControladorBatalla.NumeroCuenta.text = textoMostrado;

            // --- 🔊 Reproducir sonido cuando cambia el texto ---
            if (textoMostrado != ultimoTextoMostrado)
            {
                if (textoMostrado == "¡Recolecta!" || textoMostrado == "¡Adios!")
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

            ControladorBatalla.ComenzoBatalla = false;
            ControladorBatalla.FinalizarBatalla(true);
            Debug.Log("Terminar");
        }
    }
}
