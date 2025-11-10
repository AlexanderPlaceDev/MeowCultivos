using UnityEngine;

public class Scr_GirarCamaraBatalla : MonoBehaviour
{
    [Header("Sensibilidad")]
    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;

    [Header("Suavizado")]
    [Range(0f, 1f)] public float smoothFactor = 0.05f; // entre 0.0 y 1.0 (0 = instantáneo, 1 = muy lento)

    private Transform playerBody;
    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;
    private float xRotation;
    private float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = transform.parent;

        if (playerBody == null)
            Debug.LogError("La cámara no tiene un objeto padre asignado.");

        // Inicializa las rotaciones
        yRotation = playerBody.eulerAngles.y;
        xRotation = transform.localEulerAngles.x;
    }

    void Update()
    {
        // --- Lectura del ratón ---
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta *= new Vector2(mouseSensitivityX, mouseSensitivityY) * Time.deltaTime;

        // --- Suavizado real ---
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, mouseDelta, ref currentMouseDeltaVelocity, smoothFactor);

        // --- Aplicar rotaciones ---
        yRotation += currentMouseDelta.x;
        xRotation -= currentMouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // --- Aplicar rotaciones ---
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    // 🧩 Si el jugador es golpeado o movido bruscamente
    public void ResetSuavizado()
    {
        currentMouseDelta = Vector2.zero;
        currentMouseDeltaVelocity = Vector2.zero;
    }

    // 🧩 Si se cambia la rotación del jugador manualmente (teletransporte, etc.)
    public void SincronizarRotacion()
    {
        yRotation = playerBody.eulerAngles.y;
        xRotation = transform.localEulerAngles.x;
    }
}
