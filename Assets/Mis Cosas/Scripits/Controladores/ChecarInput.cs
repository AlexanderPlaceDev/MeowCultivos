using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChecarInput : MonoBehaviour
{
    Scr_GiroGata giro;
    PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {
        giro= GameObject.Find("Gata").GetComponent<Scr_GiroGata>();
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
}
