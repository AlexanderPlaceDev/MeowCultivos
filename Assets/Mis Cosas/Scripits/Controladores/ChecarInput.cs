using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ChecarInput : MonoBehaviour
{
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

    public void quepex()
    {

        //Debug.LogError("aaa");
    }

    public void CammbiarAction_UI()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
        playerInput.SwitchCurrentActionMap("UI");
        
    }
    public void CammbiarAction_Player()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
        playerInput.SwitchCurrentActionMap("Player");
    }

}
