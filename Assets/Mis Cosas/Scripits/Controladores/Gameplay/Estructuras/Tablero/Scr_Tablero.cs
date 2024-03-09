using PrimeTween;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Scr_Tablero : MonoBehaviour
{
    bool EstaEnRango;
    [SerializeField] float Duracion;
    [SerializeField] Sprite Icono;
    [SerializeField] Sprite Tecla;
    Transform Gata;
    float Tiempo = 0;
    public bool EstaDentro = false;
    GameObject Camara360;
    GameObject Canvas;

    void Start()
    {
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        Camara360 = GameObject.Find("Camara 360");
        Canvas = GameObject.Find("Canvas");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && EstaEnRango && !EstaDentro && Time.timeScale == 1)
        {
            EstaDentro = true;
            Camara360.SetActive(false);
            Gata.GetChild(4).gameObject.SetActive(false);
            Gata.GetComponent<Scr_Movimiento>().enabled = false;
            Gata.GetComponent<Scr_GiroGata>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);
            Tween.UIAnchoredPosition3DY(Canvas.transform.GetChild(4).GetComponent<RectTransform>(), -120, 1);
            Tween.UIAnchoredPosition3DY(Canvas.transform.GetChild(3).GetComponent<RectTransform>(), 137, 1);
            Gata.GetChild(2).GetComponent<Scr_ControladorUI>().PuedeAbrirMochila = false;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && EstaEnRango && EstaDentro && Time.timeScale == 1)
            {
                Gata.GetChild(2).GetComponent<Scr_ControladorUI>().PuedeAbrirMochila = true;
                Tiempo += 1;
                EstaDentro = false;
                Camara360.SetActive(true);
                Gata.GetChild(4).gameObject.SetActive(true);
                Gata.GetComponent<Scr_Movimiento>().enabled = true;
                Gata.GetComponent<Scr_GiroGata>().enabled = true;
                transform.GetChild(0).gameObject.SetActive(false);
                Tween.UIAnchoredPosition3DY(Canvas.transform.GetChild(4).GetComponent<RectTransform>(), 20, 1);
                Tween.UIAnchoredPosition3DY(Canvas.transform.GetChild(3).GetComponent<RectTransform>(), -20, 1);
            }

        }

        if (EstaDentro && Tiempo < Duracion)
        {
            Tiempo += Time.deltaTime;
            foreach (Material Mat in Gata.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials)
            {
                CambiarMaterial(Mat);
            }
        }

        if (!EstaDentro && Tiempo > 0)
        {
            Tiempo -= Time.deltaTime;
            foreach (Material Mat in Gata.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials)
            {
                CambiarMaterial(Mat);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = true;
            Gata.GetChild(4).gameObject.SetActive(true);
            Gata.GetChild(4).GetChild(0).GetComponent<SpriteRenderer>().sprite = Tecla;
            Gata.GetChild(4).GetChild(1).GetComponent<SpriteRenderer>().sprite = Icono;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = false;
            Gata.GetChild(4).gameObject.SetActive(false);
        }
    }

    void CambiarMaterial(Material Mat)
    {
        Mat.SetFloat("_Alpha", Mathf.Lerp(1, 0, Tiempo / Duracion));

    }
}
