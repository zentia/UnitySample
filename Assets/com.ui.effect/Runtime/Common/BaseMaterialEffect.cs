using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.Effect
{
    [DisallowMultipleComponent]
    public abstract class BaseMaterialEffect : BaseMeshEffect, IParameterTexture, IMaterialModifier
    {
        protected static readonly Hash128 k_InvalidHash = new Hash128();
        protected static readonly List<UIVertex> s_TempVerts = new List<UIVertex>();
        private static readonly StringBuilder s_StringBuilder = new StringBuilder();

        private Hash128 _effectMaterialHash;

        public int parameterIndex { get; set; }

        public virtual ParameterTexture paramTex => null;
        
        public void SetMaterialDirty()
        {
            connector.SetMaterialDirty(graphic);

            foreach (var effect in syncEffects)
            {
                effect.SetMaterialDirty();
            }
        }

        public virtual Hash128 GetMaterialHash(Material baseMaterial)
        {
            return k_InvalidHash;
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            return GetModifiedMaterial(baseMaterial, graphic);
        }

        protected virtual Material GetModifiedMaterial(Material baseMaterial, Graphic graphic)
        {
            if (!isActiveAndEnabled) 
                return baseMaterial;

            var oldHash = _effectMaterialHash;
            _effectMaterialHash = GetMaterialHash(baseMaterial);
            var modifiedMaterial = baseMaterial;
            if (_effectMaterialHash.isValid)
            {
                modifiedMaterial = MaterialCache.Register(baseMaterial, _effectMaterialHash, ModifyMaterial, graphic);
            }

            MaterialCache.Unregister(oldHash);

            return modifiedMaterial;
        }

        public virtual void ModifyMaterial(Material newMaterial, Graphic g)
        {
            if (isActiveAndEnabled)
                paramTex?.RegisterMaterial(newMaterial);
        }

        protected void SetShaderVariants(Material newMaterial, params object[] variants)
        {
            var keywords = variants.Where(x => 0 < (int) x)
                .Select(x => x.ToString().ToUpper())
                .Concat(newMaterial.shaderKeywords)
                .Distinct()
                .ToArray();
            newMaterial.shaderKeywords = keywords;

            s_StringBuilder.Length = 0;
            s_StringBuilder.Append(Path.GetFileName(newMaterial.shader.name));
            foreach (var keyword in keywords)
            {
                s_StringBuilder.Append("-");
                s_StringBuilder.Append(keyword);
            }
            newMaterial.name = s_StringBuilder.ToString();
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            if (!isActiveAndEnabled) 
                return;
            SetMaterialDirty();
            SetVerticesDirty();
            SetEffectParamsDirty();
        }

        protected override void OnValidate()
        {
            if (!isActiveAndEnabled) 
                return;
            SetVerticesDirty();
            SetEffectParamsDirty();
        }
#endif

        protected override void OnEnable()
        {
            paramTex?.Register(this);
            SetVerticesDirty();
            SetMaterialDirty();
            SetEffectParamsDirty();
        }

        protected override void OnDisable()
        {
            SetVerticesDirty();
            SetMaterialDirty();
            paramTex?.Unregister(this);
            MaterialCache.Unregister(_effectMaterialHash);
            _effectMaterialHash = k_InvalidHash;
        }
    }
}