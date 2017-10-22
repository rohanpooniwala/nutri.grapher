using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetValues : MonoBehaviour {

	public SpriteRenderer background;

	static Vector3 mean_vector = Vector3.zero;
	static int count = 0;

	Vector3 localpos = Vector3.zero;

	RawImage dispimg;
	Image ratingImage;
	Text RiviewsText;
	GameObject loading;
	string recipeName = "";
	Text timeText;

    public string website = "";


	string missing = "";
	JsonData test;

    public TextMesh title;

	public void Update(){
		transform.localPosition = localpos - mean_vector;
//		transform.position = Vector3.Lerp (transform.position, localpos - mean_vector, Time.deltaTime);
	}

    public void setValues(string[] data)
    {
		foreach (Renderer _ in GetComponentsInChildren<Renderer>()) {
			_.enabled = false;
		}
        recipeName = data[0];
        website = data[1];
		missing = data [2];

        title.text = recipeName;

        StartCoroutine(fetchValues());
    }

    IEnumerator fetchValues()
    {
        WWWForm temp = new WWWForm();
        temp.AddField("text", recipeName + "~=~" + website);

        //Encode it as a PNG.

        Debug.Log("Sending to server!");
		WWW www = new WWW("http://ec2-18-216-64-10.us-east-2.compute.amazonaws.com:5000/fetchRecipeCoor", temp);
        yield return www;

        if (www.error != null)
        {
            Debug.Log(www.error);
//            Debug.Log(recipeName);
//            Debug.Log(website);
        }
        else
        {
            if (!www.text.Equals("null"))
            {
                Debug.Log(www.text);
				string server_raw_data = www.text;
				string[] data_seperator = new string[] { "~=~" };
				string[] server_data = server_raw_data.Split(data_seperator, System.StringSplitOptions.None);

				string server_color = server_data [1];
				Debug.Log("Color :"+server_color);

				if (server_color.Length != 0) {
					if (server_color.Equals("red"))
						background.color = Color.red;
					else if (server_color.Equals("yellow"))
						background.color = new Color(0.67f,0.62f,0.0f);
					else if (server_color.Equals("brown"))
						background.color = Color.cyan;
					else if (server_color.Equals("black"))
						background.color = Color.black;
					else if (server_color.Equals("grey"))
						background.color = Color.grey;
					else if (server_color.Equals("green"))
						background.color = Color.green;
					else if (server_color.Equals ("magenta"))
						background.color = new Color (.17f, .43f, .37f);
					else
						background.color = new Color(0.67f, 0.34f, 0.22f);
					
				}

				string[] cords = server_data[0].Split(',');
				Debug.Log ((2 + float.Parse(cords[0])) * 10);
				Vector3 transformposition = new Vector3(float.Parse(cords[0]), float.Parse(cords[1]) * 100, float.Parse (cords [2]) * 5);

				count++;

				localpos = transformposition;

				mean_vector = (mean_vector + localpos) / count;
				foreach (Renderer _ in GetComponentsInChildren<Renderer>()) {
					_.enabled = true;
				}
            }
            else
            {
                Debug.Log("null recieved!");
            }
        }
    }

	public void Clicked(GameObject DetailUI, RawImage dispImage, Text titleText, Image rating, Text rvtxt, GameObject loading_temp, Text timeText_temp, Text missingText_temp){
		dispimg = dispImage;
		ratingImage = rating;
		titleText.text = recipeName;
		RiviewsText = rvtxt;
		loading = loading_temp;
		timeText = timeText_temp;
		missingText_temp.text = (missing=="")?"You have every ingredient.":"You are missing: "+missing;
		DetailUI.SetActive (true);
		StartCoroutine(fetchinfo());
	}

	IEnumerator fetchinfo(){
		WWWForm temp = new WWWForm();
		temp.AddField("text", recipeName + "~=~" + website);

		//Encode it as a PNG.

		Debug.Log("Sending to server!");
		WWW www = new WWW("http://ec2-18-216-64-10.us-east-2.compute.amazonaws.com:5000/fetchRecipeInfo", temp);
		yield return www;

		if (www.error != null)
		{
			Debug.Log(www.error);
			Debug.Log(recipeName);
			Debug.Log(website);
		}
		else
		{
			if (!www.text.Equals("null"))
			{
				test = JsonUtility.FromJson<JsonData> (www.text);

				FindObjectOfType<SetImage> ().setImage (test.graph_base64);
				WWW wwwimage = new WWW(test.img);
				yield return wwwimage;

				dispimg.texture = wwwimage.texture;

				ratingImage.fillAmount = float.Parse(test.rating.Split('~')[0]) / 5;
				RiviewsText.text = test.rating.Split ('~') [1];

				timeText.text = "Time: " + test.time;
			}
			else
			{
				Debug.Log("null recieved!");
			}
		}
		loading.SetActive (false);
	}
}
