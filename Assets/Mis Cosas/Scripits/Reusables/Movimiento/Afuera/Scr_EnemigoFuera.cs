using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoFuera : MonoBehaviour
{
    [SerializeField] public int VidaActual;
    [SerializeField] public int VidaMaxima;
    [SerializeField] public int Velocidad;
    [SerializeField] public float RadioDeDeambulacion;

    private NavMeshAgent agente;
    private Vector3 ultimaDireccion;
    private bool movimientoInvertido;

    private void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (agente == null || !agente.isActiveAndEnabled)
            return;

        if (agente.velocity.sqrMagnitude > 0.01f)
            ultimaDireccion = agente.velocity.normalized;
    }

    public void InvertirMovimiento()
    {
        if (agente == null || !agente.isOnNavMesh)
            return;

        movimientoInvertido = true;

        Vector3 direccion = ultimaDireccion;

        if (direccion.sqrMagnitude < 0.01f)
            direccion = transform.forward;

        direccion.y = 0f;
        direccion.Normalize();

        agente.isStopped = false;
        agente.SetDestination(transform.position - direccion * 5f);
    }

    public void DesbloquearMovimiento()
    {
        movimientoInvertido = false;
    }

    void Morir()
    {
        Destroy(gameObject);
    }
}