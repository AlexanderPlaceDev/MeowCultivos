using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aparecer_Fruta : MonoBehaviour
{
    public float Vida=1000;
    public GameObject Fruta;
    public float Espera=10f;
    private Coroutine Spawn_drop;
    private Coroutine Esperando;
    Scr_ControladorRecolleccion Recoleccion;
    private bool cambiandoColor = false; 
    private Color dañado = new Color(1f, 0f, 0f);
    public float DuracionCambioColor = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        Recoleccion= GameObject.Find("Controlador").GetComponent<Scr_ControladorRecolleccion>();
    }

    private void OnEnable()
    {
        Recoleccion = GameObject.Find("Controlador").GetComponent<Scr_ControladorRecolleccion>();
        StartCoroutine(Esperar_aleaotrorio()); 
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Dropear_Fruta()
    {
        while (Recoleccion.Dropear)
        {
            yield return new WaitForSeconds(Espera);
            //Debug.LogError("Fruta");
            Instantiate(Fruta, gameObject.transform.GetChild(Random.Range(0,1)).transform.position, Quaternion.identity);
        }
    }

    IEnumerator Esperar_aleaotrorio()
    {
        yield return new WaitForSeconds(Random.Range(1,5));
        Spawn_drop = StartCoroutine(Dropear_Fruta());

    }
    IEnumerator Esperar_Spawn()
    {
        yield return new WaitForSeconds(6);
        Spawn_drop = StartCoroutine(Dropear_Fruta());

    }
    public void RecibirDaño(float DañoRecibido)
    {
        Vida -= DañoRecibido;
        if (Vida <= 0)
        {
            Vida = 0; // 🔹 Evita valores negativos
            GameObject.Find("Controlador").GetComponent<Scr_ControladorRecolleccion>().CantidadPlantas--;
            Destroy(gameObject);
        }
        // Cambiar temporalmente el color si está herido
        if (!cambiandoColor )
        {
            StartCoroutine(ChangeMaterial(dañado, DuracionCambioColor));
        }
        if (Spawn_drop != null)
        {
            StopCoroutine(Spawn_drop);
        }
        if(Esperando != null)
        {
            StopCoroutine(Esperando);
        }
        Esperando = StartCoroutine(Esperar_Spawn());

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
