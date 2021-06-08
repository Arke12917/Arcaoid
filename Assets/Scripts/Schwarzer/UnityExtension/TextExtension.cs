using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Schwarzer.UnityExtension
{
    public static class TextExtension
    {
        public static float CalculateHeight(this Text text, string str)
        {
            text.verticalOverflow = VerticalWrapMode.Overflow;
            var generator = text.cachedTextGenerator;
            float ratio = 1920f / Screen.width;
            return ratio * generator.GetPreferredHeight(str, text.GetGenerationSettings(text.rectTransform.rect.size));
        }
        public static float CalculateWidth(this Text text, string str)
        {
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            var generator = text.cachedTextGenerator;
            float ratio = 1920f / Screen.width;
            return ratio * generator.GetPreferredWidth(str, text.GetGenerationSettings(text.rectTransform.rect.size));
        }
    }
}