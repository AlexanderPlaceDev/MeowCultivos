using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recolector : MonoBehaviour
{
    Scr_ControladorRecolleccion Rec;
    // Start is called before the first frame update
    void Start()
    {
        Rec = GameObject.Find("Controlador").GetComponent<Scr_ControladorRecolleccion>();
        if(Rec == null)
        {
            gameObject.SetActive(false);
        }
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
                Rec.CantFrutaRecolectadas--;
            }
            else
            {
                Rec.CantFrutaRecolectadas++;
            }
            Destroy(other.gameObject);
        }
    }
}
