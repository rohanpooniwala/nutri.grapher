using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAnalyze : MonoBehaviour {
	public GameObject analyze;
	public GameObject backbutton;

	public void onClick(){
		analyze.SetActive (false);
		backbutton.SetActive (false);
	}
}
