using System.Collections;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.AI;

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

    public bool estaStuneado=false;
    public bool estaCongelado = false;
    
    [SerializeField] GameObject CanvasDaño;
    [SerializeField] private Transform PosInicialDaño; // Posición inicial del CanvasDaño
    [SerializeField] private Transform PosFinalDaño;  // Posición final del CanvasDaño

    public enum TipoEfecfto { Nada,Stunear, Quemar, Veneno, Congelar, Empujar, Electrificar, Explotar }
    public TipoEfecfto Efecto;
    private Color dañado = new Color(1f, 0f, 0f);      // Rojo
    private Color quemado = new Color(1f, 0.365f, 0.133f);  // Naranja
    private Color congelado = new Color(0.059f, 0.816f, 1f);  // Azul claro
    private Color electrificado = new Color(1f, 0.922f, 0.118f); // Amarillo
    private Color envenenado = new Color(0.835f, 0.286f, 1f);  // Morado
    public float resistenciaStunear = 0.1f;  // 10% de probabilidad de resistir el Stun
    public float resistenciaQuemar = 0.2f;   // 20% de probabilidad de resistir el Quemar
    public float resistenciaVeneno = 0.3f;   // 30% de probabilidad de resistir el Veneno
    public float resistenciaCongelar = 0.15f; // 15% de probabilidad de resistir el Congelar
    public float resistenciaEmpujar = 0.05f; // 5% de probabilidad de resistir el Empujar
    public float resistenciaElectrificar = 0.25f; // 25% de probabilidad de resistir el Electrificar
    public float resistenciaExplotar = 0.4f; // 40% de probabilidad de resistir la Explotación
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
    public void RecibirDaño(float DañoRecibido, Color efectoDaño)
    {
        // Reducir la vida del enemigo
        Vida -= DañoRecibido;
        mostrarDaño(DañoRecibido);
        // Verificar si el enemigo debe morir
        if (Vida <= 0)
        {
            Morir();
        }
        else
        {
            // Cambiar temporalmente el color si está herido
            if (!cambiandoColor && efectoDaño!= null)
            {
                StartCoroutine(ChangeMaterial(efectoDaño, DuracionCambioColor));
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
        Scr_ControladorArmas arma= GameObject.Find("Controlador").GetComponent<Scr_ControladorArmas>();
        
        if (other.gameObject.tag == "Golpe")
        {
            arma.hizoHit = true;
            arma.golpe();
            /*
            // Verifica que las posiciones inicial y final estén asignadas
            if (PosInicialDaño == null || PosFinalDaño == null)
            {
                Debug.LogWarning("PosInicialDaño o PosFinalDaño no están asignadas.");
                return;
            }

            if (CanvasDaño != null)
            {
                GameObject canvasInstanciado = Instantiate(CanvasDaño, PosInicialDaño.position, Quaternion.identity);

                // Hacer que el Canvas sea hijo del enemigo para que se mueva con él
                canvasInstanciado.transform.SetParent(transform);
                canvasInstanciado.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Daño.ToString();
                // Iniciar el movimiento del CanvasDaño
                StartCoroutine(MoverCanvas(canvasInstanciado, PosInicialDaño.position, PosFinalDaño.position, 1f));
            }*/
            // Instanciar el CanvasDaño en la posición inicial
            int Daño = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño;
            
            // Desactivar el golpe para evitar múltiples activaciones
            if (other.gameObject.name != "Impulso")
            {
                other.gameObject.SetActive(false);
            }
            else
            {
                GetComponent<Rigidbody>().AddForce(-transform.forward.normalized, ForceMode.Impulse);
            }


            realizardaño(Daño, arma.efecto);
            // Lógica de daño
            /*Controlador.GetComponent<Scr_ControladorBatalla>().PuntosActualesHabilidad +=
                GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].PuntosXGolpe;
            RecibirDaño(Daño, dañado);
            checarEfecto(arma.efecto);*/
        }
        else if (other.gameObject.tag == "Bala")
        {
            
            // Instanciar el CanvasDaño en la posición inicial
            int Daño = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño;
            /*
            // Verifica que las posiciones inicial y final estén asignadas
            if (PosInicialDaño == null || PosFinalDaño == null)
            {
                Debug.LogWarning("PosInicialDaño o PosFinalDaño no están asignadas.");
                return;
            }

            if (CanvasDaño != null)
            {
                GameObject canvasInstanciado = Instantiate(CanvasDaño, PosInicialDaño.position, Quaternion.identity);

                // Hacer que el Canvas sea hijo del enemigo para que se mueva con él
                canvasInstanciado.transform.SetParent(transform);
                canvasInstanciado.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Daño.ToString();
                // Iniciar el movimiento del CanvasDaño
                StartCoroutine(MoverCanvas(canvasInstanciado, PosInicialDaño.position, PosFinalDaño.position, 1f));
            }*/
            // Lógica de daño
            realizardaño(Daño, arma.efecto);
            /*Controlador.GetComponent<Scr_ControladorBatalla>().PuntosActualesHabilidad +=
                GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].PuntosXGolpe;

            RecibirDaño(Daño, dañado);
            checarEfecto(arma.efecto);*/
        }

    }
    public void realizardaño(float daño, string efecto) 
    {
        // Lógica de daño
        Controlador.GetComponent<Scr_ControladorBatalla>().PuntosActualesHabilidad +=
            GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].PuntosXGolpe;
        RecibirDaño(daño, dañado);
        checarEfecto(efecto,1);
        Scr_ControladorArmas cont = GameObject.Find("Controlador").GetComponent<Scr_ControladorArmas>();
        if (cont.sangria)
        {
            Controlador.GetComponent<Scr_ControladorBatalla>().Curar(daño*.2f);
        }
        else if (cont.sangriaEspera)
        {
            Controlador.GetComponent<Scr_ControladorBatalla>().acumularCura+= (daño * .2f);
        }
    }

    private void mostrarDaño(float daño)
    {
        // Verifica que las posiciones inicial y final estén asignadas
        if (PosInicialDaño == null || PosFinalDaño == null)
        {
            Debug.LogWarning("PosInicialDaño o PosFinalDaño no están asignadas.");
            return;
        }

        if (CanvasDaño != null)
        {
            GameObject canvasInstanciado = Instantiate(CanvasDaño, PosInicialDaño.position, Quaternion.identity);

            // Hacer que el Canvas sea hijo del enemigo para que se mueva con él
            canvasInstanciado.transform.SetParent(transform);
            canvasInstanciado.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = daño.ToString();
            // Iniciar el movimiento del CanvasDaño
            StartCoroutine(MoverCanvas(canvasInstanciado, PosInicialDaño.position, PosFinalDaño.position, 1f));
        }
    }

    public void checarEfecto(string efecto, float porefecto)
    {
        // Variable que determina la probabilidad de que el efecto sea resistido
        float probabilidadResistencia = Random.Range(0f, 1f);
        switch (efecto)
        {
            case "Stunear":
                if (probabilidadResistencia <= resistenciaStunear)
                {
                    Debug.Log("Efecto Stun resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoStuneado(3f, porefecto));
                break;

            case "Quemar":
                if (probabilidadResistencia <= resistenciaQuemar)
                {
                    Debug.Log("Efecto Quemar resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoQuemando(5f, 2f, porefecto)); // duración 5s, 2 de daño por segundo
                break;

            case "Veneno":
                if (probabilidadResistencia <= resistenciaVeneno)
                {
                    Debug.Log("Efecto Veneno resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoVeneno(5f, 1f, porefecto)); // duración 5s, 1 de daño por segundo
                break;

            case "Congelar":
                if (probabilidadResistencia <= resistenciaCongelar)
                {
                    Debug.Log("Efecto Congelar resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoCongelado(4f, porefecto)); // duración 4s
                break;

            case "Empujar":
                if (probabilidadResistencia <= resistenciaEmpujar)
                {
                    Debug.Log("Efecto Empujar resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoEmpujado(250, porefecto)); // dirección y fuerza
                break;

            case "Electrificar":
                if (probabilidadResistencia <= resistenciaElectrificar)
                {
                    Debug.Log("Efecto Electrificar resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoElectrificado(3f, 3f, porefecto)); // duración 3s, daño 3 por segundo
                efecto = "Rebotar"; // esto parece un cambio de lógica que puede necesitar explicación
                break;

            case "Explotar":
                if (probabilidadResistencia <= resistenciaExplotar)
                {
                    Debug.Log("Efecto Explotar resistido.");
                    return; // Resiste el efecto
                }
                StartCoroutine(EstadoExplotado(20f, 170f, transform.position - Vector3.forward, porefecto)); // daño, fuerza, origen
                break;
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


    private IEnumerator ChangeMaterial(Color mat, float time)
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
    IEnumerator EstadoStuneado(float duracion, float por)
    {
        estaStuneado = true;
        Debug.Log("Enemigo stuneado");
        yield return new WaitForSeconds(duracion * por);
        estaStuneado = false;
        Debug.Log("Enemigo recuperado del stun");
    }

    IEnumerator EstadoQuemando(float duracion, float dañoPorSegundo, float por)
    {
        // Muestra el efecto
        
        float tiempoPasado = 0f;
        while (tiempoPasado < (duracion * por))
        {
            GameObject explosion = Instantiate(Controlador.GetComponent<Scr_ControladorBatalla>().particulaQuemado, transform.position, transform.rotation);
            explosion.transform.SetParent(transform);
            Destroy(explosion,0.6f);
            RecibirDaño(dañoPorSegundo,quemado);
            yield return new WaitForSeconds(1f);
            tiempoPasado += 1f;
        }
        Debug.Log("Efecto de quemadura terminado");
    }

    IEnumerator EstadoVeneno(float duracion, float dañoPorSegundo, float por)
    {
        // Muestra el efecto
        
        float tiempoPasado = 0f;
        while (tiempoPasado < (duracion * por))
        {
            GameObject explosion = Instantiate(Controlador.GetComponent<Scr_ControladorBatalla>().particulaEnvenado, transform.position, transform.rotation);
            explosion.transform.SetParent(transform);
            Destroy(explosion, 0.6f);
            RecibirDaño(dañoPorSegundo, envenenado);
            yield return new WaitForSeconds(1f);
            tiempoPasado += 1f;
        }
        Debug.Log("Efecto de veneno terminado");
    }

    IEnumerator EstadoCongelado(float duracion, float por)
    {
        // Muestra el efecto
        GameObject explosion = Instantiate(Controlador.GetComponent<Scr_ControladorBatalla>().particulaCongelado, transform.position, transform.rotation);
        explosion.transform.SetParent(transform);
        estaCongelado = true;
        Debug.Log("Enemigo congelado");
        StartCoroutine(ChangeMaterial(congelado, .3f));
        yield return new WaitForSeconds(duracion * por);
        estaCongelado = true;
        Debug.Log("Enemigo descongelado");
        Destroy(explosion);
    }

    IEnumerator EstadoEmpujado(float fuerza, float por)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 direccion = -transform.forward; // dirección hacia atrás del personaje
        rb.AddForce(direccion * (fuerza*por), ForceMode.Impulse);
        Debug.Log("Enemigo empujado");
        yield return null;
    }

    IEnumerator EstadoExplotado(float daño, float fuerzaEmpuje, Vector3 origenExplosion, float por)
    {
        RecibirDaño((daño*por),dañado);
        //Vector3 direccion = transform.position - origenExplosion;
        Vector3 direccion = -transform.forward; // dirección hacia atrás del personaje
        GetComponent<Rigidbody>().AddForce(direccion.normalized * (fuerzaEmpuje *por), ForceMode.Impulse);
        Debug.Log("Enemigo explotado");
        yield return null;
    }

    IEnumerator EstadoElectrificado(float duracion, float dañoPorSegundo, float por)
    {
        float tiempoPasado = 0f;
        Velocidad = Velocidad / 3;
        while (tiempoPasado < (duracion*por))
        {
            // Muestra el efecto
            GameObject explosion = Instantiate(Controlador.GetComponent<Scr_ControladorBatalla>().particulaElectrica, transform.position, transform.rotation);
            explosion.transform.SetParent(transform);
            RecibirDaño(dañoPorSegundo,electrificado);
            Debug.Log("Descarga eléctrica!");
            yield return new WaitForSeconds(1f);
            tiempoPasado += 1f;
            Destroy(explosion);
        }
        Velocidad = Velocidad * 3;
        Debug.Log("Efecto eléctrico terminado");
    }

    public void compartirefecto(string efecto, float porcentaje)
    {
        Vector3 center = transform.position;

        Collider[] colliders = Physics.OverlapSphere(center, 3f);

        foreach (Collider col in colliders)
        {
            Scr_Enemigo ene = col.GetComponent<Scr_Enemigo>();
            if (ene != null)
            {
                ene.checarEfecto(efecto, porcentaje);
            }
        }
    }
}
