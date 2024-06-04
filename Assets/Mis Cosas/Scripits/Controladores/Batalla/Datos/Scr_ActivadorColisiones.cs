using UnityEngine;

public class Scr_ActivadorColisiones : MonoBehaviour
{
    [SerializeField] GameObject Colision;
    public void ActivarColision()
    {
        Colision.SetActive(true);
    }

    public void DesactivarColision()
    {
        Colision.SetActive(false);
    }
}
