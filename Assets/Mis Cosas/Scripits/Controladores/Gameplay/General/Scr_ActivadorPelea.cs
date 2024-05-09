using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Scr_ActivadorPelea : MonoBehaviour
{
    float tiempo = 0;
    private void Update()
    {
        if (tiempo < 0.5f)
        {
            tiempo += Time.deltaTime;
        }

        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, tiempo*2);
    }
}