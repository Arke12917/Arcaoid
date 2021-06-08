using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Movinarc
{
    public class TreeView : EditorWindow
    {
        public TreeNode treenode;
        public TreeNode treeroot;
        public List<TreeNode> selectedNodes;

        public class NodePath
        {
            public TreeNode parent;
            public string path;
            public string name;
            public bool isDirectory;
        }

        public TreeView()
        {
            treenode = new TreeNode();
            selectedNodes = new List<TreeNode>();
            insertRoot();
        }

        public bool allCheckedByDefault = true;
        public bool allExpandedByDefault = true;

        /// <summary>
        /// Adds the node from path.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name = "dirList"></param>
        public void AddNodeFromPath(string path, List<string> dirList, List<string> foundList)
        {
            var ph = PathHierarchy(path);
            for (int i = 0; i < ph.Count; i++)
            {
                var p = ph[i];
                if (!treenode.PathExists(p.path))
                {
                    TreeNode pathParent = treenode.GetParentNodeInPath(p.path, treeroot);
                    TreeNode node = new TreeNode();
                    node.parent = pathParent;
                    node.name = p.name;
                    node.path = TreeNode.iHateSlashes(p.path);
                    node.isDirectory = dirList.Exists((d) => d.Equals(node.path, StringComparison.OrdinalIgnoreCase));
                    node.isChecked = foundList.Exists(f => f.Equals(node.path, StringComparison.OrdinalIgnoreCase)) && allCheckedByDefault;
                    node.lastCheckedState = node.isChecked;
                    if (node.isChecked)
                        selectedNodes.Add(node);
                    node.isExpanded = allExpandedByDefault;
                    pathParent.AddNode(node);
                }
            }
        }

        public void ParentalCheck()
        {
            foreach (var node in selectedNodes)
            {
                doParentalCheck(node);
            }
        }

        private void doParentalCheck(TreeNode node)
        {
            if (node.isDirectory && node.children.Count == 0)
            {
                node.isChecked = true;
                node.lastCheckedState = true;
            }
            if (node.isChecked && node.parent != null)
            {
                node.parent.isChecked = true;
                node.parent.lastCheckedState = true;
                doParentalCheck(node.parent);
            }
        }

        public void EmptyFolderCheck(List<string> dirs)
        {
            var all = AssetDatabase.GetAllAssetPaths().ToList();
            var dirGuids = new List<string>();
            dirs.ForEach(d => dirGuids.Add(AssetDatabase.AssetPathToGUID(d)));
            foreach (var node in treeroot.children)
            {
                doEmptyFolderCheck(node, dirGuids, all);
            }
        }

        private void doEmptyFolderCheck(TreeNode node, List<string> dirs, List<string> all)
        {
            if (node.isDirectory)
            {

                //UnityEditor
                var found = AssetDatabase.FindAssets("", new[] { node.path }).ToList();
                PUSelection.ClearWarnings();
                if (!found.Except(dirs).Any() && all.Exists(a => a.Equals(node.path, StringComparison.OrdinalIgnoreCase)))
                {
                    node.isChecked = true;
                    node.lastCheckedState = true;
                }
                foreach (var child in node.children)
                {
                    doEmptyFolderCheck(child, dirs, all);
                }
            }
        }

        public void populateTree(TreeNode node, ref Rect position, Texture2D foldIcon = null, Texture2D fileIcon = null)
        {
            if (!node.isRoot)
            {
                var content = new GUIContent();
                content.text = node.name;
                if (node != null && node.path != null)
                    position.x = 5 + 15 * node.path.Split('/').Length;
                if (node.isChecked)
                {

                }
                if (node.isDirectory)
                {
                    position.width = 15;
                    node.isExpanded = EditorGUI.Foldout(position, node.isExpanded, "");
                    position.x += 15;
                    position.width = 500;
                    node.isChecked = GUI.Toggle(position, node.isChecked, foldIcon);
                    position.x += 15;


                }
                else //is file
                {
                    position.x += 15;
                    node.isChecked = GUI.Toggle(position, node.isChecked, fileIcon);
                    position.x += 15;
                }
                position.width = 500;
                position.x += 15;
                var lblStyle = EditorStyles.label;
                if (node.isChecked)
                {
                    lblStyle = EditorStyles.boldLabel;
                    if (!selectedNodes.Contains(node))
                    {
                        selectedNodes.Add(node);
                    }
                }
                else
                {
                    if (selectedNodes.Contains(node))
                    {
                        selectedNodes.Remove(node);
                    }
                }
                GUI.Label(position, node.name, lblStyle);
                if (node.lastCheckedState != node.isChecked)
                {
                    node.lastCheckedState = node.isChecked;
                    checkChildren(node);
                }
                position.y += position.height;
            }

            if (node.isExpanded)
                foreach (var child in node.children)
                {
                    position.x += 15;
                    populateTree(child, ref position, foldIcon, fileIcon);
                }

        }

        void checkChildren(TreeNode node)
        {
            foreach (var child in node.children)
            {
                child.isChecked = node.isChecked;
                if (child.children != null && child.children.Count > 0)
                    checkChildren(child);
            }
        }

        public void checkAll(TreeNode node, bool all)
        {
            foreach (var item in node.children)
            {
                item.isChecked = all;
                item.lastCheckedState = !all;
                if (item.children != null && item.children.Count > 0)
                    checkChildren(item);
            }
        }

        public void checkNone(TreeNode node)
        {
            checkAll(node, false);
        }

        /// <summary>
        /// Paths the hierarchy.
        /// in:
        /// Assets/1/2/3 
        ///
        /// out: 
        /// Assets
        /// Assets/1
        /// Assets/1/2
        /// Assets/1/2/3
        /// </summary>
        /// <param name="path">Path.</param>
        public static List<NodePath> PathHierarchy(string path)
        {
            var ph = getPathHierarchy(path, new List<NodePath>());
            ph.Reverse();
            return ph;
        }

        static List<NodePath> getPathHierarchy(string path, List<NodePath> lst)
        {
            List<string> pl = new List<string>(path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
            if (pl.Count > 0)
            {
                lst.Add(new NodePath() { path = path, name = pl[pl.Count - 1] });
                pl.RemoveAt(pl.Count - 1);
                string joined = TreeNode.listToPath(pl);
                getPathHierarchy(joined, lst);
            }
            return lst;
        }

        private void insertRoot()
        {
            TreeNode root = new TreeNode();
            root.name = "";
            root.parent = null;
            root.isRoot = true;
            root.root = null;
            root.isExpanded = true;
            root.isChecked = true;
            treenode.AddNode(root);
            treeroot = root;
            treenode.root = treeroot;
        }
    }

}