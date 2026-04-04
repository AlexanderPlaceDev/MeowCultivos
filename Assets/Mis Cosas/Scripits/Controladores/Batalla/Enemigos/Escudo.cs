using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escudo : MonoBehaviour
{
    public float EscudoSalud = 50;
    public GameObject Controlador; 
    public Scr_Enemigo enemy;
    Scr_ControladorArmas arma;

    public float DuracionCambioColor = 0.5f; // Duración del cambio de color en segundos


    public bool cambiandoColor = false;
    // Start is called before the first frame update
    void Start()
    {
        arma = GameObject.Find("Controlador").GetComponent<Scr_ControladorArmas>();
        Controlador = GameObject.Find("Controlador"); 
        if (enemy == null)
            enemy = GetComponentInParent<Scr_Enemigo>();

        enemy.FueBloqueado = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
         
        if (other.gameObject.tag == "Golpe")
        {
            enemy.FueBloqueado = true; 
            int Daño = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño;
            quitar_Restiencia(Daño);
        }
        else if (other.gameObject.tag == "Bala")
        {
            enemy.FueBloqueado = true;
            enemy.BloquearGolpe();
            int Daño = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño; 
            quitar_Restiencia(Daño);
        }

    }

    public void quitar_Restiencia(float daño)
    {
        EscudoSalud -= daño;
        if (EscudoSalud < 0)
        {
            enemy.FueBloqueado = false;
            gameObject.SetActive(false);
        }
        else
        {
            // Cambiar temporalmente el color si está herido
            if (!cambiandoColor )
            {
                StartCoroutine(ChangeMaterial(Color.red, DuracionCambioColor));
            }
        }
    }

    private IEnumerator ChangeMaterial(Color mat, float time)
    {
        cambiandoColor = true;

        Renderer renderer = transform.GetComponent<Renderer>();
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
                materialesModificados[i].SetColor("_Base_Color", mat); // Cambiar el color
            }

            // Aplicar materiales modificados
            renderer.materials = materialesModificados;

            // Esperar el tiempo deseado
            float tiempo = 0;
            while (tiempo < time)
            {
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Restaurar materiales originales
            renderer.materials = materialesOriginales;

            // Restaurar color base original
            foreach (Material material in renderer.materials)
            {
                material.SetColor("_Base_Color", Color.white); // Cambiar el color a blanco (o restaurar el color original)
            }

            cambiandoColor = false;
        }
    }
}
