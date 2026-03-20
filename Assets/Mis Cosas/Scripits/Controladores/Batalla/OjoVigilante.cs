using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OjoVigilante : MonoBehaviour
{
    public float velocidadRotacion = 30f;
    public float anguloMaximo = 45f;

    public Transform jugador;
    public float rangoDeteccion = 10f;
    public float anguloVision = 60f;

    private float anguloActual = 0f;
    private int direccion = 1;

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
        float rotacion = velocidadRotacion * Time.deltaTime * direccion;
        transform.Rotate(0, 0, rotacion);

        anguloActual += rotacion;

        if (Mathf.Abs(anguloActual) >= anguloMaximo)
        {
            direccion *= -1;
        }
    }



    
}
