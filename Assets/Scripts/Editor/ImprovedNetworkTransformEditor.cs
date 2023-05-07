using UnityEditor;
using UnityEngine;
using Unity.Netcode.Components;

namespace Unity.Netcode.Editor
{
    /// <summary>
    /// The <see cref="CustomEditor"/> for <see cref="ImprovedNetworkTransform"/>
    /// </summary>
    [CustomEditor(typeof(ImprovedNetworkTransform), true)]
    public class ImprovedNetworkTransformEditor : UnityEditor.Editor
    {
        private SerializedProperty m_SyncPositionXProperty;
        private SerializedProperty m_SyncPositionYProperty;
        private SerializedProperty m_SyncPositionZProperty;
        private SerializedProperty m_SyncRotationXProperty;
        private SerializedProperty m_SyncRotationYProperty;
        private SerializedProperty m_SyncRotationZProperty;
        private SerializedProperty m_SyncScaleXProperty;
        private SerializedProperty m_SyncScaleYProperty;
        private SerializedProperty m_SyncScaleZProperty;
        private SerializedProperty m_PositionThresholdProperty;
        private SerializedProperty m_RotAngleThresholdProperty;
        private SerializedProperty m_ScaleThresholdProperty;
        private SerializedProperty m_InLocalSpaceProperty;
        private SerializedProperty m_InterpolateProperty;
        private SerializedProperty m_InterpolatePositionXProperty;
        private SerializedProperty m_InterpolatePositionYProperty;
        private SerializedProperty m_InterpolatePositionZProperty;
        private SerializedProperty m_InterpolateRotationXProperty;
        private SerializedProperty m_InterpolateRotationYProperty;
        private SerializedProperty m_InterpolateRotationZProperty;
        private SerializedProperty m_InterpolateScaleXProperty;
        private SerializedProperty m_InterpolateScaleYProperty;
        private SerializedProperty m_InterpolateScaleZProperty;
        private SerializedProperty m_ClientAuth;

        private static int s_ToggleOffset = 45;
        private static float s_MaxRowWidth = EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth + 5;
        private static GUIContent s_PositionLabel = EditorGUIUtility.TrTextContent("Position");
        private static GUIContent s_RotationLabel = EditorGUIUtility.TrTextContent("Rotation");
        private static GUIContent s_ScaleLabel = EditorGUIUtility.TrTextContent("Scale");

