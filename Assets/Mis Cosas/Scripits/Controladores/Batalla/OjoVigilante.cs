using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OjoVigilante : MonoBehaviour
{
    public float velocidadRotacion = 30f;
    public float anguloMaximo = 45f;

    public Transform jugador;
    [SerializeField] GameObject Detector;
    private float anguloActual = 0f;
    private int direccion = 1;

    bool dormido = false;

    public Animator Anim;
    private void Start()
    {
        jugador= GameObject.Find("Personaje").transform;
    }
    void Update()
    {
        Rotar();
    }

    void Rotar()
    {
        if (dormido) return;
        float rotacion = velocidadRotacion * Time.deltaTime * direccion;
        transform.Rotate(0, 0, rotacion);

        anguloActual += rotacion;

        if (Mathf.Abs(anguloActual) >= anguloMaximo)
        {
            direccion *= -1;
        }
    }

    public void Dormir()
    {
        if (Anim!=null)
        {
            Anim.SetBool("EstaDormido",true);
        }
        StartCoroutine(descansar(20f));
    }

    IEnumerator descansar(float duracion)
    {
        dormido = true;
        Detector.SetActive(false);
        yield return new WaitForSeconds(duracion);
        dormido = false;
        Detector.SetActive(true);
        if (Anim != null)
        {
            Anim.SetBool("EstaDormido", false);
        }
    }

    
}
