using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;

public class InputIconProvider : MonoBehaviour
{
    public static InputIconProvider Instance;

    [Header("Icon Sets por defecto")]
    public InputIconSet xboxGenericIcons;
    public InputIconSet playStationIcons;
    public InputIconSet tecladoIcons; // opcional

    [HideInInspector]
    public InputIconSet IconSetActual;

    private InputDevice lastDevice;
    public Sprite[] Iconos;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Suscribirse a la detección global de botones
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDestroy()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        // Ignorar eventos de tipo stateChange sin botones presionados
        if (device != null && eventPtr.IsA<StateEvent>() || eventPtr.IsA<DeltaStateEvent>())
        {
            lastDevice = device;
            UpdateIconSet();
        }
    }

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
        else
            IconSetActual = tecladoIcons != null ? tecladoIcons : xboxGenericIcons;
    }

    /// <summary>
    /// Devuelve el sprite según el control de la acción
    /// </summary>
    public Sprite GetIcon(InputAction action)
    {
        if (action == null || action.controls.Count == 0) return null;

        UpdateIconSet();
        InputControl control = null;

        // Buscar gamepad primero
        foreach (var c in action.controls)
        {
            if (c.device is Gamepad)
            {
                control = c;
                break;
            }
        }

        if (control == null)
            control = action.controls[0];

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

    /// <summary>
    /// Devuelve el texto de la tecla si es teclado/mouse
    /// </summary>
    public string GetKeyText(InputAction action)
    {
        if (action == null || action.controls.Count == 0) return "";

        foreach (var c in action.controls)
        {
            if (c.device is Keyboard || c.device is Mouse)
            {
                return c.displayName; // Esto da "E", "Space", "Left Mouse Button", etc.
            }
        }
        return "";
    }


    public void ActualizarIconoUI(InputAction action, Transform uiTransform, ref Sprite iconoActual, ref string textoActual, bool cambiarTam)
    {
        if (action == null) return;
        if (UsandoGamepad())
        {
            Sprite nuevoIcono = GetIcon(action);
            if (iconoActual != nuevoIcono)
            {
                Image img = uiTransform.GetComponent<Image>();

                if (img == null)
                {
                    uiTransform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    uiTransform.GetComponent<SpriteRenderer>().sprite = nuevoIcono;
                }
                else
                {
                    uiTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    uiTransform.GetComponent<Image>().sprite = nuevoIcono;
                }
                uiTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                iconoActual = nuevoIcono;
                textoActual = "";
                if (cambiarTam) 
                {
                    uiTransform.transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
        else
        {
            string tecla = GetKeyText(action);
            if (textoActual != tecla)
            {
                Image img = uiTransform.GetComponent<Image>();

                if (img == null)
                {
                    uiTransform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = tecla;
                    uiTransform.GetComponent<SpriteRenderer>().sprite = Iconos[0];
                }
                else
                {
                    uiTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tecla;
                    uiTransform.GetComponent<Image>().sprite = Iconos[0];
                }
                textoActual = tecla;
                iconoActual = Iconos[0]; 
                if (cambiarTam)
                {
                    uiTransform.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                }
            }
        }
    }

}
