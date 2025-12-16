using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruta_drop : MonoBehaviour
{
    public float Vida = 10f;
    public string Nombre = "";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Dropear_Fruta());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Dropear_Fruta()
    {
        yield return new WaitForSeconds(Vida);
        Destroy(gameObject);
    }
}
