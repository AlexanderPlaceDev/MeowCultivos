using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escudo : MonoBehaviour
{
    public float EscudoSalud = 50;
    public GameObject Controlador; 
    public Scr_Enemigo enemy;
    Scr_ControladorArmas arma;
    // Start is called before the first frame update
    void Start()
    {
        arma = GameObject.Find("Controlador").GetComponent<Scr_ControladorArmas>();
        Controlador = GameObject.Find("Controlador"); 
        if (enemy == null)
            enemy = GetComponentInParent<Scr_Enemigo>();

        enemy.FueBloqueado = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
         
        if (other.gameObject.tag == "Golpe")
        {
            enemy.FueBloqueado = true; 
            int Daño = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño;
            quitar_Restiencia(Daño);
        }
        else if (other.gameObject.tag == "Bala")
        {
            enemy.FueBloqueado = true;
            enemy.BloquearGolpe();
            int Daño = GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].Daño; 
            quitar_Restiencia(Daño);
        }

    }

    public void quitar_Restiencia(float daño)
    {
        EscudoSalud -= daño;
        if (EscudoSalud < 0)
        {
            enemy.FueBloqueado = false;
            gameObject.SetActive(false);
        }
    }
}
