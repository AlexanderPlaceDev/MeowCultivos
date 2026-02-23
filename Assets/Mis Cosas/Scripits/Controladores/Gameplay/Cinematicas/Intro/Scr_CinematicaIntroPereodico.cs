using UnityEngine;
using UnityEngine.InputSystem;

public class Scr_CinematicaIntroPereodico : MonoBehaviour
{
    [SerializeField] GameObject Iconos;
    [SerializeField] GameObject PereodicoGrande;
    [SerializeField] Scr_ControladorCinematica Cinematica;
    public GameObject Boton;
    int cont = 0;


    PlayerInput playerInput;
    private InputAction Interactuar;
    InputIconProvider IconProvider;
    private Sprite iconoActualInteractuar = null;
    private string textoActualInteractuar = "";
    private void Start()
    {
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Interactuar = playerInput.actions["Interactuar"];
    }
    void Update()
    {
        if (Iconos.activeSelf)
        {
            IconProvider.ActualizarIconoUI(Interactuar, Boton.transform, ref iconoActualInteractuar, ref textoActualInteractuar, false);
            if (Interactuar.WasPressedThisFrame())
            {
                if (cont == 0)
                {
                    cont = 1;
                    PereodicoGrande.SetActive(true);
                }
                else
                {
                    cont = 2;
                    PereodicoGrande.SetActive(false);
                    Iconos.SetActive(false);

                    // ✅ Reanuda la cinemática desde la posición pausada
                    Cinematica.PausaAlTerminar[4] = false;
                }
            }
        }
    }
}
