using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class Scr_MiniJuegoPesca : MonoBehaviour
{
    // Evento: pide cambio de cámara
    public static event Action OnSolicitarCambioCamara;

    [Header("Pivotes")]
    [SerializeField] RectTransform PivotePez;
    [SerializeField] RectTransform PivoteAnzuelo;

    [Header("Configuración")]
    [SerializeField] int dificultad = 1;
    [SerializeField] float Tolerancia = 9f;
    [SerializeField] float RotacionMaxima = 51f;

    [Header("Pez vivo")]
    [SerializeField] float velocidadBasePez = 30f;
    [SerializeField] float pausaMin = 0.15f;
    [SerializeField] float pausaMax = 1.2f;
    float margenExtremos = 5f; // grados


    [Header("Anzuelo")]
    [SerializeField] float velocidadAnzuelo = 120f;

    [Header("Progreso")]
    [SerializeField] Image Carga;
    public static event Action<bool> OnFinMiniJuego;
    // true = ganó, false = perdió


    Color colorRojo = Color.red;
    Color colorAmarillo = Color.yellow;
    Color colorVerde = Color.green;

    float rotacionPez;
    float rotacionAnzuelo;
    float direccionPez = 1f;

    float contadorCaptura;
    float tiempoPausa;
    bool pezPausado;

    // 🔹 NUEVO: control de pausas aleatorias
    float tiempoParaEvaluarPausa;

    // 🔹 NUEVO: cooldown de cambio de cámara
    float cooldownCambioCamara = 1f;
    float tiempoCambioCamara;

    PlayerInput playerInput;
    InputAction MoverHorizontal;

    void Start()
    {
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        MoverHorizontal = playerInput.actions["MoverHorizontal"];

        ProgramarPausa();
        ProgramarEvaluacionPausa();
    }

    void Update()
    {
        ActualizarCooldownCamara();
        ActualizarPez();
        ActualizarAnzuelo();
        EvaluarCaptura();
        ActualizarUI();
    }

    // ------------------ PEZ ------------------
    void ActualizarPez()
    {
        if (pezPausado)
        {
            tiempoPausa -= Time.deltaTime;
            if (tiempoPausa <= 0f)
            {
                pezPausado = false;
                CambiarDireccionPez();
                ProgramarEvaluacionPausa();
            }
            return;
        }

        float velocidad = velocidadBasePez * dificultad;
        rotacionPez += direccionPez * velocidad * Time.deltaTime;

        // Clamp duro
        rotacionPez = Mathf.Clamp(rotacionPez, -RotacionMaxima, RotacionMaxima);

        // 🔹 NUEVO: pausa aleatoria en cualquier punto del arco
        tiempoParaEvaluarPausa -= Time.deltaTime;
        if (tiempoParaEvaluarPausa <= 0f)
        {
            bool lejosDeExtremos =
                rotacionPez > -RotacionMaxima + margenExtremos &&
                rotacionPez < RotacionMaxima - margenExtremos;

            if (lejosDeExtremos && UnityEngine.Random.value < 0.25f)
            {
                IniciarPausa();
                return;
            }

            ProgramarEvaluacionPausa();
        }


        if (rotacionPez <= -RotacionMaxima)
        {
            rotacionPez = -RotacionMaxima;
            direccionPez = 1f;
        }
        else if (rotacionPez >= RotacionMaxima)
        {
            rotacionPez = RotacionMaxima;
            direccionPez = -1f;
            // 🎯 Cambio de cámara con cooldown
            if (UnityEngine.Random.value < 0.5f)
                SolicitarCambioCamaraSeguro();
        }


        PivotePez.localRotation = Quaternion.Euler(0, 0, rotacionPez);
    }

    void CambiarDireccionPez()
    {
        direccionPez = UnityEngine.Random.value < 0.5f ? -1f : 1f;

        if(direccionPez == -1)
        {
            PivotePez.GetChild(1).GetComponent<RectTransform>().localRotation= Quaternion.Euler(0, -180, 0);
        }
        else
        {
            PivotePez.GetChild(1).GetComponent<RectTransform>().localRotation= Quaternion.Euler(0, 0, 0);
        }
    }

    void IniciarPausa()
    {
        pezPausado = true;
        ProgramarPausa();
    }

    void ProgramarPausa()
    {
        float factor = Mathf.Clamp01(dificultad / 10f);
        float max = Mathf.Lerp(pausaMax, pausaMin, factor);
        tiempoPausa = UnityEngine.Random.Range(pausaMin, max);
    }

    void ProgramarEvaluacionPausa()
    {
        tiempoParaEvaluarPausa = UnityEngine.Random.Range(0.3f, 0.9f);
    }

    // ------------------ COOLDOWN CÁMARA ------------------
    void ActualizarCooldownCamara()
    {
        if (tiempoCambioCamara > 0f)
            tiempoCambioCamara -= Time.deltaTime;
    }

    void SolicitarCambioCamaraSeguro()
    {
        if (tiempoCambioCamara > 0f)
            return;

        tiempoCambioCamara = cooldownCambioCamara;
        OnSolicitarCambioCamara?.Invoke();
    }

    // ------------------ ANZUELO ------------------
    void ActualizarAnzuelo()
    {
        float input = 0;
        if (transform.parent.GetChild(1).gameObject.activeSelf)
        {
            input = -MoverHorizontal.ReadValue<float>();
        }
        else
        {
            input = MoverHorizontal.ReadValue<float>();
        }

        rotacionAnzuelo += -input * velocidadAnzuelo * Time.deltaTime;

        float limite = RotacionMaxima + Tolerancia;
        rotacionAnzuelo = Mathf.Clamp(rotacionAnzuelo, -limite, limite);

        PivoteAnzuelo.localRotation = Quaternion.Euler(0, 0, rotacionAnzuelo);
    }

    // ------------------ CAPTURA ------------------
    void EvaluarCaptura()
    {
        float diferencia = Mathf.Abs(rotacionAnzuelo - rotacionPez);

        contadorCaptura += (diferencia <= Tolerancia ? 1 : -1) * Time.deltaTime;
        contadorCaptura = Mathf.Clamp(contadorCaptura, -5f, 5f);

        if (contadorCaptura >= 5f) GanarPesca();
        if (contadorCaptura <= -5f) FallarPesca();
    }

    // ------------------ UI ------------------
    void ActualizarUI()
    {
        if (!Carga) return;

        float t = Mathf.InverseLerp(-5f, 5f, contadorCaptura);
        Carga.fillAmount = t;

        if (t < 0.5f)
            Carga.color = Color.Lerp(colorRojo, colorAmarillo, t / 0.5f);
        else
            Carga.color = Color.Lerp(colorAmarillo, colorVerde, (t - 0.5f) / 0.5f);
    }

    void GanarPesca()
    {
        OnFinMiniJuego?.Invoke(true);
    }


    void FallarPesca()
    {
        OnFinMiniJuego?.Invoke(false);
    }


    public void Resetear()
    {
        // 🔹 Estado base
        rotacionPez = 0f;
        rotacionAnzuelo = 0f;
        direccionPez = 1f;
        contadorCaptura = 0f;

        pezPausado = false;
        tiempoPausa = 0f;
        tiempoParaEvaluarPausa = 0f;
        tiempoCambioCamara = 0f;

        // 🔹 Reset visual
        PivotePez.localRotation = Quaternion.identity;
        PivoteAnzuelo.localRotation = Quaternion.identity;

        // 🔹 Reprogramar lógica
        ProgramarPausa();
        ProgramarEvaluacionPausa();

        // 🔹 🔥 CLAVE: reactivar input
        if (MoverHorizontal != null)
            MoverHorizontal.Enable();

        enabled = true;
    }



}
