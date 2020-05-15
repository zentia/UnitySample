using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.Effect
{
    public static class MaterialCache
    {
        private static readonly Dictionary<Hash128, MaterialEntry> materialMap = new Dictionary<Hash128, MaterialEntry>();
        
        private class MaterialEntry
        {
            public Material material;
            public int referenceCount;

            public void Release()
            {
                if (material)
                {
                    Object.DestroyImmediate(material, false);
                }

                material = null;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void ClearCache()
        {
            foreach (var entry in materialMap.Values)
            {
                entry.Release();
            }
            materialMap.Clear();
        }
#endif

        public static Material Register(Material baseMaterial, Hash128 hash, System.Action<Material, Graphic> onModifyMaterial, Graphic graphic)
        {
            if (!hash.isValid) 
                return null;
            if (!materialMap.TryGetValue(hash, out var entry))
            {
                entry = new MaterialEntry()
                {
                    material = new Material(baseMaterial)
                    {
                        hideFlags = HideFlags.HideAndDontSave,
                    },
                };
                onModifyMaterial(entry.material, graphic);
                materialMap.Add(hash, entry);
            }

            entry.referenceCount++;
            return entry.material;
        }

        public static void Unregister(Hash128 hash)
        {
            if (!hash.isValid || !materialMap.TryGetValue(hash, out var entry)) 
                return;
            if (--entry.referenceCount > 0) 
                return;
            entry.Release();
            materialMap.Remove(hash);
        }
    }
}