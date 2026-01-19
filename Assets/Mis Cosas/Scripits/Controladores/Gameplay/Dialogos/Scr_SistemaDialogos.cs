using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scr_SistemaDialogos : MonoBehaviour
{
    //==================================
    //=== Variables de configuración ===
    //==================================
    [SerializeField] public string NombreNPC;
    [SerializeField] Color ColorNPC;
    [SerializeField] Color ContrasteNPC;

    public TextMeshProUGUI Texto;
    public Scr_CreadorDialogos[] Dialogos;
    public Scr_CreadorDialogos DialogoSecundario;
    public Scr_CreadorDialogos DialogoArecibir;

    public float letraDelay = 0.1f;
    public float Velocidad = 1.0f;

    //==========================
    //=== Estado del diálogo ===
    //==========================
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

    public bool BrincaAudios;
    public AudioClip[] SonidoHabla;
    AudioSource source;

    private int ultimoIndiceAudio = -1;


    public GameObject Boton;
    PlayerInput playerInput;
    private InputAction Dialogo;
    InputIconProvider IconProvider;
    private Sprite iconoActualDialogo = null;
    private string textoActualDialogo = "";

    //Tutoria Pelea
    private Tutorial_peleas Tutopeleas;

    private void Start()
    {
        source = GetComponent<AudioSource>();

        if (GameObject.Find("Gata"))
        {
            ControladorMisiones = GameObject.Find("Gata")
                .transform.GetChild(4)
                .GetComponent<Scr_ControladorMisiones>();
        }

        activadorDialogos = GetComponent<Scr_ActivadorDialogos>();
        Tutopeleas = GetComponent<Tutorial_peleas>();
        playerInput = GameObject.Find("Singleton").GetComponent<PlayerInput>();
        IconProvider = GameObject.Find("Singleton").GetComponent<InputIconProvider>();
        Dialogo = playerInput.actions["Dialogo"];
    }

    private void Update()
    {
        if (Texto.transform.parent.gameObject.activeSelf)
        {
            IconProvider.ActualizarIconoUI(Dialogo, Boton.transform, ref iconoActualDialogo, ref textoActualDialogo, false);
        }
        if (!EnPausa && (EsCinematica || activadorDialogos != null))
        {
            if (Dialogo.WasPressedThisFrame())
            {
                if (Leyendo)
                    SaltarDialogo();
                else
                    SiguienteLinea();
            }
        }
        else if (!EnPausa && Tutopeleas != null)
        {
            if (Dialogo.WasPressedThisFrame())
            {
                if (Leyendo)
                    SaltarDialogo();
                else
                    SiguienteLinea();
            }
        }
    }

    //==========================
    //=== Iniciar un diálogo ===
    //==========================
    public void IniciarDialogo(bool Principal)
    {
        EnPausa = false;
        Texto.transform.parent.gameObject.SetActive(true);
        Texto.text = "";

        // Bloquea movimiento
        if (GameObject.Find("Gata") != null)
        {
            var movimiento = GameObject.Find("Gata").GetComponent<Scr_Movimiento>();
            if (movimiento != null) movimiento.enabled = false;
        }

        LineaActual = 0;

        GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1)
            .GetComponent<TextMeshProUGUI>().text = NombreNPC;

        GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(2)
            .GetComponent<Image>().color = ColorNPC;

        GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0)
            .GetComponent<Image>().color = ContrasteNPC;

        if (Principal)
        {
            if (DialogoActual >= Dialogos.Length)
                DialogoActual = Dialogos.Length - 1;

            DialogoArecibir = Dialogos[DialogoActual];
        }
        else
        {
            Debug.Log("Misiones");
            DialogoArecibir = DialogoSecundario;

            List<Scr_CreadorMisiones> misionesCompletasDelNPC = new List<Scr_CreadorMisiones>();

            for (int i = 0; i < ControladorMisiones.MisionesSecundarias.Count; i++)
            {
                var misionJugador = ControladorMisiones.MisionesSecundarias[i];
                bool esDelNPC = activadorDialogos.MisionesSecundarias
                    .Any(m => m.TituloMision == misionJugador.TituloMision);

                if (!esDelNPC) continue;

                if (ControladorMisiones.MisionesScompletas[i])
                    misionesCompletasDelNPC.Add(misionJugador);
            }

            if (misionesCompletasDelNPC.Count > 0)
            {
                int r = Random.Range(0, misionesCompletasDelNPC.Count);
                DialogoArecibir = misionesCompletasDelNPC[r].DialogoMisionCompleta;
            }
            else
            {
                List<Scr_CreadorMisiones> misionesActivas = new List<Scr_CreadorMisiones>();

                for (int i = 0; i < ControladorMisiones.MisionesSecundarias.Count; i++)
                {
                    var m = ControladorMisiones.MisionesSecundarias[i];
                    bool esDelNPC = activadorDialogos.MisionesSecundarias
                        .Any(s => s.TituloMision == m.TituloMision);

                    if (esDelNPC)
                        misionesActivas.Add(m);
                }

                if (misionesActivas.Count > 0)
                {
                    int r = Random.Range(0, misionesActivas.Count);
                    DialogoArecibir = misionesActivas[r].DialogoEnMision;
                }
                else
                {
                    DialogoArecibir = DialogoSecundario;
                }
            }
        }

        currentCoroutine = StartCoroutine(ReadDialogue());
    }
    public void IniciarDialogoTuto()
    {
        EnPausa = false;
        Texto.transform.parent.gameObject.SetActive(true);
        Texto.text = "";

        // Bloquea movimiento
        if (GameObject.Find("Gata") != null)
        {
            var movimiento = GameObject.Find("Gata").GetComponent<Scr_Movimiento>();
            if (movimiento != null) movimiento.enabled = false;
        }

        LineaActual = 0;

        if (DialogoActual >= Dialogos.Length)
            DialogoActual = Dialogos.Length - 1;

        DialogoArecibir = Dialogos[DialogoActual];


        currentCoroutine = StartCoroutine(ReadDialogue());
    }
    IEnumerator ReadDialogue()
    {
        Leyendo = true;

        foreach (char letter in DialogoArecibir.Lineas[LineaActual].ToCharArray())
        {
            if (letter == ' ' && SonidoHabla.Length > 0)
            {
                int h = 0;

                if (SonidoHabla.Length > 1)
                {
                    int intento = 0;
                    do
                    {
                        h = Random.Range(0, SonidoHabla.Length);
                        intento++;
                        if (intento > 10) break;
                    }
                    while (h == ultimoIndiceAudio);
                }

                if (BrincaAudios)
                {
                    source.PlayOneShot(SonidoHabla[h]);
                    ultimoIndiceAudio = h;
                }
                else
                {
                    if (!source.isPlaying)
                    {
                        source.PlayOneShot(SonidoHabla[h]);
                        ultimoIndiceAudio = h;
                    }
                }
            }

            Texto.text += letter;
            yield return new WaitForSeconds(letraDelay * Velocidad);
        }

        Leyendo = false;
    }

    public void SiguienteLinea()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (LineaActual < DialogoArecibir.Lineas.Length - 1)
        {
            LineaActual++;
            Texto.text = "";
            currentCoroutine = StartCoroutine(ReadDialogue());
        }
        else
        {
            SaltarDialogo();
        }
    }

    //==========================================
    //=== Mejora aplicada aquí ==================
    //==========================================
    public void SaltarDialogo()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (LineaActual < DialogoArecibir.Lineas.Length)
        {
            if (Texto.text == DialogoArecibir.Lineas[LineaActual])
            {
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
                        activadorDialogos.DesactivarDialogo();

                    //===========================================================
                    // 🚀 MEJORA: SI TIENE MISIÓN PRINCIPAL, SE ASIGNA SIEMPRE
                    //===========================================================
                    if (DialogoArecibir.EsMisionPrincipal && DialogoArecibir.Mision != null)
                    {
                        ControladorMisiones.MisionActual = DialogoArecibir.Mision;
                        ControladorMisiones.MisionPrincipal = DialogoArecibir.Mision;
                        ControladorMisiones.GuardarMisiones();

                        if (DialogoArecibir.Mision.EsContinua)
                            DialogoActual++;
                    }
                    else
                    {
                        if (!DialogoArecibir.EsUnico && DialogoActual < Dialogos.Length - 1)
                            DialogoActual++;
                    }


                    if (Tutopeleas == null)
                    {
                        Texto.transform.parent.gameObject.SetActive(false);
                    }
                    else
                    {
                        DialogoActual++;
                        Tutopeleas.IniciarDialogo();
                    }
                }
            }
            else
            {
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
