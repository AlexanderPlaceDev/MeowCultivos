using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class Scr_Estrellas : MonoBehaviour
{
    Rigidbody RB;
    SpriteRenderer Render;
    bool Activo = false;
    bool Cambiando = false;
    float Tiempo;
    float Tamaño;

    void Start()
    {
        Tamaño=Random.Range(1,5);
        transform.localScale=new Vector3(Tamaño,Tamaño,1);
        Tiempo=Random.Range(0.05f,1f);
        RB = GetComponent<Rigidbody>();
        RB.AddForce(Random.Range(-900, -400), 0, 0);
        Render = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (transform.position.x < -19)
        {
            transform.position = new Vector3(19, Random.Range(26, 45), 0.5f);
            Tamaño=Random.Range(1,5);
            transform.localScale = new Vector3(Tamaño, Tamaño, 1);
            RB.velocity = Vector3.zero;
            RB.AddForce(Random.Range(-900, -400), 0, 0);
            Tiempo=Random.Range(0.05f,1f);

        }


        if (!Cambiando)
        {
            Cambiando = true;
            StartCoroutine(Actuar());
        }
    }

    IEnumerator Actuar()
    {

        if (Render.color.a <= 0)
        {
            Activo = true;
        }
        if (Render.color.a >= 1)
        {
            Activo = false;
        }


        if (Activo)
        {
            Render.color = new Color(255, 255, 255, Render.color.a + 0.1f);
        }
        else
        {
            Render.color = new Color(255, 255, 255, Render.color.a - 0.1f);
        }

        yield return new WaitForSeconds(Tiempo);
        Cambiando = false;
    }
}
