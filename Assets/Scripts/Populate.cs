using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Populate : MonoBehaviour {

    public GameObject recipeObject;

	// Use this for initialization
	void Start () {
        string[][] data = DataClass.data;

        Debug.Log(data.Length);
        foreach (string[] row in data)
        {
//            Debug.Log(row[0] + " " + row[1]);
            GameObject temp = Instantiate(recipeObject, transform);
            temp.GetComponent<SetValues>().setValues(row);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
