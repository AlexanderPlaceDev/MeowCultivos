using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scr_ControladorBatalla : MonoBehaviour
{
    [SerializeField] Scr_CreadorArmas Arma1;
    [SerializeField] Scr_CreadorArmas Arma2;
    [SerializeField] Scr_CreadorArmas Arma3;

    [SerializeField] TextMeshProUGUI NumeroCuenta;

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
    private AsyncOperation Operacion;
    private bool escenaPrecargada = false; // Nuevo flag para saber si la escena está precargada
    private bool DioRecompensa = false;

    void Start()
    {
        // Aquí podrías iniciar cualquier configuración inicial
    }

    void Update()
    {
        Comienzo();
        Tiempo();
        RemoverEnemigosMuertos();
        Terminar();
    }

    private void RemoverEnemigosMuertos()
    {
        Enemigos.RemoveAll(enemigo => enemigo == null);
    }

    public void CuentaAtras()
    {
        NumeroCuenta.gameObject.SetActive(true);
        ComenzarCuenta = true;

        List<GameObject> Spawners = new List<GameObject>();

        foreach (Transform Objeto in GameObject.Find("Mapa").transform.GetChild(0).GetComponentInChildren<Transform>())
        {
            if (Objeto.name.Contains("Spawner"))
            {
                Spawners.Add(Objeto.gameObject);
            }
        }

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
                FinalizarBatalla();
            }
        }
    }

    private void Terminar()
    {
        if ((Minutos <= 0 && Segundos <= 0) || (Enemigos.Count <= 0 && ComenzoTiempo))
        {
            ComenzoTiempo = false;
            FinalizarBatalla();
            if (Operacion == null)
            {
                StartCoroutine(PrecargarEscena(2));
            }
        }
    }

    private void FinalizarBatalla()
    {
        ActivarControles(false);
        if (!DioRecompensa)
        {
            DioRecompensa = true;
            DarRecompensa();
        }
        PanelFinal.SetActive(true);
    }

    private void DarRecompensa()
    {
        Scr_Enemigo Enemigo = GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().Enemigo.GetComponent<Scr_Enemigo>();
        float[] Recompensas = { 0, 0, 0, 0 };
        int i;

        // Iterar sobre la cantidad de enemigos
        for (int j = 0; j < GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().CantidadDeEnemigos; j++)
        {
            i = 0;
            foreach (Scr_CreadorObjetos Objeto in Enemigo.Drops)
            {
                if (UnityEngine.Random.Range(0, 100) <= Enemigo.Probabilidades[i])
                {
                    PanelFinal.transform.GetChild(0).GetChild(6).GetChild(i).GetComponent<Image>().sprite = Objeto.IconoInventario;
                    GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().ObjetosRecompensa.Add(Objeto);
                    Recompensas[i] += 1;  // Incrementar la recompensa en lugar de establecerla
                    Debug.Log($"Recompensa {i} obtenida: {Objeto.name} con probabilidad {Enemigo.Probabilidades[i]}%");
                }
                else
                {
                    Debug.Log($"Recompensa {i} NO obtenida: {Objeto.name} con probabilidad {Enemigo.Probabilidades[i]}%");
                }
                i++;
            }
        }

        // Actualizar la interfaz de usuario con las recompensas
        for (i = 0; i < Recompensas.Length; i++)
        {
            if (Recompensas[i] != 0)
            {
                PanelFinal.transform.GetChild(0).GetChild(6).GetChild(i).gameObject.SetActive(true);
                PanelFinal.transform.GetChild(0).GetChild(6).GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = Recompensas[i].ToString();
                GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().CantidadesRecompensa.Add((int)Recompensas[i]);
                Debug.Log($"Recompensa {i} activada en la UI con cantidad: {Recompensas[i]}");
            }
            else
            {
                PanelFinal.transform.GetChild(0).GetChild(6).GetChild(i).gameObject.SetActive(false);
                Debug.Log($"Recompensa {i} no obtenida, no se activa en la UI.");
            }
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

    private IEnumerator PrecargarEscena(int escena)
    {
        // Inicia la carga asíncrona de la escena
        Operacion = SceneManager.LoadSceneAsync(escena);
        // No permitas que la escena se active automáticamente cuando termine de cargar
        Operacion.allowSceneActivation = false;

        // Opcional: Espera hasta que la escena esté completamente cargada
        while (!Operacion.isDone)
        {
            if (Operacion.progress >= 0.9f && !escenaPrecargada)
            {
                escenaPrecargada = true; // Marca la escena como precargada
            }
            yield return null;
        }
    }

    public void BotonAceptar()
    {
        foreach(Animator Anim in BarrasNegras)
        {
            Anim.Play("Cerrar");
        }
        StartCoroutine(EsperarCierre());
        PanelFinal.GetComponent<Animator>().Play("Cerrar");

    }

    IEnumerator EsperarCierre()
    {
        yield return new WaitForSeconds(1);
        CirculoCarga.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(2);
    }
}
