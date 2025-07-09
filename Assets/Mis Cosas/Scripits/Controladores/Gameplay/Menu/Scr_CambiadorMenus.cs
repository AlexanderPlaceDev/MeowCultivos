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
    bool CambiarMisiones = false;
    bool CambiarArmas = false;
    bool CambiarOpciones = false;
    bool CambiarGuia = false;
    bool CambiarHabilidades = false;
    bool RegresarInventario = false;
    bool RegresarOpciones = false;
    bool RegresarMisiones = false;
    bool RegresarGuia = false;
    bool RegresarHabilidades = false;
    public string MenuActual = "Inventario";

    private void Update()
    {
        tiempoPasado += Time.deltaTime;

        // ---- CAMBIAR ----
        if (CambiarHabilidades) LerpColores(TemaActual.ColoresMenu, TemaActual.ColoresHabilidades, () => CambiarHabilidades = false);
        if (CambiarInventario) LerpColores(TemaActual.ColoresMenu, TemaActual.ColoresInventario, () => CambiarInventario = false);
        if (CambiarOpciones) LerpColores(TemaActual.ColoresMenu, TemaActual.ColoresOpciones, () => CambiarOpciones = false);
        if (CambiarGuia) LerpColores(TemaActual.ColoresMenu, TemaActual.ColoresGuia, () => CambiarGuia = false);
        if (CambiarMisiones) LerpColores(TemaActual.ColoresMenu, TemaActual.ColoresMisiones, () => CambiarMisiones = false);

        // ---- REGRESAR ----
        if (RegresarHabilidades) LerpColores(TemaActual.ColoresHabilidades, TemaActual.ColoresMenu, () => RegresarHabilidades = false);
        if (RegresarInventario) LerpColores(TemaActual.ColoresInventario, TemaActual.ColoresMenu, () => RegresarInventario = false);
        if (RegresarOpciones) LerpColores(TemaActual.ColoresOpciones, TemaActual.ColoresMenu, () => RegresarOpciones = false);
        if (RegresarGuia) LerpColores(TemaActual.ColoresGuia, TemaActual.ColoresMenu, () => RegresarGuia = false);
        if (RegresarMisiones) LerpColores(TemaActual.ColoresMisiones, TemaActual.ColoresMenu, () => RegresarMisiones = false);
    }

    private void LerpColores(Color[] desde, Color[] hasta, System.Action onFinish)
    {
        if (tiempoPasado >= 0f)
        {
            float t = Mathf.Clamp01((tiempoPasado - 1f) / DuracionTransicion);
            Fondo.color = Color.Lerp(desde[2], hasta[2], t);
            BarraIzquierda.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(desde[0], hasta[0], t);
            BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(desde[1], hasta[1], t);
            BarraDerecha.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(desde[0], hasta[0], t);
            BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(desde[1], hasta[1], t);
        }

        if (tiempoPasado >= DuracionTransicion + 1f)
        {
            onFinish?.Invoke();
        }
    }

    public void Cambiar()
    {
        if (!GetComponent<Scr_ControladorMenuGameplay>().EstaReproduciendoAnimacion())
        {
            tiempoPasado = 0f;
            switch (MenuActual)
            {
                case "Inventario":
                    CambiarInventario = true;
                    MenuActual = "Inventario";
                    Menu.GetComponent<Animator>().Play("Cerrar 0");
                    break;
                case "Misiones":
                    CambiarMisiones = true;
                    MenuActual = "Misiones";
                    Menu.GetComponent<Animator>().Play("Cerrar 5");
                    break;
                case "Armas":
                    CambiarArmas = true;
                    MenuActual = "Armas";
                    Menu.GetComponent<Animator>().Play("Cerrar 1");
                    break;
                case "Guia":
                    CambiarGuia = true;
                    MenuActual = "Guia";
                    Menu.GetComponent<Animator>().Play("Cerrar 3");
                    break;
                case "Habilidades":
                    CambiarHabilidades = true;
                    MenuActual = "Habilidades";
                    Menu.GetComponent<Animator>().Play("Cerrar 4");
                    break;
                case "Opciones":
                    CambiarOpciones = true;
                    MenuActual = "Opciones";
                    Menu.GetComponent<Animator>().Play("Cerrar 2");
                    break;
                case "Salir":
                    SceneManager.LoadScene(0);
                    break;
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
                Menu.GetComponent<Animator>().Play("Cerrar Habilidades");
                RegresarHabilidades = true;
                break;
            case "Inventario":
                Menu.GetComponent<Animator>().Play("Cerrar Inventario");
                RegresarInventario = true;
                break;
            case "Guia":
                Menu.GetComponent<Animator>().Play("Cerrar Guia");
                RegresarGuia = true;
                break;
            case "Opciones":
                Menu.GetComponent<Animator>().Play("Cerrar Opciones");
                RegresarOpciones = true;
                break;
            case "Misiones":
                Menu.GetComponent<Animator>().Play("Cerrar Misiones");
                RegresarMisiones = true;
                break;
        }
    }
}