        /// <inheritdoc/>
        public void OnEnable()
        {
            m_SyncPositionXProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.SyncPositionX));
            m_SyncPositionYProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.SyncPositionY));
            m_SyncPositionZProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.SyncPositionZ));
            m_SyncRotationXProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.SyncRotAngleX));
            m_SyncRotationYProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.SyncRotAngleY));
            m_SyncRotationZProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.SyncRotAngleZ));
            m_SyncScaleXProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.SyncScaleX));
            m_SyncScaleYProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.SyncScaleY));
            m_SyncScaleZProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.SyncScaleZ));
            m_PositionThresholdProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.PositionThreshold));
            m_RotAngleThresholdProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.RotAngleThreshold));
            m_ScaleThresholdProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.ScaleThreshold));
            m_InLocalSpaceProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.InLocalSpace));
            m_InterpolateProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.Interpolate));
            m_InterpolatePositionXProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.InterpolatePositionX));
            m_InterpolatePositionYProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.InterpolatePositionY));
            m_InterpolatePositionZProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.InterpolatePositionZ));
            m_InterpolateRotationXProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.InterpolateRotationX));
            m_InterpolateRotationYProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.InterpolateRotationY));
            m_InterpolateRotationZProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.InterpolateRotationZ));
            m_InterpolateScaleXProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.InterpolateScaleX));
            m_InterpolateScaleYProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.InterpolateScaleY));
            m_InterpolateScaleZProperty = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.InterpolateScaleZ));
            m_ClientAuth = serializedObject.FindProperty(nameof(ImprovedNetworkTransform.IsClientAuthoritative));
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Syncing", EditorStyles.boldLabel);
            {
                GUILayout.BeginHorizontal();

                var rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, s_MaxRowWidth, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.numberField);
                var ctid = GUIUtility.GetControlID(7231, FocusType.Keyboard, rect);

                rect = EditorGUI.PrefixLabel(rect, ctid, s_PositionLabel);
                rect.width = s_ToggleOffset;

                m_SyncPositionXProperty.boolValue = EditorGUI.ToggleLeft(rect, "X", m_SyncPositionXProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_SyncPositionYProperty.boolValue = EditorGUI.ToggleLeft(rect, "Y", m_SyncPositionYProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_SyncPositionZProperty.boolValue = EditorGUI.ToggleLeft(rect, "Z", m_SyncPositionZProperty.boolValue);

                GUILayout.EndHorizontal();
            }
            {
                GUILayout.BeginHorizontal();

                var rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, s_MaxRowWidth, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.numberField);
                var ctid = GUIUtility.GetControlID(7231, FocusType.Keyboard, rect);

                rect = EditorGUI.PrefixLabel(rect, ctid, s_RotationLabel);
                rect.width = s_ToggleOffset;

                m_SyncRotationXProperty.boolValue = EditorGUI.ToggleLeft(rect, "X", m_SyncRotationXProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_SyncRotationYProperty.boolValue = EditorGUI.ToggleLeft(rect, "Y", m_SyncRotationYProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_SyncRotationZProperty.boolValue = EditorGUI.ToggleLeft(rect, "Z", m_SyncRotationZProperty.boolValue);

                GUILayout.EndHorizontal();
            }
            {
                GUILayout.BeginHorizontal();

                var rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, s_MaxRowWidth, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.numberField);
                var ctid = GUIUtility.GetControlID(7231, FocusType.Keyboard, rect);

                rect = EditorGUI.PrefixLabel(rect, ctid, s_ScaleLabel);
                rect.width = s_ToggleOffset;

                m_SyncScaleXProperty.boolValue = EditorGUI.ToggleLeft(rect, "X", m_SyncScaleXProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_SyncScaleYProperty.boolValue = EditorGUI.ToggleLeft(rect, "Y", m_SyncScaleYProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_SyncScaleZProperty.boolValue = EditorGUI.ToggleLeft(rect, "Z", m_SyncScaleZProperty.boolValue);

                GUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Individual Interpolation", EditorStyles.boldLabel);
            {
                GUILayout.BeginHorizontal();

                var rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, s_MaxRowWidth, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.numberField);
                var ctid = GUIUtility.GetControlID(7231, FocusType.Keyboard, rect);

                rect = EditorGUI.PrefixLabel(rect, ctid, s_PositionLabel);
                rect.width = s_ToggleOffset;

                m_InterpolatePositionXProperty.boolValue = EditorGUI.ToggleLeft(rect, "X", m_InterpolatePositionXProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_InterpolatePositionYProperty.boolValue = EditorGUI.ToggleLeft(rect, "Y", m_InterpolatePositionYProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_InterpolatePositionZProperty.boolValue = EditorGUI.ToggleLeft(rect, "Z", m_InterpolatePositionZProperty.boolValue);

                GUILayout.EndHorizontal();
            }
            {
                GUILayout.BeginHorizontal();

                var rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, s_MaxRowWidth, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.numberField);
                var ctid = GUIUtility.GetControlID(7231, FocusType.Keyboard, rect);

                rect = EditorGUI.PrefixLabel(rect, ctid, s_RotationLabel);
                rect.width = s_ToggleOffset;

                m_InterpolateRotationXProperty.boolValue = EditorGUI.ToggleLeft(rect, "X", m_InterpolateRotationXProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_InterpolateRotationYProperty.boolValue = EditorGUI.ToggleLeft(rect, "Y", m_InterpolateRotationYProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_InterpolateRotationZProperty.boolValue = EditorGUI.ToggleLeft(rect, "Z", m_InterpolateRotationZProperty.boolValue);

                GUILayout.EndHorizontal();
            }
            {
                GUILayout.BeginHorizontal();

                var rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, s_MaxRowWidth, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.numberField);
                var ctid = GUIUtility.GetControlID(7231, FocusType.Keyboard, rect);

                rect = EditorGUI.PrefixLabel(rect, ctid, s_ScaleLabel);
                rect.width = s_ToggleOffset;

                m_InterpolateScaleXProperty.boolValue = EditorGUI.ToggleLeft(rect, "X", m_InterpolateScaleXProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_InterpolateScaleYProperty.boolValue = EditorGUI.ToggleLeft(rect, "Y", m_InterpolateScaleYProperty.boolValue);
                rect.x += s_ToggleOffset;
                m_InterpolateScaleZProperty.boolValue = EditorGUI.ToggleLeft(rect, "Z", m_InterpolateScaleZProperty.boolValue);

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Thresholds", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_PositionThresholdProperty);
            EditorGUILayout.PropertyField(m_RotAngleThresholdProperty);
            EditorGUILayout.PropertyField(m_ScaleThresholdProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Configurations", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_InLocalSpaceProperty);
            EditorGUILayout.PropertyField(m_InterpolateProperty);
            EditorGUILayout.PropertyField(m_ClientAuth);

#if COM_UNITY_MODULES_PHYSICS
            // if rigidbody is present but network rigidbody is not present
            var go = ((ImprovedNetworkTransform)target).gameObject;
            if (go.TryGetComponent<Rigidbody>(out _) && go.TryGetComponent<NetworkRigidbody>(out _) == false)
            {
                EditorGUILayout.HelpBox("This GameObject contains a Rigidbody but no NetworkRigidbody.\n" +
                    "Add a NetworkRigidbody component to improve Rigidbody synchronization.", MessageType.Warning);
            }
#endif // COM_UNITY_MODULES_PHYSICS

#if COM_UNITY_MODULES_PHYSICS2D
            if (go.TryGetComponent<Rigidbody2D>(out _) && go.TryGetComponent<ImprovedNetworkRigidbody2D>(out _) == false)
            {
                EditorGUILayout.HelpBox("This GameObject contains a Rigidbody2D but no ImprovedNetworkRigidbody2D.\n" +
                    "Add an ImprovedNetworkRigidbody2D component to improve Rigidbody2D synchronization.", MessageType.Warning);
            }
#endif // COM_UNITY_MODULES_PHYSICS2D

            serializedObject.ApplyModifiedProperties();
        }
    }
}