using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_GirarObjeto : MonoBehaviour
{
    [SerializeField] public float VelocidadGeneral;
    [SerializeField] float VelocidadX;
    [SerializeField] float VelocidadY;
    [SerializeField] float VelocidadZ;
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(new Vector3(VelocidadX, VelocidadY, VelocidadZ)* VelocidadGeneral * Time.deltaTime);
    }
}
