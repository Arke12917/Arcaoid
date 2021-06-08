using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Schwarzer.UnityExtension
{
    public static class CameraExtension
    {
        public static Ray ScreenPointToRay(this Camera camera)
        {
            return camera.ScreenPointToRay(Input.mousePosition);
        }
    }
}