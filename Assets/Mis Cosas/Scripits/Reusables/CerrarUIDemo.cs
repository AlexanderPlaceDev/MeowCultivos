using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CerrarUIDemo : MonoBehaviour
{
    // Start is called before the first frame update
    ChecarInput Checar_input;
    void Start()
    {

        Checar_input = GameObject.Find("Singleton").GetComponent<ChecarInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CerrarDemo()
    {
        Checar_input.CammbiarAction_Player();
        Destroy(gameObject);
    }
}
