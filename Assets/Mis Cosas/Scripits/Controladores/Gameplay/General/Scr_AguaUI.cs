using System.Collections;
using TMPro;
using UnityEngine;

public class Scr_AguaUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextoCantidad;

    [SerializeField] float contadorTirarAgua = 2f; // TIEMPO requerido
    private float contadorActual = 0f;

    private Scr_Movimiento Movimiento;

    void Start()
    {
        Movimiento = GameObject.Find("Gata").GetComponent<Scr_Movimiento>();
    }

    void Update()
    {
        VerificarMovimiento();
        ActualizarUI();
    }

    void VerificarMovimiento()
    {
        // Si no está corriendo → reinicia contador y no anima
        if (Movimiento.Estado != Scr_Movimiento.Estados.Correr || PlayerPrefs.GetString("Habilidad:Regadera", "No") == "Si")
        {
            contadorActual = 0f;
            return;
        }

        // Animación "Temblar" del padre del texto
        Animator anim = TextoCantidad.transform.parent.GetComponent<Animator>();
        if (anim.gameObject.activeSelf)
        {
            AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);

        }


        if (anim.gameObject.activeSelf)
        {
            anim.Play("Temblar");  // reproduce desde el inicio
        }


        // Si está corriendo, acumula tiempo
        contadorActual += Time.deltaTime;

        // Si pasó el tiempo requerido (ej. 2 segundos)
        if (contadorActual >= contadorTirarAgua)
        {
            contadorActual = 0f; // reinicia el ciclo

            int cantidad = PlayerPrefs.GetInt("CantidadAgua", 0);

            if (cantidad > 0)
            {
                cantidad -= 1;      // siempre tira 5 si está corriendo
                cantidad = Mathf.Max(0, cantidad);
                PlayerPrefs.SetInt("CantidadAgua", cantidad);
            }
        }
    }


    void ActualizarUI()
    {
        if (PlayerPrefs.GetString("Habilidad:Regadera", "No") == "Si")
        {
            if (PlayerPrefs.GetInt("CantidadAgua", 0) > 0)
            {
                if (!transform.GetChild(0).gameObject.activeSelf)
                    transform.GetChild(0).gameObject.SetActive(true);
                else
                    TextoCantidad.text = PlayerPrefs.GetInt("CantidadAgua", 0) + "/50";
            }
            else transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetString("Habilidad:Cubeta", "No") == "Si")
        {
            if (PlayerPrefs.GetInt("CantidadAgua", 0) > 0)
            {
                if (!transform.GetChild(0).gameObject.activeSelf)
                    transform.GetChild(0).gameObject.SetActive(true);
                else
                    TextoCantidad.text = PlayerPrefs.GetInt("CantidadAgua", 0) + "/25";
            }
            else transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
