using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Arcaoid.Compose;

[CustomEditor(typeof(AdeToast))]
public class AdeToastEditor : Editor
{
    private string text;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        text = GUILayout.TextArea(text);
        if (GUILayout.Button("Send")) AdeToast.Instance.Show(text); 
    }
}
