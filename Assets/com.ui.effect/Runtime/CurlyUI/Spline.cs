using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [Serializable]
    public class Spline
    {
        private const string kErrorMessage = "Internal error: Point too close to neighbor";
        private const float kEpsilon = 0.01f;
        private List<SplineControlPoint> m_ControlPoints = new List<SplineControlPoint>();

        private bool IsPositionValid(int index, int next, Vector3 point)
        {
            int prev = (index == 0) ? (m_ControlPoints.Count - 1) : (index - 1);
            next = (next >= m_ControlPoints.Count) ? 0 : next;
            if (prev >= 0)
            {
                Vector3 diff = m_ControlPoints[prev].position - point;
                if (diff.magnitude < kEpsilon)
                    return false;
            }

            if (next < m_ControlPoints.Count)
            {
                Vector3 diff = m_ControlPoints[next].position - point;
                if (diff.magnitude < kEpsilon)
                {
                    return false;
                }
            }

            return true;
        }

        public int GetPointCount()
        {
            return m_ControlPoints.Count;
        }

        public void InsertPointAt(int index, Vector3 point)
        {
            if (!IsPositionValid(index,index,point))
            {
                throw new ArgumentException();
            }
            m_ControlPoints.Insert(index, new SplineControlPoint{position = point,height = 1.0f,corner = true});
        }

        public ShapeTangentMode GetTangentMode(int index)
        {
            return m_ControlPoints[index].mode;
        }

        public Vector3 GetLeftTangent(int index)
        {
            ShapeTangentMode mode = GetTangentMode(index);
            
            if (mode == ShapeTangentMode.Linear)
                return Vector3.zero;

            return m_ControlPoints[index].leftTangent;
        }
    }
}