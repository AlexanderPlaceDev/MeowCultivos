using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Scr_SistemaDialogos : MonoBehaviour
{

    public TextMeshProUGUI Texto;
    public Scr_CreadorDialogos[] Dialogos;

    public Scr_CreadorDialogos DialogoSecundario;
    public Scr_CreadorDialogos DialogoExtra;
    public Scr_CreadorDialogos DialogoDeRecompensaSecundario;
    public Scr_CreadorDialogos DialogoArecibir;
    public float letraDelay = 0.1f;
    public float Velocidad = 1.0f;

    public bool recompensarSecundarias=false;
    public bool DiaExtra=false;
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
        activadorDialogos=GetComponent<Scr_ActivadorDialogos>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!EnPausa && EsCinematica)
            {
                if (Leyendo)
                {
                    Debug.Log("salta dialogo");
                    SaltarDialogo();
                }
                else
                {
                    Debug.Log("Siguiente linea");
                    SiguienteLinea();
                }
            }
        }

    }
    public void IniciarDialogo()
    {
        Debug.Log("hola" + DialogoActual);
        EnPausa = false;
        Texto.transform.parent.gameObject.SetActive(true);
        Texto.text = ""; // Limpiar el texto al iniciar un nuevo diálogo
        LineaActual = 0;
        
        if (recompensarSecundarias)
        {
            DialogoArecibir = DialogoDeRecompensaSecundario;
        }
        else if (DiaExtra)
        {
            DialogoArecibir = DialogoExtra;
        }
        else if (activadorDialogos!=null && !activadorDialogos.Principal && !recompensarSecundarias)
        {
            DialogoArecibir = DialogoSecundario;
        }
        else 
        {
            DialogoArecibir = Dialogos[DialogoActual];
        }

        activadorDialogos.vaCambio = DialogoArecibir.cambia;
        currentCoroutine = StartCoroutine(ReadDialogue());
    }

    IEnumerator ReadDialogue()
    {
        Debug.Log("aes"+LineaActual);
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

        if (LineaActual < DialogoArecibir.Lineas.Length -1 ) // Verificar si hay más líneas disponibles
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
        if (DialogoActual < Dialogos.Length ) // Verificar si DialogoActual está dentro de los límites del arreglo Dialogos
        {
            Debug.Log("adsr");
            
        }

        Debug.Log(LineaActual < DialogoArecibir.Lineas.Length);
        if (LineaActual < DialogoArecibir.Lineas.Length) // Verificar si LineaActual está dentro de los límites del arreglo de líneas del diálogo actual
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

                    if (activadorDialogos != null && activadorDialogos.vaCambio)
                    {
                        Debug.LogWarning("qeu");
                        if (activadorDialogos.Principal)
                        {
                            activadorDialogos.Principal = false;
                        }
                        else
                        {
                            activadorDialogos.Principal = true;
                        }

                    }

                    //Asignar Mision
                    if (DialogoArecibir.EsMision && activadorDialogos.Principal)
                    {
                        activadorDialogos.Misionequeespera = DialogoArecibir.Mision;
                        ControladorMisiones.MisionActual = DialogoArecibir.Mision;
                        activadorDialogos.vaCambio = DialogoArecibir.cambia;
                        ControladorMisiones.MisionPrincipal = DialogoArecibir.Mision;
                        //Guardar Dialogo
                        if (GetComponent<Scr_EventosGuardado>() != null)
                        {
                            Debug.Log("Activa Evento");
                            GetComponent<Scr_EventosGuardado>().EventoDialogo(DialogoActual, "Gusano");
                        }
                        if (DialogoArecibir.Mision.EsContinua)
                        {
                            DialogoActual++;
                        }

                        //ControladorMisiones.revisar_objetivos();
                    }
                    else if (DialogoArecibir.EsMision && !activadorDialogos.Principal)
                    {
                        activadorDialogos.Misionesqueespera.Add(DialogoArecibir.Mision);
                        ControladorMisiones.MisionesExtra.Add(DialogoArecibir.Mision);
                        activadorDialogos.quitarMisionSecundaria(DialogoArecibir);
                        ControladorMisiones.MisionesScompletas.Add(false);
                        if (ControladorMisiones.MisionActual == null)
                        {
                            ControladorMisiones.MisionActual = DialogoArecibir.Mision;
                            ControladorMisiones.MisionCompleta = false;
                        }

                        //ControladorMisiones.revisar_objetivos();
                    }
                    

                    if (DialogoActual < Dialogos.Length - 1)
                    {
                        //En caso de no tener mision
                        if (ControladorMisiones != null)
                        {
                            if (ControladorMisiones.MisionActual == null)
                            {
                                DialogoActual++; // Avanzar al siguiente diálogo
                                if (GetComponent<Scr_EventosGuardado>() != null)
                                {
                                    GetComponent<Scr_EventosGuardado>().EventoDialogo(DialogoActual, "Gusano");
                                }

                            }
                        }


                    }
                    Texto.transform.parent.gameObject.SetActive(false);
                }
            }

            else
            {
                Debug.Log("ad");
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
