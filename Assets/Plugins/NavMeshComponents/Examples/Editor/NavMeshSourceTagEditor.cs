using UnityEngine.AI;

namespace UnityEditor.AI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NavMeshSourceTag))]
    class NavMeshSourceTagEditor : Editor
    {
        SerializedProperty m_Area;

        void OnEnable()
        {
            m_Area = serializedObject.FindProperty("m_Area");

            NavMeshVisualizationSettings.showNavigation++;
        }

        void OnDisable()
        {
            NavMeshVisualizationSettings.showNavigation--;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            NavMeshComponentsGUIUtility.AreaPopup("Area Type", m_Area);
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }
    }
}