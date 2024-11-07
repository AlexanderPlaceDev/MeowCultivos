using UnityEngine;

public class Scr_GirarCamaraBatalla : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float smoothTime = 0.1f; // Ajusta según el nivel de suavizado deseado

    private float xRotation = 0f;
    private float yRotation = 0f;
    private float currentXVelocity;
    private float currentYVelocity;
    private Transform playerBody;

    float targetMouseX = 0;
    float targetMouseY = 0;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = transform.parent;

        if (playerBody == null)
        {
            Debug.LogError("La cámara no tiene un objeto padre asignado.");
        }
    }

    void Update()
    {
        // Obtener la entrada del ratón
        targetMouseX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        targetMouseY += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        
    }

    private void FixedUpdate()
    {
        // Calcular la rotación en el eje Y (horizontal) suavizada
        yRotation += targetMouseX;
        float smoothYRotation = Mathf.SmoothDampAngle(playerBody.eulerAngles.y, yRotation, ref currentYVelocity, smoothTime);

        // Calcular la rotación en el eje X (vertical) suavizada y limitar el ángulo
        xRotation -= targetMouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        float smoothXRotation = Mathf.SmoothDampAngle(transform.localEulerAngles.x, xRotation, ref currentXVelocity, smoothTime);

        // Aplicar rotaciones suavizadas
        transform.localRotation = Quaternion.Euler(smoothXRotation, 0f, 0f);
        playerBody.rotation = Quaternion.Euler(0f, smoothYRotation, 0f);

        targetMouseX = 0;
        targetMouseY = 0;
    }
}
