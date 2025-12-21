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
            Fruta_drop drop = other.gameObject.GetComponent<Fruta_drop>();
            if(drop.estadoActual == Fruta_drop.EstadoFruta.Podrido)
            {
                GameObject.Find("Controlador").GetComponent<Scr_ControladorRecolleccion>().CantFrutaRecolectadas--;
            }
            else
            {
                GameObject.Find("Controlador").GetComponent<Scr_ControladorRecolleccion>().CantFrutaRecolectadas++;
            }
            Destroy(other.gameObject);
        }
    }
}
