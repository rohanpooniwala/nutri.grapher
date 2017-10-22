using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;

using maxstAR;

public class InstantTrackerSample : MonoBehaviour
{
	private InstantTrackableBehaviour instantTrackable = null;

	public GameObject camera;

	private const float TOUCH_TOLERANCE = 5;

	private float touchStartX = 0.0f;
	private float touchStartY = 0.0f;

	private float translationX = 0.0f;
	private float translationY = 0.0f;

	private float positionX = 0;
	private float positionY = 0;

	private int rotationDegree = 0;
	private bool startTrackerDone = false;
	private bool cameraStartDone = false;
	private bool findSurfaceDone = false;

	[SerializeField]
	private Text startBtnText = null;

	void Start()
	{
		instantTrackable = FindObjectOfType<InstantTrackableBehaviour>();
		if (instantTrackable == null)
		{
			return;
		}

		instantTrackable.OnTrackFail();
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			SceneStackManager.Instance.LoadPrevious();
		}

		if (instantTrackable == null)
		{
			return;
		}

		StartCamera();

		if (!startTrackerDone)
		{
			TrackerManager.GetInstance().StartTracker(MaxstARUtils.TrackerMask.INSTANT_TRACKER);
			SensorDevice.GetInstance().Start();
			startTrackerDone = true;
		}

		TrackingResult trackingResult = TrackerManager.GetInstance().GetTrackingResult();

		if (Input.touchCount > 0)
		{
			switch (Input.GetTouch(0).phase)
			{
				case TouchPhase.Began:
					if (trackingResult.GetCount() > 0)
					{
						TouchStart();
					}
					break;

				case TouchPhase.Moved:
					if (trackingResult.GetCount() > 0)
					{
						TouchMove();
					}
					break;

				case TouchPhase.Ended:
					break;
			}
		}

		instantTrackable.OnTrackFail();

		if (trackingResult.GetCount() == 0)
		{
			return;
		}

		Trackable trackable = trackingResult.GetTrackable(0);
		Matrix4x4 matrix = trackable.GetPose();

		Matrix4x4 translation = Matrix4x4.identity;
		Matrix4x4 orientationMatrix = Matrix4x4.identity;
		Quaternion orientationQuaternion = Quaternion.identity;

		orientationQuaternion.eulerAngles = new Vector3(0, 0, rotationDegree);
		orientationMatrix = MatrixUtils.MatrixFromQuaternion(orientationQuaternion);
		translation.m03 = positionX;
		translation.m13 = positionY;
			
		matrix *= translation;
		matrix *= orientationMatrix;

		instantTrackable.OnTrackSuccess(trackable.GetId(), trackable.GetName(), matrix);

		matrix = Matrix4x4.Inverse (matrix);

		camera.transform.position = MatrixUtils.PositionFromMatrix(matrix);
		camera.transform.rotation = MatrixUtils.QuaternionFromMatrix(matrix);
		camera.transform.localScale = MatrixUtils.ScaleFromMatrix(matrix);
	}

	private void TouchStart()
	{
		float touchX = Input.GetTouch(0).position.x;
		float touchY = Screen.height - Input.GetTouch(0).position.y;

		touchStartX = touchX;
		touchStartY = touchY;

		float[] screenCoordinate = new float[2];
		float[] worldCoordinate = new float[3];

		screenCoordinate[0] = touchX;
		screenCoordinate[1] = touchY;

		TrackerManager.GetInstance().GetWorldPositionFromScreenCoordinate(screenCoordinate, worldCoordinate);

		switch (Screen.orientation)
		{
			case ScreenOrientation.Portrait:
				worldCoordinate[0] = worldCoordinate[0];
				worldCoordinate[1] = -worldCoordinate[1];
				break;

			case ScreenOrientation.PortraitUpsideDown:
				worldCoordinate[0] = -worldCoordinate[0];
				worldCoordinate[1] = worldCoordinate[1];
				break;

			case ScreenOrientation.Landscape:
				worldCoordinate[0] = worldCoordinate[0];
				worldCoordinate[1] = -worldCoordinate[1];
				break;

			case ScreenOrientation.LandscapeRight:
				worldCoordinate[0] = -worldCoordinate[0];
				worldCoordinate[1] = worldCoordinate[1];
				break;
		}

		translationX = worldCoordinate[0];
		translationY = worldCoordinate[1];
	}

	private void TouchMove()
	{
		float touchX = Input.GetTouch(0).position.x;
		float touchY = Screen.height - Input.GetTouch(0).position.y;

		float dx = Math.Abs(touchX - touchStartX);
		float dy = Math.Abs(touchY - touchStartY);
		if (dx >= TOUCH_TOLERANCE || dy >= TOUCH_TOLERANCE)
		{
			touchStartX = touchX;
			touchStartY = touchY;
		}
		else
		{
			return;
		}

		float[] screenCoordinate = new float[2];
		float[] worldCoordinate = new float[3];

		screenCoordinate[0] = touchX;
		screenCoordinate[1] = touchY;
		TrackerManager.GetInstance().GetWorldPositionFromScreenCoordinate(screenCoordinate, worldCoordinate);

		switch (Screen.orientation)
		{
			case ScreenOrientation.Portrait:
				rotationDegree = 0;
				worldCoordinate[0] = worldCoordinate[0];
				worldCoordinate[1] = -worldCoordinate[1];
				break;
			case ScreenOrientation.PortraitUpsideDown:
				rotationDegree = 180;
				worldCoordinate[0] = -worldCoordinate[0];
				worldCoordinate[1] = worldCoordinate[1];
				break;
			case ScreenOrientation.Landscape:
				rotationDegree = -90;
				worldCoordinate[0] = worldCoordinate[0];
				worldCoordinate[1] = -worldCoordinate[1];
				break;
			case ScreenOrientation.LandscapeRight:
				rotationDegree = 90;
				worldCoordinate[0] = -worldCoordinate[0];
				worldCoordinate[1] = worldCoordinate[1];
				break;
		}

		positionX += (worldCoordinate[0] - translationX);
		positionY += (worldCoordinate[1] - translationY);

		translationX = worldCoordinate[0];
		translationY = worldCoordinate[1];
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			SensorDevice.GetInstance().Stop();
			TrackerManager.GetInstance().StopTracker();
			startTrackerDone = false;
			StopCamera();
		}
	}

	void OnDestroy()
	{
		SensorDevice.GetInstance().Stop();
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();
		StopCamera();
	}

	void StartCamera()
	{
		if (!cameraStartDone)
		{
			Debug.Log("Unity StartCamera");
			ResultCode result = CameraDevice.GetInstance().Start();
			if (result == ResultCode.Success)
			{
				cameraStartDone = true;
			}
		}
	}

	void StopCamera()
	{
		if (cameraStartDone)
		{
			Debug.Log("Unity StopCamera");
			CameraDevice.GetInstance().Stop();
			cameraStartDone = false;
		}
	}

	public void OnClickStart()
	{
		if (!findSurfaceDone)
		{
			TrackerManager.GetInstance().FindSurface();

			if (startBtnText != null)
			{
				startBtnText.text = "Stop Tracking";
			}
			findSurfaceDone = true;
			positionX = 0;
			positionY = 0;
		}
		else
		{
			TrackerManager.GetInstance().QuitFindingSurface();

			if (startBtnText != null)
			{
				startBtnText.text = "Start Tracking";
			}
			findSurfaceDone = false;

		}
	}
}
