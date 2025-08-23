using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falores_controllador : MonoBehaviour
{
    public GameObject[] Faroles;
    public GameObject CTiempo; //checa donde esta el controlador del tiempo
    public Scr_ControladorTiempo ContolT; //checa el tiempo y el dia

    private bool haveActivate=false;
    // Start is called before the first frame update
    void Start()
    {
        CTiempo = GameObject.Find("Controlador Tiempo");
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
    }
    private void OnEnable()
    {
        CTiempo = GameObject.Find("Controlador Tiempo");
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
    }
        
    // Update is called once per frame
    void Update()
    {
        
        
        if (ContolT.HoraActual >= 19 || ContolT.HoraActual <=8)
        {
            ActivarFaroles();
        }
        else
        {
            DesactivarFaroles();
        }
    }


    public void ActivarFaroles()
    {
        if (haveActivate == true) return;
        for(int i = 0;i < Faroles.Length; i++)
        {
            Faroles[i].transform.GetChild(0).gameObject.SetActive(true);
        }
        haveActivate = true;
    }
    public void DesactivarFaroles()
    {
        if (haveActivate == false) return;
        for (int i = 0; i < Faroles.Length; i++)
        {
            Faroles[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        haveActivate = false;
    }
}
