using UnityEngine;

public class Scr_GirarCamaraBatalla : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensibilidad del ratón

    private float xRotation = 0f;
    private Transform playerBody;

    void Start()
    {
        // Bloquea el cursor en el centro de la pantalla y lo hace invisible
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = transform.parent; // Asumiendo que la cámara es hija del objeto del personaje

        // Verificar que playerBody no sea nulo
        if (playerBody == null)
        {
            Debug.LogError("La cámara no tiene un objeto padre asignado. Asegúrate de que la cámara es hija del personaje.");
        }
    }

    void Update()
    {
        // Verificar que playerBody no sea nulo antes de continuar
        if (playerBody == null) return;

        // Obtener el movimiento del ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Ajustar la rotación en el eje X (vertical) e invertir el movimiento del ratón
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar la rotación para evitar voltear la cámara

        // Ajustar la rotación en el eje Y (horizontal) del objeto padre (personaje)
        playerBody.Rotate(Vector3.up * mouseX);

        // Aplicar la rotación en el eje X a la cámara
        transform.localEulerAngles = new Vector3(xRotation, 0f, 0f);
    }
}