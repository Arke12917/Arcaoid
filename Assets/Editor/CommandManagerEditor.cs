using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Arcaoid.Compose.Command;

[CustomEditor(typeof(CommandManager))]
public class CommandManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Undo")) CommandManager.Instance.Undo();
        if (GUILayout.Button("Redo")) CommandManager.Instance.Redo();
        EditorGUILayout.EndHorizontal();
    }
}
