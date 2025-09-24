using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Scr_Habilidades : MonoBehaviour
{
    [SerializeField] private GameObject Efecto; // Efecto visual
    [SerializeField] private Volume volumen;   // Post-Process Volume
    [SerializeField] private GameObject Armas;
    private Vignette _vignette;                // Referencia a Vignette
    private LensDistortion _lensDistortion;   // Referencia a Lens Distortion
    private ColorAdjustments _ColorAdjustments;   // Referencia a Color Adjustments

    private float velocidadInicial;           // Velocidad inicial de los enemigos
    private float DuracionAtaqueInicial;           // Velocidad inicial de los enemigos
    private float lensDistortionInicial;      // Valor inicial del Lens Distortion
    private float ColorAdjustmentsInicial;      // Valor inicial del Lens Distortion

    [SerializeField] GameObject[] AreasGarras;

    private GameObject Singleton;
    private GameObject Controlador;

    private Coroutine vignetteCoroutine; // Manejo de Vignette
    private Coroutine colorAdjustmentsCoroutine; // Manejo de ColorAdjustments
    private Coroutine LensCoroutine; // Manejo de ColorAdjustments
    private float DañoAnterior = 0;

    private int PerdigonesAnterior =0;
    private GameObject balaAnterior;

    private float DispersionAnterior = 0;

    private float SaltoAnterior = 0;


    private float VagachadoAnterior = 0;
    private float VcaminarAnterior = 0;
    private float VcorrerAnterior = 0;


    private string TipoAnterior = "";
    private void Start()
    {
        Singleton = GameObject.Find("Singleton");
        Controlador = GameObject.Find("Controlador");
    }

    public void Habilidad(string habilidad)
    {

        switch (habilidad)
        {
            case "Garras":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, new Color(232, 109, 19), 0.05f, 1.5f));
                    StartCoroutine(HabilidadGarras());
                }
                break;

            case "Rugido":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                }

                StartCoroutine(ActivarEfectoVisual(5f));
                break;

            case "Ojo":
                if (volumen.profile.TryGet<ColorAdjustments>(out _ColorAdjustments))
                {
                    ColorAdjustmentsInicial = _ColorAdjustments.hueShift.value;
                    StartCoroutine(ModificarColorAdjustments(_ColorAdjustments, -50, 5f));
                }
                if (volumen.profile.TryGet<LensDistortion>(out _lensDistortion))
                {
                    lensDistortionInicial = _lensDistortion.intensity.value;
                    StartCoroutine(ModificarLensDistortion(_lensDistortion, -0.5f, 5f));
                }

                foreach (GameObject enemigo in GameObject.FindGameObjectsWithTag("Enemigo"))
                {
                    velocidadInicial = enemigo.GetComponent<Scr_Enemigo>().Velocidad;
                    DuracionAtaqueInicial = enemigo.GetComponent<Scr_Enemigo>().DuracionDeAtaque;
                    enemigo.GetComponent<Scr_Enemigo>().DuracionDeAtaque = DuracionAtaqueInicial * 3f;
                    enemigo.GetComponent<Scr_Enemigo>().Velocidad = velocidadInicial / 3f;
                    enemigo.GetComponent<Animator>().speed = 1 / 3f;
                    enemigo.GetComponent<NavMeshAgent>().speed = velocidadInicial / 3f;

                    StartCoroutine(RegresarVelocidad(5f, enemigo));
                }
                break;
            case "Aumento de Carga":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                    StartCoroutine(HabilidadAumentoCarga());
                }
                break;

            case "Crecimiento Rapido":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.blue, 0.5f, 5f));
                    StartCoroutine(HabilidadMasArea());
                }
                break;
            case "Disparo explosivo":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                    StartCoroutine(HabilidadGranada());
                }

                break;

            case "Disparo maciso":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                    StartCoroutine(HabilidadMascizo());
                }
                break;

            case "Fuerza Imparable":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                    StartCoroutine(HabilidadImparable());
                }

                break;
            case "Municion Perpetua":
                HabilidadMinima();
                break;

            case "Pequeña planta":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                    StartCoroutine(HabilidadPlantaMini());
                }
                break;

            case "Protección del Coloso":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                    StartCoroutine(HabilidadColoso());
                }

                break;

            case "Pulso Sonoro":
                Habilidadempuje();
                break;

            case "Puño Blindado":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                    StartCoroutine(HabilidadBlindado());
                }
                break;

            case "Puño Sangria":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                    StartCoroutine(HabilidadSangria());
                }
                break;

            case "Puño volador":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                    StartCoroutine(HabilidadVolador());
                }
                break;

            case "Raices Eternas":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.red, 0.5f, 5f));
                    StartCoroutine(HabilidadRaizEterna());
                }
                break;

            case "Reflejos Felinos":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.blue, 0.5f, 5f));
                    StartCoroutine(HabilidadFelina());
                }
                break;

            case "Rugido de la Tierra":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.blue, 0.5f, 5f));
                    StartCoroutine(HabilidadRugidoTierra());
                }
                break;

            case "Semilla explosiva":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.blue, 0.5f, 5f));
                    StartCoroutine(HabilidadSemillaExplosiva());
                }
                break;

            case "Tiro explosivo":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.blue, 0.5f, 5f));
                    StartCoroutine(HabilidadGranada());
                }
                break;

            case "Tiro Imparable":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.blue, 0.5f, 5f));
                    StartCoroutine(HabilidadTiroImparable());
                }
                break;
            case "Tiro Esparcido":
                if (volumen.profile.TryGet<Vignette>(out _vignette))
                {
                    StartCoroutine(ModificarVignette(_vignette, Color.blue, 0.5f, 5f));
                    StartCoroutine(HabilidadTiroEsparcido());
                }
                break;
            default:
                Debug.Log("No se encontró la habilidad.");
                break;
        }
    }

    IEnumerator HabilidadGarras()
    {
        //Guardamos el daño que tenia
        DañoAnterior = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño;
        //Multiplicamos el daño x 4
        Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño=(int)(DañoAnterior*4);
        //Se activan las colisiones
        AreasGarras[0].SetActive(true);
        AreasGarras[1].SetActive(true);
        //Realiza animacion de quitar guantes
        Armas.transform.GetChild(0).GetComponent<Animator>().Play("DesaparecerBrazos1");
        yield return new WaitForSeconds(0.1f);

        // Obtener la dirección hacia adelante de la cámara
        Vector3 cameraForward = Camera.main.transform.forward;

        // Proyectar la dirección de la cámara en el plano XZ (eliminar componente Y)
        Vector3 flatDirection = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

        // Aplicar fuerza únicamente en el plano horizontal
        GameObject Personaje = GameObject.Find("Personaje");
        Rigidbody rb = Personaje.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(flatDirection * 500, ForceMode.Impulse);
        Armas.transform.GetChild(0).gameObject.SetActive(false);
        Armas.transform.GetChild(1).gameObject.SetActive(true);
        Efecto.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.75f / 2f);
        AreasGarras[0].SetActive(true);
        AreasGarras[1].SetActive(true);
        yield return new WaitForSeconds(0.75f / 2f);
        //Reasignamos el daño
        Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño=(int)DañoAnterior;
        Armas.transform.GetChild(0).gameObject.SetActive(true);

        Personaje.GetComponent<Scr_Movimiento>().enabled = true;
        Armas.transform.GetChild(0).GetComponent<Animator>().Play("AparecerBrazos1");
        Armas.transform.GetChild(1).gameObject.SetActive(false);
        Efecto.transform.GetChild(1).gameObject.SetActive(false);
        AreasGarras[0].SetActive(false);
        AreasGarras[1].SetActive(false);
    }

    IEnumerator HabilidadAumentoCarga()
    {
        //Guardamos La cantidad de perdigones que tenia
        PerdigonesAnterior = Controlador.GetComponent<Scr_ControladorArmas>().cantidadPerdigones;
        //Multiplicamos La cantidad de perdigones x 2
        Controlador.GetComponent<Scr_ControladorArmas>().cantidadPerdigones = (PerdigonesAnterior * 2);
        

        yield return new WaitForSeconds(5f);
        Controlador.GetComponent<Scr_ControladorArmas>().cantidadPerdigones = PerdigonesAnterior;
    }
    IEnumerator HabilidadGranada()
    {
        //Guardamos La cantidad de perdigones que tenia
        balaAnterior = Controlador.GetComponent<Scr_ControladorArmas>().BalaADisparar;

        Controlador.GetComponent<Scr_ControladorArmas>().BalaADisparar = Controlador.GetComponent<Scr_ControladorArmas>().GranadaPrefab[0];
        yield return new WaitForSeconds(3f);
        Controlador.GetComponent<Scr_ControladorArmas>().BalaADisparar = balaAnterior;
    }
    IEnumerator HabilidadMascizo()
    {
        //Guardamos La cantidad de perdigones que tenia
        DispersionAnterior = Controlador.GetComponent<Scr_ControladorArmas>().dispersion;

        Controlador.GetComponent<Scr_ControladorArmas>().dispersion = .1f;
        yield return new WaitForSeconds(7f);
        Controlador.GetComponent<Scr_ControladorArmas>().dispersion = DispersionAnterior;
    }

    IEnumerator HabilidadImparable()
    {

        Controlador.GetComponent<Scr_ControladorArmas>().havecombo = true;
        yield return new WaitForSeconds(5f);
        Controlador.GetComponent<Scr_ControladorArmas>().havecombo = false;
    }
   
    IEnumerator HabilidadPlantaMini()
    {
        //Guardamos La cantidad de perdigones que tenia
        balaAnterior = Controlador.GetComponent<Scr_ControladorArmas>().BalaADisparar;

        Controlador.GetComponent<Scr_ControladorArmas>().BalaADisparar = Controlador.GetComponent<Scr_ControladorArmas>().GranadaPrefab[2];
        yield return new WaitForSeconds(9f);
        Controlador.GetComponent<Scr_ControladorArmas>().BalaADisparar = balaAnterior;
    }

    IEnumerator HabilidadColoso()
    {
        Controlador.GetComponent<Scr_ControladorArmas>().empuje = true;
        Controlador.GetComponent<Scr_ControladorBatalla>().PorcentajeQuitar = .7f;
        yield return new WaitForSeconds(8f);
        Controlador.GetComponent<Scr_ControladorBatalla>().PorcentajeQuitar = 1f;
        Controlador.GetComponent<Scr_ControladorArmas>().empuje = false;
    }

    IEnumerator HabilidadBlindado()
    {
        Controlador.GetComponent<Scr_ControladorBatalla>().PorcentajeQuitar = .7f;
        yield return new WaitForSeconds(6f);
        Controlador.GetComponent<Scr_ControladorBatalla>().PorcentajeQuitar = 1f;
    }

    IEnumerator HabilidadSangria()
    {
        Controlador.GetComponent<Scr_ControladorArmas>().sangria = true;
        yield return new WaitForSeconds(3f);
        Controlador.GetComponent<Scr_ControladorArmas>().sangria = false;
    }

    IEnumerator HabilidadVolador()
    {
        Controlador.GetComponent<Scr_ControladorArmas>().empuje = true;
        yield return new WaitForSeconds(7f);
        Controlador.GetComponent<Scr_ControladorArmas>().empuje = false;
    }

    IEnumerator HabilidadRaizEterna()
    {
        Controlador.GetComponent<Scr_ControladorArmas>().empuje = true;
        yield return new WaitForSeconds(7f);
        Controlador.GetComponent<Scr_ControladorArmas>().empuje = false;
    }

    IEnumerator HabilidadFelina()
    {
        GameObject personaje = GameObject.Find("Personaje");
        Scr_Movimiento mov=personaje.GetComponent<Scr_Movimiento>();
        SaltoAnterior = mov.FuerzaSalto;
        VagachadoAnterior = mov.VelAgachado;
        VcaminarAnterior = mov.VelCaminar;
        VcorrerAnterior = mov.VelCorrer;
        mov.FuerzaSalto = mov.FuerzaSalto * 2;
        mov.VelAgachado = mov.VelAgachado * 2;
        mov.VelCaminar = mov.VelCaminar * 2;
        mov.VelCorrer = mov.VelCorrer * 2;
        yield return new WaitForSeconds(3f);
        mov.VelAgachado = VagachadoAnterior;
        mov.VelCaminar = VcaminarAnterior;
        mov.VelCorrer = VcorrerAnterior;
        mov.FuerzaSalto = SaltoAnterior ;
    }

    IEnumerator HabilidadRugidoTierra()
    {
        GameObject personaje = GameObject.Find("Armas");
        Transform onda = personaje.transform.GetChild(3);
        onda.gameObject.SetActive(true); 
        yield return new WaitForSeconds(.1f);
        onda.gameObject.SetActive(false);
    }

    IEnumerator HabilidadSemillaExplosiva()
    {
        GameObject personaje = GameObject.Find("Armas");
        Transform point = personaje.transform.GetChild(2);
        yield return new WaitForSeconds(2f);
        Controlador.GetComponent<Scr_ControladorArmas>().BalaADisparar = balaAnterior;

        DispararObjeto(15, Controlador.GetComponent<Scr_ControladorArmas>().GranadaPrefab[2], point);
    }
    void DispararObjeto(int cantidad, GameObject prefab, Transform point)
    {
        for (int i = 0; i < cantidad; i++)
        {
            GameObject bala = Instantiate(prefab, point.position, point.rotation);
            // Direccion base
            Vector3 direccionBase = point.up;

            // Añadir dispersión aleatoria
            Vector3 direccionConDispersion = DireccionConDispersion(direccionBase, Random.Range(0, 15));

            // Aplicar fuerza
            Rigidbody rb = bala.GetComponent<Rigidbody>();
            rb.AddForce(direccionConDispersion * 50, ForceMode.Impulse);
        }
    }
    Vector3 DireccionConDispersion(Vector3 direccion, float dispersionEnGrados)
    {
        // Convertir grados a radianes
        float maxAngle = dispersionEnGrados / 2f;

        // Generar un pequeño desvío aleatorio
        float angleX = Random.Range(-maxAngle, maxAngle);
        float angleY = Random.Range(-maxAngle, maxAngle);

        // Aplicar rotación a la dirección
        Quaternion rot = Quaternion.Euler(angleX, angleY, 0);
        return rot * direccion;
    }


    IEnumerator HabilidadTiroImparable()
    {

        Controlador.GetComponent<Scr_ControladorArmas>().Maspenetracion = 2;
        yield return new WaitForSeconds(3f);
        Controlador.GetComponent<Scr_ControladorArmas>().Maspenetracion = 0;
    }
    IEnumerator HabilidadTiroEsparcido()
    {
        TipoAnterior= Controlador.GetComponent<Scr_ControladorArmas>().Tipo;
        Controlador.GetComponent<Scr_ControladorArmas>().Tipo = "Escopeta";
        yield return new WaitForSeconds(3f);
        Controlador.GetComponent<Scr_ControladorArmas>().Tipo = TipoAnterior;
        Controlador.GetComponent<Scr_ControladorArmas>().Maspenetracion = 0;
    }

    IEnumerator HabilidadMasArea()
    {
        Controlador.GetComponent<Scr_ControladorArmas>().MasDistancia = 50;
        yield return new WaitForSeconds(9f);
        Controlador.GetComponent<Scr_ControladorArmas>().MasDistancia = 0;
    }

    private void Habilidadempuje()
    {
        Controlador.GetComponent<Scr_ControladorArmas>().empuje = true;
    }
    private void HabilidadMinima()
    {
        Controlador.GetComponent<Scr_ControladorArmas>().minLimit = true;
    }

    // Transiciona el Vignette y lo restaura tras una duración
    private IEnumerator ModificarVignette(Vignette vignette, Color colorFinal, float intensidadFinal, float duracion)
    {
        if (vignetteCoroutine != null)
        {
            StopCoroutine(vignetteCoroutine);
        }

        vignetteCoroutine = StartCoroutine(ModificarVignetteInterno(vignette, colorFinal, intensidadFinal, duracion));
        yield return vignetteCoroutine;

        vignetteCoroutine = null;
    }


    // Transiciona el Lens Distortion y lo restaura tras una duración
    private IEnumerator ModificarLensDistortion(LensDistortion lens, float intensidadFinal, float duracion)
    {
        if (LensCoroutine != null)
        {
            StopCoroutine(LensCoroutine);
        }
        LensCoroutine = StartCoroutine(ModificarLensInterno(lens,intensidadFinal,duracion));

        yield return LensCoroutine;
        LensCoroutine= null;
    }

    private IEnumerator ModificarColorAdjustments(ColorAdjustments colorAdjustments, float SaturacionFinal, float duracion)
    {
        if (colorAdjustmentsCoroutine != null)
        {
            StopCoroutine(colorAdjustmentsCoroutine);
        }

        colorAdjustmentsCoroutine = StartCoroutine(ModificarColorAdjustmentsInterno(colorAdjustments, SaturacionFinal, duracion));
        yield return colorAdjustmentsCoroutine;

        colorAdjustmentsCoroutine = null;
    }

    private IEnumerator ModificarVignetteInterno(Vignette vignette, Color colorFinal, float intensidadFinal, float duracion)
    {
        Color colorInicial = vignette.color.value;
        float intensidadInicial = vignette.intensity.value;

        yield return StartCoroutine(TransicionarPropiedad(
            t => vignette.color.value = Color.Lerp(colorInicial, colorFinal, t), 0.1f));
        yield return StartCoroutine(TransicionarPropiedad(
            t => vignette.intensity.value = Mathf.Lerp(intensidadInicial, intensidadFinal, t), 0.1f));

        yield return new WaitForSeconds(duracion);

        yield return StartCoroutine(TransicionarPropiedad(
            t => vignette.color.value = Color.Lerp(colorFinal, colorInicial, t), 0.1f));
        yield return StartCoroutine(TransicionarPropiedad(
            t => vignette.intensity.value = Mathf.Lerp(intensidadFinal, intensidadInicial, t), 0.1f));
    }

    private IEnumerator ModificarLensInterno(LensDistortion LensDistortion, float ValorFinal, float duracion)
    {
        float ValorInicial = LensDistortion.intensity.value;

        yield return StartCoroutine(TransicionarPropiedad(
            t => LensDistortion.intensity.value = Mathf.Lerp(ValorInicial, ValorFinal, t), 0.1f));

        yield return new WaitForSeconds(duracion);

        yield return StartCoroutine(TransicionarPropiedad(
            t => LensDistortion.intensity.value = Mathf.Lerp(ValorFinal, ValorInicial, t), 0.1f));
    }
    private IEnumerator ModificarColorAdjustmentsInterno(ColorAdjustments colorAdjustments, float SatFinal, float duracion)
    {
        float SatInicial = colorAdjustments.saturation.value;

        yield return StartCoroutine(TransicionarPropiedad(
            t => colorAdjustments.saturation.value = Mathf.Lerp(SatInicial, SatFinal, t), 0.1f));

        yield return new WaitForSeconds(duracion);

        yield return StartCoroutine(TransicionarPropiedad(
            t => colorAdjustments.saturation.value = Mathf.Lerp(SatFinal, SatInicial, t), 0.1f));
    }

    // Regresa la velocidad de un enemigo después de una duración
    private IEnumerator RegresarVelocidad(float duracion, GameObject enemigo)
    {
        yield return new WaitForSeconds(duracion);

        if (enemigo != null)
        {

            enemigo.GetComponent<Scr_Enemigo>().DuracionDeAtaque = DuracionAtaqueInicial;
            enemigo.GetComponent<Scr_Enemigo>().Velocidad = velocidadInicial;
            enemigo.GetComponent<Animator>().speed = 1f;
            enemigo.GetComponent<NavMeshAgent>().speed = velocidadInicial;
        }
    }

    // Activa un efecto visual y lo desactiva después de una duración
    private IEnumerator ActivarEfectoVisual(float duracion)
    {
        int DañoInicial = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño;
        Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño = DañoInicial * 2;
        Efecto.transform.GetChild(0).gameObject.SetActive(true);
        Efecto.transform.GetChild(0).GetComponent<Animator>().Play("ZoomAparecer");
        yield return new WaitForSeconds(0.05f);
        Efecto.transform.GetChild(0).GetComponent<Animator>().Play("ZoomBaile");
        yield return new WaitForSeconds(duracion - 0.1f);
        Efecto.transform.GetChild(0).GetComponent<Animator>().Play("ZoomDesaparecer");
        yield return new WaitForSeconds(0.05f);
        Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño = DañoInicial;
        Efecto.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Transiciona una propiedad de 0 a 1 en un tiempo definido
    private IEnumerator TransicionarPropiedad(System.Action<float> accion, float duracion)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            accion(tiempo / duracion);
            yield return null;
        }

        accion(1f); // Asegura que la transición termina en el valor final
    }
}
