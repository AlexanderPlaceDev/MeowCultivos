using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorArmas : MonoBehaviour
{
    [SerializeField] Scr_CreadorArmas TodasLasArmas;
    [SerializeField] GameObject ObjetoArmas;
    int ArmaActual = 0;
    bool Atacando = false;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) && !Atacando)
        {
            Disparar();
        }
    }

    void CambiarArma()
    {

    }

    void Disparar()
    {
        if (ArmaActual == 0)
        {
            Animator Anim = ObjetoArmas.transform.GetChild(0).GetComponent<Animator>();
            Anim.Play("Golpear");
            float Duracion = GetAnimationClipDuration(Anim, "Brazos_Golpe");
            StartCoroutine(EsperarAtaque(Duracion));
        }
    }

    void Recargar()
    {

    }

    IEnumerator EsperarAtaque(float Segundos)
    {
        Atacando = true;
        yield return new WaitForSeconds(Segundos);
        Atacando = false;
    }

    float GetAnimationClipDuration(Animator animator, string clipName)
    {
        if (animator == null) return 0f;

        // Obtén todos los clips del RuntimeAnimatorController
        RuntimeAnimatorController runtimeController = animator.runtimeAnimatorController;
        AnimationClip[] clips = runtimeController.animationClips;

        // Encuentra el clip con el nombre especificado
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning("Clip no encontrado: " + clipName);
        return 0f;
    }

    public void ActivarColision(GameObject Col)
    {
        Col.SetActive(true);
    }
    public void DesactivarColision(GameObject Col)
    {
        Col.SetActive(false);
    }
}
