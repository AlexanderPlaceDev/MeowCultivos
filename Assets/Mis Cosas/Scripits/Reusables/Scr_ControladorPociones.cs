using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorPociones : MonoBehaviour
{
    public Scr_ControladorBatalla ControladorBatalla;
    public Scr_ControladorArmas ControladorArmas;
    public Scr_Movimiento mov;

    public float PocionPuntos;
    public float PocionDuracion;
    public float PocionUsos;
    public string Resistencia;
    public bool PocionPermanente;
    public Color ColorPocion;
    // Start is called before the first frame update
    void Start()
    {
        ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        mov = GameObject.Find("Personaje").GetComponent<Scr_Movimiento>();
    }

    public void Pociones(string Pocion)
    {
        //Debug.LogError("Hola"+ Pocion);
        //Curativo, Dano, Velocidad, Resitencia, Vida
        switch (Pocion) 
        {
            case "Curativo":
                PocionCurativa();
                break;
            case "Dano":
                PocionDaño();
                break;
            case "Velocidad":
                PocionVelocidad();
                break;
            case "Resitencia":
                PocionResitencia();
                break;
            case "Vida":
                PocionVida();
                break;
            case "":
                Debug.Log("No hay Pocion");
                break;
        }
    }
    
    public void PocionCurativa()
    {
        ControladorBatalla.Curar(PocionPuntos);
    }

    public void PocionDaño()
    {
        if (PocionPermanente)
        {
            ControladorArmas.daño = ControladorArmas.daño + PocionPuntos;
        }
        else
        {
            StartCoroutine(DañoTemporal(PocionDuracion));
        }
    }
    IEnumerator DañoTemporal(float duracion)
    {
        float dañonormal = ControladorArmas.daño;
        ControladorArmas.daño = ControladorArmas.daño + PocionPuntos;
        yield return new WaitForSeconds(duracion);
        ControladorArmas.daño = dañonormal;
    }
    public void PocionVelocidad()
    {
        if (PocionPermanente)
        {
            mov.VelAgachado = mov.VelAgachado * PocionPuntos;
            mov.VelCaminar = mov.VelCaminar * PocionPuntos;
            mov.VelCorrer = mov.VelCorrer * PocionPuntos;
        }
        else
        {
            StartCoroutine(VelocidadTemporal(PocionDuracion));
        }
    }
    IEnumerator VelocidadTemporal(float duracion)
    {
        float VagachadoAnterior = mov.VelAgachado;
        float VcaminarAnterior = mov.VelCaminar;
        float VcorrerAnterior = mov.VelCorrer;
        mov.VelAgachado = mov.VelAgachado * PocionPuntos;
        mov.VelCaminar = mov.VelCaminar * PocionPuntos;
        mov.VelCorrer = mov.VelCorrer * PocionPuntos;
        yield return new WaitForSeconds(duracion);
        mov.VelAgachado = VagachadoAnterior;
        mov.VelCaminar = VcaminarAnterior;
        mov.VelCorrer = VcorrerAnterior;
    }

    public void PocionResitencia()
    {
        if (PocionPermanente)
        {
            checarResitencia();
        }
        else
        {
            StartCoroutine(ResitenciaTemporal(PocionDuracion));
        }
    }
    IEnumerator ResitenciaTemporal(float duracion)
    {
        checarResitencia();
        yield return new WaitForSeconds(duracion);
        quitarResitencia();
    }

    public void checarResitencia()
    {
        // Nada,Stunear, Quemar, Veneno, Congelar, Empujar, Electrificar, Explotar 
        switch (Resistencia)
        {
            case "Stunear":
                ControladorBatalla.resistenciaStunear= ControladorBatalla.resistenciaStunear + PocionPuntos;
                break;
            case "Quemar":
                ControladorBatalla.resistenciaQuemar = ControladorBatalla.resistenciaQuemar + PocionPuntos;
                break;
            case "Veneno":
                ControladorBatalla.resistenciaVeneno = ControladorBatalla.resistenciaVeneno + PocionPuntos;
                break;
            case "Congelar":
                ControladorBatalla.resistenciaCongelar = ControladorBatalla.resistenciaCongelar + PocionPuntos;
                break;
            case "Empujar":
                ControladorBatalla.resistenciaEmpujar = ControladorBatalla.resistenciaEmpujar + PocionPuntos;
                break;
            case "Electrificar":
                ControladorBatalla.resistenciaElectrificar = ControladorBatalla.resistenciaElectrificar + PocionPuntos;
                break;
            case "Explotar":
                ControladorBatalla.resistenciaExplotar = ControladorBatalla.resistenciaExplotar + PocionPuntos;
                break;
            case "Nada":
                ControladorBatalla.resistenciaStunear = ControladorBatalla.resistenciaStunear + PocionPuntos;
                ControladorBatalla.resistenciaQuemar = ControladorBatalla.resistenciaQuemar + PocionPuntos;
                ControladorBatalla.resistenciaVeneno = ControladorBatalla.resistenciaVeneno + PocionPuntos;
                ControladorBatalla.resistenciaCongelar = ControladorBatalla.resistenciaCongelar + PocionPuntos;
                ControladorBatalla.resistenciaEmpujar = ControladorBatalla.resistenciaEmpujar + PocionPuntos;
                ControladorBatalla.resistenciaElectrificar = ControladorBatalla.resistenciaElectrificar + PocionPuntos;
                ControladorBatalla.resistenciaExplotar = ControladorBatalla.resistenciaExplotar + PocionPuntos;
                break;
        }
    }
    public void quitarResitencia()
    {
        // Nada,Stunear, Quemar, Veneno, Congelar, Empujar, Electrificar, Explotar 
        switch (Resistencia)
        {
            case "Stunear":
                ControladorBatalla.resistenciaStunear = ControladorBatalla.resistenciaStunear - PocionPuntos;
                break;
            case "Quemar":
                ControladorBatalla.resistenciaQuemar = ControladorBatalla.resistenciaQuemar - PocionPuntos;
                break;
            case "Veneno":
                ControladorBatalla.resistenciaVeneno = ControladorBatalla.resistenciaVeneno - PocionPuntos;
                break;
            case "Congelar":
                ControladorBatalla.resistenciaCongelar = ControladorBatalla.resistenciaCongelar - PocionPuntos;
                break;
            case "Empujar":
                ControladorBatalla.resistenciaEmpujar = ControladorBatalla.resistenciaEmpujar - PocionPuntos;
                break;
            case "Electrificar":
                ControladorBatalla.resistenciaElectrificar = ControladorBatalla.resistenciaElectrificar - PocionPuntos;
                break;
            case "Explotar":
                ControladorBatalla.resistenciaExplotar = ControladorBatalla.resistenciaExplotar - PocionPuntos;
                break;
            case "Nada":
                ControladorBatalla.resistenciaStunear = ControladorBatalla.resistenciaStunear - PocionPuntos;
                ControladorBatalla.resistenciaQuemar = ControladorBatalla.resistenciaQuemar - PocionPuntos;
                ControladorBatalla.resistenciaVeneno = ControladorBatalla.resistenciaVeneno - PocionPuntos;
                ControladorBatalla.resistenciaCongelar = ControladorBatalla.resistenciaCongelar - PocionPuntos;
                ControladorBatalla.resistenciaEmpujar = ControladorBatalla.resistenciaEmpujar - PocionPuntos;
                ControladorBatalla.resistenciaElectrificar = ControladorBatalla.resistenciaElectrificar - PocionPuntos;
                ControladorBatalla.resistenciaExplotar = ControladorBatalla.resistenciaExplotar - PocionPuntos;
                break;
        }
    }

    public void PocionVida()
    {
        Debug.Log("vida");
        if (PocionPermanente)
        {
            ControladorBatalla.VidaMaxima = ControladorBatalla.VidaMaxima + PocionPuntos;
            ControladorBatalla.VidaActual = ControladorBatalla.VidaMaxima;
        }
        else
        {
            StartCoroutine(VidaTemporal(PocionDuracion));
        }
    }
    IEnumerator VidaTemporal(float duracion)
    {
        float vidaNormal = ControladorBatalla.VidaMaxima;
        ControladorBatalla.VidaMaxima = ControladorBatalla.VidaMaxima + PocionPuntos;
        ControladorBatalla.VidaActual = ControladorBatalla.VidaMaxima;
        yield return new WaitForSeconds(duracion);
        ControladorBatalla.VidaMaxima= vidaNormal;
        if (ControladorBatalla.VidaActual > vidaNormal)
        {
            ControladorBatalla.VidaActual = ControladorBatalla.VidaMaxima;
        }
    }
}
