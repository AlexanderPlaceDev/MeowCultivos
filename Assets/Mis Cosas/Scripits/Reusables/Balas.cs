using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balas : MonoBehaviour
{
    public float daño = 0;
    private float delete = 4f;
    private float contar=0;
    public int penetracion=0;
    public GameObject Controlador;
    public bool Rebota = false;

    private Rigidbody rb;


    public bool Planta = false;
    public bool Explosivo = false;
    public float explosionTime = 3f; // Tiempo hasta que explote
    public float explosionRadius = 5f; // Radio de la explosión
    public float explosionForce = 700f; // Fuerza de la explosión

    public GameObject ObjetoInstanciado; // Efecto visual de la explosión
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
        //Debug.Log("Daño");
        //if (!other.CompareTag("Gata")) return;
        Debug.Log("Daño");
        if (other.CompareTag("Enemigo") && !Explosivo && !Planta)
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
        // Muestra el efecto de explosión
        GameObject explosion = Instantiate(ObjetoInstanciado, transform.position, transform.rotation);

        // Destruir el efecto de explosión después de un tiempo (basado en la duración de las partículas)
        Destroy(explosion, 2f); // 2f es el tiempo en segundos; ajusta según la duración del efecto

        // Obtén todos los colliders en el radio de explosión
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider col in colliders)
        {
            Scr_Enemigo ene = col.GetComponent<Scr_Enemigo>();
            if (ene != null)
            {
                ene.RecibirDaño(daño,Color.red);
            }
        }

        // Destruye la granada después de la explosión
        Destroy(gameObject);
    }

    void Plantar()
    {
        // Muestra el efecto de explosión
        GameObject explosion = Instantiate(ObjetoInstanciado, transform.position, Quaternion.identity);

        // Destruye la granada después de la explosión
        Destroy(gameObject);
    }
}
