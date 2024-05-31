using UnityEngine;

public class Scr_GirarCamaraBatalla : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensibilidad del rat�n

    private float xRotation = 0f;
    private Transform playerBody;

    void Start()
    {
        // Bloquea el cursor en el centro de la pantalla y lo hace invisible
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = transform.parent; // Asumiendo que la c�mara es hija del objeto del personaje

        // Verificar que playerBody no sea nulo
        if (playerBody == null)
        {
            Debug.LogError("La c�mara no tiene un objeto padre asignado. Aseg�rate de que la c�mara es hija del personaje.");
        }
    }

    void Update()
    {
        // Verificar que playerBody no sea nulo antes de continuar
        if (playerBody == null) return;

        // Obtener el movimiento del rat�n
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Ajustar la rotaci�n en el eje X (vertical) e invertir el movimiento del rat�n
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar la rotaci�n para evitar voltear la c�mara

        // Ajustar la rotaci�n en el eje Y (horizontal) del objeto padre (personaje)
        playerBody.Rotate(Vector3.up * mouseX);

        // Aplicar la rotaci�n en el eje X a la c�mara
        transform.localEulerAngles = new Vector3(xRotation, 0f, 0f);
    }
}