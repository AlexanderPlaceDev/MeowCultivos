using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorMapas : MonoBehaviour
{
    [SerializeField] public bool EsMapa;
    [SerializeField] GameObject[] MapasQueActiva;
    [SerializeField] GameObject[] MapasQueDesactiva;
    [SerializeField] string NombreMapaBatalla;
    [SerializeField] Material SkyBoxDia;
    [SerializeField] Material SkyBoxNoche;

    private void Start()
    {
        if (EsMapa)
        {
            // 🔥 Sincroniza los mapas con PlayerPrefs al inicio
            CargarEstadoMapas();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Gata")
        {
            // Configura datos del singleton
            var singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
            singleton.NombreMapa = NombreMapaBatalla;
            singleton.SkyBoxDia = SkyBoxDia;
            singleton.SkyBoxNoche = SkyBoxNoche;

            PlayerPrefs.SetString("Mapa Actual", NombreMapaBatalla);

            // Activa los mapas necesarios
            foreach (GameObject mapa in MapasQueActiva)
            {
                if (!mapa.activeSelf)
                    mapa.SetActive(true);
            }

            // Desactiva los mapas que ya no se necesitan
            foreach (GameObject mapa in MapasQueDesactiva)
            {
                if (mapa.activeSelf)
                    mapa.SetActive(false);
            }

            // 🔥 Guarda el nuevo estado
            GuardarEstadoMapas();
        }
    }

    public void CargarEstadoMapas()
    {
        if (!EsMapa) return;

        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            string key = "MapaActivo:" + child.name;
            string estado = PlayerPrefs.GetString(key, "No");

            bool activo = (estado == "Si");
            child.gameObject.SetActive(activo);

            //Debug.Log($"[CargarEstadoMapas] {child.name}: {estado}");
        }
    }

    public void GuardarEstadoMapas()
    {
        if (!EsMapa) return;

        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            string key = "MapaActivo:" + child.name;
            string valor = child.gameObject.activeSelf ? "Si" : "No";

            PlayerPrefs.SetString(key, valor);
            Debug.Log($"[GuardarEstadoMapas] {child.name}: {valor}");
        }

        PlayerPrefs.Save(); // 🔥 Guarda en disco
        Debug.Log("[GuardarEstadoMapas] PlayerPrefs guardados.");
    }
}
