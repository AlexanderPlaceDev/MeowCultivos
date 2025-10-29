using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ObjetosAgregados : MonoBehaviour
{
    public List<Scr_CreadorObjetos> Lista = new List<Scr_CreadorObjetos>();
    public List<int> Cantidades = new List<int>();
    [SerializeField] GameObject[] Iconos;
    public float[] Tiempo; // ahora lo inicializamos en Start en función de Iconos.Length

    // Referencia al inventario de la gata (para actualizar cantidades)
    [SerializeField] private Scr_Inventario Inventario;

    // Referencias para Canvas Dinero / XP
    [SerializeField] private GameObject canvasXP;
    [SerializeField] private GameObject canvasDinero;
    private TextMeshProUGUI XPText;
    private TextMeshProUGUI DineroText;
    private Animator XPAnimator;
    private Animator DineroAnimator;
    private AudioSource dineroAudioSource;
    private AudioSource xpAudioSource;

    void Start()
    {
        // Asegurar que Tiempo tenga el tamaño correcto
        if (Iconos != null)
        {
            if (Tiempo == null || Tiempo.Length != Iconos.Length)
            {
                Tiempo = new float[Iconos.Length];
                for (int t = 0; t < Tiempo.Length; t++) Tiempo[t] = 2f; // valor por defecto
            }
        }

        // Si no nos dieron Inventario por inspector, intentar buscarlo en la escena
        if (Inventario == null)
        {
            Inventario = FindObjectOfType<Scr_Inventario>();
            if (Inventario == null)
                Debug.LogWarning("No se encontró Scr_Inventario en la escena. No se actualizará el inventario al consumir objetos.");
        }

        // Cachear referencias al Canvas Dinero / XP
        if (canvasDinero != null)
        {
            DineroText = canvasDinero.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            DineroAnimator = canvasDinero.GetComponent<Animator>();
            dineroAudioSource = canvasDinero.GetComponent<AudioSource>();
        }

        if (canvasXP != null)
        {
            XPText = canvasXP.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            XPAnimator = canvasXP.GetComponent<Animator>();
            xpAudioSource = canvasXP.GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        MostrarObjetosEnCanvas();

        // Gestiona el desvanecimiento de iconos
        if (Iconos == null || Iconos.Length == 0) return;

        // Recorremos por índice y vigilamos cada slot (si hay item asociado a ese slot)
        for (int i = 0; i < Iconos.Length; i++)
        {
            // Si en este slot no hay item (Lista más corta que i), reiniciamos visual y tiempo y continuamos
            if (i >= Lista.Count || Lista[i] == null)
            {
                // limpiar slot visual
                var imgComp = Iconos[i].GetComponent<Image>();
                if (imgComp != null) imgComp.sprite = null;
                var txtChild = Iconos[i].transform.childCount > 0 ? Iconos[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>() : null;
                if (txtChild != null) txtChild.text = "";
                // reiniciar tiempo del slot para la próxima vez que se use
                Tiempo[i] = 2f;
                continue;
            }

            // Si hay un icono cargado y tiempo > 0, reducirlo
            Tiempo[i] -= Time.deltaTime;
            // Actualizar alfa visual (clamp)
            float alpha = Mathf.Clamp01(Tiempo[i] / 2f); // normalizamos: 2s => 1, 0s => 0
            var image = Iconos[i].GetComponent<Image>();
            if (image != null) image.color = new Color(1f, 1f, 1f, alpha);
            var cantidadText = Iconos[i].transform.childCount > 0 ? Iconos[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>() : null;
            if (cantidadText != null) cantidadText.color = new Color(1f, 1f, 1f, alpha);

            // Si el tiempo llegó a cero o menos, "consumir" el item: agregar a inventario y quitar la entrada en la misma posición i
            if (Tiempo[i] <= 0f)
            {
                // Agregar al inventario (si existe)
                Scr_CreadorObjetos item = Lista[i];
                int cantidadAAgregar = 0;
                if (i < Cantidades.Count)
                    cantidadAAgregar = Cantidades[i];

                if (Inventario != null && item != null)
                {
                    // Buscar el índice en Inventario por nombre (como tu otro código hace)
                    bool agregado = false;
                    for (int j = 0; j < Inventario.Objetos.Length; j++)
                    {
                        if (Inventario.Objetos[j] != null && Inventario.Objetos[j].Nombre == item.Nombre)
                        {
                            Inventario.Cantidades[j] += cantidadAAgregar;
                            agregado = true;
                            break;
                        }
                    }

                    if (!agregado)
                    {
                        // Si no existe en Inventario, podrías optar por expandir arrays o loggear.
                        Debug.LogWarning($"El item '{item.Nombre}' no existe en Inventario. No se agregó la cantidad {cantidadAAgregar}.");
                    }
                    else
                    {
                        Debug.Log($"Se agregaron {cantidadAAgregar} x {item.Nombre} al inventario.");
                    }
                }
                else
                {
                    Debug.LogWarning("Inventario nulo o item nulo al intentar agregar desde Scr_ObjetosAgregados.");
                }

                // Limpiar el slot visual inmediatamente
                if (image != null) image.sprite = null;
                if (cantidadText != null) cantidadText.text = "";

                // Quitar la entrada en la posición i de las listas (importante: usamos i, no 0)
                if (i < Lista.Count) Lista.RemoveAt(i);
                if (i < Cantidades.Count) Cantidades.RemoveAt(i);

                // Reiniciar el tiempo del slot para futuros items
                Tiempo[i] = 2f;

                // Como removimos el elemento i, las siguientes entradas se "corren" a la izquierda; 
                // para mantener coherencia en este frame, bajamos i para volver a chequear en la misma posición
                i--;
            }
        }
    }

    public void AgregarExperiencia(int cantidadXP)
    {
        int xpActual = PlayerPrefs.GetInt("XPActual", 0) + cantidadXP;
        int xpSiguiente = PlayerPrefs.GetInt("XPSiguiente", 10);
        PlayerPrefs.SetInt("XPActual", xpActual);

        if (xpActual >= xpSiguiente)
        {
            PlayerPrefs.SetInt("XPActual", 0);
            PlayerPrefs.SetInt("Nivel", PlayerPrefs.GetInt("Nivel", 0) + 1);
            PlayerPrefs.SetInt("XPSiguiente", xpSiguiente * 2);
            PlayerPrefs.SetInt("PuntosDeHabilidad", PlayerPrefs.GetInt("PuntosDeHabilidad", 0) + 3);
            if (XPText != null) XPText.text = "LV.+1";
            if (xpAudioSource != null) xpAudioSource.Play();
        }
        else
        {
            if (XPText != null) XPText.text = "XP + " + cantidadXP;
        }

        if (XPAnimator != null && !XPAnimator.GetCurrentAnimatorStateInfo(0).IsName("Desaparecer"))
        {
            XPAnimator.Play("Desaparecer");
        }
    }

    public void AgregarDinero(int cantidad)
    {
        if (canvasDinero == null) return;

        // Suma el dinero al total
        int dineroActual = PlayerPrefs.GetInt("Dinero", 0) + cantidad;
        PlayerPrefs.SetInt("Dinero", dineroActual);

        // Muestra el dinero ganado en el Canvas Dinero
        if (DineroText != null) DineroText.text = "+$" + cantidad.ToString("N0");

        if (DineroAnimator != null && !DineroAnimator.GetCurrentAnimatorStateInfo(0).IsName("Desaparecer"))
        {
            DineroAnimator.Play("Desaparecer");
            if (dineroAudioSource != null) dineroAudioSource.Play();
        }
    }

    private void MostrarObjetosEnCanvas()
    {
        // Limpiamos todos los iconos primero
        for (int k = 0; k < Iconos.Length; k++)
        {
            var image = Iconos[k].GetComponent<Image>();
            var txt = Iconos[k].transform.childCount > 0 ? Iconos[k].transform.GetChild(0).GetComponent<TextMeshProUGUI>() : null;
            if (k < Lista.Count && Lista[k] != null)
            {
                image.sprite = Lista[k].Icono;
                if (txt != null) txt.text = "+" + Cantidades[k];
                // reset alpha visual en caso de que haya sido modificado
                float alpha = Mathf.Clamp01(Tiempo[k] / 2f);
                image.color = new Color(1f, 1f, 1f, alpha);
                if (txt != null) txt.color = new Color(1f, 1f, 1f, alpha);
            }
            else
            {
                // limpiar slot si no hay item
                if (image != null) image.sprite = null;
                if (txt != null) txt.text = "";
            }
        }
    }

    // Método público para agregar un item a la cola visual (lo usan otras clases)
    public void AgregarItemVisual(Scr_CreadorObjetos item, int cantidadAgregar)
    {
        if (item == null) return;
        Lista.Add(item);
        Cantidades.Add(cantidadAgregar);

        // Buscar primer slot libre en Tiempo[] y setear su tiempo a 2s
        for (int t = 0; t < Tiempo.Length; t++)
        {
            if (t >= Lista.Count - 1) // el item recién añadido está en Lista.Count-1
            {
                Tiempo[t] = 2f;
                break;
            }
        }
    }
}
