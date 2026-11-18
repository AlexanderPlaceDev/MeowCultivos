using System.Linq;
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

    [SerializeField] GameObject CanvasDemo;

    [SerializeField] bool QuitaMision;
    [SerializeField] Scr_CreadorMisiones Mision;
    Scr_Inventario Inventario;

    [SerializeField] float TiempoActualizacion = 0.2f;
    private float Temporizador;
    ChecarInput Checar_input;
    void Start()
    {
        Inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();

        if (PlayerPrefs.GetString(Estructura, "No") == "Si")
        {
            DesactivarCartel();
        }

        ActualizarMaterialesUI();
    }

    void Update()
    {
        Temporizador += Time.deltaTime;

        if (Temporizador >= TiempoActualizacion)
        {
            ActualizarMaterialesUI();
            Temporizador = 0;
        }
    }

    public void BotonAceptar()
    {
        if (!TieneTodosLosMateriales())
        {
            return;
        }

        ConsumirMateriales();

        PlayerPrefs.SetString(Estructura, "Si");

        BotonCerrar();

        if (QuitaMision) { GameObject.Find("Gata").transform.GetChild(4).GetComponent<Scr_ControladorMisiones>().ElimiminarMisionDesdeSignal(Mision); }

        CrearUIdemo();
        Start();
    }


    public void BotonCerrar()
    {
        Debug.Log("Entra");
        GetComponent<Scr_ActivadorMenuEstructuraFijo>().CerrarTablero();
    }

    void CrearUIdemo()
    {

        Checar_input = GameObject.Find("Singleton").GetComponent<ChecarInput>();
        Checar_input.CammbiarAction_UI();
        Instantiate(CanvasDemo);
    }
    private void DesactivarCartel()
    {
        Canvas.SetActive(false);
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        ObjetoAnterior.SetActive(false);
        ObjetoDespues.SetActive(true);
    }

    private bool TieneTodosLosMateriales()
    {
        for (int i = 0; i < Objetos.Length; i++)
        {
            bool encontrado = false;

            for (int j = 0; j < Inventario.Objetos.Count(); j++)
            {
                if (Inventario.Objetos[j] == Objetos[i])
                {
                    encontrado = true;

                    if (Inventario.Cantidades[j] < Cantidades[i])
                        return false;

                    break;
                }
            }

            if (!encontrado)
                return false;
        }

        return true;
    }

    private void ConsumirMateriales()
    {
        for (int i = 0; i < Objetos.Length; i++)
        {
            for (int j = 0; j < Inventario.Objetos.Count(); j++)
            {
                if (Inventario.Objetos[j] == Objetos[i])
                {
                    Inventario.Cantidades[j] -= Cantidades[i];
                    break;
                }
            }
        }
    }

    private void ActualizarMaterialesUI()
    {
        for (int i = 0; i < Objetos.Length; i++)
        {
            ObjetosUI[i].SetActive(true);

            // Asignar icono y cantidad requerida
            ObjetosUI[i].GetComponent<Image>().sprite = Objetos[i].Icono;

            TextMeshProUGUI textoCantidad =
                ObjetosUI[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            textoCantidad.text = Cantidades[i].ToString();

            // Buscar individualmente el objeto en el inventario
            int cantidadDisponible = 0;

            for (int j = 0; j <= Inventario.Objetos.Length; j++)
            {
                if (Inventario.Objetos[j] == Objetos[i])
                {
                    cantidadDisponible = Inventario.Cantidades[j];
                    break;
                }
            }

            // Comprobar si tiene los materiales necesarios
            if (cantidadDisponible >= Cantidades[i])
            {
                textoCantidad.color = Color.green;
            }
            else
            {
                textoCantidad.color = Color.red;
            }
        }

        // Desactivar espacios de objetos que no se utilizan
        for (int i = Objetos.Length; i < ObjetosUI.Length; i++)
        {
            ObjetosUI[i].SetActive(false);
        }
    }

}
