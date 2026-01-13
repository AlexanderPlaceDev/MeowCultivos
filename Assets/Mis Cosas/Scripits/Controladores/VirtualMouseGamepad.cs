using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;

public class VirtualMouseGamepad : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 1200f;
    public PlayerInput playerInput;
    public string uiActionMapName = "UI";
    public GameObject cursorUI;

    // Mouse virtual accesible desde otros scripts
    public static Mouse virtualMouse;

    private Gamepad gamepad;
    private bool virtualMouseActive;

    void Awake()
    {
        if (playerInput == null)
            playerInput = FindObjectOfType<PlayerInput>();

        if (virtualMouse == null)
        {
            virtualMouse = InputSystem.AddDevice<Mouse>();
            InputState.Change(
                virtualMouse.position,
                new Vector2(Screen.width / 2f, Screen.height / 2f)
            );
        }

        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;

        if (virtualMouse != null)
        {
            InputSystem.RemoveDevice(virtualMouse);
            virtualMouse = null;
        }
    }

    void Update()
    {
        if (playerInput == null || virtualMouse == null || InputIconProvider.Instance == null)
            return;

        bool inUI = playerInput.currentActionMap != null &&
                    playerInput.currentActionMap.name == uiActionMapName;

        bool usingGamepad = InputIconProvider.Instance.UsandoGamepad();

        if (inUI && usingGamepad && !virtualMouseActive)
            ActivateVirtualMouse();
        else if ((!inUI || !usingGamepad) && virtualMouseActive)
            DeactivateVirtualMouse();

        if (virtualMouseActive && gamepad != null)
            MoveVirtualMouse();
    }

    private void ActivateVirtualMouse()
    {
        if (gamepad == null)
            gamepad = Gamepad.current;

        if (gamepad == null)
            return;

        virtualMouseActive = true;

        InputState.Change(
            virtualMouse.position,
            new Vector2(Screen.width / 2f, Screen.height / 2f)
        );

        if (cursorUI != null)
            cursorUI.SetActive(true);

        Debug.Log("✅ Virtual Mouse ACTIVADO");
    }

    private void DeactivateVirtualMouse()
    {
        virtualMouseActive = false;

        if (cursorUI != null)
            cursorUI.SetActive(false);

        Debug.Log("🖱️ Virtual Mouse DESACTIVADO");
    }

    private void MoveVirtualMouse()
    {
        if (gamepad == null) return;

        Vector2 stick = gamepad.rightStick.ReadValue();
        if (stick.sqrMagnitude < 0.001f) stick = Vector2.zero;

        Vector2 pos = virtualMouse.position.ReadValue();
        pos += stick * speed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, 0, Screen.width);
        pos.y = Mathf.Clamp(pos.y, 0, Screen.height);

        bool click = gamepad.buttonSouth.isPressed;

        var mouseState = new MouseState
        {
            position = pos,
            delta = stick,
            buttons = (ushort)(click ? 1 : 0)
        };

        InputState.Change(virtualMouse, mouseState);

        if (cursorUI != null && cursorUI.activeSelf)
            cursorUI.transform.position = pos;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!(device is Gamepad gp)) return;

        switch (change)
        {
            case InputDeviceChange.Added:
                gamepad = gp;
                Debug.Log("🎮 Gamepad conectado");
                break;

            case InputDeviceChange.Removed:
                if (gp == gamepad)
                {
                    gamepad = null;
                    virtualMouseActive = false;
                    if (cursorUI != null)
                        cursorUI.SetActive(false);
                    Debug.Log("🎮 Gamepad desconectado");
                }
                break;
        }
    }
}
