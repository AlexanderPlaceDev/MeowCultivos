using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_controladorClima : MonoBehaviour
{
    public GameObject[] Viento;
    public GameObject[] Lluvia;
    public GameObject[] Neblina;
    public GameObject[] NeblinaRoja;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*Soleado,
        Nublado,
        Lluvioso,
        Vientoso,
        */
    public void Activar_Clima(string Clima)
    {
        ApagarClimas();
        switch (Clima)
        {
            case "Soleado":
                ApagarClimas();
                break;
            case "Nublado":
                Prender_Neblina();
                break;
            case "Lluvioso":
                Prender_Luvia();
                break;
            case "Vientoso":
                Prender_Viento();
                break;
            case "LunaRoja":
                Prender_NeblinaRoja();
                break;
        }
    }
    public void ApagarClimas()
    {
        Apagar_Luvia();
        Apagar_Neblina();
        Apagar_Viento();
    }
    public void Prender_Viento()
    {
        float fuerza= Random.Range(100, 300);
        for (int i = 0; i < Viento.Length; i++)
        {
            Viento[i].SetActive(true);
            
            Viento vient = Viento[i].GetComponent<Viento>();
            if (vient != null)
            {
                vient.Intensidad(fuerza);
            }
        }
    }
    public void Apagar_Viento()
    {
        for(int i = 0; i < Viento.Length; i++)
        {
            Viento[i].SetActive(false);
        }
    }
    public void Prender_Neblina()
    {
        for (int i = 0; i < Neblina.Length; i++)
        {
            Neblina[i].SetActive(true);
            //Debug.LogError(Neblina[i].activeSelf);
        }
    }
    public void Apagar_Neblina()
    {
        for (int i = 0; i < Neblina.Length; i++)
        {
            Neblina[i].SetActive(false);
        }
    }
    public void Prender_Luvia()
    {
        float Fuerza = Random.Range(100f, 300f);
        float friccion = CalcularFriccion(Fuerza);
        for (int i = 0; i < Lluvia.Length; i++)
        {
            Lluvia[i].SetActive(true);
            Lluvia luv = Lluvia[i].GetComponent<Lluvia>();
            if (luv != null)
            {
                luv.Intensidad(Fuerza, friccion);
            }
            //Debug.LogError(Lluvia[i].activeSelf);
        }
    }
    public void Apagar_Luvia()
    {
        for (int i = 0; i < Lluvia.Length; i++)
        {
            Lluvia[i].SetActive(false);
        }
    }
    float CalcularFriccion(float fuerza)
    {
        // Normalizamos la fuerza entre 100 y 300
        float t = Mathf.InverseLerp(100f, 300f, fuerza);

        // Fricción entre 0.1 y 0.5 según t
        return Mathf.Lerp(0.1f, 0.5f, t);
    }
    public void Prender_NeblinaRoja()
    {
        for (int i = 0; i < NeblinaRoja.Length; i++)
        {
            NeblinaRoja[i].SetActive(true);
            //Debug.LogError(NeblinaRoja[i].activeSelf);
        }
    }
    public void Apagar_NeblinaRoja()
    {
        for (int i = 0; i < NeblinaRoja.Length; i++)
        {
            NeblinaRoja[i].SetActive(false);
        }
    }
}
