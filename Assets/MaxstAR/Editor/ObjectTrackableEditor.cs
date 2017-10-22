using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
	[CustomEditor(typeof(ObjectTrackableBehaviour))]
	public class ObjectTrackableEditor : Editor
	{
		private ObjectTrackableBehaviour trackableBehaviour = null;

		private void LoadMapViewer(string filePath)
		{
			if (trackableBehaviour.mapViewer == null)
			{
				trackableBehaviour.mapViewer = new GameObject("MapViewer");
				trackableBehaviour.mapViewer.transform.parent = trackableBehaviour.transform;
				trackableBehaviour.mapViewer.AddComponent<MapViewerBehaviour>();

				GameObject keyframes = new GameObject("Keyframes");
				keyframes.transform.parent = trackableBehaviour.mapViewer.transform;
				keyframes.AddComponent<KeyframeViewerBehaviour>();

				GameObject mappoints = new GameObject("Mappoints");
				mappoints.transform.parent = trackableBehaviour.mapViewer.transform;
				mappoints.AddComponent<MappointViewerBehaviour>();
			}

			MapViewerBehaviour mapViewerBehaviour = trackableBehaviour.mapViewer.GetComponent<MapViewerBehaviour>();
			mapViewerBehaviour.Load(filePath);
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

			bool isDirty = false;

			trackableBehaviour = (ObjectTrackableBehaviour)target;

			EditorGUILayout.Separator();

			StorageType oldType = trackableBehaviour.StorageType;
			StorageType newType = (StorageType)EditorGUILayout.EnumPopup("Storage type", trackableBehaviour.StorageType);

			if (oldType != newType)
			{
				trackableBehaviour.StorageType = newType;
				isDirty = true;
			}

			//trackableBehaviour.StorageType =
			//	(StorageType)EditorGUILayout.EnumPopup("Storage type", trackableBehaviour.StorageType);
			if (trackableBehaviour.StorageType == StorageType.StreamingAssets)
			{
				EditorGUILayout.HelpBox("Just drag&drop a *.3dmap file with tracking data from your project view here", MessageType.Info);
				EditorGUILayout.Separator();

				UnityEngine.Object oldDataObject = trackableBehaviour.TrackerDataFileObject;
				UnityEngine.Object newDataObject = EditorGUILayout.ObjectField(trackableBehaviour.TrackerDataFileObject,
					typeof(UnityEngine.Object), true);

				if (oldDataObject != newDataObject)
				{
					string trackerDataFileName = AssetDatabase.GetAssetPath(newDataObject);
					if (!trackerDataFileName.EndsWith(".3dmap"))
					{
						Debug.Log("trackerDataFileName: " + trackerDataFileName);
						Debug.LogError("It's not proper tracker data file!!. File's extension should be .3dmap");
					}
					else
					{
						trackableBehaviour.TrackerDataFileObject = newDataObject;
						trackableBehaviour.TrackerDataFileName =
							trackerDataFileName.Replace("Assets/StreamingAssets/", "");
						isDirty = true;

						LoadMapViewer(trackerDataFileName);
					}
				}
			}

			if (trackableBehaviour.StorageType == StorageType.StreamingAssets)
			{
				GUILayout.BeginHorizontal(GUILayout.Width(0));
				GUILayout.FlexibleSpace();
				GUIContent content = new GUIContent("Load");

				if (GUILayout.Button(content, GUILayout.Width(100)))
				{
					string trackerDataFileName = AssetDatabase.GetAssetPath(trackableBehaviour.TrackerDataFileObject);
					if (System.IO.File.Exists(trackerDataFileName))
					{
						LoadMapViewer(trackerDataFileName);
					}
					else
					{
						EditorGUILayout.HelpBox("Error! : \"File isn't exist\"", MessageType.Error);
					}
				}
				GUILayout.EndHorizontal();
			}

			if (GUI.changed && isDirty)
			{
				EditorUtility.SetDirty(trackableBehaviour);
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				SceneManager.Instance.SceneUpdated();
			}
		}
	}
}