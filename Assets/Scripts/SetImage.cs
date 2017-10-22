using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImage : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setImage(string iconBase64String){
		Texture2D convertedBase64String = new Texture2D (1,1);

		byte[] decodedBytes = System.Convert.FromBase64String(iconBase64String);

		convertedBase64String.LoadImage(decodedBytes);

		// Instantiated object
		GetComponent<RawImage>().texture = convertedBase64String;
	}
}
