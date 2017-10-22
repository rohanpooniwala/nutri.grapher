using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resize : MonoBehaviour {
    public TextMesh tm; 
    public float xScaleFactor = 1.1f;
    public float yScaleFactor = 1.03f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        float x = tm.GetComponent<Renderer>().bounds.size.x * xScaleFactor;
        float y = tm.GetComponent<Renderer>().bounds.size.y * yScaleFactor;

        transform.localScale = new Vector3(x, y, 1);
    }
}
