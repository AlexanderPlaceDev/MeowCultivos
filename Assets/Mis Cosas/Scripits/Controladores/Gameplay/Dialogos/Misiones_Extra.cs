using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Misiones_Extra : MonoBehaviour
{
    public GameObject MisionUIprefab;
    public Transform contentPanel;


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
        Npc = np;
        dialogoNPC = Npc.GetComponent<Scr_ActivadorDialogos>();
        activardialogoNPC = Npc.GetComponent<Scr_SistemaDialogos>();
    }
    public void cerrar()
    {
        if (dialogoNPC != null)
        {
            dialogoNPC.cerrarHablar_extra();
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

            Button[] botons = obj.GetComponentsInChildren<Button>();
            Button boton = botons[0];


            TextMeshProUGUI Textbut = boton.GetComponentInChildren<TextMeshProUGUI>();
            Textbut.text = $"{instance.NombreDialogo}";
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(() =>
            {
                activardialogoNPC.DialogoExtra = instance;
                activardialogoNPC.DiaExtra = true;
                activardialogoNPC.IniciarDialogo();
                dialogoNPC.opcionesUI2();
                //activardialogoNPC.DiaExtra = false;
            });
        }
    }
}
