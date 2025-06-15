using PrimeTween;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Scr_CreadorMisiones;

public class Scr_ControladorMisiones : MonoBehaviour
{
    public Scr_CreadorMisiones MisionActual;
    public Scr_CreadorMisiones MisionPrincipal;
    public bool MisionCompleta;
    public bool MisionPCompleta;
    public List<Scr_CreadorMisiones> MisionesExtra;
    public MisionesData MisionData;
    public Scr_CreadorMisiones[] TodasLasMisiones;
    public List<bool> MisionesPcompletas;
    public List<bool> MisionesScompletas;
    public int[] enemigosEliminados;
    private Scr_Inventario inventario;
    private Transform Gata;
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

    public string[] NameEnemiCazado;
    public int[] cazados;


    public List<bool> valido;

    public string[] TargetExplorados;
    void Start()
    {

        Gata = GameObject.Find("Gata").transform;
        inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
        //CargarMisiones();
        ActualizarInfo();
    }

    void Update()
    {
        //ComprobarMision();
        ActualizarInfo();

        if (MisionActual != null && MisionActual.Tipo == Scr_CreadorMisiones.Tipos.Teclas)
        {
            ActualizarMisionTeclas();
            BotonesUI.SetActive(!MisionCompleta);
        }

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

        MisionPCompleta = System.Array.TrueForAll(TeclasPresionadas, t => t);
        if (MisionActual != null && MisionPrincipal==MisionActual) 
        {
            MisionCompleta = MisionPCompleta;
        }
    }

    public void SeleccionMisionActual(Scr_CreadorMisiones nuevaMision, bool complete)
    {
        MisionActual = nuevaMision;
        MisionCompleta = complete;
    }

