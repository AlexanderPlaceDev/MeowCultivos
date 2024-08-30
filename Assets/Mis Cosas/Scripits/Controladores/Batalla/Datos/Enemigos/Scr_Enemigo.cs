using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Scr_Enemigo : MonoBehaviour
{
    public float Vida;
    public float Velocidad;
    public float Rango;
    public float CoolDown;
    public float DañoMelee;
    public float DañoDistancia;
    public GameObject PrefabBala; // Prefab del proyectil para ataques a distancia
    public Transform SpawnBala; // Punto de inicio del proyectil
    public Scr_CreadorObjetos[] Drops;
    public int XPMinima;
    public int XPMaxima;
    public float[] Probabilidades;
    public Transform Objetivo;
    public Color ColorHerido;
    public float DuracionCambioColor = 0.5f; // Duración del cambio de color en segundos

    private float TiempoCoolDown;
    private Material[] materialesOriginales;
    private bool cambiandoColor = false;

    public enum TipoEnemigo { Terrestre, Volador }
    public TipoEnemigo tipoenemigo;

    public enum TipoComportamiento { Agresivo, Miedoso, Pacifico }
    public TipoComportamiento tipocomportamiento;

    private void Start()
    {
        TiempoCoolDown = CoolDown;

        // Asumiendo que el material está en el segundo hijo
        Renderer renderer = transform.GetChild(1).GetComponent<Renderer>();
        if (renderer != null)
        {
            materialesOriginales = renderer.materials;
        }
    }

    public virtual void Mover()
    {
        if (GetComponent<NavMeshAgent>() != null && GetComponent<NavMeshAgent>().isActiveAndEnabled)
        {
            if (Objetivo != null)
            {
                if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Mover"))
                {
                    GetComponent<Animator>().Play("Mover");
                }
                GetComponent<NavMeshAgent>().isStopped = false;
                GetComponent<NavMeshAgent>().destination = Objetivo.position;
            }
            else
            {
                GetComponent<NavMeshAgent>().isStopped = true;
            }
        }
    }

    public virtual void AtaqueMelee()
    {
        if (TiempoCoolDown >= CoolDown)
        {
            TiempoCoolDown = 0;
            GetComponent<Animator>().Play("Ataque1");
            if (GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>().VidaActual > 0)
            {
                GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>().VidaActual -= (int)DañoMelee;
            }
        }
        else
        {
            TiempoCoolDown += Time.deltaTime;
        }
    }

    public virtual void AtaqueDistancia()
    {
        Debug.Log("Ranged attack performed");
        if (PrefabBala != null && SpawnBala != null)
        {
            Instantiate(PrefabBala, SpawnBala.position, SpawnBala.rotation);
        }
    }

    public void RecibirDaño(float DañoRecibido)
    {
        Vida -= DañoRecibido;
        if (Vida <= 0)
        {
            Morir();
        }
        else
        {
            if (!cambiandoColor)
            {
                StartCoroutine(ChangeMaterialColor());
            }
        }
    }

    public virtual void Morir()
    {
        Destroy(gameObject);
    }

    public void EstablecerComportamiento(TipoComportamiento NuevoComportamiento)
    {
        tipocomportamiento = NuevoComportamiento;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bala")
        {
            // Lógica para bala
        }

        if (other.gameObject.tag == "Golpe")
        {
            other.gameObject.SetActive(false);
            RecibirDaño(1);
        }
    }

    private IEnumerator ChangeMaterialColor()
    {
        cambiandoColor = true;

        Renderer renderer = transform.GetChild(1).GetComponent<Renderer>();
        if (renderer != null)
        {
            // Obtener los materiales actuales
            Material[] materiales = renderer.materials;

            // Guardar una copia de los materiales originales
            Material[] materialesOriginales = new Material[materiales.Length];
            for (int i = 0; i < materiales.Length; i++)
            {
                materialesOriginales[i] = new Material(materiales[i]);
            }

            // Crear copias modificadas de los materiales y cambiar el _BaseColor
            Material[] materialesModificados = new Material[materiales.Length];
            for (int i = 0; i < materiales.Length; i++)
            {
                materialesModificados[i] = new Material(materiales[i]);
                materialesModificados[i].SetColor("_BaseColor", ColorHerido); // Cambiar el color a rojo
            }

            // Aplicar materiales modificados
            renderer.materials = materialesModificados;

            // Esperar el tiempo deseado
            float tiempo = 0;
            while (tiempo < DuracionCambioColor)
            {
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Restaurar materiales originales
            renderer.materials = materialesOriginales;

            cambiandoColor = false;
        }
        else
        {
            Debug.LogError("No se encontró el componente Renderer.");
        }
    }
}
