using Unity.VisualScripting;
using UnityEngine;

public class Scr_ControladorCajaVentaRegalo : MonoBehaviour
{
    [SerializeField] GameObject Regalo;
    [SerializeField] GameObject Gata;
    void Start()
    {
        Gata = GameObject.Find("Gata");
        if (PlayerPrefs.GetString("CajaVentaRegalo", "No") == "Si")
            Regalo.SetActive(true);
    }


    private void Update()
    {
        if (Vector3.Distance(Gata.transform.position, Regalo.transform.position) <= 10 && Input.GetKeyDown(KeyCode.E))
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

        Regalo.SetActive(false);
    }
}
