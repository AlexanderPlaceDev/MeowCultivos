using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scr_SaltarIntro : MonoBehaviour
{
    TextMeshProUGUI Texto;
    private float Tiempo = 5;
    private float Cont = 0;

    void Start()
    {
        Texto = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Tiempo > 0)
        {
            Tiempo -= Time.deltaTime;
            Texto.color = Color.white;
        }
        else
        {
            Texto.color = new Color(0, 0, 0, 0);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Tiempo = 5;
            if (Cont < 3)
            {
                Cont += Time.deltaTime;
            }
            else
            {
                SceneManager.LoadScene(2);
            }
        }
        else
        {
            if (Cont > 0)
            {
                Cont -= Time.deltaTime;

            }
        }

        transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(Cont / 3, 1, 1);

    }
}
