using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lugar_point : MonoBehaviour
{
    [SerializeField] private string NombreLugar;
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
        if (!other.CompareTag("Gata")) return;
        Scr_ControladorMisiones ControladorMisiones= other.gameObject.GetComponentInChildren<Scr_ControladorMisiones>();
        Debug.Log("Se detecto ");
        ControladorMisiones.actualizarTargetsExploratod(NombreLugar);
    }
    /*
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Gata")) return;

    }*/
}
