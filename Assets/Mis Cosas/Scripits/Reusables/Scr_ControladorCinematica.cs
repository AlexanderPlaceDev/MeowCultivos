using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class Scr_ControladorCinematica : MonoBehaviour
{

    int Escena = 0;
    [SerializeField] PlayableDirector Director;
    [SerializeField] PlayableAsset[] Timelines;
    [SerializeField] GameObject Panel;
    [SerializeField] bool[] Easy;
    [SerializeField] float[] Tiempos;

    void Update()
    {
        if (Director.state != PlayState.Playing && !Panel.activeSelf && Escena < Timelines.Length - 1)
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

}