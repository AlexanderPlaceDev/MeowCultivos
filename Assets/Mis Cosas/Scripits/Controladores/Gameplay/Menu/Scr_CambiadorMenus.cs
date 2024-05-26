using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_CambiadorMenus : MonoBehaviour
{
    [SerializeField] Scr_CreadorTemas TemaActual;
    [SerializeField] GameObject Menu;
    [SerializeField] Image Fondo;
    [SerializeField] Image AreaHora;
    [SerializeField] GameObject BarraIzquierda;
    [SerializeField] GameObject BarraDerecha;
    [SerializeField] float DuracionTransicion;

    private float tiempoPasado = 0.0f;
    bool CambiarInventario = false;
    bool CambiarOpciones = false;
    bool CambiarGuia = false;
    bool CambiarHabilidades = false;
    bool RegresarInventario = false;
    bool RegresarOpciones = false;
    bool RegresarGuia = false;
    bool RegresarHabilidades = false;
    public string MenuActual = "Menu";

    private void Update()
    {
        // Incrementa el tiempo transcurrido
        tiempoPasado += Time.deltaTime;

        if (CambiarInventario)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionTransicion);
                Fondo.color = Color.Lerp(TemaActual.ColoresMenu[2], TemaActual.ColoresInventario[2], t);
                AreaHora.color = Color.Lerp(TemaActual.ColoresMenu[3], TemaActual.ColoresInventario[3], t);

                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresInventario[0], t);
                BarraIzquierda.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresInventario[1], t);

                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresInventario[0], t);
                BarraDerecha.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresInventario[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionTransicion + 1)
            {
                CambiarInventario = false;
            }
        }

        if (CambiarOpciones)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionTransicion);
                Fondo.color = Color.Lerp(TemaActual.ColoresMenu[2], TemaActual.ColoresOpciones[2], t);
                AreaHora.color = Color.Lerp(TemaActual.ColoresMenu[3], TemaActual.ColoresOpciones[3], t);

                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresOpciones[0], t);
                BarraIzquierda.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresOpciones[1], t);

                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresOpciones[0], t);
                BarraDerecha.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresOpciones[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionTransicion + 1)
            {
                CambiarOpciones = false;
            }
        }

        if (CambiarGuia)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionTransicion);
                Fondo.color = Color.Lerp(TemaActual.ColoresMenu[2], TemaActual.ColoresGuia[2], t);
                AreaHora.color = Color.Lerp(TemaActual.ColoresMenu[3], TemaActual.ColoresGuia[3], t);

                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresGuia[0], t);
                BarraIzquierda.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresGuia[1], t);

                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresGuia[0], t);
                BarraDerecha.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresGuia[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionTransicion + 1)
            {
                CambiarGuia = false;
            }
        }



        if (RegresarInventario)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionTransicion);
                Fondo.color = Color.Lerp(TemaActual.ColoresInventario[2], TemaActual.ColoresMenu[2], t);
                AreaHora.color = Color.Lerp(TemaActual.ColoresInventario[3], TemaActual.ColoresMenu[3], t);

                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[0], TemaActual.ColoresMenu[0], t);
                BarraIzquierda.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[1], TemaActual.ColoresMenu[1], t);

                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[0], TemaActual.ColoresMenu[0], t);
                BarraDerecha.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[1], TemaActual.ColoresMenu[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionTransicion + 1)
            {
                RegresarInventario = false;
            }
        }

        if (RegresarOpciones)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionTransicion);
                Fondo.color = Color.Lerp(TemaActual.ColoresOpciones[2], TemaActual.ColoresMenu[2], t);
                AreaHora.color = Color.Lerp(TemaActual.ColoresOpciones[3], TemaActual.ColoresMenu[3], t);

                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresOpciones[0], TemaActual.ColoresMenu[0], t);
                BarraIzquierda.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresOpciones[1], TemaActual.ColoresMenu[1], t);

                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresOpciones[0], TemaActual.ColoresMenu[0], t);
                BarraDerecha.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresOpciones[1], TemaActual.ColoresMenu[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionTransicion + 1)
            {
                RegresarOpciones = false;
            }
        }

        if (RegresarGuia)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionTransicion);
                Fondo.color = Color.Lerp(TemaActual.ColoresGuia[2], TemaActual.ColoresMenu[2], t);
                AreaHora.color = Color.Lerp(TemaActual.ColoresGuia[3], TemaActual.ColoresMenu[3], t);

                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresGuia[0], TemaActual.ColoresMenu[0], t);
                BarraIzquierda.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresGuia[1], TemaActual.ColoresMenu[1], t);

                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresGuia[0], TemaActual.ColoresMenu[0], t);
                BarraDerecha.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresGuia[1], TemaActual.ColoresMenu[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionTransicion + 1)
            {
                RegresarGuia = false;
            }
        }

    }


    public void CambiarAInventario()
    {
        tiempoPasado = 0.0f;
        CambiarInventario = true;
        MenuActual = "Inventario";
        Menu.GetComponent<Animator>().Play("Cerrar 0");
    }
    public void CambiarAOpciones()
    {
        tiempoPasado = 0.0f;
        CambiarOpciones = true;
        MenuActual = "Opciones";
        Menu.GetComponent<Animator>().Play("Cerrar 2");
    }

    public void CambiarAguia()
    {
        tiempoPasado = 0.0f;
        CambiarGuia = true;
        MenuActual = "Guia";
        Menu.GetComponent<Animator>().Play("Cerrar 3");
    }
    public void CambiarAHabilidades()
    {
        tiempoPasado = 0.0f;
        CambiarGuia = true;
        MenuActual = "Guia";
        Menu.GetComponent<Animator>().Play("Cerrar 3");
    }

    public void BotonRegresar()
    {
        Debug.Log("Entra 1");
        tiempoPasado = 0.0f;
        switch (MenuActual)
        {
            case "Inventario":
                {
                    Menu.GetComponent<Animator>().Play("Cerrar 1");
                    RegresarInventario = true;
                    break;
                }

            case "Opciones":
                {
                    Menu.GetComponent<Animator>().Play("Cerrar Opciones");
                    RegresarOpciones = true;
                    break;
                }

            case "Guia":
                {
                    Menu.GetComponent<Animator>().Play("Cerrar Guia");
                    RegresarGuia = true;
                    break;
                }
        }
        MenuActual = "Menu";
    }


}
