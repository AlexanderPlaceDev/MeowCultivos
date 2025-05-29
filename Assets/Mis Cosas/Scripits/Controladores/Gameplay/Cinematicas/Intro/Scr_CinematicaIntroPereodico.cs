using UnityEngine;

public class Scr_CinematicaIntroPereodico : MonoBehaviour
{
    [SerializeField] GameObject Iconos;
    [SerializeField] GameObject PereodicoGrande;
    [SerializeField] Scr_ControladorCinematica Cinematica;
    int cont = 0;

    void Update()
    {
        if (Iconos.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E))
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
