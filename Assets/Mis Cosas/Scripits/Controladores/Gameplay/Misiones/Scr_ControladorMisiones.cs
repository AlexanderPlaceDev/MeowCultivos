using PrimeTween;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_ControladorMisiones : MonoBehaviour
{
    public Scr_CreadorMisiones MisionActual;
    public List<Scr_CreadorMisiones> MisionesExtra;
    public bool MisionCompleta;
    [SerializeField] Scr_CreadorMisiones[] TodasLasMisiones;

    [SerializeField] GameObject BotonesUI;
    [SerializeField] GameObject PanelMisiones;
    [SerializeField] TextMeshProUGUI TextoDescripcion;
    [SerializeField] TextMeshProUGUI TextoPagina;
    [SerializeField] KeyCode Tecla;
    [SerializeField] GameObject Objetos;

    private int PaginaActual = 1;
    private bool[] TeclasPresionadas;
    private float[] TiempoTeclas;
    private bool Oculto = true;

    void Start()
    {
        CargarMisiones();
    }

    void Update()
    {
        ComprobarMision();
        ActualizarInfo();

        if (Input.GetKeyDown(Tecla))
        {
            if (Oculto)
            {
                Oculto = false;
                Tween.PositionX(PanelMisiones.transform, 0, 1);
            }
            else
            {
                Oculto = true;
                Tween.PositionX(PanelMisiones.transform, -610, 1);
            }
        }
    }

    void ActualizarInfo()
    {
        if (MisionActual != null)
        {
            TextoDescripcion.text = MisionCompleta ? MisionActual.DescripcionFinal : MisionActual.Descripcion;
            TextoPagina.text = PaginaActual.ToString() + "/" + (MisionesExtra.Count > 0 ? MisionesExtra.Count.ToString() : "1");
            Objetos.SetActive(MisionActual.Tipo == Scr_CreadorMisiones.Tipos.Caza || MisionActual.Tipo == Scr_CreadorMisiones.Tipos.Recoleccion);
            GuardarMisiones();
        }
        else
        {
            TextoDescripcion.text = "Sin objetivo...";
            TextoPagina.text = "...";
            Objetos.SetActive(false);
        }
    }

    void ComprobarMision()
    {
        if (MisionActual == null)
        {
            MisionCompleta = false;
            return;
        }

        if (MisionActual.Tipo == Scr_CreadorMisiones.Tipos.Teclas)
        {
            ActualizarMisionTeclas();
            BotonesUI.SetActive(!MisionCompleta);
        }
    }

    void ActualizarMisionTeclas()
    {
        if (TeclasPresionadas == null || TeclasPresionadas.Length != MisionActual.Teclas.Length)
        {
            TeclasPresionadas = new bool[MisionActual.Teclas.Length];
            TiempoTeclas = new float[MisionActual.Teclas.Length];
        }

        for (int i = 0; i < MisionActual.Teclas.Length; i++)
        {
            if (TiempoTeclas[i] >= 1)
            {
                TeclasPresionadas[i] = true;
            }
            else
            {
                TiempoTeclas[i] += Input.GetKey(MisionActual.Teclas[i]) ? Time.deltaTime : -Time.deltaTime;
                TiempoTeclas[i] = Mathf.Clamp(TiempoTeclas[i], 0, 1);
            }

            if (BotonesUI != null)
            {
                Image fillImage = BotonesUI.transform.GetChild(i).GetChild(1).GetComponent<Image>();
                fillImage.fillAmount = TiempoTeclas[i];
            }
        }

        MisionCompleta = System.Array.TrueForAll(TeclasPresionadas, t => t);
    }

    public void GuardarMisiones()
    {
        if (MisionActual != null)
        {
            PlayerPrefs.SetString("MisionActual", MisionActual.name);
        }
        else
        {
            PlayerPrefs.SetString("MisionActual", "");
        }

        PlayerPrefs.SetInt("PaginaActual", PaginaActual);
        PlayerPrefs.SetInt("MisionesExtraCantidad", MisionesExtra.Count);

        for (int i = 0; i < MisionesExtra.Count; i++)
        {
            PlayerPrefs.SetString("MisionExtra_" + i, MisionesExtra[i].name);
        }

        PlayerPrefs.Save();
    }

    public void CargarMisiones()
    {
        string nombreMision = PlayerPrefs.GetString("MisionActual", "");
        if (!string.IsNullOrEmpty(nombreMision))
        {
            MisionActual = System.Array.Find(TodasLasMisiones, m => m.name == nombreMision);
        }

        PaginaActual = PlayerPrefs.GetInt("PaginaActual", 1);
        MisionesExtra.Clear();

        int misionesExtraCantidad = PlayerPrefs.GetInt("MisionesExtraCantidad", 0);
        for (int i = 0; i < misionesExtraCantidad; i++)
        {
            string nombreMisionExtra = PlayerPrefs.GetString("MisionExtra_" + i, "");
            if (!string.IsNullOrEmpty(nombreMisionExtra))
            {
                Scr_CreadorMisiones misionExtra = System.Array.Find(TodasLasMisiones, m => m.name == nombreMisionExtra);
                if (misionExtra != null)
                {
                    MisionesExtra.Add(misionExtra);
                }
            }
        }
    }
}