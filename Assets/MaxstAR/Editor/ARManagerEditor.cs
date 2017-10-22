using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace maxstAR
{
    [CustomEditor(typeof(ARManager))]
    public class ARManagerEditor : Editor
    {
        private const int maxHeight = 25;
        //private ARManager arManager = null;

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

            //arManager = (ARManager)target;

            GUIContent content = new GUIContent("Configuration");
            GUILayout.Space(10);
            if (GUILayout.Button(content, GUILayout.MaxWidth(Screen.width), GUILayout.MaxHeight(maxHeight)))
            {
                Selection.activeObject = ConfigurationScriptableObject.GetInstance();
            }
            GUILayout.Space(10);
        }
    }
}