using UnityEngine;

public class Scr_Herramienta : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        if (!other.TryGetComponent(out Scr_Recurso spawn))
            return;

        if (!spawn.GetUsaHacha() && !spawn.GetUsaPico())
            return;

        spawn.RecibirDanio(1);
    }

   
}
