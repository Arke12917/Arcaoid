using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Arcaoid.Compose;
using Arcaoid.Gameplay;

[CustomEditor(typeof(ArcSkinManager))]
public class AdeSkinManagerEditor : Editor
{
    private int[] values = new int[6];
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!Application.isPlaying) return;
        GUILayout.Label("Editor");

        GUILayout.BeginHorizontal();
        values[0] = EditorGUILayout.IntField(values[0]);
        if (GUILayout.Button("TapNote"))
        {
            ArcSkinManager.Instance.SetTapNoteSkin(values[0]);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        values[1] = EditorGUILayout.IntField(values[1]);
        if (GUILayout.Button("HoldNote"))
        {
            ArcSkinManager.Instance.SetHoldNoteSkin(values[1]);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        values[2] = EditorGUILayout.IntField(values[2]);
        if (GUILayout.Button("ArcTap"))
        {
            ArcSkinManager.Instance.SetArcTapSkin(values[2]);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        values[3] = EditorGUILayout.IntField(values[3]);
        if (GUILayout.Button("Track"))
        {
            ArcSkinManager.Instance.SetTrackSkin(values[3]);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        values[4] = EditorGUILayout.IntField(values[4]);
        if (GUILayout.Button("Critical Line"))
        {
            ArcSkinManager.Instance.SetCriticalLineSkin(values[4]);
        }
        GUILayout.EndHorizontal();
    }
}
