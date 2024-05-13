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
    [SerializeField] float DuracionInventario;

    private float tiempoPasado = 0.0f;
    bool CambiarInventario = false;
    bool RegresarInventario = false;

    private void Update()
    {
        // Incrementa el tiempo transcurrido
        tiempoPasado += Time.deltaTime;


        if (CambiarInventario)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado-1 / DuracionInventario);
                Fondo.color = Color.Lerp(TemaActual.ColoresMenu[2], TemaActual.ColoresInventario[2], t);
                AreaHora.color = Color.Lerp(TemaActual.ColoresMenu[3], TemaActual.ColoresInventario[3], t);

                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresInventario[0], t);
                BarraIzquierda.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresInventario[1], t);

                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[0], TemaActual.ColoresInventario[0], t);
                BarraDerecha.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresMenu[1], TemaActual.ColoresInventario[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionInventario+1)
            {
                CambiarInventario = false;
            }
        }

        if (RegresarInventario)
        {
            // Calcula la interpolación lineal entre los colores inicial y final

            if (tiempoPasado >= 1)
            {
                float t = Mathf.Clamp01(tiempoPasado - 1 / DuracionInventario);
                Fondo.color = Color.Lerp(TemaActual.ColoresInventario[2], TemaActual.ColoresMenu[2], t);
                AreaHora.color = Color.Lerp(TemaActual.ColoresInventario[3], TemaActual.ColoresMenu[3], t);

                BarraIzquierda.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[0], TemaActual.ColoresMenu[0], t);
                BarraIzquierda.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[1], TemaActual.ColoresMenu[1], t);

                BarraDerecha.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[0], TemaActual.ColoresMenu[0], t);
                BarraDerecha.transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(TemaActual.ColoresInventario[1], TemaActual.ColoresMenu[1], t);
            }

            // Si el tiempo ha superado la duración, desactiva la actualización
            if (tiempoPasado >= DuracionInventario + 1)
            {
                RegresarInventario = false;
            }
        }

    }


    public void CambiarAInventario()
    {
        Menu.GetComponent<Animator>().Play("Cerrar 0");
        tiempoPasado = 0.0f;
        CambiarInventario = true;
    }

    public void RegresarDelInventario()
    {
        Menu.GetComponent<Animator>().Play("Cerrar 1");
        tiempoPasado = 0.0f;
        RegresarInventario = true;
    }
}
