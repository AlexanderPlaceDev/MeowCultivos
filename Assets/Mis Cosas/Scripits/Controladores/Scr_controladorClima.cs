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
        Debug.Log("aaa");
        ApagarClimas();
        switch (Clima)
        {
            case "Soleado":
                Debug.Log("aaaaaa");
                ApagarClimas();
                break;
            case "Nublado":
                Debug.Log("ojo");
                Prender_Neblina();
                break;
            case "Lluvioso":
                Debug.Log("sssss");
                Prender_Luvia();
                break;
            case "Vientoso":
                Debug.Log("ffff");
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
        for (int i = 0; i < Viento.Length; i++)
        {
            Viento[i].SetActive(true);
            //Debug.LogError(Viento[i].activeSelf);
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
        for (int i = 0; i < Lluvia.Length; i++)
        {
            Lluvia[i].SetActive(true);
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
