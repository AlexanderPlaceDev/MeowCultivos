using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_GirarObjeto : MonoBehaviour
{
    [SerializeField] public float VelocidadGeneral;
    [SerializeField] float VelocidadX;
    [SerializeField] float VelocidadY;
    [SerializeField] float VelocidadZ;
    [SerializeField] bool rotarLocalmente;
    [SerializeField] bool rotarAlrededorDeOtroObjeto;
    [SerializeField] Transform objetoAlrededorDelCualRotar;

    void Start()
    {

    }

    void Update()
    {
        if (rotarAlrededorDeOtroObjeto && objetoAlrededorDelCualRotar != null)
        {
            RotateAroundObject();
        }
        else
        {
            RotateLocallyOrGlobally();
        }
    }

    void RotateLocallyOrGlobally()
    {
        if (rotarLocalmente)
        {
            transform.Rotate(new Vector3(VelocidadX, VelocidadY, VelocidadZ) * VelocidadGeneral * Time.deltaTime, Space.Self);
        }
        else
        {
            transform.Rotate(new Vector3(VelocidadX, VelocidadY, VelocidadZ) * VelocidadGeneral * Time.deltaTime, Space.World);
        }
    }

    void RotateAroundObject()
    {
        transform.RotateAround(objetoAlrededorDelCualRotar.position, Vector3.up, VelocidadGeneral * Time.deltaTime);
    }
}