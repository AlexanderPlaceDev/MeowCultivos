using Cinemachine;
using PrimeTween;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scr_ControladorCinematica : MonoBehaviour
{
    int Escena = 0;
    bool esperandoReproduccion = false;

    [SerializeField] PlayableDirector Director;
    [SerializeField] PlayableAsset[] Timelines;
    [SerializeField] GameObject Panel;
    [SerializeField] bool[] Easy;
    [SerializeField] float[] Tiempos;
    [SerializeField, Tooltip("Pausar en caso de terminar la cinematica en dialogo")]
    public bool[] PausaAlTerminar;
    [SerializeField] GameObject[] ObjetosApagar;
    [SerializeField] GameObject[] ObjetosEncender;
    [SerializeField] Collider ActivadorCinematicaSiguiente;
    [SerializeField] GameObject Enemigo;
    [SerializeField] int CantidadEnemigos;
    [SerializeField] Animator[] Barras;

    private AsyncOperation Operacion;

    public void Update()
    {
        // Avanza a la siguiente cinemática si ya terminó la anterior
        if (!esperandoReproduccion && Director.state != PlayState.Playing && !Panel.activeSelf && Escena < Timelines.Length && !PausaAlTerminar[Escena])
        {

            // Configura blending según si es "Easy"
            var brain = Camera.main.GetComponent<CinemachineBrain>();
            if (Easy[Escena])
            {
                brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                brain.m_DefaultBlend.m_Time = Tiempos[Escena];
            }
            else
            {
                brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
            }

            // Reproduce la cinemática
            Director.playableAsset = Timelines[Escena];
            Director.Play();

            // Aumenta el índice de escena
            Escena++;

            // Espera a que comience la reproducción antes de permitir otro avance
            StartCoroutine(EsperarInicioReproduccion());
        }

        // Si todas las cinemáticas terminaron
        if (Escena >= Timelines.Length && !Panel.activeSelf && Director.state != PlayState.Playing)
        {
            var brain = Camera.main.GetComponent<CinemachineBrain>();
            brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            Debug.Log("Actualiza");
            brain.m_DefaultBlend.m_Time = 2;

            // Encender objetos
            foreach (GameObject Objeto in ObjetosEncender)
            {
                Objeto.SetActive(true);
            }
            //Activar siguiente cinematica
            if (ActivadorCinematicaSiguiente != null)
            {
                ActivadorCinematicaSiguiente.enabled = true;
            }

            // Apagar objetos
            foreach (GameObject Objeto in ObjetosApagar)
            {
                Objeto.SetActive(false);
            }

            // ✅ REACTIVAR MOVIMIENTO Y CÁMARA 360
            GameObject gata = GameObject.Find("Gata");
            if (gata != null)
            {
                var animControl = gata.GetComponent<Scr_ControladorAnimacionesGata>();
                animControl.EstaEnCinematica = false;
                if (animControl != null)
                {
                    animControl.PuedeCaminar = true;
                }

                var movimiento = gata.GetComponent<Scr_Movimiento>();
                if (movimiento != null)
                {
                    movimiento.enabled = true;
                }
            }

            //Activar camara principal
            if (GameObject.Find("Cosas Inutiles") != null)
            {
                GameObject.Find("Cosas Inutiles").transform.GetChild(3).gameObject.SetActive(true);
            }

            Escena = 0; // Reinicia solo una vez
        }
    }

    private IEnumerator EsperarInicioReproduccion()
    {
        // Espera 0.1s y luego permite otra reproducción
        yield return new WaitForSeconds(0.1f);
        esperandoReproduccion = false;
    }

    public void CambiarEscena(int Escena)
    {
        if (Operacion != null && Operacion.progress >= 0.9f)
        {
            Operacion.allowSceneActivation = true;
        }
    }

    public void CambiarEscenaForzada(int Escena)
    {
        SceneManager.LoadScene(Escena);
    }

    public void ActivadorPrecarga(int Escena)
    {
        StartCoroutine(PrecargarEscena(Escena));
    }

    public IEnumerator PrecargarEscena(int Escena)
    {
        Operacion = SceneManager.LoadSceneAsync(Escena);
        Operacion.allowSceneActivation = false;

        while (!Operacion.isDone)
        {
            if (Operacion.progress >= 0.9f)
            {
                Debug.Log("La escena está precargada.");
            }

            yield return null;
        }
    }

    public void AsignarSinleton()
    {
        Scr_DatosSingletonBatalla Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
        Singleton.Enemigo = Enemigo.GetComponent<Scr_CambiadorBatalla>().PrefabEnemigo;
        Singleton.Mision = Enemigo.GetComponent<Scr_CambiadorBatalla>().Mision;
        Singleton.ColorMision = Enemigo.GetComponent<Scr_CambiadorBatalla>().ColorMision;
        Singleton.Complemento = Enemigo.GetComponent<Scr_CambiadorBatalla>().Complemento;
        Singleton.Item = Enemigo.GetComponent<Scr_CambiadorBatalla>().Item;
        Singleton.ColorItem = Enemigo.GetComponent<Scr_CambiadorBatalla>().ColorItem;
        Singleton.Luz = GameObject.Find("Sol").GetComponent<Light>().color;
    }

    public void GuardarCinematica(string Cinematica)
    {
        Debug.Log("Escena Guardada");
        PlayerPrefs.SetString("Cinematica " + Cinematica, "Si");

        Scr_ControladorTiempo Tiempo = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();

        PlayerPrefs.SetString("DiaCinematica:" + gameObject.transform.parent.parent.name, Tiempo.DiaActual);
        PlayerPrefs.SetInt("HoraCinematica:" + gameObject.transform.parent.parent.name, Tiempo.HoraActual);
        Debug.Log("Guarda desde: " + gameObject.name);
        if (transform.parent.parent.GetComponent<Scr_ActivadorElementos>().CinematicaSiguiente == null && transform.parent.parent.GetComponent<Scr_ActivadorElementos>().UsaEventoGeneral)
        {
            Debug.Log("Desactiva evento");
            GameObject.Find("EventosGenerales").GetComponent<Controlador_EventosGenerales>().DesactivarEvento(transform.parent.parent.GetComponent<Scr_ActivadorElementos>().NombreEventoGeneral);
        }
        PlayerPrefs.Save();

    }

    public void GuardarPosicion(Transform Trans)
    {
        PlayerPrefs.SetFloat("GataPosX", Trans.position.x);
        PlayerPrefs.SetFloat("GataPosY", Trans.position.y);
        PlayerPrefs.SetFloat("GataPosZ", Trans.position.z);

        PlayerPrefs.SetFloat("GataRotX", Trans.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("GataRotY", Trans.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("GataRotZ", Trans.rotation.eulerAngles.z);
    }

    public void PermiteGuardarPosicion()
    {
        GameObject.Find("Gata").GetComponent<Scr_Movimiento>().PuedeGuardarPosicion = true;
    }

    public void DesactivaGuardarPosicion()
    {
        GameObject.Find("Gata").GetComponent<Scr_Movimiento>().PuedeGuardarPosicion = false;
    }

    public void CerrarBarras()
    {
        foreach (Animator Anim in Barras)
        {
            Anim.Play("Cerrar");
        }
    }

    public void ActivarReloj()
    {
        GameObject Reloj = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        Reloj.SetActive(true);
        Tween.UIAnchoredPositionY(Reloj.transform.GetChild(0).GetComponent<RectTransform>(), 15, 0.5f, Ease.Default);
        Tween.UIAnchoredPositionX(Reloj.transform.GetChild(1).GetComponent<RectTransform>(), 0, 0.5f, Ease.Default);
        Tween.UIAnchoredPositionX(Reloj.transform.GetChild(2).GetComponent<RectTransform>(), -610, 0.5f, Ease.Default);

    }

    public void DesactivarReloj()
    {

        GameObject Reloj = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        Tween.UIAnchoredPositionY(Reloj.transform.GetChild(0).GetComponent<RectTransform>(), -100, 0.5f, Ease.Default);
        Tween.UIAnchoredPositionX(Reloj.transform.GetChild(1).GetComponent<RectTransform>(), 226, 0.5f, Ease.Default);
        Tween.UIAnchoredPositionX(Reloj.transform.GetChild(2).GetComponent<RectTransform>(), -800, 0.5f, Ease.Default);
        StartCoroutine(EsperarReloj(Reloj));
    }

    IEnumerator EsperarReloj(GameObject Reloj)
    {
        yield return new WaitForSeconds(0.5f);
        Reloj.SetActive(false);
    }

    public void AumentarRango(string Rama)
    {
        // Aumenta rango interno
        PlayerPrefs.SetInt(
            "Rango Barra " + Rama,
            PlayerPrefs.GetInt("Rango Barra " + Rama, 0) + 1
        );

        // Limpia la rama (Arsenal3 -> Arsenal)
        string ramaLimpia = Rama.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

        // Activa UI de rango
        GameObject rangoUI = GameObject.Find("Canvas").transform.GetChild(10).gameObject;
        rangoUI.SetActive(true);

        // Muestra rango (interno + 1 si lo necesitas visual)
        rangoUI.GetComponent<Scr_NuevoRango>()
            .MostrarRango(
                ramaLimpia,
                PlayerPrefs.GetInt("Rango Barra " + Rama, 0)
            );
    }

}