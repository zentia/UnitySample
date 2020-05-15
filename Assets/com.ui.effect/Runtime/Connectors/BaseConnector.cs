using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.Effect
{
    public class BaseConnector
    {
        private static readonly List<BaseConnector> s_Connectors = new List<BaseConnector>();
        private static readonly Dictionary<Type, BaseConnector> s_ConnectorMap = new Dictionary<Type, BaseConnector>();
        private static readonly BaseConnector s_EmptyConnector = new BaseConnector();

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            AddConnector(s_EmptyConnector);
        }

        protected static void AddConnector(BaseConnector connector)
        {
            s_Connectors.Add(connector);
            s_Connectors.Sort((x, y) => y.priority - x.priority);
        }

        public static BaseConnector FindConnector(Graphic graphic)
        {
            if (!graphic) 
                return s_EmptyConnector;

            var type = graphic.GetType();
            if (s_ConnectorMap.TryGetValue(type, out var connector)) 
                return connector;

            foreach (var c in s_Connectors)
            {
                if (!c.IsValid(graphic)) 
                    continue;
                s_ConnectorMap.Add(type, c);
                return c;
            }
            return s_EmptyConnector;
        }

        protected virtual bool IsValid(Graphic graphic)
        {
            return true;
        }

        public virtual Shader FindShader(string shaderName)
        {
            return null;
        }

        protected virtual int priority => -1;

        public virtual AdditionalCanvasShaderChannels extraChannel => AdditionalCanvasShaderChannels.None;

        public virtual void SetMaterial(Graphic graphic, Material material)
        {
        }

        public virtual Material GetMaterial(Graphic graphic)
        {
            return null;
        }

        public virtual void SetVerticesDirty(Graphic graphic)
        {
        }

        public virtual void SetMaterialDirty(Graphic graphic)
        {
        }

        public virtual void OnEnable(Graphic graphic)
        {
        }

        public virtual void OnDisable(Graphic graphic)
        {
        }

        protected virtual void OnWillRenderCanvases()
        {
        }

        public void GetPositionFactor(EffectArea area, int index, Rect rect, Vector2 position, out float x, out float y)
        {
            if (area == EffectArea.Fit)
            {
                x = Mathf.Clamp01((position.x - rect.xMin) / rect.width);
                y = Mathf.Clamp01((position.y - rect.yMin) / rect.height);
            }
            else
            {
                x = Mathf.Clamp01(position.x / rect.width + 0.5f);
                y = Mathf.Clamp01(position.y / rect.height + 0.5f);
            }
        }

        public void GetNormalizedFactor(EffectArea area, int index, Matrix2x3 matrix, Vector2 position, out Vector2 normalizedPos)
        {
            normalizedPos = matrix * position;
        }

        public virtual bool IsText(Graphic graphic)
        {
            return false;
        }

        public virtual void SetExtraChannel(ref UIVertex vertex, Vector2 value)
        {
        }
    }
}