using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Farol_controlador : MonoBehaviour
{
    public Transform target;         // El objeto al que se le mide la distancia (ej: el jugador)
    public float maxDistance = 10f;  // Distancia máxima permitida
    public GameObject CTiempo; //checa donde esta el controlador del tiempo
    public Scr_ControladorTiempo ContolT;
    public bool enableEmission = true;
    private bool haveActivate = false;

    void Start()
    {
        target= GameObject.Find("Gata").transform;
        CTiempo = GameObject.Find("Controlador Tiempo");
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
    }
    private void OnEnable()
    {
        target = GameObject.Find("Gata").transform;
        CTiempo = GameObject.Find("Controlador Tiempo");
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
    }
    void Update()
    {
        if (ContolT.HoraActual >= 19 || ContolT.HoraActual <= 8)
        {
            ActivarFaroles();
        }
        else
        {
            DesactivarFaroles();
            desactivarBlom();
        }
    }



    public void ActivarFaroles()
    {
        activarBlom();
        checarDistancia();
    }
    public void DesactivarFaroles()
    {
        if (haveActivate == false) return;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void checarDistancia()
    {
        if (target == null)
        {
            UnityEngine.Debug.LogWarning("No se ha asignado el target.");
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        //UnityEngine.Debug.Log(distance);
        if (distance > maxDistance)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false); // Desactiva este objeto
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void activarBlom()
    {

        if (haveActivate == true) return;
        Renderer rend = GetComponent<Renderer>();
        if (rend == null) return;

        Material[] mats = rend.materials;

        if (mats.Length < 2)
        {
            UnityEngine.Debug.LogWarning("El objeto no tiene un segundo material.");
            return;
        }

        Material mat = mats[1]; // Segundo material (índice 1)

        mat.EnableKeyword("_EMISSION");

        // Esto no es estrictamente necesario, pero asegura que se guarden los cambios
        rend.materials = mats;
        haveActivate = true;
    }

    public void desactivarBlom()
    {
        if (haveActivate == false) return;
        Renderer rend = GetComponent<Renderer>();
        if (rend == null) return;

        Material[] mats = rend.materials;

        if (mats.Length < 2)
        {
            UnityEngine.Debug.LogWarning("El objeto no tiene un segundo material.");
            return;
        }

        Material mat = mats[1]; // Segundo material (índice 1)

        mat.DisableKeyword("_EMISSION");

        // Esto no es estrictamente necesario, pero asegura que se guarden los cambios
        rend.materials = mats;
        haveActivate = false;
    }
}
