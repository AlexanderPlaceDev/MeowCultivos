using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class NPC_movimiento : MonoBehaviour
{
    //Hablando y ViendoMisiones para comprobar que estan en dialogo el npc 
    public List<comportamiento> Posiciones = new List<comportamiento>(); //lista de lugares a donde ir que puede moverse el npc
    public List<Transform> pos; // posiciones que se tienen que poner aparte en el mapa para que puedan ser utilizados

    public GameObject CTiempo; //checa donde esta el controlador del tiempo
    public Scr_ControladorTiempo ContolT; //checa el tiempo y el dia
    public Scr_ControladorAnimacionesNPC Anim; //el activdor de dialogos

    private NavMeshAgent agente; //para el navmesh
    private Vector3 Destino; //destino
    private bool Esperando = false; //se utiliza para pausar el movimento en caso de necesitaro
    private string diaAnterior; //checa un dia para poder moverse a la ultima posicion que deberia estar
    private Scr_ActivadorDialogos Dialogo;
    private Scr_SistemaDialogos SisDialogo;//checa cosas del activador
    private Transform Gata;

    void Start()
    {

        Gata = GameObject.Find("Gata").transform;
        CTiempo = GameObject.Find("Controlador Tiempo");
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
        agente = GetComponent<NavMeshAgent>();
        diaAnterior = ContolT.DiaActual;
        Anim = GetComponent<Scr_ControladorAnimacionesNPC>();
        Dialogo = GetComponent<Scr_ActivadorDialogos>();
        SisDialogo=GetComponent<Scr_SistemaDialogos>();
    }

    private void OnEnable()
    {
        Gata = GameObject.Find("Gata").transform;
        CTiempo = GameObject.Find("Controlador Tiempo");
        ContolT = CTiempo.GetComponent<Scr_ControladorTiempo>();
        agente = GetComponent<NavMeshAgent>();
        diaAnterior = ContolT.DiaActual;
        Anim = GetComponent<Scr_ControladorAnimacionesNPC>();
        Dialogo = GetComponent<Scr_ActivadorDialogos>();
        SisDialogo = GetComponent<Scr_SistemaDialogos>();
    }
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        PlayerPrefs.SetFloat($"{SisDialogo.NombreNPC}_X", pos.x);
        PlayerPrefs.SetFloat($"{SisDialogo.NombreNPC}_Y", pos.y);
        PlayerPrefs.SetFloat($"{SisDialogo.NombreNPC}_Z", pos.z);
        //Debug.Log(NombreNPC + "_Guardado"+pos);
    }
    void Update()
    {

        if (!Dialogo.MisionesSecundariasUI.activeSelf && !Dialogo.panelDialogo.activeSelf)
        {
            //Debug.Log("aa" + diaAnterior);
            if (ContolT.DiaActual != diaAnterior)
            {
                Esperando = false;
                ReiniciarPosiciones();
                diaAnterior = ContolT.DiaActual;
                //Debug.Log("aa");
                MoverAlLugarMasCercano();
            }

            else
            {

                if (YaLlegoAlDestino() && !Esperando)
                {
                    //Debug.Log("aaaaaattttt");
                    // Aquí podrías hacer alguna acción al llegar, como empezar una animación
                    BoxCollider[] colliders = GetComponents<BoxCollider>();
                    colliders[1].enabled = true; // Desactiva el segundo BoxCollider
                    DetenerYMirarJugador();

                    //Debug.Log("NPC: Ya llegué al destino");
                }
                else
                {
                    //Debug.Log("atttttta");
                    BoxCollider[] colliders = GetComponents<BoxCollider>();
                    colliders[1].enabled = false; // Desactiva el segundo BoxCollider
                }
                checarcambio();

            }
        }

        
        
    }
    void checarcambio()
    {
        if (!Esperando)
        {
            //Debug.Log("AAA deja ver");
            foreach (var c in Posiciones)
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
        Anim.Caminando = true;
        //Debug.Log("AAA ya me muevo");
    }

    //Reinicialas posiciones para que encuentre la mas cercana
    void ReiniciarPosiciones()
    {
        foreach (var c in Posiciones)
        {
            c.Ejecutado = false;
        }
    }

    //Mueve a la poscicion mas cercano dependiendo del dia Lunes
    void MoverAlLugarMasCercano()
    {
        float menorDiferencia = float.MaxValue;
        comportamiento eventoMasCercano = null;

        float tiempoActual = ContolT.HoraActual * 60f + ContolT.MinutoActual;

        foreach (var c in Posiciones)
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
        Anim.Caminando = false;
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

[System.Serializable]
public class comportamiento
{
    public string DiaActual;
    public int HoraActual;
    public float MinutoActual;
    public int pos;

    [HideInInspector] public bool Ejecutado = false;
}