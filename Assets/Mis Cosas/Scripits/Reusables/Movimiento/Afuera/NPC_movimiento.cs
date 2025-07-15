using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class NPC_movimiento : MonoBehaviour
{
    public List<comportamiento> comportamientos = new List<comportamiento>();
    public List<Transform> pos;

    public GameObject CTiempo;
    public Scr_ControladorTiempo ContolT;

    private NavMeshAgent agente;
    private Vector3 Destino;
    private bool Esperando = false;
    private string diaAnterior;

    void Start()
    {
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
        agente = GetComponent<NavMeshAgent>();
        diaAnterior = ContolT.DiaActual;
    }

    void Update()
    {
        if (ContolT.DiaActual != diaAnterior)
        {
            ReiniciarComportamientos();
            diaAnterior = ContolT.DiaActual;

            MoverAlEventoMasCercano();
        }

        checarcambio();
    }

    void checarcambio()
    {
        if (!Esperando)
        {
            //Debug.Log("AAA deja ver");
            foreach (var c in comportamientos)
            {
                if (c.DiaActual == ContolT.DiaActual && !c.Ejecutado)
                {
                    if (ContolT.HoraActual > c.HoraActual ||
                       (ContolT.HoraActual == c.HoraActual && ContolT.MinutoActual >= c.MinutoActual))
                    {
                        c.Ejecutado = true;
                        StartCoroutine(MoverYEspesar(c.pos, 2f));
                        break;
                    }
                }
            }
        }
    }

    IEnumerator MoverYEspesar(int posIndex, float tiempo)
    {
        Esperando = true;
        MoverANuevaPosicion(posIndex);
        yield return new WaitForSeconds(tiempo);
        Esperando = false;
    }

    void MoverANuevaPosicion(int posIndex)
    {
        if (!agente.isOnNavMesh) return;

        Destino = pos[posIndex].position;
        agente.isStopped = false;
        agente.SetDestination(Destino);

        //Debug.Log("AAA ya me muevo");
    }

    void ReiniciarComportamientos()
    {
        foreach (var c in comportamientos)
        {
            c.Ejecutado = false;
        }
    }

    void MoverAlEventoMasCercano()
    {
        float menorDiferencia = float.MaxValue;
        comportamiento eventoMasCercano = null;

        float tiempoActual = ContolT.HoraActual * 60f + ContolT.MinutoActual;

        foreach (var c in comportamientos)
        {
            if (c.DiaActual != ContolT.DiaActual) continue;

            float tiempoEvento = c.HoraActual * 60f + c.MinutoActual;
            float diferencia = Mathf.Abs(tiempoEvento - tiempoActual);

            if (diferencia < menorDiferencia)
            {
                menorDiferencia = diferencia;
                eventoMasCercano = c;
            }
        }

        if (eventoMasCercano != null)
        {
            Debug.Log("NPC: Moviendo al evento más cercano del día al cambiar de día");
            MoverANuevaPosicion(eventoMasCercano.pos);
            eventoMasCercano.Ejecutado = true; // Marcar como hecho para evitar repetirlo
        }
    }
}

[System.Serializable]
public class comportamiento
{
    public string DiaActual;
    public int HoraActual;
    public float MinutoActual;
    public int pos;

    [HideInInspector] public bool Ejecutado = false;
}