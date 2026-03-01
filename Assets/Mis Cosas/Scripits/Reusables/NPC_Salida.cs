using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Salida : MonoBehaviour
{
    public BoxCollider box;
    public Scr_ControladorTiempo ControlT;
    public Scr_ActivadorDialogos dialogos;
    public Scr_ControladorAnimacionesNPC npc;
    public int HoraSalida = 19;
    public int MinutoSalida = 5;

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
        if (dialogos != null && dialogos.Comprando)
        {
            dialogos.RegresarACamaraBase();
            dialogos.estaAdentro=false;
        }

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

    public void desactivarNPc()
    {
        gameObject.SetActive(false);
    }
}
