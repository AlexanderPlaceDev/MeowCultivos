using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_MirarCamara : MonoBehaviour
{
  [SerializeField] Transform target;


	// Use this for initialization
	void Start () 
	{
		if(target==null)
		{
			target=Camera.main.transform;
		}
	}

	// Update is called once per frame
	void Update()
	{
		// Rotate the camera every frame so it keeps looking at the target
		transform.LookAt(target);
	}
}
