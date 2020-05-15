using System.Linq;
using UnityEngine;
using UnityEngine.UI.Effect;

namespace UnityEditor.UI.Effect
{
    internal class MaterialDirtyScope : EditorGUI.ChangeCheckScope
    {
        private readonly Object[] targets;

        public MaterialDirtyScope(Object[] targets)
        {
            this.targets = targets;
        }

        protected override void CloseScope()
        {
            if (changed)
            {
                foreach (var effect in targets.OfType<BaseMaterialEffect>())
                {
                    effect.SetMaterialDirty();
                }
            }
            base.CloseScope();
        }
    }
}
