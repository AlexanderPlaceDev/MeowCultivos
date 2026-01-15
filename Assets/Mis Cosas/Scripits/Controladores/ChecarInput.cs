using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ChecarInput : MonoBehaviour
{
    Scr_GiroGata giro;
    PlayerInput playerInput;
    public GameObject EventSystem;

    // Start is called before the first frame update
    void Start()
    {
        //giro= GameObject.Find("Gata").GetComponent<Scr_GiroGata>();
        playerInput= GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CambiarControl()
    {
        if (playerInput != null) 
        {
            if(playerInput.currentControlScheme== "Control" || playerInput.currentControlScheme == "PlayController")
            {
                if (giro != null)
                {
                    giro.Control = true;
                    giro.checar_Control();
                }
            }
            else
            {
                if (giro != null)
                {
                    giro.Control = false;
                    giro.checar_Control();
                }
            }
        }
    }
    public void quepex()
    {

        //Debug.LogError("aaa");
    }
    public void ControlDesconectado()
    {
        if (giro != null)
        {
            giro.Control = false;
            giro.checar_Control();
        }
    }


    public void Chechar_controlador_Interfaz()
    {
        CammbiarAction_UI();
        if (playerInput.currentControlScheme == "Control" || playerInput.currentControlScheme == "PlayController")
        {
            Debug.LogError("aaaaaes");
            CammbiarAction_UI();
        }
    }
    public void Cerrar_controlador_Interfaz()
    {
        if (playerInput.currentControlScheme == "Control" || playerInput.currentControlScheme == "PlayController")
        {
            CammbiarAction_Player();
        }
    }
    public void CammbiarAction_UI()
    {
        playerInput.SwitchCurrentActionMap("UI");
        //.SetActive(true);
        VirtualCursorFollow virtualCursor = GameObject.Find("Camera").GetComponent <VirtualCursorFollow>();
        if (virtualCursor != null)
        {
            virtualCursor.cursorUI.SetActive(true);
        }
    }
    public void CammbiarAction_Player()
    {
        playerInput.SwitchCurrentActionMap("Player");
        EventSystem.SetActive(false); VirtualCursorFollow virtualCursor = GameObject.Find("Camera").GetComponent<VirtualCursorFollow>();
        if (virtualCursor != null)
        {
            virtualCursor.cursorUI.SetActive(false);
        }
    }

}
