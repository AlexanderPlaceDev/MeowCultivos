using UnityEngine;

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
        FoodTruck.SetActive(false);
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
    }

    void Update()
    {
        string DiaActual = ContolT.DiaActual;

        if (EsDiaActivo(DiaActual))
        {
            if (!yaAparecioHoy && ContolT.HoraActual == horaAparecer && ContolT.MinutoActual == MinAparecer)
            {
                ActivarFoodTruck();
                yaAparecioHoy = true;
            }
        }
        else
        {
            if (FoodTruck.activeSelf)
                FoodTruck.SetActive(false);

            yaAparecioHoy = false;
        }
    }

    bool EsDiaActivo(string dia)
    {
        foreach (string d in DiasActivo)
            if (dia == d)
                return true;

        return false;
    }

    void ActivarFoodTruck()
    {
        Play_sonido();
        FoodTruck.SetActive(true);
        tienda.GenerarObjetos();
    }

    void Play_sonido()
    {
        source.clip = sonido;
        source.Play();
    }
}
