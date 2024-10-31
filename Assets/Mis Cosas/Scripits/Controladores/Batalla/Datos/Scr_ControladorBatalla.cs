using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scr_ControladorBatalla : MonoBehaviour
{
    [Header("Panel Final")]
    [SerializeField] Color[] ColoresBoton;
    [SerializeField] TextMeshProUGUI TextoNivel;
    [SerializeField] TextMeshProUGUI TextoSiguienteNivel;
    [SerializeField] Image Barra;


    [SerializeField] Scr_CreadorArmas Arma1;
    [SerializeField] Scr_CreadorArmas Arma2;
    [SerializeField] Scr_CreadorArmas Arma3;

    [SerializeField] TextMeshProUGUI NumeroCuenta;
    [SerializeField] GameObject Mirilla;

    float Cuenta = 4;
    [SerializeField] TextMeshProUGUI TextoMinutos;
    int Minutos = 2;
    [SerializeField] TextMeshProUGUI TextoSegundos;
    float Segundos = 60;
    bool ComenzarCuenta = false;
    bool ComenzoTiempo = false;

    [SerializeField] private GameObject PanelFinal;
    [SerializeField] Animator[] BarrasNegras;
    [SerializeField] GameObject CirculoCarga;

    public List<GameObject> Enemigos = new List<GameObject>();
    private bool DioRecompensa = false;

    public int VidaAnterior = 10;
    public int VidaActual = 10;

    private Scr_DatosSingletonBatalla Singleton;

    void Start()
    {
        Singleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            PlayerPrefs.DeleteKey("Nivel");
            PlayerPrefs.DeleteKey("XPActual");
            PlayerPrefs.DeleteKey("XPSiguiente");
        }

        Comienzo();
        Tiempo();
        RemoverEnemigosMuertos();
        ActualizarVida();
        Terminar();
    }

    private void ActualizarVida()
    {
        if (VidaActual != VidaAnterior)
        {
            VidaAnterior = VidaActual;
            var asistente = GameObject.Find("Asistente").gameObject.GetComponent<Scr_ControladorAsistente>();
            if (asistente != null && !asistente.OrdenDeEstados.Contains("Disparando"))
            {
                asistente.OrdenDeEstados.Add("Vida");
            }

        }
    }
    private void RemoverEnemigosMuertos()
    {
        Enemigos.RemoveAll(enemigo => enemigo == null);
    }

    public void CuentaAtras()
    {
        List<GameObject> Spawners = new List<GameObject>();
        foreach (Transform Mapa in GameObject.Find("Mapa").transform.GetComponentInChildren<Transform>(true))
        {
            if (Mapa.name == Singleton.NombreMapa)
            {
                Mapa.gameObject.SetActive(true);
                foreach (Transform Objeto in Mapa.GetComponentInChildren<Transform>(true))
                {
                    if (Objeto.name.Contains("Spawner"))
                    {
                        Spawners.Add(Objeto.gameObject);
                    }
                }
            }
        }
        NumeroCuenta.gameObject.SetActive(true);
        ComenzarCuenta = true;






        Scr_DatosSingletonBatalla DatosSingleton = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();

        for (int i = 0; i < DatosSingleton.CantidadDeEnemigos; i++)
        {
            int Posicion = UnityEngine.Random.Range(0, Spawners.Count);

            GameObject Enemigo = Instantiate(DatosSingleton.Enemigo, Spawners[Posicion].transform.position, Quaternion.identity);
            Enemigos.Add(Enemigo);
        }
    }

    private void Comienzo()
    {
        if (ComenzarCuenta)
        {
            if (Cuenta > 0)
            {
                Cuenta -= Time.deltaTime;
                NumeroCuenta.text = Cuenta < 1 ? "Pelea" : ((int)Cuenta).ToString();
            }
            else
            {
                NumeroCuenta.gameObject.SetActive(false);
                ComenzarCuenta = false;
                Cuenta = 4;
                ActivarControles(true);
                ComenzoTiempo = true;

                foreach (GameObject Enemigo in Enemigos)
                {
                    Enemigo.GetComponent<NavMeshAgent>().enabled = true;
                    Enemigo.GetComponent<BoxCollider>().enabled = true;
                }
            }
        }
    }

    private void Tiempo()
    {
        if (ComenzoTiempo)
        {
            Mirilla.SetActive(true);
            if (Minutos > 0 || Segundos > 0)
            {
                Segundos -= Time.deltaTime;
                if (Segundos < 0)
                {
                    Segundos = 59;
                    Minutos--;
                }
                TextoSegundos.text = Segundos > 9 ? ((int)Segundos).ToString() : "0" + ((int)Segundos).ToString();
                TextoMinutos.text = Minutos.ToString();
            }
            else
            {
                ComenzoTiempo = false;
                FinalizarBatalla(false);
            }
        }
    }

    private void Terminar()
    {
        if (VidaActual <= 0)
        {
            if (VidaActual < 0)
            {
                VidaActual = 0;
            }
            ComenzoTiempo = false;
            FinalizarBatalla(false);

        }
        if (Enemigos.Count <= 0 && ComenzoTiempo)
        {
            ComenzoTiempo = false;
            FinalizarBatalla(true);

        }

    }

    private void FinalizarBatalla(bool Gano)
    {



        ActivarControles(false);
        if (Gano && !DioRecompensa)
        {
            DioRecompensa = true;
            DarRecompensa();
        }

        //Asigar Nivel

        if (PlayerPrefs.GetInt("XPActual", 0) >= PlayerPrefs.GetInt("XPSiguiente", 10))
        {
            PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual", 0) - PlayerPrefs.GetInt("XPSiguiente", 10));
            PlayerPrefs.SetInt("Nivel", PlayerPrefs.GetInt("Nivel", 0) + 1);
            PlayerPrefs.SetInt("XPSiguiente", PlayerPrefs.GetInt("XPSiguiente", 10) * 2);
            PlayerPrefs.SetInt("PuntosDeHabilidad", PlayerPrefs.GetInt("PuntosDeHabilidad", 0) + 3);
        }

        TextoNivel.text = PlayerPrefs.GetInt("Nivel", 0).ToString();
        TextoSiguienteNivel.text = "Siguiente Nivel: " + PlayerPrefs.GetInt("XPActual", 0) + "/" + PlayerPrefs.GetInt("XPSiguiente", 10);
        Barra.fillAmount = ((float)PlayerPrefs.GetInt("XPActual", 0) / (float)PlayerPrefs.GetInt("XPSiguiente", 10));

        PanelFinal.SetActive(true);
    }

    private void DarRecompensa()
    {
        Scr_Enemigo Enemigo = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().Enemigo.GetComponent<Scr_Enemigo>();

        // Verificar la longitud de Drops y Probabilidades
        if (Enemigo.Drops.Length != Enemigo.Probabilidades.Length)
        {
            Debug.LogError("La longitud de Drops y Probabilidades no coincide.");
            return;
        }

        // Inicializar diccionario para contar recompensas
        Dictionary<Scr_CreadorObjetos, float> recompensasDict = new Dictionary<Scr_CreadorObjetos, float>();

        for (int j = 0; j < GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().CantidadDeEnemigos; j++)
        {
            for (int i = 0; i < Enemigo.Drops.Length; i++)
            {
                // Generar probabilidad
                if (UnityEngine.Random.Range(0, 100) <= Enemigo.Probabilidades[i])
                {
                    if (recompensasDict.ContainsKey(Enemigo.Drops[i]))
                    {
                        recompensasDict[Enemigo.Drops[i]] += 1;
                    }
                    else
                    {
                        recompensasDict.Add(Enemigo.Drops[i], 1);
                    }

                    Debug.Log($"Recompensa {i} obtenida: {Enemigo.Drops[i].name} con probabilidad {Enemigo.Probabilidades[i]}%");
                }
                else
                {
                    Debug.Log($"Recompensa {i} NO obtenida: {Enemigo.Drops[i].name} con probabilidad {Enemigo.Probabilidades[i]}%");
                }
                PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual") + UnityEngine.Random.Range(Enemigo.XPMinima, Enemigo.XPMaxima));
            }
        }

        //Dar XP

        // Limpiar listas antes de llenarlas
        Scr_DatosSingletonBatalla datos = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>();
        datos.ObjetosRecompensa.Clear();
        datos.CantidadesRecompensa.Clear();

        // Convertir diccionario a listas
        foreach (var kvp in recompensasDict)
        {
            datos.ObjetosRecompensa.Add(kvp.Key);
            datos.CantidadesRecompensa.Add((int)kvp.Value);

            // Actualizar UI
            int index = datos.ObjetosRecompensa.Count - 1;
            PanelFinal.transform.GetChild(0).GetChild(6).GetChild(index).GetComponent<Image>().sprite = kvp.Key.IconoInventario;
            PanelFinal.transform.GetChild(0).GetChild(6).GetChild(index).gameObject.SetActive(true);
            PanelFinal.transform.GetChild(0).GetChild(6).GetChild(index).GetComponent<Image>().color = Color.white;
            PanelFinal.transform.GetChild(0).GetChild(6).GetChild(index).GetChild(0).gameObject.SetActive(true);
            PanelFinal.transform.GetChild(0).GetChild(6).GetChild(index).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((int)kvp.Value).ToString();

            Debug.Log($"Recompensa {index} activada en la UI con cantidad: {kvp.Value}");
        }

        // Verificación final
        if (datos.ObjetosRecompensa.Count != datos.CantidadesRecompensa.Count)
        {
            Debug.LogError("La cantidad de objetos y recompensas no coincide.");
        }
    }







    private void ActivarControles(bool activar)
    {
        Cursor.visible = true;
        var camara = Camera.main;
        Cursor.lockState = CursorLockMode.None;
        GetComponent<Scr_ControladorArmas>().enabled = activar;
        camara.GetComponent<Scr_GirarCamaraBatalla>().enabled = activar;
        camara.transform.parent.GetComponent<Rigidbody>().useGravity = activar;
        camara.transform.parent.GetComponent<Scr_Movimiento>().enabled = activar;
    }

    public void BotonAceptar()
    {
        foreach (Animator Anim in BarrasNegras)
        {
            Anim.Play("Cerrar");
        }
        StartCoroutine(EsperarCierre());
        PanelFinal.GetComponent<Animator>().Play("Cerrar");

    }

    public void CambiarColorAceptar(bool Entra)
    {
        if (Entra)
        {
            PanelFinal.transform.GetChild(0).GetChild(5).GetComponent<Image>().color = ColoresBoton[0];
        }
        else
        {
            PanelFinal.transform.GetChild(0).GetChild(5).GetComponent<Image>().color = ColoresBoton[1];
        }
    }

    IEnumerator EsperarCierre()
    {
        yield return new WaitForSeconds(1);
        CirculoCarga.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(2);
    }
}
