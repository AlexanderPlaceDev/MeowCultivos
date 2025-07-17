using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balas : MonoBehaviour
{
    public float da�o = 0;
    private float delete = 2f;
    private float contar=0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        contar += Time.deltaTime;
        if(contar > delete)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Da�o");
        //if (!other.CompareTag("Gata")) return;

        Debug.Log("Da�o");
        if (other.CompareTag("Enemigo"))
        {
            Debug.Log("Da�o");
            var ene = other.GetComponent<Scr_Enemigo>();
            ene.RecibirDa�o(da�o);
            Destroy(gameObject);
        }
    }
}
