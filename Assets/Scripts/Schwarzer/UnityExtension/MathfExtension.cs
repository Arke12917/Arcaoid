using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Schwarzer.UnityExtension
{
    public static class MathfExtension
    {
        public static float EvaluateEllipse(float degree, float a, float b)
        {
            float cos = Mathf.Cos(degree * Mathf.Deg2Rad);
            float sin = Mathf.Sin(degree * Mathf.Deg2Rad);
            return Mathf.Abs(a * b / Mathf.Sqrt(b * b * cos * cos + a * a * sin * sin));
        }
    }
}
