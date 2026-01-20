using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
public class VirtualMouseGamepad : MonoBehaviour
{
    [SerializeField]
    public PlayerInput playerInput;

    [SerializeField]
    private RectTransform cursorTransform;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform canvasRectTransForm;

    [SerializeField]
    private float cursorSpeed = 1000;
    [SerializeField]
    private float padding = 35f;

    private bool previousMouseState;
    private Mouse virtualMouse;
    private Mouse currentMouse;
    private Camera mainCamera;

    private bool TieneControl;

    private string PreviousControlSheme = "";
    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard&Mouse";
    private const string playControllerScheme = "PlayController";  // Agregado para el esquema de PlayStation
    private const string uiActionMap = "UI"; // El nombre del Action Map de UI

    InputIconProvider IconProvider;
    private void OnEnable()
    {
        mainCamera = Camera.main;
        currentMouse = Mouse.current;

        // Asegúrate de que el mouse virtual se cree si no existe
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice("VirtualMouse");
        }

        // Empareja el ratón virtual con el usuario
        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        // Inicializa la posición del cursor en el centro de la pantalla
        if (cursorTransform != null)
        {
            Vector2 centerPosition = new Vector2(Screen.width / 2, Screen.height / 2);
            InputState.Change(virtualMouse.position, centerPosition);
        }

        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();

        InputSystem.onAfterUpdate += UpdateMotion;
        playerInput.onControlsChanged += OnControlsChange;
    }

    private void OnDisable()
    {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChange;
    }

    private void Update()
    {
        if (playerInput.currentActionMap.name == uiActionMap && Gamepad.current != null)
        {
            // Si no es el Action Map de UI, desactiva el cursor virtual
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
        }
        else
        {
            // Si es el Action Map de UI, activa el cursor virtual
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
        }
    }

    private void UpdateMotion()
    {
        // Asegúrate de que no haya controladores nulos
        if (virtualMouse == null || Gamepad.current == null){ return; }
        // Lee el movimiento del stick izquierdo
        Vector2 DeltaValue = Gamepad.current.leftStick.ReadValue();
        DeltaValue *= cursorSpeed * Time.deltaTime;

        // Obtiene la posición actual del ratón y calcula la nueva posición
        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPostition = currentPosition + DeltaValue;

        // Limita la posición para evitar que se salga de la pantalla
        newPostition.x = Mathf.Clamp(newPostition.x, padding, Screen.width - padding);
        newPostition.y = Mathf.Clamp(newPostition.y, padding, Screen.height - padding);

        // Actualiza el estado del ratón virtual
        InputState.Change(virtualMouse.position, newPostition);
        InputState.Change(virtualMouse.delta, DeltaValue);

        // Gestiona el clic del botón A
        bool aButtonIspressed = Gamepad.current.aButton.IsPressed();
        if (previousMouseState != aButtonIspressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, aButtonIspressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = aButtonIspressed;
        }

        // Actualiza la posición del cursor en la UI
        AnchorCursor(newPostition);
    }

    private void AnchorCursor(Vector2 position)
    {
        // Convierte la posición de la pantalla a coordenadas locales de la UI
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransForm, position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }

    private void OnControlsChange(PlayerInput input)
    {
        // Detecta el esquema de control y actúa en consecuencia
        if (playerInput.currentControlScheme == mouseScheme && PreviousControlSheme != mouseScheme)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            PreviousControlSheme = mouseScheme;
            Debug.LogError("es mouse");
        }
        else if (playerInput.currentControlScheme == gamepadScheme && PreviousControlSheme != gamepadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, currentMouse.position.ReadDefaultValue());
            AnchorCursor(currentMouse.position.ReadValue());
            PreviousControlSheme = gamepadScheme;
            Debug.LogError("es pad");
        }
        else if (playerInput.currentControlScheme == playControllerScheme && PreviousControlSheme != playControllerScheme)
        {
            // Agregado manejo para el controlador de PlayStation
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, currentMouse.position.ReadDefaultValue());
            AnchorCursor(currentMouse.position.ReadValue());
            PreviousControlSheme = playControllerScheme;

            Debug.LogError("es play");
        }
    }

}
