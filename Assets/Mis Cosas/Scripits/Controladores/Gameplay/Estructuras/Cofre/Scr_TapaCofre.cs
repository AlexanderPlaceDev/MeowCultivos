using UnityEngine;

public class Scr_TapaCofre : MonoBehaviour
{
    [SerializeField] private Vector3 RotacionOriginal;
    [SerializeField] private Vector3 RotacionAbierta;
    [SerializeField] private GameObject ObjetoObservado;

    private Quaternion rotOriginal;
    private Quaternion rotAbierta;

    private float tiempo = 0f;
    private float duracion = 1f;

    private Quaternion rotInicio;
    private Quaternion rotObjetivo;

    private bool estadoAnterior;

    void Start()
    {
        rotOriginal = Quaternion.Euler(RotacionOriginal);
        rotAbierta = Quaternion.Euler(RotacionAbierta);

        transform.localRotation = rotOriginal;

        estadoAnterior = ObjetoObservado.activeSelf;
        rotInicio = transform.localRotation;
        rotObjetivo = estadoAnterior ? rotAbierta : rotOriginal;
    }

    void Update()
    {
        bool estadoActual = ObjetoObservado.activeSelf;

        if (estadoActual != estadoAnterior)
        {
            estadoAnterior = estadoActual;

            tiempo = 0f;
            rotInicio = transform.localRotation;
            rotObjetivo = estadoActual ? rotAbierta : rotOriginal;
        }

        if (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;

            transform.localRotation = Quaternion.Slerp(rotInicio, rotObjetivo, t);
        }
    }
}