    public bool revisarMisionPrincipal()
    {
        if (MisionPrincipal == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Terminar_MisionPrincipal()
    {
        if (MisionPrincipal != null && MisionPCompleta)
        {
            MisionesPcompletas.Add(MisionPrincipal);
            MisionPrincipal = null;
            MisionPCompleta = false;
        }
    }

    public void RevisarTodasLasMisionesSecundarias()
    {
        // Misión Principal
        /*
        if (MisionPrincipal != null && !MisionPCompleta)
        {
            ComprobarProgreso(MisionPrincipal, ref MisionPCompleta);

        }*/
        if (MisionesExtra != null)
        {
            // Misiones Secundarias
            for (int i = 0; i < MisionesExtra.Count; i++)
            {
                if (!MisionesScompletas[i])
                {
                    bool completada = false;
                    ComprobarProgreso(MisionesExtra[i], ref completada);
                    if (completada)
                    {
                        MisionesScompletas[i] = true;
                        if (MisionesScompletas[i] == MisionActual)
                        {
                            MisionCompleta=true;
                        }
                    }
                }
            }
        }
        

        ActualizarInfo(); // Refresca el panel de UI si es necesario
    }

    public void TerminarMisionSexundaria(Scr_CreadorMisiones mision)
    {
        for (int i = 0; i < MisionesExtra.Count; i++)
        {
            if (mision == MisionesExtra[i])
            {
                if (mision.DaObjetos && mision.QuitaObjetos)
                {
                    quitarObjetos(mision);
                    recompensa(mision);
                }/*
                else if (!mision.DaObjetos && !mision.QuitaObjetos)
                {
                    quitarObjetos(mision);
                }*/
                else if (mision.DaObjetos && !mision.QuitaObjetos)
                {
                    recompensa(mision);
                }
                MisionesExtra.RemoveAt(i);
                MisionesScompletas.RemoveAt(i);
                break;
            }
        }
    }
    public void quitarObjetos(Scr_CreadorMisiones mision)
    {
        for (int i = 0; i < mision.ObjetosNecesarios.Length; i++)
        {
            for (int u = 0; u < inventario.Objetos.Length; u++)
            {
                if (mision.ObjetosNecesarios[i] == inventario.Objetos[u])
                {
                    inventario.Cantidades[u] = inventario.Cantidades[u] - mision.CantidadesQuita[i];
                }
            }
        }
    }
    
    public void recompensa(Scr_CreadorMisiones mision)
    {
        for (int i = 0; i < mision.ObjetosRecompensa.Length; i++)
        {
            for (int u = 0; u < inventario.Objetos.Length; u++)
            {
                if (mision.ObjetosRecompensa[i] == inventario.Objetos[u])
                {
                    UnityEngine.Debug.LogWarning("Recompesa "+ mision.ObjetosRecompensa[i] );
                    inventario.Cantidades[u] = inventario.Cantidades[u] + mision.CantidadesDa[i];
                }
            }
        }
    }
    void ComprobarProgreso(Scr_CreadorMisiones mision, ref bool completada)
    {
        /*if (mision.prioridad == prioridadM.Principal)
        {

        }
        else 
        {
            
        }*/

        switch (mision.Tipo)
        {
            case Tipos.Caza:
                completada = VerificarCaza(mision);
                break;
            case Tipos.Recoleccion:
                completada = VerificarRecoleccion(mision);
                break;
            case Tipos.Exploracion:
                completada = VerificarExploracion(mision);
                break;
        }
    }
    public bool VerificarCaza(Scr_CreadorMisiones mision)
    {
        valido.Clear();
        for (int i = 0; i < mision.Objetivocaza.Length; i++)
        {
            for (int u = 0; u < NameEnemiCazado.Length; u++)
            {
                if (mision.Objetivocaza[i].ToString() == NameEnemiCazado[u])
                {
                    if (cazados[u] >= mision.cantidad_caza[i])
                    {
                        valido.Add(true);
                    }
                    else
                    {
                        valido.Add(false);
                    }

                }
            }
        }
        int e=0;
        for (int s = 0; s < valido.Count; s++) 
        {
            if(valido[s] == true)
            {
                e++;
            }
        }
        if(e == valido.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool VerificarRecoleccion(Scr_CreadorMisiones mision)
    {
        valido.Clear();
        for (int i = 0; i < mision.ObjetosNecesarios.Length; i++)
        {
            for (int u = 0; u < inventario.Objetos.Length; u++)
            {
                if (mision.ObjetosNecesarios[i]== inventario.Objetos[u])
                {
                    if (inventario.Cantidades[u] >=mision.CantidadesQuita[i])
                    {
                        valido.Add(true);
                    }
                    else
                    {
                        valido.Add(false);
                    }

                }
            }
        }
        int e = 0;
        for (int s = 0; s < valido.Count; s++)
        {
            if (valido[s] == true)
            {
                e++;
            }
        }
        if (e == valido.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool VerificarExploracion(Scr_CreadorMisiones mision)
    {
        for (int s = 0; s < TargetExplorados.Length; s++)
        {
            if(TargetExplorados[s] == mision.TargetExplorado)
            {
                return true;
            }
        }
        return false;
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
        if (MisionPrincipal != null)
        {
            PlayerPrefs.SetString("MisionPrincipal", MisionPrincipal.name);
        }
        
        PlayerPrefs.SetInt("PaginaActual", PaginaActual);
        PlayerPrefs.SetInt("MisionesExtraCantidad", MisionesExtra.Count);
        if (MisionesExtra != null)
        {
            for (int i = 0; i < MisionesExtra.Count; i++)
            {
                PlayerPrefs.SetString("MisionExtra_" + i, MisionesExtra[i].name);
            }
        }

        PlayerPrefs.Save();
    }

    public void CargarMisiones()
    {
        string nombreMision = PlayerPrefs.GetString("MisionActual", "");
        if (!string.IsNullOrEmpty(nombreMision))
        {
            MisionActual= MisionData.GetByName(nombreMision);
            //MisionActual = System.Array.Find(TodasLasMisiones, m => m.name == nombreMision);
        }
        string nombreMisionP = PlayerPrefs.GetString("MisionPrincipal", "");
        if (!string.IsNullOrEmpty(nombreMisionP))
        {
            MisionPrincipal = MisionData.GetByName(nombreMisionP);
            //MisionActual = System.Array.Find(TodasLasMisiones, m => m.name == nombreMision);
        }
        PaginaActual = PlayerPrefs.GetInt("PaginaActual", 1);
        MisionesExtra.Clear();

        int misionesExtraCantidad = PlayerPrefs.GetInt("MisionesExtraCantidad", 0);
        for (int i = 0; i < misionesExtraCantidad; i++)
        {
            string nombreMisionExtra = PlayerPrefs.GetString("MisionExtra_" + i, "");
            if (!string.IsNullOrEmpty(nombreMisionExtra))
            {
                Scr_CreadorMisiones misionExtra = MisionData.GetByName(nombreMision);
                //Scr_CreadorMisiones misionExtra = System.Array.Find(TodasLasMisiones, m => m.name == nombreMisionExtra);
                if (misionExtra != null)
                {
                    MisionesExtra.Add(misionExtra);
                }
            }
        }
    }
}