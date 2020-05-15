using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.Effect
{
    [AddComponentMenu("UI/Effects/消隐", 5)]
    public class UITransitionEffect : BaseMaterialEffect
    {
        public enum EffectMode
        {
            Fade = 1,
            Cutoff = 2,
            Dissolve = 3,
        }

        private const uint k_ShaderId = 5 << 3;
        private static readonly int k_TransitionTexId = Shader.PropertyToID("_TransitionTex");
        private static readonly ParameterTexture s_ParamTex = new ParameterTexture(8, 128, "_ParamTex");

        private bool _lastKeepAspectRatio;
        private static Texture _defaultTransitionTexture;

        [SerializeField]
        private EffectMode m_EffectMode = EffectMode.Cutoff;

        [Tooltip("Effect factor between 0(hidden) and 1(shown).")] [SerializeField] [Range(0, 1)]
        private float m_EffectFactor = 0.5f;

        [Tooltip("Transition texture (single channel texture).")] [SerializeField]
        private Texture m_TransitionTexture;

        [Header("Advanced Option")] [SerializeField]
        private EffectArea m_EffectArea = EffectArea.RectTransform;

        [Tooltip("Keep effect aspect ratio.")] [SerializeField]
        private bool m_KeepAspectRatio;

        [Tooltip("Dissolve edge width.")] [SerializeField] [Range(0, 1)]
        private float m_DissolveWidth = 0.5f;

        [Tooltip("Dissolve edge softness.")] [SerializeField] [Range(0, 1)]
        private float m_DissolveSoftness = 0.5f;

        [Tooltip("Dissolve edge color.")] [SerializeField] [ColorUsage(false)]
        private Color m_DissolveColor = new Color(0.0f, 0.25f, 1.0f);

        [Tooltip("Disable the graphic's raycast target on hidden.")] [SerializeField]
        private bool m_PassRayOnHidden;

        public float effectFactor
        {
            get  => m_EffectFactor;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_EffectFactor, value)) return;
                m_EffectFactor = value;
                SetEffectParamsDirty();
            }
        }

        public Texture transitionTexture
        {
            get => m_TransitionTexture;
            set
            {
                if (m_TransitionTexture == value) 
                    return;
                m_TransitionTexture = value;
                SetMaterialDirty();
            }
        }

        public EffectMode effectMode
        {
            get => m_EffectMode; 
            set
            {
                if (m_EffectMode == value) return;
                m_EffectMode = value;
                SetMaterialDirty();
            }
        }

        public bool keepAspectRatio
        {
            get => m_KeepAspectRatio;
            set
            {
                if (m_KeepAspectRatio == value) 
                    return;
                m_KeepAspectRatio = value;
                SetVerticesDirty();
            }
        }

        public override ParameterTexture paramTex => s_ParamTex;

        public float dissolveWidth
        {
            get  => m_DissolveWidth;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_DissolveWidth, value)) return;
                m_DissolveWidth = value;
                SetEffectParamsDirty();
            }
        }

        public float dissolveSoftness
        {
            get  => m_DissolveSoftness;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_DissolveSoftness, value)) return;
                m_DissolveSoftness = value;
                SetEffectParamsDirty();
            }
        }

        public Color dissolveColor
        {
            get => m_DissolveColor;
            set
            {
                if (m_DissolveColor == value) 
                    return;
                m_DissolveColor = value;
                SetEffectParamsDirty();
            }
        }

        public override Hash128 GetMaterialHash(Material material)
        {
            if (!isActiveAndEnabled || !material || !material.shader || !transitionTexture)
                return k_InvalidHash;
            var shaderVariantId = (uint) ((int) m_EffectMode << 6);
            var resourceId = (uint) transitionTexture.GetInstanceID();
            return new Hash128((uint) material.GetInstanceID(), k_ShaderId + shaderVariantId, resourceId, 0);
        }

        public override void ModifyMaterial(Material newMaterial, Graphic g)
        {
            var c = BaseConnector.FindConnector(g);
            newMaterial.shader = c.FindShader("UITransition");
            SetShaderVariants(newMaterial, m_EffectMode);
            newMaterial.SetTexture(k_TransitionTexId, transitionTexture);
            paramTex.RegisterMaterial(newMaterial);
        }

        public override void ModifyMesh(VertexHelper vh, Graphic g)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            var normalizedIndex = paramTex.GetNormalizedIndex(this);
            var tex = transitionTexture;
            var aspectRatio = m_KeepAspectRatio && tex ? ((float) tex.width) / tex.height : -1;
            var rect = m_EffectArea.GetEffectArea(vh, rectTransform.rect, aspectRatio);
            var vertex = default(UIVertex);
            var count = vh.currentVertCount;
            for (var i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                connector.GetPositionFactor(m_EffectArea, i, rect, vertex.position, out var x, out var y);
                vertex.uv0 = new Vector2(Packer.ToFloat(vertex.uv0.x, vertex.uv0.y), Packer.ToFloat(x, y, normalizedIndex));
                vh.SetUIVertex(vertex, i);
            }
        }

        protected override void SetEffectParamsDirty()
        {
            paramTex.SetData(this, 0, m_EffectFactor); // param1.x : effect factor
            if (m_EffectMode == EffectMode.Dissolve)
            {
                paramTex.SetData(this, 1, m_DissolveWidth); // param1.y : width
                paramTex.SetData(this, 2, m_DissolveSoftness); // param1.z : softness
                paramTex.SetData(this, 4, m_DissolveColor.r); // param2.x : red
                paramTex.SetData(this, 5, m_DissolveColor.g); // param2.y : green
                paramTex.SetData(this, 6, m_DissolveColor.b); // param2.z : blue
            }

            if (m_PassRayOnHidden)
            {
                graphic.raycastTarget = 0 < m_EffectFactor;
            }
        }

        protected override void SetVerticesDirty()
        {
            base.SetVerticesDirty();

            _lastKeepAspectRatio = m_KeepAspectRatio;
        }

        protected override void OnDidApplyAnimationProperties()
        {
            base.OnDidApplyAnimationProperties();

            if (_lastKeepAspectRatio != m_KeepAspectRatio)
                SetVerticesDirty();
        }
    }
}