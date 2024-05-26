using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_GirarCamaraBatalla : MonoBehaviour
{
    public float mouseSensitivity = 100.0f; // Sensibilidad del mouse
    public Transform playerBody; // El cuerpo del jugador (probablemente el objeto padre de la c�mara)

    private float xRotation = 0f;

    private void Start()
    {
        // Bloquear el cursor en el centro de la pantalla y ocultarlo
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Obtener el movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Aplicar la rotaci�n horizontal al cuerpo del jugador
        playerBody.Rotate(Vector3.up * mouseX);

        // Aplicar la rotaci�n vertical a la c�mara
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar la rotaci�n vertical

        // Aplicar la rotaci�n en el eje X a la c�mara
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
