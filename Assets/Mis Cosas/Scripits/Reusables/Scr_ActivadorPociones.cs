using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_ActivadorPociones : MonoBehaviour
{
    [SerializeField] float TiempoActual = 0;
    [SerializeField] float TiempoMaximo = 5;
    [SerializeField] GameObject TextoTiempo;
    [SerializeField] GameObject Bloqueo;
    [SerializeField] bool EsFinal;
    [SerializeField] GameObject Porcentaje;
    [SerializeField] Sprite[] IconosBloqueo;
    [SerializeField] Color[] Colores;
    [SerializeField] KeyCode Tecla;

    [SerializeField] GameObject Boton;
    public int usos;
    public bool Espermanente = false;
    public Color ColorHabilidad;
    private bool usando = false;
    // Referencia al script Scr_Habilidades
    private Scr_ControladorPociones Pociones;

    Scr_ControladorBatalla ControladorBatalla;

    Scr_DatosArmas DatosArmas;

    public int cargaHabilidad;


    PlayerInput playerInput;
    InputIconProvider IconProvider;
    private InputAction Pocion;

    private Sprite iconoActualPocion = null;
    private string textoActualPocion = "";

    string PocionChec = "";
    void Start()
    {
        ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        DatosArmas = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>();
        // Buscar el script Scr_Habilidades en el mismo objeto o en otro específico
        Pociones = GetComponent<Scr_ControladorPociones>(); playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();

        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Pocion = playerInput.actions["Pocion"];
    }
    private void OnEnable()
    {
        ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        DatosArmas = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>();
        // Buscar el script Scr_Habilidades en el mismo objeto o en otro específico
        Pociones = GetComponent<Scr_ControladorPociones>();
        ObtenerPocion(); 
        Pociones = GetComponent<Scr_ControladorPociones>(); playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Pocion = playerInput.actions["Pocion"];
    }
    void Update()
    {
        if (Espermanente) return;

        IconProvider.ActualizarIconoUI(Pocion, Boton.transform, ref iconoActualPocion, ref textoActualPocion, false);
        ActivarHabilidad();

        if (TiempoActual > 0 && usando)
        {
            TiempoActual -= Time.deltaTime;

            Bloqueo.SetActive(true);
            TextoTiempo.SetActive(true);

            TextoTiempo.GetComponent<TextMeshProUGUI>().text = ((int)TiempoActual + 1).ToString();

            Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[1];
            TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[1];

            // Cuando el tiempo de efecto acaba → cambiar a cooldown
            if (TiempoActual <= 0)
            {
                usando = false;              // ← CAMBIO AL SEGUNDO IF
                TiempoActual = cargaHabilidad; // ← Ajusta el tiempo de enfriamiento
            }

            return;
        }

        if (TiempoActual > 0 && !usando)
        {
            TiempoActual -= Time.deltaTime;

            Bloqueo.SetActive(true);
            TextoTiempo.SetActive(true);

            TextoTiempo.GetComponent<TextMeshProUGUI>().text = ((int)TiempoActual + 1).ToString();
            Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[0];
            TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[0];

            return;
        }

        if (TiempoActual <= 0)
        {
            TiempoActual = 0;
            Bloqueo.SetActive(false);
            TextoTiempo.SetActive(false);
        }
    }

    private void ActivarHabilidad()
    {
        if (Pocion.IsPressed() && !Espermanente)
        {
            if (Pociones != null )
            {
                if (TiempoActual == 0 && usos != 0)
                {
                    Pociones.Pociones(PocionChec);
                    TiempoActual = TiempoMaximo;
                    usos--;
                    usando = true;
                }
                else
                {
                    Debug.LogWarning("No hay mas");
                }
            }
            else
            {
                Debug.LogWarning("No se encontró el script Scr_ControladorPociones");
            }
        }
        else if (Espermanente)
        {
            Pociones.Pociones(PocionChec);
            TiempoActual = TiempoMaximo;
            usos--;
            gameObject.SetActive(false);
        }
    }


    private void ObtenerPocion()
    {
        string Pocion = "";
        Pocion = ControladorBatalla.Pocion; // Cambiar por la habilidad deseada
        if (Pocion != "")
        {
            for (int i = 0; i < DatosArmas.Pociones.Length; i++)
            {
                if (DatosArmas.Pociones[i].Nombre == Pocion)
                {
                    Pociones.ControladorBatalla = ControladorBatalla;
                    Pociones.mov=GameObject.Find("Personaje").GetComponent<Scr_Movimiento>();
                    Pociones.ControladorArmas = GameObject.Find("Controlador").GetComponent<Scr_ControladorArmas>();
                    usos = DatosArmas.Pociones[i].Usos;
                    TiempoMaximo = DatosArmas.Pociones[i].Duracion;
                    cargaHabilidad = DatosArmas.Pociones[i].Enfriamiento;
                    Pociones.PocionDuracion = DatosArmas.Pociones[i].Duracion;
                    Pociones.ColorPocion = DatosArmas.Pociones[i].Color;
                    Pociones.PocionPuntos = DatosArmas.Pociones[i].Puntos;
                    Pociones.PocionUsos = DatosArmas.Pociones[i].Usos;
                    Pociones.Resistencia = DatosArmas.Pociones[i].efecto.ToString();
                    Pociones.PocionPermanente = DatosArmas.Pociones[i].Permanente;
                    ColorHabilidad = DatosArmas.Pociones[i].Color;
                    transform.GetChild(0).GetComponent<Image>().sprite = DatosArmas.Pociones[i].Icono;
                    PocionChec = DatosArmas.Pociones[i].Tipo.ToString();
                    Pociones.PocionPermanente = DatosArmas.Pociones[i].Permanente;
                    if (DatosArmas.Pociones[i].Permanente)
                    {
                        Transform hijo = transform.GetChild(1);
                        hijo.gameObject.SetActive(false);
                        Espermanente = true;
                        ActivarHabilidad();
                    }
                    break;
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
