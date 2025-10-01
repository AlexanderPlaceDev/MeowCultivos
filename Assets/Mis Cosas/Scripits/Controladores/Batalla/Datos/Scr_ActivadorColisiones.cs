using Unity.VisualScripting;
using UnityEngine;

public class Scr_ActivadorColisiones : MonoBehaviour
{
    [SerializeField] GameObject Colision1;
    [SerializeField] GameObject Colision2;


    private Scr_ControladorArmas Controlador;
    void Start()
    {
        Controlador = GameObject.Find("Controlador").GetComponent<Scr_ControladorArmas>();
    }

    public void ActivarGolpeDistancia()
    {
        Controlador.Golpea();
    }
    public void ActivarColision1()
    {
        Colision1.SetActive(true);
    }

    public void DesactivarColision1()
    {
        Colision1.SetActive(false);
    }

    public void ActivarColision2()
    {
        Colision2.SetActive(true);
    }

    public void DesactivarColision2()
    {
        Colision2.SetActive(false);
    }
}
