using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorPortal : MonoBehaviour
{
    [SerializeField] Collider coli;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        StartCoroutine(ActivarCollider());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator ActivarCollider()
    {
        coli.enabled = false;
        yield return new WaitForSeconds(5f);
        coli.enabled = true;
    }
}
