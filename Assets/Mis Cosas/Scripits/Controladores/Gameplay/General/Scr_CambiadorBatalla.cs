using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scr_CambiadorBatalla : MonoBehaviour
{
    [SerializeField] public GameObject PrefabEnemigo;
    [SerializeField] float CantidadEnemigosMinima;
    [SerializeField] float CantidadEnemigosMaxima;
    [SerializeField] public string Mision;
    [SerializeField] public Color ColorMision;
    [SerializeField] public string Complemento;
    [SerializeField] public string Item;
    [SerializeField] public Color ColorItem;

    [SerializeField] public Scr_DatosSingletonBatalla.Modo Modo;
    [SerializeField] public int Pista;
    public bool Cambiando;
    private Transform Gata;
    GameObject Carga;
    GameObject Reloj;

    private static bool escenaCargada = false;

    void Start()
    {
        Debug.Log($"[Start] 🟦 Iniciando {gameObject.name}");

        escenaCargada = false;
        Debug.Log("[Start] escenaCargada = FALSE");

        Reloj = GameObject.Find("Canvas")?.transform.GetChild(2).gameObject;
        Debug.Log($"[Start] Reloj encontrado: {Reloj != null}");

        Carga = GameObject.Find("Canvas")?.transform.GetChild(6).gameObject;
        Debug.Log($"[Start] Carga encontrada: {Carga != null}");

        Gata = GameObject.Find("Gata")?.GetComponent<Transform>();
        Debug.Log($"[Start] Gata encontrada: {Gata != null}");

        if (Gata == null)
        {
            Debug.LogError("❌ No se encontró el objeto 'Gata' en la escena.");
        }
    }

    // ⛔ YA NO USAMOS DISTANCIA / UPDATE / FIXEDUPDATE
    // SOLO DETECTAMOS MEDIANTE TRIGGER

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Trigger] Algo entró en el trigger de {gameObject.name}: {other.name}");

        if (!other.CompareTag("Gata"))
        {
            Debug.Log("[Trigger] ❌ El objeto NO es la Gata. Ignorado.");
            return;
        }

        Debug.Log("[Trigger] ✔ Es la Gata.");

        if (escenaCargada)
        {
            Debug.Log("[Trigger] ❌ escenaCargada ya era TRUE. No repetimos.");
            return;
        }

        bool puedeCaminar = Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar;
        Debug.Log($"[Trigger] PuedeCaminar = {puedeCaminar}");

        if (!puedeCaminar)
        {
            Debug.Log("[Trigger] ❌ La gata NO puede caminar. No cambiamos escena.");
            return;
        }

        // Si llegó aquí: todas las condiciones se cumplieron
        Debug.Log("[Trigger] 🟢 TODAS LAS CONDICIONES SE CUMPLEN → INICIAR COROUTINA");

        Cambiando = true;
        escenaCargada = true;

        Debug.Log("[Trigger] Cambiando = TRUE, escenaCargada = TRUE");

        StartCoroutine(CargarEscena());
    }


    IEnumerator CargarEscena()
    {
        Debug.Log($"[CargarEscena] 🟧 CORRUTINA INICIADA por: {gameObject.name}");
        Debug.Log($"[CargarEscena] Estado inicial → Cambiando={Cambiando}, escenaCargada={escenaCargada}");

        PlayerPrefs.Save();
        Debug.Log("[CargarEscena] PlayerPrefs guardados");

        Debug.Log("[CargarEscena] Preparando cámara...");

        var mainCamera = Camera.main;
        Debug.Log($"[CargarEscena] Cámara principal: {mainCamera != null}");

        if (mainCamera != null)
        {
            var animador1 = mainCamera.transform.GetChild(0)?.GetComponent<Animator>();
            var animador2 = mainCamera.transform.GetChild(1)?.GetComponent<Animator>();

            Debug.Log($"[CargarEscena] Animator 1 encontrado: {animador1 != null}");
            Debug.Log($"[CargarEscena] Animator 2 encontrado: {animador2 != null}");

            if (animador1 != null) animador1.Play("Cerrar");
            if (animador2 != null) animador2.Play("Cerrar");
        }

        if (Reloj != null)
        {
            Debug.Log("[CargarEscena] Desactivando reloj.");
            Reloj.SetActive(false);
        }
        else Debug.LogWarning("[CargarEscena] ⚠ No se encontró el reloj");

        var singleton = GameObject.Find("Singleton")?.GetComponent<Scr_DatosSingletonBatalla>();
        Debug.Log($"[CargarEscena] Singleton encontrado: {singleton != null}");

        if (singleton == null)
        {
            Debug.LogError("❌ No se encontró el Singleton. No se puede continuar.");
            Cambiando = false;
            yield break;
        }

        Debug.Log("[CargarEscena] Asignando valores al Singleton...");

        singleton.Enemigo = PrefabEnemigo;
        singleton.Mision = Mision;
        singleton.ColorMision = ColorMision;
        singleton.Complemento = Complemento;
        singleton.Item = Item;
        singleton.ColorItem = ColorItem;
        singleton.ModoSeleccionado = Modo;
        singleton.Pista = Pista;
        Debug.Log("[CargarEscena] Valores básicos asignados.");

        var sol = GameObject.Find("Sol");
        Debug.Log($"[CargarEscena] Sol encontrado: {sol != null}");

        singleton.Luz = sol.GetComponent<Light>().color;

        var controladorTiempo = GameObject.Find("Controlador Tiempo");
        Debug.Log($"[CargarEscena] Controlador Tiempo encontrado: {controladorTiempo != null}");

        singleton.HoraActual = controladorTiempo.GetComponent<Scr_ControladorTiempo>().HoraActual;

        if (Carga != null)
        {
            Debug.Log("[CargarEscena] Activando pantalla de carga.");
            Carga.SetActive(true);
        }
        else Debug.LogWarning("[CargarEscena] ⚠ No se encontró la pantalla de carga.");

        Debug.Log("[CargarEscena] Esperando 1 segundo...");
        yield return new WaitForSeconds(1);

        Debug.Log("[CargarEscena] Comprobando build settings...");

        if (SceneManager.sceneCountInBuildSettings > 3)
        {
            Debug.Log("[CargarEscena] OK. Iniciando carga asíncrona...");

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                Debug.Log($"📡 Progreso de carga: {(asyncLoad.progress * 100f)}%");

                if (asyncLoad.progress >= 0.9f)
                {
                    Debug.Log("💯 Carga al 90%, activando escena...");
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }

            Debug.Log("🎉 Escena activada correctamente.");
        }
        else
        {
            Debug.LogError("❌ La escena 3 no está en los Build Settings.");
        }

        Debug.Log("[CargarEscena] FIN de la corrutina.");
    }
}
