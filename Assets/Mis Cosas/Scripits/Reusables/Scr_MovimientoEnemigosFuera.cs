using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Scr_MovimientoEnemigosFuera : MonoBehaviour
{
    public GameObject Prefab;
    public bool SeEstaMoviendo = false;
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public GameObject Jugador;
    public float velocidad = 3.5f;
    public float DistanciaDeFrenado = 3.5f;

    private NavMeshAgent agent;
    private float timer;
    private bool Cambiando = false;
    private GameObject Reloj;
    private GameObject CirculoCarga;

    void Start()
    {
        CirculoCarga = GameObject.Find("Canvas").transform.GetChild(6).gameObject;
        Reloj = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;
        agent.speed = velocidad;
        agent.stoppingDistance = DistanciaDeFrenado;

        if (Jugador != null)
        {
            agent.SetDestination(Jugador.transform.position);
            if (Vector3.Distance(transform.position, Jugador.transform.position) <= 3 && !Cambiando)
            {
                Cambiando = true;
                CerrarBarras();
                StartCoroutine(Cambiar());

                Reloj.SetActive(false);

                var datosSingleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
                datosSingleton.Enemigo = Prefab;
                datosSingleton.CantidadDeEnemigos = 1;
            }
        }
        else if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        SeEstaMoviendo = agent.pathPending || agent.remainingDistance > agent.stoppingDistance;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    public void CambiarEscena(int escena)
    {
        SceneManager.LoadScene(escena);
    }

    void CerrarBarras()
    {
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            var animator1 = mainCamera.transform.GetChild(0).GetComponent<Animator>();
            var animator2 = mainCamera.transform.GetChild(1).GetComponent<Animator>();

            if (animator1 != null && animator2 != null)
            {
                animator1.Play("Cerrar");
                animator2.Play("Cerrar");
            }
        }
    }

    IEnumerator Cambiar()
    {
        yield return new WaitForSeconds(1);
        CirculoCarga.SetActive(true);
        yield return new WaitForSeconds(3);
        CambiarEscena(3);
    }
}
