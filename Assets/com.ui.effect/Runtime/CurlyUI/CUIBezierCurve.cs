using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Effect
{
    [Serializable]
    public class CUIBezierCurve
    {
        public const int CubicBezierCurvePtNum = 3;

        [SerializeField]
        protected Vector2[] controlPoints;
        public AnimationCurve xCurve = new AnimationCurve();
        public AnimationCurve yCurve = new AnimationCurve();

        public Vector2[] ControlPoints
        {
            get
            {
                ReportSet();
                return controlPoints;
            }

        }
        public Vector2 GetPoint(float _time)
        {
            var point = Evaluate(_time);
            return point;
        }

        private Vector2 Evaluate(float t)
        {
            
            var length = controlPoints.Length;
            for (int i = xCurve.length - 1; i >= 0; i--)
            {
                xCurve.RemoveKey(i);
                yCurve.RemoveKey(i);
            }
            for (int i = 0; i < length; i++)
            {
                var time = (float)i / length;
                xCurve.AddKey(time, controlPoints[i].x);
                yCurve.AddKey(time, controlPoints[i].y);
            }
            return new Vector2(xCurve.Evaluate(t),yCurve.Evaluate(t));
        }

        public Vector3 GetTangent(float _time)
        {
            float oneMinusTime = 1 - _time;

            return 3 * oneMinusTime * oneMinusTime * (controlPoints[1] - controlPoints[0]) +
                6 * oneMinusTime * _time * (controlPoints[2] - controlPoints[1]) +
                3 * _time * _time * (controlPoints[3] - controlPoints[2]);
        }

        private void ReportSet()
        {
            if (controlPoints == null || controlPoints.Length != CubicBezierCurvePtNum)
            {
                controlPoints = new Vector2[CubicBezierCurvePtNum];
                var length = controlPoints.Length;
                for (int i = 0; i < length; i++)
                {
                    var time = (float)i / length;
                    xCurve.AddKey(time, controlPoints[i].x);
                    yCurve.AddKey(time, controlPoints[i].y);
                }    
            }
        }
    }
}