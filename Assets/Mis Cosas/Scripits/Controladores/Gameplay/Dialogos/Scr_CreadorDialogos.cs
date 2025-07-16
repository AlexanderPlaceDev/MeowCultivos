using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CrearObjeto", menuName = "Crear Objeto/Dialogo", order = 0)]
public class Scr_CreadorDialogos : ScriptableObject
{
    public string NombreDialogo;
    public int NumeroDialogo;
    public bool EsUnico;
    public bool EsMisionPrincipal;
    public bool EsMisionSecundaria;
    public Scr_CreadorMisiones Mision;
    [SerializeField,TextArea(4,6)] public string[] Lineas;
    [SerializeField,TextArea(4,6)] public string[] LineasSecundarias;
}
