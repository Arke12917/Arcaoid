using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Arcaoid.Compose;

[CustomEditor(typeof(ArcaoidComposeManager))]
public class ArcaoidComposeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!Application.isPlaying) return;
        GUILayout.Label("Editor");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Play"))
        {
            ArcaoidComposeManager.Instance.Play();
        }
        if (GUILayout.Button("Pause"))
        {
            ArcaoidComposeManager.Instance.Pause();
        }
        GUILayout.EndHorizontal();
    }
}
