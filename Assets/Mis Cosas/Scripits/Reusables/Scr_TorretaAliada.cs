using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scr_TorretaAliada : MonoBehaviour
{
    [SerializeField] public float Daño;
    [SerializeField] public float TiempoEntreDisparos;

    [SerializeField] public float TiempoDevida;
    [SerializeField] public float Radio;
    [SerializeField] public GameObject PrefabBala;
    [SerializeField] public GameObject SpawnBala;
    [SerializeField] public float VelocidadBala;
    Animator Anim;

    bool Disparando = false;
    bool Esperando = false;

    float Contador = 0;
    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        Invoke("Dead", TiempoDevida);
    }

    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(1);
        Disparando = false;
        Contador = TiempoEntreDisparos;
    }

    void Dead()
    {
        Destroy(gameObject);
    }
    void Update()
    {
        if (!Disparando)
        {
            if (!Esperando && Contador <= 0)
            {
                Disparando = true;
                SpawnearBala();
                //Anim.Play("CosaDisparar");
                StartCoroutine(Esperar());
            }
            else
            {
                if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("CosaIddle"))
                {
                    //Anim.Play("CosaIddle");
                }
            }
        }


        if (Contador > 0)
        {
            Contador -= Time.deltaTime;
        }

        if (Contador <= 0)
        {
            Esperando = false;
        }
    }

    public void SpawnearBala()
    {
        // Obtén todos los colliders en el radio de explosión
        Collider[] colliders = Physics.OverlapSphere(transform.position, Radio);
        Scr_Enemigo enemigoCercano = null;
        float distanciaMinima = Mathf.Infinity;

        // Busca el enemigo más cercano
        foreach (Collider col in colliders)
        {
            Scr_Enemigo ene = col.GetComponent<Scr_Enemigo>();
            if (ene != null)
            {
                float distancia = Vector3.Distance(transform.position, ene.transform.position);
                if (distancia < distanciaMinima)
                {
                    distanciaMinima = distancia;
                    enemigoCercano = ene;
                }
            }
        }

        // Si encontramos un enemigo cercano, disparar la bala hacia él
        if (enemigoCercano != null)
        {
            // Orientar el enemigo hacia la posición del enemigo más cercano
            Vector3 direccion = enemigoCercano.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direccion);

            // Instanciar la bala con la rotación adecuada
            GameObject Bala = Instantiate(PrefabBala, SpawnBala.transform.position, transform.rotation);

            // Configurar el daño de la bala
            Bala.GetComponent<Balas>().daño = Daño;
            // Aplicar fuerza
            Rigidbody rb = Bala.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * VelocidadBala);
        }
        Debug.Log("Disparaene");
    }
}
