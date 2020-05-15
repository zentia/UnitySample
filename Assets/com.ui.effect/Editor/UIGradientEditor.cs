using Coffee.UIExtensions;
using UnityEditor;

namespace UnityEngine.UI.Effect
{
    [CustomEditor(typeof(UIGradient))]
    [CanEditMultipleObjects]
    public class UIGradientEditor : Editor
    {
        private static readonly GUIContent k_TextVerticalOffset = new GUIContent("Vertical Offset");
        private static readonly GUIContent k_TextHorizontalOffset = new GUIContent("Horizontal Offset");
        private static readonly GUIContent k_TextOffset = new GUIContent("Offset");
        private static readonly GUIContent k_TextLeft = new GUIContent("Left");
        private static readonly GUIContent k_TextRight = new GUIContent("Right");
        private static readonly GUIContent k_TextTop = new GUIContent("Top");
        private static readonly GUIContent k_TextBottom = new GUIContent("Bottom");
        private static readonly GUIContent k_TextColor1 = new GUIContent("Color 1");
        private static readonly GUIContent k_TextColor2 = new GUIContent("Color 2");
        private static readonly GUIContent k_TextDiagonalColor = new GUIContent("Diagonal Color");

        private SerializedProperty _spDirection;
        private SerializedProperty _spColor1;
        private SerializedProperty _spColor2;
        private SerializedProperty _spColor3;
        private SerializedProperty _spColor4;
        private SerializedProperty _spRotation;
        private SerializedProperty _spOffset1;
        private SerializedProperty _spOffset2;
        private SerializedProperty _spIgnoreAspectRatio;
        private SerializedProperty _spGradientStyle;
        private SerializedProperty _spColorSpace;

        protected void OnEnable()
        {
            _spIgnoreAspectRatio = serializedObject.FindProperty("m_IgnoreAspectRatio");
            _spDirection = serializedObject.FindProperty("m_Direction");
            _spColor1 = serializedObject.FindProperty("m_Color1");
            _spColor2 = serializedObject.FindProperty("m_Color2");
            _spColor3 = serializedObject.FindProperty("m_Color3");
            _spColor4 = serializedObject.FindProperty("m_Color4");
            _spRotation = serializedObject.FindProperty("m_Rotation");
            _spOffset1 = serializedObject.FindProperty("m_Offset1");
            _spOffset2 = serializedObject.FindProperty("m_Offset2");
            _spGradientStyle = serializedObject.FindProperty("m_GradientStyle");
            _spColorSpace = serializedObject.FindProperty("m_ColorSpace");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_spDirection);
            switch ((UIGradient.Direction) _spDirection.intValue)
            {
                case UIGradient.Direction.Horizontal:
                    EditorGUILayout.PropertyField(_spColor1, k_TextLeft);
                    EditorGUILayout.PropertyField(_spColor2, k_TextRight);
                    break;
                case UIGradient.Direction.Vertical:
                    EditorGUILayout.PropertyField(_spColor1, k_TextTop);
                    EditorGUILayout.PropertyField(_spColor2, k_TextBottom);
                    break;
                case UIGradient.Direction.Angle:
                    EditorGUILayout.PropertyField(_spColor1, k_TextColor1);
                    EditorGUILayout.PropertyField(_spColor2, k_TextColor2);
                    break;
                case UIGradient.Direction.Diagonal:
                    Rect r = EditorGUILayout.GetControlRect(false, 34);

                    r = EditorGUI.PrefixLabel(r, k_TextDiagonalColor);
                    float w = r.width / 2;

                    EditorGUI.PropertyField(new Rect(r.x, r.y, w, 16), _spColor3, GUIContent.none);
                    EditorGUI.PropertyField(new Rect(r.x + w, r.y, w, 16), _spColor4, GUIContent.none);
                    EditorGUI.PropertyField(new Rect(r.x, r.y + 18, w, 16), _spColor1, GUIContent.none);
                    EditorGUI.PropertyField(new Rect(r.x + w, r.y + 18, w, 16), _spColor2, GUIContent.none);
                    break;
            }
            if ((int) UIGradient.Direction.Angle <= _spDirection.intValue)
            {
                EditorGUILayout.PropertyField(_spRotation);
            }
            if ((int) UIGradient.Direction.Diagonal == _spDirection.intValue)
            {
                EditorGUILayout.PropertyField(_spOffset1, k_TextVerticalOffset);
                EditorGUILayout.PropertyField(_spOffset2, k_TextHorizontalOffset);
            }
            else
            {
                EditorGUILayout.PropertyField(_spOffset1, k_TextOffset);
            }
            EditorGUILayout.LabelField("Advanced Options", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_spGradientStyle);
            EditorGUILayout.PropertyField(_spColorSpace);
            EditorGUILayout.PropertyField(_spIgnoreAspectRatio);
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}