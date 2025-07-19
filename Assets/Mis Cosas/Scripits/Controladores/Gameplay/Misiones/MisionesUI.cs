using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class MisionesUI : MonoBehaviour
{
    public GameObject MisionUIprefab;
    public Transform contentPanel;

    public List<Scr_CreadorMisiones> MisionesenCurso;

    public List<bool> MisionesenCursoCompletadas;
    private Scr_ControladorMisiones ControladorMisiones;
    public GameObject mis;

    public Color32 PrincipalC;

    public Color32 SecundarioC;


    public Color32 Vacio;

    TextMeshProUGUI MisionActual;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("Gata"))
        {
            ControladorMisiones = GameObject.Find("Gata").transform.GetChild(4).GetComponent<Scr_ControladorMisiones>();
            checarMisionActual();
            checarMisionesCurso();
            MostrarMisiones();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            checarMisionesCurso();
            MostrarMisiones();
        }
    }
    void OnEnable()
    {
        Debug.Log("El GameObject se ha activado");
        // Aquí puedes llamar a tu función personalizada
        checarMisionesCurso();
    }
    public void checarMisionesCurso()
    {
        //actualiza las misiones que tiene el jugador en su controlador inventario
        MisionesenCurso.Clear();
        MisionesenCursoCompletadas.Clear();
        //ControladorMisiones.revisarMisionPrincipal();
        //ControladorMisiones.RevisarTodasLasMisionesSecundarias();
        if (ControladorMisiones.MisionPrincipal != null)
        {
            MisionesenCurso.Add(ControladorMisiones.MisionPrincipal);
            MisionesenCursoCompletadas.Add(ControladorMisiones.MisionPCompleta);
        }

        for (int i = 0; i < ControladorMisiones.MisionesSecundarias.Count; i++)
        {
            MisionesenCurso.Add(ControladorMisiones.MisionesSecundarias[i]);
            MisionesenCursoCompletadas.Add(ControladorMisiones.MisionesScompletas[i]);
        }
        Debug.Log("Revisado");
    }
    public void checarMisionActual()
    {
        //cambia la mision principal la cual se acualiza en el controlador
        TextMeshProUGUI texto1;
        TextMeshProUGUI texto2;
        TextMeshProUGUI[] textos = mis.GetComponentsInChildren<TextMeshProUGUI>();

        if (textos.Length >= 2)
        {
            texto1 = textos[0];
            texto2 = textos[1];
        }
        else
        {
            texto1 = null;
            texto2 = null;
        }

        //marca si es principal o secundaria
        Image img = mis.GetComponent<Image>();
        if (ControladorMisiones.MisionActual !=null && ControladorMisiones.MisionActual.prioridad == Scr_CreadorMisiones.prioridadM.Principal)
        {
            img.color = PrincipalC;
        }
        else if (ControladorMisiones.MisionActual != null && ControladorMisiones.MisionActual.prioridad == Scr_CreadorMisiones.prioridadM.Secundaria)
        {
            img.color = SecundarioC;
        }
        else
        {
            img.color = Vacio;
        }
        //comprueba si ya esta completa o no para poder elejir la descripcion
        if (ControladorMisiones.MisionActual!= null)
        {
            if (!ControladorMisiones.MisionActualCompleta)
            {
                texto2.text = $"{ControladorMisiones.MisionActual.Descripcion}";
            }
            else
            {
                texto2.text = $"{ControladorMisiones.MisionActual.DescripcionCompleta}";
            }
            texto1.text = $"{ControladorMisiones.MisionActual.name}";
        }
        else
        {
            texto1.text = "No Hay Objetivo";
            texto2.text ="";
        }
        
        
    }
    //muestra las misiones con su respectivo boton para marcarla como la actual
    void MostrarMisiones()
    {
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        for (int i = 0; i < MisionesenCurso.Count; i++)
        {
            Scr_CreadorMisiones instance = MisionesenCurso[i];
            //string misionname= instance.name;
            //MisionesData baseData = MisionesData.GetByName(misionname);
            if (instance == null) continue;

            GameObject obj = Instantiate(MisionUIprefab, contentPanel);
            TextMeshProUGUI texto1;
            TextMeshProUGUI texto2;
            TextMeshProUGUI[] textos = obj.GetComponentsInChildren<TextMeshProUGUI>();

            if (textos.Length >= 2)
            {
                texto1 = textos[0];
                texto2 = textos[1];
            }
            else
            {
                texto1 = null;
                texto2 = null;
            }
            //marca si es principal o secundaria
            Image img = obj.GetComponent<Image>();
            
            if (instance.prioridad == Scr_CreadorMisiones.prioridadM.Principal)
            {
                img.color = PrincipalC;
            }
            else
            {
                img.color = SecundarioC;
            }
            Button[] botons = obj.GetComponentsInChildren<Button>();
            Button boton = botons[0];

            //comprueba si ya esta completa o no para poder elejir la descripcion
            texto1.text = $"{instance.MisionName}";
            if (!MisionesenCursoCompletadas[i])
            {

                texto2.text = $"{instance.Descripcion}";
            }
            else
            {
                texto2.text = $"{instance.DescripcionCompleta}";
            }
            TextMeshProUGUI Textbut= boton.GetComponentInChildren<TextMeshProUGUI>();
            Textbut.text = "Seguir";
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(() =>
            {
                //ControladorMisiones.SeleccionMisionActual(instance, MisionesenCursoCompletadas[i-1]);
                checarMisionActual();
            });
        }
    }
}
