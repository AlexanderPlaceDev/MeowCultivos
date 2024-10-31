using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_DialogosRandomBatalla : MonoBehaviour
{
    [SerializeField] string[] Dialogos;
    [SerializeField] GameObject Conjunto;
    [SerializeField] float FrecuenciaDialogos;
    float Contador = 0;
    void Start()
    {
    }

    void Update()
    {
        Contador += Time.deltaTime;
        if (Contador >= FrecuenciaDialogos)
        {
            Contador = 0;

            int r = Random.Range(0, Dialogos.Length+1);
            Debug.Log("R:" + r + " Cant:" + Dialogos.Length);

            if (r < Dialogos.Length)
            {
                Conjunto.SetActive(true);
                Conjunto.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Dialogos[r];
            }
            else
            {
                Conjunto.SetActive(false);
            }

        }
    }
}
