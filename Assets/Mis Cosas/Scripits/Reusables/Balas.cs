using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balas : MonoBehaviour
{
    public float daño = 0;
    private float delete = 2f;
    private float contar=0;
    public int penetracion=0;
    public GameObject Controlador;
    public bool Rebota = false;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        Controlador = GameObject.Find("Controlador");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        contar += Time.deltaTime;
        if(contar > delete)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Daño");
        //if (!other.CompareTag("Gata")) return;
        Debug.Log("Daño");
        if (other.CompareTag("Enemigo"))
        {
            Debug.Log("Daño");
            penetracion_menos();
            /*var ene = other.GetComponent<Scr_Enemigo>();
            ene.RecibirDaño(daño);
            
            // Lógica de daño
            Controlador.GetComponent<Scr_ControladorBatalla>().PuntosActualesHabilidad +=
                GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].PuntosXGolpe;
            */
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Rebote
        if (Rebota)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 direccionActual = rb.velocity;
            Vector3 direccionRebote = Vector3.Reflect(direccionActual, contact.normal);
            rb.velocity = direccionRebote;
        }
    }
    void penetracion_menos()
    {
        penetracion--;
        if (penetracion < 0)
        {
            Destroy(gameObject);
        }
    }
}
