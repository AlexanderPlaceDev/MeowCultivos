using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Sonidos : MonoBehaviour
{
    public AudioClip[] caminar_sonido;
    public AudioClip[] correr_sonido;
    public AudioClip[] recoger_sonido;
    public AudioClip[] talar_sonido;
    public AudioClip[] picar_sonido;


    public int caminar;
    public int correr;
    public int recoger;
    public int talar;
    public int picar;
    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.volume = .2f;
        /*source.clip = caminar_sonido;
        source.spatialBlend = 1.0f; // Hacerlo 3D
        source.minDistance = 1f;
        source.maxDistance = 15f;
        source.Play();*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play_caminar()
    {
        if (caminar_sonido[caminar] == null) return;
        source.clip = caminar_sonido[caminar];
        source.Play();
    }
    public void Play_correr()
    {
        if (correr_sonido[correr] == null) return;
        source.clip = correr_sonido[correr];
        source.Play();
    }
    public void Play_recoger()
    {
        if (recoger_sonido[recoger] == null) return;
        source.clip = recoger_sonido[recoger];
        source.Play();
    }

    public void Play_talar()
    {

        if (recoger_sonido[talar] == null) return;
        source.clip = recoger_sonido[talar];
        source.Play();
    }
    public void Play_picar()
    {
        if (recoger_sonido[picar] == null) return;
        source.clip = recoger_sonido[picar];
        source.Play();
    }
}
