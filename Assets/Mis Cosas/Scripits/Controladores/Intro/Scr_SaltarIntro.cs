using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scr_SaltarIntro : MonoBehaviour
{
    public GameObject Boton;
    TextMeshProUGUI Texto;
    private float Tiempo = 7;
    private float Cont = 0;

    PlayerInput playerInput;
    private InputAction Saltar;
    InputIconProvider IconProvider;
    private Sprite iconoActualInteractuar = null;
    private string textoActualInteractuar = "";

    void Start()
    {
        Texto = GetComponent<TextMeshProUGUI>();
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Saltar = playerInput.actions["Recargar"];
    }

    void Update()
    {
        if (Tiempo > 0)
        {
            Tiempo -= Time.deltaTime;
            Texto.color = Color.white;
            Boton.SetActive(true);
            IconProvider.ActualizarIconoUI(Saltar, Boton.transform, ref iconoActualInteractuar, ref textoActualInteractuar, false);
        }
        else
        {
            Texto.color = new Color(0, 0, 0, 0);
            Boton.SetActive(false);
        }

        if (Saltar.IsPressed())
        {
            Tiempo = 7;
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
