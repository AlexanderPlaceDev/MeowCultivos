using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MisionesSecundrias_UI : MonoBehaviour
{
    public GameObject MisionUIprefab;
    public Transform contentPanel;

    public Color32 PrincipalC;
    public Color32 SecundarioC;


    public Color32 Vacio;

    public List<Scr_CreadorDialogos> Misiones;

    public GameObject Npc;

    public Scr_ActivadorDialogos dialogoNPC;
    public Scr_SistemaDialogos activardialogoNPC;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void conseguirNPC(GameObject np)
    {
        Npc=np;
        dialogoNPC=Npc.GetComponent<Scr_ActivadorDialogos>();
        activardialogoNPC = Npc.GetComponent<Scr_SistemaDialogos>();
    }
    public void cerrar()
    {
        if (dialogoNPC != null)
        {
            dialogoNPC.cerrarMisionesSecundaris();
        }
    }
    public void coseguir_misiones(List<Scr_CreadorDialogos> mis)
    {
        Misiones.Clear();
        for (int i = 0; i < mis.Count; i++)
        {
            Misiones.Add(mis[i]);
        }
        MostrarMisiones();
    }
    public void MostrarMisiones()
    {
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        for (int i = 0; i < Misiones.Count; i++)
        {
            Scr_CreadorDialogos instance = Misiones[i];
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

            Image img = obj.GetComponent<Image>();

            if (instance.Mision.prioridad == Scr_CreadorMisiones.prioridadM.Principal)
            {
                img.color = PrincipalC;
            }
            else
            {
                img.color = SecundarioC;
            }
            Button[] botons = obj.GetComponentsInChildren<Button>();
            Button boton = botons[0];


            texto1.text = $"{instance.name}";
            
            texto2.text = $"{instance.Mision.DescripcionFinal}";
            TextMeshProUGUI Textbut = boton.GetComponentInChildren<TextMeshProUGUI>();
            Textbut.text = "Activar";
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(() =>
            {
                activardialogoNPC.DialogoSecundario = instance;
                activardialogoNPC.IniciarDialogo();
                dialogoNPC.opcionesUI();
            });
        }
    }
}
