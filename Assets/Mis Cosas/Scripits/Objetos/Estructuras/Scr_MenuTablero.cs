using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_MenuTablero : MonoBehaviour
{
    [Header("Estructuras")]
    [SerializeField] Scr_CreadorEstructuras[] EstructurasIndustriales;
    [SerializeField] GameObject[] ObjEstructurasIndustriales;
    public bool[] EstructurasIndustrialesGuardadas;
    [SerializeField] Scr_CreadorEstructuras[] EstructurasGranja;
    [SerializeField] GameObject[] ObjEstructurasGranja;
    public bool[] EstructurasGranjaGuardadas;
    [SerializeField] Scr_CreadorEstructuras[] EstructurasArma;
    [SerializeField] GameObject[] ObjEstructurasArma;
    public bool[] EstructurasArmaGuardadas;


    [Header("Menu")]
    [SerializeField] GameObject[] BotonesTipos;
    [SerializeField] TextMeshProUGUI Nombre;
    [SerializeField] TextMeshProUGUI Descripcion;
    [SerializeField] GameObject BotonConstruir;
    [SerializeField] GameObject ImagenConstruido;
    [SerializeField] Image Imagen;
    [SerializeField] Sprite[] TodosLosMateriales;
    [SerializeField] Image[] Materiales;
    [SerializeField] TextMeshProUGUI[] Cantidades;

    [Header("Inventario")]
    [SerializeField] Scr_ControladorInventario Inventario;
    public int TipoActual = 1;
    public int EstructuraActual = 0;
    float cont = 0;
    void Start()
    {
        ActivarEstructuras(false);
    }

    void Update()
    {
        RellenarCirculo();
        if (GetComponent<Scr_Tablero>().EstaDentro)
        {
            ActualizarEstructura();
            GetComponent<Scr_EventosGuardado>().GuardarTablero(TipoActual, EstructuraActual);

        }
    }

    private void RellenarCirculo()
    {
        //Esta parte Rellena el circulo
        if (cont < 0.5f)
        {
            cont += Time.deltaTime;
            BotonesTipos[TipoActual].transform.GetChild(0).GetComponent<Image>().fillAmount = cont * 2;
        }
        //Esta lo elimina de los que no estan seleccionados
        for (int i = 0; i < BotonesTipos.Length; i++)
        {
            if (i != TipoActual)
            {
                BotonesTipos[i].transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
            }
        }
    }

    //Aqui se actualizan los datos de la estructura
    void ActualizarEstructura()
    {
        switch (TipoActual)
        {
            case 0:
                {
                    //Actualiza si esta comprada la estructura
                    if (EstructurasIndustrialesGuardadas[EstructuraActual])
                    {
                        BotonConstruir.SetActive(false);
                        ImagenConstruido.SetActive(true);
                    }
                    else
                    {
                        BotonConstruir.SetActive(true);
                        ImagenConstruido.SetActive(false);
                    }


                    //Para cada estructura en la lista de industrial
                    for (int i = 0; i < EstructurasIndustriales.Length; i++)
                    {
                        //En caso de ser la estructura actual
                        if (i == EstructuraActual)
                        {
                            //Actualiza los datos
                            Nombre.text = EstructurasIndustriales[i].Nombre;
                            Descripcion.text = EstructurasIndustriales[i].Descripcion;
                            Imagen.sprite = EstructurasIndustriales[i].Imagen;

                            //Actualiza los materiales
                            //Para cada material
                            foreach (Sprite Imagen in TodosLosMateriales)
                            {
                                //Para cada material en el tablero
                                for (int j = 0; j < Materiales.Length; j++)
                                {
                                    //En caso de los materiales vacios
                                    if (j >= EstructurasIndustriales[i].Materiales.Length)
                                    {
                                        Materiales[j].gameObject.SetActive(false);
                                    }
                                    else
                                    {
                                        //En caso de los materiales necesarios
                                        if (Imagen.name == EstructurasIndustriales[i].Materiales[j])
                                        {
                                            Materiales[j].gameObject.SetActive(true);
                                            Materiales[j].sprite = Imagen;
                                            Cantidades[j].text = EstructurasIndustriales[i].Cantidades[j].ToString();
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            case 1:
                {
                    //Actualiza si esta comprada la estructura
                    if (EstructurasGranjaGuardadas[EstructuraActual])
                    {
                        BotonConstruir.SetActive(false);
                        ImagenConstruido.SetActive(true);
                    }
                    else
                    {
                        BotonConstruir.SetActive(true);
                        ImagenConstruido.SetActive(false);
                    }

                    //Para cada estructura en la lista de granja
                    for (int i = 0; i < EstructurasGranja.Length; i++)
                    {
                        //En caso de ser la estructura actual
                        if (i == EstructuraActual)
                        {
                            //Actualiza los datos
                            Nombre.text = EstructurasGranja[i].Nombre;
                            Descripcion.text = EstructurasGranja[i].Descripcion;
                            Imagen.sprite = EstructurasGranja[i].Imagen;

                            //Actualiza los materiales
                            //Para cada material
                            foreach (Sprite Imagen in TodosLosMateriales)
                            {
                                //Para cada material en el tablero
                                for (int j = 0; j < Materiales.Length; j++)
                                {
                                    //En caso de los materiales vacios
                                    if (j >= EstructurasGranja[i].Materiales.Length)
                                    {
                                        Materiales[j].gameObject.SetActive(false);
                                    }
                                    else
                                    {
                                        //En caso de los materiales necesarios
                                        if (Imagen.name == EstructurasGranja[i].Materiales[j])
                                        {
                                            Materiales[j].gameObject.SetActive(true);
                                            Materiales[j].sprite = Imagen;
                                            Cantidades[j].text = EstructurasGranja[i].Cantidades[j].ToString();
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
        }

    }


    //Aqui se selecciona el tipo de estructura
    public void SeleccionarTipo(int Tipo)
    {
        if (GetComponent<Scr_Tablero>().EstaDentro)
        {
            TipoActual = Tipo;
            cont = 0;

        }
    }

    public void ColorBoton(bool Entrada)
    {
        if (GetComponent<Scr_Tablero>().EstaDentro)
        {
            if (Entrada)
            {
                switch (TipoActual)
                {
                    case 0:
                        {
                            if (VerificarCantidades(EstructurasIndustriales[EstructuraActual]))
                            {
                                BotonConstruir.GetComponent<Image>().color = Color.green;
                                BotonConstruir.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
                            }
                            else
                            {
                                BotonConstruir.GetComponent<Image>().color = Color.red;
                                BotonConstruir.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                            }
                            break;
                        }
                    case 1:
                        {
                            if (VerificarCantidades(EstructurasGranja[EstructuraActual]))
                            {
                                BotonConstruir.GetComponent<Image>().color = Color.green;
                                BotonConstruir.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
                            }
                            else
                            {
                                BotonConstruir.GetComponent<Image>().color = Color.red;
                                BotonConstruir.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                            }
                            break;
                        }
                }
            }
            else
            {
                BotonConstruir.GetComponent<Image>().color = Color.white;
                BotonConstruir.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            }
        }
    }

    public void ComprarEstructura()
    {
        if (GetComponent<Scr_Tablero>().EstaDentro)
        {
            switch (TipoActual)
            {
                case 0:
                    {
                        if (VerificarCantidades(EstructurasIndustriales[EstructuraActual]))
                        {
                            EstructurasIndustrialesGuardadas[EstructuraActual] = true;
                            GetComponent<Scr_EventosGuardado>().GuardarEstructurasTablero(TipoActual, EstructuraActual);
                            QuitarObjetos();
                            BotonConstruir.SetActive(false);
                            ImagenConstruido.SetActive(true);
                            ActivarEstructuras(true);

                        }
                        break;
                    }

                case 1:
                    {
                        if (VerificarCantidades(EstructurasGranja[EstructuraActual]))
                        {
                            EstructurasGranjaGuardadas[EstructuraActual] = true;
                            GetComponent<Scr_EventosGuardado>().GuardarEstructurasTablero(TipoActual, EstructuraActual);
                            QuitarObjetos();
                            BotonConstruir.SetActive(false);
                            ImagenConstruido.SetActive(true);
                            ActivarEstructuras(true);

                        }
                        break;
                    }
            }


        }
    }

    private bool VerificarCantidades(Scr_CreadorEstructuras Estructura)
    {
        int ObjetosCumplidos = 0;
        int ObjetoActual = 0;
        foreach (Image ObjetoNecesario in Materiales)
        {
            if (Materiales[ObjetoActual].IsActive())
            {
                int TotalDeObjetos = 0;
                int CasillaActual = 0;
                foreach (string Item in Inventario.CasillasContenido)
                {
                    if (Item.Contains(ObjetoNecesario.sprite.name))
                    {
                        TotalDeObjetos += (int)Inventario.Cantidades[CasillaActual];
                    }
                    CasillaActual++;
                }

                if (TotalDeObjetos / Estructura.Tamaños[ObjetoActual] >= Estructura.Cantidades[ObjetosCumplidos])
                {
                    ObjetosCumplidos++;
                }
                else
                {
                    break;
                }

                ObjetoActual++;
            }

        }

        Debug.Log("Objetos Cumplidos: " + ObjetosCumplidos);
        Debug.Log("Objetos necesarios: " + Estructura.Cantidades.Length);
        if (ObjetosCumplidos >= Estructura.Cantidades.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void QuitarObjetos()
    {
        int MaterialActual = 0;
        foreach (Image Material in Materiales)
        {
            //Total necesario para cada objeto
            int TotalNecesario = int.Parse(Cantidades[MaterialActual].text);


            //En caso de que este activo el material
            if (Material.isActiveAndEnabled)
            {
                int ItemActual = 0;
                foreach (string Objeto in Inventario.CasillasContenido)
                {
                    if (TotalNecesario > 0)
                    {
                        //Si encuentra el material
                        if (Objeto.Contains(Material.sprite.name))
                        {
                            //En caso de tener mas de los que piden
                            if (Inventario.Cantidades[ItemActual] > TotalNecesario)
                            {
                                foreach (Image Casilla in Inventario.Casillas[ItemActual].GetComponent<Scr_CasillaInventario>().CasillasHermanas)
                                {
                                    Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] -= TotalNecesario;
                                }
                                TotalNecesario = 0;
                            }
                            else
                            {
                                //En caso de tener menos
                                if (Inventario.Cantidades[ItemActual] < TotalNecesario)
                                {
                                    bool YaResto = false;
                                    foreach (Image Casilla in Inventario.Casillas[ItemActual].GetComponent<Scr_CasillaInventario>().CasillasHermanas)
                                    {
                                        if (!YaResto)
                                        {
                                            YaResto = true;
                                            TotalNecesario -= (int)Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero];
                                        }
                                        Inventario.CasillasContenido[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = "";
                                        Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = 0;
                                    }
                                }
                                else
                                {
                                    foreach (Image Casilla in Inventario.Casillas[ItemActual].GetComponent<Scr_CasillaInventario>().CasillasHermanas)
                                    {
                                        TotalNecesario = 0;
                                        Inventario.CasillasContenido[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = "";
                                        Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = 0;
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        break;
                    }

                    ItemActual++;
                }
            }
            MaterialActual++;
        }
    }

    private void ActivarEstructuras(bool PrimeraCompra)
    {
        if (PrimeraCompra)
        {
            switch (TipoActual)
            {
                case 0:
                    {
                        ObjEstructurasIndustriales[EstructuraActual].SetActive(true);
                        break;
                    }
                case 1:
                    {
                        ObjEstructurasGranja[EstructuraActual].SetActive(true);
                        break;
                    }
            }
        }
        else
        {
            for (int i = 0; i < EstructurasIndustrialesGuardadas.Length; i++)
            {
                if (EstructurasIndustrialesGuardadas[i])
                {
                    ObjEstructurasIndustriales[i].SetActive(true);
                }
            }
            for (int i = 0; i < EstructurasGranjaGuardadas.Length; i++)
            {
                if (EstructurasGranjaGuardadas[i])
                {
                    ObjEstructurasGranja[i].SetActive(true);
                }
            }
        }
    }
}
