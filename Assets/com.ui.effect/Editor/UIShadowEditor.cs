using UnityEditor;

namespace UnityEngine.UI.Effect
{
    [CustomEditor(typeof(UIShadow))]
    [CanEditMultipleObjects]
    public class UIShadowEditor : Editor
    {
        private UIEffect uiEffect;
        private SerializedProperty _spStyle;
        private SerializedProperty _spEffectDistance;
        private SerializedProperty _spEffectColor;
        private SerializedProperty _spUseGraphicAlpha;
        private SerializedProperty _spBlurFactor;

        private void OnEnable()
        {
            uiEffect = (target as UIShadow).GetComponent<UIEffect>();
            _spStyle = serializedObject.FindProperty("m_Style");
            _spEffectDistance = serializedObject.FindProperty("m_EffectDistance");
            _spEffectColor = serializedObject.FindProperty("m_EffectColor");
            _spUseGraphicAlpha = serializedObject.FindProperty("m_UseGraphicAlpha");
            _spBlurFactor = serializedObject.FindProperty("m_BlurFactor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_spStyle);

            if (_spStyle.intValue != (int) ShadowStyle.None)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_spEffectDistance);
                EditorGUILayout.PropertyField(_spEffectColor);
                EditorGUILayout.PropertyField(_spUseGraphicAlpha);
                if (uiEffect && uiEffect.blurMode != BlurMode.None)
                {
                    EditorGUILayout.PropertyField(_spBlurFactor);
                }
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
