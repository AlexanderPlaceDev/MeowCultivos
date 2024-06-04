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
    [SerializeField] TextMeshProUGUI TextoMinutos;
    int Minutos = 2;
    [SerializeField] TextMeshProUGUI TextoSegundos;
    float Segundos = 60;
    bool ComenzarCuenta = false;
    bool ComenzoTiempo = false;

    void Start()
    {

    }

    void Update()
    {
        Comienzo();
        Tiempo();
    }

    public void CuentaAtras()
    {
        NumeroCuenta.gameObject.SetActive(true);
        ComenzarCuenta = true;
    }

    private void Comienzo()
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
                ComenzoTiempo = true;
            }
        }
    }

    private void Tiempo()
    {
        if (ComenzoTiempo)
        {
            if (Minutos > 0)
            {
                if (Segundos >= 0)
                {
                    if (Segundos > 9)
                    {
                        TextoSegundos.text = ((int)Segundos).ToString();
                    }
                    else
                    {
                        TextoSegundos.text = "0"+((int)Segundos).ToString();
                    }
                    Segundos -= Time.deltaTime;

                }
                else
                {
                    Segundos = 60;
                    Minutos--;
                    TextoMinutos.text = Minutos.ToString();
                }
            }
            else
            {
                ComenzoTiempo = false;
                GetComponent<Scr_ControladorArmas>().enabled = true;
                Camera.main.transform.parent.GetComponent<Scr_Movimiento>().enabled = true;
                Camera.main.GetComponent<Scr_GirarCamaraBatalla>().enabled = true;
            }


        }
    }
}
