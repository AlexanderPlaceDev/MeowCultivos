using UnityEngine;
using UnityEngine.AI;

public class Scr_EnemigoFuera : MonoBehaviour
{
    [SerializeField] public int VidaActual;
    [SerializeField] public int VidaMaxima;
    [SerializeField] public int Velocidad;
    [SerializeField] public float RadioDeDeambulacion;

    private NavMeshAgent agenteEnemigo;
    private Vector3 ultimaDireccion;
    public bool movimientoInvertido=false;

    private void Awake()
    {
        agenteEnemigo = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (agenteEnemigo == null || !agenteEnemigo.isActiveAndEnabled)
            return;

        if (agenteEnemigo.velocity.sqrMagnitude > 0.01f)
            ultimaDireccion = agenteEnemigo.velocity.normalized;
    }

    public void InvertirMovimiento()
    {
        if (agenteEnemigo == null || !agenteEnemigo.isOnNavMesh)
            return;

        movimientoInvertido = true;

        Vector3 direccion = ultimaDireccion;

        if (direccion.sqrMagnitude < 0.01f)
            direccion = transform.forward;

        direccion.y = 0f;
        direccion.Normalize();

        agenteEnemigo.isStopped = false;
        agenteEnemigo.SetDestination(transform.position - direccion * 5f);
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