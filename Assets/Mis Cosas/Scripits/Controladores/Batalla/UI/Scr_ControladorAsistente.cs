using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorAsistente : MonoBehaviour
{

    [SerializeField] Sprite ImagenDisparando;
    [SerializeField] Sprite[] ImagenesVida;
    [SerializeField] Color[] Colores;
    bool Esperando = false;
    public List<string> OrdenDeEstados = new List<string>();
    public int Balas = 0;


    void Start()
    {

    }

    void Update()
    {
        if (OrdenDeEstados.Count > 0)
        {
            GetComponent<Animator>().enabled = false;
            if (OrdenDeEstados.ToArray()[0] == "Disparando")
            {
                Debug.Log("Coloca");
                GetComponent<Image>().sprite = ImagenDisparando;
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Colores[1];
                if (Balas >= 0)
                {
                    transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Balas.ToString();
                }
                else
                {
                    Balas = 0;
                    transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Balas.ToString();
                }
                if (!Esperando)
                {
                    StartCoroutine(Esperar());
                }
            }
            else
            {
                ActualizarVida(GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>().VidaActual);
            }
        }
    }

    IEnumerator Esperar()
    {
        Esperando = true;
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().enabled = true;
        OrdenDeEstados.RemoveAt(0);
        transform.GetChild(0).gameObject.SetActive(false);
        Esperando = false;
    }

    void ActualizarVida(int Vida)
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Vida.ToString();
        transform.GetChild(0).gameObject.SetActive(true);
        if (Vida > 7)
        {
            GetComponent<Image>().sprite = ImagenesVida[0];
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Colores[0];

        }
        else
        {

            if (Vida > 5)
            {
                GetComponent<Image>().sprite = ImagenesVida[1];
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Colores[2];
            }
            else
            {

                if (Vida > 2)
                {
                    GetComponent<Image>().sprite = ImagenesVida[2];
                    transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Colores[3];

                }
                else
                {
                    GetComponent<Image>().sprite = ImagenesVida[3];
                    transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Colores[4];

                }
            }
        }
        if (!Esperando)
        {
            StartCoroutine(Esperar());

        }
    }
}