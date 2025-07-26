using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

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
                Debug.Log("Ajusta Tiempo ease");
                brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                brain.m_DefaultBlend.m_Time = Tiempos[Escena];
            }
            else
            {
                Debug.Log("Entra Cut");
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
            GameObject.Find("Cosas Inutiles").transform.GetChild(3).gameObject.SetActive(true);

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

    private IEnumerator PrecargarEscena(int Escena)
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
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().Enemigo = Enemigo;
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().Luz = GameObject.Find("Sol").GetComponent<Light>().color;
    }

    public void GuardarCinematica(string Cinematica)
    {
        Debug.Log("Escena Guardada");
        PlayerPrefs.SetString("Cinematica " + Cinematica, "Si");
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
}
