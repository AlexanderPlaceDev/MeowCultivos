using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_CofreMarvin : MonoBehaviour
{
    [SerializeField] Scr_CreadorObjetos[] ObjetosNecesarios;
    [SerializeField] int[] CantidadesNecesarias;
    [SerializeField] Image[] Items;
    [SerializeField] private bool BorrarInfo;
    Scr_Inventario Inventario;

    // --- ROTACIÓN ---
    [Header("Rotación del child(3)")]
    [Tooltip("Eje de rotación (vector). Por ejemplo (0,1,0) para Y.")]
    [SerializeField] Vector3 EjeRotacion = Vector3.up;
    [Tooltip("Puedes arrastrar aquí el child que debe rotar (si no, usa transform.GetChild(3)).")]
    [SerializeField] Transform ChildToRotate;
    [Tooltip("Panel/Canvas que activa el UI (si no se asigna intentará usar transform.GetChild(1)).")]
    [SerializeField] GameObject CanvasUI;
    [SerializeField] bool MostrarLogs = false;

    Quaternion rotacionOriginal;
    Quaternion rotacionObjetivo;
    bool estabaActivo = false;

    void Start()
    {
        if (BorrarInfo)
        {
            for (int i = 0; i < ObjetosNecesarios.Length; i++)
            {
                PlayerPrefs.SetInt(KeyDonado(i), 0);
            }
        }

        // Si ya se completó antes, no mostrar más el cofre
        if (PlayerPrefs.GetInt("Rango Barra Industrial5", 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        Inventario = GameObject.Find("Gata").transform.GetChild(7).GetComponent<Scr_Inventario>();

        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].sprite = ObjetosNecesarios[i].IconoInventario;
            ActualizarTexto(i);
            ActualizarBotones(i);
        }

        // Fallbacks: CanvasUI y ChildToRotate si no fueron asignados en inspector
        if (CanvasUI == null)
        {
            if (transform.childCount > 1)
                CanvasUI = transform.GetChild(1).gameObject;
            else
                CanvasUI = gameObject; // fallback al mismo objeto si no hay child(1)
        }

        if (ChildToRotate == null)
        {
            if (transform.childCount > 3)
                ChildToRotate = transform.GetChild(3);
            else
                ChildToRotate = null;
        }

        // Preparar rotaciones
        if (ChildToRotate != null)
        {
            rotacionOriginal = ChildToRotate.localRotation;

            Vector3 axis = EjeRotacion;
            if (axis == Vector3.zero) axis = Vector3.up; // evita vectores nulos
            axis = axis.normalized;

            // Rotación objetivo = rotación original + 90° alrededor del eje local
            rotacionObjetivo = rotacionOriginal * Quaternion.AngleAxis(90f, axis);

            // Estado inicial según el canvas
            estabaActivo = CanvasUI != null ? CanvasUI.activeInHierarchy : gameObject.activeInHierarchy;

            // Aplicar estado inicial
            ChildToRotate.localRotation = estabaActivo ? rotacionObjetivo : rotacionOriginal;
        }
        else
        {
            if (MostrarLogs) Debug.LogWarning("Scr_CofreMarvin: no se encontró child a rotar (ChildToRotate == null).");
        }
    }

    void Update()
    {
        for (int i = 0; i < ObjetosNecesarios.Length; i++)
        {
            ActualizarBotones(i);
        }

        RevisarCompletado();

        // Manejo de rotación al cambiar estado del canvas
        if (ChildToRotate != null)
        {
            bool activo = CanvasUI != null ? CanvasUI.activeInHierarchy : gameObject.activeInHierarchy;
            if (activo != estabaActivo)
            {
                if (activo)
                {
                    // activar -> ir a rotación objetivo
                    ChildToRotate.localRotation = rotacionObjetivo;
                    if (MostrarLogs) Debug.Log("Scr_CofreMarvin: roté hacia objetivo (activo).");
                }
                else
                {
                    // desactivar -> restaurar rotación original
                    ChildToRotate.localRotation = rotacionOriginal;
                    if (MostrarLogs) Debug.Log("Scr_CofreMarvin: restauré rotación original (desactivado).");
                }
                estabaActivo = activo;
            }
        }
    }

    // ========================
    //  BOTONES (+) y (-)
    // ========================

    public void Agregar(int index)
    {
        if (index < 0 || index >= ObjetosNecesarios.Length) return;

        int donado = PlayerPrefs.GetInt(KeyDonado(index), 0);
        int requerido = CantidadesNecesarias[index];
        Scr_CreadorObjetos obj = ObjetosNecesarios[index];

        int cantidadInv = GetCantidadInventario(obj);
        if (cantidadInv > 0 && donado < requerido)
        {
            if (DecrementarUnoDelInventario(obj))
            {
                donado++;
                PlayerPrefs.SetInt(KeyDonado(index), donado);
                PlayerPrefs.Save();

                ActualizarTexto(index);
                ActualizarBotones(index);

                RevisarCompletado();
            }
        }
    }

    public void Quitar(int index)
    {
        if (index < 0 || index >= ObjetosNecesarios.Length) return;

        int donado = PlayerPrefs.GetInt(KeyDonado(index), 0);
        if (donado <= 0) return;

        Scr_CreadorObjetos obj = ObjetosNecesarios[index];

        // --- CAMBIO: En vez de usar AgregarObjeto(), lo agregamos directo al inventario ---
        int j = 0;
        bool agregado = false;
        foreach (var it in Inventario.Objetos)
        {
            if (it == obj)
            {
                Inventario.Cantidades[j] += 1;
                agregado = true;
                break;
            }
            j++;
        }
        if (!agregado)
        {
            if (MostrarLogs) Debug.LogWarning("Scr_CofreMarvin: el objeto no existe en Inventario.Objetos, no se pudo agregar directamente.");
            // opcional: aquí podrías manejar agregar un nuevo slot si tu inventario lo permite
        }

        donado--;
        PlayerPrefs.SetInt(KeyDonado(index), donado);
        PlayerPrefs.Save();

        ActualizarTexto(index);
        ActualizarBotones(index);
    }

    // ========================
    //  UI
    // ========================

    void ActualizarTexto(int index)
    {
        int donado = PlayerPrefs.GetInt(KeyDonado(index), 0);
        Items[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            donado + " / " + CantidadesNecesarias[index];
    }

    void ActualizarBotones(int index)
    {
        int donado = PlayerPrefs.GetInt(KeyDonado(index), 0);
        int requerido = CantidadesNecesarias[index];
        Scr_CreadorObjetos obj = ObjetosNecesarios[index];

        var cont = Items[index].transform.GetChild(0);
        var btnMas = cont.GetChild(0).gameObject;
        var btnMenos = cont.GetChild(1).gameObject;

        int cantidadInv = GetCantidadInventario(obj);

        bool puedeSumar = (cantidadInv > 0) && (donado < requerido);
        btnMas.SetActive(puedeSumar);

        bool puedeRestar = (donado > 0);
        btnMenos.SetActive(puedeRestar);
    }

    // ========================
    //  HELPERS Inventario
    // ========================

    int GetCantidadInventario(Scr_CreadorObjetos obj)
    {
        int j = 0;
        foreach (var it in Inventario.Objetos)
        {
            if (it == obj)
                return Inventario.Cantidades[j];
            j++;
        }
        return 0;
    }

    bool DecrementarUnoDelInventario(Scr_CreadorObjetos obj)
    {
        int j = 0;
        foreach (var it in Inventario.Objetos)
        {
            if (it == obj)
            {
                if (Inventario.Cantidades[j] > 0)
                {
                    Inventario.Cantidades[j]--;
                    return true;
                }
                return false;
            }
            j++;
        }
        return false;
    }

    string KeyDonado(int index) => "CantidadDonadaMarvinRango1" + index;

    void RevisarCompletado()
    {
        for (int i = 0; i < ObjetosNecesarios.Length; i++)
        {
            int donado = PlayerPrefs.GetInt(KeyDonado(i), 0);
            if (donado < CantidadesNecesarias[i])
                return; // aún falta algo
        }

        // Todo completo
        PlayerPrefs.SetInt("Rango Barra Industrial5", 1);
        PlayerPrefs.Save();

        GetComponent<Scr_ActivadorMenuEstructuraFijo>().CerrarTablero();
        gameObject.SetActive(false);
    }

    public void BotonCerrar()
    {
        Debug.Log("Entra");
        GetComponent<Scr_ActivadorMenuEstructuraFijo>().CerrarTablero();
        // aquí asumes que el panel está en child(1)
        if (transform.childCount > 1)
            transform.GetChild(1).gameObject.SetActive(false);
    }
}
