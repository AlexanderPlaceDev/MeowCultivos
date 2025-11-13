using UnityEngine;

public class Scr_PerseguidorSonido : MonoBehaviour
{
    [Header("Puntos de la recta 3D")]
    [Tooltip("Punto inicial del segmento (A)")]
    [SerializeField] private Transform posicionA;

    [Tooltip("Punto final del segmento (B)")]
    [SerializeField] private Transform posicionB;

    [Header("Configuración de movimiento")]
    [Tooltip("Velocidad con la que el perseguidor se ajusta al punto proyectado")]
    [SerializeField] private float velocidad = 5f;

    [Tooltip("Si está activado, el objeto rota para mirar hacia Gata")]
    [SerializeField] private bool mirarHaciaGata = true;

    private Transform gata;

    void Start()
    {
        // Buscar a la Gata por tag
        GameObject objGata = GameObject.FindGameObjectWithTag("Gata");
        if (objGata != null)
        {
            gata = objGata.transform;
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró ningún objeto con el tag 'Gata'. Asegúrate de que tenga el tag correcto.");
        }
    }

    void Update()
    {
        if (gata == null || posicionA == null || posicionB == null)
            return;

        // Calcula el punto más cercano en el segmento 3D A–B
        Vector3 puntoMasCercano = ObtenerPuntoMasCercano3D(gata.position);

        // Movimiento suave hacia ese punto
        transform.position = Vector3.Lerp(transform.position, puntoMasCercano, Time.deltaTime * velocidad);

        // Rotar para mirar hacia Gata si está activo
        if (mirarHaciaGata)
        {
            Vector3 direccion = gata.position - transform.position;
            if (direccion.sqrMagnitude > 0.001f)
            {
                Quaternion rot = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
            }
        }
    }

    /// <summary>
    /// Devuelve el punto más cercano a 'posicionObjetivo' sobre el segmento 3D A–B.
    /// </summary>
    private Vector3 ObtenerPuntoMasCercano3D(Vector3 posicionObjetivo)
    {
        Vector3 A = posicionA.position;
        Vector3 B = posicionB.position;

        // Vector AB (la recta)
        Vector3 AB = B - A;
        // Vector desde A hasta la Gata
        Vector3 AG = posicionObjetivo - A;

        // Proyección escalar de AG sobre AB
        float t = Vector3.Dot(AG, AB) / Vector3.Dot(AB, AB);

        // Clampear para mantenerlo dentro del segmento (0 = A, 1 = B)
        t = Mathf.Clamp01(t);

        // Punto proyectado sobre la línea
        return A + AB * t;
    }

    // Gizmos para visualizar la línea y el punto actual
    void OnDrawGizmos()
    {
        if (posicionA != null && posicionB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(posicionA.position, posicionB.position);
            Gizmos.DrawSphere(posicionA.position, 0.1f);
            Gizmos.DrawSphere(posicionB.position, 0.1f);

            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, 0.1f);
            }
        }
    }
}
