using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.U2D.Path;
using UnityEditor.U2D.Common;

namespace UnityEngine.UI.Effect
{
    public interface IShortcutToolContext
    {
        bool active { get; }
    }
    public static class EditorBridge
    {
        public class ShortcutContext : IShortcutToolContext
        {
            public Func<bool> isActive;

            public bool active
            {
                get
                {
                    if (isActive != null)
                        return isActive();
                    return true;
                }
            }
            public object context { get; set; }
        }

        public static void RegisterShortcutContext(ShortcutContext context)
        {
            
        }
    }

    [Serializable]
    public class SpriteShapeData
    {
        public float height = -1;
        public int spriteIndex;
        public bool corner = true;
    }
    
    public class CustomPath : GenericScriptablePath<SpriteShapeData>{}

    [EditorTool("Edit SVGShape", typeof(CUIGraphic))]
    internal class SpriteShapeEditorTool : PathEditorTool<CustomPath>
    {
        private static InternalEditorBridge.ShortcutContext m_ShortcutContext;
        public static SpriteShapeEditorTool activeSpriteShapeEditorTool
        {
            get
            {
                if (m_ShortcutContext != null)
                    return m_ShortcutContext.context as SpriteShapeEditorTool;
                return null;
            }
        }
        protected override void SetShape(CustomPath path, SerializedObject serializedObject)
        {
            serializedObject.Update();
            var refCurvesProp = serializedObject.FindProperty("refCurves");
            var controlPointsProp = refCurvesProp.FindPropertyRelative("controlPoints");
            
            
            for (int i = 0; i < path.pointCount; i++)
            {
                
            }
            serializedObject.ApplyModifiedProperties();
        }

        protected override IShape GetShape(Object target)
        {
            return Polygon.empty;
        }
    }
}