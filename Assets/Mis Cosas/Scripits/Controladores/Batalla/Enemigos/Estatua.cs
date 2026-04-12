using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Estatua : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Boss"))
        {
            Scr_Sonidos ojo= other.transform.GetChild(0).GetComponent<Scr_Sonidos>();
            if (ojo != null) 
            {
                ojo.Play_picar();
            }
            Destroy(gameObject);
        }
    }
}
