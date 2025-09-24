using UnityEngine;
using UnityEngine.AI;

public class scr_Luciernaga : MonoBehaviour
{
    [Header("Tamaño")]
    public float TamañoMaximo;
    public float TamañoMinimo;

    [Header("Movimiento")]
    public float radioMovimiento = 10f;
    public float velocidad = 3.5f; // velocidad base

    [Header("Tiempos de espera")]
    public float tiempoEsperaMin = 2f;
    public float tiempoEsperaMax = 5f;

    [Header("Oscilación Hijos")]
    public int alturaMinY = 0;       // Valor mínimo en Y
    public int alturaMaxY = 2;       // Valor máximo en Y
    public float duracionOscilacion = 2f; // Tiempo de un ciclo completo

    [Header("Titileo Alpha")]
    public float duracionTitileo = 2f; // Tiempo de un ciclo completo de alpha

    private NavMeshAgent agent;
    private bool esperando = false;

    private Transform[] hijos;
    private Material[] materiales;
    private Vector3[] posicionesIniciales;

    // Offsets
    private float[] titileoOffsets;
    private float oscilacionOffset;   // nuevo offset de oscilación

    void Start()
    {
        // Escala aleatoria
        float tamaño = Random.Range(TamañoMinimo, TamañoMaximo);
        transform.localScale = new Vector3(tamaño, tamaño, tamaño);

        // Velocidad aleatoria
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocidad * Random.Range(0.5f, 1.5f);
        MoverANuevaPosicion();

        // Guardar referencias a los hijos y materiales
        hijos = new Transform[transform.childCount];
        materiales = new Material[hijos.Length];
        posicionesIniciales = new Vector3[hijos.Length];
        titileoOffsets = new float[hijos.Length];

        for (int i = 0; i < transform.childCount; i++)
        {
            hijos[i] = transform.GetChild(i);
            posicionesIniciales[i] = hijos[i].localPosition;

            Renderer rend = hijos[i].GetComponent<Renderer>();
            if (rend != null)
            {
                // Clonamos el material para no afectar a otros objetos
                materiales[i] = rend.material;
            }

            // Offset aleatorio para titileo
            titileoOffsets[i] = Random.Range(0f, duracionTitileo);
        }

        // Offset aleatorio para oscilación
        oscilacionOffset = Random.Range(0f, duracionOscilacion);
    }

    void Update()
    {
        if (!esperando && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(EsperarYSeguir());
        }

        ActualizarOscilacion();
        ActualizarTitileo();
    }

    private void MoverANuevaPosicion()
    {
        Vector3 puntoAleatorio = Random.insideUnitSphere * radioMovimiento;
        puntoAleatorio += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(puntoAleatorio, out hit, radioMovimiento, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private System.Collections.IEnumerator EsperarYSeguir()
    {
        esperando = true;
        float tiempoEspera = Random.Range(tiempoEsperaMin, tiempoEsperaMax);
        yield return new WaitForSeconds(tiempoEspera);
        MoverANuevaPosicion();
        esperando = false;
    }

    private void ActualizarOscilacion()
    {
        if (hijos == null || hijos.Length == 0) return;

        // Se añade offset para que no estén sincronizadas
        float t = Mathf.PingPong((Time.time + oscilacionOffset) / duracionOscilacion, 1f);
        float nuevaY = Mathf.Lerp(alturaMinY, alturaMaxY, t);

        for (int i = 0; i < hijos.Length; i++)
        {
            Vector3 pos = posicionesIniciales[i];
            pos.y += nuevaY;
            hijos[i].localPosition = pos;
        }
    }

    private void ActualizarTitileo()
    {
        if (materiales == null || materiales.Length == 0) return;

        for (int i = 0; i < materiales.Length; i++)
        {
            if (materiales[i] != null)
            {
                float t = Mathf.PingPong((Time.time + titileoOffsets[i]) / duracionTitileo, 1f);
                float alpha = Mathf.Lerp(0f, 1f, t);
                materiales[i].SetFloat("_Alpha", alpha);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioMovimiento);
    }
}
