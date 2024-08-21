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
    [SerializeField] bool mostrarRaycast = false;  // Nueva variable para activar/desactivar visualización del Raycast

    int ArmaActual = 0;
    bool Atacando = false;
    public int CantBalasActual = 0;

    [SerializeField] float cadenciaDisparo = 0.5f;
    private float tiempoUltimoDisparo;

    GameObject Gata;
    void Start()
    {
        Gata = GameObject.Find("Personaje");
        CantBalasActual = TodasLasArmas[ArmaActual].Capacidad;
        tiempoUltimoDisparo = -cadenciaDisparo;

        VerificarReferencias();
    }

    void Update()
    {
        // Verificar colisión con el Raycast
        VerificarEnemigoConRaycast();

        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= tiempoUltimoDisparo + cadenciaDisparo && PuedeDisparar())
        {
            Disparar();
        }
    }

    void VerificarReferencias()
    {
        if (TodasLasArmas == null || TodasLasArmas.Length == 0)
        {
            Debug.LogError("TodasLasArmas no está asignado o está vacío en el Inspector.");
        }
        if (ObjetoArmas == null)
        {
            Debug.LogError("ObjetoArmas no está asignado en el Inspector.");
        }
    }

    bool PuedeDisparar()
    {
        return CantBalasActual > 0 || TodasLasArmas[ArmaActual].Tipo == "Cuerpo a Cuerpo";
    }

    void Disparar()
    {
        if (!Atacando && ObjetoArmas != null)
        {
            Transform ArmaAct = ObjetoArmas.transform.GetChild(ArmaActual);
            if (ArmaAct != null)
            {
                Animator Anim = ArmaAct.GetComponent<Animator>();
                if (Anim != null)
                {
                    EjecutarAtaque(Anim);
                }
                else
                {
                    Debug.LogError("Animator no encontrado en el hijo de ObjetoArmas.");
                }
            }
            else
            {
                Debug.LogError("ObjetoArmas no tiene hijos.");
            }

            tiempoUltimoDisparo = Time.time;
        }
    }

    void EjecutarAtaque(Animator Anim)
    {
        Debug.Log("Disparando arma...");

        if (CantBalasActual > 0 && TodasLasArmas[ArmaActual].Tipo != "Cuerpo a Cuerpo")
        {
            CantBalasActual--;
        }

        Anim.Play("Golpear");
        float Duracion = GetAnimationClipDuration(Anim, "Brazos_Golpe");
        StartCoroutine(EsperarAtaque(Duracion));

        if (TodasLasArmas[ArmaActual].Tipo != "Cuerpo a Cuerpo")
        {
            ActualizarAsistente();
        }
    }

    void ActualizarAsistente()
    {
        var asistente = GameObject.Find("Asistente").gameObject.GetComponent<Scr_ControladorAsistente>();
        if (asistente != null && !asistente.OrdenDeEstados.Contains("Disparando"))
        {
            asistente.Balas = CantBalasActual;
            asistente.OrdenDeEstados.Add("Disparando");
        }
    }

    void VerificarEnemigoConRaycast()
    {

        RaycastHit hit;
        Vector3 origen = Gata.transform.position + origenRaycast;
        Vector3 direccion = Gata.transform.GetChild(0).forward;

        // Dibujar Raycast en el editor si está habilitado
        if (mostrarRaycast)
        {
            Debug.DrawRay(origen, direccion * LongitudRaycast, Color.red);
        }

        if (Physics.Raycast(origen, direccion, out hit, LongitudRaycast))
        {
            if (hit.collider.CompareTag("Enemigo"))
            {
                Mira.sprite = Mirillas[1];
                Mira.color = ColoresMirillas[1];
                return;
            }
        }

        Mira.color = ColoresMirillas[0];
        Mira.sprite = Mirillas[0];
    }

    void CambiarArma()
    {
        // Implementar lógica para cambiar de arma si es necesario
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

        RuntimeAnimatorController runtimeController = animator.runtimeAnimatorController;
        if (runtimeController == null)
        {
            Debug.LogError("RuntimeAnimatorController es nulo.");
            return 0f;
        }

        foreach (AnimationClip clip in runtimeController.animationClips)
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
