using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_ControladorBatalla : MonoBehaviour
{
    [SerializeField] Scr_CreadorArmas Arma1;
    [SerializeField] Scr_CreadorArmas Arma2;
    [SerializeField] Scr_CreadorArmas Arma3;

    [SerializeField] TextMeshProUGUI NumeroCuenta;

    float Cuenta = 4;
    bool ComenzarCuenta = false;

    void Start()
    {

    }

    void Update()
    {
        if (ComenzarCuenta && Cuenta > 0)
        {
            Cuenta -= Time.deltaTime;
            if (Cuenta < 1)
            {
                NumeroCuenta.text = "Pelea";
            }
            else
            {
                NumeroCuenta.text = ((int)Cuenta).ToString();
            }
        }
        else
        {
            if (Cuenta <= 0)
            {
                NumeroCuenta.gameObject.SetActive(false);
                ComenzarCuenta = false;
                Cuenta = 4;
                GetComponent<Scr_ControladorArmas>().enabled = true;
                Camera.main.transform.parent.GetComponent<Scr_Movimiento>().enabled = true;
                Camera.main.transform.parent.GetComponent<Rigidbody>().useGravity = true;
                Camera.main.GetComponent<Scr_GirarCamaraBatalla>().enabled = true;
            }
        }
    }

    public void CuentaAtras()
    {
        NumeroCuenta.gameObject.SetActive(true);
        ComenzarCuenta = true;
    }
}
