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
    public bool Leido = false;
    private int LineaActual = 0;
    private int DialogoActual = 0;
    private Coroutine currentCoroutine;

    void IniciarDialogo()
    {
        Texto.text = ""; // Limpiar el texto al iniciar un nuevo di�logo
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

    public void SiguienteLetra()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        if (LineaActual < Dialogos[DialogoActual].Lineas.Length)
        {
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
            if (LineaActual < Dialogos[DialogoActual].Lineas.Length) // Verificar si LineaActual est� dentro de los l�mites del arreglo de l�neas del di�logo actual
            {
                if (Texto.text == Dialogos[DialogoActual].Lineas[LineaActual])
                {
                    Texto.text = ""; // Limpiar el texto antes de mostrar la siguiente l�nea
                    LineaActual++; // Avanzar a la siguiente l�nea
                    if (LineaActual < Dialogos[DialogoActual].Lineas.Length)
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
                        if (DialogoActual < Dialogos.Length-1)
                        {
                            DialogoActual++; // Avanzar al siguiente di�logo
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
}
