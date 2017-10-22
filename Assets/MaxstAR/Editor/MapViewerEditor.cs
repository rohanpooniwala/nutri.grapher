using UnityEngine;
using UnityEditor;
using System.Collections;

namespace maxstAR
{
    [CustomEditor(typeof(MapViewerBehaviour))]
    public class MapViewerEditor : Editor
    {
        private MapViewerBehaviour mapController = null;

        private bool changedSlideBar = false;
        private bool changedCheckBox = false;
        private bool autoCamera = false;
        private bool transparent = false;
        private bool reconstruction = false;
        private int keyframeId = 0;

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

            mapController = (MapViewerBehaviour)target;

            changedSlideBar = false;
            changedCheckBox = false;

            keyframeId = EditorGUILayout.IntSlider("Keyframe Id", mapController.KeyframeNumber, 0, mapController.KeyframeSize - 1);
            reconstruction = EditorGUILayout.Toggle("Generate Mesh", mapController.Reconstruction);
            autoCamera = EditorGUILayout.Toggle("Auto Camera", mapController.AutoCamera);
            transparent = EditorGUILayout.Toggle("Transparent", mapController.Transparent);

            if (mapController.KeyframeNumber != keyframeId)
            {
                changedSlideBar = true;
                mapController.KeyframeNumber = keyframeId;
            }

            if (mapController.Reconstruction != reconstruction)
            {
                changedCheckBox = true;
                mapController.Reconstruction = reconstruction;
            }

            if (mapController.AutoCamera != autoCamera)
            {
                changedCheckBox = true;
                mapController.AutoCamera = autoCamera;
            }

            if (mapController.Transparent != transparent)
            {
                changedCheckBox = true;
                mapController.Transparent = transparent;
            }

            serializedObject.Update();

            if (GUI.changed)
            {
                if (mapController != null)
                {
                    if (changedSlideBar)
                    {
                        mapController.SceneUpdate();
                    }
                    if (changedCheckBox)
                    {
                        mapController.SceneUpdate();
                    }

                    EditorUtility.SetDirty(mapController);
                }
            }
        }

        void OnSceneGUI()
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDrag:
                case EventType.ScrollWheel:
                    if (autoCamera)
                    {
                        mapController.SetAutoKeyframe(SceneView.lastActiveSceneView.camera.transform.position, SceneView.lastActiveSceneView.camera.transform.rotation);
                        mapController.SceneUpdate();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}