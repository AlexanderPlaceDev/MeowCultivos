using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Scr_EstructuraConstruible : MonoBehaviour
{
    [SerializeField] string Estructura;
    [SerializeField] GameObject ObjetoAnterior;
    [SerializeField] GameObject ObjetoDespues;

    [SerializeField] Scr_CreadorObjetos[] Objetos;
    [SerializeField] int[] Cantidades;

    [SerializeField] GameObject[] ObjetosUI;
    [SerializeField] GameObject Canvas;

    Scr_Inventario Inventario;

    void Start()
    {
        Inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();
        if (PlayerPrefs.GetString(Estructura, "No") == "Si")
        {
            DesactivarCartel();
        }
        for (int i = 0; i < 4; i++)
        {
            if (Objetos.Length > i)
            {
                ObjetosUI[i].GetComponent<Image>().sprite = Objetos[i].Icono;
                ObjetosUI[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Cantidades[i].ToString();

                int i2 = 0;
                foreach (Scr_CreadorObjetos Objeto in Inventario.Objetos)
                {
                    if (Objeto == Objetos[i])
                    {
                        if (Inventario.Cantidades[i2] >= Cantidades[i])
                        {
                            ObjetosUI[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
                        }
                        else
                        {
                            ObjetosUI[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                        }
                    }
                    i2++;
                }
            }
            else
            {
                ObjetosUI[i].SetActive(false);
            }
        }

    }

    public void BotonAceptar()
    {
        BotonCerrar();
        PlayerPrefs.SetString(Estructura, "Si");
        Start();
    }

    public void BotonCerrar()
    {
        Debug.Log("Entra");
        GetComponent<Scr_ActivadorMenuEstructuraFijo>().CerrarTablero();
    }

    private void DesactivarCartel()
    {
        Canvas.SetActive(false );
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        ObjetoAnterior.SetActive(false);
        ObjetoDespues.SetActive(true);
    }
}
