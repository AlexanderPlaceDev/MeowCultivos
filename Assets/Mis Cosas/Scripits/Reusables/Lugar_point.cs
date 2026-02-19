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


    public float fadeDuration = 0.5f; 
    private float ladoAnterior = 0f;
    private bool jugadorDentro;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Encontrado: " + entornoUI.gameObject.name);
        Entorno = entornoUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(transform.position, transform.forward * 5f, Color.blue);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (PlayerPrefs.GetString("TutorialPeleas", "NO") == "NO") return;
        if (!other.CompareTag("Gata") ||isshow == true) return;
        if (isshow) return;

        Debug.DrawRay(transform.position, transform.forward * 5f, Color.blue);
        //Scr_ControladorMisiones ControladorMisiones= other.gameObject.GetComponentInChildren<Scr_ControladorMisiones>();
        //Debug.Log("Se detecto ");
        //ControladorMisiones.actualizarTargetsExploratod(NombreLugar);

        Vector3 direccion = other.transform.position - transform.position;
        direccion.y = 0;

        float lado = Vector3.Dot(transform.forward, direccion);

        Debug.Log("Lado al entrar: " + lado);

        // Si entra por el frente
        if (lado > 0)
        {
            entornoUI.SetActive(true);
            StartCoroutine(Showtext(NombreLugar));
            isshow = true;
        }
        //float dotRight = Vector3.Dot(transform.right, direccionAlJugador);

    }

    private IEnumerator Showtext(string texto)
    {
        if (Entorno == null) yield break;

        isshow = true;
        Entorno.text = texto;

        float t = 0;
        float fadeDuration = 0.5f;

        // Fade In
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);

            Color c = Entorno.color;
            c.a = alpha;
            Entorno.color = c;

            yield return null;
        }

        yield return new WaitForSeconds(esperaTime);

        // Fade Out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);

            Color c = Entorno.color;
            c.a = alpha;
            Entorno.color = c;

            yield return null;
        }

        isshow = false;

        entornoUI.SetActive(false);
    }

}
