using UnityEngine;

public class Scr_WindEffect : MonoBehaviour
{
    public float windStrength = 0.5f;       // Fuerza del viento
    public float baseWindSpeed = 0.1f;      // Velocidad constante del viento
    public float windAffectZone = 0.5f;     // Controla qué parte del objeto será afectada (0: nada, 1: todo)

    private Mesh mesh;
    private Vector3[] originalVertices;
    private float minZ, maxZ;               // Valores mínimo y máximo de Z en los vértices
    private float windOffset;               // Desfase de tiempo único para cada árbol

    void Start()
    {
        // Obtiene el mesh del objeto
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;

        // Calcula la posición mínima y máxima en el eje Z de los vértices
        minZ = float.MaxValue;
        maxZ = float.MinValue;

        foreach (var vertex in originalVertices)
        {
            if (vertex.z < minZ) minZ = vertex.z;
            if (vertex.z > maxZ) maxZ = vertex.z;
        }

        // Genera un desfase de tiempo aleatorio para cada árbol
        windOffset = Random.Range(0f, 2f * Mathf.PI);  // Valor entre 0 y 2π para variar el desfase del seno
    }

    void Update()
    {
        // Usa la velocidad constante del viento (baseWindSpeed)
        float windSpeed = baseWindSpeed;  // Mantener una velocidad constante

        Vector3[] vertices = new Vector3[originalVertices.Length];

        // Recorre los vértices originales para aplicar el movimiento
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];

            // Determina el rango en el que el viento afecta, basado en windAffectZone
            float normalizedZ = Mathf.InverseLerp(minZ, maxZ, vertex.z);  // Normaliza el valor Z (0 a 1)

            if (normalizedZ >= 1.0f - windAffectZone)  // Solo mueve si está en la zona afectada
            {
                // Movimiento en el eje X simulando viento, solo para vértices en el rango afectado
                float heightFactor = Mathf.Clamp01(vertex.y / 5.0f);  // Ajusta según la altura del árbol

                // Aplicar el desfase (windOffset) para que cada árbol esté desincronizado
                vertex.x += Mathf.Sin(Time.time * windSpeed + vertex.x + windOffset) * windStrength * heightFactor;
            }

            vertices[i] = vertex;
        }

        // Actualiza los vértices del mesh
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
