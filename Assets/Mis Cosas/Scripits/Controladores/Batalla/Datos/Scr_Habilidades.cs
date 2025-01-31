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
    private float lensDistortionInicial;      // Valor inicial del Lens Distortion
    private float ColorAdjustmentsInicial;      // Valor inicial del Lens Distortion

    [SerializeField] GameObject[] AreasGarras;

    private GameObject Singleton;
    private GameObject Controlador;

    private Coroutine vignetteCoroutine; // Manejo de Vignette
    private Coroutine colorAdjustmentsCoroutine; // Manejo de ColorAdjustments
    private Coroutine LensCoroutine; // Manejo de ColorAdjustments

    private void Start()
    {
        Singleton = GameObject.Find("Singleton");
        Controlador = GameObject.Find("Controlador");
    }

    public void Habilidad(string habilidad)
    {
        Debug.Log($"Habilidad {habilidad} activada");

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
                    enemigo.GetComponent<Scr_Enemigo>().Velocidad = velocidadInicial / 3f;
                    enemigo.GetComponent<Animator>().speed = 1 / 3f;
                    enemigo.GetComponent<NavMeshAgent>().speed = velocidadInicial / 3f;

                    StartCoroutine(RegresarVelocidad(5f, enemigo));
                }
                break;

            default:
                Debug.Log("No se encontró la habilidad.");
                break;
        }
    }

    IEnumerator HabilidadGarras()
    {
        AreasGarras[0].SetActive(true);
        AreasGarras[1].SetActive(true);
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
        Armas.transform.GetChild(0).gameObject.SetActive(true);
        Personaje.GetComponent<Scr_Movimiento>().enabled = true;
        Armas.transform.GetChild(0).GetComponent<Animator>().Play("AparecerBrazos1");
        Armas.transform.GetChild(1).gameObject.SetActive(false);
        Efecto.transform.GetChild(1).gameObject.SetActive(false);
        AreasGarras[0].SetActive(false);
        AreasGarras[1].SetActive(false);
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
