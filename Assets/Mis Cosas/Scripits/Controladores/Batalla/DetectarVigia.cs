using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarVigia : MonoBehaviour
{
    [SerializeField] OjoVigilante vigia;
    [SerializeField] GameObject PuntoVigia;
    [SerializeField] Scr_Enemigo.TipoEfecfto efecfto;
    Scr_ControladorBatalla controlador;
    // Start is called before the first frame update
    void Start()
    {
        controlador = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
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


            RaycastHit[] hits = Physics.RaycastAll(origen, direccion, distancia);

            bool hayEstatua = false;
            bool vioJugador = false;

            foreach (RaycastHit h in hits)
            {
                if (h.collider.CompareTag("Estatua"))
                {
                    hayEstatua = true;
                }

                if (h.collider.CompareTag("Gata"))
                {
                    vioJugador = true;
                }
            }

            if (hayEstatua)
            {
                Debug.Log("Hay una estatua bloqueando la vista");
                return;
            }

            if (vioJugador && !hayEstatua)
            {
                Debug.Log("No hay estatua bloqueando la vista");
                quitarIntento();
            }
        }
    }

    public void quitarIntento()
    {

        controlador.RecibirEfecto(efecfto.ToString());
        // efectos del ataque
        Tween.ShakeCamera(Camera.main, 3);
        Scr_Activador_boss boss = GameObject.Find("Activador_BOSS").GetComponent<Scr_Activador_boss>();
        boss.detectaron(true);
        vigia.Dormir();
    }
}
