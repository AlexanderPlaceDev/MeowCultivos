using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ObjetosAgregados : MonoBehaviour
{
    public List<Scr_CreadorObjetos> Lista = new List<Scr_CreadorObjetos>();
    public List<int> Cantidades = new List<int>();
    [SerializeField] GameObject[] Iconos;
    public float[] Tiempo = { 2, 2, 2, 2 };
    int xptotal = 0;
    [SerializeField] string HabilidadXP;

    // Bandera para evitar agregar objetos repetidamente
    private bool objetosAgregados = false;

    void Start()
    {
    }

    void Update()
    {
        //Debug.LogWarning("aparezo" + gameObject.name);
        AgregarObjetosBatalla();

        if (Lista.Count > 0)
        {
            Debug.Log("Entra2");
            if (!GameObject.Find("Canvas XP").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desaparecer"))
            {
                GameObject.Find("Canvas XP").GetComponent<Animator>().Play("Desaparecer");
            }
            Scr_ControladorMisiones mis = GameObject.Find("ControladorMisiones").GetComponent<Scr_ControladorMisiones>();
            mis.revisarMisionPrincipal();
            mis.RevisarTodasLasMisionesSecundarias();

            int ObjetoActual = 0;
            xptotal = 0;
            foreach (Scr_CreadorObjetos Objeto in Lista)
            {
                if (ObjetoActual == 4 || Lista[ObjetoActual] == null)
                {
                    Debug.Log("Objeto numero 4 rompe ciclo");
                    break;
                }
                else
                {
                    xptotal += Objeto.XPRecolecta;
                    Iconos[ObjetoActual].GetComponent<Image>().sprite = Lista[ObjetoActual].Icono;
                    Iconos[ObjetoActual].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + Cantidades[ObjetoActual].ToString();
                }
                ObjetoActual++;
            }

            if (Iconos[0].GetComponent<Image>().sprite != null && GameObject.Find("Canvas XP").transform.GetChild(0).GetComponent<TextMeshProUGUI>().color.a == 0)
            {
                //Debug.LogWarning("doy xp");
                if (PlayerPrefs.GetString("Habilidad:" + HabilidadXP, "No") == "Si")
                {
                    xptotal = xptotal * 2;
                }
                PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual") + xptotal);
                if (PlayerPrefs.GetInt("XPActual", 0) >= PlayerPrefs.GetInt("XPSiguiente", 10))
                {
                    PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual", 0) - PlayerPrefs.GetInt("XPSiguiente", 10));
                    PlayerPrefs.SetInt("Nivel", PlayerPrefs.GetInt("Nivel", 0) + 1);
                    PlayerPrefs.SetInt("XPSiguiente", PlayerPrefs.GetInt("XPSiguiente", 10) * 2);
                    PlayerPrefs.SetInt("PuntosDeHabilidad", PlayerPrefs.GetInt("PuntosDeHabilidad", 0) + 3);
                    GameObject.Find("Canvas XP").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "LV.+1";
                }
                else
                {
                    GameObject.Find("Canvas XP").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "XP + " + xptotal;
                }

            }
        }

        int i = 0;
        foreach (GameObject Icono in Iconos)
        {
            if (Icono.GetComponent<Image>().sprite == null) { break; }

            Tiempo[i] -= Time.deltaTime;
            Icono.GetComponent<Image>().color = new Color(1, 1, 1, Tiempo[i]);
            Icono.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, Tiempo[i]);
            if (Tiempo[i] <= 0 && Lista.Count > 0)
            {
                Icono.GetComponent<Image>().sprite = null;
                Lista.RemoveAt(0);
                Cantidades.RemoveAt(0);
                Tiempo[i] = 2;




            }
            i++;
        }
    }

    void AgregarObjetosBatalla()
    {
        Scr_DatosSingletonBatalla Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();

        if (!objetosAgregados && Singleton.CantidadesRecompensa.Count > 0 && Singleton.CantidadesRecompensa[0] != 0)
        {
            for (int i = 0; i < Mathf.Min(Singleton.ObjetosRecompensa.Count, Singleton.CantidadesRecompensa.Count); i++)
            {
                Scr_CreadorObjetos Objeto = Singleton.ObjetosRecompensa[i];
                Lista.Add(Objeto);
                Cantidades.Add(Singleton.CantidadesRecompensa[i]);
            }
            objetosAgregados = true;
        }
        else
        {
            // Verifica si la lista no está vacía antes de acceder al primer índice
            if (Singleton.CantidadesRecompensa.Count > 0 && Singleton.CantidadesRecompensa[0] > 0)
            {
                Singleton.CantidadesRecompensa.Clear();
                Singleton.ObjetosRecompensa.Clear();
            }
        }

        
    }


}
