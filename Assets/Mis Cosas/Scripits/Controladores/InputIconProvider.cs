using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class InputIconProvider : MonoBehaviour
{
    public static InputIconProvider Instance;

    [Header("Icon Sets Gamepad")]
    public InputIconSet xboxGenericIcons;
    public InputIconSet playStationIcons;

    [HideInInspector]
    public InputIconSet IconSetActual;

    [Header("Icono base teclado")]
    public Sprite tecladoBaseIcon;
    public Sprite tecladoMoreIcon;
    [Header("Iconos especiales de Mouse")]
    public Sprite mouseLeftIcon;
    public Sprite mouseRightIcon;
    public Sprite mouseMiddleIcon;
    public Sprite mouseBackIcon;
    public Sprite mouseForwardIcon;
    public Sprite mouseScrollUpIcon;
    public Sprite mouseScrollDownIcon;

    private InputDevice lastDevice;

    private const float STICK_DEADZONE = 0.2f;
    private const float AXIS_DEADZONE = 0.1f;

    // ---------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Suscribirse a la detección global de inputs
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDestroy()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    // --------------------------------------------------
    // Detecta último dispositivo usado
    // --------------------------------------------------
    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (device == null) return;

        bool inputReal = false;

        if (device is Gamepad gamepad)
        {
            // Botones presionados
            inputReal = gamepad.allControls.Any(c =>
                (c is ButtonControl btn && btn.wasPressedThisFrame) ||
                (c is AxisControl axis && Mathf.Abs(axis.ReadValue()) > AXIS_DEADZONE) ||
                (c is StickControl stick && stick.ReadValue().magnitude > STICK_DEADZONE)
            );
        }
        else if (device is Keyboard || device is Mouse)
        {
            // Cualquier input de teclado/mouse
            inputReal = true;
        }

        if (!inputReal) return;
        if (lastDevice == device) return;

        lastDevice = device;
        UpdateIconSet();
    }

    // --------------------------------------------------
    public bool UsandoGamepad()
    {
        return lastDevice is Gamepad;
    }

    public bool IsPlayStation()
    {
        return lastDevice is DualShockGamepad || lastDevice is DualSenseGamepadHID;
    }

    public void UpdateIconSet()
    {
        if (UsandoGamepad())
            IconSetActual = IsPlayStation() ? playStationIcons : xboxGenericIcons;
    }

    // --------------------------------------------------
    // ICONOS GAMEPAD
    // --------------------------------------------------
    public Sprite GetGamepadIcon(InputAction action)
    {
        if (action == null || action.controls.Count == 0) return null;

        UpdateIconSet();

        InputControl control = action.controls
            .FirstOrDefault(c => c.device is Gamepad) ?? action.controls[0];

        string path = control.path.ToLower();
        InputIconSet icons = IconSetActual;

        if (path.Contains("buttonsouth")) return icons.buttonSouth;
        if (path.Contains("buttonnorth")) return icons.buttonNorth;
        if (path.Contains("buttoneast")) return icons.buttonEast;
        if (path.Contains("buttonwest")) return icons.buttonWest;
        if (path.Contains("leftshoulder")) return icons.leftShoulder;
        if (path.Contains("rightshoulder")) return icons.rightShoulder;
        if (path.Contains("lefttrigger")) return icons.leftTrigger;
        if (path.Contains("righttrigger")) return icons.rightTrigger;
        if (path.Contains("dpad/up")) return icons.dpadUp;
        if (path.Contains("dpad/down")) return icons.dpadDown;
        if (path.Contains("dpad/left")) return icons.dpadLeft;
        if (path.Contains("dpad/right")) return icons.dpadRight;
        if (path.Contains("leftstick")) return icons.leftStick;
        if (path.Contains("rightstick")) return icons.rightStick;

        return null;
    }

    // --------------------------------------------------
    // ICONOS ESPECIALES DE MOUSE
    // --------------------------------------------------
    private Sprite GetMouseIcon(InputAction action)
    {
        foreach (var c in action.controls)
        {
            if (c.device is not Mouse) continue;

            string path = c.path.ToLower();
            Debug.LogError(path);
            if (path.Contains("leftbutton")) return mouseLeftIcon;
            if (path.Contains("rightbutton")) return mouseRightIcon;
            if (path.Contains("middlebutton")) return mouseMiddleIcon;
            if (path.Contains("backbutton")) return mouseBackIcon;
            if (path.Contains("forwardbutton")) return mouseForwardIcon;

            if (path.Contains("scroll"))
            {
                float scroll = Mouse.current.scroll.ReadValue().y;
                if (scroll > 0) return mouseScrollUpIcon;
                if (scroll < 0) return mouseScrollDownIcon;
            }
        }

        return null;
    }

    // --------------------------------------------------
    // TEXTO TECLADO
    // --------------------------------------------------
    private string GetKeyText(InputAction action)
    {
        foreach (var c in action.controls)
        {
            if (c.device is Keyboard)
                return c.displayName;
        }
        return "";
    }

    // --------------------------------------------------
    // ACTUALIZACIÓN DE UI
    // --------------------------------------------------
    public void ActualizarIconoUI(
        InputAction action,
        Transform uiTransform,
        ref Sprite iconoActual,
        ref string textoActual,
        bool cambiarTam)
    {
        if (action == null) return;

        Image img = uiTransform.GetComponent<Image>();
        SpriteRenderer sr = uiTransform.GetComponent<SpriteRenderer>();
        TextMeshProUGUI txt = uiTransform.GetComponentInChildren<TextMeshProUGUI>();

        // ---------------- GAMEPAD ----------------
        if (UsandoGamepad())
        {
            Sprite icon = GetGamepadIcon(action);
            if (icon == null || iconoActual == icon) return;

            if (img) img.sprite = icon;
            if (sr) sr.sprite = icon;
            if (txt) txt.text = "";

            iconoActual = icon;
            textoActual = "";

            if (cambiarTam)
                uiTransform.localScale = Vector3.one;

            return;
        }

        // ---------------- TECLADO ----------------
        string tecla = GetKeyText(action);

        // Elegimos el icono según la longitud de la tecla
        Sprite icono = tecladoBaseIcon;

        bool ocupaText = false;
        if (tecla.Length > 1 || tecla == "")
        {
            icono = GetMouseIcon(action);
            if (icono == null)
            {
                icono = tecladoMoreIcon;
                tecla = TeclasEspeciales(tecla);
            }
            else
            {
                ocupaText = true;
            }
        }

        if (ocupaText)
        {
            if (icono == null || iconoActual == icono) return;

            if (img) img.sprite = icono;
            if (sr) sr.sprite = icono;
            if (txt) txt.text = "";

            iconoActual = icono;
            textoActual = "";

            if (cambiarTam)
                uiTransform.localScale = Vector3.one;

            return;
        }
        else
        {
            // Actualizamos solo si cambió el texto o el icono
            if (textoActual != tecla || iconoActual != icono)
            {
                if (img) img.sprite = icono;
                if (sr) sr.sprite = icono;
                if (txt) txt.text = tecla;

                textoActual = tecla;
                iconoActual = icono;

                if (cambiarTam)
                {
                    // Escala: puedes ajustar según icono
                    if (icono == tecladoMoreIcon)
                        uiTransform.localScale = Vector3.one * 1.2f;
                    else
                        uiTransform.localScale = Vector3.one * 1.5f;
                }
            }
        }
    }

    public string TeclasEspeciales(string tecla)
    {
        switch (tecla) 
        {
            case "Tabulacion":
                return "TAB";
            case "Barra Espaciadora":
                return "Space";
            case "Escape":
                return "Esc";
            default:
                return "ESP";
                    
        }
    }
}
