using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balas : MonoBehaviour
{
    public float da�o = 0;
    private float delete = 4f;
    private float contar=0;
    public int penetracion=0;
    public GameObject Controlador;
    public bool Rebota = false;

    private Rigidbody rb;


    public bool Planta = false;
    public bool Explosivo = false;
    public float explosionTime = 3f; // Tiempo hasta que explote
    public float explosionRadius = 5f; // Radio de la explosi�n
    public float explosionForce = 700f; // Fuerza de la explosi�n

    public GameObject ObjetoInstanciado; // Efecto visual de la explosi�n
    // Start is called before the first frame update
    void Start()
    {
        Controlador = GameObject.Find("Controlador");
        rb = GetComponent<Rigidbody>();
        if (Explosivo)
        {
            Invoke("Explode", explosionTime);
        }
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
        //Debug.Log("Da�o");
        //if (!other.CompareTag("Gata")) return;
        Debug.Log("Da�o");
        if (other.CompareTag("Enemigo") && !Explosivo && !Planta)
        {
            Debug.Log("Da�o");
            penetracion_menos();
            /*var ene = other.GetComponent<Scr_Enemigo>();
            ene.RecibirDa�o(da�o);
            
            // L�gica de da�o
            Controlador.GetComponent<Scr_ControladorBatalla>().PuntosActualesHabilidad +=
                GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].PuntosXGolpe;
            */
        }
        if (other.CompareTag("Suelo") && !Explosivo && Planta)
        {
            Debug.Log("Plnat");
            Plantar();
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
        if (penetracion < 0 )
        {
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        // Muestra el efecto de explosi�n
        GameObject explosion = Instantiate(ObjetoInstanciado, transform.position, transform.rotation);

        // Destruir el efecto de explosi�n despu�s de un tiempo (basado en la duraci�n de las part�culas)
        Destroy(explosion, 2f); // 2f es el tiempo en segundos; ajusta seg�n la duraci�n del efecto

        // Obt�n todos los colliders en el radio de explosi�n
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider col in colliders)
        {
            Scr_Enemigo ene = col.GetComponent<Scr_Enemigo>();
            if (ene != null)
            {
                ene.RecibirDa�o(da�o,Color.red);
            }
        }

        // Destruye la granada despu�s de la explosi�n
        Destroy(gameObject);
    }

    void Plantar()
    {
        // Muestra el efecto de explosi�n
        GameObject explosion = Instantiate(ObjetoInstanciado, transform.position, Quaternion.identity);

        // Destruye la granada despu�s de la explosi�n
        Destroy(gameObject);
    }
}
