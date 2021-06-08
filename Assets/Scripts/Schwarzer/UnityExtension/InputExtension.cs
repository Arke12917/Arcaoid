using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Schwarzer.UnityExtension
{
    public static class InputExtension
    {
        public static Vector2 scaledMousePosition
        {
            get
            {
                return ScaledMousePosition(new Vector2(1920, 1080));
            }
        }
        public static Vector2 ScaledMousePosition(Vector2 size)
        {
            float ratio = size.x / Screen.width;
            Vector2 mousePosition = Input.mousePosition;
            return mousePosition * ratio;
        }
    }
}