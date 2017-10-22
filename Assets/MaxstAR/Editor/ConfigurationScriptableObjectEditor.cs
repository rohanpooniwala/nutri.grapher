using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
	[CustomEditor(typeof(ConfigurationScriptableObject))]
	public class ConfigurationScriptableObjectEditor : Editor
	{
		private ConfigurationScriptableObject configuration = null;
		private bool isDirty = false;

		private bool licenseFold = true;
		private bool cameraFold = true;

		private string[] LoadWebcamDeviceList()
		{
			WebCamDevice[] devices = WebCamTexture.devices;
			string[] deviceList = new string[devices.Length];
			for (int i = 0; i < devices.Length; i++)
			{
				deviceList[i] = devices[i].name;
				if (devices[i].name == "")
				{
					deviceList[i] = "Unknown Device " + i;
				}
			}

			return deviceList;
		}

		public void OnEnable()
		{
			if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
			{
				return;
			}
		}

		public override void OnInspectorGUI()
		{
			if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
			{
				return;
			}

			configuration = (ConfigurationScriptableObject)target;

			isDirty = false;

			licenseFold = EditorGUILayout.Foldout(licenseFold, "License Manager");
			if (licenseFold)
			{
				EditorGUILayout.LabelField("License key");
				string licenseKey = configuration.LicenseKey;
				configuration.LicenseKey = EditorGUILayout.TextArea(licenseKey, GUILayout.MaxHeight(40));
				EditorGUILayout.HelpBox("Please register your app at www.maxstar.com.", MessageType.Info);
				EditorGUILayout.Space();
				if (string.Equals(licenseKey, configuration.LicenseKey) == false)
				{
					isDirty = true;
				}
			}

			cameraFold = EditorGUILayout.Foldout(cameraFold, "Camera Manager");
			if (cameraFold)
			{
				MaxstARUtils.CameraType mobileType = configuration.MobileType;
				configuration.MobileType = (MaxstARUtils.CameraType)EditorGUILayout.EnumPopup("Mobile Camera Type", mobileType);
				EditorGUILayout.HelpBox("Camera settings in mobile app", MessageType.Info);
				EditorGUILayout.Space();
				if (string.Equals(mobileType, configuration.MobileType) == false)
				{
					isDirty = true;
				}

				int webcamType = configuration.WebcamType;
				configuration.WebcamType = EditorGUILayout.Popup("Webcam Type", webcamType, LoadWebcamDeviceList());
				EditorGUILayout.HelpBox("Webcam settings in Editor mode.", MessageType.Info);
				EditorGUILayout.Space();
				if (string.Equals(webcamType, configuration.WebcamType) == false)
				{
					isDirty = true;
				}

				MaxstARUtils.CameraResolution resolution = configuration.Resolution;
				configuration.Resolution = (MaxstARUtils.CameraResolution)EditorGUILayout.EnumPopup("Camera Resolution", resolution);
				EditorGUILayout.HelpBox("Please select a supported resolution.", MessageType.Info);
				EditorGUILayout.Space();
				if (string.Equals(resolution, configuration.Resolution) == false)
				{
					isDirty = true;
				}
			}

			if (GUI.changed && isDirty)
			{
				EditorUtility.SetDirty(configuration);
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}

		void DrawSeperator()
		{
			EditorGUILayout.Space();
			Texture2D tex = new Texture2D(1, 1);  //1 by 1 Pixel
			GUI.color = Color.gray;
			float y = GUILayoutUtility.GetLastRect().yMax;
			GUI.DrawTexture(new Rect(10.0f, y, Screen.width - 10.0f, 1.0f), tex);
			tex.hideFlags = HideFlags.DontSave;
			GUI.color = Color.white;
			EditorGUILayout.Space();
		}
	}
}