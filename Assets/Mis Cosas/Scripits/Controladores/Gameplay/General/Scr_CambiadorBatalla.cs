using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scr_CambiadorBatalla : MonoBehaviour
{
    [SerializeField] float DistanciaDeCargadoCerca = 1f;
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

    private static bool escenaCargada = false;

    void Start()
    {
        escenaCargada = false;
        Reloj = GameObject.Find("Canvas")?.transform.GetChild(2).gameObject;
        Carga = GameObject.Find("Canvas")?.transform.GetChild(6).gameObject;
        Gata = GameObject.Find("Gata")?.GetComponent<Transform>();

        if (Gata == null)
        {
            Debug.LogError("❌ No se encontró el objeto 'Gata' en la escena.");
        }
    }

    void Update()
    {
        if (Gata != null && !escenaCargada)
        {
            float distanciaAlJugador = Vector3.Distance(transform.position, Gata.position);

            if (distanciaAlJugador < DistanciaDeCargadoCerca)
            {
                StartCoroutine(CargarEscena());
            }
        }
    }

    IEnumerator CargarEscena()
    {
        if (Cambiando || escenaCargada)
        {
            Debug.LogWarning("⚠️ La escena ya está cambiando. Cancelando nueva carga.");
            yield break;
        }
        PlayerPrefs.Save();
        Cambiando = true;
        escenaCargada = true;

        Debug.Log("✅ Cargando escena por " + gameObject.name);

        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Debug.Log("🎥 Cámara encontrada. Reproduciendo animaciones.");

            var animador1 = mainCamera.transform.GetChild(0)?.GetComponent<Animator>();
            var animador2 = mainCamera.transform.GetChild(1)?.GetComponent<Animator>();

            if (animador1 != null) animador1.Play("Cerrar");
            if (animador2 != null) animador2.Play("Cerrar");
        }
        else
        {
            Debug.LogError("❌ No se encontró la cámara principal.");
        }

        if (Reloj != null)
        {
            Debug.Log("⏳ Desactivando reloj.");
            Reloj.SetActive(false);
        }

        var singleton = GameObject.Find("Singleton")?.GetComponent<Scr_DatosSingletonBatalla>();
        if (singleton == null)
        {
            Debug.LogError("❌ No se encontró el Singleton. No se puede continuar.");
            Cambiando = false;
            yield break;
        }

        Debug.Log("📦 Asignando valores al Singleton.");
        singleton.Enemigo = PrefabEnemigo;
        singleton.Mision = Mision;
        singleton.ColorMision = ColorMision;
        singleton.Complemento = Complemento;
        singleton.Item = Item;
        singleton.ColorItem = ColorItem;
        singleton.Luz = GameObject.Find("Sol").GetComponent<Light>().color;
        singleton.HoraActual = GameObject.Find("Controlador Tiempo").GetComponent<Scr_ControladorTiempo>().HoraActual;

        if (Carga != null)
        {
            Debug.Log("📊 Activando pantalla de carga.");
            Carga.SetActive(true);
        }

        Debug.Log("⏳ Esperando 1 segundo antes de comenzar la carga asíncrona.");
        yield return new WaitForSeconds(1);

        if (SceneManager.sceneCountInBuildSettings > 3)
        {
            Debug.Log("✅ Iniciando carga asíncrona de la escena 3...");

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                Debug.Log($"📡 Progreso de carga: {asyncLoad.progress * 100}%");

                // Cuando la carga llegue al 90% (casi lista), activamos la escena
                if (asyncLoad.progress >= 0.9f)
                {
                    Debug.Log("✅ Escena 3 cargada al 90%. Activando...");
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }

            Debug.Log("✅ Escena 3 activada correctamente.");
        }
        else
        {
            Debug.LogError("❌ La escena 3 no está en los Build Settings.");
        }
    }


}
