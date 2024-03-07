using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Scr_MovimientoPorPuntos : MonoBehaviour
{
    public int ValorPropio;
    public bool Activo=false;
    [SerializeField] GameObject Anterior;
    [SerializeField] GameObject Siguiente;
    [SerializeField] GameObject Gata;
    float DistanciaAnteriorX;
    float DistanciaAnteriorZ;
    float DistanciaSiguienteX;
    float DistanciaSiguienteZ;
    float ValorSiguiente;
    float ValorAnterior;
    [SerializeField] CinemachineVirtualCamera Camara; 

    void Update()
    {
        if (Activo)
        {
            if (Anterior != null)
            {
                DistanciaAnteriorX =Anterior.transform.position.x;
                DistanciaAnteriorZ =Anterior.transform.position.z;

                float DistanciaTotalX = transform.position.x - DistanciaAnteriorX;
                float DistanciaTotalZ = transform.position.z - DistanciaAnteriorZ;
                float DistanciaGatoX = transform.position.x - Gata.transform.position.x;
                float DistanciaGatoZ=transform.position.z-Gata.transform.position.z;
                float a = 1 / DistanciaTotalX * DistanciaGatoX;
                float b=1/DistanciaTotalZ*DistanciaGatoZ;

                ValorAnterior=(a+b)/2;
                //Debug.Log("A:"+a +" B:"+b+ " AB: "+(a+b)+" AB/2: "+ValorAnterior);
            }
            if (Siguiente != null)
            {
               DistanciaSiguienteX=Siguiente.transform.position.x;
               DistanciaSiguienteZ=Siguiente.transform.position.z;

                float DistanciaTotalX = transform.position.x - DistanciaSiguienteX;
                float DistanciaTotalZ = transform.position.z - DistanciaSiguienteZ;
                float DistanciaGatoX = transform.position.x - Gata.transform.position.x;
                float DistanciaGatoZ=transform.position.z-Gata.transform.position.z;
                float a = 1 / DistanciaTotalX * DistanciaGatoX;
                float b=1/DistanciaTotalZ*DistanciaGatoZ;

                ValorSiguiente=(a+b)/2;
                //Debug.Log("A:"+a +" B:"+b+ " AB: "+(a+b)+" AB/2: "+ValorSiguiente);
            }


            if(ValorSiguiente>ValorAnterior)
            {
                Camara.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition=ValorPropio+ValorSiguiente;
            }else
            {
                Camara.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition =ValorPropio-ValorAnterior;
            }


            if(ValorSiguiente>0.5f)
            {
                Siguiente.GetComponent<Scr_MovimientoPorPuntos>().Activo=true;
                Activo=false;
            }
            if(ValorAnterior>0.5f)
            {
                Anterior.GetComponent<Scr_MovimientoPorPuntos>().Activo=true;
                Activo=false;
            }
        }
    }
}
