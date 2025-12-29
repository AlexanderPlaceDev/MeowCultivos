using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Recolector : MonoBehaviour
{
    Scr_ControladorRecolleccion Rec;
    private Scr_DatosSingletonBatalla singleton;
    // Start is called before the first frame update
    void Start()
    {
        singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
        Rec = GameObject.Find("Controlador").GetComponent<Scr_ControladorRecolleccion>();
        if(singleton.ModoSeleccionado == Scr_DatosSingletonBatalla.Modo.Pelea)
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
