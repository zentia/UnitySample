using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.UI.Effect
{
    [RequireComponent(typeof(Graphic))]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public abstract class BaseMeshEffect : UIBehaviour, IMeshModifier
    {
        private RectTransform _rectTransform;
        private Graphic _graphic;
        private BaseConnector _connector;

        protected BaseConnector connector =>_connector ?? (_connector = BaseConnector.FindConnector(graphic));
        
        public Graphic graphic =>_graphic ? _graphic : _graphic = GetComponent<Graphic>();

        protected RectTransform rectTransform => _rectTransform ? _rectTransform : _rectTransform = GetComponent<RectTransform>();
        
        internal readonly List<UISyncEffect> syncEffects = new List<UISyncEffect>(0);

        public virtual void ModifyMesh(Mesh mesh)
        {
        }

        public virtual void ModifyMesh(VertexHelper vh)
        {
            ModifyMesh(vh, graphic);
        }

        public virtual void ModifyMesh(VertexHelper vh, Graphic g)
        {
        }

        protected virtual void SetVerticesDirty()
        {
            connector.SetVerticesDirty(graphic);
            foreach (var effect in syncEffects)
            {
                effect.SetVerticesDirty();
            }
        }

        protected override void OnEnable()
        {
            connector.OnEnable(graphic);
            SetVerticesDirty();
        }

        protected override void OnDisable()
        {
            connector.OnDisable(graphic);
            SetVerticesDirty();
        }

        protected virtual void SetEffectParamsDirty()
        {
            if (!isActiveAndEnabled) 
                return;
            SetVerticesDirty();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            if (!isActiveAndEnabled) 
                return;
            SetEffectParamsDirty();
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            if (!isActiveAndEnabled) 
                return;
            SetVerticesDirty();
        }

        protected override void OnValidate()
        {
            if (!isActiveAndEnabled) 
                return;
            SetEffectParamsDirty();
        }
#endif
    }
}