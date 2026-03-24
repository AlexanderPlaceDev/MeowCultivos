using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Scr_Enemigo;

public class GolpesOjoBOSS : MonoBehaviour
{
    OjoBoss ojo;
    public Transform[] puntoGolpe;
    public Scr_Enemigo.TipoEfecfto efecfto;
    // Start is called before the first frame update
    void Start()
    {
        ojo = GetComponentInParent<OjoBoss>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void golpe1()
    {
        ojo.Ataque(puntoGolpe[0], ojo.Efecto.ToString());
    }
    public void golpe2()
    {
        ojo.Ataque(puntoGolpe[1], efecfto.ToString());
    }


    public void AparecerSubs()
    {
        ojo.ActivarSubditos();
    }
}
