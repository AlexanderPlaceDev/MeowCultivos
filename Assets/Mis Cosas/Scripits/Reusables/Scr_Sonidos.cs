using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Sonidos : MonoBehaviour
{
    public AudioClip sonidoespecial;
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
        source = GetComponent<AudioSource>();
        //aplica el volumen 
        int volumen_general = PlayerPrefs.GetInt("Volumen", 50);
        int volumen_ambiental = PlayerPrefs.GetInt("Volumen_Ambiente", 20);
        float volumen = (volumen_general * volumen_ambiental) / 100;
        source.volume = volumen;
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

    public void Play_Sonido()
    {
        if (sonidoespecial == null) return;
        source.PlayOneShot(sonidoespecial);
    }
    public void Play_caminar()
    {
        if (caminar_sonido[caminar] == null) return;
        source.PlayOneShot(caminar_sonido[caminar]);
    }
    public void Play_correr()
    {
        if (correr_sonido[correr] == null) return;
        source.PlayOneShot(correr_sonido[correr]);
    }
    public void Play_recoger()
    {
        if (recoger_sonido[recoger] == null) return;
        source.PlayOneShot(recoger_sonido[recoger]);
    }

    public void Play_talar()
    {
        if (talar_sonido[talar] == null) return; 
        source.PlayOneShot(talar_sonido[talar]);
    }

    public void Play_picar()
    {
        if (picar_sonido[picar] == null) return;
        source.PlayOneShot(picar_sonido[picar]);
    }

}
