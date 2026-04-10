using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarVigia : MonoBehaviour
{
    [SerializeField] OjoVigilante vigia;
    [SerializeField] GameObject PuntoVigia;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gata"))
        {
            Vector3 origen = PuntoVigia.transform.position;
            Vector3 direccion = (other.transform.position - origen).normalized;
            float distancia = Vector3.Distance(origen, other.transform.position);

            RaycastHit hit;

            if (Physics.Raycast(origen, direccion, out hit, distancia))
            {
                // Si el primer objeto que golpea es una estatua, no hacer nada
                if (hit.collider.CompareTag("Estatua"))
                {
                    Debug.Log("Hay una estatua bloqueando la vista");
                    return;
                }

                // Si golpea directamente al jugador
                if (hit.collider.CompareTag("Gata"))
                {
                    Debug.Log("No Hay una estatua bloqueando la vista");
                    quitarIntento();
                }
            }
        }
    }

    public void quitarIntento()
    {
        Scr_Activador_boss boss = GameObject.Find("Activador_BOSS").GetComponent<Scr_Activador_boss>();
        boss.detectaron(true);
        vigia.Dormir();
    }
}
