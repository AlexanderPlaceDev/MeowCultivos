using UnityEngine;

public class Scr_GirarCamaraBatalla : MonoBehaviour
{
    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;
    public float smoothTime = 0.1f;

    private float xRotation;
    private float yRotation;
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

        // Inicializar la rotación con la del jugador actual
        yRotation = playerBody.eulerAngles.y;
        xRotation = transform.localEulerAngles.x;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (mouseX != 0 || mouseY != 0)
        {
            targetMouseX += mouseX * mouseSensitivityX * Time.deltaTime;
            targetMouseY += mouseY * mouseSensitivityY * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (targetMouseX != 0 || targetMouseY != 0)
        {
            // Calcular rotación Y (horizontal)
            yRotation += targetMouseX;
            float smoothYRotation = Mathf.SmoothDampAngle(playerBody.eulerAngles.y, yRotation, ref currentYVelocity, smoothTime);

            // Calcular rotación X (vertical)
            xRotation -= targetMouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            float smoothXRotation = Mathf.SmoothDampAngle(transform.localEulerAngles.x, xRotation, ref currentXVelocity, smoothTime);

            // Aplicar rotaciones suavizadas
            transform.localRotation = Quaternion.Euler(smoothXRotation, 0f, 0f);
            playerBody.rotation = Quaternion.Euler(0f, smoothYRotation, 0f);
        }

        targetMouseX = 0;
        targetMouseY = 0;
    }
}
