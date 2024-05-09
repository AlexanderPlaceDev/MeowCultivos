using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scr_MiSingleton : MonoBehaviour
{
    // Variable est�tica que contendr� la instancia del singleton
    private static Scr_MiSingleton instance;

    // Propiedad est�tica para acceder a la instancia del singleton
    public static Scr_MiSingleton Instance
    {
        get
        {
            // Si la instancia no existe, la crea
            if (instance == null)
            {
                // Busca la instancia en la escena
                instance = FindObjectOfType<Scr_MiSingleton>();

                // Si no se encuentra, crea un nuevo objeto y lo marca como persistente entre escenas
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(Scr_MiSingleton).Name);
                    instance = singletonObject.AddComponent<Scr_MiSingleton>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            // Devuelve la instancia
            return instance;
        }
    }

    // Este m�todo puede ser utilizado para realizar acciones espec�ficas del singleton
    public void MetodoDelSingleton()
    {
        Debug.Log("�M�todo del Singleton ejecutado!");
    }

    // Este m�todo se ejecutar� cuando se cargue la escena
    private void Awake()
    {
        // Si la instancia ya existe y no es esta, destruye este objeto
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Marca este objeto como persistente entre escenas
        DontDestroyOnLoad(gameObject);
    }

    // Opcionalmente, puedes incluir cualquier otro c�digo espec�fico que necesites para tu Singleton

    public int Escena=0;

    public void CambiarScena(int Numero)
    {
        SceneManager.LoadScene(Numero);
    }
}
