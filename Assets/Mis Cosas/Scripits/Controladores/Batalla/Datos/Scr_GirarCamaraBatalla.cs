using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_GirarCamaraBatalla : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensibilidad del rat�n

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // Bloquea el cursor en el centro de la pantalla y lo hace invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Obtener el movimiento del rat�n
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Ajustar la rotaci�n en el eje X (vertical) e invertir el movimiento del rat�n
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar la rotaci�n para evitar voltear la c�mara

        // Ajustar la rotaci�n en el eje Y (horizontal)
        yRotation += mouseX;

        // Aplicar la rotaci�n a la c�mara
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
