using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scr_CambiadorBatalla : MonoBehaviour
{
    [SerializeField] float DistanciaDeCargadoCerca = 1f; // Distancia para detección cercana
    [SerializeField] float DistanciaDeCargadoLejos = 5f; // Distancia para detección lejana
    [SerializeField] GameObject PrefabEnemigo;
    [SerializeField] float CantidadEnemigosMinima;
    [SerializeField] float CantidadEnemigosMaxima;
    private Scr_MovimientoEnemigosFuera Mov;
    GameObject Carga;
    GameObject Reloj;

    // Variable estática para asegurarnos que solo un enemigo cargue la escena
    private static bool escenaCargada = false;

    void Start()
    {
        Reloj = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        Carga = GameObject.Find("Canvas").transform.GetChild(6).gameObject;
        Mov = GetComponent<Scr_MovimientoEnemigosFuera>();
    }

    void Update()
    {
        if (Mov.Jugador != null && !escenaCargada)
        {
            // Distancia al jugador
            float distanciaAlJugador = Vector3.Distance(transform.position, Mov.Jugador.transform.position);

            // Verificar si está dentro de la distancia cercana
            if (distanciaAlJugador < DistanciaDeCargadoCerca)
            {
                StartCoroutine(CargarEscena("cerca"));
            }
            /* Verificar si está dentro de la distancia lejana
            else if (distanciaAlJugador < DistanciaDeCargadoLejos)
            {
                CargarEscena("lejos");
            }*/
        }
    }

    IEnumerator CargarEscena(string tipoCarga)
    {
        // Marca la escena como cargada para evitar que otros enemigos la carguen
        escenaCargada = true;

        // Aquí puedes cargar la escena de la batalla u otra lógica
        Debug.Log("Escena cargada por " + gameObject.name + " a distancia: " + tipoCarga);
        Camera.main.gameObject.GetComponent<Transform>().GetChild(0).GetComponent<Animator>().Play("Cerrar");
        Camera.main.gameObject.GetComponent<Transform>().GetChild(1).GetComponent<Animator>().Play("Cerrar");
        Reloj.SetActive(false);
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().Enemigo = PrefabEnemigo;
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().CantidadDeEnemigos = (int)Random.Range(CantidadEnemigosMinima,CantidadEnemigosMaxima);
        // Activar la pantalla de carga o hacer lo que sea necesario
        if (Carga != null)
        {
            Carga.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        escenaCargada = false;
        SceneManager.LoadScene(3);
        // Aquí puedes agregar el código para cargar la escena, por ejemplo:
        // SceneManager.LoadScene("NombreDeLaEscena");
    }
}
