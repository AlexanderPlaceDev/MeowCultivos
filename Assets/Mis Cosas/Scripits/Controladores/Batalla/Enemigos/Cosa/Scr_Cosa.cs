using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scr_Cosa : Scr_Enemigo
{

    Animator Anim;
    [SerializeField] float VelocidadBala;
    [SerializeField] float OffsetBala;
    [SerializeField] float TiempoEntreDisparos;
    [SerializeField] ParticleSystem Particulas;
    GameObject Gata;
    NavMeshAgent Agente;
    bool Disparando = false;
    bool Esperando = false;
    float Contador = 0;

    protected override void Start()
    {
        base.Start();
        Anim = GetComponent<Animator>();
        Gata = GameObject.Find("Personaje");
        Agente = GetComponent<NavMeshAgent>();
    }

    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(1);
        Disparando = false;
        Contador = TiempoEntreDisparos;
    }


    void Update()
    {
        if (EstaMuerto) return;


        if (Agente.isActiveAndEnabled)
        {
            if (!Disparando)
            {
                if (!Esperando && Contador <= 0)
                {
                    Disparando = true;
                    Anim.Play("CosaDisparar");
                    StartCoroutine(Esperar());
                }
                else
                {
                    if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("CosaIddle"))
                    {
                        Anim.Play("CosaIddle");
                    }
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
        Vector3 Pos = new Vector3(Gata.transform.position.x, Gata.transform.position.y+OffsetBala, Gata.transform.position.z);
        transform.LookAt(Pos);
        // Instancia la bala con la misma rotación que el enemigo
        GameObject Bala = Instantiate(PrefabBala, SpawnBala.position, transform.rotation);
        Bala.GetComponent<Scr_BalaBtalla>().Daño = DañoDistancia;

        // Agrega fuerza en la dirección correcta
        Bala.GetComponent<Rigidbody>().AddForce(transform.forward * VelocidadBala);
    }

    public override void Morir()
    {
        Particulas.Play();
        StartCoroutine(Desaparecer());
    }

    IEnumerator Desaparecer()
    {
        Disparando = true;
        Esperando= true;
        transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
        yield return new WaitForSeconds(1);
        base.Morir();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.tag == "Gata")
        {
            Particulas.Play();
            StartCoroutine(Desaparecer());
        }
    }
}
