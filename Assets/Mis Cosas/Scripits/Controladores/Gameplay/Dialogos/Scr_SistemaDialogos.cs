using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Scr_SistemaDialogos : MonoBehaviour
{

    public TextMeshProUGUI Texto;
    public Scr_CreadorDialogos[] Dialogos;
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

    private void Start()
    {
        if (GameObject.Find("Gata"))
        {
            ControladorMisiones = GameObject.Find("Gata").transform.GetChild(3).GetComponent<Scr_ControladorMisiones>();
        }
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
        Texto.text = ""; // Limpiar el texto al iniciar un nuevo diálogo
        LineaActual = 0;
        currentCoroutine = StartCoroutine(ReadDialogue());
    }

    IEnumerator ReadDialogue()
    {
        Leyendo = true;
        foreach (char letter in Dialogos[DialogoActual].Lineas[LineaActual].ToCharArray())
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

        if (LineaActual < Dialogos[DialogoActual].Lineas.Length - 1) // Verificar si hay más líneas disponibles
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

        if (DialogoActual < Dialogos.Length) // Verificar si DialogoActual está dentro de los límites del arreglo Dialogos
        {
            if (LineaActual < Dialogos[DialogoActual].Lineas.Length) // Verificar si LineaActual está dentro de los límites del arreglo de líneas del diálogo actual
            {
                if (Texto.text == Dialogos[DialogoActual].Lineas[LineaActual])
                {
                    Texto.text = ""; // Limpiar el texto antes de mostrar la siguiente línea
                    LineaActual++; // Avanzar a la siguiente línea
                    if (LineaActual < Dialogos[DialogoActual].Lineas.Length)
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

                        //Asignar Mision
                        if (Dialogos[DialogoActual].EsMision)
                        {
                            ControladorMisiones.MisionActual = Dialogos[DialogoActual].Mision;
                            //Guardar Dialogo
                            if (GetComponent<Scr_EventosGuardado>() != null)
                            {
                                GetComponent<Scr_EventosGuardado>().EventoDialogo(DialogoActual, "Gusano");
                            }
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
                    Texto.text = Dialogos[DialogoActual].Lineas[LineaActual];
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
