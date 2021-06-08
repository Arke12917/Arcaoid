using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Schwarzer.UnityExtension
{
    public static class CollectionExtension
    {
        public static bool OutOfRange(this ICollection collection, int index)
        {
            return index < 0 || index > collection.Count - 1;
        }
    }
}