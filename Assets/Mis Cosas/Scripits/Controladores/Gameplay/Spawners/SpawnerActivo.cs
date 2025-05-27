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
    [SerializeField] public int VidaMaxima;
    [SerializeField] public bool UsaPico;
    [SerializeField] public bool UsaHacha;
    [SerializeField] string HabilidadRequerida;
    [SerializeField] string HabilidadRequerida2;
    [SerializeField] float[] TiempoRespawn;
    [SerializeField] GameObject Herramienta;

    private Transform Gata;
    private bool estaLejos;
    private bool objetoAgregado = false;
    private float Tiempo;
    private float TiempoRespawnAleatorio;

    private MeshRenderer meshRenderer;
    private Collider Col;
    private ParticleSystem Particulas;

    private bool particulasReproducidas = false; // Evitar repetición de partículas

    void Start()
    {
        Vida = VidaMaxima;
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        TiempoRespawnAleatorio = Random.Range(TiempoRespawn[0], TiempoRespawn[1]);

        Herramienta = Gata.GetChild(0).GetChild(0).GetChild(0).GetChild(1)
            .GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;

        meshRenderer = GetComponent<MeshRenderer>();
        Col = GetComponent<Collider>();
        Particulas = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (PlayerPrefs.GetString("Habilidad:" + HabilidadRequerida, "No") == "Si" || string.IsNullOrEmpty(HabilidadRequerida))
        {
            if (Vector3.Distance(Gata.position, transform.position) < Distancia && meshRenderer.enabled)
            {
                SpawnearHerramienta();
                ActivarUI();
                estaLejos = false;
            }
            else if (!estaLejos)
            {
                DesactivarUI();
                Herramienta.SetActive(false);
                Herramienta.transform.GetChild(0).gameObject.SetActive(false);
                Herramienta.transform.GetChild(1).gameObject.SetActive(false);
                estaLejos = true;
            }
        }

        if (Vida <= 0)
        {
            Tiempo += Time.deltaTime;
            if (Tiempo >= TiempoRespawnAleatorio)
            {
                Tiempo = 0;
                Vida = VidaMaxima;
                meshRenderer.enabled = true;
                Col.enabled = true;
                particulasReproducidas = false; // Resetea el estado de las partículas
                objetoAgregado = false;
            }
            else
            {
                if (!particulasReproducidas && !Particulas.isPlaying)
                {
                    Particulas.Play();
                    particulasReproducidas = true;
                DesactivarUI();
                Herramienta.SetActive(false);
                }

                meshRenderer.enabled = false;
                Col.enabled = false;


                if (!objetoAgregado)
                {
                    objetoAgregado = true;
                    int cantidad = Random.Range(MinimoMaximo[0], MinimoMaximo[1]);
                    if (PlayerPrefs.GetString("Habilidad:" + HabilidadRequerida2, "No") == "Si" || string.IsNullOrEmpty(HabilidadRequerida2))
                    {
                        AgregarObjetoInventario(cantidad * 2, ObjetoQueDa);
                    }
                    else
                    {
                        AgregarObjetoInventario(cantidad, ObjetoQueDa);
                    }
                }
            }
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
        Gata.GetChild(3).gameObject.SetActive(true);
        Gata.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Tecla;
        Gata.GetChild(3).GetChild(0).GetComponent<Image>().sprite = TeclaIcono;
        Gata.GetChild(3).GetChild(1).GetComponent<Image>().sprite = Icono;
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = true;
    }

    void DesactivarUI()
    {
        Gata.GetChild(3).gameObject.SetActive(false);
        Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeTalar = false;
    }

    void AgregarObjetoInventario(int cantidad, Scr_CreadorObjetos objeto)
    {
        Scr_Inventario inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
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
