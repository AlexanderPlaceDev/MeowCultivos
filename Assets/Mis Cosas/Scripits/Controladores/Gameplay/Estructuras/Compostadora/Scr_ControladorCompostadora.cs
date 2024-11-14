using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorCompostadora : MonoBehaviour
{
    [SerializeField] Scr_CreadorObjetos[] ObjetosAceptados;
    [SerializeField] Scr_CreadorObjetos[] ObjetosQueProduce;
    [SerializeField] Image Semilla;
    [SerializeField] TextMeshProUGUI Texto;
    [SerializeField] Image Barra;
    [SerializeField] TextMeshProUGUI TextoPorcentaje1;
    [SerializeField] TextMeshProUGUI TextoPorcentaje2;

    Scr_ObjetosAgregados ObjetosAgregados;
    Scr_Inventario Inventario;
    int ObjetoActual = 0;
    int PosicionInventario = 0;
    int PorcentajePlus = 0;

    // Guarda la referencia del objeto semilla
    GameObject Abono;

    void Start()
    {
        ObjetosAgregados = GameObject.Find("ObjetosAgregados").GetComponent<Scr_ObjetosAgregados>();
        Inventario = GameObject.Find("Gata").transform.GetChild(6).GetComponent<Scr_Inventario>();

        // Guarda la referencia de la semilla
        Abono = transform.GetChild(2).gameObject;
    }

    void Update()
    {
        ActualizarUI();
        ActualizarSemilla();
    }

    void ActualizarSemilla()
    {
        for (int i = 0; i < Inventario.Objetos.Count(); i++)
        {
            if (Inventario.Objetos[i] == ObjetosAceptados[ObjetoActual])
            {
                Semilla.sprite = Inventario.Objetos[i].Icono;
                Texto.text = Inventario.Cantidades[i].ToString();
                PosicionInventario = i;
                break;
            }
        }
    }

    void ActualizarUI()
    {
        bool isActive = transform.GetChild(0).gameObject.activeSelf;
        transform.GetChild(1).GetChild(0).gameObject.SetActive(isActive);
        transform.GetChild(1).GetChild(1).gameObject.SetActive(isActive);
        transform.GetChild(1).GetChild(2).gameObject.SetActive(isActive);
        transform.GetChild(1).GetChild(3).gameObject.SetActive(isActive);
        transform.GetChild(1).GetChild(7).gameObject.SetActive(isActive);
        transform.GetChild(1).GetChild(8).gameObject.SetActive(isActive);

        TextoPorcentaje1.text = (100 - PorcentajePlus) + "%";
        TextoPorcentaje2.text = PorcentajePlus + "%";
    }

    public void FlechaDerecha()
    {
        ObjetoActual = (ObjetoActual + 1) % ObjetosAceptados.Length;
    }

    public void FlechaIzquierda()
    {
        ObjetoActual = (ObjetoActual == 0) ? ObjetosAceptados.Length - 1 : ObjetoActual - 1;
    }

    public void BotonAgregar()
    {
        if (Inventario.Cantidades[PosicionInventario] > 0)
        {
            if (Barra.fillAmount <= 0.9f)
            {
                Barra.fillAmount += 0.1f;
                Inventario.Cantidades[PosicionInventario]--;
                PorcentajePlus += (int)Inventario.Objetos[PosicionInventario].ValorComposta;

                // Activa la semilla y la mueve en el eje Y controladamente
                Abono.SetActive(true);
                MoverObjetoEnY(Abono, 0.1f); // Mueve en Y de 0.1
            }
            else
            {
                Barra.fillAmount = 0;
                Inventario.Cantidades[PosicionInventario]--;
                Abono.SetActive(false);
                // Asegura que la posición en Y se resetea a la posición original
                ResetearPosicionAbono();
                DarObjeto();
            }
        }
    }

    // Método para mover el objeto en el eje Y ajustando la posición directamente
    void MoverObjetoEnY(GameObject obj, float incrementoZ)
    {
        Vector3 nuevaPosicion = obj.transform.localPosition; // Usa localPosition en lugar de position

        // Solo modificamos el eje Y
        nuevaPosicion.z += incrementoZ;

        // Asignamos la nueva posición local
        obj.transform.localPosition = nuevaPosicion;
    }

    // Método para resetear la posición del objeto 'Abono'
    void ResetearPosicionAbono()
    {
        // Guarda la posición original en Y (o la posición inicial en tu escenario)
        // Ejemplo, si la posición original en Y es 0.9f
        float posicionInicialZ = 0.9f;

        Vector3 nuevaPosicion = Abono.transform.localPosition;
        nuevaPosicion.z = posicionInicialZ; // Resetear solo el eje Y

        // Asignar la nueva posición local
        Abono.transform.localPosition = nuevaPosicion;
    }

    void DarObjeto()
    {
        int R = Random.Range(1, 100);
        if (R > PorcentajePlus)
        {
            ObjetosAgregados.Lista.Add(ObjetosQueProduce[0]);
        }
        else
        {
            ObjetosAgregados.Lista.Add(ObjetosQueProduce[1]);
        }

        ObjetosAgregados.Cantidades.Add(1);

        // Resetea PorcentajePlus para el siguiente uso
        PorcentajePlus = 0;
    }

    public void BotonCerrar()
    {
        GetComponent<Scr_ActivadorMenuEstructuraFijo>().CerrarTablero();
    }

    public void BotonBasura()
    {
        PorcentajePlus = 0;
        Abono.SetActive(false);
        ResetearPosicionAbono();
        Barra.fillAmount = 0;
    }
}
