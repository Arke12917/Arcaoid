﻿using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arcaoid.Compose.Cursor.Builder
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
    public class VerticalPanelMeshBuilder : MonoBehaviour
    {
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;
        private void Start()
        {
            meshCollider = GetComponent<MeshCollider>();
            meshFilter = GetComponent<MeshFilter>();

            Vector3[] vertices = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] triangles = new int[6];

            vertices[0] = new Vector3(-8.5f, 0, 0);
            uv[0] = new Vector2();
            vertices[1] = new Vector3(8.5f, 0, 0);
            uv[1] = new Vector2(1, 0);
            vertices[2] = new Vector3(8.5f, 5.5f, 0);
            uv[2] = new Vector2(1, 1);
            vertices[3] = new Vector3(-8.5f, 5.5f, 0);
            uv[3] = new Vector2(0, 1);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            Mesh mesh = new Mesh()
            {
                vertices = vertices,
                uv = uv,
                triangles = triangles
            };

            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
    }
}
