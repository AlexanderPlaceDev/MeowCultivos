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
    public Scr_ActivadorDialogos Dialogo;

    private NavMeshAgent agente;
    private Vector3 Destino;
    private bool Esperando = false;
    private string diaAnterior;

    private Transform Gata;

    void Start()
    {

        Gata = GameObject.Find("Gata").transform;
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
        agente = GetComponent<NavMeshAgent>();
        diaAnterior = ContolT.DiaActual;
        Dialogo = GetComponent<Scr_ActivadorDialogos>();
    }

    void Update()
    {
        if (ContolT.DiaActual != diaAnterior)
        {
            Esperando = false;
            ReiniciarComportamientos();
            diaAnterior = ContolT.DiaActual;

            MoverAlEventoMasCercano();
        }
        /*
        else if (Dialogo.panelDialogo.activeSelf)
        {
            DetenerYMirarJugador();
        }*/
        else
        {

            if (YaLlegoAlDestino() && !Esperando)
            {
                // Aquí podrías hacer alguna acción al llegar, como empezar una animación
                BoxCollider[] colliders = GetComponents<BoxCollider>();
                colliders[1].enabled = true; // Desactiva el segundo BoxCollider
                DetenerYMirarJugador();

                Debug.Log("NPC: Ya llegué al destino");
            }
            else
            {
                BoxCollider[] colliders = GetComponents<BoxCollider>();
                colliders[1].enabled = false; // Desactiva el segundo BoxCollider
            }
            checarcambio();


        }

        void checarcambio()
        {
            if (!Esperando)
            {
                Debug.Log("AAA deja ver");
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

        //Mueve al evento mas cercano dependiendo del dia Lunes
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

        //detiene el movimiento y gira a mirar al npc
        void DetenerYMirarJugador()
        {
            if (agente.isOnNavMesh)
            {
                agente.isStopped = true; // Detener el movimiento
            }
            //Solo se voltea solo si esta a una distancia
            float distancia = Vector3.Distance(transform.position, Gata.position);
            //Debug.Log(distancia);
            if (distancia <= 12.5f) // Cambia '2f' al rango deseado
            {
                Vector3 direccion = Gata.position - transform.position;
                direccion.y = 0; // Mantener solo la rotación horizontal

                if (direccion != Vector3.zero)
                {
                    Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime * 5f);
                }
            }
        }
        //Detecta que si llego a su destino
        bool YaLlegoAlDestino()
        {
            if (!agente.pathPending)
            {
                if (agente.remainingDistance <= agente.stoppingDistance)
                {
                    if (!agente.hasPath || agente.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
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