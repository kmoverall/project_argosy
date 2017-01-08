using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour {

    public Transform target;
    public Vector3 offset;

	void Start () {
	
	}
	
	void Update () {
        transform.position = target.position + offset;
	}
}
