using System;

namespace UnityEngine.UI.Extensions
{
    public enum ShapeTangentMode
    {
        Linear,
        Continuous,
        Broken,
    }
    [Serializable]
    public class SplineControlPoint
    {
        public Vector3 position;
        public Vector3 leftTangent;
        public Vector3 rightTangent;
        public ShapeTangentMode mode;
        public float height = 1f;
        public float bevelCutoff;
        public float bevelSize;
        public int spriteIndex;
        public bool corner;

        public override int GetHashCode()
        {
            return ((int) position.x).GetHashCode() ^ ((int) position.y).GetHashCode() ^ position.GetHashCode() ^
                   (leftTangent.GetHashCode() << 2) ^ (rightTangent.GetHashCode() >> 2) ^
                   ((int) mode).GetHashCode() ^ height.GetHashCode() ^ spriteIndex.GetHashCode() ^
                   corner.GetHashCode();
        }
    }
    public class SpriteShape
    {
        
    }
}