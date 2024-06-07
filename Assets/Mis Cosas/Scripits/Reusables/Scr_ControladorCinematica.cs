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

    private UnityEngine.AsyncOperation Operacion;

    void Update()
    {
        if (Director.state != PlayState.Playing && !Panel.activeSelf && Escena < Timelines.Length - 1 && !PausaAlTerminar[Escena])
        {
            Escena++;
            if (Easy[Escena])
            {
                Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = Tiempos[Escena];
            }
            else
            {
                Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
            }
            Director.playableAsset = Timelines[Escena];
            Director.Play();
        }
    }

    public void CambiarEscena(int Escena)
    {
        if (Operacion != null && Operacion.progress >= 0.9f)
        {
            Operacion.allowSceneActivation = true;
        }
    }

    public void ActivadorPrecarga(int Escena)
    {
        StartCoroutine(PrecargarEscena(Escena));
    }

    private IEnumerator PrecargarEscena(int Escena)
    {
        // Inicia la carga as�ncrona de la escena
        Operacion = SceneManager.LoadSceneAsync(Escena);
        // No permitas que la escena se active autom�ticamente cuando termine de cargar
        Operacion.allowSceneActivation = false;

        // Opcional: Espera hasta que la escena est� completamente cargada
        while (!Operacion.isDone)
        {
            // Comprueba si la carga est� completa
            if (Operacion.progress >= 0.9f)
            {
                Debug.Log("La escena est� precargada.");
                // Puedes realizar alguna acci�n aqu� si es necesario
            }

            // Espera un frame antes de volver a comprobar
            yield return null;
        }
    }
}
