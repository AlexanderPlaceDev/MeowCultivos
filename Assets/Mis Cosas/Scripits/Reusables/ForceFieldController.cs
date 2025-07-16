using UnityEngine;

public class AlphaChanger : MonoBehaviour
{
    public Transform gata; // Referencia al transform de la gata
    public Transform pointA; // Punto A de la l�nea
    public Transform pointB; // Punto B de la l�nea
    public GameObject Muro;
    public float maxDistance = 10.0f; // Distancia m�xima a la que el objeto se vuelve completamente transparente
    public float minDistance = 1.0f; // Distancia m�nima a la que el objeto es completamente opaco
    public bool useXAxis = true; // Considerar el eje X
    public bool useYAxis = true; // Considerar el eje Y
    public bool useZAxis = true; // Considerar el eje Z
    private Material material; // Material que queremos modificar

    [SerializeField] Scr_SistemaDialogos Dialogo;
    [SerializeField] int NumDialogo;

    void Start()
    {
        // Obtener el material del objeto
        material = Muro.GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (Dialogo != null)
        {
            if (Dialogo.DialogoActual >= NumDialogo)
            {
                gameObject.SetActive(false);
            }
        }



        // Proyectar la posici�n de la gata en la l�nea definida por A y B
        Vector3 closestPointOnLine = ClosestPointOnLine(pointA.position, pointB.position, gata.position);

        // Si las variables de eje est�n activadas, considerar solo esos ejes
        Vector3 direction = Vector3.zero;
        if (useXAxis) direction.x = closestPointOnLine.x - gata.position.x;
        if (useYAxis) direction.y = closestPointOnLine.y - gata.position.y;
        if (useZAxis) direction.z = closestPointOnLine.z - gata.position.z;

        // Calcular la distancia considerando solo los ejes activados
        float distance = direction.magnitude;

        // Calcular el valor de alpha en funci�n de la distancia
        float alpha = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));

        // Invertir el alpha para que sea 1 cuando est� cerca y 0 cuando est� lejos
        alpha = 1.0f - alpha;

        // Aplicar el valor de alpha al material
        material.SetFloat("_Alpha", alpha);
    }

    // Funci�n para calcular el punto m�s cercano en una l�nea a un punto dado
    Vector3 ClosestPointOnLine(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 ab = b - a;
        Vector3 ap = point - a;
        float t = Mathf.Clamp01(Vector3.Dot(ap, ab) / Vector3.Dot(ab, ab));
        return a + t * ab;
    }
}
