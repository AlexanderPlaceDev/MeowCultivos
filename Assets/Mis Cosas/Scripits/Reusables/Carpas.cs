using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Carpas : MonoBehaviour
{

    [SerializeField] Sprite IconoTecla;
    [SerializeField] string Letra;


    public Scr_ControladorTiempo ControlT;
    public GameObject carpaUI;
    public CambioTiempo cam;
    Transform Gata;
    public bool openUI = false;
    bool EstaEnRango = false;
    public int HoraDeSiesta=19;
    // Start is called before the first frame update
    void Start()
    {
        cam = carpaUI.GetComponent<CambioTiempo>();
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        ControlT = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !openUI && EstaEnRango)
        {
            Debug.Log("carpita");
            carpaUI.SetActive(true);
            cam.carpa = this;
            cam.cabTiempo();
            cam.cabRadio();
            openUI = true;
        }


        if ((PlayerPrefs.GetString("Habilidad:Despertador", "No") == "Si") && !transform.GetChild(1).gameObject.activeSelf)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            cam.Puede_Ajustar = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            if (ControlT.HoraActual > HoraDeSiesta)
            {
                //Debug.LogError(ContolT.HoraActual > HoraDeSiesta);
                EstaEnRango = true;
                Gata.GetChild(3).gameObject.SetActive(true);
                Gata.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Letra;
                Gata.GetChild(3).GetChild(0).GetComponent<Image>().sprite = IconoTecla;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = false;
            Gata.GetChild(3).gameObject.SetActive(false);
        }
    }
}
