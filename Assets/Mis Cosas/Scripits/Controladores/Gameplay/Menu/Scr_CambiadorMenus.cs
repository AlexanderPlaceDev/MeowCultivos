using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scr_CambiadorMenus : MonoBehaviour
{
    [SerializeField] Scr_CreadorTemas TemaActual;
    [SerializeField] GameObject Menu;
    [SerializeField] Image Fondo;
    [SerializeField] GameObject BarraIzquierda;
    [SerializeField] GameObject BarraDerecha;
    [SerializeField] float DuracionTransicion;
    [SerializeField] GameObject BotonesMenu;
    private float tiempoPasado = 0.0f;
    bool CambiarInventario = false;
    bool CambiarArmas = false;
    bool CambiarOpciones = false;
    bool CambiarGuia = false;
    bool CambiarHabilidades = false;
    bool RegresarInventario = false;
    bool RegresarOpciones = false;
    bool RegresarGuia = false;
    bool RegresarHabilidades = false;
    public string MenuActual = "Inventario";

    private void Update()
    {
        // Incrementa el tiempo transcurrido
        tiempoPasado += Time.deltaTime;

        if (CambiarHabilidades)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionTransicion);
                Fondo.color = Color.Lerp(TemaActual.ColoresMenu[2], TemaActual.ColoresHabilidades[2], t);

                BarraIzquierda.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresHabilidades[0], t);
                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresHabilidades[1], t);

                BarraDerecha.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresHabilidades[0], t);
                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresHabilidades[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionTransicion + 1)
            {
                CambiarHabilidades = false;
                GetComponent<Scr_ControladorMenuHabilidades>().enabled = true;
            }
        }

        if (CambiarInventario)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionTransicion);
                Fondo.color = Color.Lerp(TemaActual.ColoresMenu[2], TemaActual.ColoresInventario[2], t);

                BarraIzquierda.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresInventario[0], t);
                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresInventario[1], t);

                BarraDerecha.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresInventario[0], t);
                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresInventario[1], t);
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

                BarraIzquierda.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresOpciones[0], t);
                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresOpciones[1], t);

                BarraDerecha.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresOpciones[0], t);
                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresOpciones[1], t);
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

                BarraIzquierda.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresGuia[0], t);
                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresGuia[1], t);

                BarraDerecha.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresGuia[0], t);
                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresGuia[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionTransicion + 1)
            {
                CambiarGuia = false;
            }
        }


        if (RegresarHabilidades)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionTransicion);
                Fondo.color = Color.Lerp(TemaActual.ColoresHabilidades[2], TemaActual.ColoresMenu[2], t);

                BarraIzquierda.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresHabilidades[0], TemaActual.ColoresMenu[0], t);
                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresHabilidades[1], TemaActual.ColoresMenu[1], t);

                BarraDerecha.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresHabilidades[0], TemaActual.ColoresMenu[0], t);
                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresHabilidades[1], TemaActual.ColoresMenu[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionTransicion + 1)
            {
                RegresarHabilidades = false;
                GetComponent<Scr_ControladorMenuHabilidades>().enabled = false;
                GetComponent<Scr_ControladorMenuHabilidades>().HabilidadActual = "";
                GetComponent<Scr_ControladorMenuHabilidades>().BotonActual = null;
            }
        }

        if (RegresarInventario)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionTransicion);
                Fondo.color = Color.Lerp(TemaActual.ColoresInventario[2], TemaActual.ColoresMenu[2], t);

                BarraIzquierda.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[0], TemaActual.ColoresMenu[0], t);
                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[1], TemaActual.ColoresMenu[1], t);

                BarraDerecha.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[0], TemaActual.ColoresMenu[0], t);
                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[1], TemaActual.ColoresMenu[1], t);
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

                BarraIzquierda.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresOpciones[0], TemaActual.ColoresMenu[0], t);
                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresOpciones[1], TemaActual.ColoresMenu[1], t);

                BarraDerecha.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresOpciones[0], TemaActual.ColoresMenu[0], t);
                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresOpciones[1], TemaActual.ColoresMenu[1], t);
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

                BarraIzquierda.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresGuia[0], TemaActual.ColoresMenu[0], t);
                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresGuia[1], TemaActual.ColoresMenu[1], t);

                BarraDerecha.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresGuia[0], TemaActual.ColoresMenu[0], t);
                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresGuia[1], TemaActual.ColoresMenu[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionTransicion + 1)
            {
                RegresarGuia = false;
            }
        }

    }

    public void Cambiar()
    {
        if (!GetComponent<Scr_ControladorMenuGameplay>().EstaReproduciendoAnimacion())
        {
            tiempoPasado = 0;
            switch (MenuActual)
            {
                case "Inventario":
                    {
                        CambiarInventario = true;
                        MenuActual = "Inventario";
                        Menu.GetComponent<Animator>().Play("Cerrar 0");
                        break;
                    }

                case "Armas":
                    {
                        CambiarArmas = true;
                        MenuActual = "Armas";
                        Menu.GetComponent<Animator>().Play("Cerrar 1");
                        break;
                    }

                case "Guia":
                    {
                        CambiarGuia = true;
                        MenuActual = "Guia";
                        Menu.GetComponent<Animator>().Play("Cerrar 3");
                        break;
                    }

                case "Habilidades":
                    {
                        CambiarHabilidades = true;
                        MenuActual = "Habilidades";
                        Menu.GetComponent<Animator>().Play("Cerrar 4");
                        break;
                    }

                case "Opciones":
                    {
                        CambiarOpciones = true;
                        MenuActual = "Opciones";
                        Menu.GetComponent<Animator>().Play("Cerrar 2");
                        break;
                    }

                case "Salir":
                    {
                        SceneManager.LoadScene(0);
                        break;
                    }

                default:
                    {
                        Debug.Log("No encontre el menu");
                        break;
                    }
            }
        }

    }

    public void CambiarArriba()
    {
        if (!GetComponent<Scr_ControladorMenuGameplay>().EstaReproduciendoAnimacion())
        {
            BotonesMenu.GetComponent<scr_BotonesMenuJuego>().ClicFlecha(true);
            tiempoPasado = 0;
            switch (MenuActual)
            {
                case "Inventario":
                    {
                        CambiarHabilidades = true;
                        MenuActual = "Habilidades";
                        Menu.GetComponent<Animator>().Play("Cerrar 4");
                        break;
                    }

                case "Armas":
                    {
                        CambiarGuia = true;
                        MenuActual = "Guia";
                        Menu.GetComponent<Animator>().Play("Cerrar 3");
                        break;
                    }

                case "Guia":
                    {
                        CambiarInventario = true;
                        MenuActual = "Inventario";
                        Menu.GetComponent<Animator>().Play("Cerrar 0");
                        break;
                    }

                case "Habilidades":
                    {
                        SceneManager.LoadScene(0);
                        break;
                    }

                case "Opciones":
                    {
                        CambiarArmas = true;
                        MenuActual = "Armas";
                        Menu.GetComponent<Animator>().Play("Cerrar 1");
                        break;
                    }

                case "Salir":
                    {
                        CambiarOpciones = true;
                        MenuActual = "Opciones";
                        Menu.GetComponent<Animator>().Play("Cerrar 2");
                        break;
                    }
            }
        }
    }
    public void CambiarAbajo()
    {
        if (!GetComponent<Scr_ControladorMenuGameplay>().EstaReproduciendoAnimacion())
        {
            BotonesMenu.GetComponent<scr_BotonesMenuJuego>().ClicFlecha(false);
            tiempoPasado = 0;
            switch (MenuActual)
            {
                case "Inventario":
                    {

                        CambiarGuia = true;
                        MenuActual = "Guia";
                        Menu.GetComponent<Animator>().Play("Cerrar 3");
                        break;
                    }

                case "Armas":
                    {
                        CambiarOpciones = true;
                        MenuActual = "Opciones";
                        Menu.GetComponent<Animator>().Play("Cerrar 2");

                        break;
                    }

                case "Guia":
                    {
                        CambiarArmas = true;
                        MenuActual = "Armas";
                        Menu.GetComponent<Animator>().Play("Cerrar 1");
                        break;
                    }

                case "Habilidades":
                    {
                        CambiarInventario = true;
                        MenuActual = "Inventario";
                        Menu.GetComponent<Animator>().Play("Cerrar 0");
                        break;
                    }

                case "Opciones":
                    {
                        SceneManager.LoadScene(0);

                        break;
                    }

                case "Salir":
                    {
                        CambiarHabilidades = true;
                        MenuActual = "Habilidades";
                        Menu.GetComponent<Animator>().Play("Cerrar 4");

                        break;
                    }
            }
        }
    }

    public void BotonRegresar()
    {
        Debug.Log("Entra 1");
        tiempoPasado = 0.0f;
        switch (MenuActual)
        {
            case "Habilidades":
                {
                    Menu.GetComponent<Animator>().Play("Cerrar Habilidades");
                    RegresarHabilidades = true;
                    break;
                }

            case "Inventario":
                {
                    Menu.GetComponent<Animator>().Play("Cerrar 1");
                    RegresarInventario = true;
                    break;
                }

            case "Guia":
                {
                    Menu.GetComponent<Animator>().Play("Cerrar Guia");
                    RegresarGuia = true;
                    break;
                }

            case "Opciones":
                {
                    Menu.GetComponent<Animator>().Play("Cerrar Opciones");
                    RegresarOpciones = true;
                    break;
                }
        }
    }
}
