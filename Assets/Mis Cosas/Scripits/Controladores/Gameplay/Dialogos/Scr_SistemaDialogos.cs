using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Scr_SistemaDialogos : MonoBehaviour
{

    public TextMeshProUGUI Texto;
    public Scr_CreadorDialogos[] Dialogos;

    public Scr_CreadorDialogos DialogoSecundario;
    public Scr_CreadorDialogos DialogoArecibir;
    public float letraDelay = 0.1f;
    public float Velocidad = 1.0f;

    public bool EnPausa = true;
    public bool Leyendo = false;
    public int DialogoActual = 0;
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
                    SaltarDialogo();
                }
                else
                {
                    SiguienteLinea();
                }
            }
        }

    }
    public void IniciarDialogo()
    {
        EnPausa = false;
        Texto.transform.parent.gameObject.SetActive(true);
        Texto.text = ""; // Limpiar el texto al iniciar un nuevo di�logo
        LineaActual = 0;
        if (!activadorDialogos.Principal)
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

        if (LineaActual < DialogoArecibir.Lineas.Length - 1 ) // Verificar si hay m�s l�neas disponibles
        {
            LineaActual++; // Incrementar el �ndice de la l�nea actual
            Texto.text = ""; // Limpiar el texto antes de mostrar la siguiente l�nea
            currentCoroutine = StartCoroutine(ReadDialogue());
        }
        else
        {
            SaltarDialogo(); // Si estamos en la �ltima l�nea, avanzamos al siguiente di�logo
        }
    }

    public void SaltarDialogo()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        if (DialogoActual < Dialogos.Length) // Verificar si DialogoActual est� dentro de los l�mites del arreglo Dialogos
        {
            if (LineaActual < DialogoArecibir.Lineas.Length) // Verificar si LineaActual est� dentro de los l�mites del arreglo de l�neas del di�logo actual
            {
                if (Texto.text == DialogoArecibir.Lineas[LineaActual])
                {
                    Texto.text = ""; // Limpiar el texto antes de mostrar la siguiente l�nea
                    LineaActual++; // Avanzar a la siguiente l�nea
                    if (LineaActual < DialogoArecibir.Lineas.Length)
                    {
                        currentCoroutine = StartCoroutine(ReadDialogue());
                    }
                    else
                    {
                        // Aqu� termina el di�logo
                        LineaActual = 0;
                        EnPausa = true;
                        Leyendo = false;
                        Leido = true;

                        //Asignar Mision
                        if (DialogoArecibir.EsMision && activadorDialogos.Principal)
                        {
                            activadorDialogos.Misionequeespera = DialogoArecibir.Mision;
                            ControladorMisiones.MisionActual = DialogoArecibir.Mision;
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
                        }
                        else
                        {
                            activadorDialogos.Misionesqueespera.Add(DialogoArecibir.Mision);
                            ControladorMisiones.MisionesExtra.Add(DialogoArecibir.Mision);
                        }

                        if (DialogoActual < Dialogos.Length - 1)
                        {
                            //En caso de no tener mision
                            if (ControladorMisiones != null)
                            {
                                if (ControladorMisiones.MisionActual == null)
                                {
                                    DialogoActual++; // Avanzar al siguiente di�logo
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
                    Texto.text = DialogoArecibir.Lineas[LineaActual];
                }
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
