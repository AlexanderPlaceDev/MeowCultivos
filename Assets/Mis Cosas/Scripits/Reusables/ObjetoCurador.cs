using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoCurador : MonoBehaviour
{
    public bool vidaCompleta;
    public float porcentaje;
    private float vidaacurar;
    Scr_ControladorBatalla ControladorBatalla;
    // Start is called before the first frame update
    void Start()
    {
        ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        if (vidaCompleta)
        {
            vidaacurar = ControladorBatalla.VidaMaxima;
        }
        else
        {
            vidaacurar = ControladorBatalla.VidaMaxima * porcentaje;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gata"))
        {
            ControladorBatalla.Curar(vidaacurar);
        }
    }
}
