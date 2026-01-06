using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorCajaVentaRegalo : MonoBehaviour
{
    [SerializeField] GameObject Regalo;
    [SerializeField] Sprite IconoAccion;
    [SerializeField] GameObject Gata;

    bool uiActivo = false;
    Scr_Inventario Inventario;

    void Start()
    {
        if (PlayerPrefs.GetString("CajaVentaRegalo", "No") == "Si")
            Regalo.SetActive(true);
        Inventario = Gata.transform.GetChild(7).GetComponent<Scr_Inventario>();
    }


    private void Update()
    {
        bool dentroDelRango =
        Vector3.Distance(Gata.transform.position, Regalo.transform.position) <= 6 &&
        PlayerPrefs.GetString("CajaVentaRegalo", "No") == "Si";

        // ENTRAR AL RANGO
        if (dentroDelRango && !uiActivo)
        {
            uiActivo = true;

            var ui = Gata.transform.GetChild(3).gameObject;
            ui.SetActive(true);

            ui.transform.GetChild(0).GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = "F";

            ui.transform.GetChild(1)
                .GetComponent<Image>().sprite = IconoAccion;
        }

        // SALIR DEL RANGO
        if (!dentroDelRango && uiActivo)
        {
            uiActivo = false;
            Gata.transform.GetChild(3).gameObject.SetActive(false);
        }

        // INPUT SOLO SI ESTÁ ACTIVO
        if (uiActivo && Input.GetKeyDown(KeyCode.F))
        {
            AbrirRegalo();
        }

        if (PlayerPrefs.GetString("CajaVentaRegalo", "No") == "Si")
            Regalo.SetActive(true);

    }


    public void AbrirRegalo()
    {
        PlayerPrefs.SetString("CajaVentaRegalo", "No");
        PlayerPrefs.SetInt("CajasVendidas", 0);
        PlayerPrefs.Save();

        string Objeto = Inventario.Objetos[Random.Range(0, Inventario.Objetos.Length)].Nombre;
        int Cantidad = Random.Range(1, 6);
        Inventario.AgregarObjeto(Objeto, Cantidad, true, false);

        GetComponent<AudioSource>().Play();

        Gata.transform.GetChild(3).gameObject.SetActive(false);
        Regalo.SetActive(false);
    }
}
