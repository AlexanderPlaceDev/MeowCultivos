using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using TMPro;

public class Scr_Enemigo : MonoBehaviour
{
    public string NombreEnemigo;
    public float Vida;
    public float Velocidad;
    public float DañoMelee;
    public float DañoDistancia;
    public float DistanciaDeAtaque;
    public float DuracionDeAtaque = 1;
    public bool SaleDelArea = true;
    public string NombreAnimacionAparecer;
    public bool Aparecio;
    public bool SpawnDentro;
    public bool SpawnMedio;
    public bool SpawnLejos;
    public bool SpawnDistancia;
    public float CantidadDeOleadas;
    public int CantidadEnemigosPorOleada;
    public float DificultadInicial;
    public float PuntoDeHuida;
    public bool EstaMuerto = false;
    public bool UsaParticulasAlMorir;
    public ParticleSystem ParticulasMuerte;
    public GameObject MeshMuerte;
    public GameObject PrefabBala; // Prefab del proyectil para ataques a distancia
    public Transform SpawnBala; // Punto de inicio del proyectil
    public Scr_CreadorObjetos[] Drops;
    public int XPMinima;
    public int XPMaxima;
    public float[] Probabilidades;
    public Transform Objetivo;
    public Color ColorHerido;
    public float DuracionCambioColor = 0.5f; // Duración del cambio de color en segundos
    private Material[] materialesOriginales;
    private bool cambiandoColor = false;
    private Scr_DatosSingletonBatalla Singleton;

    public enum TipoEnemigo { Terrestre, Volador }
    public TipoEnemigo tipoenemigo;

    public enum TipoComportamiento { Agresivo, Miedoso, Pacifico }
    public TipoComportamiento tipocomportamiento;

    public GameObject Controlador;

    [SerializeField] GameObject CanvasDaño;
    [SerializeField] private Transform PosInicialDaño; // Posición inicial del CanvasDaño
    [SerializeField] private Transform PosFinalDaño;  // Posición final del CanvasDaño
    protected virtual void Start()
    {
        // Asumiendo que el material está en el segundo hijo
        Renderer renderer = transform.GetChild(1).GetComponent<Renderer>();
        if (renderer != null)
        {
            materialesOriginales = renderer.materials;
        }


    }

    private void OnEnable()
    {
        Controlador = GameObject.Find("Controlador");
        Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
    }
    public void RecibirDaño(float DañoRecibido)
    {
        // Reducir la vida del enemigo
        Vida -= DañoRecibido;

        // Verificar si el enemigo debe morir
        if (Vida <= 0)
        {
            Morir();
        }
        else
        {
            // Cambiar temporalmente el color si está herido
            if (!cambiandoColor)
            {
                StartCoroutine(ChangeMaterialColor());
            }
        }
    }
    public virtual void Morir()
    {
        if (UsaParticulasAlMorir)
        {
            EstaMuerto = true; // <- Añadido
            DañoDistancia = 0;
            DañoMelee = 0;
            Velocidad = 0;

            if (GetComponent<NavMeshAgent>() != null)
            {
                GetComponent<NavMeshAgent>().enabled = false; // <- MUY IMPORTANTE
            }

            MeshMuerte.SetActive(false);
            ParticulasMuerte.Play();
            StartCoroutine(EsperarMuerte());
        }
        else
        {
            AgregarEnemigoAlSingleton();
            Destroy(gameObject);
        }
    }

    void AgregarEnemigoAlSingleton()
    {
        bool Encontro = false;
        int pos = 0;
        foreach (string Enemigo in Singleton.EnemigosCazados)
        {
            if (Enemigo == NombreEnemigo)
            {
                Encontro = true;
                break;
            }
            pos++;
        }

        if (Encontro)
        {
            Singleton.CantidadCazados[pos]++;

        }
        else
        {
            Singleton.EnemigosCazados.Add(NombreEnemigo);
            Singleton.CantidadCazados.Add(1);
        }
    }

