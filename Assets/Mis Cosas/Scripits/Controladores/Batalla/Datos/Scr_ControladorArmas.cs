using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorArmas : MonoBehaviour
{
    [SerializeField] Scr_CreadorArmas[] TodasLasArmas;
    [SerializeField] GameObject ObjetoArmas;
    [SerializeField] Image Mira;
    [SerializeField] Sprite[] Mirillas;
    [SerializeField] Color[] ColoresMirillas;
    [SerializeField] float LongitudRaycast;
    [SerializeField] Vector3 origenRaycast;
    [SerializeField] bool mostrarRaycast = false;

    int ArmaActual = 0;
    bool Atacando = false;  // Evita que se interrumpa la animación actual
    public int CantBalasActual = 0;

    private float tiempoDesdeUltimoGolpe = 0f;
    private int numGolpe = 1;

    GameObject Gata;

    void Start()
    {
        Gata = GameObject.Find("Personaje");
        CantBalasActual = TodasLasArmas[ArmaActual].Capacidad;
        Physics.IgnoreLayerCollision(7, 8);
    }

    void Update()
    {
        // Contador de tiempo para resetear el combo
        if (tiempoDesdeUltimoGolpe > 0)
        {
            tiempoDesdeUltimoGolpe -= Time.deltaTime;
            if (tiempoDesdeUltimoGolpe <= 0)
            {
                numGolpe = 1; // Si pasan más de 5s sin atacar, vuelve a Golpe 1
            }
        }

        // Solo permite atacar si no está atacando y puede disparar
        if (Input.GetKey(KeyCode.Mouse0) && !Atacando && PuedeDisparar())
        {
            Disparar();
        }
    }

    bool PuedeDisparar()
    {
        return CantBalasActual > 0 || TodasLasArmas[ArmaActual].Tipo == "Cuerpo a Cuerpo";
    }

    void Disparar()
    {
        if (ObjetoArmas == null) return;

        Transform ArmaAct = ObjetoArmas.transform.GetChild(ArmaActual);
        if (ArmaAct == null) return;

        Animator Anim = ArmaAct.GetComponent<Animator>();
        if (Anim == null) return;

        EjecutarAtaque(Anim);
    }

    void EjecutarAtaque(Animator Anim)
    {
        // Bloquea el ataque hasta que termine la animación
        Atacando = true;

        // Selecciona la animación según el golpe actual
        string animacion = "Golpe " + numGolpe;
        Anim.Play(animacion);

        // Obtiene la duración del golpe y espera antes de permitir otro ataque
        float duracion = GetAnimationClipDuration(Anim, animacion);
        StartCoroutine(EsperarAtaque(duracion));

        // Avanza en la secuencia de golpes
        numGolpe = (numGolpe % 3) + 1;

        // Reinicia el contador de tiempo
        tiempoDesdeUltimoGolpe = 5f;
    }

    IEnumerator EsperarAtaque(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        Atacando = false;  // Ahora puede volver a atacar
    }

    float GetAnimationClipDuration(Animator animator, string clipName)
    {
        if (animator == null) return 0f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) return clip.length;
        }
        return 0f;
    }
}
