using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class SpawnerActivo : MonoBehaviour
{
    [SerializeField] private Sprite Icono;
    [SerializeField] private string Tecla;
    [SerializeField] private Sprite TeclaIcono;
    [SerializeField] private float Distancia;
    [SerializeField] private Scr_CreadorObjetos ObjetoQueDa;
    [SerializeField] private int[] MinimoMaximo;
    [SerializeField] public int Vida;
    [SerializeField] public bool UsaPico;
    [SerializeField] public bool UsaHacha;

    private GameObject Herramienta;
    private Transform Gata;
    private bool estaLejos;
    private bool objetoAgregado = false; // Variable para controlar si se ha agregado el objeto al inventario

    void Start()
    {
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        Herramienta = Gata.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
    }

    void Update()
    {
        if (Vector3.Distance(Gata.position, transform.position) < Distancia && GetComponent<MeshRenderer>().enabled)
        {
            SpawnearHerramienta();
            ActivarUI();
            estaLejos = false;
        }
        else
        {
            if (!estaLejos)
            {
                DesactivarUI();
                Herramienta.SetActive(false);
                estaLejos = true;
            }
        }

        if (Vida <= 0)
        {
            //Desactivar Mesh
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<MeshCollider>().enabled = false;
            // Reproducir efecto de partículas
            ParticleSystem particleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
            if (!particleSystem.isPlaying)
            {
                particleSystem.Play();
            }
            Herramienta.SetActive(false);
            DesactivarUI();
            // Incrementar la cantidad de objetos en el inventario solo si no se ha agregado antes
            if (!objetoAgregado)
            {
                objetoAgregado = true;
                int cantidad = Random.Range(MinimoMaximo[0], MinimoMaximo[1]);
                AgregarObjetoInventario(cantidad,ObjetoQueDa);
            }
            // Esperar hasta que el sistema de partículas termine para destruir el objeto
            StartCoroutine(DestruirDespuesDeParticulas(particleSystem));
        }
    }

    void SpawnearHerramienta()
    {
        Herramienta.SetActive(true);
        Herramienta.transform.GetChild(0).gameObject.SetActive(UsaHacha);
        Herramienta.transform.GetChild(1).gameObject.SetActive(UsaPico);
    }

    void ActivarUI()
    {
        Gata.GetChild(2).gameObject.SetActive(true);
        Gata.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Tecla;
        Gata.GetChild(2).GetChild(0).GetComponent<Image>().sprite = TeclaIcono;
        Gata.GetChild(2).GetChild(1).GetComponent<Image>().sprite = Icono;
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = true;
    }

    void DesactivarUI()
    {
        Gata.GetChild(2).gameObject.SetActive(false);
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = false;
    }

    void AgregarObjetoInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        Scr_Inventario inventario = Gata.GetChild(6).GetComponent<Scr_Inventario>();
        inventario.AgregarObjeto(cantidad, objeto.Nombre);
        Scr_ObjetosAgregados controlador = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<Scr_ObjetosAgregados>();
        if (controlador.Lista.Contains(objeto))
        {
            int indice = controlador.Lista.IndexOf(objeto);
            controlador.Cantidades[indice] += cantidad;
            if (indice <= 3)
            {
                controlador.Tiempo[indice] = 2;
            }
        }
        else
        {
            controlador.Lista.Add(objeto);
            controlador.Cantidades.Add(cantidad);
        }
    }

    IEnumerator DestruirDespuesDeParticulas(ParticleSystem particleSystem)
    {
        // Esperar hasta que el sistema de partículas termine
        yield return new WaitForSeconds(particleSystem.main.duration);

        // Destruir el objeto actual después de que termine el sistema de partículas
        Destroy(gameObject);
    }

    void ActualizarInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        Scr_Inventario inventario = Gata.GetChild(6).GetComponent<Scr_Inventario>();
        inventario.AgregarObjeto(cantidad, objeto.Nombre);
        Scr_ObjetosAgregados controlador = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<Scr_ObjetosAgregados>();
        if (controlador.Lista.Contains(objeto))
        {
            int indice = controlador.Lista.IndexOf(objeto);
            controlador.Cantidades[indice] += cantidad;
            if (indice <= 3)
            {
                controlador.Tiempo[indice] = 2;
            }
        }
        else
        {
            controlador.Lista.Add(objeto);
            controlador.Cantidades.Add(cantidad);
        }
    }
}