    IEnumerator EsperarMuerte()
    {
        yield return new WaitForSeconds(ParticulasMuerte.main.duration);
        AgregarEnemigoAlSingleton();
        Destroy(gameObject);
    }
    public void EstablecerComportamiento(TipoComportamiento NuevoComportamiento)
    {
        tipocomportamiento = NuevoComportamiento;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Golpe")
        {
            GameObject.Find("Controlador").GetComponent<Scr_ControladorArmas>().hizoHit = true;
            // Verifica que las posiciones inicial y final estén asignadas
            if (PosInicialDaño == null || PosFinalDaño == null)
            {
                Debug.LogWarning("PosInicialDaño o PosFinalDaño no están asignadas.");
                return;
            }

            // Instanciar el CanvasDaño en la posición inicial
            int Daño = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño;
            if (CanvasDaño != null)
            {
                GameObject canvasInstanciado = Instantiate(CanvasDaño, PosInicialDaño.position, Quaternion.identity);

                // Hacer que el Canvas sea hijo del enemigo para que se mueva con él
                canvasInstanciado.transform.SetParent(transform);
                canvasInstanciado.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Daño.ToString();
                // Iniciar el movimiento del CanvasDaño
                StartCoroutine(MoverCanvas(canvasInstanciado, PosInicialDaño.position, PosFinalDaño.position, 1f));
            }

            // Desactivar el golpe para evitar múltiples activaciones
            if (other.gameObject.name != "Impulso")
            {
                other.gameObject.SetActive(false);
            }
            else
            {
                GetComponent<Rigidbody>().AddForce(-transform.forward.normalized, ForceMode.Impulse);
            }



            // Lógica de daño
            Controlador.GetComponent<Scr_ControladorBatalla>().PuntosActualesHabilidad +=
                GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].PuntosXGolpe;

            RecibirDaño(Daño);
        }
        else if (other.gameObject.tag == "Bala")
        {
            // Verifica que las posiciones inicial y final estén asignadas
            if (PosInicialDaño == null || PosFinalDaño == null)
            {
                Debug.LogWarning("PosInicialDaño o PosFinalDaño no están asignadas.");
                return;
            }

            // Instanciar el CanvasDaño en la posición inicial
            int Daño = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño;
            if (CanvasDaño != null)
            {
                GameObject canvasInstanciado = Instantiate(CanvasDaño, PosInicialDaño.position, Quaternion.identity);

                // Hacer que el Canvas sea hijo del enemigo para que se mueva con él
                canvasInstanciado.transform.SetParent(transform);
                canvasInstanciado.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Daño.ToString();
                // Iniciar el movimiento del CanvasDaño
                StartCoroutine(MoverCanvas(canvasInstanciado, PosInicialDaño.position, PosFinalDaño.position, 1f));
            }
            /*
            // Desactivar el golpe para evitar múltiples activaciones
            if (other.gameObject.name != "Impulso")
            {
                other.gameObject.SetActive(false);
            }
            else
            {
                GetComponent<Rigidbody>().AddForce(-transform.forward.normalized, ForceMode.Impulse);
            }*/



            // Lógica de daño
            Controlador.GetComponent<Scr_ControladorBatalla>().PuntosActualesHabilidad +=
                GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].PuntosXGolpe;

            RecibirDaño(Daño);
        }

    }
    private IEnumerator MoverCanvas(GameObject canvas, Vector3 inicio, Vector3 fin, float duracion)
    {
        float tiempo = 0f;

        // Interpolar la posición del CanvasDaño
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;

            // Mover el CanvasDaño entre la posición inicial y final
            Vector3 nuevaPosicion = canvas.transform.position;
            nuevaPosicion.y = Mathf.Lerp(inicio.y, fin.y, t);
            canvas.transform.position = nuevaPosicion;
            yield return null;
        }

        // Asegurar que termine en la posición final
        canvas.transform.position = fin;

        // Destruir el CanvasDaño
        Destroy(canvas);
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
                materialesModificados[i].SetFloat("_RecibeDa_o", 1); // Cambiar el color a rojo
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
            foreach (Material Mat in renderer.materials)
            {
                Mat.SetFloat("_RecibeDa_o", 0); // Cambiar el color a rojo
            }


            cambiandoColor = false;
        }
    }


}
