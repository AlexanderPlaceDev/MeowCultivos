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

    public List<string> NameEnemiCazado;
    //public string[] NameEnemiCazado;
    public List<int> cazados;


    public List<bool> valido;

    public List<string> TargetExplorados;


    public GameObject[] todos;
    void Start()
    {
        UnityEngine.Debug.Log("APARECE GATA");
        Gata = GameObject.Find("Gata").transform;
        inventario = Gata.GetChild(7).GetComponent<Scr_Inventario>();
        CargarMisiones();
        todos= Buscartag.BuscarObjetosConTagInclusoInactivos("Construcciones");

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
        }
        else
        {
            TextoDescripcion.text = "Sin objetivo...";
            TextoPagina.text = "...";
            Objetos.SetActive(false);
        }
        GuardarMisiones();
    }

    public void actualizarTargetsExploratod(string target)
    {
        if (!existirTarget(target))
        {
            TargetExplorados.Add(target);
            UnityEngine.Debug.Log("Se agrego" + target);
        }

        UnityEngine.Debug.Log("Revisando");
        revisarMisionPrincipal();
        RevisarTodasLasMisionesSecundarias();
    }
    public bool existirTarget(string target)
    {
        for (int i = 0; i < TargetExplorados.Count; i++)
        {
            if (target == TargetExplorados[i])
            {
                return true;
            }
        }
        return false;
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

    public void revisar_objetivos()
    {
        if(MisionPrincipal != null)
        {
            if(MisionPrincipal.Objetivocaza.Length > 0)
            {
                for (int i = 0; i < MisionPrincipal.Objetivocaza.Length; i++) 
                {
                    checkazados(MisionPrincipal.Objetivocaza[i], MisionPrincipal.cantidad_caza[i]);
                }
            }
        }
        if (MisionesExtra.Count > 0)
        { 

            for (int u = 0; u < MisionPrincipal.Objetivocaza.Length; u++)
            {
                if (MisionesExtra[u].Objetivocaza.Length > 0)
                {
                    for (int i = 0; i < MisionesExtra[u].Objetivocaza.Length; i++)
                    {
                        checkazados(MisionesExtra[u].Objetivocaza[i], MisionesExtra[u].cantidad_caza[i]);
                    }
                }
            }
        }
    }
    public void revisarMisionPrincipal()
    {
        if (MisionPrincipal != null)
        {
            bool completada = false;
            ComprobarProgreso(MisionPrincipal, ref completada);
            if (completada)
            {
                MisionPCompleta = true;
                if (MisionPrincipal == MisionActual)
                {
                    MisionCompleta = true;
                }
            }
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
        if (MisionesExtra != null && MisionesExtra.Count >0 && MisionesScompletas!=null && MisionesScompletas.Count>0)
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

                if(MisionActual== MisionesExtra[i])
                {
                    MisionActual = null;
                    MisionCompleta = false;
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
        if (mision.ObjetosRecompensa.Length > 0)
        {
            for (int i = 0; i < mision.ObjetosRecompensa.Length; i++)
            {
                for (int u = 0; u < inventario.Objetos.Length; u++)
                {
                    if (mision.ObjetosRecompensa[i] == inventario.Objetos[u])
                    {
                        UnityEngine.Debug.LogWarning("Recompesa " + mision.ObjetosRecompensa[i]);
                        inventario.Cantidades[u] = inventario.Cantidades[u] + mision.CantidadesDa[i];
                    }
                }
            }
        }

        if (mision.xpTotal > 0)
        {
            PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual") + mision.xpTotal);
            if (PlayerPrefs.GetInt("XPActual", 0) >= PlayerPrefs.GetInt("XPSiguiente", 10))
            {
                PlayerPrefs.SetInt("XPActual", PlayerPrefs.GetInt("XPActual", 0) - PlayerPrefs.GetInt("XPSiguiente", 10));
                PlayerPrefs.SetInt("Nivel", PlayerPrefs.GetInt("Nivel", 0) + 1);
                PlayerPrefs.SetInt("XPSiguiente", PlayerPrefs.GetInt("XPSiguiente", 10) * 2);
                PlayerPrefs.SetInt("PuntosDeHabilidad", PlayerPrefs.GetInt("PuntosDeHabilidad", 0) + 3);
                GameObject.Find("Canvas XP").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "LV.+1";
            }
            else
            {
                GameObject.Find("Canvas XP").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "XP + " + mision.xpTotal;
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
            case Tipos.Construccion:
                completada = VerificarConstruccion(mision);
                break;
        }
    }
    public void checkazados(string caza, int cantidad)
    {
        bool have=false;
        for (int i = 0; i < 10; i++) 
        {
            if(NameEnemiCazado[i].ToString() == caza.ToString())
            {
                cazados[i]=cazados[i]+cantidad;
                have = true; 
                break;
            }
        }
        if (!have)
        {
            NameEnemiCazado.Add(caza);
            cazados.Add(cantidad);
        }
    }
    public bool VerificarCaza(Scr_CreadorMisiones mision)
    {
        valido.Clear();
        for (int i = 0; i < mision.Objetivocaza.Length; i++)
        {
            for (int u = 0; u < NameEnemiCazado.Count; u++)
            {
                if (mision.Objetivocaza[i].ToString() == NameEnemiCazado[u].ToString())
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
        for (int s = 0; s < TargetExplorados.Count; s++)
        {
            UnityEngine.Debug.LogWarning((TargetExplorados[s] == mision.TargetExplorado)+"ade");
            if (TargetExplorados[s] == mision.TargetExplorado)
            {
                return true;
            }
        }
        return false;
    }

    public bool VerificarConstruccion(Scr_CreadorMisiones mision)
    {
        valido.Clear();
        for (int s = 0; s < todos.Length; s++)
        {
            for (int i = 0; i < mision.objetoaCostruir.Length; i++)
            {
                if (todos[s].name == mision.objetoaCostruir[i].name && todos[s].activeSelf)
                {
                    valido.Add(true);
                }
                else
                {
                    
                    valido.Add(false);
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
        
        if (MisionesExtra.Count>0)
        {
            PlayerPrefs.SetInt("MisionesExtraCantidad", MisionesExtra.Count); 
            UnityEngine.Debug.Log("33333333333");
            for (int i = 0; i < MisionesExtra.Count; i++)
            {
                PlayerPrefs.SetString("MisionExtra_" + i, MisionesExtra[i].name);
                UnityEngine.Debug.Log("00000000000000");
            }
        }
        else
        {
            int misionesExtraCantidad = PlayerPrefs.GetInt("MisionesExtraCantidad", 0);
            //UnityEngine.Debug.Log("se borran" + MisionesExtra.Count);
            for (int i = 0; i < misionesExtraCantidad; i++)
            {
                PlayerPrefs.DeleteKey("MisionExtra_" + i);
            }
            PlayerPrefs.SetInt("MisionesExtraCantidad", 0);
        }

        PlayerPrefs.SetInt("Target_Cantidad", TargetExplorados.Count);

        PlayerPrefs.SetInt("Cazado_Cantidad", NameEnemiCazado.Count);
        if (TargetExplorados.Count > 0)
        {
            for (int i = 0; i < TargetExplorados.Count; i++)
            {
                PlayerPrefs.SetString("Target_" + i, TargetExplorados[i]);
            }
        }
        if(NameEnemiCazado.Count > 0)
        {
            for (int i = 0; i < NameEnemiCazado.Count; i++)
            {
                PlayerPrefs.SetString("Cazado_" + i, NameEnemiCazado[i]);
                PlayerPrefs.SetInt("cazado_cant" + i, cazados[i]);
            }
        }

        
        
        //UnityEngine.Debug.Log("se gaurd" + MisionesExtra.Count);
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

        int misionesExtraCantidad = PlayerPrefs.GetInt("MisionesExtraCantidad",0);
        //UnityEngine.Debug.Log("checa------"+misionesExtraCantidad);
        for (int i = 0; i < misionesExtraCantidad; i++)
        {
            string nombreMisionExtra = PlayerPrefs.GetString("MisionExtra_" + i, "");
            //UnityEngine.Debug.Log(nombreMisionExtra);
            if (!string.IsNullOrEmpty(nombreMisionExtra))
            {
                Scr_CreadorMisiones misionExtra = MisionData.GetByName(nombreMision);
                //Scr_CreadorMisiones misionExtra = System.Array.Find(TodasLasMisiones, m => m.name == nombreMisionExtra);
                if (misionExtra != null)
                {
                    MisionesExtra.Add(misionExtra);
                }
                else
                {
                    UnityEngine.Debug.Log("no exisre misione");
                }
            }
        }
        TargetExplorados.Clear();
        int targets = PlayerPrefs.GetInt("Target_Cantidad", 0);
        //UnityEngine.Debug.Log(targets);
        for (int i = 0; i < targets; i++)
        {
            string nombretarget = PlayerPrefs.GetString("Target_" + i, "");
            if (!string.IsNullOrEmpty(nombretarget))
            {
                //Scr_CreadorMisiones misionExtra = MisionData.GetByName(nombreMision);
                //Scr_CreadorMisiones misionExtra = System.Array.Find(TodasLasMisiones, m => m.name == nombreMisionExtra);
                TargetExplorados.Add(nombretarget);
            }
        }
        NameEnemiCazado.Clear();
        cazados.Clear();
        int cazado = PlayerPrefs.GetInt("Cazado_Cantidad", 0);
        //UnityEngine.Debug.Log("ae"+cazado);
        for (int i = 0; i < cazado; i++)
        {
            string nombrecazado = PlayerPrefs.GetString("Cazado_" + i, "");
            int cantCazados = PlayerPrefs.GetInt("cazado_cant" + i, 0);
            if (!string.IsNullOrEmpty(nombrecazado) && cantCazados>0)
            {
                //Scr_CreadorMisiones misionExtra = MisionData.GetByName(nombreMision);
                //Scr_CreadorMisiones misionExtra = System.Array.Find(TodasLasMisiones, m => m.name == nombreMisionExtra);
                NameEnemiCazado.Add(nombrecazado);
                cazados.Add(cantCazados);
            }
        }
    }
}