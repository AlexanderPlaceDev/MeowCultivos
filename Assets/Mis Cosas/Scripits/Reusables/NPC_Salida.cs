using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Salida : MonoBehaviour
{
    public BoxCollider box;
    public Scr_ControladorTiempo ControlT;
    public Scr_ActivadorDialogos dialogos;
    //public Scr_ControladorAnimacionesNPC npc;
    public int HoraSalida = 22;
    public int MinutoSalida = 0;

    private bool yaSeFue = false;

    void Start()
    {
        ControlT = GameObject.Find("Controlador Tiempo")
            .GetComponent<Scr_ControladorTiempo>();
    }

    void Update()
    {
        TimeSpan horaActual = new TimeSpan(
            ControlT.HoraActual,
            (int)ControlT.MinutoActual,
            0
        );

        TimeSpan horaSalida = new TimeSpan(
            HoraSalida,
            MinutoSalida,
            0
        );

        // Si ya es hora de salida
        if (horaActual >= horaSalida && !yaSeFue)
        {
            yaSeFue = true;
            quitardialogo();
        }

        // Si todavía no es hora de salida
        if (horaActual < horaSalida && yaSeFue)
        {
            yaSeFue = false;
            aparecerCollider();
        }
    }

    public void quitardialogo()
    {
        if (dialogos.Comprando || dialogos.ViendoMisiones)
        {
            dialogos.interrumpirNPC();
        }
        dialogos.estaAdentro = false;
        dialogos.OcultarIconos();
        quitarCollider();
    }

    public void aparecerCollider()
    {
        if (box != null)
            box.enabled = true;
    }

    public void quitarCollider()
    {
        if (box != null)
            box.enabled = false;
    }

    public void desactivarNPc(GameObject obj)
    {
        obj.gameObject.SetActive(false);
    }
}
