using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_DatosArmas : MonoBehaviour
{
    public Scr_CreadorArmas[] TodasLasArmas;
    public bool[] ArmasDesbloqueadas;
    void Start()
    {
        for (int i = 1; i < TodasLasArmas.Length; i++)
        {
            if (PlayerPrefs.GetString("Arma" + TodasLasArmas[i].Nombre, "No") == "Si")
            {
                ArmasDesbloqueadas[i] = true;

            }
            else
            {
                ArmasDesbloqueadas[i] = false;
            }
        }
    }

    void Update()
    {

    }
}
