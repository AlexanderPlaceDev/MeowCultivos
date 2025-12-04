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
    [SerializeField] public float Distancia;
    [SerializeField] public bool Bala;
    [SerializeField] public GameObject PrefabBala;
    [SerializeField] public GameObject SpawnBala;
    [SerializeField] public float VelocidadBala;
    Animator Anim;

    bool Disparando = false;
    bool Esperando = false;

    float Contador = 0;
    // Start is called before the first frame update

    Scr_Enemigo enemigoCercano;
    void Start()
    {
        Anim = GetComponent<Animator>();
        if (Anim == null)
        {
            Anim = transform.GetChild(0).GetComponent<Animator>();
        }

        if (TiempoDevida > 0)
        {
            Invoke("Dead", TiempoDevida);  // Destruye la torreta después de TiempoDevida
        }
    }

    // Espera un segundo antes de permitir otro disparo
    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(1);
        Disparando = false;
        Contador = TiempoEntreDisparos;
    }

    // Destruye la torreta después de su vida útil
    void Dead()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        checarenemigo();  // Verifica si hay un enemigo cercano

        if (!Disparando && enemigoCercano != null)
        {
            if (!Esperando && Contador <= 0)
            {
                Disparando = true;

                if (Bala)
                {
                    SpawnearBala();  // Dispara la bala
                }
                else
                {
                    golpe();  // Realiza el golpe cuerpo a cuerpo
                }

                StartCoroutine(Esperar());  // Espera antes de permitir otro disparo
            }
            else
            {
                // Si no está disparando y no está esperando, reproduce animación de idle
                if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("CosaIddle"))
                {
                    if (Bala)
                    {
                        Anim.Play("CosaIddle");
                    }
                    else
                    {
                        Anim.Play("idle_planta");
                    }
                }
            }
        }

        // Temporizador para el próximo disparo
        if (Contador > 0)
        {
            Contador -= Time.deltaTime;
        }

        if (Contador <= 0)
        {
            Esperando = false;
        }
    }

    // Función que revisa si hay enemigos cercanos
    public void checarenemigo()
    {
        // Obtén todos los colliders dentro del radio de explosión
        Collider[] colliders = Physics.OverlapSphere(transform.position, Radio);
        float distanciaMinima = Mathf.Infinity;
        enemigoCercano = null;  // Resetea el enemigo cercano en cada chequeo

        // Busca el enemigo más cercano
        foreach (Collider col in colliders)
        {
            Scr_Enemigo ene = col.GetComponent<Scr_Enemigo>();
            if (ene != null)
            {
                float distancia = Vector3.Distance(transform.position, ene.transform.position);
                // Asegúrate de que el enemigo está dentro del ángulo de visión de la torreta
                Vector3 direccion = ene.transform.position - transform.position;

                if (distancia < distanciaMinima) // 45 grados de campo de visión
                {
                    distanciaMinima = distancia;
                    enemigoCercano = ene;
                }
            }
        }

        // Si hemos encontrado un enemigo, rota la torreta hacia él
        if (enemigoCercano != null)
        {
            Vector3 direccion = enemigoCercano.transform.position - transform.position;
            direccion.y = 0;  // Eliminamos la componente Y para que no gire en el eje vertical

            if (direccion.magnitude > 0.1f)
            {
                Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime);  // Suaviza la rotación
            }
        }
    }

    // Función para disparar una bala hacia el enemigo
    public void SpawnearBala()
    {
        if (enemigoCercano != null)
        {
            // Calcula la dirección hacia el enemigo
            Vector3 direccion = enemigoCercano.transform.position - transform.position;

            // Instanciar la bala en el SpawnBala, con la rotación hacia el enemigo
            GameObject Bala = Instantiate(PrefabBala, SpawnBala.transform.position, Quaternion.LookRotation(direccion));

            // Configura el daño de la bala
            Bala.GetComponent<Balas>().daño = Daño;

            // Aplica fuerza a la bala para que se mueva
            Rigidbody rb = Bala.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * VelocidadBala, ForceMode.VelocityChange);
        }
        Debug.Log("Disparando bala hacia el enemigo.");
    }

    // Función para realizar un golpe cuerpo a cuerpo
    public void golpe()
    {
        Anim.Play("Morder_PlantaAliada");
    }

    public void GolpeAdelante()
    { // rango es currentWeapon.range, attackOrigin es el punto del jugador (ej. frente)
        Vector3 center = SpawnBala != null ? SpawnBala.transform.position : transform.position;
        float radius = Distancia * 10; // Ajusta el radio de daño según lo que necesites
        Collider[] colliders = Physics.OverlapSphere(center, radius);

        foreach (Collider col in colliders)
        {
            Scr_Enemigo ene = col.GetComponent<Scr_Enemigo>();
            if (ene != null)
            {
                ene.RecibirDaño(Daño, Color.red);  // Aplica daño
                ene.realizardaño(Daño * .1f, "Veneno", "Nada");  // Aplica daño extra o debuff si es necesario
            }
        }
    }
}
