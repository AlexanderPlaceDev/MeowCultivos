using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scr_ControladorIntro : MonoBehaviour
{
    [SerializeField] GameObject Camara1;
    [SerializeField] GameObject Camara2;
    [SerializeField] GameObject Camara3;
    [SerializeField] GameObject Camara4;
    [SerializeField] Scr_SistemaDialogos Dialogo;

    [SerializeField] GameObject Gusano;
    [SerializeField] GameObject Gata;
    [SerializeField] GameObject Meteorito;
    [SerializeField] GameObject PereodicoChico;
    [SerializeField] GameObject PereodicoGrande;
    [SerializeField] GameObject PantallaNegra;


    float cont1 = 0;
    float cont2 = 0;
    float cont3 = 0;
    float cont4 = 0;
    float cont5 = 0;
    float cont6 = 0;
    float cont7 = 0;
    float cont8 = 0;
    float cont9 = 0;
    float cont10 = 0;
    float cont11 = 0;

    void Start()
    {
        PantallaNegra.GetComponent<Animator>().Play("Aclarar");
    }

    void Update()
    {

        //Rutina();
        Pereodico();

       

    }


    void Pereodico()
    {
        if (PereodicoGrande.gameObject.activeSelf && Input.GetKeyDown("e"))
        {
            PereodicoGrande.SetActive(false);
            Gata.transform.GetChild(0).gameObject.SetActive(false);
            Dialogo.Leido = false;
            cont9=-1;
        }
        if (Gata.transform.GetChild(0).gameObject.activeSelf && Input.GetKeyDown("e"))
        {
            PereodicoGrande.SetActive(true);
            Gata.GetComponent<Animator>().Play("Posicion");
        }

    }
    void Rutina()
    {
        if (cont1 < 5 && cont1 >= 0)
        {

            cont1 += Time.deltaTime;
        }

        if (cont1 == -1 && cont2 < 2 && cont2 >= 0 && !Dialogo.gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            cont2 += Time.deltaTime;
        }

        if (cont2 == -1 && cont3 < 4 && cont3 >= 0)
        {
            cont3 += Time.deltaTime;
        }

        if (cont3 == -1 && cont4 < 1.5f && cont4 >= 0 && !Dialogo.gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            cont4 += Time.deltaTime;
        }

        if (cont4 == -1 && cont5 < 3 && cont5 >= 0)
        {
            cont5 += Time.deltaTime;
        }

        if (cont5 == -1 && cont6 < 1 && cont6 >= 0 && !Dialogo.gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            cont6 += Time.deltaTime;
        }

        if (cont6 == -1 && cont7 < 2 && cont7 >= 0)
        {
            cont7 += Time.deltaTime;
        }

        if (cont7 == -1 && cont8 < 0.2f && cont8 >= 0 && !Dialogo.gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            cont8 += Time.deltaTime;
        }

        if (cont7 == -1 && cont8 < 0.2f && cont8 >= 0 && !Dialogo.gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            cont8 += Time.deltaTime;
        }

        if (cont9 == -1 && cont10 < 1 && cont10 >= 0)
        {
            cont10 += Time.deltaTime;
        }

        if (cont10 == -1 && cont11 < 8 && cont11 >= 0)
        {
            cont11 += Time.deltaTime;
        }

        if (cont1 >= 5 && cont1 != 0 && Dialogo.DialogoActual == 0)
        {
            if (!Dialogo.Leido)
            {
                if (!Dialogo.Leyendo)
                {
                    
                        Dialogo.IniciarDialogo();
                        cont1 = -1;
                    

                }
            }
            else
            {

                cont1 = -1;
            }
        }

        if (cont2 >= 2 && cont2 != 0)
        {
            Dialogo.DialogoActual++;
            Camara1.SetActive(false);
            cont2 = -1;
            Dialogo.Leido = false;

        }

        if (cont3 >= 4 && cont3 != 0 && Dialogo.DialogoActual == 1)
        {
            if (!Dialogo.Leido)
            {
                if (!Dialogo.Leyendo)
                {
                        Dialogo.IniciarDialogo();
                        cont3 = -1;
                }
            }
        }

        if (cont4 >= 1.5f && cont4 != 0)
        {
            cont4 = -1;
            Dialogo.Leido = false;
            Dialogo.DialogoActual=2;
            Gusano.GetComponent<Animator>().Play("Moverse1");
        }


        if (cont5 >= 3 && cont5 != 0 && Dialogo.DialogoActual == 2)
        {
            if (!Dialogo.Leido)
            {
                if (!Dialogo.Leyendo)
                {
                        Dialogo.IniciarDialogo();
                        cont5 = -1;
                }
            }
        }

        if (cont6 >= 1 && cont6 != 0)
        {
            cont6 = -1;
            Dialogo.Leido = false;
            Dialogo.DialogoActual=3;
            Camara2.SetActive(false);
            Gusano.GetComponent<Animator>().Play("Moverse2");
            Gata.GetComponent<Animator>().Play("GataIntro");
        }

        if (cont7 >= 2 && cont7 != 0 && Dialogo.DialogoActual == 3)
        {
            if (!Dialogo.Leido)
            {
                if (!Dialogo.Leyendo)
                {
                        Dialogo.IniciarDialogo();
                        cont7 = -1;
                }
            }
        }

        if (cont8 >= 0.2f && cont8 != 0)
        {
            cont8=-1;
            Dialogo.DialogoActual=4;
            PereodicoChico.GetComponent<Animator>().Play("Volar");
            Gata.GetComponent<Animator>().Play("ActivarIcono");
            Camara3.SetActive(false);
        }

        if (cont10 >= 1 && cont7 != 0 && Dialogo.DialogoActual == 4)
        {
            if (!Dialogo.Leido)
            {
                if (!Dialogo.Leyendo)
                {
                    
                        Dialogo.IniciarDialogo();
                        cont10 = -1;
                }
            }
        }

        if(cont10==-1 && !Dialogo.gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            Gata.GetComponent<Animator>().Play("Salir");
            GameObject.Find("Ovni").GetComponent<Animator>().Play("Ovni");
            Camara4.SetActive(false);
        }

        if (cont11 >= 8 && cont11 != 0)
        {
            cont11=-1;
            PantallaNegra.GetComponent<Animator>().Play("Obscurecer");
            StartCoroutine(Cambiar());
        }
    }

    IEnumerator Cambiar(){

        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(2);
    }
}
