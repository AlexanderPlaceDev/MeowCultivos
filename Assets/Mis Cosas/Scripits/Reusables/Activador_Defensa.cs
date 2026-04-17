using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Activador_Defensa : MonoBehaviour
{
    [SerializeField] private Sprite icono2;
    Scr_ActivadorMenuEstructuraFijo ActivadorMenu;
    PlayerInput playerInput;
    private InputAction Recolectar;

    private Scr_CambiadorBatalla batalla;
    Scr_ControladorSembradioUI SembradioUI;
    Transform Gata;
    InputIconProvider IconProvider;

    private Sprite iconoActualRecolectar = null;
    private string textoActualRecolectar = "";

    Scr_ControladorMisiones Mis;
    //batalla.Fruta = TipoFruta.Nombre;
    //batalla.Item = TipoFruta.Nombre;
    // Start is called before the first frame update
    void Start()
    {

        Gata = GameObject.Find("Gata").GetComponent<Transform>();
        batalla = GetComponent<Scr_CambiadorBatalla>();
        SembradioUI = GetComponent<Scr_ControladorSembradioUI>();

        ActivadorMenu = GetComponent<Scr_ActivadorMenuEstructuraFijo>();

        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        Recolectar = playerInput.actions["Interactuar"];

        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();

        Mis = GameObject.Find("ControladorMisiones").GetComponent<Scr_ControladorMisiones>();

    }

    // Update is called once per frame
    void Update()
    {
        if (ActivadorMenu.EstaEnRango && Mis.HayMisionRecolectar())
        {
            if (!Gata.GetChild(3).GetChild(3).gameObject.activeSelf)
            {
                Gata.GetChild(3).GetChild(2).gameObject.SetActive(true);
                Gata.GetChild(3).GetChild(3).gameObject.SetActive(true);

                Gata.GetChild(3).GetChild(3).GetComponent<Image>().sprite = icono2;

                Gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(1, 0, 0);
                Gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(3, 0, 0);
            }
            IconProvider.ActualizarIconoUI(Recolectar, Gata.GetChild(3).GetChild(3), ref iconoActualRecolectar, ref textoActualRecolectar, true);
            if (Recolectar.IsPressed() && !batalla.escenaCargada && SembradioUI.SemillaPlantada != null)
            {
                batalla.Fruta = SembradioUI.SemillaPlantada.Nombre;
                batalla.Item = SembradioUI.SemillaPlantada.Nombre;
                Debug.Log("aaasss");
                batalla.Iniciar(gameObject);
            }

        }
        else if (!ActivadorMenu.EstaEnRango && Mis.HayMisionRecolectar())
        {
            if (!Gata.GetChild(3).GetChild(3).gameObject.activeSelf)
            {
                if (Mis != null)
                {
                    if (Mis.HayMisionRecolectar())
                    {
                        Gata.GetChild(3).GetChild(2).gameObject.SetActive(false);
                        Gata.GetChild(3).GetChild(3).gameObject.SetActive(false);
                    }
                }

                Gata.GetChild(3).GetChild(0).transform.localPosition = new Vector3(-1, 0, 0);
                Gata.GetChild(3).GetChild(1).transform.localPosition = new Vector3(1, 0, 0);
            }
        }
    }
}
