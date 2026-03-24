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
        StartCoroutine(descansar(20f));
    }

    IEnumerator descansar(float duracion)
    {
        dormido = true;
        Detector.SetActive(false);
        yield return new WaitForSeconds(duracion);
        dormido = false;
        Detector.SetActive(true);
    }

    
}
