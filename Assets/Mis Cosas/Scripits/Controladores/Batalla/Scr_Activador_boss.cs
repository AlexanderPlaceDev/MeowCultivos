using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Activador_boss : MonoBehaviour
{
    public OjoBoss ojazo;
    public int intentos=3;
    public GameObject[] Vigias;
    void Start()
    {

        ojazo = GameObject.Find("OjoBoss").GetComponent<OjoBoss>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void detectaron(bool subs)
    {
        intentos--;
        if(intentos <= 0)
        {
            Fase2();
        }
        else
        {
            if (subs)
            {
                ojazo.ActivarSubditos();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.name == "Personaje" || other.CompareTag("Gata"))
        {
            Fase2();
        }
    }

    public void Fase2()
    {
        for (int i = 0; i < Vigias.Length; i++) 
        {
            Destroy(Vigias[i]);
        }
        ojazo.Despertar();
        Destroy(gameObject);
    }
}
