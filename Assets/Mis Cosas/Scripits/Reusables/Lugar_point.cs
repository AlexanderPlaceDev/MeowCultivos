using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Lugar_point : MonoBehaviour
{
    [SerializeField] private string NombreLugar;
    public GameObject entornoUI;
    public TextMeshProUGUI Entorno;
    private bool isshow=false;
    public float esperaTime=2;
    // Start is called before the first frame update
    void Start()
    {
        GameObject resultado = BuscarGameObjectPorTag("Entorno");
        if (resultado != null)
        {
            Debug.Log("Encontrado: " + resultado.gameObject.name);
            entornoUI = resultado;
            Entorno= entornoUI.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.Log("No se encontró ningún GameObject con ese tag.");
        }
    }
    GameObject BuscarGameObjectPorTag(string tag)
    {
        GameObject[] todos = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject go in todos)
        {
            // Asegurarse de que no sea parte del editor (como prefabs)
            /*if (go.CompareTag(tag) && !EditorUtility.IsPersistent(go))
            {
                return go;
            }*/
        }

        return null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Gata")) return;
        if (isshow) return;
        Scr_ControladorMisiones ControladorMisiones= other.gameObject.GetComponentInChildren<Scr_ControladorMisiones>();
        Debug.Log("Se detecto ");
        //ControladorMisiones.actualizarTargetsExploratod(NombreLugar);
        StartCoroutine(Showtext());
    }

    private IEnumerator Showtext()
    {
        isshow = true;
        entornoUI.SetActive(true);
        Entorno.text = NombreLugar;
        yield return new WaitForSeconds(esperaTime);
        entornoUI.SetActive(false);
        isshow = false;
    }

        /*
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Gata")) return;

        }*/
}
