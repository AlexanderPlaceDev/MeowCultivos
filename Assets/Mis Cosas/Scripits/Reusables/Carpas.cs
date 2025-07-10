using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Carpas : MonoBehaviour
{

    [SerializeField] Sprite IconoTecla;
    [SerializeField] string Letra;


    public GameObject carpaUI;
    public CambioTiempo cam;
    Transform Gata;
    public bool openUI=false;
    bool EstaEnRango = false;
    // Start is called before the first frame update
    void Start()
    {
        cam= carpaUI.GetComponent<CambioTiempo>();
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
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
            openUI = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gata" || other.name == "Gato Mesh")
        {
            EstaEnRango = true;
            Gata.GetChild(3).gameObject.SetActive(true);
            Gata.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Letra;
            Gata.GetChild(3).GetChild(0).GetComponent<Image>().sprite = IconoTecla;
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
