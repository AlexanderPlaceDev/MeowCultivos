using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Estatua : MonoBehaviour
{

    public ParticleSystem ParticulasMuerte;
    public CapsuleCollider Collider;
    public MeshRenderer MeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ParticulasMuerte= gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        Collider= gameObject.GetComponent<CapsuleCollider>();
        MeshRenderer = gameObject.GetComponent<MeshRenderer>();
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

            ParticulasMuerte.Play();
            StartCoroutine(EsperarMuerte());
        }
    }

    IEnumerator EsperarMuerte()
    {
        Collider.enabled = false;
        MeshRenderer.enabled = false;
        yield return new WaitForSeconds(ParticulasMuerte.main.duration);
        Destroy(gameObject);
    }
}
