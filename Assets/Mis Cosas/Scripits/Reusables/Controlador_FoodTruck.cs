using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Controlador_FoodTruck : MonoBehaviour
{
    public GameObject FoodTruck;
    public Tienda_3D tienda;
    public string[] DiasActivo;
    public int horaAparecer;
    public int MinAparecer;
    public GameObject CTiempo;
    public Scr_ControladorTiempo ContolT;

    public AudioSource source;
    public AudioClip sonido;

    private bool yaAparecioHoy = false;

    void Start()
    {
        //source = gameObject.AddComponent<AudioSource>();
        FoodTruck.SetActive(false);
        //tienda = FoodTruck.GetComponent<Tienda_3D>();

        //aplica el volumen 
        int volumen_general = PlayerPrefs.GetInt("Volumen", 50);
        int volumen_ambiental = PlayerPrefs.GetInt("Volumen_Ambiente", 20);
        float volumen = (volumen_general * volumen_ambiental) / 100;
        source.volume = volumen;
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
    }

    void Update()
    {
        string DiaActual = ContolT.DiaActual;

        // Si hoy es un día activo
        if (EsDiaActivo(DiaActual))
        {
            // Si es la hora y minuto exactos, y aún no ha aparecido
            if (!yaAparecioHoy && ContolT.HoraActual == horaAparecer && ContolT.MinutoActual == MinAparecer)
            {
                ActivarFoodTruck();
                yaAparecioHoy = true;
            }
        }
        else
        {
            // Si no es día activo, aseguramos que esté apagado
            if (FoodTruck.activeSelf)
                FoodTruck.SetActive(false);

            yaAparecioHoy = false;
        }

        // Reiniciar bandera al cambiar de día
        if (!EsDiaActivo(DiaActual))
        {
            yaAparecioHoy = false;
        }
    }

    bool EsDiaActivo(string dia)
    {
        foreach (string d in DiasActivo)
        {
            if (dia == d)
                return true;
        }
        return false;
    }

    void ActivarFoodTruck()
    {
        Play_sonido();
        FoodTruck.SetActive(true);
        tienda.nevosObjetos();  // <- asumo que es "nuevosObjetos()"?
    }

    void Play_sonido()
    {
        source.clip = sonido;
        source.Play();
    }
}
