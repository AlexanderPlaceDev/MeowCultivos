using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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

    List<GameObject> Enemigos = new List<GameObject>();

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

        List<GameObject> Spawners = new List<GameObject>();

        foreach (Transform Objeto in GameObject.Find("Mapa").transform.GetChild(0).GetComponentInChildren<Transform>())
        {
            if (Objeto.name.Contains("Spawner"))
            {
                Spawners.Add(Objeto.gameObject);
            }
        }

        Scr_DatosSingletonBatalla DatosSingleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();

        for (int i = 0; i < DatosSingleton.CantidadDeEnemigos; i++)
        {
            int Posicion = Random.Range(0, Spawners.Count);

            GameObject Enemigo = Instantiate(DatosSingleton.Enemigo, Spawners.ToArray()[Posicion].transform.position, Quaternion.identity, null);
            Enemigos.Add(Enemigo);
        }
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

                foreach (GameObject Enemigo in Enemigos)
                {
                    Enemigo.GetComponent<NavMeshAgent>().enabled = true;
                    Enemigo.GetComponent<BoxCollider>().enabled = true;
                }


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
                        TextoSegundos.text = "0" + ((int)Segundos).ToString();
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
