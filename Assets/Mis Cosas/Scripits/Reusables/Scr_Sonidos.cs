using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Sonidos : MonoBehaviour
{
    [Header("Sonido especial")]
    public AudioClip sonidoespecial;

    [Header("Listas de sonidos")]
    public AudioClip[] caminar_sonido;
    public AudioClip[] correr_sonido;
    public AudioClip[] recoger_sonido;
    public AudioClip[] talar_sonido;
    public AudioClip[] picar_sonido;

    private AudioSource source;
    Scr_ControladorAnimacionesGata Anim;
    void Start()
    {
        source = GetComponent<AudioSource>();
        if (gameObject.name == "Gata")
        {
            Anim = GetComponent<Scr_ControladorAnimacionesGata>();
        }
    }

    // ---------------- FUNCIONES ----------------

    public void Play_Sonido()
    {
        if (sonidoespecial == null) return;
        source.PlayOneShot(sonidoespecial);
    }

    public void Play_caminar()
    {
        if (caminar_sonido.Length == 0) return;
        AudioClip clip = caminar_sonido[Random.Range(0, caminar_sonido.Length)];
        source.PlayOneShot(clip);
    }

    public void Play_correr()
    {
        if (correr_sonido.Length == 0) return;
        AudioClip clip = correr_sonido[Random.Range(0, correr_sonido.Length)];
        source.PlayOneShot(clip);
    }

    public void Play_recoger()
    {
        if (recoger_sonido.Length == 0) return;
        AudioClip clip = recoger_sonido[Random.Range(0, recoger_sonido.Length)];
        source.PlayOneShot(clip);
    }

    public void Play_talar()
    {
        if (talar_sonido.Length == 0) return;
        AudioClip clip = talar_sonido[Random.Range(0, talar_sonido.Length)];
        source.PlayOneShot(clip);
    }

    public void herramienta()
    {
        if (Anim == null) return;
        if(Anim.HabilidadUsando== "Hacha Madera")//contains
        {
            Play_talar();
        }
        else if (Anim.HabilidadUsando == "Pico Madera")
        {
            Play_picar();
        }
    }

    public void Play_picar()
    {
        if (picar_sonido.Length == 0) return;
        AudioClip clip = picar_sonido[Random.Range(0, picar_sonido.Length)];
        source.PlayOneShot(clip);
    }

}
