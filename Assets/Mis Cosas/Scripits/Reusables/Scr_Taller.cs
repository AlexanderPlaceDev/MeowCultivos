using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class Scr_Taller : MonoBehaviour
{
    [Header("Plano")]
    [SerializeField] private GameObject Plano;//ui de Plano
    [SerializeField] private GameObject Menu;//ui de Menu

    [Header("Objetos")]
    [SerializeField] private GameObject[] OjetosSelec; //objetos que puede seleccionar
    [SerializeField] private List<int> Ojetosint;// cantidad de objetos que tiene el jugador
    [SerializeField] private GameObject[] BotonesOBJ; //flechas de objetos que puede seleccionar
    [SerializeField] private Sprite[] MarcoSelec; //Marco para mostar que esta seleccionado
    [SerializeField] private GameObject ObjetoPrincipal;//Objeto que muestra
    [SerializeField] private TextMeshProUGUI Seccion; //que seccion del taller muestra
    [SerializeField] private int lugar;// que seccion del taller esta

    [Header("Armas")]
    [SerializeField] private Sprite[] Rango; //Marco para mostar que esta seleccionado
    [SerializeField] private GameObject Slots;// ui de slots de habilidades
    [SerializeField] private GameObject HabilidadesSec;
    [SerializeField] private GameObject[] Habilidades;// ui de slots de habilidades
    [SerializeField] private GameObject[] HabilidadesSelec; //habilidades que puede seleccionar
    [SerializeField] private GameObject[] SlotHabilidad; //Slots de habilidad de la arma selecionada
    [SerializeField] private List<int> Habilidadesint;
    [SerializeField] private GameObject[] BotonesH;
    [SerializeField] private GameObject advertencia;
    [SerializeField] Sprite IconoVacio;
    private int Hablugar;
    private int objetoPrincipalInt =0;
    [SerializeField] private TextMeshProUGUI NombreHabilidad;
    [SerializeField] private TextMeshProUGUI DescripcionHabilidad;
    [SerializeField] private GameObject MostarDescipcion;
    private string htEnEspera; 
    private int cantidadht;
    private string ht;
    private string h1;
    private string h2;
    private string hE;

    private List<int> CantidadObjNessesarios= new List<int>();

    [SerializeField] private Color32[] ColoresBotones;

    GameObject Singleton;

    Scr_DatosArmas Datosarmas;
    public List<Scr_CreadorHabilidadesBatalla> HabilidadesTemporales;
    public List<Scr_CreadorHabilidadesBatalla> HabilidadesPermanentes;
    public List<Scr_CreadorHabilidadesBatalla> HabilidadesEspeciales;

    

    [Header("Crafteo")]
    [SerializeField] public GameObject BotCraftear;
    [SerializeField] public Color PoscicionadoCraft;
    [SerializeField] public Color NormalCraft;
    [SerializeField] private GameObject Crafteo;//ui de crafteo
    [SerializeField] private Scr_CreadorObjetos[] ObjetosACraftear;//objetos que se pueden craftear
    [SerializeField] private GameObject[] CrafteosSelec;


    public List<Scr_CreadorObjetos> ObjetosNessesarios = new List<Scr_CreadorObjetos>();
    private Transform Gata;
    private Scr_Inventario inventario;
    Animator Anim;

    private bool isOpen = false;
    private Scr_ActivadorMenuEstructuraFijo Tablero => GetComponent<Scr_ActivadorMenuEstructuraFijo>();
    // Start is called before the first frame update
    void Start()
    {
        BuscarSingleton();
        checarSeccion();
        Anim = Plano.gameObject.GetComponent<Animator>();
        Gata = GameObject.Find("Gata").transform;
        inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Tablero.EstaDentro)
        {
            abrirPlano();
        }
        else
        {
            cerrarPlano();
        }
    }
    private void OnEnable()
    {
        Anim = Plano.gameObject.GetComponent<Animator>();
        BuscarSingleton();
        checarSeccion();
    }
    public void abrirPlano()
    {
        if (isOpen) return;
        StartCoroutine(EsperarAbrir(1f));
        StartCoroutine(EsperarAbrirMenu(1.4f));
        isOpen = true;
    }
    IEnumerator EsperarAbrir(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        Anim.Play("Abriendo"); 
    }
    IEnumerator EsperarAbrirMenu(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        Menu.SetActive(true);
    }
    public void Craftear_ON()
    {
        BotCraftear.GetComponent<Image>().color = PoscicionadoCraft;
    }
    public void Craftear_Exit()
    {
        BotCraftear.GetComponent<Image>().color = NormalCraft;
    }
    public void cerrarPlano()
    {
        if (!isOpen) return;
        Anim.Play("Cerrando");
        Menu.SetActive(false);
        isOpen = false;
    }
    private void BuscarSingleton()
    {
        if (Singleton == null)
        {
            Singleton = GameObject.Find("Singleton");
            Datosarmas = Singleton.GetComponent<Scr_DatosArmas>();
        }
    }
    //cuando seleccione cambia el contorno
    public void objeto_ON(int i)
    {
        //Escondervertencia();
        OjetosSelec[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = MarcoSelec[1]; 
        MostarDescipcion.SetActive(false);
    }
    public void objeto_Exit(int i)
    {
        //Escondervertencia();
        OjetosSelec[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = MarcoSelec[0];
        MostarDescipcion.SetActive(false);
    }

    public void slot_ON(int i)
    {
        //Escondervertencia();
        SlotHabilidad[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = MarcoSelec[1];
        MostarDescipcion.SetActive(false);
        ObjetoPrincipal.SetActive(true);
    }
    public void slot_Exit(int i)
    {
        //Escondervertencia();
        SlotHabilidad[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = MarcoSelec[0];
        MostarDescipcion.SetActive(false);
        ObjetoPrincipal.SetActive(true);
    }
    public void habilidad_Exit(int i)
    {
        //Escondervertencia();
        HabilidadesSelec[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = MarcoSelec[0];
        MostarDescipcion.SetActive(false);
        ObjetoPrincipal.SetActive(true);
    }
    //cuando seleccione cambia el contorno
    public void Habilidad_ON(int i)
    {
        //Escondervertencia();
        HabilidadesSelec[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = MarcoSelec[1];
        MostarDescipcion.SetActive(true);

        ObjetoPrincipal.SetActive(false);
        int habS = Habilidadesint[i];
        switch (Hablugar)
        {
            case 0:
                NombreHabilidad.text = HabilidadesTemporales[habS].Nombre;
                DescripcionHabilidad.text = HabilidadesTemporales[habS].Descripcion;
                break;
            case 1:
                NombreHabilidad.text = HabilidadesPermanentes[habS].Nombre;
                DescripcionHabilidad.text = HabilidadesPermanentes[habS].Descripcion;
                break;
            case 2:
                NombreHabilidad.text = HabilidadesPermanentes[habS].Nombre;
                DescripcionHabilidad.text = HabilidadesPermanentes[habS].Descripcion;
                break;
            case 3:
                NombreHabilidad.text = HabilidadesPermanentes[habS].Nombre;
                DescripcionHabilidad.text = HabilidadesPermanentes[habS].Descripcion;
                break;
        }
    }
    //muestra los objetos, habilidades o armas dependiendo de la seccion
    public void mostrarObj(int i)
    {
        Escondervertencia();
        MostarDescipcion.SetActive(false);
        checar_HabilidadesTemporales();
        checar_HabilidadesPermanentes();
        int objShow = Ojetosint[i];
        switch (lugar)
        {
            case 0:
                ObjetoPrincipal.SetActive(true); 
                objetoPrincipalInt = objShow;
                ObjetoPrincipal.transform.GetChild(0).gameObject.SetActive(true);
                ObjetoPrincipal.transform.GetChild(0).GetComponent<Image>().sprite = ObjetosACraftear[objShow].Icono; 
                Slots.SetActive(false);
                Crafteo.SetActive(true);
                HabilidadesSec.SetActive(false);
                mostraRecursosOBJ();
                break;

            case 1:
                ObjetoPrincipal.SetActive(true);
                objetoPrincipalInt = objShow;
                ObjetoPrincipal.transform.GetChild(0).gameObject.SetActive(true);
                ObjetoPrincipal.transform.GetChild(0).GetComponent<Image>().sprite = HabilidadesTemporales[objShow].Icono;
                Slots.SetActive(false);
                Crafteo.SetActive(true);
                HabilidadesSec.SetActive(false);
                mostraRecursosTemp();
                break;

            case 2:
                ObjetoPrincipal.SetActive(true);
                ObjetoPrincipal.transform.GetChild(0).gameObject.SetActive(true);
                ObjetoPrincipal.transform.GetChild(1).gameObject.SetActive(true);
                int e = PlayerPrefs.GetInt("Rango " + Datosarmas.TodasLasArmas[objShow].Nombre, 1);
                Debug.Log(e);
                ObjetoPrincipal.transform.GetChild(0).GetComponent<Image>().sprite = Datosarmas.TodasLasArmas[objShow].Icono;
                ObjetoPrincipal.transform.GetChild(1).GetComponent<Image>().sprite = Rango[e];
                objetoPrincipalInt = objShow;
                Slots.SetActive(true);
                Crafteo.SetActive(false);
                HabilidadesSec.SetActive(false);
                mostrarArmasHabilidades();
                break;
        }
    }

    
    public void mostrarPrincipalDescripccion()
    {
        MostarDescipcion.SetActive(true);
        ObjetoPrincipal.SetActive(false);
        switch (lugar)
        {
            case 0:
                NombreHabilidad.text = ObjetosACraftear[objetoPrincipalInt].Nombre;
                DescripcionHabilidad.text = ObjetosACraftear[objetoPrincipalInt].Descripcion;
                break;

            case 1:
                NombreHabilidad.text = HabilidadesTemporales[objetoPrincipalInt].Nombre;
                DescripcionHabilidad.text = HabilidadesTemporales[objetoPrincipalInt].Descripcion;
                break;

            case 2:

                NombreHabilidad.text = Datosarmas.TodasLasArmas[objetoPrincipalInt].Nombre;
                DescripcionHabilidad.text = Datosarmas.TodasLasArmas[objetoPrincipalInt].Descripcion;
                break;
        }
    }
    public void OcultarPrincipalDescripccion()
    {
        MostarDescipcion.SetActive(false);
        ObjetoPrincipal.SetActive(true);
    }
    //Singleton.GetComponent<Scr_DatosArmas>().ArmasDesbloqueadas[i]

    //checa que seccion estan
    public void checarSeccion()
    {
        ObjetoPrincipal.transform.GetChild(0).gameObject.SetActive(false);
        ObjetoPrincipal.transform.GetChild(1).gameObject.SetActive(false);  
        Slots.SetActive(false);
        Crafteo.SetActive(false);
        MostarDescipcion.SetActive(false);
        Escondervertencia();
        switch (lugar)
        {
            case 0:
                Seccion.text = "Objetos";
                checar_objetosCrafteables();
                break;

            case 1:
                Seccion.text = "Gadgets";
                checar_HabilidadesTemporales();
                mostrar_HabilidadesCrafteables();
                break;

            case 2:
                Seccion.text = "Armas";
                checar_armasActivas();
                break;
        }
    }
    //cambia de seccion de lado Izquierda
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
    //cambia de seccion de lado derecha
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

    //cambia el objeto u habilidad dependiendo de la seccion pero desde lado derecho
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
    }

    //cambia el objeto u habilidad dependiendo de la seccion pero desde lado izquierdo
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
    }

    //checa el el objeto u habilidad 
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
                    OjetosSelec[i].GetComponent<Image>().sprite = Datosarmas.HabilidadesTemporales[Ojetosint[i]].Icono;
                    break;
                case 2:
                    OjetosSelec[i].GetComponent<Image>().sprite = Datosarmas.TodasLasArmas[Ojetosint[i]].Icono;
                    break;
            }
        }
    }

    //checa el el objeto que se pueden craftear
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
    //mostrar los objetos que necesita para crear el objeto
    public void mostraRecursosOBJ()
    {
        BotCraftear.SetActive(false);
        htEnEspera = ObjetosACraftear[objetoPrincipalInt].Nombre;
        if (ObjetosNessesarios != null)
        {
            ObjetosNessesarios.Clear();
        }
        if (CantidadObjNessesarios != null)
        {
            CantidadObjNessesarios.Clear();
        }
        Scr_CreadorObjetos objetocraftear = ObjetosACraftear[objetoPrincipalInt];
        for (int i = 0; i < objetocraftear.MaterialesDeProduccion.Length; i++)
        {
            if (objetocraftear.MaterialesDeProduccion[i] != null)
            {
                Scr_CreadorObjetos obj = objetocraftear.MaterialesDeProduccion[i];
                int cantobjt = objetocraftear.CantidadMaterialesDeProduccion[i];
                Debug.Log(obj);
                ObjetosNessesarios.Add(obj);
                CantidadObjNessesarios.Add(cantobjt);

            }
        }
        esconderCrafteosSelec();
        int Selec = CrafteosSelec.Length;
        int Chec = 0;
        if (ObjetosNessesarios.Count<= CrafteosSelec.Length)
        {
            Selec=ObjetosNessesarios.Count;
        }
        for (int i = 0; i < Selec; i++)
        {
            if (ObjetosNessesarios[i] !=null)
            {
                CrafteosSelec[i].SetActive(true);
                CrafteosSelec[i].GetComponent<Image>().sprite = ObjetosNessesarios[i].Icono;
                CrafteosSelec[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = CantidadObjNessesarios[i].ToString();
                bool tieneMateriales = CalcularObjetos(
                        ObjetosNessesarios[i],
                        CantidadObjNessesarios[i]
                    );
                CrafteosSelec[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = tieneMateriales ? Color.white : Color.red;
                if (tieneMateriales)
                {
                    Chec++;
                }
            }
        }
        if (Chec == Selec)
        {
            BotCraftear.SetActive(true);
        }
    }

    public void esconderCrafteosSelec()
    {
        for (int i = 0; i < CrafteosSelec.Length; i++)
        {
            CrafteosSelec[i].SetActive(false);
            
        }
    }

    //mostrar los objetos que necesita para crear la habilidad
    public void mostraRecursosTemp()
    {
        BotCraftear.SetActive(false);
        htEnEspera = HabilidadesTemporales[objetoPrincipalInt].Nombre;
        if (ObjetosNessesarios != null)
        {
            ObjetosNessesarios.Clear();
        }
        if (CantidadObjNessesarios != null)
        {
            CantidadObjNessesarios.Clear();
        }
        for (int i = 0; i < HabilidadesTemporales[objetoPrincipalInt].ItemsRequeridos.Length; i++)
        {
            ObjetosNessesarios.Add(HabilidadesTemporales[objetoPrincipalInt].ItemsRequeridos[i]);
            CantidadObjNessesarios.Add(HabilidadesTemporales[objetoPrincipalInt].CantidadesRequeridas[i]);
        }
        esconderCrafteosSelec();
        int Selec = CrafteosSelec.Length;
        int Chec = 0;
        if (ObjetosNessesarios.Count <= CrafteosSelec.Length)
        {
            Selec = ObjetosNessesarios.Count;
        }
        for (int i = 0; i < Selec; i++)
        {
            if (ObjetosNessesarios[i] != null)
            {
                CrafteosSelec[i].SetActive(true);
                CrafteosSelec[i].GetComponent<Image>().sprite = ObjetosNessesarios[i].Icono;
                CrafteosSelec[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = CantidadObjNessesarios[i].ToString();
                bool tieneMateriales = CalcularObjetos(
                        ObjetosNessesarios[i],
                        CantidadObjNessesarios[i]
                    );
                CrafteosSelec[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = tieneMateriales ? Color.white : Color.red;
                if (tieneMateriales)
                {
                    Chec++;
                }
            }
        }
        if (Chec == Selec)
        {
            BotCraftear.SetActive(true);
        }
    }
    private bool CalcularObjetos(Scr_CreadorObjetos objetosNecesarios, int cantidadesNecesarias)
    {
        for (int j = 0; j < inventario.Objetos.Length; j++)
        {
            if (inventario.Objetos[j].Nombre == objetosNecesarios.Nombre && inventario.Cantidades[j] >= cantidadesNecesarias)
            {
                return true;
            }
        }
        return false;
    }
    public void craftearObjeto()
    {
        switch (lugar)
        {
            case 0:
                quitarobjetos();
                inventario.AgregarObjeto(1, htEnEspera);
                mostraRecursosOBJ();
                break;
            case 1:
                Datosarmas.AgregarUsosTemporales(htEnEspera);
                quitarobjetos();
                mostraRecursosTemp();
                break;
        }
    }

    public void quitarobjetos()
    {
        for (int i = 0; i < ObjetosNessesarios.Count; i++)
        {
            inventario.QuitarObjeto(CantidadObjNessesarios[i], ObjetosNessesarios[i].Nombre);
        }
    }
    //checa las habilidades dependiendo del slot seleccionado
    public void checar_Habilidades(int hab)
    {
        //Ojetosint.Clear();
        //Aparecer_botones();
        HabilidadesSec.SetActive(true);
        switch (hab)
        {
            case 0:
                mostrar_HabilidadesTemporales();
                Hablugar = 0;
                break;
            case 1:
                mostrar_HabilidadesPermanentes();
                Hablugar = 1;
                break;
            case 2:
                mostrar_HabilidadesPermanentes();
                Hablugar = 2;
                break;
            case 3:
                mostrar_HabilidadesEspeciales();
                Hablugar = 3;
                break;
        }
    }

    //mueve las habilidades que tienen
    public void cambiarHabIzq()
    {
        if (Habilidadesint == null || Habilidadesint.Count <= 1) return;

        int primerValor = Habilidadesint[0];

        for (int i = 0; i < Habilidadesint.Count - 1; i++)
        {
            Habilidadesint[i] = Habilidadesint[i + 1];
        }

        Habilidadesint[Habilidadesint.Count - 1] = primerValor;
        checarIconosHab();
    }
    //mueve las habilidades que tienen
    public void cambiarHabDer()
    {
        if (Habilidadesint == null || Habilidadesint.Count <= 1) return;

        int ultimoValor = Habilidadesint[Habilidadesint.Count - 1];

        for (int i = Habilidadesint.Count - 1; i > 0; i--)
        {
            Habilidadesint[i] = Habilidadesint[i - 1];
        }

        Habilidadesint[0] = ultimoValor;
        checarIconosHab();
    }
    //checa las habilidades que deben de mostrar
    private void checarIconosHab()
    {
        for (int i = 0; i < 3; i++)
        {
            switch (Hablugar)
            {
                case 0:
                    HabilidadesSelec[i].GetComponent<Image>().sprite = HabilidadesTemporales[Habilidadesint[i]].Icono;
                    break;
                case 1:
                    HabilidadesSelec[i].GetComponent<Image>().sprite = Datosarmas.HabilidadesPermanentes[Habilidadesint[i]].Icono;
                    break;
                case 2:
                    HabilidadesSelec[i].GetComponent<Image>().sprite = Datosarmas.HabilidadesPermanentes[Habilidadesint[i]].Icono;
                    break;
                case 3:
                    HabilidadesSelec[i].GetComponent<Image>().sprite = Datosarmas.HabilidadesPermanentes[Habilidadesint[i]].Icono;
                    break;
            }
        }
    }

    //Muestra las habilidades Temporales que deben de mostrar
    public void mostrar_HabilidadesTemporales()
    {
        Habilidadesint.Clear();
        escoonder_botonesH();
        int show = 0;
        for (int i = 0; i < HabilidadesTemporales.Count; i++)
        {
            if (HabilidadesTemporales[i].Arma== Datosarmas.TodasLasArmas[objetoPrincipalInt].Nombre || HabilidadesTemporales[i].Arma== "Todos")
            {
                if(ht != HabilidadesTemporales[i].Nombre)
                {
                    if (show < 3 && Datosarmas.UsosHabilidadesT[i] > 0)
                    {
                        HabilidadesSelec[show].SetActive(true);
                        HabilidadesSelec[show].GetComponent<Image>().sprite = HabilidadesTemporales[i].Icono;
                        show++;
                    }
                    Habilidadesint.Add(i);
                }
            }
            
        }
        if (Habilidadesint.Count > 3)
        {
            Aparecer_botonesH();
        }
    }
    public void mostrar_HabilidadesCrafteables()
    {
        Ojetosint.Clear();
        escoonder_botones();
        int show = 0;
        if (HabilidadesTemporales.Count > 6)
        {
            Aparecer_botones();
        }
        for (int i = 0; i < HabilidadesTemporales.Count; i++)
        {
            if (show < 6)
            {
                OjetosSelec[show].SetActive(true);
                OjetosSelec[show].GetComponent<Image>().sprite = HabilidadesTemporales[i].Icono;
                show++;
            }
            Ojetosint.Add(i);
        }
    }
    //checa las habilidades temporales que deben de mostrar
    public void checar_HabilidadesTemporales()
    {
        HabilidadesTemporales.Clear();
        for (int i = 0; i < Datosarmas.HabilidadesTemporales.Length; i++) 
        {
            if (Datosarmas.HabilidatTDesbloqueadas[i])
            {
                HabilidadesTemporales.Add(Datosarmas.HabilidadesTemporales[i]);
            }
        }
    }
    //muestra las habilidades permanentes que deben de mostrar
    public void mostrar_HabilidadesPermanentes()
    {
        Habilidadesint.Clear();
        escoonder_botonesH();
        int show = 0;

        for (int i = 0; i < HabilidadesPermanentes.Count; i++)
        {
            if (HabilidadesPermanentes[i].Arma == Datosarmas.TodasLasArmas[objetoPrincipalInt].Nombre || HabilidadesPermanentes[i].Arma == "Todos")
            {
                if(h1 != HabilidadesPermanentes[i].Nombre && h2 != HabilidadesPermanentes[i].Nombre)
                {
                    if (HabilidadesPermanentes[i].Tipo == "Normal" || HabilidadesPermanentes[i].Tipo == "Pasiva")
                    {
                        if (show < 3)
                        {
                            HabilidadesSelec[show].SetActive(true);
                            HabilidadesSelec[show].GetComponent<Image>().sprite = HabilidadesPermanentes[i].Icono;
                            show++;
                        }
                        Habilidadesint.Add(i);
                    }
                }
            }

        }
        if (Habilidadesint.Count > 3)
        {
            Aparecer_botonesH();
        }
    }
    //muestra las habilidades especiales que deben de mostrar
    public void mostrar_HabilidadesEspeciales()
    {
        Habilidadesint.Clear();
        escoonder_botonesH();
        int show = 0;
        for (int i = 0; i < HabilidadesPermanentes.Count; i++)
        {
            if (HabilidadesPermanentes[i].Arma == Datosarmas.TodasLasArmas[objetoPrincipalInt].Nombre || HabilidadesPermanentes[i].Arma == "Todos")
            {
                if(hE != HabilidadesPermanentes[i].Nombre)
                {
                    if (HabilidadesPermanentes[i].Tipo == "Especial")
                    {
                        if (show < 3)
                        {
                            HabilidadesSelec[show].SetActive(true);
                            HabilidadesSelec[show].GetComponent<Image>().sprite = HabilidadesPermanentes[i].Icono;
                            show++;
                        }
                        Habilidadesint.Add(i);
                    }
                }
            }
        }
        if (Habilidadesint.Count > 3)
        {
            Aparecer_botonesH();
        }
    }
    //checa las habilidades permantentes que deben de mostrar
    public void checar_HabilidadesPermanentes()
    {
        HabilidadesPermanentes.Clear();
        for (int i = 0; i < Datosarmas.HabilidadesPermanentes.Length; i++)
        {
            if (Datosarmas.HabilidatPDesbloqueadas[i])
            {
                HabilidadesPermanentes.Add(Datosarmas.HabilidadesPermanentes[i]);
            }
        }
    }
    public void NuevaHabilidad(int hab)
    {
        int habS=Habilidadesint[hab];
        switch (Hablugar)
        {
            case 0:
                htEnEspera = HabilidadesTemporales[habS].Nombre;
                cantidadht = HabilidadesTemporales[habS].Usos;
                NuevaHabilidadTemporal();
                break;
            case 1:
                h1 = HabilidadesPermanentes[habS].Nombre;
                guardarArmasHabilidades();
                break;
            case 2:
                h2 = HabilidadesPermanentes[habS].Nombre;
                guardarArmasHabilidades();
                break;
            case 3:
                hE = HabilidadesPermanentes[habS].Nombre;
                guardarArmasHabilidades();
                break;
        }

        HabilidadesSec.SetActive(false);
        mostrarArmasHabilidades();
    }

    public void NuevaHabilidadTemporal()
    {
        string arma = Datosarmas.TodasLasArmas[objetoPrincipalInt].Nombre;
        if (PlayerPrefs.GetString(arma + "HT", "Nada")== "Nada")
        {
            ht = htEnEspera;
            Datosarmas.QuitarUsosTemporales(htEnEspera); 
            guardarArmasHabilidades();
        }
        else
        {
            mostraradevertencia();
        }
    }

    public void AceptarHabilidadTemporal()
    {
        ht = htEnEspera;
        Datosarmas.QuitarUsosTemporales(htEnEspera);
        guardarArmasHabilidades();
        Escondervertencia(); 
        mostrarArmasHabilidades();
    }
    public void mostraradevertencia()
    {
        advertencia.SetActive(true);
        HabilidadesSec.SetActive(false);
    }

    public void Escondervertencia()
    {
        cantidadht = 0;
        advertencia.SetActive(false);
        HabilidadesSec.SetActive(true);
    }
    public void mostrarArmasHabilidades()
    {
        string arma = Datosarmas.TodasLasArmas[objetoPrincipalInt].Nombre;
        //cantidadht = PlayerPrefs.GetInt(arma + "Usos", 0);
        Debug.LogWarning(PlayerPrefs.GetString(arma + "HT", "Nada"));
        ht = PlayerPrefs.GetString(arma + "HT", "Nada");
        h1 = PlayerPrefs.GetString(arma + "H1", "Ojo");
        h2 = PlayerPrefs.GetString(arma + "H2", "Rugido");
        hE = PlayerPrefs.GetString(arma + "HE", "Garras");

        Scr_CreadorHabilidadesBatalla HabT = Datosarmas.BuscarHabilidadTemporalPorNombre(ht);
        Scr_CreadorHabilidadesBatalla Hab1 = Datosarmas.BuscarHabilidadPermanentePorNombre(h1);
        Scr_CreadorHabilidadesBatalla Hab2 = Datosarmas.BuscarHabilidadPermanentePorNombre(h2);
        Scr_CreadorHabilidadesBatalla HabE = Datosarmas.BuscarHabilidadPermanentePorNombre(hE);

        if(ht== "Nada")
        {
            Habilidades[0].GetComponent<Image>().sprite = IconoVacio;
        }
        else
        {
            Habilidades[0].GetComponent<Image>().sprite = HabT.Icono;
        }
        Habilidades[1].GetComponent<Image>().sprite = Hab1.Icono;
        Habilidades[2].GetComponent<Image>().sprite = Hab2.Icono;
        Habilidades[3].GetComponent<Image>().sprite = HabE.Icono;
    }

    public void guardarArmasHabilidades()
    {
        string arma = Datosarmas.TodasLasArmas[objetoPrincipalInt].Nombre;
        PlayerPrefs.SetString(arma + "HT", ht);
        PlayerPrefs.SetString(arma + "H1", h1);
        PlayerPrefs.SetString(arma + "H2", h2);
        PlayerPrefs.SetString(arma + "HE", hE);

        PlayerPrefs.SetInt(arma + "Usos", cantidadht);
        Datosarmas.guardarHabilidades();
    }
    //checa las armas que deben de mostrar
    public void checar_armasActivas()
    {
        Ojetosint.Clear();
        if (Datosarmas.ArmasDesbloqueadas.Length <6)
        {
            escoonder_botones();
            for (int i = 0; i < Datosarmas.ArmasDesbloqueadas.Length; i++)
            {
                if (Datosarmas.ArmasDesbloqueadas[i])
                {
                    OjetosSelec[i].SetActive(true);
                    OjetosSelec[i].GetComponent<Image>().sprite = Datosarmas.TodasLasArmas[i].Icono;
                    Ojetosint.Add(i);
                }
            }
        }
        else
        {
            Aparecer_botones();
            for (int i = 0; i < Datosarmas.ArmasDesbloqueadas.Length; i++)
            {
                if (Datosarmas.ArmasDesbloqueadas[i])
                {
                    if (i < 6)
                    {
                        OjetosSelec[i].GetComponent<Image>().sprite = Datosarmas.TodasLasArmas[i].Icono;
                    }
                    Ojetosint.Add(i);
                }
            }
        }
    }

    //esconde las flechas de las habilidades solo en caso de que sean menos de 3
    private void escoonder_botonesH()
    {
        for (int i = 0; i < 3; i++)
        {
            HabilidadesSelec[i].SetActive(false);
        }
        for (int i = 0; i < BotonesOBJ.Length; i++)
        {
            BotonesH[i].SetActive(false);
        }
    }
    //aparcec las flechas de las habilidades solo en caso de que sean mas de 3
    private void Aparecer_botonesH()
    {
        for (int i = 0; i < BotonesOBJ.Length; i++)
        {
            BotonesH[i].SetActive(true);
        }
    }

    //esconde las flechas de los objetos o las habilidades solo en caso de que sean menos de 6
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
    //esconde las flechas de los objetos o las habilidades solo en caso de que sean mas de 6
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
