using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_GirarCamaraBatalla : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensibilidad del ratón

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // Bloquea el cursor en el centro de la pantalla y lo hace invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Obtener el movimiento del ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Ajustar la rotación en el eje X (vertical) e invertir el movimiento del ratón
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar la rotación para evitar voltear la cámara

        // Ajustar la rotación en el eje Y (horizontal)
        yRotation += mouseX;

        // Aplicar la rotación a la cámara
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
