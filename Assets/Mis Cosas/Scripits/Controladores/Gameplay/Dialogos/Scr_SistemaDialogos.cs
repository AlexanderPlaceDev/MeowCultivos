using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_SistemaDialogos : MonoBehaviour
{
    //==================================
    //=== Variables de configuración ===
    //==================================

    [SerializeField] public string NombreNPC; // Nombre del NPC que habla
    [SerializeField] Color ColorNPC; // Color principal para el cuadro de diálogo
    [SerializeField] Color ContrasteNPC; // Color de contraste para la UI

    public TextMeshProUGUI Texto; // Referencia al texto donde se escribe el diálogo
    public Scr_CreadorDialogos[] Dialogos; // Lista de diálogos principales

    public Scr_CreadorDialogos DialogoSecundario; // Diálogo alternativo (misiones secundarias)
    public Scr_CreadorDialogos DialogoArecibir; // Diálogo actual que se está mostrando

    public float letraDelay = 0.1f; // Tiempo entre letras al escribir
    public float Velocidad = 1.0f; // Multiplicador de velocidad de escritura

    //==========================
    //=== Estado del diálogo ===
    //==========================
    public bool EnPausa = true; // Indica si el diálogo está en pausa (esperando input)
    public bool Leyendo = false; // Indica si está escribiendo texto en la pantalla
    public int DialogoActual = 0; // Índice del diálogo actual en la lista principal
    public int DialogoSecundariActual = 0; // Índice para los diálogos secundarios
    public bool Leido = false; // Si el diálogo actual fue completamente leído
    public bool EsCinematica = false; // Si es parte de una cinemática automática
    public int LineaActual = 0; // Línea actual del diálogo mostrado
    private Coroutine currentCoroutine; // Referencia a la coroutine activa

    private Scr_ControladorMisiones ControladorMisiones; // Controlador de misiones de la gata
    private Scr_ActivadorDialogos activadorDialogos; // Referencia al activador de diálogos asociado

    private void Start()
    {
        // Buscar controlador de misiones dentro de la jerarquía de la gata
        if (GameObject.Find("Gata"))
        {
            ControladorMisiones = GameObject.Find("Gata").transform.GetChild(4).GetComponent<Scr_ControladorMisiones>();
        }

        // Obtener referencia al activador de diálogos
        activadorDialogos = GetComponent<Scr_ActivadorDialogos>();
    }

    private void Update()
    {
        // Si el diálogo no está en pausa y es una cinemática o activador válido...
        if (!EnPausa && (EsCinematica || activadorDialogos != null))
        {
            // Escucha input de avance o salto de diálogo
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Leyendo)
                {
                    SaltarDialogo(); // Completa línea instantáneamente
                }
                else
                {
                    SiguienteLinea(); // Avanza a la siguiente línea
                }
            }
        }
    }

    //==========================
    //=== Iniciar un diálogo ===
    //==========================
    public void IniciarDialogo(bool Principal)
    {
        EnPausa = false; // Desbloquear el flujo del diálogo
        Texto.transform.parent.gameObject.SetActive(true); // Activar panel del diálogo
        Texto.text = ""; // Limpiar texto previo

        // Bloquear movimiento del jugador (Gata)
        if (GameObject.Find("Gata") != null)
        {
            var movimiento = GameObject.Find("Gata").GetComponent<Scr_Movimiento>();
            if (movimiento != null)
                movimiento.enabled = false;
        }

        LineaActual = 0; // Reiniciar índice de línea

        // Configurar UI (nombre y colores)
        GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = NombreNPC;
        GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().color = ColorNPC;
        GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetComponent<Image>().color = ContrasteNPC;

        // Elegir qué diálogo cargar
        if (Principal)
        {
            if (DialogoActual >= Dialogos.Length)
            {
                DialogoActual = Dialogos.Length - 1;
            }
            DialogoArecibir = Dialogos[DialogoActual]; // Diálogo principal actual
        }
        else
        {
            Debug.Log("Misiones");
            // Por defecto, usar el diálogo secundario
            DialogoArecibir = DialogoSecundario;

            // Lista de misiones completas de este NPC
            List<Scr_CreadorMisiones> misionesCompletasDelNPC = new List<Scr_CreadorMisiones>();

            for (int i = 0; i < ControladorMisiones.MisionesSecundarias.Count; i++)
            {
                var misionJugador = ControladorMisiones.MisionesSecundarias[i];

                // Validar si pertenece a este NPC
                bool esDelNPC = activadorDialogos.MisionesSecundarias.Any(m => m.TituloMision == misionJugador.TituloMision);

                if (!esDelNPC)
                    continue;

                if (ControladorMisiones.MisionesScompletas[i])
                {
                    misionesCompletasDelNPC.Add(misionJugador);
                }
            }

            // Si hay misiones completas, elegir una al azar para mostrar su diálogo de misión completada
            if (misionesCompletasDelNPC.Count > 0)
            {
                int randomIndex = Random.Range(0, misionesCompletasDelNPC.Count);
                DialogoArecibir = misionesCompletasDelNPC[randomIndex].DialogoMisionCompleta;
            }
            else
            {
                // Si no hay ninguna completa, buscar TODAS las activas de este NPC
                List<Scr_CreadorMisiones> misionesActivasDelNPC = new List<Scr_CreadorMisiones>();

                for (int i = 0; i < ControladorMisiones.MisionesSecundarias.Count; i++)
                {
                    var misionJugador = ControladorMisiones.MisionesSecundarias[i];
                    bool esDelNPC = activadorDialogos.MisionesSecundarias.Any(m => m.TituloMision == misionJugador.TituloMision);

                    if (esDelNPC)
                    {
                        misionesActivasDelNPC.Add(misionJugador);
                    }
                }

                if (misionesActivasDelNPC.Count > 0)
                {
                    int randomIndex = Random.Range(0, misionesActivasDelNPC.Count);
                    DialogoArecibir = misionesActivasDelNPC[randomIndex].DialogoEnMision;
                }
                else
                {
                    // Si no hay ninguna activa ni completa, usar diálogo secundario por defecto
                    DialogoArecibir = DialogoSecundario;
                }
            }


        }

        // Iniciar escritura de la primera línea
        currentCoroutine = StartCoroutine(ReadDialogue());
    }

    //==========================================
    //=== Leer diálogo caracter por caracter ===
    //==========================================
    IEnumerator ReadDialogue()
    {
        Leyendo = true;

        // Escribir letra por letra la línea actual
        foreach (char letter in DialogoArecibir.Lineas[LineaActual].ToCharArray())
        {
            Texto.text += letter;
            yield return new WaitForSeconds(letraDelay * Velocidad);
        }

        Leyendo = false; // Terminó de escribir la línea
    }

    //====================================
    //=== Avanzar a la siguiente línea ===
    //====================================
    public void SiguienteLinea()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine); // Detener escritura actual
        }

        if (LineaActual < DialogoArecibir.Lineas.Length - 1)
        {
            LineaActual++; // Ir a la siguiente línea
            Texto.text = ""; // Limpiar texto anterior
            currentCoroutine = StartCoroutine(ReadDialogue()); // Empezar a escribir
        }
        else
        {
            SaltarDialogo(); // Si no hay más líneas, cerrar diálogo
        }
    }

    //========================================
    //=== Completar diálogo o línea actual ===
    //========================================
    public void SaltarDialogo()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        if (LineaActual < DialogoArecibir.Lineas.Length)
        {
            if (Texto.text == DialogoArecibir.Lineas[LineaActual])
            {
                // Si línea completa, avanzar o cerrar
                Texto.text = "";
                LineaActual++;
                if (LineaActual < DialogoArecibir.Lineas.Length)
                {
                    currentCoroutine = StartCoroutine(ReadDialogue());
                }
                else
                {
                    //=========================
                    //=== Finalizar diálogo ===
                    //=========================
                    LineaActual = 0;
                    EnPausa = true;
                    Leyendo = false;
                    Leido = true;
                    Input.ResetInputAxes();

                    if (activadorDialogos != null)
                        activadorDialogos.DesactivarDialogo(); // Notificar al activador

                    // Asignar misión o avanzar diálogo si es necesario
                    if (!DialogoArecibir.EsUnico)
                    {
                        if (DialogoArecibir.EsMisionPrincipal)
                        {
                            ControladorMisiones.MisionActual = DialogoArecibir.Mision;
                            ControladorMisiones.MisionPrincipal = DialogoArecibir.Mision;
                            ControladorMisiones.GuardarMisiones(); // Guardar inmediatamente después de asignar

                            if (DialogoArecibir.Mision.EsContinua)
                                DialogoActual++;
                        }


                        if (DialogoActual < Dialogos.Length - 1 && ControladorMisiones.MisionActual == null)
                        {
                            DialogoActual++;
                        }

                        DialogoActual = Mathf.Clamp(DialogoActual, 0, Dialogos.Length - 1);
                    }

                    Texto.transform.parent.gameObject.SetActive(false); // Ocultar diálogo
                }
            }
            else
            {
                // Mostrar línea completa al instante si no se terminó de escribir
                Texto.text = DialogoArecibir.Lineas[LineaActual];
            }
        }
    }

    //==========================
    //=== Métodos auxiliares ===
    //==========================
    public void PauseDialogue(bool pause)
    {
        EnPausa = pause;
    }

    public void CambiarDialogo(int Numero)
    {
        DialogoActual = Mathf.Clamp(Numero, 0, Dialogos.Length - 1);
    }

    public void AumentarDialogo()
    {
        if (DialogoActual < Dialogos.Length - 1)
            DialogoActual++;
    }
}
