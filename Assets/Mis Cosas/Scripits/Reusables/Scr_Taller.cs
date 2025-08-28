using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class Scr_Taller : MonoBehaviour
{
    [SerializeField] private GameObject[] OjetosSelec; //objetos que puede seleccionar
    [SerializeField] private List<int> Ojetosint;
    [SerializeField] private GameObject[] BotonesOBJ; //flechas de objetos que puede seleccionar
    [SerializeField] private Sprite[] MarcoSelec; //Marco para mostar que esta seleccionado
    [SerializeField] private GameObject ObjetoPrincipal;//Objeto que muestra
    [SerializeField] private TextMeshProUGUI Seccion; //que seccion del taller muestra
    [SerializeField] private int lugar;// que seccion del taller esta

    [SerializeField] private GameObject Slots;// ui de slots de habilidades
    [SerializeField] private GameObject[] HabilidadesSelec; //habilidades que puede seleccionar
    [SerializeField] private GameObject SlotHabilidad1; //Slot para habilidad
    [SerializeField] private GameObject SlotHabilidad2; //Slot para habilidad
    
    [SerializeField] private GameObject Crafteo;//ui de crafteo
    [SerializeField] private Scr_CreadorObjetos[] ObjetosACraftear;//objetos que se pueden craftear
    [SerializeField] private GameObject[] CrafteosSelec;


    [SerializeField] private Color32[] ColoresBotones;

    GameObject Singleton;
    // Start is called before the first frame update
    void Start()
    {
        BuscarSingleton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void BuscarSingleton()
    {
        if (Singleton == null)
        {
            Singleton = GameObject.Find("Singleton");
        }
    }


    public void objeto_ON(int i)
    {
        OjetosSelec[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = MarcoSelec[1];
    }
    public void objeto_Exit(int i)
    {
        OjetosSelec[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = MarcoSelec[0];
    }

    public void mostrarObj(int i)
    {
        int objShow = Ojetosint[i];
        switch (lugar)
        {
            case 0:
                ObjetoPrincipal.SetActive(true);
                ObjetoPrincipal.transform.GetChild(0).gameObject.SetActive(true);
                ObjetoPrincipal.transform.GetChild(0).GetComponent<Image>().sprite = ObjetosACraftear[objShow].Icono; 
                Slots.SetActive(false);
                Crafteo.SetActive(true);
                break;

            case 1:
                ObjetoPrincipal.SetActive(true);
                ObjetoPrincipal.transform.GetChild(0).gameObject.SetActive(true);
                Slots.SetActive(false);
                Crafteo.SetActive(true);
                break;

            case 2:
                ObjetoPrincipal.SetActive(true);
                ObjetoPrincipal.transform.GetChild(0).gameObject.SetActive(true);
                ObjetoPrincipal.transform.GetChild(1).gameObject.SetActive(true);

                ObjetoPrincipal.transform.GetChild(0).GetComponent<Image>().sprite = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[objShow].Icono;

                Slots.SetActive(true);
                Crafteo.SetActive(false);
                break;
        }
    }
    //Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas[i]
    public void checarSeccion()
    {
        ObjetoPrincipal.transform.GetChild(0).gameObject.SetActive(false);
        ObjetoPrincipal.transform.GetChild(1).gameObject.SetActive(false);

        Slots.SetActive(false);
        Crafteo.SetActive(false);
        switch (lugar)
        {
            case 0:
                Seccion.text = "Objetos";
                checar_objetosCrafteables();
                break;

            case 1:
                Seccion.text = "Habilidades";
                break;

            case 2:
                Seccion.text = "Armas";
                checar_armasActivas();
                break;
        }
    }
    public void cambiarSeccionIzq()
    {
        if (lugar == 0)
        {
            lugar = 2;
            checarSeccion();
        }
        else
        {
            lugar--;
            checarSeccion();
        }
    }
    public void cambiarSeccionDer()
    {
        if (lugar == 2)
        {
            lugar = 0;
            checarSeccion();
        }
        else
        {
            lugar++;
            checarSeccion();
        }
    }

    public void cambiarOBjDer()
    {
        if (Ojetosint == null || Ojetosint.Count <= 1) return;

        int ultimoValor = Ojetosint[Ojetosint.Count - 1];

        for (int i = Ojetosint.Count - 1; i > 0; i--)
        {
            Ojetosint[i] = Ojetosint[i - 1];
        }

        Ojetosint[0] = ultimoValor;
        checarIconosOBJ();
        /*
        if (Ojetosint.Count == 6)
        {
            int e = 0;

            switch (lugar)
            {
                case 0:
                    e = ObjetosACraftear.Length;
                    break;
                case 1:

                    break;
                case 2:
                    e = Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas.Length;
                    break;
            }

            List<int> copiaDer = new List<int>(Ojetosint);

            // Guardamos el último valor que vamos a procesar
            int valorProcesado = copiaDer[5];

            // Desplazar todos los valores hacia la derecha
            for (int i = Ojetosint.Count - 1; i > 0; i--)
            {
                Ojetosint[i] = copiaDer[i - 1];
                switch (lugar)
                {
                    case 0:
                        OjetosSelec[i].GetComponent<Image>().sprite = ObjetosACraftear[copiaDer[i - 1]].Icono;
                        break;
                    case 1:
                        // Aquí puedes agregar la lógica para herramientas si quieres
                        break;
                    case 2:
                        OjetosSelec[i].GetComponent<Image>().sprite = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[copiaDer[i - 1]].Icono;
                        break;
                }
            }

            // Procesar y colocar el último valor al inicio (posición 0)
            if (valorProcesado == 5)
            {
                if ((valorProcesado + 1) <= e)
                    Ojetosint[0] = 0;
                else
                    Ojetosint[0] = valorProcesado + 1;
            }
            else
            {
                Ojetosint[0] = valorProcesado;
            }
        }*/
    }
    public void cambiarOBjIzq() 
    {

        if (Ojetosint == null || Ojetosint.Count <= 1) return;

        int primerValor = Ojetosint[0];

        for (int i = 0; i < Ojetosint.Count - 1; i++)
        {
            Ojetosint[i] = Ojetosint[i + 1];
        }

        Ojetosint[Ojetosint.Count - 1] = primerValor;
        checarIconosOBJ();
        /* if (Ojetosint.Count == 6)
         {
             int e = 0;
             switch (lugar)
             {
                 case 0:
                     e = ObjetosACraftear.Length;
                     break;
                 case 1:
                     // Aquí puedes agregar la lógica para herramientas si quieres
                     break;
                 case 2:
                     e = Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas.Length;
                     break;
             }

             List<int> copiaIzq = new List<int>(Ojetosint);

             // Guardamos el primer valor que vamos a procesar
             int valorProcesado = copiaIzq[0];

             // Desplazar todos los valores hacia la izquierda
             for (int i = 0; i < Ojetosint.Count - 1; i++)
             {
                 Ojetosint[i] = copiaIzq[i + 1];
                 switch (lugar)
                 {
                     case 0:
                         OjetosSelec[i].GetComponent<Image>().sprite = ObjetosACraftear[copiaIzq[i + 1]].Icono;
                         break;
                     case 1:
                         // Aquí puedes agregar la lógica para herramientas si quieres
                         break;
                     case 2:
                         OjetosSelec[i].GetComponent<Image>().sprite = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[copiaIzq[i + 1]].Icono;
                         break;
                 }
             }

             // Procesar y colocar el primer valor al final (posición 5)
             if (valorProcesado == 5)
             {
                 if ((valorProcesado + 1) <= e)
                     Ojetosint[5] = 0;
                 else
                     Ojetosint[5] = valorProcesado + 1;
             }
             else
             {
                 Ojetosint[5] = valorProcesado;
             }
         }*/
    }

    private void checarIconosOBJ()
    {
        for (int i = 0; i < 6; i++)
        {
            switch (lugar)
            {
                case 0:
                    OjetosSelec[i].GetComponent<Image>().sprite = ObjetosACraftear[Ojetosint[i]].Icono;
                    break;
                case 1:
                    // Aquí puedes agregar la lógica para herramientas si quieres
                    break;
                case 2:
                    OjetosSelec[i].GetComponent<Image>().sprite = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[Ojetosint[i]].Icono;
                    break;
            }
        }
    }
    public void checar_objetosCrafteables()
    {
        Ojetosint.Clear();
        if (ObjetosACraftear.Length < 6)
        {
            escoonder_botones();
            for (int i = 0; i < ObjetosACraftear.Length; i++)
            {
                OjetosSelec[i].SetActive(true);
                OjetosSelec[i].GetComponent<Image>().sprite = ObjetosACraftear[i].Icono;
                Ojetosint.Add(i);
            }
        }
        else
        {
            Aparecer_botones();
            for (int i = 0; i < ObjetosACraftear.Length; i++)
            {
                if (i < 6)
                {
                    OjetosSelec[i].GetComponent<Image>().sprite = ObjetosACraftear[i].Icono;
                }
                Ojetosint.Add(i);
            }
        }
    }
    public void checar_Habilidades()
    {
        Ojetosint.Clear();
        Aparecer_botones();
        /*
        if (ObjetosACraftear.Length < 6)
        {
            escoonder_botones();
            for (int i = 0; i < ObjetosACraftear.Length; i++)
            {
                OjetosSelec[i].SetActive(true);
                OjetosSelec[i].GetComponent<UnityEngine.UI.Image>().sprite = ObjetosACraftear[i].Icono;
            }
        }
        else
        {
            Aparecer_botones();
            for (int i = 0; i < 6; i++)
            {
                OjetosSelec[i].GetComponent<UnityEngine.UI.Image>().sprite = ObjetosACraftear[i].Icono;
            }
        }*/
    }
    public void checar_armasActivas()
    {
        Ojetosint.Clear();
        if (Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas.Length <6)
        {
            escoonder_botones();
            for (int i = 0; i < Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas.Length; i++)
            {
                if (Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas[i])
                {
                    OjetosSelec[i].SetActive(true);
                    OjetosSelec[i].GetComponent<Image>().sprite = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[i].Icono;
                    Ojetosint.Add(i);
                }
            }
        }
        else
        {
            Aparecer_botones();
            for (int i = 0; i < Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas.Length; i++)
            {
                if (Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas[i])
                {
                    if (i < 6)
                    {
                        OjetosSelec[i].GetComponent<Image>().sprite = Singleton.GetComponent<Scr_DatosArmas>().TodasLasArmas[i].Icono;
                    }
                    Ojetosint.Add(i);
                }
            }
        }
    }

    private void escoonder_botones()
    {
        for (int i = 0; i < 6; i++) 
        {
            OjetosSelec[i].SetActive(false);
        }
        for (int i = 0; i <BotonesOBJ.Length; i++)
        {
            BotonesOBJ[i].SetActive(false);
        }
    }
    private void Aparecer_botones()
    {
        for (int i = 0; i < 6; i++)
        {
            OjetosSelec[i].SetActive(true);
        }
        for (int i = 0; i < BotonesOBJ.Length; i++)
        {
            BotonesOBJ[i].SetActive(true);
        }
    }
}
