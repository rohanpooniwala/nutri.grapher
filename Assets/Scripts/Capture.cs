using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Capture : MonoBehaviour {

    public GameObject image;
    public RawImage im;
    public RectTransform rawImageRT;
    public AspectRatioFitter rawImageARF;
	public GameObject AnalyzeImage;
	public GameObject backbutton;

	public Text messageText;

    public GameObject loader;
    WebCamTexture _webcamtex;
    

    // Use this for initialization
    void Start () {
        _webcamtex = new WebCamTexture();
        im.texture = _webcamtex;
        _webcamtex.Play();

        
    }

    // Update is called once per frame
    void Update(){
        if (_webcamtex.width < 500)
        {
            Debug.Log("Still waiting another frame for correct info...");
            return;
        }

        int cwNeeded = _webcamtex.videoRotationAngle;
        int ccwNeeded = -cwNeeded;

        if (_webcamtex.videoVerticallyMirrored) ccwNeeded += 180;

        rawImageRT.localEulerAngles = new Vector3(0f, 0f, ccwNeeded);

        float videoRatio = (float)_webcamtex.width / (float)_webcamtex.height;

        rawImageARF.aspectRatio = videoRatio;

        image.transform.localScale = new Vector3(videoRatio, videoRatio, 1);

        if (_webcamtex.videoVerticallyMirrored)
            im.uvRect = new Rect(1, 0, -1, 1);  // means flip on vertical axis
        else
            im.uvRect = new Rect(0, 0, 1, 1);  // means no flip
    }

    public void buttonClick(){
        StartCoroutine(CaptureTextureAsPNG());
    }

    IEnumerator CaptureTextureAsPNG()
    {
        loader.active = true;

        Debug.Log("clicking!");

        yield return new WaitForEndOfFrame();
        Texture2D _TextureFromCamera = new Texture2D(im.texture.width, im.texture.height);
        _TextureFromCamera.SetPixels((im.texture as WebCamTexture).GetPixels());
        _TextureFromCamera.Apply();
        byte[] bytes = _TextureFromCamera.EncodeToPNG();
        WWWForm temp = new WWWForm();
        temp.AddBinaryData("file", bytes, "test.png", "image/png");

        //Encode it as a PNG.

        Debug.Log("Sending to server!");
		WWW www = new WWW("http://ec2-18-216-64-10.us-east-2.compute.amazonaws.com:5000/fetchRecipeNames", temp);
        yield return www;

        if (www.error != null)
        {
            Debug.Log(www.error);
        }
        else
        {
            string results = www.text;

//            Debug.Log(results);

            string[] data_seperator = new string[] { "~<>~" };
            string[] link_seperator = new string[] { "~=~" };
            string[] links = results.Split(data_seperator, System.StringSplitOptions.RemoveEmptyEntries);

            string[][] data = new string[links.Length][];

            for (int i = 0; i < links.Length; i++)
            {
                data[i] = links[i].Split(link_seperator, System.StringSplitOptions.None);
            }

 //           Debug.Log(data.Length);
//            foreach(string[] row in data)
//            {
//				Debug.Log(row[0] + " " + row[1] + " " + row[2]);
//            }
            DataClass.data = data;
            SceneManager.LoadScene(1);
            _webcamtex.Stop();
        }
    }

	public void analyzeClick(){
		StartCoroutine(AnalyzePNG());
	}

	IEnumerator AnalyzePNG()
	{
		loader.active = true;

		Debug.Log("clicking!");

		yield return new WaitForEndOfFrame();
		Texture2D _TextureFromCamera = new Texture2D(im.texture.width, im.texture.height);
		_TextureFromCamera.SetPixels((im.texture as WebCamTexture).GetPixels());
		_TextureFromCamera.Apply();


		byte[] bytes = _TextureFromCamera.EncodeToPNG();
		WWWForm temp = new WWWForm();
		temp.AddBinaryData("file", bytes, "test.png", "image/png");

		//Encode it as a PNG.

		Debug.Log("Sending to server!");
		WWW www = new WWW("http://ec2-18-216-64-10.us-east-2.compute.amazonaws.com:5000/analyzeList", temp);
		yield return www;

		if (www.error != null)
		{
			Debug.Log(www.error);
		}
		else
		{
			string raw_results = www.text;
			string[] data_seperator = new string[] { ";~**~**~&&~;" };
			string[] results = raw_results.Split(data_seperator, System.StringSplitOptions.None);

			Texture2D convertedBase64String = new Texture2D (1,1);

			byte[] decodedBytes = System.Convert.FromBase64String(results[0]);
			messageText.text = results[1];

			convertedBase64String.LoadImage(decodedBytes);

			// Instantiated object
			AnalyzeImage.GetComponentInChildren<RawImage>().texture = convertedBase64String;

			//			Debug.Log(results);
			loader.active = false;
			AnalyzeImage.SetActive (true);
			backbutton.SetActive (true);
		}
	}
}
