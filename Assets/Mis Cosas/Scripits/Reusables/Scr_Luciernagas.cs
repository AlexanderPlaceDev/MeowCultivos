using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Luciernagas : MonoBehaviour
{
    public int HoraDeSiestaInicio = 19;
    public int HoraDeSiestaFin = 5;
    bool activados=true;
    public Scr_ControladorTiempo ControlT;
    // Start is called before the first frame update
    void Start()
    {
        ControlT = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        cehcarLuciernagas();
    }

    public bool checarHora()
    {
        // Calculamos si estamos dentro del horario
        if (HoraDeSiestaInicio < HoraDeSiestaFin)
        {
            return ControlT.HoraActual >= HoraDeSiestaInicio && ControlT.HoraActual < HoraDeSiestaFin;
        }
        else
        {
            return ControlT.HoraActual >= HoraDeSiestaInicio || ControlT.HoraActual < HoraDeSiestaFin;
        }
    }

    public void cehcarLuciernagas()
    {
        if (checarHora() && !activados)
        {
            activar();
        }
        else if (!checarHora() && activados)
        {
            Desactivar();
        }
    }
    public void activar()
    {
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(true);
        }
        activados = true;
    }
    public void Desactivar()
    {
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(false);
        }
        activados = false;
    }
}
