using PrimeTween;
using System.Collections;
using UnityEngine;

public class Scr_ShakerCamara : MonoBehaviour
{
    [SerializeField] float TiempoShakeo;
    [SerializeField] float FuerzaShakeo;
    bool Shakeando = false;
    void Update()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift) && !Shakeando)
        {
            StartCoroutine(Shakeo());
        }
    }

    IEnumerator Shakeo()
    {
        Shakeando = true;
        Tween.ShakeCamera(Camera.main, FuerzaShakeo);
        yield return new WaitForSeconds(TiempoShakeo);
        Shakeando = false;
    }
}
