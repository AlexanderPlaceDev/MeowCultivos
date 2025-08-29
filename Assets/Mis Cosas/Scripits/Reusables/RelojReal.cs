using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelojReal : MonoBehaviour
{
    public TextMeshProUGUI Reloj;
    public GameObject CTiempo; //checa donde esta el controlador del tiempo
    public Scr_ControladorTiempo ContolT;
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
        Reloj.text = $"{ContolT.HoraActual}: {ContolT.MinutoActual}"; 
    }
}
