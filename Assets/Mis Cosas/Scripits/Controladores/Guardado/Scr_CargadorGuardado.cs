using Cinemachine;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Playables;

public class Scr_CargadorGuardado : MonoBehaviour
{
    [Header("Objetos con datos a guardar")]
    [SerializeField] GameObject CinematicaInicial;
    [SerializeField] GameObject Gata;
    [SerializeField] GameObject Camara360;
    [SerializeField] public GameObject[] Personajes;
    [SerializeField] GameObject Reloj;
    [SerializeField] GameObject Tablero;
    [SerializeField] GameObject MuroTutorial;

    [SerializeField] bool Moviendo;

    GameObject camara;

    private void Awake()
    {
        //Animacion autobus
        camara = Camera.main.gameObject;
        if (PlayerPrefs.GetString("CinematicaInicial", "No") == "Si")
        {
            CinematicaInicial.GetComponent<PlayableDirector>().enabled = false;
            camara.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        }
        else
        {
            if (CinematicaInicial.GetComponent<PlayableDirector>().enabled == false)
            {
                camara.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            }
        }
        //Posicion y Rotacion
        if (PlayerPrefs.GetInt("DialogoMiguel", 0) > 0)
        {
            Gata.transform.position = new Vector3(PlayerPrefs.GetFloat("GataPosX", 62), PlayerPrefs.GetFloat("GataPosY", 7), PlayerPrefs.GetFloat("GataPosZ", 103.5f));
            Gata.transform.rotation = Quaternion.Euler(PlayerPrefs.GetFloat("GataRotX", 0), PlayerPrefs.GetFloat("GataRotY", -67), PlayerPrefs.GetFloat("GataRotZ", 0));
            Camara360.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x = 0;
        }
        //Movimiento
        if (PlayerPrefs.GetString("Movimiento", "No") == "Si")
        {
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
            Gata.GetComponent<Scr_Movimiento>().enabled=true;
        }
        //Mapa Batalla
        GameObject.Find("Singleton").GetComponent<Scr_DatosSingletonBatalla>().NombreMapa = PlayerPrefs.GetString("Mapa Actual", "Mapa Estacion Batalla");
        //Reloj
        if (PlayerPrefs.GetString("Reloj", "No") == "Si")
        {
            Reloj.SetActive(true);
            Gata.transform.GetChild(6).GetComponent<Scr_ControladorMenuGameplay>().enabled = true;
        }
        else
        {
            Gata.transform.GetChild(6).GetComponent<Scr_ControladorMenuGameplay>().enabled = false;
        }
        //Dialogos
        Personajes[0].GetComponent<Scr_SistemaDialogos>().DialogoActual = PlayerPrefs.GetInt("DialogoMiguel", 0);
        Personajes[1].GetComponent<Scr_SistemaDialogos>().DialogoActual = PlayerPrefs.GetInt("DialogoBony", 0);
        if (PlayerPrefs.GetInt("DialogoMiguel", 0) > 0)
        {
            Gata.GetComponent<Scr_ControladorAnimacionesGata>().PuedeCaminar = true;
            MuroTutorial.SetActive(false);
            FindObjectOfType<NavMeshSurface>().BuildNavMesh();
        }

        //Activar personajes
        if (PlayerPrefs.GetString("Cinematica " + "Bony", "No") == "Si")
        {
            Personajes[1].SetActive(true);
        }
    }

    private void Update()
    {
        if (Moviendo)
        {
            Gata.transform.position = new Vector3(65, 7, 122);
        }
    }
}
