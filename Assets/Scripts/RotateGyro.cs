using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGyro : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Input.gyro.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(-Input.gyro.rotationRate.x, -Input.gyro.rotationRate.y, Input.gyro.rotationRate.z);
    }
}
