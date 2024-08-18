using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
public class Scr_ControladorCinematica : MonoBehaviour
{

    int Escena = 0;
    [SerializeField] PlayableDirector Director;
    [SerializeField] PlayableAsset[] Timelines;
    [SerializeField] GameObject Panel;
    [SerializeField] bool[] Easy;
    [SerializeField] float[] Tiempos;
    [SerializeField] public bool[] PausaAlTerminar;
    [SerializeField] GameObject[] ObjetosApagar;
    [SerializeField] GameObject[] ObjetosEncender;
    [SerializeField] GameObject Enemigo;
    [SerializeField] int CantidadEnemigos;
    [SerializeField] Animator[] Barras;

    private AsyncOperation Operacion;

    void Update()
    {
        // Condiciones para cambiar de escena
        if (Director.state != PlayState.Playing && !Panel.activeSelf && Escena < Timelines.Length && !PausaAlTerminar[Escena])
        {
            // Configura la transición de la cámara dependiendo si la escena es fácil o no
            if (Easy[Escena])
            {
                Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = Tiempos[Escena];
            }
            else
            {
                Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
            }

            // Asigna el nuevo asset de la timeline al Director
            Director.playableAsset = Timelines[Escena];
            Director.Play();  // Inicia la reproducción del nuevo asset de la timeline
            Escena++;  // Incrementa el índice de la escena para pasar a la siguiente
        }

        if (Escena >= Timelines.Length && !Panel.activeSelf && Director.state != PlayState.Playing)
        {
            Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = 2;

            //Apagar Elementos y cargar objetos
            foreach (GameObject Objeto in ObjetosEncender)
            {
                Objeto.SetActive(true);
            }
            foreach(GameObject Objeto in ObjetosApagar)
            {
                Escena = 0;
                Objeto.SetActive(false);
            }

        }
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
        // Inicia la carga asíncrona de la escena
        Operacion = SceneManager.LoadSceneAsync(Escena);
        // No permitas que la escena se active automáticamente cuando termine de cargar
        Operacion.allowSceneActivation = false;

        // Opcional: Espera hasta que la escena esté completamente cargada
        while (!Operacion.isDone)
        {
            // Comprueba si la carga está completa
            if (Operacion.progress >= 0.9f)
            {
                Debug.Log("La escena está precargada.");
                // Puedes realizar alguna acción aquí si es necesario
            }

            // Espera un frame antes de volver a comprobar
            yield return null;
        }
    }

    public void AsignarSinleton()
    {
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().Enemigo = Enemigo;
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().CantidadDeEnemigos = CantidadEnemigos;
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
        foreach(Animator Anim in Barras)
        {
            Anim.Play("Cerrar");
        }
    }


}