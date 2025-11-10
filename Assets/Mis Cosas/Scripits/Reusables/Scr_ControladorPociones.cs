using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ControladorPociones : MonoBehaviour
{
    Scr_ControladorBatalla ControladorBatalla;
    Scr_ControladorArmas ControladorArmas;
    Scr_Movimiento mov;
    // Start is called before the first frame update
    void Start()
    {
        ControladorBatalla = GameObject.Find("Controlador").GetComponent<Scr_ControladorBatalla>();
        mov = GameObject.Find("Personaje").GetComponent<Scr_Movimiento>();
        if (ControladorBatalla.PocionPermanente)
        {
            Pociones();
        }
    }

    public void Pociones()
    {
        //Curativo, Dano, Velocidad, Resitencia, Vida
        switch (ControladorBatalla.Pocion) 
        {
            case "Curativa":
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
                break;
            case "":
                Debug.Log("No hay Pocion");
                break;
        }
    }
    
    public void PocionCurativa()
    {
        ControladorBatalla.Curar(ControladorBatalla.PocionPuntos);
    }

    public void PocionDaño()
    {
        if (ControladorBatalla.PocionPermanente)
        {
            ControladorArmas.daño = ControladorArmas.daño + ControladorBatalla.PocionPuntos;
        }
        else
        {
            StartCoroutine(DañoTemporal(ControladorBatalla.PocionDuracion));
        }
    }
    IEnumerator DañoTemporal(float duracion)
    {
        float dañonormal = ControladorArmas.daño;
        ControladorArmas.daño = ControladorArmas.daño + ControladorBatalla.PocionPuntos;
        yield return new WaitForSeconds(duracion);
        ControladorArmas.daño = dañonormal;
    }
    public void PocionVelocidad()
    {
        if (ControladorBatalla.PocionPermanente)
        {
            mov.VelAgachado = mov.VelAgachado * ControladorBatalla.PocionPuntos;
            mov.VelCaminar = mov.VelCaminar * ControladorBatalla.PocionPuntos;
            mov.VelCorrer = mov.VelCorrer * ControladorBatalla.PocionPuntos;
        }
        else
        {
            StartCoroutine(VelocidadTemporal(ControladorBatalla.PocionDuracion));
        }
    }
    IEnumerator VelocidadTemporal(float duracion)
    {
        float VagachadoAnterior = mov.VelAgachado;
        float VcaminarAnterior = mov.VelCaminar;
        float VcorrerAnterior = mov.VelCorrer;
        mov.VelAgachado = mov.VelAgachado * ControladorBatalla.PocionPuntos;
        mov.VelCaminar = mov.VelCaminar * ControladorBatalla.PocionPuntos;
        mov.VelCorrer = mov.VelCorrer * ControladorBatalla.PocionPuntos;
        yield return new WaitForSeconds(duracion);
        mov.VelAgachado = VagachadoAnterior;
        mov.VelCaminar = VcaminarAnterior;
        mov.VelCorrer = VcorrerAnterior;
    }

    public void PocionResitencia()
    {
        if (ControladorBatalla.PocionPermanente)
        {
            checarResitencia();
        }
        else
        {
            StartCoroutine(ResitenciaTemporal(ControladorBatalla.PocionDuracion));
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
        switch (ControladorBatalla.Resistencia)
        {
            case "Stunear":
                ControladorBatalla.resistenciaStunear= ControladorBatalla.resistenciaStunear + ControladorBatalla.PocionPuntos;
                break;
            case "Quemar":
                ControladorBatalla.resistenciaQuemar = ControladorBatalla.resistenciaQuemar + ControladorBatalla.PocionPuntos;
                break;
            case "Veneno":
                ControladorBatalla.resistenciaVeneno = ControladorBatalla.resistenciaVeneno + ControladorBatalla.PocionPuntos;
                break;
            case "Congelar":
                ControladorBatalla.resistenciaCongelar = ControladorBatalla.resistenciaCongelar + ControladorBatalla.PocionPuntos;
                break;
            case "Empujar":
                ControladorBatalla.resistenciaEmpujar = ControladorBatalla.resistenciaEmpujar + ControladorBatalla.PocionPuntos;
                break;
            case "Electrificar":
                ControladorBatalla.resistenciaElectrificar = ControladorBatalla.resistenciaElectrificar + ControladorBatalla.PocionPuntos;
                break;
            case "Explotar":
                ControladorBatalla.resistenciaExplotar = ControladorBatalla.resistenciaExplotar + ControladorBatalla.PocionPuntos;
                break;
            case "Nada":
                ControladorBatalla.resistenciaStunear = ControladorBatalla.resistenciaStunear + ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaQuemar = ControladorBatalla.resistenciaQuemar + ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaVeneno = ControladorBatalla.resistenciaVeneno + ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaCongelar = ControladorBatalla.resistenciaCongelar + ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaEmpujar = ControladorBatalla.resistenciaEmpujar + ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaElectrificar = ControladorBatalla.resistenciaElectrificar + ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaExplotar = ControladorBatalla.resistenciaExplotar + ControladorBatalla.PocionPuntos;
                break;
        }
    }
    public void quitarResitencia()
    {
        // Nada,Stunear, Quemar, Veneno, Congelar, Empujar, Electrificar, Explotar 
        switch (ControladorBatalla.Resistencia)
        {
            case "Stunear":
                ControladorBatalla.resistenciaStunear = ControladorBatalla.resistenciaStunear - ControladorBatalla.PocionPuntos;
                break;
            case "Quemar":
                ControladorBatalla.resistenciaQuemar = ControladorBatalla.resistenciaQuemar - ControladorBatalla.PocionPuntos;
                break;
            case "Veneno":
                ControladorBatalla.resistenciaVeneno = ControladorBatalla.resistenciaVeneno - ControladorBatalla.PocionPuntos;
                break;
            case "Congelar":
                ControladorBatalla.resistenciaCongelar = ControladorBatalla.resistenciaCongelar - ControladorBatalla.PocionPuntos;
                break;
            case "Empujar":
                ControladorBatalla.resistenciaEmpujar = ControladorBatalla.resistenciaEmpujar - ControladorBatalla.PocionPuntos;
                break;
            case "Electrificar":
                ControladorBatalla.resistenciaElectrificar = ControladorBatalla.resistenciaElectrificar - ControladorBatalla.PocionPuntos;
                break;
            case "Explotar":
                ControladorBatalla.resistenciaExplotar = ControladorBatalla.resistenciaExplotar - ControladorBatalla.PocionPuntos;
                break;
            case "Nada":
                ControladorBatalla.resistenciaStunear = ControladorBatalla.resistenciaStunear - ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaQuemar = ControladorBatalla.resistenciaQuemar - ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaVeneno = ControladorBatalla.resistenciaVeneno - ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaCongelar = ControladorBatalla.resistenciaCongelar - ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaEmpujar = ControladorBatalla.resistenciaEmpujar - ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaElectrificar = ControladorBatalla.resistenciaElectrificar - ControladorBatalla.PocionPuntos;
                ControladorBatalla.resistenciaExplotar = ControladorBatalla.resistenciaExplotar - ControladorBatalla.PocionPuntos;
                break;
        }
    }

    public void PocionVida()
    {
        if (ControladorBatalla.PocionPermanente)
        {
            ControladorBatalla.VidaMaxima = ControladorBatalla.VidaMaxima + ControladorBatalla.PocionPuntos;
        }
        else
        {
            StartCoroutine(VidaTemporal(ControladorBatalla.PocionDuracion));
        }
    }
    IEnumerator VidaTemporal(float duracion)
    {
        float vidaNormal = ControladorBatalla.VidaMaxima;
        ControladorBatalla.VidaMaxima = ControladorBatalla.VidaMaxima + ControladorBatalla.PocionPuntos;
        yield return new WaitForSeconds(duracion);
        ControladorBatalla.VidaMaxima= vidaNormal;
    }
}
