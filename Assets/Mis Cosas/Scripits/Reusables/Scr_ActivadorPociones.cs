using TMPro;
using UnityEngine;
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

    public int usos;
    public bool Espermanente = false;
    public Color ColorHabilidad;
    // Referencia al script Scr_Habilidades
    private Scr_ControladorPociones Pociones;

    Scr_ControladorBatalla ControladorBatalla;

    Scr_DatosArmas DatosArmas;

    public int cargaHabilidad;

    
    string PocionChec = "";
    void Start()
    {
        ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        DatosArmas = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>();
        // Buscar el script Scr_Habilidades en el mismo objeto o en otro específico
        Pociones = GetComponent<Scr_ControladorPociones>();
    }
    private void OnEnable()
    {
        ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        DatosArmas = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>();
        // Buscar el script Scr_Habilidades en el mismo objeto o en otro específico
        Pociones = GetComponent<Scr_ControladorPociones>();
        ObtenerPocion();
    }
    void Update()
    {
        if (Espermanente) return;
        ActivarHabilidad();
        if (TiempoActual > 0)
        {
            TiempoActual -= Time.deltaTime;

            Bloqueo.SetActive(true);
            TextoTiempo.SetActive(true);

            TextoTiempo.GetComponent<TextMeshProUGUI>().text = ((int)TiempoActual + 1).ToString();

            float porcentajeTiempo = TiempoActual / TiempoMaximo;

            if (porcentajeTiempo > 0.66f)
            {
                Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[0];
                TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[0];
            }
            else if (porcentajeTiempo > 0.33f)
            {
                Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[1];
                TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[1];
            }
            else
            {
                Bloqueo.GetComponent<Image>().sprite = IconosBloqueo[2];
                TextoTiempo.GetComponent<TextMeshProUGUI>().color = Colores[2];
            }
        }
        else
        {
            TiempoActual = 0;

            Bloqueo.SetActive(false);
            TextoTiempo.SetActive(false);
        }
    }

    private void ActivarHabilidad()
    {
        if (Input.GetKeyDown(Tecla) && !Espermanente)
        {
            if (Pociones != null )
            {
                if (TiempoActual == 0 && usos != 0)
                {
                    Pociones.Pociones(PocionChec);
                    TiempoActual = TiempoMaximo;
                    usos--;
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
