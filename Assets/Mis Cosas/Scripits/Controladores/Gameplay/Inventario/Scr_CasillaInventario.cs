using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Scr_CasillaInventario : MonoBehaviour
{
    [SerializeField] Material Padre;
    [SerializeField] public float Numero;
    [SerializeField] Sprite CasillaVacia;

    Scr_ControladorInventario Inventario;
    public List<Image> CasillasHermanas = new List<Image>();
    public bool[] FormaConHermanas;

    public bool PuedeAgarrar;

    private void Start()
    {
        if (FormaConHermanas.Length == 0)
        {
            FormaConHermanas = new bool[18];
        }
        Inventario = GameObject.Find("Gata").transform.GetChild(3).GetComponent<Scr_ControladorInventario>();
    }

    void Update()
    {
        GetComponent<Image>().color = Padre.color;

        //Encender Opciones
        if (Vector3.Distance(transform.position, Input.mousePosition) < 35 && GetComponent<Image>().sprite != CasillaVacia && PuedeAgarrar)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                transform.GetChild(1).gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && GetComponent<Image>().sprite != CasillaVacia)
            {
                if(CasillasHermanas.ToArray().Length>0)
                {
                    Inventario.AgarrarObjetoDelInventario(CasillasHermanas.ToArray(), false, false);
                }
            }

        }
        else
        {
            //Si da click lejos se apaga
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        //Si se aleja del menu apaga
        if (Vector3.Distance(transform.GetChild(1).position, Input.mousePosition) > 150 || Inventario.ObjetoEnMano.Nombre != "")
        {
            transform.GetChild(1).gameObject.SetActive(false);

        }
    }

    public void EncenderAgarrarOpciones()
    {
        transform.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(false);
    }

    public void ApagarAgarrarOpciones()
    {
        transform.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(false);
        transform.GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(false);
    }

    public void EncenderEliminarOpciones()
    {
        transform.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(false);
        transform.GetChild(1).GetChild(0).GetChild(3).gameObject.SetActive(false);
        transform.GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(true);
    }

    public void ApagarEliminarOpciones()
    {
        transform.GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(false);
    }

    public void ReiniciarCasilla()
    {
        PuedeAgarrar = false;
        GetComponent<Image>().sprite = CasillaVacia;
        CasillasHermanas = new List<Image>();
        FormaConHermanas = new bool[18];
    }

    public void AgarrarMitad()
    {
        Inventario.AgarrarObjetoDelInventario(CasillasHermanas.ToArray(), true, false);
    }
    public void AgarrarUno()
    {
        Inventario.AgarrarObjetoDelInventario(CasillasHermanas.ToArray(), false, true);
    }

    public void Eliminar()
    {

        transform.GetChild(1).gameObject.SetActive(false);
        Inventario.EliminarObjeto(CasillasHermanas.ToArray());
    }
}
