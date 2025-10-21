using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Scr_ActivadorParticular : MonoBehaviour
{
    [SerializeField]
    private bool ActivaDespuesDeCinematica;
    [SerializeField]
    private string NombreCinematica;
    [SerializeField]
    GameObject[] ObjetosAEncender;
    [SerializeField]
    GameObject[] ObjetosAApagar;
    [SerializeField]
    private NavMeshSurface Nav;


    private void OnEnable()
    {
        if (ActivaDespuesDeCinematica)
        {
            if (PlayerPrefs.GetString("Cinematica " + NombreCinematica, "No") == "Si")
            {
                foreach (GameObject obj in ObjetosAEncender) { obj.SetActive(true); }
                foreach (GameObject obj in ObjetosAApagar) { obj.SetActive(false); }
            }
            else
            {
                foreach (GameObject obj in ObjetosAEncender) { obj.SetActive(false); }
            }
        }
        if (Nav != null)
        {
            Nav.BuildNavMesh();
        }
    }
}
