using UnityEngine;
using System.Collections;

public class Scr_Tambaleo : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float Intensidad = 15f;
    [SerializeField] private float Velocidad = 5f;
    [SerializeField] private int Repeticiones = 2;

    [Header("Ejes")]
    [SerializeField] private bool UsaEjeX = true;
    [SerializeField] private bool UsaEjeY = false;
    [SerializeField] private bool UsaEjeZ = true;

    private Quaternion RotacionInicial;

    private Coroutine CoroutineTambaleo;

    private void Awake()
    {
        RotacionInicial = transform.localRotation;
    }

    private void OnEnable()
    {
        RotacionInicial = transform.localRotation;

        // Comienza automáticamente al activarse
        IniciarTambaleo();
    }

    private void OnDisable()
    {
        if (CoroutineTambaleo != null)
        {
            StopCoroutine(CoroutineTambaleo);
            CoroutineTambaleo = null;
        }

        transform.localRotation = RotacionInicial;
    }

    public void IniciarTambaleo()
    {
        if (CoroutineTambaleo != null)
            StopCoroutine(CoroutineTambaleo);

        CoroutineTambaleo = StartCoroutine(Tambalear());
    }

    private IEnumerator Tambalear()
    {
        while (true)
        {
            // Generamos una rotación aleatoria
            float X = UsaEjeX ? Random.Range(-Intensidad, Intensidad) : 0f;
            float Y = UsaEjeY ? Random.Range(-Intensidad, Intensidad) : 0f;
            float Z = UsaEjeZ ? Random.Range(-Intensidad, Intensidad) : 0f;

            Quaternion RotacionObjetivo =
                RotacionInicial * Quaternion.Euler(X, Y, Z);

            // Ir hacia la nueva posición
            yield return StartCoroutine(
                RotarSuavemente(
                    transform.localRotation,
                    RotacionObjetivo
                )
            );

            // Volver al centro
            yield return StartCoroutine(
                RotarSuavemente(
                    transform.localRotation,
                    RotacionInicial
                )
            );

            // Repeticiones adicionales
            for (int i = 1; i < Repeticiones; i++)
            {
                X = UsaEjeX ? Random.Range(-Intensidad, Intensidad) : 0f;
                Y = UsaEjeY ? Random.Range(-Intensidad, Intensidad) : 0f;
                Z = UsaEjeZ ? Random.Range(-Intensidad, Intensidad) : 0f;

                RotacionObjetivo =
                    RotacionInicial * Quaternion.Euler(X, Y, Z);

                yield return StartCoroutine(
                    RotarSuavemente(
                        transform.localRotation,
                        RotacionObjetivo
                    )
                );

                yield return StartCoroutine(
                    RotarSuavemente(
                        transform.localRotation,
                        RotacionInicial
                    )
                );
            }
        }
    }

    private IEnumerator RotarSuavemente(
        Quaternion Desde,
        Quaternion Hacia)
    {
        float Tiempo = 0f;

        while (Tiempo < 1f)
        {
            Tiempo += Time.deltaTime * Velocidad;

            transform.localRotation =
                Quaternion.Slerp(
                    Desde,
                    Hacia,
                    Mathf.SmoothStep(0f, 1f, Tiempo)
                );

            yield return null;
        }

        transform.localRotation = Hacia;
    }
}