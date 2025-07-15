using System.Collections;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class Scr_SistemaDialogos : MonoBehaviour
{

    [SerializeField] public string NombreNPC;
    [SerializeField] Color ColorNPC;
    [SerializeField] Color ContrasteNPC;

    public TextMeshProUGUI Texto;
    public Scr_CreadorDialogos[] Dialogos;

    public Scr_CreadorDialogos DialogoSecundario;
    public Scr_CreadorDialogos DialogoExtra;
    public Scr_CreadorDialogos DialogoDeRecompensaSecundario;
    public Scr_CreadorDialogos DialogoArecibir;
    public float letraDelay = 0.1f;
    public float Velocidad = 1.0f;

    public bool recompensarSecundarias = false;
    public bool DiaExtra = false;
    public bool EnPausa = true;
    public bool Leyendo = false;
    public int DialogoActual = 0;
    public int DialogoSecundariActual = 0;
    public bool Leido = false;
    public bool EsCinematica = false;
    public int LineaActual = 0;
    private Coroutine currentCoroutine;
    private Scr_ControladorMisiones ControladorMisiones;
    private Scr_ActivadorDialogos activadorDialogos;

    private void Start()
    {
        if (GameObject.Find("Gata"))
        {
            ControladorMisiones = GameObject.Find("Gata").transform.GetChild(4).GetComponent<Scr_ControladorMisiones>();
        }
        activadorDialogos = GetComponent<Scr_ActivadorDialogos>();
    }

    private void Update()
    {
        if (!EnPausa && (EsCinematica || activadorDialogos != null))

        {
            if (Input.GetKeyDown(KeyCode.E)) // Usa la tecla que quieras
            {
                if (Leyendo)
                {
                    SaltarDialogo(); // Autocompleta la línea actual
                }
                else
                {
                    SiguienteLinea(); // Avanza a la siguiente línea
                }
            }
        }

    }

    public void IniciarDialogo()
    {
        EnPausa = false;
        Texto.transform.parent.gameObject.SetActive(true);
        Texto.text = ""; // Limpiar texto

        // 🔥 Bloquear movimiento al iniciar diálogo
        if (GameObject.Find("Gata") != null)
        {
            var movimiento = GameObject.Find("Gata").GetComponent<Scr_Movimiento>();
            if (movimiento != null)
                movimiento.enabled = false;
        }


        LineaActual = 0;

        // Configura UI
        GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = NombreNPC;
        GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().color = ColorNPC;
        GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetComponent<Image>().color = ContrasteNPC;

        // Selección de diálogo
        if (recompensarSecundarias && DialogoDeRecompensaSecundario != null)
        {
            DialogoArecibir = DialogoDeRecompensaSecundario;
        }
        else if (DiaExtra && DialogoExtra != null)
        {
            DialogoArecibir = DialogoExtra;
        }
        else if (activadorDialogos != null && !recompensarSecundarias && DialogoSecundario != null)
        {
            DialogoArecibir = DialogoSecundario;
        }
        else
        {
            DialogoArecibir = Dialogos[DialogoActual];
        }

        currentCoroutine = StartCoroutine(ReadDialogue());
    }

    IEnumerator ReadDialogue()
    {
        Leyendo = true;
        foreach (char letter in DialogoArecibir.Lineas[LineaActual].ToCharArray())
        {
            Texto.text += letter;
            yield return new WaitForSeconds(letraDelay * Velocidad);
        }
        Leyendo = false;
    }

    public void SiguienteLinea()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        if (LineaActual < DialogoArecibir.Lineas.Length - 1) // Verificar si hay más líneas disponibles
        {
            LineaActual++; // Incrementar el índice de la línea actual
            Texto.text = ""; // Limpiar el texto antes de mostrar la siguiente línea
            currentCoroutine = StartCoroutine(ReadDialogue());
        }
        else
        {
            SaltarDialogo(); // Si estamos en la última línea, avanzamos al siguiente diálogo
        }
    }

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
                Texto.text = ""; // Limpiar el texto antes de mostrar la siguiente línea
                LineaActual++; // Avanzar a la siguiente línea
                if (LineaActual < DialogoArecibir.Lineas.Length)
                {
                    currentCoroutine = StartCoroutine(ReadDialogue());
                }
                else
                {
                    // Aquí termina el diálogo
                    LineaActual = 0;
                    EnPausa = true;
                    Leyendo = false;
                    Leido = true;
                    Input.ResetInputAxes(); // 🛑 Limpia inputs pendientes

                    // Regresar cámara
                    if (activadorDialogos != null)
                        activadorDialogos.DesactivarDialogo();

                    // ✅ SOLO si NO es único, asignar misión y avanzar diálogo
                    if (!DialogoArecibir.EsUnico)
                    {
                        if (DialogoArecibir.EsMisionPrincipal)
                        {
                            ControladorMisiones.MisionActual = DialogoArecibir.Mision;
                            ControladorMisiones.MisionPrincipal = DialogoArecibir.Mision;
                            if (DialogoArecibir.Mision.EsContinua)
                                DialogoActual++;
                        }

                        if (DialogoActual < Dialogos.Length - 1 && ControladorMisiones != null)
                        {
                            if (ControladorMisiones.MisionActual == null)
                                DialogoActual++;
                        }
                    }

                    Texto.transform.parent.gameObject.SetActive(false);
                }

            }
            else
            {
                Texto.text = DialogoArecibir.Lineas[LineaActual];
                recompensarSecundarias = false;
                DiaExtra = false;
            }
        }
    }

    public void PauseDialogue(bool pause)
    {
        EnPausa = pause;
    }

    public void CambiarDialogo(int Numero)
    {
        DialogoActual = Numero;
    }

    public void AumentarDialogo()
    {
        DialogoActual++;
    }

}
