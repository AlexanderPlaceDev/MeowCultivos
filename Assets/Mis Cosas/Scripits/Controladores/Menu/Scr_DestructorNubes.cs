using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_DestructorNubes : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Destructor")
        {
            Destroy(gameObject);
        }
    }
}
