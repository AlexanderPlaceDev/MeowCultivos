using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Singleton : MonoBehaviour
{
    // La instancia única del Singleton
    private static Scr_Singleton _instance;

    // Propiedad pública para acceder a la instancia
    public static Scr_Singleton Instance
    {
        get
        {
            if (_instance == null)
            {
                // Buscar el objeto en la escena
                _instance = FindObjectOfType<Scr_Singleton>();

                // Si no se encuentra, crear uno nuevo
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<Scr_Singleton>();
                    singletonObject.name = typeof(Scr_Singleton).ToString() + " (Singleton)";

                    // Opcional: Asegurarse de que el objeto no se destruya al cargar una nueva escena
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    // Asegurarse de que el Singleton es único al cargar la escena
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Método de ejemplo
    public void DoSomething()
    {
        Debug.Log("Singleton is working!");
    }
}