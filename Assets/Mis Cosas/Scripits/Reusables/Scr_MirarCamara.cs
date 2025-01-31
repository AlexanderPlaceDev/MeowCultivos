using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_MirarCamara : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] bool EsCanvas;
    [SerializeField] bool IgnoraX;
    [SerializeField] bool IgnoraY;
    [SerializeField] bool IgnoraZ;
    [SerializeField] float OffsetX;
    [SerializeField] float OffsetY;
    [SerializeField] float OffsetZ;

    Vector3 NTarget;

    void Start()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }

    }

    void Update()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }

        NTarget = target.position;
        if (IgnoraX)
        {
            NTarget = new Vector3(0, target.position.y, target.position.z);
        }
        if (IgnoraY)
        {
            NTarget = new Vector3(target.position.x, 0, target.position.z);
        }
        if (IgnoraZ)
        {
            NTarget = new Vector3(target.position.z, target.position.y, 0);
        }
        NTarget = new Vector3(NTarget.x + OffsetX, NTarget.y + OffsetY, NTarget.z + OffsetZ);

        // Rotate the camera every frame so it keeps looking at the target
        transform.LookAt(NTarget);
        if (EsCanvas)
        {
            transform.Rotate(0, 180, 0);
        }
    }
}
