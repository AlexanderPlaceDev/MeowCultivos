using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Mina : MonoBehaviour
{
    [SerializeField] Scr_SpawnerRecolectable Carrito;
    [SerializeField] Scr_ControladorTiempo Tiempo;

    string claveDiaSpawn = "Carrito_DiaUltimoSpawn";
    string claveRecogido = "Carrito_Recogido";

    string ultimoDiaSpawn;
    bool recogidoHoy;

    void Start()
    {
        ultimoDiaSpawn = PlayerPrefs.GetString(claveDiaSpawn, "");
        recogidoHoy = PlayerPrefs.GetInt(claveRecogido, 0) == 1;

        // Si ya fue recogido hoy, asegurar que esté apagado
        if (recogidoHoy)
        {
            Carrito.TieneObjeto = false;
            Carrito.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        VerificarRespawn();
    }

    void LateUpdate()
    {
        // Si el jugador lo recogió (TieneObjeto pasó a false)
        if (Carrito.TieneObjeto == false && !recogidoHoy)
        {
            recogidoHoy = true;

            PlayerPrefs.SetInt(claveRecogido, 1);
            PlayerPrefs.Save();

            Carrito.gameObject.SetActive(false);
        }
    }

    void VerificarRespawn()
    {
        // Solo cuando sea las 12
        if (Tiempo.HoraActual == 12)
        {
            // Si es un nuevo día
            if (Tiempo.DiaActual != ultimoDiaSpawn)
            {
                ultimoDiaSpawn = Tiempo.DiaActual;
                PlayerPrefs.SetString(claveDiaSpawn, ultimoDiaSpawn);

                // Resetear estado diario
                recogidoHoy = false;
                PlayerPrefs.SetInt(claveRecogido, 0);

                // Activar carrito
                Carrito.TieneObjeto = true;
                Carrito.gameObject.SetActive(true);

                PlayerPrefs.Save();
            }
        }
    }
}