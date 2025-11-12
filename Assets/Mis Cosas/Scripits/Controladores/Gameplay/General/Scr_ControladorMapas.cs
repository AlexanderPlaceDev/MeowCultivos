using UnityEngine;

public class Scr_ControladorMapas : MonoBehaviour
{
    [SerializeField] public bool EsMapa;
    [SerializeField] GameObject[] MapasQueActiva;
    [SerializeField] GameObject[] MapasQueDesactiva;
    [SerializeField] string NombreMapaBatalla;
    [SerializeField] Material SkyBoxDia;
    [SerializeField] Material SkyBoxNoche;

    // Flag para evitar guardados mientras se está cargando
    private bool cargando = false;

    private void Start()
    {
        if (EsMapa)
            CargarEstadoMapas();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Gata") return;

        // Configura datos del singleton (esto es independiente de EsMapa)
        var singletonObj = GameObject.Find("Singleton");
        if (singletonObj != null)
        {
            var singleton = singletonObj.GetComponent<Scr_DatosSingletonBatalla>();
            if (singleton != null)
            {
                singleton.NombreMapa = NombreMapaBatalla;
                singleton.SkyBoxDia = SkyBoxDia;
                singleton.SkyBoxNoche = SkyBoxNoche;
            }
        }

        PlayerPrefs.SetString("Mapa Actual", NombreMapaBatalla);

        // Activar / desactivar según las listas del objeto que recibe el trigger
        foreach (GameObject mapa in MapasQueActiva)
        {
            if (mapa && !mapa.activeSelf) mapa.SetActive(true);
        }

        foreach (GameObject mapa in MapasQueDesactiva)
        {
            if (mapa && mapa.activeSelf) mapa.SetActive(false);
        }

        // IMPORTANT: Si este script es el trigger (EsMapa == false), debemos pedir
        // explícitamente al controlador principal (EsMapa == true) que guarde.
        if (!EsMapa)
        {
            Scr_ControladorMapas controladorPrincipal = FindObjectOfType<Scr_ControladorMapas>();
            if (controladorPrincipal != null && controladorPrincipal.EsMapa)
            {
                controladorPrincipal.GuardarEstadoMapas();
            }
            else
            {
                // Fallback: si no encontramos controlador principal, guardamos aquí (menos ideal)
                GuardarEstadoMapas();
            }
        }
        else
        {
            // Si por alguna razón el propio objeto es EsMapa (no trigger), guarda normalmente
            GuardarEstadoMapas();
        }
    }

    public void CargarEstadoMapas()
    {
        if (!EsMapa) return;

        cargando = true;

        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            string key = "MapaActivo:" + child.name;

            if (PlayerPrefs.HasKey(key))
            {
                string estado = PlayerPrefs.GetString(key);
                bool activo = (estado == "Si");
                child.gameObject.SetActive(activo);
            }
            else
            {
                // No hay dato guardado: respetamos el estado por defecto del editor
                Debug.Log($"[CargarEstadoMapas] {child.name} sin datos guardados, mantiene: {child.gameObject.activeSelf}");
            }
        }

        cargando = false;
    }

    public void GuardarEstadoMapas()
    {
        // Solo el controlador "Mapa" debe escribir los PlayerPrefs en condiciones normales
        if (!EsMapa) return;

        if (cargando)
        {
            Debug.Log("[GuardarEstadoMapas] Cancelado: aún se está cargando.");
            return;
        }

        Debug.Log("[GuardarEstadoMapas] Guardando estados de los hijos...");
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            string key = "MapaActivo:" + child.name;
            string valor = child.gameObject.activeSelf ? "Si" : "No";
            PlayerPrefs.SetString(key, valor);
            Debug.Log($"[GuardarEstadoMapas] {child.name} = {valor}");
        }

        PlayerPrefs.Save();
        Debug.Log("[GuardarEstadoMapas] PlayerPrefs guardados.");
    }

    private void OnApplicationQuit()
    {
        if (EsMapa)
            PlayerPrefs.Save();
    }
}
