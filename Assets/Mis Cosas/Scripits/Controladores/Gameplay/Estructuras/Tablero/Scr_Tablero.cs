using PrimeTween;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Scr_Tablero : MonoBehaviour
{
    bool EstaEnRango;
    [SerializeField] float Duracion;
    [SerializeField] Sprite Icono;
    [SerializeField] string Letra;
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
        if (Input.GetKeyDown(KeyCode.E) && EstaEnRango && !EstaDentro)
        {
            EstaDentro = true;
            Camara360.SetActive(false);
            Gata.GetChild(2).gameObject.SetActive(false);
            Gata.GetComponent<Scr_Movimiento>().enabled = false;
            Gata.GetComponent<Scr_GiroGata>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);
            Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetComponent<RectTransform>(), -200, 1);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && EstaEnRango && EstaDentro)
            {
                CerrarTablero();
            }

        }

        if (EstaDentro && Tiempo < Duracion)
        {
            Gata.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Tiempo += Time.deltaTime;
            foreach (Material Mat in Gata.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials)
            {
                CambiarMaterial(Mat);
            }
        }

        if (!EstaDentro)
        {
            if (Tiempo > 0)
            {
                Tiempo -= Time.deltaTime;
                foreach (Material Mat in Gata.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials)
                {
                    CambiarMaterial(Mat);
                }
                Gata.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = true;
            Gata.GetChild(2).gameObject.SetActive(true);
            Gata.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text=Letra;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = false;
            Gata.GetChild(2).gameObject.SetActive(false);
        }
    }

    void CambiarMaterial(Material Mat)
    {
        Mat.SetFloat("_Alpha", Mathf.Lerp(1, 0, Tiempo / Duracion));

    }

    public void CerrarTablero()
    {
        Tiempo += 1;
        EstaDentro = false;
        Camara360.SetActive(true);
        Gata.GetChild(2).gameObject.SetActive(true);
        Gata.GetComponent<Scr_Movimiento>().enabled = true;
        Gata.GetComponent<Scr_GiroGata>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Scr_MenuTablero>().Botones[0].GetComponent<Image>().color = GetComponent<Scr_MenuTablero>().ColoresBotones[0];
        GetComponent<Scr_MenuTablero>().Botones[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = GetComponent<Scr_MenuTablero>().ColoresBotones[1];
        Tween.UIAnchoredPosition3DX(Canvas.transform.GetChild(2).GetComponent<RectTransform>(), 35, 1);
    }
}
