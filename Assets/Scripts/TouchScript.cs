using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TouchScript : MonoBehaviour {
	public RawImage dipimage;
	public GameObject detailUI;
	public Text TitleText;
	public Text RiviewsText;
	public Image rating;
	public GameObject loading;
	public Text timeText;
	public Text missingText;

	public string openWebsite = "";

	// Use this for initialization
	void Start () {
		loading.SetActive (false);
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.touchCount == 1)
			{
				Touch theTouch = Input.GetTouch (0);
				if (theTouch.phase == TouchPhase.Moved) {
					transform.Rotate (theTouch.deltaPosition.y * 0.25f, -theTouch.deltaPosition.x * 0.25f, 0f, Space.World);
				}
				if (theTouch.phase == TouchPhase.Ended)
				{
					checkTouch(theTouch.position);
				}
			}

			if (Input.touchCount == 2) {
				Touch theTouch = Input.GetTouch (0);
				if (theTouch.phase == TouchPhase.Moved) {
					transform.Translate (0, 0, theTouch.deltaPosition.y * 0.25f, Space.World);
				}
			}

		}
		else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			if (Input.GetMouseButtonDown(0))
			{
				checkTouch(Input.mousePosition);
			}
		}

	}

	private void checkTouch(Vector3 pos)
	{
		Ray ray = Camera.main.ScreenPointToRay (pos);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) {
			detailUI.SetActive (true);
			loading.SetActive (true);
			Debug.Log (" You just hit " + hit.collider.gameObject.name);
			hit.collider.gameObject.GetComponentInParent<SetValues> ().Clicked (detailUI, dipimage, TitleText, rating, RiviewsText, loading, timeText, missingText);
			openWebsite = hit.collider.gameObject.GetComponentInParent<SetValues> ().website;
		}
	}

	public void openClicked(){
		Application.OpenURL (openWebsite);
	}
}
