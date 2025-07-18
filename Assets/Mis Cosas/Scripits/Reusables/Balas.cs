using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balas : MonoBehaviour
{
    public float da�o = 0;
    private float delete = 2f;
    private float contar=0;
    public int penetracion=0;
    public GameObject Controlador;
    // Start is called before the first frame update
    void Start()
    {
        Controlador = GameObject.Find("Controlador");
    }

    // Update is called once per frame
    void Update()
    {
        contar += Time.deltaTime;
        if(contar > delete)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Da�o");
        //if (!other.CompareTag("Gata")) return;

        Debug.Log("Da�o");
        if (other.CompareTag("Enemigo"))
        {
            Debug.Log("Da�o");
            var ene = other.GetComponent<Scr_Enemigo>();
            ene.RecibirDa�o(da�o);
            penetracion_menos();
            // L�gica de da�o
            Controlador.GetComponent<Scr_ControladorBatalla>().PuntosActualesHabilidad +=
                GameObject.Find("Singleton").GetComponent<Scr_DatosArmas>().TodasLasArmas[Controlador.GetComponent<Scr_ControladorUIBatalla>().ArmaActual].PuntosXGolpe;
        }
    }
    void penetracion_menos()
    {
        penetracion--;
        if (penetracion < 0)
        {
            Destroy(gameObject);
        }
    }
}
