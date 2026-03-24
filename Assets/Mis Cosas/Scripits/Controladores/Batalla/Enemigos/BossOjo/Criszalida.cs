using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Criszalida : MonoBehaviour
{
    [SerializeField] GameObject Base;
    Transform gata;
    Scr_Activador_boss actboss;
    OjoBoss boss;
    // Start is called before the first frame update
    void Start()
    {
        actboss = GameObject.Find("Activador_BOSS").GetComponent<Scr_Activador_boss>();
        boss = GameObject.Find("OjoBoss").GetComponent<OjoBoss>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void quitarvida()
    {
        int valor=Random.Range(0, 1);
        if (actboss != null) 
        {
            actboss.detectaron(valor == 0);
            boss.crizalidamenos(.25f);
        }
        else
        {
            boss.parar();
        }
        Destroy(Base);
    }
}
