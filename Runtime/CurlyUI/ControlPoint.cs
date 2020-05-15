using System;

namespace UnityEngine.UI.Extensions
{
    public enum TangentMode
    {
        Linear = 0,
        Continuous = 1,
        Broken = 2
    }

    [Serializable]
    public struct TangentCache
    {
        public Vector2 leftTangent;
        public Vector2 rightTangent;
    }

    [Serializable]
    public struct ControlPoint
    {
        public Vector2 position;
        public Vector2 localLeftTangent;
        public Vector2 localRightTangent;
        public TangentMode tangentMode;
        public TangentCache continuousCache;
        public TangentCache brokenCache;
        public bool mirrorLeft;

        public Vector2 leftTangent
        {
            get { return localLeftTangent + position; }
            set { localLeftTangent = value - position; }
        }

        public Vector2 rightTangent
        {
            get { return localRightTangent + position; }
            set { localRightTangent = value - position; }
        }

        public void StoreTangent()
        {
            if (tangentMode == TangentMode.Continuous)
            {
                continuousCache.leftTangent = localLeftTangent;
                continuousCache.rightTangent = localRightTangent;
            }
            else if (tangentMode == TangentMode.Broken)
            {
                brokenCache.leftTangent = localLeftTangent;
                brokenCache.rightTangent = localRightTangent;
            }
        }

        public void RestoreTangents()
        {
            if (tangentMode == TangentMode.Continuous)
            {
                localLeftTangent = continuousCache.leftTangent;
                localRightTangent = continuousCache.rightTangent;
            }
            else if (tangentMode == TangentMode.Broken)
            {
                localLeftTangent = brokenCache.leftTangent;
                localRightTangent = brokenCache.rightTangent;
            }
        }
    }
}