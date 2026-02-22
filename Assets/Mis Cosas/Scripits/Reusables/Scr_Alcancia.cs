using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_Alcancia : MonoBehaviour
{
    private double dineroGuardado = 0;
    private int dineroActual = 0;

    [Header("Configuración")]
    public double interesDiario = 0.02; // 2%

    [Header("UI")]
    public TextMeshProUGUI DineroGuardadoTXT;
    public TextMeshProUGUI DineroActualTXT;
    public TextMeshProUGUI DineroaGuardarTXT;

    private Scr_ControladorTiempo tiempo;

    private const string KEY_DINERO_GUARDADO = "AlcanciaDinero";
    private const string KEY_ULTIMO_DIA = "AlcanciaUltimoDia";
    private const string KEY_DINERO_JUGADOR = "Dinero";

    void Start()
    {
        tiempo = FindObjectOfType<Scr_ControladorTiempo>();
        ActualizarBanco();
    }
    private void OnEnable()
    {
        ActualizarBanco();
    }
    // ==============================
    // INICIALIZAR / ACTUALIZAR
    // ==============================

    public void ActualizarBanco()
    {
        CargarDatos();
        VerificarNuevoDia();
        DineroaGuardarTXT.text = "0";
        ActualizarUI();
    }

    void ActualizarUI()
    {
        DineroGuardadoTXT.text = "Dinero Guardado\n" + Mathf.FloorToInt((float)dineroGuardado);
        DineroActualTXT.text = "Dinero Actual\n" + dineroActual;
    }

    // ==============================
    // DEPÓSITO
    // ==============================

    public void Depositar()
    {
        if (!int.TryParse(DineroaGuardarTXT.text, out int cantidad))
            return;

        if (cantidad <= 0 || cantidad > dineroActual)
            return;

        dineroGuardado += cantidad;
        dineroActual -= cantidad;

        PlayerPrefs.SetInt(KEY_DINERO_JUGADOR, dineroActual);

        DineroaGuardarTXT.text = "0";

        GuardarDatos();
        ActualizarUI();
    }

    // ==============================
    // RETIRAR TODO
    // ==============================

    public void Reclamar()
    {
        int total = PlayerPrefs.GetInt(KEY_DINERO_JUGADOR, 0) +
                    Mathf.FloorToInt((float)dineroGuardado);

        PlayerPrefs.SetInt(KEY_DINERO_JUGADOR, total);
        PlayerPrefs.Save();

        dineroActual = total;
        dineroGuardado = 0;

        GuardarDatos();
        ActualizarUI();
    }

    // ==============================
    // INTERÉS DIARIO
    // ==============================

    public void VerificarNuevoDia()
    {
        string ultimoDiaGuardado = PlayerPrefs.GetString(KEY_ULTIMO_DIA, "");

        if (ultimoDiaGuardado != tiempo.DiaActual)
        {
            AplicarInteres();
            PlayerPrefs.SetString(KEY_ULTIMO_DIA, tiempo.DiaActual);
            PlayerPrefs.Save();
        }
    }

    void AplicarInteres()
    {
        if (dineroGuardado <= 0)
            return;

        dineroGuardado += dineroGuardado * interesDiario;

        Debug.Log("Interés aplicado. Nuevo total: " + dineroGuardado);

        GuardarDatos();
    }

    // ==============================
    // BOTONES DE CANTIDAD
    // ==============================

    public void Dar_Mitad()
    {
        int cantidad = dineroActual / 2;
        DineroaGuardarTXT.text = cantidad.ToString();
    }

    public void Dar_Maximo()
    {
        DineroaGuardarTXT.text = dineroActual.ToString();
    }

    public void Dar_Uno()
    {
        if (dineroActual <= 0)
            return;

        if (!int.TryParse(DineroaGuardarTXT.text, out int cantidadActual))
            cantidadActual = 0;

        dineroActual--;
        cantidadActual++;

        DineroaGuardarTXT.text = cantidadActual.ToString();
        ActualizarUI();
    }

    public void Quitar_Uno()
    {
        if (!int.TryParse(DineroaGuardarTXT.text, out int cantidadActual))
            return;

        if (cantidadActual <= 0)
            return;

        dineroActual++;
        cantidadActual--;

        DineroaGuardarTXT.text = cantidadActual.ToString();
        ActualizarUI();
    }

    // ==============================
    // GUARDAR / CARGAR
    // ==============================

    void GuardarDatos()
    {
        PlayerPrefs.SetString(KEY_DINERO_GUARDADO, dineroGuardado.ToString());
        PlayerPrefs.SetInt(KEY_DINERO_JUGADOR, dineroActual);
        PlayerPrefs.Save();
    }

    void CargarDatos()
    {
        dineroGuardado = double.Parse(PlayerPrefs.GetString(KEY_DINERO_GUARDADO, "0"));
        dineroActual = PlayerPrefs.GetInt(KEY_DINERO_JUGADOR, 0);
    }
}
