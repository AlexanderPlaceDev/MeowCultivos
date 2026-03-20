using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Activador_boss : MonoBehaviour
{
    public OjoBoss ojazo;
    public int intentos=3;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void detectaron()
    {
        intentos--;
    }
    private void OnTriggerEnter(Collider other)
    {
        ojazo.Despertar();
        Destroy(gameObject);
    }
}
