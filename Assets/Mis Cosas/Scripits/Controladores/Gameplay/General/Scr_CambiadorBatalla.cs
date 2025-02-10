using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scr_CambiadorBatalla : MonoBehaviour
{
    [SerializeField] float DistanciaDeCargadoCerca = 1f; // Distancia para detección cercana
    [SerializeField] GameObject PrefabEnemigo;
    [SerializeField] float CantidadEnemigosMinima;
    [SerializeField] float CantidadEnemigosMaxima;
    [SerializeField] string Mision;
    [SerializeField] Color ColorMision;
    [SerializeField] string Complemento;
    [SerializeField] string Item;
    [SerializeField] Color ColorItem;
    public bool Cambiando;
    private Transform Gata;
    GameObject Carga;
    GameObject Reloj;

    // Variable estática para asegurarnos que solo un enemigo cargue la escena
    private static bool escenaCargada = false;

    void Start()
    {
        Reloj = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        Carga = GameObject.Find("Canvas").transform.GetChild(6).gameObject;
        Gata = GameObject.Find("Gata").GetComponent<Transform>();
    }

    void Update()
    {
        if (Gata != null && !escenaCargada)
        {
            // Distancia al jugador
            float distanciaAlJugador = Vector3.Distance(transform.position, Gata.position);

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
        Cambiando = true;
        // Marca la escena como cargada para evitar que otros enemigos la carguen
        escenaCargada = true;

        // Aquí puedes cargar la escena de la batalla u otra lógica
        Debug.Log("Escena cargada por " + gameObject.name + " a distancia: " + tipoCarga);
        Camera.main.gameObject.GetComponent<Transform>().GetChild(0).GetComponent<Animator>().Play("Cerrar");
        Camera.main.gameObject.GetComponent<Transform>().GetChild(1).GetComponent<Animator>().Play("Cerrar");
        Reloj.SetActive(false);
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().Enemigo = PrefabEnemigo;
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().CantidadDeEnemigos = (int)Random.Range(CantidadEnemigosMinima, CantidadEnemigosMaxima);
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().Mision = Mision;
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().ColorMision = ColorMision;
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().Complemento = Complemento;
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().Item = Item;
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().ColorItem = ColorItem;
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
