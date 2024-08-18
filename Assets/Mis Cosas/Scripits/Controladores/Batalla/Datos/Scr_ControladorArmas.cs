using System.Collections;
using UnityEngine;

public class Scr_ControladorArmas : MonoBehaviour
{
    [SerializeField] Scr_CreadorArmas TodasLasArmas;
    [SerializeField] GameObject ObjetoArmas;
    int ArmaActual = 0;
    bool Atacando = false;

    // Añadir la cadencia de disparo (tiempo entre disparos)
    [SerializeField] float cadenciaDisparo = 0.5f;
    private float tiempoUltimoDisparo;

    void Start()
    {
        tiempoUltimoDisparo = -cadenciaDisparo; // Permitir disparar inmediatamente al inicio

        // Verificar referencias
        if (TodasLasArmas == null)
        {
            Debug.LogError("TodasLasArmas no está asignado en el Inspector.");
        }
        if (ObjetoArmas == null)
        {
            Debug.LogError("ObjetoArmas no está asignado en el Inspector.");
        }
    }

    void Update()
    {
        // Detectar si el botón del ratón está siendo mantenido
        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= tiempoUltimoDisparo + cadenciaDisparo)
        {
            Disparar();
        }
    }

    void CambiarArma()
    {
        // Implementar lógica para cambiar de arma si es necesario
    }

    void Disparar()
    {
        if (!Atacando)
        {
            if (ArmaActual == 0)
            {
                if (ObjetoArmas != null)
                {
                    Transform childTransform = ObjetoArmas.transform.GetChild(0);
                    if (childTransform != null)
                    {
                        Animator Anim = childTransform.GetComponent<Animator>();
                        if (Anim != null)
                        {
                            Debug.Log("Disparando arma...");
                            Anim.Play("Golpear");
                            float Duracion = GetAnimationClipDuration(Anim, "Brazos_Golpe");
                            StartCoroutine(EsperarAtaque(Duracion));
                        }
                        else
                        {
                            Debug.LogError("Animator no encontrado en el primer hijo de ObjetoArmas.");
                        }
                    }
                    else
                    {
                        Debug.LogError("ObjetoArmas no tiene hijos.");
                    }
                }
                else
                {
                    Debug.LogError("ObjetoArmas es nulo.");
                }
            }

            // Registrar el tiempo del último disparo
            tiempoUltimoDisparo = Time.time;
        }
    }

    void Recargar()
    {
        // Implementar lógica para recargar el arma si es necesario
    }

    IEnumerator EsperarAtaque(float Segundos)
    {
        Atacando = true;
        yield return new WaitForSeconds(Segundos);
        Atacando = false;
    }

    float GetAnimationClipDuration(Animator animator, string clipName)
    {
        if (animator == null)
        {
            Debug.LogError("Animator es nulo.");
            return 0f;
        }

        // Obtener todos los clips del RuntimeAnimatorController
        RuntimeAnimatorController runtimeController = animator.runtimeAnimatorController;
        if (runtimeController == null)
        {
            Debug.LogError("RuntimeAnimatorController es nulo.");
            return 0f;
        }

        AnimationClip[] clips = runtimeController.animationClips;

        // Encontrar el clip con el nombre especificado
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
        if (Col != null)
        {
            Col.SetActive(true);
        }
        else
        {
            Debug.LogError("Col es nulo.");
        }
    }

    public void DesactivarColision(GameObject Col)
    {
        if (Col != null)
        {
            Col.SetActive(false);
        }
        else
        {
            Debug.LogError("Col es nulo.");
        }
    }
}
