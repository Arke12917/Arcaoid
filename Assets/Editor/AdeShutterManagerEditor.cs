using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Arcaoid.Compose;

[CustomEditor(typeof(AdeShutterManager))]
public class AdeShutterManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!Application.isPlaying) return;
        GUILayout.Label("Editor");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Open"))
        {
            AdeShutterManager.Instance.Open();
        }
        if (GUILayout.Button("Close"))
        {
            AdeShutterManager.Instance.Close();
        }
        GUILayout.EndHorizontal();
    }
}
