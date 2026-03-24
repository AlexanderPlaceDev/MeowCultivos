using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarVigia : MonoBehaviour
{
    [SerializeField] OjoVigilante vigia;
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
        if (other.gameObject.CompareTag("Gata"))
        {
            quitarIntento();
        }
    }

    public void quitarIntento()
    {
        Scr_Activador_boss boss = GameObject.Find("Activador_BOSS").GetComponent<Scr_Activador_boss>();
        boss.detectaron(true);
        vigia.Dormir();
    }
}
