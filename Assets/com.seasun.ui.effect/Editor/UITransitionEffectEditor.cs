using UnityEngine.UI.Effect;

namespace UnityEditor.UI.Effect
{
    [CustomEditor(typeof(UITransitionEffect))]
    [CanEditMultipleObjects]
    public class UITransitionEffectEditor : Editor
    {
        SerializedProperty _spEffectMode;
        SerializedProperty _spEffectFactor;
        SerializedProperty _spEffectArea;
        SerializedProperty _spKeepAspectRatio;
        SerializedProperty _spDissolveWidth;
        SerializedProperty _spDissolveSoftness;
        SerializedProperty _spDissolveColor;
        SerializedProperty _spTransitionTexture;

        protected void OnEnable()
        {
            _spEffectMode = serializedObject.FindProperty("m_EffectMode");
            _spEffectFactor = serializedObject.FindProperty("m_EffectFactor");
            _spEffectArea = serializedObject.FindProperty("m_EffectArea");
            _spKeepAspectRatio = serializedObject.FindProperty("m_KeepAspectRatio");
            _spDissolveWidth = serializedObject.FindProperty("m_DissolveWidth");
            _spDissolveSoftness = serializedObject.FindProperty("m_DissolveSoftness");
            _spDissolveColor = serializedObject.FindProperty("m_DissolveColor");
            _spTransitionTexture = serializedObject.FindProperty("m_TransitionTexture");
        }

        public override void OnInspectorGUI()
        {
            using (new MaterialDirtyScope(targets))
                EditorGUILayout.PropertyField(_spEffectMode);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_spEffectFactor);
            if (_spEffectMode.intValue == (int) UITransitionEffect.EffectMode.Dissolve)
            {
                EditorGUILayout.PropertyField(_spDissolveWidth);
                EditorGUILayout.PropertyField(_spDissolveSoftness);
                EditorGUILayout.PropertyField(_spDissolveColor);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(_spEffectArea);
            using (new MaterialDirtyScope(targets))
                EditorGUILayout.PropertyField(_spTransitionTexture);
            EditorGUILayout.PropertyField(_spKeepAspectRatio);
            serializedObject.ApplyModifiedProperties();
        }
    }
}