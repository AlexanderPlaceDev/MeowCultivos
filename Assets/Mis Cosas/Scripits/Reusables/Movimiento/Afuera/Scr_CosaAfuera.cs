using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_CosaAfuera : Scr_EnemigoFuera
{
    [SerializeField] float DistanciaNervisoso;
    [SerializeField] float DistanciaDisparo;
    [SerializeField] float TiempoEntreDisparos;
    [SerializeField] ParticleSystem Particulas;
    [SerializeField] GameObject PrefabBala;
    [SerializeField] Transform PosicionBala;
    [SerializeField] float VelocidadBala;
    bool Disparando = false;
    bool Desaparecido = false;
    float Contador = 0;
    GameObject Gata;
    Animator Anim;

    private void Start()
    {
        Anim = GetComponent<Animator>();
        Gata = GameObject.Find("Gata");
    }

    IEnumerator Esperar()
    {
        Debug.Log("Entra 3");
        yield return new WaitForSeconds(1);
        Disparando = false;
        Contador = TiempoEntreDisparos;
    }

    private void Update()
    {
        float distancia = Vector3.Distance(Gata.transform.position, transform.position);
        Debug.Log(distancia);

        if (distancia < DistanciaDisparo)
        {
            Debug.Log("Entra 1");

            if (!Disparando && Contador <= 0)
            {
                Debug.Log("Entra 2");

                // **Rotar hacia la Gata**
                transform.LookAt(Gata.transform);

                Anim.Play("CosaDisparar");
                Disparando = true;
                StartCoroutine(Esperar());
            }
        }
        else
        {
            if (distancia < DistanciaNervisoso)
            {
                Debug.Log("Entra 3");
                if (Contador <= 0)
                {
                    Contador = 10;
                    Anim.Play("CosaTemblar", 0, 0);
                }
            }
        }

        if (Contador > 0)
        {
            Contador -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gata")
        {
            if (!Disparando && !Desaparecido)
            {
                Contador = 4;
                Desaparecido = true;
                Particulas.Play();
                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
                StartCoroutine(Morir());
            }
        }
    }

    IEnumerator Morir()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    public void SpawnearBala()
    {
        // Instancia la bala con la misma rotaci�n que el enemigo
        GameObject Bala = Instantiate(PrefabBala, PosicionBala.position, transform.rotation);

        // Agrega fuerza en la direcci�n correcta
        Bala.GetComponent<Rigidbody>().AddForce(transform.forward * VelocidadBala);
    }
}
