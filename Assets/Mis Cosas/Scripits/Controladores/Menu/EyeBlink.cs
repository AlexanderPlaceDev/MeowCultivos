using UnityEngine;
using System.Collections;

public class EyeBlink : MonoBehaviour
{
    [Header("Materiales del ojo")]
    public Material materialOjoAbierto;   // Material cuando el ojo está abierto
    public Material materialOjoCerrado;   // Material cuando el ojo está cerrado

    [Header("Configuración del parpadeo")]
    [Tooltip("Tiempo mínimo entre parpadeos (segundos)")]
    public float intervaloMinimo = 2f;

    [Tooltip("Tiempo máximo entre parpadeos (segundos)")]
    public float intervaloMaximo = 5f;

    [Tooltip("Duración del ojo cerrado (segundos)")]
    public float duracionCerrado = 0.15f;

    private MeshRenderer renderizador;
    private Material[] materialesActuales;
    private float temporizador;

    private void Start()
    {
        renderizador = GetComponent<MeshRenderer>();

        if (renderizador == null)
        {
            Debug.LogError("❌ No se encontró un MeshRenderer en este objeto.");
            enabled = false;
            return;
        }
        // Guardamos los materiales actuales del objeto
        materialesActuales = renderizador.materials;

        // Definimos el primer intervalo aleatorio
        temporizador = Random.Range(intervaloMinimo, intervaloMaximo);
    }

    private void Update()
    {
        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            StartCoroutine(Parpadear());
            // Reiniciamos el temporizador con un nuevo valor aleatorio
            temporizador = Random.Range(intervaloMinimo, intervaloMaximo);
        }
    }

    private IEnumerator Parpadear()
    {
        // Cambia al material del ojo cerrado
        materialesActuales[1] = materialOjoCerrado;
        renderizador.materials = materialesActuales;

        yield return new WaitForSeconds(duracionCerrado);

        // Regresa al material del ojo abierto
        materialesActuales[1] = materialOjoAbierto;
        renderizador.materials = materialesActuales;
    }

    // Método opcional para forzar el parpadeo manualmente desde otro script o evento
    public void ForzarParpadeo()
    {
        StartCoroutine(Parpadear());
    }
}
