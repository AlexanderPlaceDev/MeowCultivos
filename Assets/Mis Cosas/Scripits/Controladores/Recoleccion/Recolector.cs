using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recolector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fruta")
        {
            GameObject.Find("Controlador").GetComponent<Scr_ControladorRecolleccion>().CantFrutaRecolectadas++;
            Destroy(other.gameObject);
        }
    }
}
